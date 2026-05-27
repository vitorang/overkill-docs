import {
    computed,
    inject,
    Injectable,
    Injector,
    runInInjectionContext,
    Signal,
    signal,
} from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { API } from '@core/constants/api.constants';
import { AuthService } from '@core/services/auth.service';
import { ChatHub } from '@features/chat/hubs/chat.hub';
import { DocumentIndexHub } from '@features/document/hubs/document-index.hub';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';
import * as signalR from '@microsoft/signalr';
import { filter, Subject } from 'rxjs';

export interface ResponseListener {
    name: string;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    listener: Subject<any>;
}

export interface IRawMessage {
    method: string;
    data: unknown;
}

export interface IHubState {
    connected: Signal<boolean>;
    disconnected: Signal<boolean>;
    connecting: Signal<boolean>;
    current: Signal<HubState>;
}

export type HubState = 'DISCONNECTED' | 'CONNECTED' | 'CONNECTING';

export interface IMainHub {
    state: IHubState;
    onReceived: Subject<IRawMessage>;
    onSended: Subject<IRawMessage>;
    send<T>(method: string, data?: T): Promise<void>;
}

export interface IHub {
    responseListeners: ResponseListener[];
    state: IHubState;
}

@Injectable({ providedIn: 'root' })
export class MainHub {
    private connectionState = signal<HubState>('DISCONNECTED');
    private injector = inject(Injector);

    state = {
        connected: computed(() => this.connectionState() === 'CONNECTED'),
        connecting: computed(() => this.connectionState() === 'CONNECTING'),
        disconnected: computed(() => this.connectionState() === 'DISCONNECTED'),
        current: this.connectionState.asReadonly(),
    };

    public get hubs(): IHub[] {
        let value: IHub[] = [];

        runInInjectionContext(this.injector, () => {
            value = [inject(ChatHub), inject(DocumentIndexHub), inject(DocumentViewerHub)];
        });

        return value;
    }

    onReceived = new Subject<IRawMessage>();
    onSended = new Subject<IRawMessage>();

    private hubConnection: signalR.HubConnection | null = null;
    private listenerNames = new Set<string>();
    private authService = inject(AuthService);

    constructor() {
        toObservable(this.authService.token)
            .pipe(
                takeUntilDestroyed(),
                filter((token) => !token),
            )
            .subscribe(() => {
                this.disposeConnection();
            });
    }

    get mainHub(): IMainHub {
        return {
            state: this.state,
            onReceived: this.onReceived,
            onSended: this.onSended,
            send: this.send,
        };
    }

    connect = (): void => {
        if (!this.state.disconnected()) {
            return;
        }

        if (!this.hubConnection) {
            this.hubConnection = new signalR.HubConnectionBuilder()
                .withUrl(API.HUB.MAIN(this.authService.token()!))
                .withAutomaticReconnect()
                .build();

            this.hubConnection.onreconnecting(() => this.connectionState.set('CONNECTING'));
            this.hubConnection.onreconnected(() => this.connectionState.set('CONNECTED'));
            this.hubConnection.onclose((error) => {
                if (error) {
                    console.error('Hub:', error);
                }

                this.connectionState.set('DISCONNECTED');
            });
            this.connectionState.set('CONNECTING');

            this.registerListeners();
        }

        this.hubConnection
            .start()
            .then(() => this.connectionState.set('CONNECTED'))
            .catch((error) => {
                console.error('Hub:', error);
                this.connectionState.set('DISCONNECTED');
            });
    };

    disconnect = (): void => {
        this.hubConnection?.stop()?.then();
    };

    private send = <T>(method: string, data: T): Promise<void> => {
        if (!this.hubConnection) {
            throw 'Conexão indefinida.';
        }

        this.onSended.next({ method, data });
        if (data === undefined) {
            return this.hubConnection.invoke(method);
        }

        return this.hubConnection.invoke(method, data);
    };

    private registerListeners = () => {
        const addListener = (event: ResponseListener) => {
            if (this.listenerNames.has(event.name)) {
                throw `Hub: ${event.listener} já foi registrado`;
            }
            if (!this.hubConnection) {
                throw 'Conexão indefinida.';
            }

            this.listenerNames.add(event.name);
            this.hubConnection.on(event.name, (data: unknown) => {
                event.listener.next(data);
                this.onReceived.next({ method: event.name, data });
            });
        };

        this.hubs.forEach((hub) =>
            hub.responseListeners.forEach((listener) => addListener(listener)),
        );
    };

    private disposeConnection = () => {
        this.listenerNames.forEach((name) => this.hubConnection?.off(name));
        this.listenerNames.clear();
        this.disconnect();
        this.hubConnection = null;
    };
}

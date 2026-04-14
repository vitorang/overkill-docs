import { computed, inject, Injectable, Signal, signal } from "@angular/core";
import * as signalR from '@microsoft/signalr';
import { defer, filter, finalize, Observable, shareReplay, Subject } from "rxjs";
import { API } from "../../constants/api.constants";
import { AuthService } from "../auth.service";
import { ChatHubService } from "./chat-hub.service";
import { takeUntilDestroyed, toObservable } from "@angular/core/rxjs-interop";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export interface ResponseListener { name: string, listener: Subject<any> };

export interface IRawMessage {
    method: string
    data: unknown
}

export interface IHubState {
    connected: Signal<boolean>,
    disconnected: Signal<boolean>,
    connecting: Signal<boolean>,
    current: Signal<HubState>;
}

export type HubState = 'DISCONNECTED' | 'CONNECTED' | 'CONNECTING';

export interface IMainHub {
    connection: Observable<unknown>
    state: IHubState,
    onReceived: Subject<IRawMessage>
    onSended: Subject<IRawMessage>
    send<T>(method: string, data?: T): Promise<void>
    forceDisconnect(): void
    forceConnect(): void
}

@Injectable({ providedIn: 'root' })
export class HubService {
    private connectionState = signal<HubState>('DISCONNECTED');
    private state = {
        connected: computed(() => this.connectionState() === 'CONNECTED'),
        connecting: computed(() => this.connectionState() === 'CONNECTING'),
        disconnected: computed(() => this.connectionState() === 'DISCONNECTED'),
        current: this.connectionState.asReadonly(),
    };

    private onReceived = new Subject<IRawMessage>();
    private onSended = new Subject<IRawMessage>();

    private hubConnection: signalR.HubConnection | null = null;
    private listenerNames = new Set<string>();
    private authService = inject(AuthService);

    constructor() {
        toObservable(this.authService.token).pipe(
            takeUntilDestroyed(),
            filter(token => !token)
        ).subscribe(() => {
            this.disposeConnection();
        });
    }

    get mainHub(): IMainHub {
        return {
            connection: this.connection,
            state: this.state,
            onReceived: this.onReceived,
            onSended: this.onSended,
            send: this.send,
            forceConnect: this.connect,
            forceDisconnect: this.disconnect,
        }
    }

    private connection = defer(() => {
        this.connect();
        return new Observable(subscriber => subscriber.next(this.hubConnection));
    }).pipe(
        shareReplay({ bufferSize: 1, refCount: true }),
        finalize(() => this.disconnect()),
    );

    private connect = (): void => {
        if (!this.state.disconnected())
            return;

        if (!this.hubConnection) {
            this.hubConnection = new signalR.HubConnectionBuilder()
                .withUrl(API.HUB.MAIN(this.authService.token()!))
                .withAutomaticReconnect()
                .build();

            this.hubConnection.onreconnecting(() => this.connectionState.set('CONNECTING'));
            this.hubConnection.onreconnected(() => this.connectionState.set('CONNECTED'));
            this.hubConnection.onclose((error) => {
                if (error)
                    console.error('Hub:', error);

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
                this.connectionState.set('DISCONNECTED')
            });
    }

    private disconnect = (): void => {
        this.hubConnection?.stop()?.then();
    }

    private send = <T>(method: string, data: T): Promise<void> => {
        if (!this.hubConnection)
            throw 'Conexão indefinida.';

        this.onSended.next({ method, data });
        if (data === undefined)
            return this.hubConnection.invoke(method);

        return this.hubConnection.invoke(method, data);
    }

    private registerListeners = () => {
        const addListener = (event: ResponseListener) => {
            if (this.listenerNames.has(event.name))
                throw `Hub: ${event.listener} já foi registrado`;
            if (!this.hubConnection)
                throw 'Conexão indefinida.';

            this.listenerNames.add(event.name);
            this.hubConnection.on(event.name, (data: unknown) => {
                event.listener.next(data);
                this.onReceived.next({ method: event.name, data })
            });
        }

        [
            ...inject(ChatHubService).responseListeners
        ].forEach(listener => addListener(listener));
    }

    private disposeConnection = () => {
        this.listenerNames.forEach(name => this.hubConnection?.off(name));
        this.listenerNames.clear();
        this.disconnect();
        this.hubConnection = null;
    }
}


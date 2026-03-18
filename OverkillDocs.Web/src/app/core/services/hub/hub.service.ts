import { computed, inject, Injectable, Signal, signal } from "@angular/core";
import * as signalR from '@microsoft/signalr';
import { defer, finalize, Observable, shareReplay, Subject } from "rxjs";
import { API } from "../../constants/api.constants";
import { HubState } from "../../models/common.model";
import { AuthService } from "../auth.service";
import { ChatHubService } from "./chat-hub.service";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export interface ResponseListener { name: string, listener: Subject<any> };

export interface IRawMessage {
    method: string
    data: unknown
}

export interface IMainHub {
    connection: Observable<unknown>
    connectionState: Signal<HubState>
    isConnected: Signal<boolean>
    onReceived: Subject<IRawMessage>
    onSended: Subject<IRawMessage>
    send<T>(method: string, data?: T): Promise<void>
    forceDisconnect(): void
    forceConnect(): void
}

@Injectable({ providedIn: 'root' })
export class HubService {
    private connectionState = signal<HubState>(HubState.DISCONNECTED);
    private isConnected = computed(() => this.connectionState() === HubState.CONNECTED);

    private onReceived = new Subject<IRawMessage>();
    private onSended = new Subject<IRawMessage>();

    private hubConnection!: signalR.HubConnection;
    private listenerNames = new Set<string>();
    private authService = inject(AuthService);

    get mainHub(): IMainHub {
        return {
            connection: this.connection,
            connectionState: this.connectionState,
            isConnected: this.isConnected,
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
        if (this.connectionState() !== HubState.DISCONNECTED)
            return;

        if (!this.hubConnection) {
            this.hubConnection = new signalR.HubConnectionBuilder()
                .withUrl(API.HUB.MAIN(this.authService.token()!))
                .withAutomaticReconnect()
                .build();

            this.hubConnection.onreconnecting(() => this.connectionState.set(HubState.CONNECTING));
            this.hubConnection.onreconnected(() => this.connectionState.set(HubState.CONNECTED));
            this.hubConnection.onclose((error) => {
                if (error)
                    console.error('Hub:', error);

                this.connectionState.set(HubState.DISCONNECTED);
            });
            this.connectionState.set(HubState.CONNECTING);

            this.registerListeners();
        }

        this.hubConnection
            .start()
            .then(() => this.connectionState.set(HubState.CONNECTED))
            .catch((error) => {
                console.error('Hub:', error);
                this.connectionState.set(HubState.DISCONNECTED)
            });
    }

    private disconnect = (): void => {
        this.hubConnection?.stop()?.then();
    }

    private send = <T>(method: string, data: T): Promise<void> => {
        this.onSended.next({ method, data });
        if (data === undefined)
            return this.hubConnection.invoke(method);

        return this.hubConnection.invoke(method, data);
    }

    private registerListeners = () => {
        const addListener = (event: ResponseListener) => {
            if (this.listenerNames.has(event.name))
                throw `Hub: ${event.listener} já foi registrado`;

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
}


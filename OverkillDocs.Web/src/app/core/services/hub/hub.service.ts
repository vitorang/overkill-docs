import { computed, inject, Injectable, signal } from "@angular/core";
import * as signalR from '@microsoft/signalr';
import { defer, finalize, Observable, shareReplay, Subject } from "rxjs";
import { API } from "../../constants/api.constants";
import { HubState } from "../../models/common.model";
import { AuthService } from "../auth.service";
import { ChatHubService } from "./chat-hub.service";
import { IMainHub, ResponseListener } from "./base-hub.service";
import { DebugHubService } from "./debug-hub.service";

export interface IRawMessage {
    method: string
    data: unknown
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

    get debugHub(): DebugHubService { return new DebugHubService(this.hubParams) };
    get chatHub(): ChatHubService { return new ChatHubService(this.hubParams); }

    private get hubParams(): IMainHub {
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
        console.log(this.hubConnection);
        this.hubConnection?.stop()?.then();
    }

    private send = <T>(method: string, data: T): Promise<void> => {
        this.onSended.next({ method, data });
        if (data === undefined)
            return this.hubConnection.invoke(method);

        return this.hubConnection.invoke(method, data);
    }

    private registerListeners = () => {
        const addListener = <T>(event: ResponseListener) => {
            if (this.listenerNames.has(event.name))
                throw `Hub: ${event.listener} já foi registrado`;

            this.listenerNames.add(event.name);
            this.hubConnection.on(event.name, (data: T) => event.listener.next(data));
            this.hubConnection.on(event.name, (data: T) => this.onReceived.next({ method: event.name, data }));
        }

        [
            ...this.chatHub.responseListeners
        ].forEach(listener => addListener(listener));
    }
}


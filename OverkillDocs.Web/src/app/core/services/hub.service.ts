import { computed, inject, Injectable, signal } from "@angular/core";
import * as signalR from '@microsoft/signalr';
import { defer, finalize, Observable, shareReplay, Subject, tap } from "rxjs";
import { API } from "../constants/api.constants";
import { HubState } from "../models/common.model";
import { AuthService } from "./auth.service";
import { ChatMessage } from "../../features/chat/models/chat-message.model";
import { toObservable } from "@angular/core/rxjs-interop";

export interface IRawMessage {
    method: string
    data: unknown
}

@Injectable({ providedIn: 'root' })
export class HubService {
    readonly connectionState = signal<HubState>(HubState.DISCONNECTED);
    readonly isConnected = computed(() => this.connectionState() === HubState.CONNECTED);

    readonly onReceived = new Subject<IRawMessage>();
    readonly onSended = new Subject<IRawMessage>();
    readonly chatHub = new ChatHub(this);

    private hubConnection!: signalR.HubConnection;
    private listenerNames = new Set<string>();
    private authService = inject(AuthService);

    readonly connection = defer(() => {
        this.connect();
        return new Observable(subscriber => subscriber.next(this.hubConnection));
    }).pipe(
        shareReplay({ bufferSize: 1, refCount: true }),
        finalize(() => this.disconnect()),
    );

    private connect(): void {
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

    private disconnect(): void {
        this.hubConnection?.stop();
    }

    send<T>(method: string, data: T): Promise<void> {
        this.onSended.next({ method, data });
        return this.hubConnection.invoke(method, data);
    }

    private addListener<T>(eventName: string, listener: Subject<T>) {
        if (this.listenerNames.has(eventName))
            throw `Hub: ${eventName} já foi registrado`;

        this.listenerNames.add(eventName);
        this.hubConnection.on(eventName, (data: T) => listener.next(data));
        this.hubConnection.on(eventName, (data: T) => this.onReceived.next({ method: eventName, data }));
    }

    private registerListeners() {
        this.addListener('OnMessageReceived', this.chatHub.onMessageReceived);
    }
}

class ChatHub {
    constructor(private mainHub: HubService) { };

    readonly onMessageReceived = new Subject<ChatMessage>();
    readonly sendMessage = (text: string) => this.mainHub.send('SendMessage', text);

    get isConnected() { return this.mainHub.isConnected; }
    get connection() { return this.mainHub.connection; }
}
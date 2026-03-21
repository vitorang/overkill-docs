import { distinctUntilChanged, map, Observable, Subject, switchMap } from "rxjs";
import { ChatMessage } from "../../../features/chat/models/chat-message.model";
import { takeUntilDestroyed, toObservable } from "@angular/core/rxjs-interop";
import { inject, Injectable, Signal } from "@angular/core";
import { HubService, ResponseListener } from "./hub.service";

const Hub = {
    join: 'Chat:Join',
    requestRecentMessages: 'Chat:RequestRecentMessages',
    sendMessage: 'Chat:SendMessage',

    onMessageReceived: 'Chat:OnMessageReceived',
    onRecentMessagesReceived: "Chat:OnRecentMessagesReceived",
} as const


@Injectable({ providedIn: 'root' })
export class ChatHubService {
    private mainHub = inject(HubService).mainHub;

    private _onMessageReceived = new Subject<ChatMessage>();
    readonly onMessageReceived = this._onMessageReceived.pipe(
        map(message => this.mapMessage(message))
    );

    private _onRecentMessagesReceived = new Subject<ChatMessage[]>();
    readonly onRecentMessagesReceived = this._onRecentMessagesReceived.pipe(
        map(messages => messages.map(message => this.mapMessage(message)))
    );

    readonly sendMessage = (text: string): Promise<void> => this.mainHub.send(Hub.sendMessage, text);

    get connection(): Observable<boolean> {
        return this.mainHub.connection.pipe(
            takeUntilDestroyed(),
            switchMap(() => toObservable(this.mainHub.isConnected)),
            distinctUntilChanged()
        );
    }

    readonly join = (): Promise<void> => this.mainHub.send(Hub.join);

    readonly requestRecentMessages = (): Promise<void> => this.mainHub.send(Hub.requestRecentMessages);

    get responseListeners(): ResponseListener[] {
        return [
            { name: Hub.onMessageReceived, listener: this._onMessageReceived },
            { name: Hub.onRecentMessagesReceived, listener: this._onRecentMessagesReceived }
        ];
    }

    get isConnected(): Signal<boolean> {
        return this.mainHub.isConnected;
    }

    private mapMessage(message: ChatMessage): ChatMessage {
        return { ...message, timestamp: new Date(message.timestamp as unknown as string) };
    }
}
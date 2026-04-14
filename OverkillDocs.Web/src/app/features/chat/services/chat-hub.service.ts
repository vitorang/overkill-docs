import { distinctUntilChanged, map, Observable, Subject, switchMap } from "rxjs";
import { takeUntilDestroyed, toObservable } from "@angular/core/rxjs-interop";
import { inject, Injectable } from "@angular/core";
import { HubService, IHubState, ResponseListener } from "@core/services/hub.service";
import { ChatMessage } from "@features/chat/models/chat-message.model";

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
            switchMap(() => toObservable(this.state.connected)),
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

    get state(): IHubState {
        return this.mainHub.state;
    }

    private mapMessage(message: ChatMessage): ChatMessage {
        return { ...message, timestamp: new Date(message.timestamp as unknown as string) };
    }
}
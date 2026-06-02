import { inject, Injectable } from '@angular/core';
import { IHub, IHubState, MainHub, ResponseListener } from '@core/hubs/main.hub';
import { ChatMessage } from '@features/chat/chat.models';
import { map, Subject } from 'rxjs';

const group = 'Chat';
const Hub = {
    join: `${group}:Join`,
    requestRecentMessages: `${group}:RequestRecentMessages`,
    sendMessage: `${group}:SendMessage`,

    onMessageReceived: `${group}:OnMessageReceived`,
    onRecentMessagesReceived: `${group}:OnRecentMessagesReceived`,
} as const;

@Injectable({ providedIn: 'root' })
export class ChatHub implements IHub {
    private mainHub = inject(MainHub).mainHub;

    private _onMessageReceived = new Subject<ChatMessage>();
    readonly onMessageReceived = this._onMessageReceived.pipe(
        map((message) => this.mapMessage(message)),
    );

    private _onRecentMessagesReceived = new Subject<ChatMessage[]>();
    readonly onRecentMessagesReceived = this._onRecentMessagesReceived.pipe(
        map((messages) => messages.map((message) => this.mapMessage(message))),
    );

    readonly sendMessage = (text: string): Promise<void> =>
        this.mainHub.send(Hub.sendMessage, text);

    readonly join = (): Promise<void> => this.mainHub.send(Hub.join);

    readonly requestRecentMessages = (): Promise<void> =>
        this.mainHub.send(Hub.requestRecentMessages);

    responseListeners: ResponseListener[] = [
        { name: Hub.onMessageReceived, listener: this._onMessageReceived },
        { name: Hub.onRecentMessagesReceived, listener: this._onRecentMessagesReceived },
    ];

    get state(): IHubState {
        return this.mainHub.state;
    }

    private mapMessage(message: ChatMessage): ChatMessage {
        return { ...message, timestamp: new Date(message.timestamp as unknown as string) };
    }
}

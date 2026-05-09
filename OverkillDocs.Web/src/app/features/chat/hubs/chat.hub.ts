import { distinctUntilChanged, map, Observable, Subject, switchMap } from 'rxjs';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { inject, Injectable } from '@angular/core';
import { MainHub, IHubState, ResponseListener } from '@core/hubs/main.hub';
import { ChatMessage } from '@features/chat/chat.models';

const group = 'Chat';
const Hub = {
    join: `${group}:Join`,
    requestRecentMessages: `${group}:RequestRecentMessages`,
    sendMessage: `${group}:SendMessage`,

    onMessageReceived: `${group}:OnMessageReceived`,
    onRecentMessagesReceived: `${group}:OnRecentMessagesReceived`,
} as const;

@Injectable({ providedIn: 'root' })
export class ChatHub {
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

    get connection(): Observable<boolean> {
        return this.mainHub.connection.pipe(
            takeUntilDestroyed(),
            switchMap(() => toObservable(this.state.connected)),
            distinctUntilChanged(),
        );
    }

    readonly join = (): Promise<void> => this.mainHub.send(Hub.join);

    readonly requestRecentMessages = (): Promise<void> =>
        this.mainHub.send(Hub.requestRecentMessages);

    get responseListeners(): ResponseListener[] {
        return [
            { name: Hub.onMessageReceived, listener: this._onMessageReceived },
            { name: Hub.onRecentMessagesReceived, listener: this._onRecentMessagesReceived },
        ];
    }

    get state(): IHubState {
        return this.mainHub.state;
    }

    private mapMessage(message: ChatMessage): ChatMessage {
        return { ...message, timestamp: new Date(message.timestamp as unknown as string) };
    }
}

import { distinctUntilChanged, Observable, Subject, switchMap } from "rxjs";
import { BaseHubService, ResponseListener } from "./base-hub.service";
import { ChatMessage } from "../../../features/chat/models/chat-message.model";
import { takeUntilDestroyed, toObservable } from "@angular/core/rxjs-interop";

export class ChatHubService extends BaseHubService {
    readonly onMessageReceived = new Subject<ChatMessage>();
    readonly sendMessage = (text: string): Promise<void> => this.mainHub.send('Chat:SendMessage', text);

    override get connection(): Observable<boolean> {
        return this.mainHub.connection.pipe(
            takeUntilDestroyed(),
            switchMap(() => toObservable(this.mainHub.isConnected)),
            distinctUntilChanged()
        );
    }

    readonly join = (): Promise<void> => this.mainHub.send('Chat:Join');
    override get responseListeners(): ResponseListener[] {
        return [{ name: 'Chat:OnMessageReceived', listener: this.onMessageReceived }]
    }
}
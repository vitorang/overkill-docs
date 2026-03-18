import { distinctUntilChanged, Observable, Subject, switchMap } from "rxjs";
import { ChatMessage } from "../../../features/chat/models/chat-message.model";
import { takeUntilDestroyed, toObservable } from "@angular/core/rxjs-interop";
import { inject, Injectable, Signal } from "@angular/core";
import { HubService, ResponseListener } from "./hub.service";

@Injectable({ providedIn: 'root' })
export class ChatHubService {
    private mainHub = inject(HubService).mainHub;

    readonly onMessageReceived = new Subject<ChatMessage>();
    readonly sendMessage = (text: string): Promise<void> => this.mainHub.send('Chat:SendMessage', text);

    get connection(): Observable<boolean> {
        return this.mainHub.connection.pipe(
            takeUntilDestroyed(),
            switchMap(() => toObservable(this.mainHub.isConnected)),
            distinctUntilChanged()
        );
    }

    readonly join = (): Promise<void> => this.mainHub.send('Chat:Join');
    get responseListeners(): ResponseListener[] {
        return [{ name: 'Chat:OnMessageReceived', listener: this.onMessageReceived }];
    }

    get isConnected(): Signal<boolean> {
        return this.mainHub.isConnected;
    }
}
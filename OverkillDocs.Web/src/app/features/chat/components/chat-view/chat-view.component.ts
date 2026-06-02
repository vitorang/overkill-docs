import { Component, inject, signal } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { ChatViewportComponent } from '@features/chat/components/chat-viewport/chat-viewport.component';
import { ChatHub } from '@features/chat/hubs/chat.hub';
import { SHARED } from '@shared/index';

import { filter } from 'rxjs';

@Component({
    selector: 'okd-chat-view',
    imports: [SHARED, ChatViewportComponent],
    templateUrl: './chat-view.component.html',
    styleUrl: './chat-view.component.scss',
})
export class ChatViewComponent {
    protected maxLength = 250;
    protected message = signal('');
    private chatHub = inject(ChatHub);
    protected connected = this.chatHub.state.connected;

    constructor() {
        toObservable(this.chatHub.state.connected)
            .pipe(filter((connected) => connected))
            .subscribe(() => {
                this.chatHub.join();
                this.chatHub.requestRecentMessages();
            });
    }

    protected onPressEnter(event: Event): void {
        const keyboardEvent = event as KeyboardEvent;
        if (keyboardEvent.shiftKey) {
            return;
        }

        event.preventDefault();
        this.sendMessage();
    }

    protected async sendMessage(): Promise<void> {
        const text = this.message().trim();

        if (text && this.chatHub.state.connected()) {
            await this.chatHub.sendMessage(text);
            this.message.set('');
        }
    }
}

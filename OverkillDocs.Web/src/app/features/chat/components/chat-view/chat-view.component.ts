import { Component, inject, signal } from '@angular/core';
import { SHARED_NATIVE } from '../../../../shared';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ChatViewportComponent } from '../chat-viewport/chat-viewport.component';
import { ChatHubService } from '../../../../core/services/hub/chat-hub.service';

@Component({
    selector: 'okd-chat-view',
    imports: [SHARED_NATIVE, ChatViewportComponent],
    templateUrl: './chat-view.component.html',
    styleUrl: './chat-view.component.scss',
})
export class ChatViewComponent {
    protected maxLength = 250;
    protected message = signal('');
    private chatHub = inject(ChatHubService);

    constructor() {
        this.chatHub.connection.pipe(takeUntilDestroyed()).subscribe((connected) => {
            if (connected)
                this.chatHub.join();
        });
    }

    protected onPressEnter(event: Event): void {
        const keyboardEvent = event as KeyboardEvent;
        if (keyboardEvent.shiftKey)
            return;

        event.preventDefault();
        this.sendMessage();
    }

    protected async sendMessage(): Promise<void> {
        const text = this.message().trim();

        if (text && this.chatHub.isConnected()) {
            await this.chatHub.sendMessage(text);
            this.message.set('');
        }
    }
}

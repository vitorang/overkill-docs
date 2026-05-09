import { Component, computed, effect, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { ChatHub } from '@features/chat/hubs/chat.hub';
import { DocumentLayoutService } from '@features/document/services/document-layout.services';
import { SHARED } from '@shared/index';
import { filter } from 'rxjs';

@Component({
    selector: 'okd-toggle-chat-button',
    imports: [SHARED],
    templateUrl: './toggle-chat-button.component.html',
    styleUrl: './toggle-chat-button.component.scss',
})
export class ToggleChatButtonComponent {
    private documentLayoutService = inject(DocumentLayoutService);
    private chatHub = inject(ChatHub);
    protected hasUnreadMessage = signal(false);

    protected activeSection = this.documentLayoutService.activeSection.asReadonly();
    protected isMobile = this.documentLayoutService.isMobile.asReadonly();
    protected toggleSection = this.documentLayoutService.toggleSection;

    constructor() {
        this.chatHub.onMessageReceived.pipe(takeUntilDestroyed()).subscribe(() => {
            this.hasUnreadMessage.set(
                this.isMobile() && this.documentLayoutService.activeSection() !== 'chat',
            );
        });

        effect(() => {
            if (!this.isMobile() || this.activeSection() === 'chat')
                this.hasUnreadMessage.set(false);
        });
    }
}

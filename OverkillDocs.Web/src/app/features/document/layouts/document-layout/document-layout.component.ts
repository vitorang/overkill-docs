import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SHARED_CUSTOM, SHARED_NATIVE } from '../../../../shared';
import { BreakpointObserver } from '@angular/cdk/layout';
import { BreakpointQueries } from '../../../../shared/constants/breakpoints.constant';
import { ChatViewComponent } from '../../../chat/components/chat-view/chat-view.component';
import { ChatHubService } from '../../../../core/services/hub/chat-hub.service';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { filter } from 'rxjs';

type TabSection = 'editor' | 'chat';

@Component({
    selector: 'okd-document-layout',
    imports: [SHARED_NATIVE, SHARED_CUSTOM, ChatViewComponent],
    templateUrl: './document-layout.component.html',
    styleUrl: './document-layout.component.scss'
})
export class DocumentLayoutComponent {
    private breakpointObserver = inject(BreakpointObserver);
    private chatHub = inject(ChatHubService);

    protected activeSection = signal<TabSection>('editor');
    protected isMobile = signal(false);
    protected hasUnreadMessage = signal(false);

    constructor() {
        this.breakpointObserver
            .observe([BreakpointQueries.smallMedium])
            .subscribe(result => {
                const isMobile = result.matches;

                this.isMobile.set(isMobile);
                if (!isMobile)
                    this.hasUnreadMessage.set(false);
            });

        this.chatHub.onMessageReceived
            .pipe(takeUntilDestroyed())
            .subscribe(() => {
                this.hasUnreadMessage.set(this.isMobile() && this.activeSection() !== 'chat');
            });

        toObservable(this.activeSection)
            .pipe(
                takeUntilDestroyed(),
                filter(session => session === 'chat')
            )
            .subscribe(() => this.hasUnreadMessage.set(false))
    }

    protected toggleSection(): void {
        this.activeSection.set(this.activeSection() === 'editor' ? 'chat' : 'editor');
    }
}

import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SHARED_CUSTOM, SHARED_NATIVE } from '../../../../shared';
import { BreakpointObserver } from '@angular/cdk/layout';
import { BreakpointQueries } from '../../../../shared/constants/breakpoints.constant';
import { ChatViewComponent } from '../../../chat/components/chat-view/chat-view.component';

type TabSection = 'editor' | 'chat';

@Component({
    selector: 'okd-document-layout',
    imports: [SHARED_NATIVE, SHARED_CUSTOM, RouterOutlet, ChatViewComponent],
    templateUrl: './document-layout.component.html',
    styleUrl: './document-layout.component.scss'
})
export class DocumentLayoutComponent {
    protected breakpointObserver = inject(BreakpointObserver);
    protected activeSection = signal<TabSection>('editor');
    protected isMobile = signal(false);

    constructor() {
        this.breakpointObserver
            .observe([BreakpointQueries.smallMedium])
            .subscribe(result => {
                this.isMobile.set(result.matches);
            });
    }

    protected toggleSection(): void {
        this.activeSection.set(this.activeSection() === 'editor' ? 'chat' : 'editor');
    }
}

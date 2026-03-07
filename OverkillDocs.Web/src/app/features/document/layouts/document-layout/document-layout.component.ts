import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ChatComponent } from '../../components/chat/chat.component';
import { SHARED_CUSTOM, SHARED_NATIVE } from '../../../../shared';
import { BreakpointObserver } from '@angular/cdk/layout';
import { BreakpointQueries } from '../../../../shared/constants/breakpoints.constant';
import { NavigationRailItem, NavigationRailOrientation } from '../../../../shared/components/navigation-rail/navigation-rail.component';

type TabSection = 'editor' | 'chat';

@Component({
    selector: 'okd-document-layout',
    imports: [SHARED_NATIVE, SHARED_CUSTOM, RouterOutlet, ChatComponent],
    templateUrl: './document-layout.component.html',
    styleUrl: './document-layout.component.scss'
})
export class DocumentLayoutComponent {
    protected breakpointObserver = inject(BreakpointObserver);
    protected activeTab = signal<TabSection>('editor');
    protected isMobile = signal(false);
    protected orientation = signal<NavigationRailOrientation>('horizontal');
    protected tabs: NavigationRailItem<TabSection>[] = [
        {
            icon: 'edit-document',
            label: 'Editor',
            value: 'editor'
        },
        {
            icon: 'chat-bubble',
            label: 'Chat',
            value: 'chat'
        }
    ];

    constructor() {
        this.breakpointObserver
            .observe([BreakpointQueries.smallMedium])
            .subscribe(result => {
                this.isMobile.set(result.matches);
            });

        this.breakpointObserver
            .observe(['(orientation: landscape)'])
            .subscribe(result => {
                this.orientation.set(result.matches ? 'vertical' : 'horizontal');
            });
    }
}

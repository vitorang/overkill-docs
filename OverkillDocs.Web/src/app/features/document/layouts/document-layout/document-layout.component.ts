import { Component, computed, inject, signal } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { filter } from 'rxjs';
import { SHARED } from '@shared/index';
import { ChatViewComponent } from '@features/chat/components/chat-view/chat-view.component';
import { ChatHubService } from '@features/chat/services/chat-hub.service';
import { BreakpointQueries } from '@shared/constants/breakpoints.constant';
import { HubMonitorComponent } from '@features/debug/components/hub-monitor/hub-monitor.component';
import { MainHeaderComponent } from '@shared/components/main-header/main-header.component';
import { UserService } from '@core/services/user.service';
import { ReconnectionOverlayComponent } from '@shared/components/reconnection-overlay/reconnection-overlay.component';
import { RouterOutlet } from '@angular/router';
import { DebugService } from '@features/debug/services/debug.service';
import { DocumentLayoutService } from '@features/document/services/document-layout.services';

@Component({
    selector: 'okd-document-layout',
    imports: [
        SHARED,
        ChatViewComponent,
        HubMonitorComponent,
        MainHeaderComponent,
        ReconnectionOverlayComponent,
        RouterOutlet,
    ],
    templateUrl: './document-layout.component.html',
    styleUrl: './document-layout.component.scss',
    providers: [UserService],
})
export class DocumentLayoutComponent {
    private debugService = inject(DebugService);
    private documentLayoutService = inject(DocumentLayoutService);
    protected displayEditor = computed(
        () =>
            !this.documentLayoutService.isMobile() ||
            this.documentLayoutService.activeSection() === 'editor',
    );

    protected displayChat = computed(
        () =>
            !this.documentLayoutService.isMobile() ||
            this.documentLayoutService.activeSection() === 'chat',
    );

    protected debugModeEnabled = this.debugService.debugModeEnabled;
}

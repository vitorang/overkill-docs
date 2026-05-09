import { Component, computed, inject } from '@angular/core';
import { SHARED } from '@shared/index';
import { ChatViewComponent } from '@features/chat/components/chat-view/chat-view.component';
import { HubMonitorComponent } from '@features/debug/components/hub-monitor/hub-monitor.component';
import { MainHeaderComponent } from '@shared/components/main-header/main-header.component';
import { UserService } from '@core/services/user.service';
import { ReconnectionOverlayComponent } from '@shared/components/reconnection-overlay/reconnection-overlay.component';
import { RouterOutlet } from '@angular/router';
import { DebugService } from '@features/debug/services/debug.service';
import { DocumentLayoutService } from '@features/document/services/document-layout.services';
import { DocumentIndexComponent } from '@features/document/components/document-index/document-index.component';
import { ToggleChatButtonComponent } from '@features/document/components/toggle-chat-button/toggle-chat-button.component';

@Component({
    selector: 'okd-document-layout',
    imports: [
        SHARED,
        ChatViewComponent,
        HubMonitorComponent,
        ReconnectionOverlayComponent,
        RouterOutlet,
        DocumentIndexComponent,
        MainHeaderComponent,
        ToggleChatButtonComponent,
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

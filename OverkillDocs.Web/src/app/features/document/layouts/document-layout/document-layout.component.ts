import { Component, computed, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { SHARED } from '@shared/index';
import { ChatViewComponent } from '@features/chat/components/chat-view/chat-view.component';
import { UserService } from '@core/services/user.service';
import { ReconnectionOverlayComponent } from '@shared/components/reconnection-overlay/reconnection-overlay.component';
import { RouterOutlet } from '@angular/router';
import { DocumentIndexComponent } from '@features/document/components/document-index/document-index.component';
import { PATHS } from '@core/constants/routes.constant';
import { DebugViewerComponent } from '@features/debug/components/debug-viewer/debug-viewer.component';
import { BrandComponent } from '@shared/components/brand/brand.component';
import { BreakpointObserver } from '@angular/cdk/layout';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { BreakpointQueries } from '@shared/constants/breakpoints.constant';
import { map } from 'rxjs';
import { AccountService } from '@features/account/services/account.service';
import { ChatHub } from '@features/chat/hubs/chat.hub';
import { MainHub } from '@core/hubs/main.hub';

type SidePanel = 'documents' | 'chat' | 'debug';

@Component({
    selector: 'okd-document-layout',
    imports: [
        SHARED,
        ChatViewComponent,
        ReconnectionOverlayComponent,
        RouterOutlet,
        DocumentIndexComponent,
        DebugViewerComponent,
        BrandComponent,
    ],
    templateUrl: './document-layout.component.html',
    styleUrl: './document-layout.component.scss',
    providers: [UserService],
})
export class DocumentLayoutComponent implements OnInit, OnDestroy {
    private breakpointObserver = inject(BreakpointObserver);
    private accountService = inject(AccountService);
    private chatHub = inject(ChatHub);
    private mainHub = inject(MainHub);

    protected isMobile = toSignal(
        this.breakpointObserver
            .observe([BreakpointQueries.smallMedium])
            .pipe(map((result) => result.matches)),
        { initialValue: false },
    );

    protected activePanel = signal<SidePanel | null>('documents');
    protected hasUnreadMessage = signal(false);
    protected accountSettingsPath = PATHS.ACCOUNT.SETTINGS;
    protected sidenavClosing = signal(false);
    protected activeButton = computed(() => !this.sidenavClosing() && this.activePanel());

    constructor() {
        this.chatHub.onMessageReceived.pipe(takeUntilDestroyed()).subscribe(() => {
            if (this.activePanel() !== 'chat') {
                this.hasUnreadMessage.set(true);
            }
        });
    }

    ngOnInit(): void {
        this.mainHub.connect();
    }

    ngOnDestroy(): void {
        this.mainHub.disconnect();
    }

    protected toggleActivePanel(panel: SidePanel): void {
        if (panel === 'chat') {
            this.hasUnreadMessage.set(false);
        }

        this.activePanel.set(panel !== this.activePanel() ? panel : null);
    }

    protected logout(): void {
        this.accountService.logout();
    }

    protected onSidenavClosed(): void {
        this.activePanel.set(null);
        this.sidenavClosing.set(false);
    }
}

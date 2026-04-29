import { Component, effect, inject } from '@angular/core';
import { HubService } from '@core/services/hub.service';
import { AccountService } from '@features/account/services/account.service';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-reconnection-overlay',
    imports: [SHARED],
    templateUrl: './reconnection-overlay.component.html',
    styleUrl: './reconnection-overlay.component.scss',
    host: {
        '[attr.data-connected]': 'connected()',
    },
})
export class ReconnectionOverlayComponent {
    private mainHub = inject(HubService).mainHub;
    private accountService = inject(AccountService);
    protected connected = this.mainHub.state.connected;
    protected connecting = this.mainHub.state.connecting;

    constructor() {
        effect(() => {
            if (!this.connected()) (document.activeElement as HTMLElement)?.blur();
        });
    }

    protected reconnect(): void {
        this.mainHub.forceConnect();
    }

    protected logout(): void {
        this.accountService.logout();
    }
}

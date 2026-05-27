import { Component, effect, inject } from '@angular/core';
import { MainHub } from '@core/hubs/main.hub';
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
    private mainHub = inject(MainHub);
    private accountService = inject(AccountService);
    protected connected = this.mainHub.state.connected;
    protected connecting = this.mainHub.state.connecting;

    constructor() {
        effect(() => {
            if (!this.connected()) {
                (document.activeElement as HTMLElement)?.blur();
            }
        });
    }

    protected reconnect(): void {
        this.mainHub.connect();
    }

    protected logout(): void {
        this.accountService.logout();
    }
}

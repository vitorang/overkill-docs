import { Component, inject } from '@angular/core';
import { HubMonitorComponent } from '@features/debug/components/hub-monitor/hub-monitor.component';
import { DebugService } from '@features/debug/services/debug.service';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-debug-viewer',
    imports: [SHARED, HubMonitorComponent],
    templateUrl: './debug-viewer.component.html',
    styleUrl: './debug-viewer.component.scss',
})
export class DebugViewerComponent {
    protected debugService = inject(DebugService);
    protected debugModeEnabled = this.debugService.debugModeEnabled;

    toggleDebugMode(): void {
        this.debugService.debugModeEnabled.set(!this.debugService.debugModeEnabled());
    }
}

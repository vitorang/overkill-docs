import { Component, computed, input, output } from '@angular/core';
import { ApiHandler } from '@core/utils/api-handler.utils';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-request-overlay',
    imports: [SHARED],
    templateUrl: './request-overlay.component.html',
    styleUrl: './request-overlay.component.scss',
})
export class RequestOverlayComponent {
    requestHandler = input.required<ApiHandler>();
    retry = output();
    protected locked = computed(() => this.requestHandler().loading() || this.requestHandler().error())
}

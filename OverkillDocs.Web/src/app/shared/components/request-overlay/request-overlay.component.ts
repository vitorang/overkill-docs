import { Component, computed, input, output } from '@angular/core';
import { SHARED_NATIVE } from '../..';
import { HttpHandler } from '../../../core/utils/http-handler.utils';

@Component({
    selector: 'okd-request-overlay',
    imports: [SHARED_NATIVE],
    templateUrl: './request-overlay.component.html',
    styleUrl: './request-overlay.component.scss',
})
export class RequestOverlayComponent {
    requestHandler = input.required<HttpHandler>();
    retry = output();
    protected locked = computed(() => this.requestHandler().loading() || this.requestHandler().error())
}

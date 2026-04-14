import { Component, input, output } from '@angular/core';
import { RequestState } from '../../../core/models/common.model';
import { SHARED_NATIVE } from '../..';

@Component({
    selector: 'okd-request-overlay',
    imports: [SHARED_NATIVE],
    templateUrl: './request-overlay.component.html',
    styleUrl: './request-overlay.component.scss',
})
export class RequestOverlayComponent {
    requestState = input.required<RequestState>();
    retry = output();

    protected RequestState = RequestState;
}

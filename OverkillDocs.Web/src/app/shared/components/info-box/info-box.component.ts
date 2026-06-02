import { Component, input } from '@angular/core';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-info-box',
    imports: [SHARED],
    templateUrl: './info-box.component.html',
    styleUrl: './info-box.component.scss',
})
export class InfoBoxComponent {
    icon = input.required<string>();
}

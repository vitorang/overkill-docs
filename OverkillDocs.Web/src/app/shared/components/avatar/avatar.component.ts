import { CommonModule } from '@angular/common';
import { Component, HostBinding, input } from '@angular/core';

type sizeUnit = 'px' | 'em' | 'rem' | '%';

@Component({
    selector: 'okd-avatar',
    imports: [CommonModule],
    templateUrl: './avatar.component.html',
    styleUrl: './avatar.component.scss',
})
export class AvatarComponent {
    readonly hashId = input<string>();
    readonly size = input.required<number>();
    readonly sizeUnit = input.required<sizeUnit>();

    @HostBinding('style.--avatar-size')
    get avatarSize(): string {
        return `${this.size()}${this.sizeUnit()}`;
    }
}

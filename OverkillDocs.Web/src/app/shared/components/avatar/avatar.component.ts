import { CommonModule } from '@angular/common';
import { Component, HostBinding, inject, input, signal } from '@angular/core';
import { createAvatar } from '@dicebear/core';
import { botttsNeutral } from '@dicebear/collection';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { SimpleUser } from '../../../core/models/user.model';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { filter } from 'rxjs';

type sizeUnit = 'px' | 'em' | 'rem' | '%';

@Component({
    selector: 'okd-avatar',
    imports: [CommonModule],
    templateUrl: './avatar.component.html',
    styleUrl: './avatar.component.scss',
})
export class AvatarComponent {
    readonly user = input<SimpleUser | null>();
    readonly size = input.required<number>();
    readonly sizeUnit = input.required<sizeUnit>();

    private domSanitizer = inject(DomSanitizer);
    protected svgAvatar = signal(null as SafeHtml | null);

    constructor() {
        this.svgAvatar.set('');

        toObservable(this.user)
            .pipe(
                takeUntilDestroyed(),
                filter(user => !!user)
            )
            .subscribe(user => this.createAvatar(user));
    }

    private createAvatar = (user: SimpleUser) => {
        const radius = 16;

        const avatar = createAvatar(botttsNeutral, { seed: user?.hashId ?? '', radius });
        this.svgAvatar.set(this.domSanitizer.bypassSecurityTrustHtml(avatar.toString()));
    }

    @HostBinding('style.--avatar-size')
    protected get avatarSize(): string {
        return `${this.size()}${this.sizeUnit()}`;
    }
}

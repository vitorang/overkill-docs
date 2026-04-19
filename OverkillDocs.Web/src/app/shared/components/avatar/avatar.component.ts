import { CommonModule } from '@angular/common';
import { Component, HostBinding, inject, input, signal, OnInit, effect } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { AvatarService } from '@core/services/avatar.service';

type sizeUnit = 'px' | 'em' | 'rem' | '%';

@Component({
    selector: 'okd-avatar',
    imports: [CommonModule],
    templateUrl: './avatar.component.html',
    styleUrl: './avatar.component.scss',
})
export class AvatarComponent implements OnInit {
    private avatarService = inject(AvatarService);

    readonly avatarCode = input.required<string>();
    readonly seed = input.required<string>();

    readonly size = input.required<number>();
    readonly sizeUnit = input.required<sizeUnit>();

    private domSanitizer = inject(DomSanitizer);
    protected svgAvatar = signal(null as SafeHtml | null);

    constructor() {
        effect(() => {
            const code = this.avatarCode();
            const seed = this.seed();

            this.createAvatar(code, seed);
        });
    }

    ngOnInit(): void {
        this.createAvatar(this.avatarCode(), this.seed());
    }

    private createAvatar(avatarCode: string, seed: string) {
        const radius = 16;
        const svg = this.avatarService.generateSvg(avatarCode, seed, radius);
        this.svgAvatar.set(this.domSanitizer.bypassSecurityTrustHtml(svg));
    }

    @HostBinding('style.--avatar-size')
    protected get avatarSize(): string {
        return `${this.size()}${this.sizeUnit()}`;
    }
}

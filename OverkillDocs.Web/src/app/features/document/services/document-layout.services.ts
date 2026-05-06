import { BreakpointObserver } from '@angular/cdk/layout';
import { inject, Injectable, signal } from '@angular/core';
import { BreakpointQueries } from '@shared/constants/breakpoints.constant';

@Injectable({ providedIn: 'root' })
export class DocumentLayoutService {
    private breakpointObserver = inject(BreakpointObserver);
    isMobile = signal(false);
    activeSection = signal<'editor' | 'chat'>('editor');

    constructor() {
        this.breakpointObserver.observe([BreakpointQueries.smallMedium]).subscribe((result) => {
            const isMobile = result.matches;
            this.isMobile.set(isMobile);
        });
    }

    toggleSection = (): void => {
        this.activeSection.set(this.activeSection() === 'editor' ? 'chat' : 'editor');
    };
}

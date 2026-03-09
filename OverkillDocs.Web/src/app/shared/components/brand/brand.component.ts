import { Component, input } from '@angular/core';

@Component({
    selector: 'okd-brand',
    imports: [],
    templateUrl: './brand.component.html',
    styleUrl: './brand.component.scss',
    host: {
        '[class.compact]': 'compact()'
    }
})
export class BrandComponent {
    compact = input(false);

    protected get name1(): string {
        return this.compact() ? 'OK' : 'OverKill'
    }

    protected get name2(): string {
        return this.compact() ? 'D' : 'Docs'
    }
}

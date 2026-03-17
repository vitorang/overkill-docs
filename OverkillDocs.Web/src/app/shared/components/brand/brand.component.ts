import { Component, computed, input } from '@angular/core';

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

    protected name1 = computed(() => this.compact() ? 'OK' : 'OverKill');
    protected name2 = computed(() => this.compact() ? 'D' : 'Docs');
}

import { Component } from '@angular/core';
import { BrandComponent } from '@shared/components/brand/brand.component';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-document-intro-page',
    imports: [SHARED, BrandComponent],
    templateUrl: './document-intro-page.component.html',
    styleUrl: './document-intro-page.component.scss',
})
export class DocumentIntroPageComponent {
    protected openGithub(): void {
        window.open('https://github.com/vitorang/overkill-docs#readme', '_blank');
    }

    protected openSwagger(): void {
        window.open('/api/swagger', '_blank');
    }
}

import { Component, input } from '@angular/core';
import { DocumentFragment } from '@features/document/models/document.models';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-article-add-fragment',
    imports: [SHARED],
    templateUrl: './article-add-fragment.component.html',
    styleUrl: './article-add-fragment.component.scss',
})
export class ArticleAddFragmentComponent {
    fragments = input.required<DocumentFragment[]>();
    insertAfter = input.required<DocumentFragment | null>();
}

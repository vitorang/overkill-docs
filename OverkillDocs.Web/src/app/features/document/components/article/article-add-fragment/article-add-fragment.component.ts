import { Component, input } from '@angular/core';
import { DocumentFragmentModel } from '@features/document/models/document.model';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-article-add-fragment',
    imports: [SHARED],
    templateUrl: './article-add-fragment.component.html',
    styleUrl: './article-add-fragment.component.scss',
})
export class ArticleAddFragmentComponent {
    insertAfter = input.required<DocumentFragmentModel | null>();
}

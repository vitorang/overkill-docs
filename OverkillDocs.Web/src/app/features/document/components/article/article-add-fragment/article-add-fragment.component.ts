import { Component, output } from '@angular/core';
import { DocumentFragmentType } from '@features/document/models/document.models';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-article-add-fragment',
    imports: [SHARED],
    templateUrl: './article-add-fragment.component.html',
    styleUrl: './article-add-fragment.component.scss',
})
export class ArticleAddFragmentComponent {
    create = output<DocumentFragmentType>();

    protected Type = DocumentFragmentType;
}

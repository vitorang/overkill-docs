import { Component, input } from '@angular/core';
import { ArticleImageFragment } from '@features/document/models/article.models';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-article-image-fragment',
    imports: [SHARED],
    templateUrl: './article-image-fragment.component.html',
    styleUrl: './article-image-fragment.component.scss',
})
export class ArticleImageFragmentComponent {
    fragment = input.required<ArticleImageFragment>();
}

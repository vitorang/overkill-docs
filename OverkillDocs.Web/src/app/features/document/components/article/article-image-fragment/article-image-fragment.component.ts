import { Component, computed, input } from '@angular/core';
import { ArticleImageFragment } from '@features/document/models/article.models';
import { SHARED } from '@shared/index';
import { ArticlePlaceholderFragmentComponent } from '../article-placeholder-fragment/article-placeholder-fragment.component';

@Component({
    selector: 'okd-article-image-fragment',
    imports: [SHARED, ArticlePlaceholderFragmentComponent],
    templateUrl: './article-image-fragment.component.html',
    styleUrl: './article-image-fragment.component.scss',
})
export class ArticleImageFragmentComponent {
    fragment = input.required<ArticleImageFragment>();

    protected showPlaceholder = computed(() => !this.fragment().url.trim());
}

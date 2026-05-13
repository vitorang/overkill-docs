import { Component, computed, input } from '@angular/core';
import {
    ArticleImageFragment,
    DocumentFragmentModel,
} from '@features/document/models/document.model';

@Component({
    selector: 'okd-article-image-fragment',
    imports: [],
    templateUrl: './article-image-fragment.component.html',
    styleUrl: './article-image-fragment.component.scss',
})
export class ArticleImageFragmentComponent {
    fragment = input.required<DocumentFragmentModel>();
    value = computed(() => JSON.parse(this.fragment().value) as ArticleImageFragment);
}

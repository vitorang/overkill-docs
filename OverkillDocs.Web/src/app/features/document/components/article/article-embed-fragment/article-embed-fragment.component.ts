import { Component, computed, inject, input } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ArticleEmbedFragment } from '@features/document/models/article.models';
import { SHARED } from '@shared/index';
import { ArticlePlaceholderFragmentComponent } from '../article-placeholder-fragment/article-placeholder-fragment.component';

@Component({
    selector: 'okd-article-embed-fragment',
    imports: [SHARED, ArticlePlaceholderFragmentComponent],
    templateUrl: './article-embed-fragment.component.html',
    styleUrl: './article-embed-fragment.component.scss',
})
export class ArticleEmbedFragmentComponent {
    fragment = input.required<ArticleEmbedFragment>();
    isEditing = input.required<boolean>();

    private sanitizer = inject(DomSanitizer);
    protected isVertical = computed(() => this.isVerticalContent(this.fragment().url));
    protected url = computed((): SafeResourceUrl | null => {
        const url = this.embedUrlFrom(this.fragment().url);
        if (url === null) {
            return null;
        }
        return this.sanitizer.bypassSecurityTrustResourceUrl(url);
    });

    protected showPlaceholder = computed(() => !this.url());

    private embedUrlFrom(url: string): string | null {
        const a = document.createElement('a');
        a.href = url;
        const queryParams = new URLSearchParams(a.search.split('?')[1] ?? '');

        let videoId = '';

        if (a.host === 'www.youtube.com') {
            if (a.pathname.startsWith('/shorts/')) {
                videoId = a.pathname.split('/')[2];
                return `https://www.youtube.com/embed/${videoId}`;
            } else if (queryParams.get('v')) {
                videoId = queryParams.get('v')!;
                return `https://www.youtube.com/embed/${videoId}`;
            }
        }

        if (a.host === 'youtu.be') {
            videoId = a.pathname.split('/')[1];
            return `https://www.youtube.com/embed/${videoId}`;
        }

        return null;
    }

    isVerticalContent(url: string): boolean {
        const a = document.createElement('a');
        a.href = url;

        if (a.host === 'www.youtube.com') {
            return a.pathname.startsWith('/shorts/');
        }

        return false;
    }
}

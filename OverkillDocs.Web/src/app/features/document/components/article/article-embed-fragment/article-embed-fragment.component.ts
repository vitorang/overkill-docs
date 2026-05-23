import { Component, computed, inject, input, model, output, signal } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ArticleEmbedFragment } from '@features/document/models/article.models';
import { SHARED } from '@shared/index';
import { ArticlePlaceholderFragmentComponent } from '../article-placeholder-fragment/article-placeholder-fragment.component';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { filter, map, merge } from 'rxjs';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';

@Component({
    selector: 'okd-article-embed-fragment',
    imports: [SHARED, ArticlePlaceholderFragmentComponent],
    templateUrl: './article-embed-fragment.component.html',
    styleUrl: './article-embed-fragment.component.scss',
})
export class ArticleEmbedFragmentComponent {
    fragment = input.required<ArticleEmbedFragment>();
    isEditing = input.required<boolean>();
    fragmentChanged = output<ArticleEmbedFragment>();

    private viewerHub = inject(DocumentViewerHub);
    protected urlModel = model<string>('');
    protected current = signal({
        updatedAt: '',
        url: null as SafeResourceUrl | null,
        isVertical: false,
    });

    constructor() {
        toObservable(this.isEditing)
            .pipe(filter((isEditing) => isEditing))
            .subscribe(() => this.urlModel.set(this.fragment().url));

        merge(
            toObservable(this.fragment),
            this.viewerHub.onFragmentChanged.pipe(map((e) => e as ArticleEmbedFragment)),
        )
            .pipe(
                takeUntilDestroyed(),
                filter((fragment) => fragment.hashId === this.fragment().hashId),
                filter((fragment) => fragment.updatedAt > this.current().updatedAt),
                filter(() => this.viewerHub.state.connected()),
            )
            .subscribe((fragment) => {
                this.current.set({
                    updatedAt: fragment.updatedAt,
                    url: this.embedUrlFrom(fragment.url),
                    isVertical: this.isVerticalContent(fragment.url),
                });
            });
    }

    private sanitizer = inject(DomSanitizer);
    protected showPlaceholder = computed(() => !this.current().url);

    private embedUrlFrom(url: string): SafeResourceUrl | null {
        const a = document.createElement('a');
        a.href = url;
        const queryParams = new URLSearchParams(a.search.split('?')[1] ?? '');

        let videoId = '';
        const safe = (url: string) => this.sanitizer.bypassSecurityTrustResourceUrl(url);

        if (a.host === 'www.youtube.com') {
            if (a.pathname.startsWith('/shorts/')) {
                videoId = a.pathname.split('/')[2];
                return safe(`https://www.youtube.com/embed/${videoId}`);
            } else if (queryParams.get('v')) {
                videoId = queryParams.get('v')!;
                return safe(`https://www.youtube.com/embed/${videoId}`);
            }
        }

        if (a.host === 'youtu.be') {
            videoId = a.pathname.split('/')[1];
            return safe(`https://www.youtube.com/embed/${videoId}`);
        }

        return null;
    }

    private isVerticalContent(url: string): boolean {
        const a = document.createElement('a');
        a.href = url;

        if (a.host === 'www.youtube.com') {
            return a.pathname.startsWith('/shorts/');
        }

        return false;
    }

    protected onInputBlur(): void {
        const modelChanged = this.urlModel() !== this.fragment().url;
        if (modelChanged) {
            this.fragmentChanged.emit({
                ...this.fragment(),
                url: this.urlModel(),
            });
        }
    }
}

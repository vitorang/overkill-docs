import { Component, computed, inject, input, model, output, signal } from '@angular/core';
import { ArticleImageFragment } from '@features/document/models/article.models';
import { SHARED } from '@shared/index';
import { ArticlePlaceholderFragmentComponent } from '../article-placeholder-fragment/article-placeholder-fragment.component';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { filter, map, merge } from 'rxjs';

@Component({
    selector: 'okd-article-image-fragment',
    imports: [SHARED, ArticlePlaceholderFragmentComponent],
    templateUrl: './article-image-fragment.component.html',
    styleUrl: './article-image-fragment.component.scss',
})
export class ArticleImageFragmentComponent {
    fragment = input.required<ArticleImageFragment>();
    isEditing = input.required<boolean>();
    fragmentChanged = output<ArticleImageFragment>();

    private viewerHub = inject(DocumentViewerHub);
    protected urlModel = model<string>('');
    protected altModel = model<string>('');
    protected current = signal({
        updatedAt: '',
        url: '',
        alt: '',
    });

    protected showPlaceholder = computed(() => !this.current().url.trim());

    constructor() {
        toObservable(this.isEditing)
            .pipe(filter((isEditing) => isEditing))
            .subscribe(() => {
                this.urlModel.set(this.fragment().url);
                this.altModel.set(this.fragment().alt);
            });

        merge(
            toObservable(this.fragment),
            this.viewerHub.onFragmentChanged.pipe(map((e) => e as ArticleImageFragment)),
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
                    url: fragment.url,
                    alt: fragment.alt,
                });
            });
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

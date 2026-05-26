import {
    Component,
    computed,
    inject,
    input,
    model,
    output,
    signal,
    TemplateRef,
    viewChild,
} from '@angular/core';
import { ArticleImageFragment } from '@features/document/models/article.models';
import { SHARED } from '@shared/index';
import { ArticlePlaceholderFragmentComponent } from '../article-placeholder-fragment/article-placeholder-fragment.component';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { filter, map, merge } from 'rxjs';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';

@Component({
    selector: 'okd-article-image-fragment',
    imports: [SHARED, ArticlePlaceholderFragmentComponent],
    templateUrl: './article-image-fragment.component.html',
    styleUrl: './article-image-fragment.component.scss',
})
export class ArticleImageFragmentComponent {
    fragment = input.required<ArticleImageFragment>();
    isEditing = input.required<boolean>();
    finishEdit = output<ArticleImageFragment | null>();

    private toolbarContent = viewChild<TemplateRef<void>>('toolbarContent');
    private viewerHub = inject(DocumentViewerHub);
    private viewerService = inject(DocumentViewerService);
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
                this.viewerService.toolbar.set({
                    template: this.toolbarContent()!,
                    showTitle: true,
                });
            });

        merge(
            toObservable(this.fragment),
            this.viewerHub.onFragmentChanged.pipe(map((e) => e as ArticleImageFragment)),
        )
            .pipe(
                takeUntilDestroyed(),
                filter((fragment) => fragment.hashId === this.fragment().hashId),
                filter((fragment) => fragment.updatedAt > this.current().updatedAt),
            )
            .subscribe((fragment) => {
                this.current.set({
                    updatedAt: fragment.updatedAt,
                    url: fragment.url,
                    alt: fragment.alt,
                });
            });
    }

    protected saveAndFinishEdit(): void {
        const modelChanged = this.urlModel() !== this.fragment().url;

        if (modelChanged) {
            this.finishEdit.emit({
                ...this.fragment(),
                url: this.urlModel(),
            });
        } else {
            this.finishEdit.emit(null);
        }
    }
}

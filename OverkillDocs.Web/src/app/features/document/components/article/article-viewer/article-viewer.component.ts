import { Component, computed, DestroyRef, inject, signal } from '@angular/core';
import { SHARED } from '@shared/index';
import { ArticleMarkdownFragmentComponent } from '../article-markdown-fragment/article-markdown-fragment.component';
import { ArticleImageFragmentComponent } from '../article-image-fragment/article-image-fragment.component';
import { ArticleEmbedFragmentComponent } from '../article-embed-fragment/article-embed-fragment.component';
import { DocumentFragment, DocumentFragmentType } from '@features/document/models/document.models';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';
import { InfoBoxComponent } from '@shared/components/info-box/info-box.component';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { filter, finalize } from 'rxjs';
import {
    ArticleEmbedFragment,
    ArticleImageFragment,
    ArticleMarkdownFragment,
} from '@features/document/models/article.models';
import { ArticleAddFragmentComponent } from '@features/document/components/article/article-add-fragment/article-add-fragment.component';
import { ArticleEditFragmentComponent } from '@features/document/components/article/article-edit-fragment/article-edit-fragment.component';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';

@Component({
    selector: 'okd-article-viewer',
    imports: [
        SHARED,
        ArticleMarkdownFragmentComponent,
        ArticleImageFragmentComponent,
        ArticleEmbedFragmentComponent,
        InfoBoxComponent,
        ArticleAddFragmentComponent,
        ArticleEditFragmentComponent,
    ],
    templateUrl: './article-viewer.component.html',
    styleUrl: './article-viewer.component.scss',
})
export class ArticleViewerComponent {
    private viewerHub = inject(DocumentViewerHub);
    private viewerService = inject(DocumentViewerService);
    private destroyRef = inject(DestroyRef);

    protected FragmentType = DocumentFragmentType;
    protected document = this.viewerService.document;
    protected editModeEnabled = signal(false);
    protected isLoading = signal(false);

    protected editingFragmentId = signal<string | null>(null);
    protected isEditingFragment = computed(() => !!this.editingFragmentId());
    protected toolbar = this.viewerService.toolbar;

    constructor() {
        toObservable(this.editModeEnabled)
            .pipe(filter((enabled) => !enabled))
            .subscribe(() => {
                this.editingFragmentId.set(null);
                this.toolbar.set(null);
            });
    }

    protected addFragment(type: DocumentFragmentType, after: DocumentFragment | null): void {
        if (this.isLoading() || this.isEditingFragment()) {
            return;
        }

        this.isLoading.set(true);
        this.viewerService
            .createFragment(type, after?.hashId || null)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe();
    }

    protected edit(hashId: string | null): void {
        this.editingFragmentId.set(hashId);
        if (hashId === null) {
            this.toolbar.set(null);
        }
    }

    protected delete(fragment: DocumentFragment): void {
        this.isLoading.set(true);
        this.viewerService
            .deleteFragment(fragment)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe();
    }

    protected asMarkdown(fragment: DocumentFragment): ArticleMarkdownFragment {
        return fragment as ArticleMarkdownFragment;
    }

    protected asImage(fragment: DocumentFragment): ArticleImageFragment {
        return fragment as ArticleImageFragment;
    }

    protected asEmbed(fragment: DocumentFragment): ArticleEmbedFragment {
        return fragment as ArticleEmbedFragment;
    }

    protected onFragmentChanged(fragment: DocumentFragment): void {
        if (!this.viewerHub.state.connected()) {
            return;
        }

        this.viewerService
            .updateFragment(fragment)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe();
    }

    protected saveAndRelease(fragment: DocumentFragment | null): void {
        if (!this.viewerHub.state.connected()) {
            return;
        }

        if (fragment === null) {
            return this.edit(null);
        }

        this.viewerService
            .updateFragment(fragment)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(() => {
                this.edit(null);
            });
    }
}

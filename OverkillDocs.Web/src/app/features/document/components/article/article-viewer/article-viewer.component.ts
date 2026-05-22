import { Component, computed, DestroyRef, inject, signal } from '@angular/core';
import { SHARED } from '@shared/index';
import { ArticleMarkdownFragmentComponent } from '../article-markdown-fragment/article-markdown-fragment.component';
import { ArticleImageFragmentComponent } from '../article-image-fragment/article-image-fragment.component';
import { ArticleEmbedFragmentComponent } from '../article-embed-fragment/article-embed-fragment.component';
import { DocumentFragment, DocumentFragmentType } from '@features/document/models/document.models';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';
import { InfoBoxComponent } from '@shared/components/info-box/info-box.component';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { filter, finalize, take } from 'rxjs';
import {
    ArticleEmbedFragment,
    ArticleImageFragment,
    ArticleMarkdownFragment,
} from '@features/document/models/article.models';
import { ArticleFragmentActionsComponent } from '../article-fragment-actions/article-fragment-actions.component';

@Component({
    selector: 'okd-article-viewer',
    imports: [
        SHARED,
        ArticleMarkdownFragmentComponent,
        ArticleImageFragmentComponent,
        ArticleEmbedFragmentComponent,
        InfoBoxComponent,
        ArticleFragmentActionsComponent,
    ],
    templateUrl: './article-viewer.component.html',
    styleUrl: './article-viewer.component.scss',
})
export class ArticleViewerComponent {
    private viewerService = inject(DocumentViewerService);
    private destroyRef = inject(DestroyRef);

    protected FragmentType = DocumentFragmentType;
    protected document = this.viewerService.document;
    protected editModeEnabled = signal(false);
    protected isLoading = signal(false);

    protected editingFragmentId = signal('');
    protected isEditingFragment = computed(() => !!this.editingFragmentId());

    constructor() {
        toObservable(this.editModeEnabled)
            .pipe(filter((enabled) => !enabled))
            .subscribe(() => this.editingFragmentId.set(''));
    }

    protected addFragment(type: DocumentFragmentType, after: DocumentFragment | null): void {
        this.isLoading.set(true);
        this.viewerService
            .createFragment(type, after?.hashId || null)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe();
    }

    protected edit(documentHashId: string): void {
        this.editingFragmentId.set(documentHashId);
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
}

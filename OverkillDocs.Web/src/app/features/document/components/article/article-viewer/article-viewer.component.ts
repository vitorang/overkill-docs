import { Component, DestroyRef, inject, signal } from '@angular/core';
import { SHARED } from '@shared/index';
import { ArticleMarkdownFragmentComponent } from '../article-markdown-fragment/article-markdown-fragment.component';
import { ArticleImageFragmentComponent } from '../article-image-fragment/article-image-fragment.component';
import { ArticleEmbedFragmentComponent } from '../article-embed-fragment/article-embed-fragment.component';
import { DocumentFragment, DocumentFragmentType } from '@features/document/models/document.models';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';
import { ArticleAddFragmentComponent } from '../article-add-fragment/article-add-fragment.component';
import { InfoBoxComponent } from '@shared/components/info-box/info-box.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';
import {
    ArticleEmbedFragment,
    ArticleImageFragment,
    ArticleMarkdownFragment,
} from '@features/document/models/article.models';

@Component({
    selector: 'okd-article-viewer',
    imports: [
        SHARED,
        ArticleMarkdownFragmentComponent,
        ArticleImageFragmentComponent,
        ArticleEmbedFragmentComponent,
        ArticleAddFragmentComponent,
        InfoBoxComponent,
    ],
    templateUrl: './article-viewer.component.html',
    styleUrl: './article-viewer.component.scss',
})
export class ArticleViewerComponent {
    private viewerService = inject(DocumentViewerService);
    private destroyRef = inject(DestroyRef);

    protected FragmentType = DocumentFragmentType;
    protected document = this.viewerService.document;
    protected editMode = signal(false);
    protected isLoading = signal(false);

    protected addFragment(type: DocumentFragmentType, after: DocumentFragment | null): void {
        this.isLoading.set(true);
        this.viewerService
            .addFragment(type, after)
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

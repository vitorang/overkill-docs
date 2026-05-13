import { Component, inject, signal } from '@angular/core';
import { SHARED } from '@shared/index';
import { ArticleMarkdownFragmentComponent } from '../article-markdown-fragment/article-markdown-fragment.component';
import { ArticleImageFragmentComponent } from '../article-image-fragment/article-image-fragment.component';
import { ArticleEmbedFragmentComponent } from '../article-embed-fragment/article-embed-fragment.component';
import { DocumentFragmentType } from '@features/document/models/document.model';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';
import { ArticleAddFragmentComponent } from '../article-add-fragment/article-add-fragment.component';

@Component({
    selector: 'okd-article-viewer',
    imports: [
        SHARED,
        ArticleMarkdownFragmentComponent,
        ArticleImageFragmentComponent,
        ArticleEmbedFragmentComponent,
        ArticleAddFragmentComponent,
    ],
    templateUrl: './article-viewer.component.html',
    styleUrl: './article-viewer.component.scss',
})
export class ArticleViewerComponent {
    private viewerService = inject(DocumentViewerService);
    protected document = this.viewerService.document;
    protected editMode = signal(true);

    protected FragmentType = DocumentFragmentType;
}

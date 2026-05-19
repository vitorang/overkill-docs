import { Component, inject, signal } from '@angular/core';
import { SHARED } from '@shared/index';
import { ArticleMarkdownFragmentComponent } from '../article-markdown-fragment/article-markdown-fragment.component';
import { ArticleImageFragmentComponent } from '../article-image-fragment/article-image-fragment.component';
import { ArticleEmbedFragmentComponent } from '../article-embed-fragment/article-embed-fragment.component';
import { DocumentFragmentType } from '@features/document/models/document.models';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';
import { ArticleAddFragmentComponent } from '../article-add-fragment/article-add-fragment.component';
import { InfoBoxComponent } from '@shared/components/info-box/info-box.component';

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
    protected editMode = signal(false);
    protected document = this.viewerService.document;

    protected FragmentType = DocumentFragmentType;
}

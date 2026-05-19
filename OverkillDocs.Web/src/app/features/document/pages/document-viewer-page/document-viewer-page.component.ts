import { Component, inject } from '@angular/core';
import { DocumentType } from '@features/document/models/document.models';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';
import { SHARED } from '@shared/index';
import { ArticleViewerComponent } from '@features/document/components/article/article-viewer/article-viewer.component';
import { RequestOverlayComponent } from '@shared/components/request-overlay/request-overlay.component';

@Component({
    selector: 'okd-document-viewer-page',
    imports: [SHARED, ArticleViewerComponent, RequestOverlayComponent],
    templateUrl: './document-viewer-page.component.html',
    styleUrl: './document-viewer-page.component.scss',
    providers: [DocumentViewerService],
})
export class DocumentViewerPageComponent {
    private viewerService = inject(DocumentViewerService);
    protected documentHandler = this.viewerService.documentHandler;
    protected document = this.viewerService.document;
    protected DocumentType = DocumentType;
}

import { Component, inject, OnInit } from '@angular/core';
import { DocumentModelType } from '@features/document/models/document.model';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';
import { SHARED } from '@shared/index';
import { ArticleViewerComponent } from '@features/document/components/article/article-viewer/article-viewer.component';

@Component({
    selector: 'okd-document-viewer-page',
    imports: [SHARED, ArticleViewerComponent],
    templateUrl: './document-viewer-page.component.html',
    styleUrl: './document-viewer-page.component.scss',
    providers: [DocumentViewerService],
})
export class DocumentViewerPageComponent implements OnInit {
    private viewerService = inject(DocumentViewerService);
    protected document = this.viewerService.document;
    protected DocumentType = DocumentModelType;

    ngOnInit(): void {
        this.viewerService.load();
    }
}

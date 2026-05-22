import { Component, inject, input, OnInit } from '@angular/core';
import { DocumentType } from '@features/document/models/document.models';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';
import { SHARED } from '@shared/index';
import { ArticleViewerComponent } from '@features/document/components/article/article-viewer/article-viewer.component';
import { RequestOverlayComponent } from '@shared/components/request-overlay/request-overlay.component';
import { UserService } from '@core/services/user.service';

@Component({
    selector: 'okd-document-viewer',
    imports: [SHARED, ArticleViewerComponent, RequestOverlayComponent],
    templateUrl: './document-viewer.component.html',
    styleUrl: './document-viewer.component.scss',
    providers: [DocumentViewerService, UserService],
})
export class DocumentViewerComponent implements OnInit {
    documentHashId = input.required<string>();

    private viewerService = inject(DocumentViewerService);
    protected documentHandler = this.viewerService.documentHandler;
    protected document = this.viewerService.document;
    protected DocumentType = DocumentType;

    ngOnInit(): void {
        this.viewerService.initialize(this.documentHashId());
    }
}

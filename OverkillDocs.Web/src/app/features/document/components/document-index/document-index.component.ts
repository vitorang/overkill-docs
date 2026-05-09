import { Component, effect, inject, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { SEGMENTS } from '@core/constants/routes.constant';
import { DebugService } from '@features/debug/services/debug.service';
import { DocumentEditDialogComponent } from '@features/document/components/dialogs/document-edit-dialog/document-edit-dialog.component';
import { DocumentModel } from '@features/document/models/document.model';
import { DocumentIndexService } from '@features/document/services/document-index.service';
import { SHARED } from '@shared/index';
import { Router } from '@angular/router';

@Component({
    selector: 'okd-document-index',
    imports: [SHARED],
    templateUrl: './document-index.component.html',
    styleUrl: './document-index.component.scss',
    providers: [DocumentIndexService],
})
export class DocumentIndexComponent {
    private dialog = inject(MatDialog);
    private injector = inject(Injector);
    private router = inject(Router);

    private documentIndexService = inject(DocumentIndexService);
    protected documents = this.documentIndexService.documents.asReadonly();
    protected debugModeEnabled = inject(DebugService).debugModeEnabled;

    protected rootUrl = `/${SEGMENTS.DOCUMENT.ROOT}`;
    protected createdDocumentId: string | null = null;

    protected generateMany = (): void => this.documentIndexService.debugGenerateMany(5);
    protected deleteAll = (): void => this.documentIndexService.debugDeleteAll();

    constructor() {
        effect(() => this.redirectToCreatedDocument(this.documents()));
    }

    protected openCreationDialog(): void {
        this.dialog
            .open(DocumentEditDialogComponent, {
                width: '500px',
                data: null as DocumentModel | null,
                injector: this.injector,
            })
            .afterClosed()
            .subscribe((document: DocumentModel | null) => {
                this.createdDocumentId = document?.hashId ?? null;
            });
    }

    private redirectToCreatedDocument(documents: DocumentModel[]) {
        if (this.createdDocumentId === null) return;

        const document = documents.find((doc) => doc.hashId === this.createdDocumentId);

        if (document) {
            this.router.navigate([this.rootUrl, document.hashId]);
            this.createdDocumentId = null;
        }
    }
}

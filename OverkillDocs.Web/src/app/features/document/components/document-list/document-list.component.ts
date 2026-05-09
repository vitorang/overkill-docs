import { Component, inject, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { SEGMENTS } from '@core/constants/routes.constant';
import { DebugService } from '@features/debug/services/debug.service';
import { DocumentEditDialogComponent } from '@features/document/components/dialogs/document-edit-dialog/document-edit-dialog.component';
import { DocumentModel } from '@features/document/models/document.model';
import { DocumentService } from '@features/document/services/document.service';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-document-list',
    imports: [SHARED],
    templateUrl: './document-list.component.html',
    styleUrl: './document-list.component.scss',
    providers: [DocumentService],
})
export class DocumentListComponent {
    private dialog = inject(MatDialog);
    private injector = inject(Injector);

    private documentService = inject(DocumentService);
    protected documents = this.documentService.documents;
    protected debugModeEnabled = inject(DebugService).debugModeEnabled;

    protected rootUrl = `/${SEGMENTS.DOCUMENT.ROOT}`;

    protected generateMany = (): void => this.documentService.debugGenerateMany(5);
    protected deleteAll = (): void => this.documentService.debugDeleteAll();

    protected openCreationDialog(): void {
        this.dialog.open(DocumentEditDialogComponent, {
            width: '500px',
            data: null as DocumentModel | null,
            injector: this.injector,
        });
    }
}

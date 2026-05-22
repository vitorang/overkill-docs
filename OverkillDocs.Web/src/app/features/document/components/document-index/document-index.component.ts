import { Component, inject, Injector } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { SEGMENTS } from '@core/constants/routes.constant';
import { DebugService } from '@features/debug/services/debug.service';
import { DocumentEditDialogComponent } from '@features/document/components/dialogs/document-edit-dialog/document-edit-dialog.component';
import { DocumentSummary } from '@features/document/models/document.models';
import { DocumentIndexService } from '@features/document/services/document-index.service';
import { SHARED } from '@shared/index';
import { Router } from '@angular/router';
import { InfoBoxComponent } from '@shared/components/info-box/info-box.component';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { skip } from 'rxjs';

@Component({
    selector: 'okd-document-index',
    imports: [SHARED, InfoBoxComponent],
    templateUrl: './document-index.component.html',
    styleUrl: './document-index.component.scss',
    providers: [DocumentIndexService],
})
export class DocumentIndexComponent {
    private dialog = inject(MatDialog);
    private injector = inject(Injector);
    private router = inject(Router);

    private indexService = inject(DocumentIndexService);
    protected documents = this.indexService.documents.asReadonly();
    protected debugModeEnabled = inject(DebugService).debugModeEnabled;

    protected rootUrl = `/${SEGMENTS.DOCUMENT.ROOT}`;
    protected createdDocumentId: string | null = null;

    protected generateMany = (): void => this.indexService.debugGenerateMany(5);
    protected deleteAll = (): void => this.indexService.debugDeleteAll();

    constructor() {
        toObservable(this.documents)
            .pipe(takeUntilDestroyed(), skip(1))
            .subscribe((documents) => {
                const hashId = this.currentDocumentHashId;
                if (hashId && !documents.find((e) => e.hashId === hashId)) {
                    this.router.navigate([this.rootUrl]);
                }

                this.redirectToCreatedDocument(this.documents());
            });
    }

    protected openCreationDialog(): void {
        this.dialog
            .open(DocumentEditDialogComponent, {
                width: '500px',
                data: null as DocumentSummary | null,
                injector: this.injector,
            })
            .afterClosed()
            .subscribe((document: DocumentSummary | null) => {
                this.createdDocumentId = document?.hashId ?? null;
            });
    }

    private redirectToCreatedDocument(documents: DocumentSummary[]) {
        if (this.createdDocumentId === null) {
            return;
        }

        const document = documents.find((doc) => doc.hashId === this.createdDocumentId);
        if (document) {
            this.router.navigate([this.rootUrl, document.hashId]);
            this.createdDocumentId = null;
        }
    }

    get currentDocumentHashId(): string | null {
        return this.getParamFromRoute('documentHashId');
    }

    private getParamFromRoute(paramName: string): string | null {
        let route = this.router.routerState.snapshot.root;
        let paramValue = route.paramMap.get(paramName);
        while (route.firstChild) {
            route = route.firstChild;
            if (route.paramMap.has(paramName)) {
                paramValue = route.paramMap.get(paramName);
            }
        }

        return paramValue;
    }
}

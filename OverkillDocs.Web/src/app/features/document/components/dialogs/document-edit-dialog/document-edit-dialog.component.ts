import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, DestroyRef, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ProblemDetails } from '@core/models/problem-details.model';
import { apiHandler, ApiHandler } from '@core/utils/api-handler.utils';
import { FormUtils } from '@core/utils/form.utils';
import { DocumentSummary, DocumentType } from '@features/document/models/document.models';
import { DocumentIndexService } from '@features/document/services/document-index.service';
import { SHARED } from '@shared/index';
import { AlertService } from '@shared/services/alert.service';

type DocumentForm = FormGroup<{
    [K in keyof DocumentSummary]: FormControl<DocumentSummary[K]>;
}>;

@Component({
    selector: 'okd-document-edit-dialog',
    imports: [SHARED],
    templateUrl: './document-edit-dialog.component.html',
    styleUrl: './document-edit-dialog.component.scss',
})
export class DocumentEditDialogComponent implements OnInit {
    private dialogRef = inject(MatDialogRef<DocumentEditDialogComponent>);
    private data = inject<DocumentSummary | null>(MAT_DIALOG_DATA);

    private alertService = inject(AlertService);
    private documentService = inject(DocumentIndexService);
    protected documentHandler = apiHandler();

    protected isEditing = computed(() => !!this.data?.hashId);
    protected readonly DocumentType = DocumentType;

    private formBuilder = inject(NonNullableFormBuilder);
    protected formGroup: DocumentForm = this.formBuilder.group({
        hashId: [''],
        title: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
        type: [{ value: DocumentType.Article, disabled: this.isEditing() }],
    });

    ngOnInit(): void {
        const emptyDocument: DocumentSummary = {
            hashId: '',
            title: '',
            type: DocumentType.Article,
        };
        this.formGroup.setValue(this.data || emptyDocument);
    }

    protected onSubmit(): void {
        if (!this.formGroup.valid || this.documentHandler.loading()) return;

        const document: DocumentSummary = this.formGroup.getRawValue();

        const observable = this.isEditing()
            ? this.documentService.update(document)
            : this.documentService.create(document);

        this.documentHandler.execute(
            observable,
            (result) => this.dialogRef.close(result),
            (err) => this.onError(err),
        );
    }

    private onError(err: HttpErrorResponse) {
        const problem = err.error as ProblemDetails | undefined;
        if (problem?.errors) FormUtils.injectError(this.formGroup, problem.errors);
        else this.alertService.error(problem?.detail);
    }

    protected get titleError(): string {
        return FormUtils.getFieldError(this.formGroup.controls.title);
    }
}

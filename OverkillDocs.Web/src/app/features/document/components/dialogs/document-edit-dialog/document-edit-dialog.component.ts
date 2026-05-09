import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, DestroyRef, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ProblemDetails } from '@core/models/problem-details.model';
import { ApiHandler } from '@core/utils/api-handler.utils';
import { FormUtils } from '@core/utils/form.utils';
import { DocumentModel, DocumentModelType } from '@features/document/models/document.model';
import { DocumentService } from '@features/document/services/document.service';
import { SHARED } from '@shared/index';
import { AlertService } from '@shared/services/alert.service';

type DocumentForm = FormGroup<{
    [K in keyof DocumentModel]: FormControl<DocumentModel[K]>;
}>;

@Component({
    selector: 'okd-document-edit-dialog',
    imports: [SHARED],
    templateUrl: './document-edit-dialog.component.html',
    styleUrl: './document-edit-dialog.component.scss',
})
export class DocumentEditDialogComponent implements OnInit {
    private dialogRef = inject(MatDialogRef<DocumentEditDialogComponent>);
    private data = inject<DocumentModel | null>(MAT_DIALOG_DATA);

    private alertService = inject(AlertService);
    private documentService = inject(DocumentService);
    protected documentHandler = new ApiHandler(inject(DestroyRef));

    protected isEditing = computed(() => !!this.data?.hashId);
    protected readonly DocumentType = DocumentModelType;

    private formBuilder = inject(NonNullableFormBuilder);
    protected formGroup: DocumentForm = this.formBuilder.group({
        hashId: [''],
        title: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(100)]],
        type: [{ value: DocumentModelType.Article, disabled: this.isEditing() }],
    });

    ngOnInit(): void {
        const emptyDocument: DocumentModel = {
            hashId: '',
            title: '',
            type: DocumentModelType.Article,
        };
        this.formGroup.setValue(this.data || emptyDocument);
    }

    protected onSubmit(): void {
        if (!this.formGroup.valid || this.documentHandler.loading()) return;

        const document: DocumentModel = this.formGroup.getRawValue();

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

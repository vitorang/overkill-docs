import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SHARED } from '@shared/index';

export interface IConfirmDialog {
    message: string;
}

@Component({
    selector: 'okd-confirm-dialog',
    imports: [SHARED],
    templateUrl: './confirm-dialog.component.html',
})
export class ConfirmDialogComponent {
    private dialogRef = inject(MatDialogRef<ConfirmDialogComponent>);
    protected data = inject<IConfirmDialog>(MAT_DIALOG_DATA);

    protected close(result: boolean): void {
        this.dialogRef.close(result);
    }
}

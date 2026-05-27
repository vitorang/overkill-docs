import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SHARED } from '@shared/index';

export interface IConfirmModal {
    message: string;
}

@Component({
    selector: 'okd-confirm-modal',
    imports: [SHARED],
    templateUrl: './confirm-modal.component.html',
})
export class ConfirmModalComponent {
    private dialogRef = inject(MatDialogRef<ConfirmModalComponent>);
    protected data = inject<IConfirmModal>(MAT_DIALOG_DATA);

    protected close(result: boolean): void {
        this.dialogRef.close(result);
    }
}

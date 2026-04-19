import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { ProblemDetails } from '@core/models/problem-details.model';
import { AlertService } from '@shared/services/alert.service';
import { AuthService } from '@core/services/auth.service';
import { apiHandler } from '@core/utils/api-handler.utils';
import { AccountDeletion } from '@features/account/account.models';
import { AccountService } from '@features/account/services/account.service';
import { SHARED } from '@shared/index';
import { PATHS } from '@core/constants/routes.constant';

@Component({
    selector: 'okd-account-deletion-dialog',
    imports: [SHARED],
    templateUrl: './account-deletion-dialog.component.html',
})
export class AccountDeletionDialogComponent {
    private accountService = inject(AccountService);
    private authService = inject(AuthService);
    private alertService = inject(AlertService);
    private dialogRef = inject(MatDialogRef<AccountDeletionDialogComponent>);

    protected authHandler = apiHandler();
    protected passwordControl = new FormControl('', Validators.required);

    protected submit = (): void => {
        if (this.passwordControl.invalid || this.authHandler.loading()) return;

        const accountDeletion: AccountDeletion = {
            password: this.passwordControl.value ?? '',
        };

        this.authHandler.execute(
            this.accountService.deleteAccount(accountDeletion),
            () => this.onDeleteAccount(),
            (err: HttpErrorResponse) => {
                const problem = err.error as ProblemDetails | undefined;
                this.alertService.error(problem?.detail);
            },
        );
    };

    private onDeleteAccount() {
        this.authService.deleteToken();
        this.dialogRef.close();
        window.location.href = PATHS.ROOT;
    }
}

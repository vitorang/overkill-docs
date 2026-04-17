import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, output } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { ProblemDetails } from '@core/models/problem-details.model';
import { AlertService } from '@core/services/alert.service';
import { AuthService } from '@core/services/auth.service';
import { apiHandler } from '@core/utils/api-handler.utils';
import { FormUtils } from '@core/utils/form.utils';
import { PASSWORD_VALIDATORS } from '@features/account/constants/form-validators.constants';
import { PasswordChange } from '@features/account/models/password-change.model';
import { SHARED_NATIVE } from '@shared/index';

type PasswordChangeForm = FormGroup<{
    [K in keyof PasswordChange]: FormControl<PasswordChange[K]>
}>;

@Component({
    selector: 'okd-password-change-form',
    imports: [SHARED_NATIVE],
    templateUrl: './password-change-form.component.html',
    styleUrl: './password-change-form.component.scss',
})
export class PasswordChangeFormComponent {
    saved = output<void>();

    private formBuilder = inject(NonNullableFormBuilder);
    private authService = inject(AuthService);
    private alertService = inject(AlertService);
    protected authHandler = apiHandler();

    protected formGroup: PasswordChangeForm = this.formBuilder.group({
        currentPassword: ['', [Validators.required]],
        newPassword: ['', [...PASSWORD_VALIDATORS]],
    });


    protected onSubmit(): void {
        if (!this.formGroup.valid || this.authHandler.loading())
            return;

        const value: PasswordChange = this.formGroup.getRawValue();

        this.authHandler.execute(
            this.authService.changePassword(value),
            () => this.saved.emit(),
            (err: HttpErrorResponse) => {
                const problem = err.error as ProblemDetails | undefined;
                if (problem?.errors)
                    FormUtils.injectError(this.formGroup, problem.errors);
                else
                    this.alertService.error(problem?.detail);
            }
        )
    }

    protected get currentPasswordError(): string {
        return FormUtils.getFieldError(this.formGroup.controls.currentPassword);
    }

    protected get newPasswordError(): string {
        return FormUtils.getFieldError(this.formGroup.controls.newPassword);
    }
}

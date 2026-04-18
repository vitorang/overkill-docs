import { Component, inject, output } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { SHARED } from '@shared/index';
import { AlertService } from '@core/services/alert.service';
import { apiHandler } from '@core/utils/api-handler.utils';
import { ProblemDetails } from '@core/models/problem-details.model';
import { FormUtils } from '@core/utils/form.utils';
import { PASSWORD_VALIDATORS } from '@features/account/constants/form-validators.constants';
import { AccountService } from '@features/account/services/account.service';
import { AuthStorageMode } from '@core/constants/auth.constants';
import { AuthRequest } from '@features/account/account.models';
import { BrandComponent } from "@shared/components/brand/brand.component";


interface AuthFormData extends AuthRequest {
    storage: AuthStorageMode;
}

type LoginForm = FormGroup<{
    [K in keyof AuthFormData]: FormControl<AuthFormData[K]>
}>;

@Component({
    selector: 'okd-auth-form',
    imports: [SHARED, BrandComponent],
    templateUrl: './auth-form.component.html',
    styleUrl: './auth-form.component.scss',
})
export class AuthFormComponent {
    authSuccess = output<void>();

    private formBuilder = inject(NonNullableFormBuilder);
    private accountService = inject(AccountService);
    private alertService = inject(AlertService);
    protected authHandler = apiHandler();

    protected readonly AuthStorage = AuthStorageMode;
    protected isLogin = true;

    protected loginForm: LoginForm = this.formBuilder.group({
        username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15)]],
        password: ['', [...PASSWORD_VALIDATORS]],
        userAgent: [navigator.userAgent],
        storage: [AuthStorageMode.LocalStorage]
    });

    protected onSubmit(): void {
        if (!this.loginForm.valid || this.authHandler.loading())
            return;

        const value = this.loginForm.getRawValue();
        const storage: AuthStorageMode = value.storage;
        const data: AuthRequest = {
            username: value.username,
            password: value.password,
            userAgent: value.userAgent
        };

        const request = this.isLogin
            ? this.accountService.login(data, storage)
            : this.accountService.register(data, storage);

        this.authHandler.execute(
            request,
            () => this.authSuccess.emit(),
            (err: HttpErrorResponse) => {
                const problem = err.error as ProblemDetails | undefined;
                if (problem?.errors)
                    FormUtils.injectError(this.loginForm, problem.errors);
                else
                    this.alertService.error(problem?.detail);
            }
        )
    }

    protected get usernameError(): string {
        return FormUtils.getFieldError(this.loginForm.controls.username);
    }

    protected get passwordError(): string {
        return FormUtils.getFieldError(this.loginForm.controls.password);
    }
}

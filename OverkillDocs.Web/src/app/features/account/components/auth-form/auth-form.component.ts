import { Component, computed, EventEmitter, inject, Output } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { AuthRequest, AuthStorageMode } from '../../../../core/models/auth.model';
import { SHARED_CUSTOM, SHARED_NATIVE } from '../../../../shared';
import { AuthService } from '../../../../core/services/auth.service';
import { FormUtils } from '../../../../core/utils/form.utils';
import { HttpErrorResponse } from '@angular/common/http';
import { AlertService } from '../../../../core/services/alert.service';
import { ProblemDetails } from '../../../../core/models/problem-details.mode';
import { RequestState } from '../../../../core/models/common.model';

interface AuthFormData extends AuthRequest {
    storage: AuthStorageMode;
}

@Component({
    selector: 'okd-auth-form',
    imports: [SHARED_NATIVE, SHARED_CUSTOM],
    templateUrl: './auth-form.component.html',
    styleUrl: './auth-form.component.scss',
})
export class AuthFormComponent {
    @Output() authSuccess = new EventEmitter<void>();

    private formBuilder = inject(NonNullableFormBuilder);
    private authService = inject(AuthService);
    private alertService = inject(AlertService);

    protected loginForm: FormGroup<{
        [K in keyof AuthFormData]: FormControl<AuthFormData[K]>;
    }>;
    protected readonly AuthStorage = AuthStorageMode;
    protected isLogin = true;

    readonly isLoading = computed(() => this.authService.loginState() === RequestState.LOADING);

    constructor() {
        this.loginForm = this.formBuilder.group({
            username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15)]],
            password: ['', [Validators.required, Validators.minLength(3)]],
            userAgent: [navigator.userAgent],
            storage: [AuthStorageMode.LocalStorage]
        });
    }

    onSubmit(): void {
        if (!this.loginForm.valid)
            return;

        const value = this.loginForm.getRawValue();
        const storage: AuthStorageMode = value.storage;
        const data: AuthRequest = {
            username: value.username,
            password: value.password,
            userAgent: value.userAgent
        };

        const request = this.isLogin
            ? this.authService.login(data, storage)
            : this.authService.register(data, storage);

        request.subscribe({
            next: () => {
                this.authSuccess.emit();
            },
            error: (err: HttpErrorResponse) => {
                const problem = err.error as ProblemDetails | undefined;
                if (problem?.errors)
                    FormUtils.injectError(this.loginForm, problem.errors);
                else
                    this.alertService.error(problem?.detail);
            }
        });
    }

    get usernameError(): string {
        return FormUtils.getFieldError(this.loginForm.controls.username);
    }

    get passwordError(): string {
        return FormUtils.getFieldError(this.loginForm.controls.password);
    }
}

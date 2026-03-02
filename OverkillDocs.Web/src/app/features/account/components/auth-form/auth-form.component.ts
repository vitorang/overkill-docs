import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { AuthRequest, AuthStorageMode } from '../../../../core/models/auth.model';
import { SHARED_CUSTOM, SHARED_NATIVE } from '../../../../shared';

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
    private formBuilder = inject(NonNullableFormBuilder);
    protected loginForm: FormGroup<{
        [K in keyof AuthFormData]: FormControl<AuthFormData[K]>;
    }>;
    protected readonly AuthStorage = AuthStorageMode;
    protected isLogin = true;

    constructor() {
        this.loginForm = this.formBuilder.group({
            username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15)]],
            password: ['', [Validators.required, Validators.minLength(3)]],
            userAgent: [navigator.userAgent],
            storage: [AuthStorageMode.LocalStorage]
        });
    }

    onSubmit(): void {
        if (this.loginForm.valid) {
            const data: AuthRequest = this.loginForm.getRawValue();
            console.log(data);
        }
    }
}

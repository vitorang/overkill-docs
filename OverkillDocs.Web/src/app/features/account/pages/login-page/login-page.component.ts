import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { PATHS } from '@core/constants/routes.constant';
import { AuthFormWrapperComponent } from '@features/account/components/auth-form-wrapper/auth-form-wrapper.component';
import { AuthFormComponent } from '@features/account/components/forms/auth-form/auth-form.component';
import { SHARED_CUSTOM, SHARED_NATIVE } from '@shared/index';


@Component({
    selector: 'okd-login-page',
    imports: [SHARED_NATIVE, SHARED_CUSTOM, AuthFormComponent, AuthFormWrapperComponent],
    templateUrl: './login-page.component.html',
    styleUrl: './login-page.component.scss',
})
export class LoginPageComponent {
    private router = inject(Router);

    protected onAuthSuccess(): void {
        this.router.navigateByUrl(PATHS.ROOT);
    }
}

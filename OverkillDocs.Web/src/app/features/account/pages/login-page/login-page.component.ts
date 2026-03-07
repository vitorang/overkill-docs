import { Component, inject } from '@angular/core';
import { SHARED_CUSTOM, SHARED_NATIVE } from '../../../../shared';
import { AuthFormComponent } from '../../components/auth-form/auth-form.component';
import { AuthFormWrapperComponent } from '../../components/auth-form-wrapper/auth-form-wrapper.component';
import { Router } from '@angular/router';
import { PATHS } from '../../../../core/constants/routes.constant';

@Component({
    selector: 'okd-login-page',
    imports: [SHARED_NATIVE, SHARED_CUSTOM, AuthFormComponent, AuthFormWrapperComponent],
    templateUrl: './login-page.component.html',
    styleUrl: './login-page.component.scss',
})
export class LoginPageComponent {
    private router = inject(Router);

    protected onAuthSuccess(): void {
        this.router.navigate([PATHS.ROOT]);
    }
}

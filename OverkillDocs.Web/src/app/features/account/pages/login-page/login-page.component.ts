import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { PATHS } from '@core/constants/routes.constant';
import { AuthFormWrapperComponent } from '@features/account/components/auth-form-wrapper/auth-form-wrapper.component';
import { AuthFormComponent } from '@features/account/components/forms/auth-form/auth-form.component';
import { SHARED } from '@shared/index';
import { BackgroundImageComponent } from "@shared/components/background-image/background-image.component";


@Component({
    selector: 'okd-login-page',
    imports: [SHARED, AuthFormComponent, AuthFormWrapperComponent, BackgroundImageComponent],
    templateUrl: './login-page.component.html',
    styleUrl: './login-page.component.scss',
})
export class LoginPageComponent {
    private router = inject(Router);

    protected onAuthSuccess(): void {
        this.router.navigateByUrl(PATHS.ROOT);
    }
}

import { Component, inject } from '@angular/core';
import { SHARED_NATIVE } from '../..';
import { BrandComponent } from '../brand/brand.component';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { PATHS } from '../../../core/constants/routes.constant';

@Component({
    selector: 'okd-main-header',
    imports: [SHARED_NATIVE, BrandComponent],
    templateUrl: './main-header.component.html',
    styleUrl: './main-header.component.scss',
})
export class MainHeaderComponent {
    protected authService = inject(AuthService);
    protected router = inject(Router);

    protected logout(): void {
        this.authService.logout().subscribe(
            {
                next: () => this.router.navigateByUrl(PATHS.ACCOUNT.LOGIN)
            }
        );
    }

    protected goToSettings(): void {
        this.router.navigateByUrl(PATHS.ACCOUNT.SETTINGS);
    }
}

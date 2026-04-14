import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { PATHS } from '@core/constants/routes.constant';
import { AuthService } from '@core/services/auth.service';
import { BrandComponent } from '@shared/components/brand/brand.component';
import { SHARED_NATIVE } from '@shared/index';

@Component({
    selector: 'okd-main-header',
    imports: [SHARED_NATIVE, BrandComponent],
    templateUrl: './main-header.component.html',
    styleUrl: './main-header.component.scss',
})
export class MainHeaderComponent {
    private authService = inject(AuthService);
    private router = inject(Router);

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

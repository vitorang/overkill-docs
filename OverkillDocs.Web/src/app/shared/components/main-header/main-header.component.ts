import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { PATHS } from '@core/constants/routes.constant';
import { BrandComponent } from '@shared/components/brand/brand.component';
import { SHARED } from '@shared/index';
import { ClearButtonDirective } from "@shared/directives/clear-button.directive";
import { AccountService } from '@features/account/services/account.service';

@Component({
    selector: 'okd-main-header',
    imports: [SHARED, BrandComponent, ClearButtonDirective],
    templateUrl: './main-header.component.html',
    styleUrl: './main-header.component.scss',
})
export class MainHeaderComponent {
    private accountService = inject(AccountService);
    private router = inject(Router);

    protected logout(): void {
        this.accountService.logout().subscribe(() => this.router.navigateByUrl(PATHS.ACCOUNT.LOGIN));
    }

    protected goToSettings(): void {
        this.router.navigateByUrl(PATHS.ACCOUNT.SETTINGS);
    }

    protected goToHome(): void {
        this.router.navigateByUrl(PATHS.ROOT);
    }
}

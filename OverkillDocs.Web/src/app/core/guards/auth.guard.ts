import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PATHS } from '@core/constants/routes.constant';
import { AuthService } from '@core/services/auth.service';

export const authGuard: CanActivateFn = (_route, _state) => {
    const authService = inject(AuthService);

    if (authService.token()) return true;

    return inject(Router).parseUrl(PATHS.ACCOUNT.LOGIN);
};

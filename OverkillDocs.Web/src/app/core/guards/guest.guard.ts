import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { PATHS } from '@core/constants/routes.constant';
import { AuthService } from '@core/services/auth.service';

export const guestGuard: CanActivateFn = (_route, _state) => {
    const authService = inject(AuthService);

    if (authService.token()) return inject(Router).parseUrl(PATHS.ROOT);

    return true;
};

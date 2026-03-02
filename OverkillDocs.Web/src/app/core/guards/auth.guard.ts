import { inject } from "@angular/core";
import { AuthService } from "../services/auth.service";
import { CanActivateFn, Router } from "@angular/router";
import { PATHS } from "../constants/routes.constant";

export const authGuard: CanActivateFn = (_route, _state) => {
    const authService = inject(AuthService);

    if (authService.token())
        return true;

    return inject(Router).parseUrl(PATHS.ACCOUNT.LOGIN);
};
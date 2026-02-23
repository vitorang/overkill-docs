import { inject } from "@angular/core";
import { AuthService } from "../services/auth.service";
import { NavigationService } from "../services/navigation.service";
import { CanActivateFn } from "@angular/router";
import { PATHS } from "../constants/routes.constant";

export const authGuard: CanActivateFn = (_route, _state) => {
    const authService = inject(AuthService);
    const navService = inject(NavigationService);
    
    if (authService.token())
        return true;
    
    navService.navigate(PATHS.ACCOUNT.LOGIN);
    return false;
};
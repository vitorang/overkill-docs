import { CanActivateFn, Router } from "@angular/router";
import { PATHS } from "../constants/routes.constant";
import { inject } from "@angular/core";
import { AuthService } from "../services/auth.service";

export const guestGuard: CanActivateFn = (_route, _state) => {
    const authService = inject(AuthService);
    
    if (authService.token())
        return  inject(Router).parseUrl(PATHS.HOME); 
    
    return true;
};
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { NavigationService } from '../services/navigation.service';
import { PATHS } from '../constants/routes.constant';


export const authInterceptor: HttpInterceptorFn = (request, next) => {
    const authService = inject(AuthService);
    const navigationService = inject(NavigationService);
    const token = authService.token();
    
    let authRequest = request;
    if (token) {
        authRequest = request.clone({
            setHeaders: { Authorization: `Bearer ${token}` }
        });
    }
    
    return next(authRequest).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
                authService.deleteToken();
                navigationService.navigate(PATHS.ACCOUNT.LOGIN);
            }
            return throwError(() => error);
        })
    );
};
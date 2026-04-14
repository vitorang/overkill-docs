import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '@core/services/auth.service';
import { PATHS } from '@core/constants/routes.constant';


export const authInterceptor: HttpInterceptorFn = (request, next) => {
    const authService = inject(AuthService);
    const router = inject(Router);
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
                router.navigateByUrl(PATHS.ACCOUNT.LOGIN);
            }
            return throwError(() => error);
        })
    );
};
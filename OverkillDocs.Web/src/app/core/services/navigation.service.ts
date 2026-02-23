import { inject, Injectable } from '@angular/core';
import { Router, Params } from '@angular/router';
import { AUTH } from '../constants/auth.constant';

@Injectable({ providedIn: 'root' })
export class NavigationService {
    private readonly router = inject(Router);
    
    get authToken(): string | null {
        return this.router.routerState.snapshot.root.queryParamMap.get(AUTH.TOKEN);
    }
    
    set authToken(value: string | null) {
        this.router.navigate([], {
            queryParams: { [AUTH.TOKEN]: value || null },
            queryParamsHandling: 'merge',
            replaceUrl: true,
        });
    }
    
    navigate(path: string, queryParams: Params = {}): void {
        const currentToken = this.authToken;
        
        this.router.navigate([path], {
            queryParams: {
                ...(currentToken ? { [AUTH.TOKEN]: currentToken } : {}),
                ...queryParams
            }
        });
    }
}
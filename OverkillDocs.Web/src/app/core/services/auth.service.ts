import { inject, Injectable, signal } from '@angular/core';
import { UserService } from '@core/services/user.service';
import { AUTH, AuthStorageMode } from '@core/constants/auth.constants';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private userService = inject(UserService);

    token = signal<string | null>(this.getToken());

    saveToken(token: string, storage: AuthStorageMode): void {
        this.token.set(token);
        if (storage === AuthStorageMode.LocalStorage) localStorage.setItem(AUTH.TOKEN, token);
        else if (storage === AuthStorageMode.SessionStorage)
            sessionStorage.setItem(AUTH.TOKEN, token);
    }

    getToken(): string | null {
        return sessionStorage.getItem(AUTH.TOKEN) || localStorage.getItem(AUTH.TOKEN);
    }

    deleteToken(): void {
        this.token.set(null);
        sessionStorage.removeItem(AUTH.TOKEN);
        localStorage.removeItem(AUTH.TOKEN);
        this.userService.currentUser.set(null);
    }
}

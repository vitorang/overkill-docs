import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';
import { UserService } from '@core/services/user.service';
import { API } from '@core/constants/api.constants';
import { AUTH } from '@core/constants/auth.constant';
import { UserSession } from '@features/account/models/user-session.model';
import { parseUserAgent } from '@core/utils/browser.utils';
import { AuthRequest, AuthResponse, AuthStorageMode } from '@features/account/models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private http = inject(HttpClient);
    private userService = inject(UserService);

    token = signal<string | null>(this.getToken());

    private authRequest<T>(observable: Observable<T>, onSuccess: (result: T) => void): Observable<T> {
        return observable.pipe(
            tap({
                next: (response) => {
                    onSuccess(response);
                }
            }),
        );
    }

    login(credentials: AuthRequest, storage: AuthStorageMode): Observable<AuthResponse> {
        return this.authRequest(this.http.post<AuthResponse>(API.ACCOUNT.LOGIN, credentials), result => this.saveToken(result.token, storage));
    }

    logout(): Observable<void> {
        return this.authRequest(this.http.post<void>(API.ACCOUNT.LOGOUT, {}), () => this.deleteToken());
    }

    logoutById(sessionHashId: string): Observable<void> {
        return this.http.post<void>(API.ACCOUNT.LOGOUT_BY_ID(sessionHashId), {});
    }

    register(credentials: AuthRequest, storage: AuthStorageMode): Observable<AuthResponse> {
        return this.authRequest(this.http.post<AuthResponse>(API.ACCOUNT.REGISTER, credentials), result => this.saveToken(result.token, storage));
    }

    private saveToken(token: string, storage: AuthStorageMode) {
        this.token.set(token);
        if (storage === AuthStorageMode.LocalStorage)
            localStorage.setItem(AUTH.TOKEN, token);
        else if (storage === AuthStorageMode.SessionStorage)
            sessionStorage.setItem(AUTH.TOKEN, token);
    }

    private getToken() {
        return sessionStorage.getItem(AUTH.TOKEN) || localStorage.getItem(AUTH.TOKEN);
    }

    deleteToken(): void {
        this.token.set(null);
        sessionStorage.removeItem(AUTH.TOKEN);
        localStorage.removeItem(AUTH.TOKEN);
        this.userService.currentUser.set(null);
    }

    listSessions(): Observable<UserSession[]> {
        return this.http.get<UserSession[]>(API.ACCOUNT.SESSIONS).pipe(
            map(sessions => sessions.map(session => {
                return {
                    ...session,
                    ...parseUserAgent(session.userAgent)
                }
            }))
        );
    }
}
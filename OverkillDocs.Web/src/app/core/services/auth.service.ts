import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthRequest, AuthResponse, AuthStorageMode } from '../../features/account/models/auth.model';
import { RequestState } from '../models/common.model';
import { finalize, Observable, tap } from 'rxjs';
import { AUTH } from '../constants/auth.constant';
import { API } from '../constants/api.constants';
import { UserService } from './user.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private http = inject(HttpClient);
    private userService = inject(UserService);

    token = signal<string | null>(this.getToken());
    loginState = signal<RequestState>(RequestState.IDLE);

    private authRequest<T>(observable: Observable<T>, onSuccess: (result: T) => void): Observable<T> {
        this.loginState.set(RequestState.LOADING);
        return observable.pipe(
            tap({
                next: (response) => {
                    onSuccess(response);
                    this.loginState.set(RequestState.SUCCESS);
                }
            }),
            finalize(() => {
                if (this.loginState() !== RequestState.SUCCESS)
                    this.loginState.set(RequestState.IDLE);
            })
        );
    }

    login(credentials: AuthRequest, storage: AuthStorageMode): Observable<AuthResponse> {
        return this.authRequest(this.http.post<AuthResponse>(API.ACCOUNT.LOGIN, credentials), result => this.saveToken(result.token, storage));
    }

    logout(): Observable<void> {
        return this.authRequest(this.http.post<void>(API.ACCOUNT.LOGOUT, {}), () => this.deleteToken());
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
}
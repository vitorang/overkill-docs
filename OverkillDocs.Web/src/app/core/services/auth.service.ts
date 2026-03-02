import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthRequest, AuthResponse, AuthStorageMode } from '../models/auth.model';
import { RequestState } from '../models/common.model';
import { finalize, Observable, tap } from 'rxjs';
import { AUTH } from '../constants/auth.constant';
import { API } from '../constants/api.constants';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private http = inject(HttpClient);

    token = signal<string | null>(this.getToken());
    loginState = signal<RequestState>(RequestState.IDLE);

    login(credentials: AuthRequest, storage: AuthStorageMode): Observable<AuthResponse> {
        this.loginState.set(RequestState.LOADING);

        return this.http.post<AuthResponse>(API.ACCOUNT.LOGIN, credentials).pipe(
            tap({
                next: (response) => {
                    this.saveToken(response.token, storage);
                    this.loginState.set(RequestState.SUCCESS);
                },
                error: () => {
                    this.loginState.set(RequestState.ERROR);
                }
            }),
            finalize(() => {
                if (this.loginState() === RequestState.LOADING) {
                    this.loginState.set(RequestState.IDLE);
                }
            })
        );
    }

    /*logout(): Observable<void>{
        return this.http.post(this.url('logout'), {});
    }
    
    register(credentials: AuthRequest, storage: AuthStorageMode): Observable<AuthResponse> {
       
    }*/

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
    }
}
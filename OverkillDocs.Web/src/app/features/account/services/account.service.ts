import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { API } from "@core/constants/api.constants";
import { AuthStorageMode } from "@core/constants/auth.constants";
import { AuthService } from "@core/services/auth.service";
import { AccountDeletion, AuthRequest, AuthResponse } from "@features/account/account.models";
import { Observable, tap } from "rxjs";

@Injectable({ providedIn: 'root' })
export class AccountService {
    private http = inject(HttpClient);
    private authService = inject(AuthService);

    private authRequest<T>(observable: Observable<T>, onSuccess: (result: T) => void): Observable<T> {
        return observable.pipe(
            tap({
                next: (response) => {
                    onSuccess(response);
                }
            }),
        );
    }

    deleteAccount(accountDeletion: AccountDeletion): Observable<void> {
        return this.http.post<void>(API.ACCOUNT.DELETE_ACCOUNT, accountDeletion);
    }

    login(credentials: AuthRequest, storage: AuthStorageMode): Observable<AuthResponse> {
        return this.authRequest(this.http.post<AuthResponse>(API.ACCOUNT.LOGIN, credentials), result => this.authService.saveToken(result.token, storage));
    }

    logout(): Observable<void> {
        return this.authRequest(this.http.post<void>(API.ACCOUNT.LOGOUT, {}), () => this.authService.deleteToken());
    }

    logoutById(sessionHashId: string): Observable<void> {
        return this.http.post<void>(API.ACCOUNT.LOGOUT_BY_ID(sessionHashId), {});
    }

    register(credentials: AuthRequest, storage: AuthStorageMode): Observable<AuthResponse> {
        return this.authRequest(this.http.post<AuthResponse>(API.ACCOUNT.REGISTER, credentials), result => this.authService.saveToken(result.token, storage));
    }
}
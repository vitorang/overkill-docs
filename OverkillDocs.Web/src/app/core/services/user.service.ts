import { catchError, finalize, map, Observable, of, shareReplay, tap } from 'rxjs';
import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SimpleUser } from '@core/models/user.model';
import { AlertService } from '@shared/services/alert.service';
import { API } from '@core/constants/api.constants';

@Injectable({ providedIn: 'root' })
export class UserService {
    readonly currentUser = signal(null as SimpleUser | null);

    private cache: Record<string, SimpleUser> = {};
    private requests: Record<string, Observable<SimpleUser | null>> = {};
    private http = inject(HttpClient);
    private alertService = inject(AlertService);

    public loadCurrentUser(reload?: boolean): Observable<boolean> {
        if (this.currentUser() && !reload) return of(true);

        return this.loadUser(API.USER.CURRENT).pipe(
            tap((user) => this.currentUser.set(user)),
            map((user) => !!user),
        );
    }

    public getUser(hashId: string): Observable<SimpleUser | null> {
        if (this.cache[hashId]) return of(this.cache[hashId]);

        return this.loadUser(API.USER.BY_ID(hashId));
    }

    private loadUser(url: string): Observable<SimpleUser | null> {
        if (!this.requests[url]) {
            this.requests[url] = this.http.get<SimpleUser>(url).pipe(
                tap((user) => {
                    if (user) this.cache[user.hashId] = user;
                }),
                shareReplay(1),
                finalize(() => delete this.requests[url]),
                catchError(() => {
                    this.alertService.error('Erro ao carregar usuário');
                    return of(null);
                }),
            );
        }

        return this.requests[url];
    }
}

import { catchError, finalize, Observable, of, shareReplay, tap } from 'rxjs';
import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SimpleUser } from '@core/models/user.model';
import { API } from '@core/constants/api.constants';

@Injectable()
export class UserService {
    readonly currentUser = signal<SimpleUser | null>(null);

    private cache: Record<string, SimpleUser> = {};
    private requests: Record<string, Observable<SimpleUser | null>> = {};
    private http = inject(HttpClient);

    defaultUser: SimpleUser = {
        avatar: '',
        hashId: '',
        name: '...',
    } as const;

    loadCurrentUser(reload?: boolean): Observable<SimpleUser | null> {
        if (this.currentUser() && !reload) {
            return of(this.currentUser());
        }

        return this.loadUser(API.USER.CURRENT).pipe(tap((user) => this.currentUser.set(user)));
    }

    getUser(hashId: string): Observable<SimpleUser | null> {
        if (this.cache[hashId]) {
            return of(this.cache[hashId]);
        }

        return this.loadUser(API.USER.BY_ID(hashId));
    }

    private loadUser(url: string): Observable<SimpleUser | null> {
        if (!this.requests[url]) {
            this.requests[url] = this.http.get<SimpleUser>(url).pipe(
                tap((user) => {
                    if (user) {
                        this.cache[user.hashId] = user;
                    }
                }),
                shareReplay(1),
                finalize(() => delete this.requests[url]),
                catchError(() => of(null)),
            );
        }

        return this.requests[url];
    }
}

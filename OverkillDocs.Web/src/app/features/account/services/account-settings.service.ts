import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API } from '@core/constants/api.constants';
import { parseUserAgent } from '@core/utils/browser.utils';
import { PasswordChange, Profile, UserSession } from '@features/account/account.models';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AccountSettingsService {
    private http = inject(HttpClient);

    changePassword(credentials: PasswordChange): Observable<void> {
        return this.http.post<void>(API.ACCOUNT.CHANGE_PASSWORD, credentials);
    }

    listSessions(): Observable<UserSession[]> {
        return this.http.get<UserSession[]>(API.ACCOUNT.SESSIONS).pipe(
            map((sessions) =>
                sessions.map((session) => {
                    return {
                        ...session,
                        ...parseUserAgent(session.userAgent),
                    };
                }),
            ),
        );
    }

    loadProfile = (): Observable<Profile> => {
        return this.http.get<Profile>(API.ACCOUNT.PROFILE);
    };

    updateProfile = (profile: Profile): Observable<Profile> => {
        return this.http.post<Profile>(API.ACCOUNT.PROFILE, profile);
    };
}

import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { API } from "@core/constants/api.constants";
import { Profile } from "@features/account/models/profile.model";
import { Observable } from "rxjs";

@Injectable({ providedIn: 'root' })
export class ProfileService {
    private http = inject(HttpClient);

    load = (): Observable<Profile> => {
        return this.http.get<Profile>(API.ACCOUNT.PROFILE);

    }

    update = (profile: Profile): Observable<Profile> => {
        return this.http.post<Profile>(API.ACCOUNT.PROFILE, profile);
    }
}
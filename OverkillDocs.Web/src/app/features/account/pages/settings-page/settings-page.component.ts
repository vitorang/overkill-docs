import { Component, inject, signal, OnInit } from '@angular/core';
import { AuthService } from '@core/services/auth.service';
import { apiHandler } from '@core/utils/api-handler.utils';
import { Profile } from '@features/account/models/profile.model';
import { UserSession } from '@features/account/models/user-session.model';
import { ProfileService } from '@features/account/services/profile.service';
import { SHARED_CUSTOM, SHARED_NATIVE } from '@shared/index';
import { ProfileFormComponent } from "@features/account/components/forms/profile-form/profile-form.component";
import { AlertService } from '@core/services/alert.service';
import { PasswordChangeFormComponent } from "@features/account/components/forms/password-change-form/password-change-form.component";

@Component({
    selector: 'okd-settings-page',
    imports: [SHARED_NATIVE, SHARED_CUSTOM, ProfileFormComponent, PasswordChangeFormComponent],
    templateUrl: './settings-page.component.html',
    styleUrl: './settings-page.component.scss',
})
export class SettingsPageComponent implements OnInit {
    private alertService = inject(AlertService);
    private authService = inject(AuthService);
    private profileService = inject(ProfileService);

    protected profileHandler = apiHandler();
    protected profile = signal<Profile>({
        avatar: '',
        name: '',
        username: ''
    });

    protected sessionsHandler = apiHandler();
    protected sessions = signal<UserSession[]>([]);

    ngOnInit(): void {
        this.loadProfile();
        this.loadSessions();
    }

    protected loadProfile = (): void => {
        this.profileHandler.execute(
            this.profileService.load(),
            result => this.profile.set(result)
        );
    }

    protected loadSessions = (): void => {
        this.sessionsHandler.execute(
            this.authService.listSessions(),
            result => this.sessions.set(result)
        );
    }

    protected logout = (session: UserSession): void => {
        this.authService.logoutById(session.hashId).subscribe(() => this.loadSessions());
    }

    protected onProfileSaved = (value: Profile): void => {
        this.profile.set(value);
        this.alertSuccess();
    }

    protected alertSuccess(): void {
        this.alertService.info('Alterações salvas!')
    }
}

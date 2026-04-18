import { Component, inject, signal, OnInit } from '@angular/core';
import { apiHandler } from '@core/utils/api-handler.utils';
import { SHARED } from '@shared/index';
import { ProfileFormComponent } from '@features/account/components/forms/profile-form/profile-form.component';
import { AlertService } from '@shared/services/alert.service';
import { PasswordChangeFormComponent } from '@features/account/components/forms/password-change-form/password-change-form.component';
import { MatDialog } from '@angular/material/dialog';
import { AccountDeletionDialogComponent } from '@features/account/components/account-deletion-dialog/account-deletion-dialog.component';
import { AccountService } from '@features/account/services/account.service';
import { Profile, UserSession } from '@features/account/account.models';
import { AccountSettingsService } from '@features/account/services/account-settings.service';
import { RequestOverlayComponent } from '@shared/components/request-overlay/request-overlay.component';
import { MainHeaderComponent } from '@shared/components/main-header/main-header.component';

@Component({
    selector: 'okd-settings-page',
    imports: [
        SHARED,
        ProfileFormComponent,
        PasswordChangeFormComponent,
        RequestOverlayComponent,
        MainHeaderComponent,
    ],
    templateUrl: './settings-page.component.html',
    styleUrl: './settings-page.component.scss',
})
export class SettingsPageComponent implements OnInit {
    private alertService = inject(AlertService);
    private accountService = inject(AccountService);
    private accountSettingsService = inject(AccountSettingsService);
    private dialog = inject(MatDialog);

    protected profileHandler = apiHandler();
    protected profile = signal<Profile>({
        avatar: '',
        name: '',
        username: '',
    });

    protected sessionsHandler = apiHandler();
    protected sessions = signal<UserSession[]>([]);

    protected logoutHandler = apiHandler();

    ngOnInit(): void {
        this.loadProfile();
        this.loadSessions();
    }

    protected loadProfile(): void {
        this.profileHandler.execute(this.accountSettingsService.loadProfile(), (result) =>
            this.profile.set(result),
        );
    }

    protected loadSessions(): void {
        this.sessionsHandler.execute(this.accountSettingsService.listSessions(), (result) =>
            this.sessions.set(result),
        );
    }

    protected logout(session: UserSession): void {
        const refreshSessions = (): void => {
            this.logoutHandler.execute(this.accountSettingsService.listSessions(), (result) =>
                this.sessions.set(result),
            );
        };

        this.logoutHandler.execute(this.accountService.logoutById(session.hashId), () =>
            refreshSessions(),
        );
    }

    protected onProfileSaved(value: Profile): void {
        this.profile.set(value);
        this.alertSuccess();
    }

    protected alertSuccess(): void {
        this.alertService.info('Alterações salvas!');
    }

    protected showAccountDeletionDialog(): void {
        this.dialog.open(AccountDeletionDialogComponent, {
            width: '450px',
            autoFocus: 'input',
            disableClose: true,
        });
    }
}

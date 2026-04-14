import { Component, inject, signal, OnInit } from '@angular/core';
import { SHARED_CUSTOM, SHARED_NATIVE } from '../../../../shared';
import { UserSession } from '../../models/user-session.model';
import { AuthService } from '../../../../core/services/auth.service';
import { httpHandler } from '../../../../core/utils/http-handler.utils';

@Component({
    selector: 'okd-settings-page',
    imports: [SHARED_NATIVE, SHARED_CUSTOM],
    templateUrl: './settings-page.component.html',
    styleUrl: './settings-page.component.scss',
})
export class SettingsPageComponent implements OnInit {
    protected profileHandler = httpHandler();
    protected profile = {};

    protected sessionsHandler = httpHandler();
    protected sessions = signal<UserSession[]>([]);

    private authService = inject(AuthService);

    ngOnInit(): void {
        this.loadProfile();
        this.loadSessions();
    }

    protected loadProfile = (): void => {
        console.log('prof');
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

}

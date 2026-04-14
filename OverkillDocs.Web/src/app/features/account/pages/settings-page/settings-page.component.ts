import { Component, DestroyRef, inject, signal, OnInit } from '@angular/core';
import { SHARED_CUSTOM, SHARED_NATIVE } from '../../../../shared';
import { RequestState } from '../../../../core/models/common.model';
import { UserSession } from '../../models/user-session.model';
import { AuthService } from '../../../../core/services/auth.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, throwError } from 'rxjs';

@Component({
    selector: 'okd-settings-page',
    imports: [SHARED_NATIVE, SHARED_CUSTOM],
    templateUrl: './settings-page.component.html',
    styleUrl: './settings-page.component.scss',
})
export class SettingsPageComponent implements OnInit {
    protected RequestState = RequestState;

    protected profileState = signal(RequestState.LOADING);
    protected profile = {};

    protected sessionsState = signal(RequestState.ERROR);
    protected sessions = signal<UserSession[]>([]);

    private destroyRef = inject(DestroyRef);
    private authService = inject(AuthService);

    ngOnInit(): void {
        this.loadProfile();
        this.loadSessions();
    }

    protected loadProfile = (): void => {
        console.log('prof');
    }

    protected loadSessions = (): void => {
        this.sessionsState.set(RequestState.LOADING);

        this.authService.listSessions().pipe(
            takeUntilDestroyed(this.destroyRef),
            catchError(error => {
                this.sessionsState.set(RequestState.ERROR);
                return throwError(() => error);
            })
        ).subscribe(result => {
            this.sessions.set(result);
            this.sessionsState.set(RequestState.SUCCESS);
        });
    }

    protected logout = (session: UserSession): void => {
        this.authService.logoutById(session.hashId).subscribe(() => this.loadSessions());
    }

}

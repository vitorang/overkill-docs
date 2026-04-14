import { Routes } from "@angular/router";

import { SEGMENTS } from "@core/constants/routes.constant";
import { guestGuard } from "@core/guards/guest.guard";
import { authGuard } from "@core/guards/auth.guard";
import { LoginPageComponent } from "@features/account/pages/login-page/login-page.component";
import { SettingsPageComponent } from "@features/account/pages/settings-page/settings-page.component";

const S = SEGMENTS.ACCOUNT;
export const ACCOUNT_ROUTES: Routes = [
    { path: S.LOGIN, component: LoginPageComponent, canActivate: [guestGuard] },
    { path: S.SETTINGS, component: SettingsPageComponent, canActivate: [authGuard] },
];
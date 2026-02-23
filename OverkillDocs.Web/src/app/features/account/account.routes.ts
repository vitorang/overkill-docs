import { Routes } from "@angular/router";
import { LoginPageComponent } from "./pages/login-page/login-page.component";
import { RegisterPageComponent } from "./pages/register-page/register-page.component";
import { SettingsPageComponent } from "./pages/settings-page/settings-page.component";
import { SEGMENTS } from "../../core/constants/routes.constant";
import { guestGuard } from "../../core/guards/guest.guard";
import { authGuard } from "../../core/guards/auth.guard";

const S = SEGMENTS.ACCOUNT;
export const ACCOUNT_ROUTES: Routes = [
    { path: S.LOGIN, component: LoginPageComponent, canActivate: [guestGuard] },
    { path: S.REGISTER, component: RegisterPageComponent, canActivate: [guestGuard] },
    { path: S.SETTINGS, component: SettingsPageComponent, canActivate: [authGuard]},
];
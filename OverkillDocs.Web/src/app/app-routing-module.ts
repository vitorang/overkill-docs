import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SEGMENTS } from '@core/constants/routes.constant';
import { authGuard } from '@core/guards/auth.guard';
import { NotFoundPageComponent } from '@features/error/pages/not-found-page/not-found-page.component';


const S = SEGMENTS;
const routes: Routes = [
    { path: '', redirectTo: S.DOCUMENT.ROOT, pathMatch: 'full' },
    {
        path: S.ACCOUNT.ROOT,
        loadChildren: () => import('./features/account/account.routes').then(r => r.ACCOUNT_ROUTES)
    },
    {
        path: S.DOCUMENT.ROOT,
        loadChildren: () => import('./features/document/document.routes').then(r => r.DOCUMENT_ROUTES),
        canActivate: [authGuard]
    },
    { path: '**', component: NotFoundPageComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }



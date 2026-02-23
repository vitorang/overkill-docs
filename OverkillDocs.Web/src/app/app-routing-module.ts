import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundPageComponent } from './features/error/pages/not-found-page/not-found-page.component';
import { SEGMENTS } from './core/constants/routes.constant';

const S = SEGMENTS;
const routes: Routes = [
    {
        path: S.ACCOUNT.ROOT,
        loadChildren: () => import('./features/account/account.routes').then(r => r.ACCOUNT_ROUTES)
    },
    { path: '**', component: NotFoundPageComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }



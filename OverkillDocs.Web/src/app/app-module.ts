import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth.interceptor';


@NgModule({
    declarations: [
        App,
    ],
    imports: [
        BrowserModule,
        AppRoutingModule
    ],
    providers: [
        provideBrowserGlobalErrorListeners(),
        provideHttpClient(
            withInterceptors([authInterceptor])
        )
    ],
    bootstrap: [App]
})
export class AppModule { }

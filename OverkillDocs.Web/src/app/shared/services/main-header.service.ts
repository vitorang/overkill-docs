import { TemplatePortal } from '@angular/cdk/portal';
import { Injectable, signal } from '@angular/core';

interface IMainHeaderPortal {
    left?: TemplatePortal;
    center?: TemplatePortal;
    right?: TemplatePortal;
}

@Injectable({ providedIn: 'root' })
export class MainHeaderService {
    readonly portal = signal<IMainHeaderPortal | null>(null);
}

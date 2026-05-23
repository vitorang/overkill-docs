import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class DebugService {
    readonly debugModeEnabled = signal(false);
}

import { inject, Injectable } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { MainHub, ResponseListener } from '@core/hubs/main.hub';
import { distinctUntilChanged, Observable, Subject, switchMap } from 'rxjs';

const group = 'DocumentIndex';
const Hub = {
    join: `${group}:Join`,
    onChanged: `${group}:OnChanged`,
} as const;

@Injectable({ providedIn: 'root' })
export class DocumentIndexHub {
    private mainHub = inject(MainHub).mainHub;
    readonly onChanged = new Subject<void>();

    readonly join = (): Promise<void> => this.mainHub.send(Hub.join);

    get responseListeners(): ResponseListener[] {
        return [{ name: Hub.onChanged, listener: this.onChanged }];
    }

    get connection(): Observable<boolean> {
        return this.mainHub.connection.pipe(
            takeUntilDestroyed(),
            switchMap(() => toObservable(this.mainHub.state.connected)),
            distinctUntilChanged(),
        );
    }
}

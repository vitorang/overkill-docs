import { inject, Injectable } from '@angular/core';
import { IHub, IHubState, MainHub, ResponseListener } from '@core/hubs/main.hub';
import { Subject } from 'rxjs';

const group = 'DocumentIndex';
const Hub = {
    join: `${group}:Join`,
    onChanged: `${group}:OnChanged`,
} as const;

@Injectable({ providedIn: 'root' })
export class DocumentIndexHub implements IHub {
    private mainHub = inject(MainHub).mainHub;
    readonly onChanged = new Subject<void>();

    readonly join = (): Promise<void> => this.mainHub.send(Hub.join);

    responseListeners: ResponseListener[] = [{ name: Hub.onChanged, listener: this.onChanged }];

    get state(): IHubState {
        return this.mainHub.state;
    }
}

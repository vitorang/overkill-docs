import { Subject } from 'rxjs';
import { inject, Injectable } from '@angular/core';
import { MainHub, IHubState, IRawMessage } from '@core/hubs/main.hub';

@Injectable({ providedIn: 'root' })
export class DebugHubService {
    private mainHub = inject(MainHub).mainHub;

    get state(): IHubState {
        return this.mainHub.state;
    }
    get onReceived(): Subject<IRawMessage> {
        return this.mainHub.onReceived;
    }
    get onSended(): Subject<IRawMessage> {
        return this.mainHub.onSended;
    }
    readonly forceConnect = (): void => this.mainHub.forceConnect();
    readonly forceDisconnect = (): void => this.mainHub.forceDisconnect();
}

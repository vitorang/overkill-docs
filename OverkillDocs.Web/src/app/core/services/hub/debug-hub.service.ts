import { Subject } from "rxjs";
import { HubService, IRawMessage } from "./hub.service";
import { inject, Injectable, Signal } from "@angular/core";
import { HubState } from "../../models/common.model";

@Injectable({ providedIn: 'root' })
export class DebugHubService {
    private mainHub = inject(HubService).mainHub;

    get connectionState(): Signal<HubState> { return this.mainHub.connectionState; }
    get onReceived(): Subject<IRawMessage> { return this.mainHub.onReceived; }
    get onSended(): Subject<IRawMessage> { return this.mainHub.onSended; }
    readonly forceConnect = (): void => this.mainHub.forceConnect();
    readonly forceDisconnect = (): void => this.mainHub.forceDisconnect();
}
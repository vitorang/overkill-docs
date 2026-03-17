import { Subject } from "rxjs";
import { BaseHubService } from "./base-hub.service";
import { IRawMessage } from "./hub.service";

export class DebugHubService extends BaseHubService {
    get onReceived(): Subject<IRawMessage> { return this.mainHub.onReceived; }
    get onSended(): Subject<IRawMessage> { return this.mainHub.onSended; }
    readonly forceConnect = (): void => this.mainHub.forceConnect();
    readonly forceDisconnect = (): void => this.mainHub.forceDisconnect();
}
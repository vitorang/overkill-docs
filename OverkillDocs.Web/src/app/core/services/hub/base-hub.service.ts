import { Signal } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { IRawMessage } from "./hub.service";
import { HubState } from "../../models/common.model";

export interface IMainHub {
    connection: Observable<unknown>
    connectionState: Signal<HubState>
    isConnected: Signal<boolean>
    onReceived: Subject<IRawMessage>
    onSended: Subject<IRawMessage>
    send<T>(method: string, data?: T): Promise<void>
    forceDisconnect(): void
    forceConnect(): void
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export interface ResponseListener { name: string, listener: Subject<any> };

export abstract class BaseHubService {
    constructor(protected mainHub: IMainHub) { };

    get connection(): Observable<unknown> { return this.mainHub.connection; }
    get connectionState(): Signal<HubState> { return this.mainHub.connectionState; }
    get isConnected(): Signal<boolean> { return this.mainHub.isConnected; }

    get responseListeners(): ResponseListener[] {
        return [];
    }
}


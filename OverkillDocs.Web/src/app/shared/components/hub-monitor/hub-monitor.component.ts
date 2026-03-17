import { Component, inject, signal } from '@angular/core';
import { HubService, IRawMessage } from '../../../core/services/hub/hub.service';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { HubState } from '../../../core/models/common.model';
import { SHARED_NATIVE } from '../..';


type Mode = 'received' | 'sended' | 'stateChanged'
interface ILog {
    id: number
    method: string
    mode: Mode
    data: string
    timestamp: Date
}


@Component({
    selector: 'okd-hub-monitor',
    imports: [SHARED_NATIVE],
    templateUrl: './hub-monitor.component.html',
    styleUrl: './hub-monitor.component.scss',
})
export class HubMonitorComponent {
    protected debugHub = inject(HubService).debugHub;
    protected logs = signal<ILog[]>([]);
    private static lastId = 0;
    private connectionState = toObservable(this.debugHub.connectionState);

    protected stateColors: Record<HubState, string> = {
        [HubState.CONNECTED]: 'green',
        [HubState.CONNECTING]: 'chocolate',
        [HubState.DISCONNECTED]: 'firebrick',
    }

    constructor() {
        this.debugHub.onReceived.pipe(takeUntilDestroyed()).subscribe(this.onReceived);
        this.debugHub.onSended.pipe(takeUntilDestroyed()).subscribe(this.onSended);
        this.connectionState.pipe(takeUntilDestroyed()).subscribe(this.onStatusChanged)
        this.debugHub.connection.pipe(takeUntilDestroyed()).subscribe();
    }

    protected formatTime(date: Date): string {
        return date.toLocaleTimeString('pt-BR', {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            fractionalSecondDigits: 3
        })
    }

    protected toggleConnection(): void {
        if (this.debugHub.connectionState() === HubState.CONNECTED)
            this.debugHub.forceDisconnect();
        else if (this.debugHub.connectionState() === HubState.DISCONNECTED)
            this.debugHub.forceConnect();
    }

    private onReceived = (message: IRawMessage) => {
        this.addLog(message, 'received');
    }

    private onSended = (message: IRawMessage) => {
        this.addLog(message, 'sended');
    }

    private onStatusChanged = (state: HubState) => {
        this.addLog({ method: state, data: '' }, 'stateChanged');
    }

    private addLog(message: IRawMessage, mode: Mode) {
        let data = '';
        if (typeof (message.data) === 'string')
            data = message.data as string;
        else
            data = JSON.stringify(message.data, null, 2);

        const log: ILog = {
            id: (HubMonitorComponent.lastId++),
            mode,
            method: message.method,
            data,
            timestamp: new Date()
        };

        this.logs.set([log, ...this.logs().slice(0, 100)]);
    }
}

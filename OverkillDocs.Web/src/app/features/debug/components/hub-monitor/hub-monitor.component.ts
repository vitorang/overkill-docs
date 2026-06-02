import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { HubState, IRawMessage, MainHub } from '@core/hubs/main.hub';
import { SHARED } from '@shared/index';

type Mode = 'received' | 'sended' | 'stateChanged';
interface ILog {
    id: number;
    method: string;
    mode: Mode;
    data: string;
    timestamp: Date;
}

@Component({
    selector: 'okd-hub-monitor',
    imports: [SHARED],
    templateUrl: './hub-monitor.component.html',
    styleUrl: './hub-monitor.component.scss',
})
export class HubMonitorComponent {
    protected mainHub = inject(MainHub);
    protected logs = signal<ILog[]>([]);
    private static lastId = 0;
    private connectionState = toObservable(this.mainHub.state.current);

    protected stateColor = computed(() => this.stateColors[this.mainHub.state.current()]);
    private stateColors: Record<HubState, string> = {
        CONNECTED: 'green',
        CONNECTING: 'chocolate',
        DISCONNECTED: 'firebrick',
    };

    constructor() {
        this.mainHub.onReceived.pipe(takeUntilDestroyed()).subscribe(this.onReceived);
        this.mainHub.onSended.pipe(takeUntilDestroyed()).subscribe(this.onSended);
        this.connectionState.pipe(takeUntilDestroyed()).subscribe(this.onStatusChanged);
    }

    protected formatTime(date: Date): string {
        return date.toLocaleTimeString('pt-BR', {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            fractionalSecondDigits: 3,
        });
    }

    protected toggleConnection(): void {
        if (this.mainHub.state.connected()) {
            this.mainHub.disconnect();
        } else if (this.mainHub.state.disconnected()) {
            this.mainHub.connect();
        }
    }

    private onReceived = (message: IRawMessage) => {
        this.addLog(message, 'received');
    };

    private onSended = (message: IRawMessage) => {
        this.addLog(message, 'sended');
    };

    private onStatusChanged = (state: HubState) => {
        this.addLog({ method: state, data: '' }, 'stateChanged');
    };

    private addLog(message: IRawMessage, mode: Mode) {
        let data = '';
        if (typeof message.data === 'string') {
            data = message.data as string;
        } else {
            data = JSON.stringify(message.data, null, 2);
        }

        const log: ILog = {
            id: HubMonitorComponent.lastId++,
            mode,
            method: message.method,
            data,
            timestamp: new Date(),
        };

        this.logs.set([log, ...this.logs().slice(0, 100)]);
    }
}

import { inject, Injectable } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { IHubState, MainHub, ResponseListener } from '@core/hubs/main.hub';
import { DocumentFragment, DocumentFragmentLock } from '@features/document/models/document.models';
import { distinctUntilChanged, Observable, Subject, switchMap } from 'rxjs';

const group = 'DocumentViewer';
const Hub = {
    requestActiveLocks: `${group}:RequestActiveLocks`,
    join: `${group}:Join`,
    leave: `${group}:Leave`,

    onActiveLocksChanged: `${group}:OnActiveLocksChanged`,
    onDocumentChanged: `${group}:OnDocumentChanged`,
    onFragmentChanged: `${group}:OnFragmentChanged`,
} as const;

@Injectable({ providedIn: 'root' })
export class DocumentViewerHub {
    private mainHub = inject(MainHub).mainHub;
    readonly onActiveLocksChanged = new Subject<DocumentFragmentLock[]>();
    readonly onDocumentChanged = new Subject<string>();
    readonly onFragmentChanged = new Subject<DocumentFragment>();
    readonly onJoinRequested = new Subject<void>();

    readonly join = async (documentHashId: string): Promise<void> => {
        await this.mainHub.send(Hub.join, documentHashId);
        this.onJoinRequested.next();
    };

    readonly leave = (documentHashId: string): Promise<void> =>
        this.mainHub.send(Hub.leave, documentHashId);

    readonly requestActiveLocks = (documentHashId: string): Promise<void> =>
        this.mainHub.send(Hub.requestActiveLocks, documentHashId);

    get responseListeners(): ResponseListener[] {
        return [
            { name: Hub.onActiveLocksChanged, listener: this.onActiveLocksChanged },
            { name: Hub.onDocumentChanged, listener: this.onDocumentChanged },
            { name: Hub.onFragmentChanged, listener: this.onFragmentChanged },
        ];
    }

    get state(): IHubState {
        return this.mainHub.state;
    }

    get connection(): Observable<boolean> {
        return this.mainHub.connection.pipe(
            takeUntilDestroyed(),
            switchMap(() => toObservable(this.mainHub.state.connected)),
            distinctUntilChanged(),
        );
    }
}

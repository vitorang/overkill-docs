import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { distinctUntilChanged, filter, map, startWith } from 'rxjs';
import { DocumentViewerComponent } from '@features/document/components/document-viewer/document-viewer.component';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';

@Component({
    selector: 'okd-document-viewer-page',
    imports: [DocumentViewerComponent],
    templateUrl: './document-viewer-page.component.html',
    styleUrl: './document-viewer-page.component.scss',
})
export class DocumentViewerPageComponent {
    private viewerHub = inject(DocumentViewerHub);
    private router = inject(Router);
    protected documentHashIds = toSignal(
        this.router.events.pipe(
            filter((event) => event instanceof NavigationEnd),
            startWith(null),
            map(() => [this.getIdFromRoute()]),
            distinctUntilChanged(),
        ),
    );

    private getIdFromRoute(): string | null {
        let route = this.router.routerState.snapshot.root;
        while (route.firstChild) {
            route = route.firstChild;
        }
        return route.paramMap.get('documentHashId');
    }

    async leaveViewerHub(): Promise<void> {
        if (!this.viewerHub.state.connected()) {
            return;
        }

        const hashId = (this.documentHashIds() || [])[0];
        if (hashId) {
            await this.viewerHub.leave(hashId);
        }
    }
}

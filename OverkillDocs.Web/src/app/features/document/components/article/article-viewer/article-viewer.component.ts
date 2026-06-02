import {
    Component,
    computed,
    DestroyRef,
    inject,
    signal,
    OnDestroy,
    Injector,
} from '@angular/core';
import { SHARED } from '@shared/index';
import { ArticleMarkdownFragmentComponent } from '../article-markdown-fragment/article-markdown-fragment.component';
import { ArticleImageFragmentComponent } from '../article-image-fragment/article-image-fragment.component';
import { ArticleEmbedFragmentComponent } from '../article-embed-fragment/article-embed-fragment.component';
import {
    asSummary,
    DocumentFragment,
    DocumentFragmentType,
} from '@features/document/models/document.models';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import {
    catchError,
    distinctUntilChanged,
    exhaustMap,
    filter,
    finalize,
    first,
    interval,
    of,
    Subscription,
} from 'rxjs';
import {
    ArticleEmbedFragment,
    ArticleImageFragment,
    ArticleMarkdownFragment,
} from '@features/document/models/article.models';
import { ArticleAddFragmentComponent } from '@features/document/components/article/article-add-fragment/article-add-fragment.component';
import { ArticleEditFragmentComponent } from '@features/document/components/article/article-edit-fragment/article-edit-fragment.component';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';
import { AlertService } from '@shared/services/alert.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ProblemDetails } from '@core/models/problem-details.model';
import { MatDialog } from '@angular/material/dialog';
import { DocumentEditDialogComponent } from '@features/document/components/dialogs/document-edit-dialog/document-edit-dialog.component';
import {
    ConfirmDialogComponent,
    IConfirmDialog,
} from '@shared/components/confirm-dialog/confirm-dialog.component';
import { DocumentIndexService } from '@features/document/services/document-index.service';

@Component({
    selector: 'okd-article-viewer',
    imports: [
        SHARED,
        ArticleMarkdownFragmentComponent,
        ArticleImageFragmentComponent,
        ArticleEmbedFragmentComponent,
        ArticleAddFragmentComponent,
        ArticleEditFragmentComponent,
    ],
    templateUrl: './article-viewer.component.html',
    styleUrl: './article-viewer.component.scss',
})
export class ArticleViewerComponent implements OnDestroy {
    private viewerHub = inject(DocumentViewerHub);
    private viewerService = inject(DocumentViewerService);
    private indexService = inject(DocumentIndexService);
    private destroyRef = inject(DestroyRef);
    private alertService = inject(AlertService);
    private dialog = inject(MatDialog);
    private injector = inject(Injector);

    private lockRenewer = {
        interval: null as Subscription | null,
        milliseconds: 45000,
    };

    private locksRefresher = {
        interval: null as Subscription | null,
        milliseconds: 45000,
    };

    protected FragmentType = DocumentFragmentType;
    protected document = this.viewerService.document;
    protected editModeEnabled = signal(false);
    protected isLoading = signal(false);

    protected editingFragmentId = signal<string | null>(null);
    protected isEditingFragment = computed(() => !!this.editingFragmentId());
    protected toolbar = this.viewerService.toolbar;

    constructor() {
        toObservable(this.editModeEnabled).subscribe((enabled) => {
            if (enabled) {
                this.viewerHub.requestActiveLocks(this.document().hashId);
                this.startLocksRefresher();
            } else {
                this.editingFragmentId.set(null);
                this.stopLocksRefresher();
            }
        });

        toObservable(this.editingFragmentId)
            .pipe(distinctUntilChanged())
            .subscribe((id) => {
                if (id) {
                    this.startLockRenewer();
                } else {
                    this.toolbar.set(null);
                    this.stopLockRenewer();
                }
            });

        toObservable(this.viewerHub.state.connected)
            .pipe(filter((connected) => !connected))
            .subscribe(() => this.editModeEnabled.set(false));
    }

    ngOnDestroy(): void {
        this.stopLockRenewer();
        this.stopLocksRefresher();

        if (this.editingFragmentId()) {
            this.viewerService
                .unlockFragment(this.editingFragmentId()!)
                .pipe(
                    first(),
                    catchError(() => of(null)),
                )
                .subscribe();
        }
    }

    protected deleteDocument(): void {
        this.openConfirmModal('Deseja excluir o documento?').subscribe((value) => {
            if (!value) {
                return;
            }

            this.isLoading.set(true);
            this.indexService
                .delete(this.document().hashId)
                .pipe(
                    takeUntilDestroyed(this.destroyRef),
                    finalize(() => this.isLoading.set(false)),
                )
                .subscribe();
        });
    }

    protected renameDocument(): void {
        this.dialog.open(DocumentEditDialogComponent, {
            width: '500px',
            data: asSummary(this.document()),
            injector: this.injector,
        });
    }

    protected addFragment(type: DocumentFragmentType, after: DocumentFragment | null): void {
        if (this.isLoading() || this.isEditingFragment()) {
            return;
        }

        this.isLoading.set(true);
        this.viewerService
            .createFragment(type, after?.hashId || null)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe();
    }

    protected edit(hashId: string | null): void {
        const obs = {
            next: () => this.editingFragmentId.set(hashId),
            error: (err: HttpErrorResponse) =>
                this.alertService.error(
                    (err.error as ProblemDetails | undefined)?.detail || 'Erro ao executar ação',
                ),
        };

        if (hashId) {
            this.viewerService.lockFragment(hashId).subscribe(obs);
        } else if (hashId === null && this.editingFragmentId() !== null) {
            this.viewerService.unlockFragment(this.editingFragmentId()!).subscribe(obs);
        }
    }

    protected delete(fragment: DocumentFragment): void {
        this.openConfirmModal('Deseja excluir o fragmento?').subscribe((value) => {
            if (!value) {
                return;
            }

            this.isLoading.set(true);
            this.viewerService
                .deleteFragment(fragment)
                .pipe(
                    takeUntilDestroyed(this.destroyRef),
                    finalize(() => this.isLoading.set(false)),
                )
                .subscribe();
        });
    }

    protected asMarkdown(fragment: DocumentFragment): ArticleMarkdownFragment {
        return fragment as ArticleMarkdownFragment;
    }

    protected asImage(fragment: DocumentFragment): ArticleImageFragment {
        return fragment as ArticleImageFragment;
    }

    protected asEmbed(fragment: DocumentFragment): ArticleEmbedFragment {
        return fragment as ArticleEmbedFragment;
    }

    protected onFragmentChanged(fragment: DocumentFragment): void {
        if (!this.viewerHub.state.connected()) {
            return;
        }

        this.viewerService
            .updateFragment(fragment)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe();
    }

    protected saveAndRelease(fragment: DocumentFragment | null): void {
        if (!this.viewerHub.state.connected()) {
            return;
        }

        if (fragment === null) {
            return this.edit(null);
        }

        this.viewerService
            .updateFragment(fragment)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(() => {
                this.edit(null);
            });
    }

    private startLockRenewer() {
        this.stopLockRenewer();
        this.lockRenewer.interval = interval(this.lockRenewer.milliseconds)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                filter(() => this.isEditingFragment()),
                filter(() => this.viewerHub.state.connected()),
                exhaustMap(() =>
                    this.viewerService
                        .lockFragment(this.editingFragmentId()!)
                        .pipe(catchError(() => of(null))),
                ),
            )
            .subscribe();
    }

    private stopLockRenewer() {
        this.lockRenewer.interval?.unsubscribe();
        this.lockRenewer.interval = null;
    }

    private startLocksRefresher() {
        this.stopLocksRefresher();
        this.locksRefresher.interval = interval(this.locksRefresher.milliseconds)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                filter(() => this.editModeEnabled() && !this.isEditingFragment()),
                filter(() => this.viewerHub.state.connected()),
            )
            .subscribe(() => this.viewerHub.requestActiveLocks(this.document().hashId));
    }

    private stopLocksRefresher() {
        this.locksRefresher.interval?.unsubscribe();
        this.locksRefresher.interval = null;
    }

    private openConfirmModal(message: string) {
        return this.dialog
            .open<ConfirmDialogComponent, IConfirmDialog, boolean>(ConfirmDialogComponent, {
                width: '350px',
                data: { message },
                disableClose: false,
                autoFocus: false,
            })
            .afterClosed();
    }
}

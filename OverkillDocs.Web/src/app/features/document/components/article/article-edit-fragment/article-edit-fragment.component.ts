import { Component, computed, DestroyRef, inject, input, output, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { SimpleUser } from '@core/models/user.model';
import { UserService } from '@core/services/user.service';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';
import { DocumentFragmentLock } from '@features/document/models/document.models';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-article-edit-fragment',
    imports: [SHARED, AvatarComponent],
    templateUrl: './article-edit-fragment.component.html',
    styleUrl: './article-edit-fragment.component.scss',
})
export class ArticleEditFragmentComponent {
    fragmentHashId = input.required<string>();
    disabled = input.required<boolean>();
    edit = output<void>();
    delete = output<void>();

    private destroyRef = inject(DestroyRef);
    private userService = inject(UserService);
    private viewerHub = inject(DocumentViewerHub);
    private fragmentLocks = signal<DocumentFragmentLock[]>([]);
    private lockedBy = computed(
        () => this.fragmentLocks().find((e) => e.fragmentHashId === this.fragmentHashId()) || null,
    );

    protected user = signal<SimpleUser>(this.userService.defaultUser);
    protected showAvatar = computed(() => this.lockedBy());
    protected showActions = computed(
        () =>
            !this.lockedBy() ||
            (this.lockedBy()?.userHashId === this.userService.currentUser()?.hashId &&
                !this.disabled()),
    );

    constructor() {
        this.userService.loadCurrentUser().subscribe();

        this.viewerHub.onActiveLocksChanged
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((locks) => this.fragmentLocks.set(locks));

        toObservable(this.lockedBy).subscribe((lock) => {
            if (lock) {
                this.userService
                    .getUser(lock.userHashId)
                    .pipe(takeUntilDestroyed(this.destroyRef))
                    .subscribe((user) => {
                        if (user && user.hashId === this.lockedBy()?.userHashId) {
                            this.user.set(user);
                        }
                    });
            } else {
                this.user.set(this.userService.defaultUser);
            }
        });
    }
}

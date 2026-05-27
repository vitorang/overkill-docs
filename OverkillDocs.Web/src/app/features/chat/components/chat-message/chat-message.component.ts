import { Component, DestroyRef, inject, input, signal, OnInit } from '@angular/core';
import { takeUntilDestroyed, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { UserService } from '@core/services/user.service';
import { ChatMessage } from '@features/chat/chat.models';
import { SHARED } from '@shared/index';
import { distinctUntilChanged, filter, map, switchMap } from 'rxjs';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { SimpleUser } from '@core/models/user.model';

@Component({
    selector: 'okd-chat-message',
    imports: [SHARED, AvatarComponent],
    templateUrl: './chat-message.component.html',
    styleUrl: './chat-message.component.scss',
})
export class ChatMessageComponent implements OnInit {
    readonly message = input.required<ChatMessage>();
    readonly showUser = input.required<boolean>();

    private destroyRef = inject(DestroyRef);
    private userService = inject(UserService);
    protected user = signal<SimpleUser>(this.userService.defaultUser);

    ngOnInit(): void {
        this.userService
            .getUser(this.message().userHashId)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                filter((user) => !!user),
            )
            .subscribe((user) => this.user.set(user));
    }
}

import { Component, inject, input } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { UserService } from '@core/services/user.service';
import { ChatMessage } from '@features/chat/models/chat-message.model';
import { SHARED_CUSTOM, SHARED_NATIVE } from '@shared/index';
import { distinctUntilChanged, map, switchMap } from 'rxjs';


@Component({
    selector: 'okd-chat-message',
    imports: [SHARED_NATIVE, SHARED_CUSTOM],
    templateUrl: './chat-message.component.html',
    styleUrl: './chat-message.component.scss',
})
export class ChatMessageComponent {
    readonly message = input.required<ChatMessage>();
    readonly showUser = input.required<boolean>();

    private userService = inject(UserService);

    protected user = toSignal(
        toObservable(this.message).pipe(
            map(m => m.userHashId),
            distinctUntilChanged(),
            switchMap(hashId => this.userService.getUser(hashId))
        )
    );
}

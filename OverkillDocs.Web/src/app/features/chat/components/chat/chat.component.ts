import { Component, inject, signal } from '@angular/core';
import { SHARED_NATIVE } from '../../../../shared';
import { FragmentCollectionComponent } from '../../../document/components/fragment-collection/fragment-collection.component';
import { HubService } from '../../../../core/services/hub/hub.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
    selector: 'okd-chat',
    imports: [SHARED_NATIVE, FragmentCollectionComponent],
    templateUrl: './chat.component.html',
    styleUrl: './chat.component.scss',
})
export class ChatComponent {
    protected maxLength = 250;
    protected message = signal('');
    private chatHub = inject(HubService).chatHub;

    constructor() {
        this.chatHub.connection.pipe(takeUntilDestroyed()).subscribe((connected) => {
            if (connected)
                this.chatHub.join();
        });
    }

    protected onPressEnter(event: Event): void {
        const keyboardEvent = event as KeyboardEvent;
        if (keyboardEvent.shiftKey)
            return;

        event.preventDefault();
        this.sendMessage();
    }

    protected async sendMessage(): Promise<void> {
        const text = this.message().trim();

        if (text && this.chatHub.isConnected()) {
            await this.chatHub.sendMessage(text);
            this.message.set('');
        }
    }
}

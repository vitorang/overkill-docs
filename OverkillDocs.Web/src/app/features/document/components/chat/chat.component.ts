import { Component, computed, signal } from '@angular/core';
import { SHARED_NATIVE } from '../../../../shared';
import { FragmentCollectionComponent } from '../fragment-collection/fragment-collection.component';

@Component({
    selector: 'okd-chat',
    imports: [SHARED_NATIVE, FragmentCollectionComponent],
    templateUrl: './chat.component.html',
    styleUrl: './chat.component.scss',
})
export class ChatComponent {
    protected maxLength = 250;
    protected message = signal('');
    protected canSend = computed(() => this.message().trim().length > 0);

    protected onPressEnter(event: Event): void {
        const keyboardEvent = event as KeyboardEvent;
        if (keyboardEvent.shiftKey)
            return;

        event.preventDefault();
        this.sendMessage();
    }

    protected sendMessage(): void {
        const text = this.message().trim();

        if (text) {
            console.log('Mensagem enviada:', text);
            this.message.set('');
        }
    }
}

import {
    afterNextRender,
    AfterViewInit,
    Component,
    ElementRef,
    inject,
    Injector,
    OnDestroy,
    signal,
    ViewChild,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ChatMessageComponent } from '@features/chat/components/chat-message/chat-message.component';
import { ChatHubService } from '@features/chat/services/chat-hub.service';
import { BrowserService } from '@core/services/browser.service';
import { ChatMessage } from '@features/chat/models/chat-message.model';

@Component({
    selector: 'okd-chat-viewport',
    imports: [ChatMessageComponent],
    templateUrl: './chat-viewport.component.html',
    styleUrl: './chat-viewport.component.scss',
})
export class ChatViewportComponent implements AfterViewInit, OnDestroy {
    @ViewChild('scrollContainer') scrollContainer?: ElementRef<HTMLDivElement>;
    private injector = inject(Injector);

    private maxMessages = 100;
    private chatHub = inject(ChatHubService);
    private browserService = inject(BrowserService);

    protected messages = signal<ChatMessage[]>([]);

    constructor() {
        this.chatHub.onMessageReceived.pipe(takeUntilDestroyed()).subscribe((message) => {
            const messages = [...this.messages(), message];
            this.messages.set(this.filterRecent(messages));
            this.autoScrollToBottom();
        });

        this.chatHub.onRecentMessagesReceived
            .pipe(takeUntilDestroyed())
            .subscribe((recentMessages) => {
                const messages = [...this.messages()];
                for (const recent of recentMessages) {
                    if (!messages.some((m) => m.id === recent.id)) messages.push(recent);
                }

                messages.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

                this.messages.set(this.filterRecent(messages));
                this.autoScrollToBottom();
            });
    }

    ngAfterViewInit(): void {
        this.resizeObserver.observe(this.scrollContainer!.nativeElement);
    }

    ngOnDestroy(): void {
        this.resizeObserver.disconnect();
    }

    private resizeObserver = new ResizeObserver(() => {
        this.scrollToBottom(false);
    });

    private autoScrollToBottom(): void {
        if (!this.scrollContainer) return;

        const { scrollHeight, scrollTop, clientHeight } = this.scrollContainer.nativeElement;
        const tolerance = 100;

        if (scrollHeight - scrollTop <= clientHeight + tolerance) this.scrollToBottom(true);
    }

    private scrollToBottom(smooth: boolean): void {
        afterNextRender(
            () => {
                const scrollContainer = this.scrollContainer?.nativeElement;
                if (!scrollContainer) return;

                scrollContainer.scrollTo({
                    top: scrollContainer.scrollHeight,
                    behavior: smooth && this.browserService.documentIsVisible() ? 'smooth' : 'auto',
                });
            },
            { injector: this.injector },
        );
    }

    private filterRecent(messages: ChatMessage[]): ChatMessage[] {
        if (messages.length > this.maxMessages)
            messages = messages.slice(messages.length - this.maxMessages, messages.length);
        return messages;
    }
}

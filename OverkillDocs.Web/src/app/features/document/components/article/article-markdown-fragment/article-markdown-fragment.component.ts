import {
    Component,
    input,
    computed,
    inject,
    output,
    signal,
    viewChild,
    ElementRef,
    ViewChild,
    DestroyRef,
    TemplateRef,
} from '@angular/core';
import MarkdownIt from 'markdown-it';
import DOMPurify from 'dompurify';
import { ArticleMarkdownFragment } from '@features/document/models/article.models';
import { ArticlePlaceholderFragmentComponent } from '../article-placeholder-fragment/article-placeholder-fragment.component';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import {
    debounceTime,
    distinctUntilChanged,
    filter,
    fromEventPattern,
    map,
    merge,
    tap,
} from 'rxjs';
import EasyMDE from 'easymde';
import { SHARED } from '@shared/index';
import { DocumentViewerService } from '@features/document/services/document-viewer.service';

type MdeAction = 'heading' | 'bold' | 'italic' | 'quote' | 'list' | 'link';

@Component({
    selector: 'okd-article-markdown-fragment',
    imports: [SHARED, ArticlePlaceholderFragmentComponent],
    templateUrl: './article-markdown-fragment.component.html',
    styleUrl: './article-markdown-fragment.component.scss',
})
export class ArticleMarkdownFragmentComponent {
    fragment = input.required<ArticleMarkdownFragment>();
    isEditing = input.required<boolean>();
    fragmentChanged = output<ArticleMarkdownFragment>();
    finishEdit = output<ArticleMarkdownFragment | null>();

    private viewerHub = inject(DocumentViewerHub);
    private viewerService = inject(DocumentViewerService);
    private destroyRef = inject(DestroyRef);

    private textToSave: string | null = null;
    protected current = signal({
        updatedAt: '',
        html: '',
        markdown: '',
    });

    private toolbarContent = viewChild<TemplateRef<void>>('toolbarContent');
    private easyMDE: EasyMDE | null = null;
    @ViewChild('mdeTextarea', { static: false }) set textareaRef(
        element: ElementRef<HTMLTextAreaElement> | undefined,
    ) {
        if (element && !this.easyMDE) {
            this.initializeEditor(element.nativeElement);
        }
    }

    constructor() {
        toObservable(this.isEditing)
            .pipe(distinctUntilChanged())
            .subscribe(() => {
                if (this.isEditing()) {
                    this.viewerService.toolbar.set({
                        template: this.toolbarContent()!,
                        showTitle: false,
                    });
                } else {
                    this.easyMDE?.toTextArea();
                    this.easyMDE = null;
                }
            });

        merge(
            toObservable(this.fragment),
            this.viewerHub.onFragmentChanged.pipe(map((e) => e as ArticleMarkdownFragment)),
        )
            .pipe(
                takeUntilDestroyed(),
                filter((fragment) => fragment.hashId === this.fragment().hashId),
                filter((fragment) => fragment.updatedAt > this.current().updatedAt),
            )
            .subscribe((fragment) => {
                this.current.set({
                    updatedAt: fragment.updatedAt,
                    html: this.getHtml(fragment.text),
                    markdown: fragment.text,
                });
            });
    }

    private initializeEditor(htmlElement: HTMLTextAreaElement) {
        this.easyMDE = new EasyMDE({
            element: htmlElement,
            initialValue: this.current().markdown,
            spellChecker: false,
            status: false,
            theme: 'none',
            minHeight: '0',
            toolbar: false,
        });

        fromEventPattern(
            (handler) => this.easyMDE!.codemirror.on('change', handler),
            (handler) => this.easyMDE!.codemirror.off('change', handler),
        )
            .pipe(
                map(() => this.easyMDE!.value()),
                tap((text) => (this.textToSave = text)),
                debounceTime(3000),
                takeUntilDestroyed(this.destroyRef),
                filter(() => this.textToSave !== null && this.isEditing()),
            )
            .subscribe(() => this.saveText(false));
    }

    private markdownIt: MarkdownIt = (() => {
        const instance = new MarkdownIt({
            html: false,
            linkify: true,
            typographer: true,
        });

        const defaultRender =
            instance.renderer.rules['link_open'] ||
            function (tokens, idx, options, env, self) {
                return self.renderToken(tokens, idx, options);
            };

        instance.renderer.rules['link_open'] = (tokens, idx, options, env, self) => {
            const aIndex = tokens[idx].attrIndex('target');

            if (aIndex < 0) {
                tokens[idx].attrPush(['target', '_blank']);
                tokens[idx].attrPush(['rel', 'noopener noreferrer']);
            } else {
                tokens[idx].attrs![aIndex][1] = '_blank';
            }

            return defaultRender(tokens, idx, options, env, self);
        };

        return instance;
    })();

    protected showPlaceholder = computed(() => !this.current().html);

    private getHtml(text: string): string {
        if (!text.trim()) {
            return '';
        }

        let html = this.markdownIt.render(text);
        html = DOMPurify.sanitize(html, {
            ADD_ATTR: ['target'],
            FORBID_TAGS: ['img', 'video', 'audio', 'iframe'],
            USE_PROFILES: { html: true },
        });

        return html;
    }

    private saveText(finishEdit: boolean): void {
        const text = this.textToSave;
        this.textToSave = null;

        const content = {
            ...this.fragment(),
            text: text || '',
        };

        if (finishEdit) {
            this.finishEdit.emit(text !== null ? content : null);
        } else if (text !== null) {
            this.fragmentChanged.emit(content);
        }
    }

    protected saveAndFinishEdit(): void {
        this.saveText(true);
    }

    executeAction(action: MdeAction): void {
        const nativeActions: Record<MdeAction, (editor: EasyMDE) => void> = {
            heading: EasyMDE.toggleHeadingSmaller,
            bold: EasyMDE.toggleBold,
            italic: EasyMDE.toggleItalic,
            quote: EasyMDE.toggleBlockquote,
            list: EasyMDE.toggleUnorderedList,
            link: EasyMDE.drawLink,
        };

        if (this.easyMDE) {
            nativeActions[action](this.easyMDE);
        }
    }
}

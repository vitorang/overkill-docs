import {
    Component,
    input,
    computed,
    inject,
    output,
    signal,
    model,
    viewChild,
    ElementRef,
    effect,
    ViewChild,
    DestroyRef,
} from '@angular/core';
import MarkdownIt from 'markdown-it';
import DOMPurify from 'dompurify';
import { DomSanitizer } from '@angular/platform-browser';
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
    Observable,
    tap,
} from 'rxjs';
import EasyMDE from 'easymde';

@Component({
    selector: 'okd-article-markdown-fragment',
    imports: [ArticlePlaceholderFragmentComponent],
    templateUrl: './article-markdown-fragment.component.html',
    styleUrl: './article-markdown-fragment.component.scss',
})
export class ArticleMarkdownFragmentComponent {
    fragment = input.required<ArticleMarkdownFragment>();
    isEditing = input.required<boolean>();
    fragmentChanged = output<ArticleMarkdownFragment>();

    private destroyRef = inject(DestroyRef);
    private easyMDE: EasyMDE | null = null;
    @ViewChild('mdeTextarea', { static: false }) set textareaRef(
        element: ElementRef<HTMLTextAreaElement> | undefined,
    ) {
        if (element && !this.easyMDE) {
            this.initializeEditor(element.nativeElement);
        }
    }

    private viewerHub = inject(DocumentViewerHub);
    protected textModel = model<string>('');
    protected current = signal({
        updatedAt: '',
        html: '',
        markdown: '',
    });

    constructor() {
        toObservable(this.isEditing)
            .pipe(distinctUntilChanged())
            .subscribe(() => {
                if (this.isEditing()) {
                    this.textModel.set(this.fragment().text);
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
                tap(() => console.log('editando')),
                debounceTime(3000),
                tap(() => console.log('editado')),
                takeUntilDestroyed(this.destroyRef),
                filter(() => this.viewerHub.state.connected()),
            )
            .subscribe((text) => this.saveText(text));
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

    private sanitizer = inject(DomSanitizer);
    /* protected htmlContent = computed(() => {
        let html = this.markdownIt.render(this.fragment().text);

        html = DOMPurify.sanitize(html, {
            ADD_ATTR: ['target'],
            FORBID_TAGS: ['img', 'video', 'audio', 'iframe'],
            USE_PROFILES: { html: true },
        });

        return this.sanitizer.bypassSecurityTrustHtml(html);
    });*/

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

    protected saveText(text: string): void {
        this.fragmentChanged.emit({
            ...this.fragment(),
            text,
        });
    }
}

import { Component, input, computed, inject } from '@angular/core';
import MarkdownIt from 'markdown-it';
import DOMPurify from 'dompurify';
import { DomSanitizer } from '@angular/platform-browser';
import { ArticleMarkdownFragment } from '@features/document/models/article.models';

@Component({
    selector: 'okd-article-markdown-fragment',
    imports: [],
    templateUrl: './article-markdown-fragment.component.html',
    styleUrl: './article-markdown-fragment.component.scss',
})
export class ArticleMarkdownFragmentComponent {
    fragment = input.required<ArticleMarkdownFragment>();

    private sanitizer = inject(DomSanitizer);

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

    protected htmlContent = computed(() => {
        let html = this.markdownIt.render(this.fragment().text);

        html = DOMPurify.sanitize(html, {
            ADD_ATTR: ['target'],
            FORBID_TAGS: ['img', 'video', 'audio', 'iframe'],
            USE_PROFILES: { html: true },
        });

        return this.sanitizer.bypassSecurityTrustHtml(html);
    });
}

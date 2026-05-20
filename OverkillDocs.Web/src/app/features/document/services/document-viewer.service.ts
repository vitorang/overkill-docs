import { HttpClient } from '@angular/common/http';
import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { API } from '@core/constants/api.constants';
import { apiHandler } from '@core/utils/api-handler.utils';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';
import {
    ArticleEmbedFragment,
    ArticleImageFragment,
    ArticleMarkdownFragment,
} from '@features/document/models/article.models';
import {
    DocumentType,
    DocumentDetail,
    DocumentFragmentType,
    DocumentFragment,
    typedFragment,
} from '@features/document/models/document.models';
import { filter, merge, Observable } from 'rxjs';

@Injectable()
export class DocumentViewerService {
    private viewerHub = inject(DocumentViewerHub);
    private http = inject(HttpClient);
    private documentHashId = '';
    private connected = toObservable(this.viewerHub.state.connected);
    private initialized = false;
    private destroyRef = inject(DestroyRef);

    documentHandler = apiHandler();
    document = signal<DocumentDetail>(this.emptyDocument);

    initialize(documentHashId: string): void {
        if (this.initialized) {
            throw 'Serviço já foi inicializado';
        }
        this.initialized = true;
        this.documentHashId = documentHashId;

        merge(this.viewerHub.onJoinRequested, this.viewerHub.onDocumentChanged)
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(() => this.load());

        this.viewerHub.onFragmentChanged
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe((fragment) => {
                const document = this.document();
                const index = document.fragments.findIndex((e) => e.hashId === fragment.hashId);
                if (index !== -1) {
                    document.fragments[index] = fragment;
                    this.document.set(document);
                }
            });

        this.connected
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                filter((connected) => connected),
            )
            .subscribe(() => this.viewerHub.join(this.documentHashId));
    }

    private load(): void {
        if (!this.documentHashId) {
            return;
        }

        this.documentHandler.execute(
            this.http.get<DocumentDetail>(API.DOCUMENTS.BY_ID(this.documentHashId)),
            (result) => {
                if (result.updatedAt > this.document().updatedAt) {
                    this.document.set(result);
                }
            },
        );
    }

    private get emptyDocument(): DocumentDetail {
        return {
            hashId: '',
            title: '',
            type: DocumentType.Unknown,
            fragments: [],
            updatedAt: '',
        };
    }

    private createFragment(
        type: DocumentFragmentType,
        order: number,
    ): Observable<DocumentFragment> {
        let fragment: DocumentFragment = {
            hashId: '',
            documentHashId: this.documentHashId!,
            type,
            order,
        };

        if (type === DocumentFragmentType.Markdown) {
            const markdown: ArticleMarkdownFragment = {
                ...fragment,
                type,
                text: '',
            };

            fragment = markdown;
        }

        if (type === DocumentFragmentType.Image) {
            const image: ArticleImageFragment = {
                ...fragment,
                type,
                alt: '',
                url: '',
            };

            fragment = image;
        }

        if (type === DocumentFragmentType.Embed) {
            const embed: ArticleEmbedFragment = {
                ...fragment,
                type,
                url: '',
            };

            fragment = embed;
        }

        return this.http.post<DocumentFragment>(
            API.DOCUMENT_FRAGMENTS.INDEX,
            typedFragment(fragment),
        );
    }

    addFragment(
        type: DocumentFragmentType,
        after: DocumentFragment | null,
    ): Observable<DocumentFragment> {
        const fragments = this.document()
            .fragments.map((e) => ({ hashId: e.hashId, order: e.order }))
            .sort((a, b) => a.order - b.order);

        let order = 0;
        const index = fragments.findIndex((e) => e.hashId === after?.hashId);

        if (fragments.length === 0) {
            order = 1000;
        } else if (after === null) {
            order = fragments[0].order - 1000;
        } else if (index === -1 || after.hashId === fragments.at(-1)!.hashId) {
            order = fragments.at(-1)!.order + 1000;
        } else {
            order = (fragments[index].order + fragments[index + 1].order) / 2;
        }

        return this.createFragment(type, order);
    }
}

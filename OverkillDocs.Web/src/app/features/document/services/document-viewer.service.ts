import { HttpClient } from '@angular/common/http';
import { DestroyRef, inject, Injectable, signal, TemplateRef } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { API } from '@core/constants/api.constants';
import { apiHandler } from '@core/utils/api-handler.utils';
import { DocumentViewerHub } from '@features/document/hubs/document-viewer.hub';
import {
    DocumentType,
    DocumentDetail,
    DocumentFragmentType,
    DocumentFragmentCreation,
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
    toolbar = signal<{ template: TemplateRef<void>; showTitle: boolean } | null>(null);

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

    createFragment(type: DocumentFragmentType, insertAfterHashId: string | null): Observable<void> {
        const fragment: DocumentFragmentCreation = {
            type,
            insertAfterHashId,
            documentHashId: this.document().hashId,
        };

        return this.http.post<void>(API.DOCUMENT_FRAGMENTS.INDEX, fragment);
    }

    deleteFragment(fragment: DocumentFragment): Observable<void> {
        return this.http.delete<void>(API.DOCUMENT_FRAGMENTS.BY_ID(fragment.hashId));
    }

    updateFragment(fragment: DocumentFragment): Observable<void> {
        return this.http.put<void>(API.DOCUMENT_FRAGMENTS.INDEX, typedFragment(fragment));
    }
}

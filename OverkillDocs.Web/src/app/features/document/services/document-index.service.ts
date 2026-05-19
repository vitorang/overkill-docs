import { HttpClient } from '@angular/common/http';
import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { API } from '@core/constants/api.constants';
import { faker } from '@faker-js/faker';
import { DocumentIndexHub } from '@features/document/hubs/document-index.hub';
import { DocumentSummary, DocumentType } from '@features/document/models/document.models';
import { AlertService } from '@shared/services/alert.service';
import { asyncScheduler, filter, forkJoin, Observable, throttleTime } from 'rxjs';

@Injectable()
export class DocumentIndexService {
    private documentIndexHub = inject(DocumentIndexHub);
    private http = inject(HttpClient);
    private alertService = inject(AlertService);
    private destroyRef = inject(DestroyRef);
    readonly documents = signal<DocumentSummary[]>([]);

    constructor() {
        this.documentIndexHub.connection
            .pipe(
                takeUntilDestroyed(),
                filter((connected) => connected),
            )
            .subscribe(() => {
                this.documentIndexHub.join();
                this.load();
            });

        this.documentIndexHub.onChanged
            .pipe(
                throttleTime(250, asyncScheduler, { leading: false, trailing: true }),
                takeUntilDestroyed(),
            )
            .subscribe(() => {
                this.load();
            });
    }

    private load(): void {
        this.list()
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: (value) => this.onDocumentsReceived(value),
                error: () => this.alertService.error('Erro ao carregar documentos'),
            });
    }

    private onDocumentsReceived(documents: DocumentSummary[]) {
        documents.sort((a, b) =>
            a.title.localeCompare(b.title, undefined, { numeric: true, sensitivity: 'base' }),
        );
        this.documents.set(documents);
    }

    private list(): Observable<DocumentSummary[]> {
        return this.http.get<DocumentSummary[]>(API.DOCUMENTS.ROOT);
    }

    create(document: DocumentSummary): Observable<DocumentSummary> {
        return this.http.post<DocumentSummary>(API.DOCUMENTS.ROOT, document);
    }

    update(document: DocumentSummary): Observable<DocumentSummary> {
        return this.http.put<DocumentSummary>(API.DOCUMENTS.ROOT, document);
    }

    getById(hashId: string): Observable<DocumentSummary> {
        return this.http.get<DocumentSummary>(API.DOCUMENTS.BY_ID(hashId));
    }

    delete(hashId: string): Observable<void> {
        return this.http.delete<void>(API.DOCUMENTS.BY_ID(hashId));
    }

    debugGenerateMany(quantity: number): void {
        const requests = [];

        for (let i = 0; i < quantity; i++) {
            const observable = this.create({
                hashId: '',
                title: `${faker.commerce.productAdjective()} ${faker.commerce.productName()}`,
                type: DocumentType.Article,
            });
            requests.push(observable);
        }

        forkJoin(requests).subscribe({
            next: () => this.load(),
        });
    }

    debugDeleteAll(): void {
        const requests = [];
        for (const doc of this.documents()) {
            const observable = this.delete(doc.hashId);
            requests.push(observable);
        }

        forkJoin(requests).subscribe({
            next: () => this.load(),
        });
    }
}

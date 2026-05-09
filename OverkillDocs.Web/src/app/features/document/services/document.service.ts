import { HttpClient } from '@angular/common/http';
import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { API } from '@core/constants/api.constants';
import { faker } from '@faker-js/faker';
import { DocumentModel, DocumentModelType } from '@features/document/models/document.model';
import { AlertService } from '@shared/services/alert.service';
import { forkJoin, Observable } from 'rxjs';

@Injectable()
export class DocumentService {
    private http = inject(HttpClient);
    private alertService = inject(AlertService);
    private destroyRef = inject(DestroyRef);
    readonly documents = signal<DocumentModel[]>([]);

    constructor() {
        this.load();
    }

    private load(): void {
        this.list()
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
                next: (value) => this.documents.set(value),
                error: () => this.alertService.error('Erro ao carregar documentos'),
            });
    }

    private list(): Observable<DocumentModel[]> {
        return this.http.get<DocumentModel[]>(API.DOCUMENTS.ROOT);
    }

    create(document: DocumentModel): Observable<DocumentModel> {
        return this.http.post<DocumentModel>(API.DOCUMENTS.ROOT, document);
    }

    update(document: DocumentModel): Observable<DocumentModel> {
        return this.http.put<DocumentModel>(API.DOCUMENTS.ROOT, document);
    }

    getById(hashId: string): Observable<DocumentModel> {
        return this.http.get<DocumentModel>(API.DOCUMENTS.BY_ID(hashId));
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
                type: DocumentModelType.Article,
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

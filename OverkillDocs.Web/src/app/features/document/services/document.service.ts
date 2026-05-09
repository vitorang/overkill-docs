import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API } from '@core/constants/api.constants';
import { DocumentModel, DocumentSearchResult } from '@features/document/models/document.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DocumentService {
    private http = inject(HttpClient);

    search(text: string, page: number): Observable<DocumentSearchResult> {
        return this.http.get<DocumentSearchResult>(API.DOCUMENTS.SEARCH, {
            params: { text, page },
        });
    }

    create(document: DocumentModel): Observable<DocumentModel> {
        return this.http.post<DocumentModel>(API.DOCUMENTS.ROOT, document);
    }

    update(document: DocumentModel): Observable<DocumentModel> {
        return this.http.put<DocumentModel>(API.DOCUMENTS.ROOT, document);
    }

    load(hashId: string): Observable<DocumentModel> {
        return this.http.get<DocumentModel>(API.DOCUMENTS.BY_ID(hashId));
    }

    delete(hashId: string): Observable<void> {
        return this.http.delete<void>(API.DOCUMENTS.BY_ID(hashId));
    }
}

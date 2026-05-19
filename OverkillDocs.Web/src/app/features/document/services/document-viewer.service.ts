import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { API } from '@core/constants/api.constants';
import { apiHandler } from '@core/utils/api-handler.utils';
import { faker } from '@faker-js/faker';
import {
    ArticleEmbedFragment,
    ArticleImageFragment,
    ArticleMarkdownFragment,
} from '@features/document/models/article.models';
import {
    DocumentType,
    DocumentDetail,
    DocumentFragmentType,
} from '@features/document/models/document.models';
import { filter, map, startWith } from 'rxjs';

@Injectable()
export class DocumentViewerService {
    private http = inject(HttpClient);
    public documentHandler = apiHandler();
    private router = inject(Router);

    constructor() {
        this.router.events
            .pipe(
                filter((event) => event instanceof NavigationEnd),
                startWith(null),
                map(() => this.getIdFromRoute()),
                filter((documentHashId) => !!documentHashId),
            )
            .subscribe((documentHashId) => this.load(documentHashId!));
    }

    document = signal<DocumentDetail>(this.emptyDocument);

    private getIdFromRoute(): string | null {
        let route = this.router.routerState.snapshot.root;
        while (route.firstChild) {
            route = route.firstChild;
        }
        return route.paramMap.get('documentHashId');
    }

    private load(documentHashId: string): void {
        this.document.set(this.emptyDocument);
        this.documentHandler.execute(
            this.http.get<DocumentDetail>(API.DOCUMENTS.BY_ID(documentHashId)),
            (result) => {
                if (result.updatedAt > this.document().updatedAt) this.document.set(result);
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

    private mock(): void {
        this.document.set({
            ...this.document(),
            type: DocumentType.Article,
            title: faker.lorem.sentence(),
            fragments: [
                {
                    hashId: faker.string.ulid(),
                    order: 1,
                    type: DocumentFragmentType.Markdown,
                    text: [
                        `# ${faker.lorem.sentence(10)}`,
                        faker.lorem.paragraphs(),
                        '- A',
                        '- B',
                        '\nMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM',
                        '\n[Google](https://www.google.com)',
                        '\n<big>Textão!</big>',
                        '![Descrição da Imagem](https://upload.wikimedia.org/wikipedia/commons/d/dd/Paullinia_cupana_-_K%C3%B6hler%E2%80%93s_Medizinal-Pflanzen-234.jpg)',
                    ].join('\n'),
                } as ArticleMarkdownFragment,
                {
                    hashId: faker.string.ulid(),
                    order: 2,
                    type: DocumentFragmentType.Embed,
                    url: 'https://www.youtube.com/watch?v=Y7fUHwLtjJ8',
                } as ArticleEmbedFragment,

                {
                    hashId: faker.string.ulid(),
                    order: 3,
                    type: DocumentFragmentType.Embed,
                    url: 'https://youtu.be/7lIRGyhpEuU?si=PsBj2ST8yL7Q58jN',
                } as ArticleEmbedFragment,
                {
                    hashId: faker.string.ulid(),
                    order: 4,
                    type: DocumentFragmentType.Embed,
                    url: 'https://www.youtube.com/shorts/qBBlfT86GUk',
                } as ArticleEmbedFragment,
                {
                    hashId: faker.string.ulid(),
                    order: 4,
                    type: DocumentFragmentType.Image,
                    url: 'https://upload.wikimedia.org/wikipedia/commons/d/dd/Paullinia_cupana_-_K%C3%B6hler%E2%80%93s_Medizinal-Pflanzen-234.jpg',
                    alt: 'Descrição anatômica de Paullinia cupana',
                } as ArticleImageFragment,
            ],
        });
    }
}

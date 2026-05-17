import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
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
import { map } from 'rxjs';

@Injectable()
export class DocumentViewerService {
    private http = inject(HttpClient);
    private route = inject(ActivatedRoute);
    public documentId = toSignal(this.route.params.pipe(map((p) => p['id'])));
    protected documentHandler = apiHandler();

    document = signal<DocumentDetail>({
        hashId: this.documentId(),
        title: '',
        type: DocumentType.Unknown,
        fragments: [],
    });

    load(): void {
        this.mock();
    }

    mock(): void {
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

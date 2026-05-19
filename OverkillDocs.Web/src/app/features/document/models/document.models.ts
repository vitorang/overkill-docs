import {
    ArticleEmbedFragment,
    ArticleImageFragment,
    ArticleMarkdownFragment,
} from '@features/document/models/article.models';

export enum DocumentType {
    Unknown = 0,
    Article = 1,
}

export enum DocumentFragmentType {
    Markdown = 1,
    Image = 2,
    Embed = 3,
}

export interface DocumentSummary {
    hashId: string;
    title: string;
    type: DocumentType;
}

export interface DocumentDetail {
    hashId: string;
    title: string;
    type: DocumentType;
    fragments: (ArticleEmbedFragment | ArticleImageFragment | ArticleMarkdownFragment)[];
    updatedAt: string;
}

export interface DocumentFragment {
    hashId: string;
    type: DocumentFragmentType;
    order: number;
}

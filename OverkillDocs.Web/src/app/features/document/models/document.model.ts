export enum DocumentModelType {
    Unknown = 0,
    Article = 1,
}

export enum DocumentFragmentType {
    Markdown = 1,
    Image = 2,
    Embed = 3,
}

export interface DocumentModel {
    hashId: string;
    title: string;
    type: DocumentModelType;
}

export interface DocumentStructure {
    hashId: string;
    title: string;
    type: DocumentModelType;
    fragments: DocumentFragmentModel[];
}

export interface DocumentFragmentModel {
    hashId: string;
    type: DocumentFragmentType;
    order: number;
    value: string;
}

export interface ArticleImageFragment {
    url: string;
    alt: string;
}

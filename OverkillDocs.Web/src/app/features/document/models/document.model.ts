export enum DocumentType {
    Article = 1,
}

export interface DocumentModel {
    hashId: string;
    title: string;
    type: DocumentType;
}

export interface DocumentSearchResult {
    text: string;
    page: number;
    total: number;
    hasMore: boolean;
    items: DocumentModel[];
}

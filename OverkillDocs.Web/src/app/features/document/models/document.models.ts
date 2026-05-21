export enum DocumentType {
    Unknown = 0,
    Article = 1,
}

export enum DocumentFragmentType {
    Markdown = 1,
    Image = 2,
    Embed = 3,
}

export interface DocumentCreation {
    title: string;
    type: DocumentType;
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
    fragments: DocumentFragment[];
    updatedAt: string;
}

export interface DocumentFragment {
    hashId: string;
    documentHashId: string;
    type: DocumentFragmentType;
    order: number;
}

export interface DocumentFragmentCreation {
    documentHashId: string;
    insertAfterHashId: string | null;
    type: DocumentFragmentType;
}

export interface DocumentFragmentLock {
    userHashId: string;
    fragmentHashId: string;
}

export function typedFragment(
    fragment: DocumentFragment,
): DocumentFragment & { $type: DocumentFragmentType } {
    return {
        $type: fragment.type,
        ...fragment,
    };
}

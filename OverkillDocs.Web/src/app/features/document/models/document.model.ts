export enum DocumentModelType {
    Article = 1,
}

export interface DocumentModel {
    hashId: string;
    title: string;
    type: DocumentModelType;
}

import { DocumentFragment, DocumentFragmentType } from '@features/document/models/document.models';

export interface ArticleEmbedFragment extends DocumentFragment {
    type: DocumentFragmentType.Embed;
    url: string;
}

export interface ArticleImageFragment extends DocumentFragment {
    type: DocumentFragmentType.Image;
    url: string;
    alt: string;
}

export interface ArticleMarkdownFragment extends DocumentFragment {
    type: DocumentFragmentType.Markdown;
    text: string;
}

import { Routes } from "@angular/router";

import { SEGMENTS } from "@core/constants/routes.constant";
import { DocumentLayoutComponent } from "@features/document/layouts/document-layout/document-layout.component";
import { DocumentEditorPageComponent } from "@features/document/pages/document-editor-page/document-editor-page.component";
import { DocumentIndexPageComponent } from "@features/document/pages/document-index-page/document-index-page.component";

const S = SEGMENTS.DOCUMENT;
export const DOCUMENT_ROUTES: Routes = [
    {
        path: '',
        component: DocumentLayoutComponent,
        children: [
            { path: '', component: DocumentIndexPageComponent },
            { path: S.EDITOR, component: DocumentEditorPageComponent },
        ]
    }
];
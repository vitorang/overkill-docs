import { Routes } from '@angular/router';

import { SEGMENTS } from '@core/constants/routes.constant';
import { DocumentLayoutComponent } from '@features/document/layouts/document-layout/document-layout.component';
import { DocumentEditorPageComponent } from '@features/document/pages/document-editor-page/document-editor-page.component';
import { DocumentIntroPageComponent } from '@features/document/pages/document-intro-page/document-intro-page.component';

const S = SEGMENTS.DOCUMENT;
export const DOCUMENT_ROUTES: Routes = [
    {
        path: '',
        component: DocumentLayoutComponent,
        children: [
            { path: '', component: DocumentIntroPageComponent },
            { path: S.EDITOR, component: DocumentEditorPageComponent },
        ],
    },
];

import { Routes } from '@angular/router';

import { SEGMENTS } from '@core/constants/routes.constant';
import { DocumentLayoutComponent } from '@features/document/layouts/document-layout/document-layout.component';
import { DocumentIntroPageComponent } from '@features/document/pages/document-intro-page/document-intro-page.component';
import { DocumentViewerPageComponent } from '@features/document/pages/document-viewer-page/document-viewer-page.component';

const S = SEGMENTS.DOCUMENT;
export const DOCUMENT_ROUTES: Routes = [
    {
        path: '',
        component: DocumentLayoutComponent,
        children: [
            { path: '', component: DocumentIntroPageComponent },
            {
                path: S.EDITOR,
                component: DocumentViewerPageComponent,
                canDeactivate: [
                    async (component: DocumentViewerPageComponent): Promise<boolean> => {
                        try {
                            await component.leaveViewerHub();
                        } catch (e) {
                            console.error(e);
                        }
                        return true;
                    },
                ],
            },
        ],
    },
];

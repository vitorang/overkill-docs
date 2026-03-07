import { Routes } from "@angular/router";
import { SEGMENTS } from "../../core/constants/routes.constant";
import { DocumentIndexPageComponent } from "./pages/document-index-page/document-index-page.component";
import { DocumentEditorPageComponent } from "./pages/document-editor-page/document-editor-page.component";
import { DocumentLayoutComponent } from "./layouts/document-layout/document-layout.component";

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
import { Component, input, output } from '@angular/core';
import {
    DocumentFragment,
    DocumentFragmentLock,
    DocumentFragmentType,
} from '@features/document/models/document.models';
import { SHARED } from '@shared/index';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';

@Component({
    selector: 'okd-article-fragment-actions',
    imports: [SHARED, AvatarComponent],
    templateUrl: './article-fragment-actions.component.html',
    styleUrl: './article-fragment-actions.component.scss',
})
export class ArticleFragmentActionsComponent {
    fragment = input.required<DocumentFragment | null>();
    lock = input<DocumentFragmentLock>();
    isLoading = input.required<boolean>();
    create = output<DocumentFragmentType>();
    edit = output<void>();
    delete = output<void>();

    protected Type = DocumentFragmentType;
}

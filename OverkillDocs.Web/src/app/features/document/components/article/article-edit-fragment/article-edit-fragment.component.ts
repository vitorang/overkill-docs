import { Component, output } from '@angular/core';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-article-edit-fragment',
    imports: [SHARED, AvatarComponent],
    templateUrl: './article-edit-fragment.component.html',
    styleUrl: './article-edit-fragment.component.scss',
})
export class ArticleEditFragmentComponent {
    edit = output<void>();
    delete = output<void>();
}

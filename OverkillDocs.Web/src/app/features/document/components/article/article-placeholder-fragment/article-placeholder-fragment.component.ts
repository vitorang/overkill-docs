import { Component, computed, input, signal } from '@angular/core';
import { faker } from '@faker-js/faker';
import { DocumentFragmentType } from '@features/document/models/document.models';
import { SHARED } from '@shared/index';

@Component({
    selector: 'okd-article-placeholder-fragment',
    imports: [SHARED],
    templateUrl: './article-placeholder-fragment.component.html',
    styleUrl: './article-placeholder-fragment.component.scss',
})
export class ArticlePlaceholderFragmentComponent {
    type = input.required<DocumentFragmentType>();
    protected Type = DocumentFragmentType;

    protected textPlaceholder = signal(faker.lorem.words(50));
    protected icon = computed(() => {
        const icons: Partial<Record<DocumentFragmentType, string>> = {
            [DocumentFragmentType.Image]: 'image',
            [DocumentFragmentType.Embed]: 'play_circle',
        };

        return icons[this.type()] ?? 'question_mark';
    });
}

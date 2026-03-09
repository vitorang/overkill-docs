import { Component, input } from '@angular/core';

type FragmentCollectionType = 'chat' | 'document'

@Component({
    selector: 'okd-fragment-collection',
    imports: [],
    templateUrl: './fragment-collection.component.html',
    styleUrl: './fragment-collection.component.scss',
})
export class FragmentCollectionComponent {
    type = input.required<FragmentCollectionType>();
}

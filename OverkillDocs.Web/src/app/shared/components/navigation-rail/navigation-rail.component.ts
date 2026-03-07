import { Component, input, model } from '@angular/core';
import { SHARED_NATIVE } from '../..';

export interface NavigationRailItem<T> {
    icon: string,
    activeIcon?: string,
    label: string,
    value: T
}

export type NavigationRailOrientation = 'vertical' | 'horizontal'

@Component({
    selector: 'okd-navigation-rail',
    imports: [SHARED_NATIVE],
    templateUrl: './navigation-rail.component.html',
    styleUrl: './navigation-rail.component.scss',
})
export class NavigationRailComponent<T> {
    items = input.required<NavigationRailItem<T>[]>();
    orientation = input.required<NavigationRailOrientation>();
    value = model.required<T>();
}

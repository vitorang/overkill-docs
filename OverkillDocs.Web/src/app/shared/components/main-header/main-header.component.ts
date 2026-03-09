import { Component } from '@angular/core';
import { SHARED_NATIVE } from '../..';
import { BrandComponent } from '../brand/brand.component';

@Component({
    selector: 'okd-main-header',
    imports: [SHARED_NATIVE, BrandComponent],
    templateUrl: './main-header.component.html',
    styleUrl: './main-header.component.scss',
})
export class MainHeaderComponent {

}

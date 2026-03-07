import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NavigationRailComponent } from './navigation-rail.component';

describe('NavigationRailComponent', () => {
    let component: NavigationRailComponent<string>;
    let fixture: ComponentFixture<NavigationRailComponent<string>>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [NavigationRailComponent]
        })
            .compileComponents();

        fixture = TestBed.createComponent(NavigationRailComponent<string>);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});

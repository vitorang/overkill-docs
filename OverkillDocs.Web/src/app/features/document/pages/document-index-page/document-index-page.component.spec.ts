import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentIndexPageComponent } from './document-index-page.component';

describe('DocumentIndexPageComponent', () => {
    let component: DocumentIndexPageComponent;
    let fixture: ComponentFixture<DocumentIndexPageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [DocumentIndexPageComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(DocumentIndexPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});

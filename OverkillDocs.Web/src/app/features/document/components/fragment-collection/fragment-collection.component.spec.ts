import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FragmentCollectionComponent } from './fragment-collection.component';

describe('FragmentCollectionComponent', () => {
  let component: FragmentCollectionComponent;
  let fixture: ComponentFixture<FragmentCollectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FragmentCollectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FragmentCollectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

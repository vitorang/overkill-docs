import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AvatarCustomizationDialogComponent } from './avatar-customization-dialog.component';

describe('AvatarCustomizationDialogComponent', () => {
  let component: AvatarCustomizationDialogComponent;
  let fixture: ComponentFixture<AvatarCustomizationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvatarCustomizationDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AvatarCustomizationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

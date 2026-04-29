import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReconnectionOverlayComponent } from './reconnection-overlay.component';

describe('ReconnectionOverlayComponent', () => {
  let component: ReconnectionOverlayComponent;
  let fixture: ComponentFixture<ReconnectionOverlayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReconnectionOverlayComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReconnectionOverlayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

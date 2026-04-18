import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HubMonitorComponent } from './hub-monitor.component';

describe('HubMonitorComponent', () => {
    let component: HubMonitorComponent;
    let fixture: ComponentFixture<HubMonitorComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [HubMonitorComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(HubMonitorComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});

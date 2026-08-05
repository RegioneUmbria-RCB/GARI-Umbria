import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CatastoZoneEditComponent } from './catasto-zone-edit.component';

describe('CatastoZoneEditComponent', () => {
    let component: CatastoZoneEditComponent;
    let fixture: ComponentFixture<CatastoZoneEditComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ CatastoZoneEditComponent ]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(CatastoZoneEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});

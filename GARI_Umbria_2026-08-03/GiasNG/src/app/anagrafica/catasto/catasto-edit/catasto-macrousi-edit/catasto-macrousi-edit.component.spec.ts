import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CatastoMacrousiEditComponent } from './catasto-macrousi-edit.component';

describe('CatastoMacrousiEditComponent', () => {
    let component: CatastoMacrousiEditComponent;
    let fixture: ComponentFixture<CatastoMacrousiEditComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ CatastoMacrousiEditComponent ]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(CatastoMacrousiEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});

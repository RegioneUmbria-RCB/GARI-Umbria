import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CatastoClassamentoEditComponent } from './catasto-classamento-edit.component';

describe('CatastoClassamentoEditComponent', () => {
    let component: CatastoClassamentoEditComponent;
    let fixture: ComponentFixture<CatastoClassamentoEditComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ CatastoClassamentoEditComponent ]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(CatastoClassamentoEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});

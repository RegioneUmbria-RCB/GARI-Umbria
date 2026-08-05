import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CatastoPossessiEditComponent } from './catasto-possessi-edit.component';

describe('CatastoPossessiEditComponent', () => {
    let component: CatastoPossessiEditComponent;
    let fixture: ComponentFixture<CatastoPossessiEditComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ CatastoPossessiEditComponent ]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(CatastoPossessiEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});

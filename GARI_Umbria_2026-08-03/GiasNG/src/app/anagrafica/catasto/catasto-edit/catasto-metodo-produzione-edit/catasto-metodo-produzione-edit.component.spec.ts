import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CatastoMetodoProduzioneEditComponent } from './catasto-metodo-produzione-edit.component';

describe('CatastoMetodoProduzioneEditComponent', () => {
    let component: CatastoMetodoProduzioneEditComponent;
    let fixture: ComponentFixture<CatastoMetodoProduzioneEditComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ CatastoMetodoProduzioneEditComponent ]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(CatastoMetodoProduzioneEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});

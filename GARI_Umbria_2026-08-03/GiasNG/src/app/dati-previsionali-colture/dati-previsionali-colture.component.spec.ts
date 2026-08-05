import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DatiPrevisionaliColtureComponent } from './dati-previsionali-colture.component';

describe('DatiPrevisionaliColtureComponent', () => {
  let component: DatiPrevisionaliColtureComponent;
  let fixture: ComponentFixture<DatiPrevisionaliColtureComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DatiPrevisionaliColtureComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DatiPrevisionaliColtureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

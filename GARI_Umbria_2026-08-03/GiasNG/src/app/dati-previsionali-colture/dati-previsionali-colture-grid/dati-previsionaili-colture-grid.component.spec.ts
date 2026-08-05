import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DatiPrevisionailiColtureGridComponent } from './dati-previsionaili-colture-grid.component';

describe('DatiPrevisionailiColtureGridComponent', () => {
  let component: DatiPrevisionailiColtureGridComponent;
  let fixture: ComponentFixture<DatiPrevisionailiColtureGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DatiPrevisionailiColtureGridComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DatiPrevisionailiColtureGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GruppiRaccoltaComponent } from './gruppi-raccolta.component';

describe('GruppiRaccoltaComponent', () => {
  let component: GruppiRaccoltaComponent;
  let fixture: ComponentFixture<GruppiRaccoltaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GruppiRaccoltaComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GruppiRaccoltaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

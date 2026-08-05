import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FiltriComponent } from './filtri.component';

describe('FiltriComponent', () => {
  let component: FiltriComponent;
  let fixture: ComponentFixture<FiltriComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FiltriComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FiltriComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

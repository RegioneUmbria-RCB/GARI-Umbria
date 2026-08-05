import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestAnagraficaComponent } from './test-anagrafica.component';

describe('TestAnagraficaComponent', () => {
  let component: TestAnagraficaComponent;
  let fixture: ComponentFixture<TestAnagraficaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TestAnagraficaComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TestAnagraficaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

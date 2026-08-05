import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestAnagraficaImpiantiComponent } from './test-anagrafica-impianti.component';

describe('TestAnagraficaImpiantiComponent', () => {
  let component: TestAnagraficaImpiantiComponent;
  let fixture: ComponentFixture<TestAnagraficaImpiantiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TestAnagraficaImpiantiComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TestAnagraficaImpiantiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

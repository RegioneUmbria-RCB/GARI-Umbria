import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestAnagraficaCampiComponent } from './test-anagrafica-campi.component';

describe('TestAnagraficaCampiComponent', () => {
  let component: TestAnagraficaCampiComponent;
  let fixture: ComponentFixture<TestAnagraficaCampiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TestAnagraficaCampiComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TestAnagraficaCampiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestAnagraficaImpreseComponent } from './test-anagrafica-imprese.component';

describe('TestAnagraficaImpreseComponent', () => {
  let component: TestAnagraficaImpreseComponent;
  let fixture: ComponentFixture<TestAnagraficaImpreseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TestAnagraficaImpreseComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TestAnagraficaImpreseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

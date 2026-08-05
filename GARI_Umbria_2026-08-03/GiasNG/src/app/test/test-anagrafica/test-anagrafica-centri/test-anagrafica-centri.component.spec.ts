import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestAnagraficaCentriComponent } from './test-anagrafica-centri.component';

describe('TestAnagraficaCentriComponent', () => {
  let component: TestAnagraficaCentriComponent;
  let fixture: ComponentFixture<TestAnagraficaCentriComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TestAnagraficaCentriComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TestAnagraficaCentriComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

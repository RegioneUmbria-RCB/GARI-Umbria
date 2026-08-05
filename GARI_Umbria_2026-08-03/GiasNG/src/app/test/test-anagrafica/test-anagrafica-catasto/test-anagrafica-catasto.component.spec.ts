import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestAnagraficaCatastoComponent } from './test-anagrafica-catasto.component';

describe('TestAnagraficaCatastoComponent', () => {
  let component: TestAnagraficaCatastoComponent;
  let fixture: ComponentFixture<TestAnagraficaCatastoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TestAnagraficaCatastoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TestAnagraficaCatastoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

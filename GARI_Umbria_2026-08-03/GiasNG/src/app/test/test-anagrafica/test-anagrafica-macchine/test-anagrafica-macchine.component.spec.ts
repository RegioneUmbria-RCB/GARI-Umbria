import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestAnagraficaMacchineComponent } from './test-anagrafica-macchine.component';

describe('TestAnagraficaMacchineComponent', () => {
  let component: TestAnagraficaMacchineComponent;
  let fixture: ComponentFixture<TestAnagraficaMacchineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TestAnagraficaMacchineComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TestAnagraficaMacchineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

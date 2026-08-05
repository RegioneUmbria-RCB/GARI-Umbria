import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MacchineEditCaratteristicheComponent } from './macchine-edit-caratteristiche.component';

describe('MacchineEditCaratteristicheComponent', () => {
  let component: MacchineEditCaratteristicheComponent;
  let fixture: ComponentFixture<MacchineEditCaratteristicheComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MacchineEditCaratteristicheComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MacchineEditCaratteristicheComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

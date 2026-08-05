import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetGridComponent } from './widget-grid.component';

describe('PreferitiConfigComponent', () => {
  let component: WidgetGridComponent;
  let fixture: ComponentFixture<WidgetGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WidgetGridComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WidgetGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

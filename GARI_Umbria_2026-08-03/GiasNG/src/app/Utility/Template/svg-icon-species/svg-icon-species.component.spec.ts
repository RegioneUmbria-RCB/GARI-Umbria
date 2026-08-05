import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SvgIconSpeciesComponent } from './svg-icon-species.component';

describe('SvgIconComponent', () => {
  let component: SvgIconComponent;
  let fixture: ComponentFixture<SvgIconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SvgIconComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SvgIconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

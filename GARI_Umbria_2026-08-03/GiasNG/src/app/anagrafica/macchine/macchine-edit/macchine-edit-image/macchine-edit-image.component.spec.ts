import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MacchineEditImageComponent } from './macchine-edit-image.component';

describe('MacchineEditImageComponent', () => {
  let component: MacchineEditImageComponent;
  let fixture: ComponentFixture<MacchineEditImageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MacchineEditImageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MacchineEditImageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

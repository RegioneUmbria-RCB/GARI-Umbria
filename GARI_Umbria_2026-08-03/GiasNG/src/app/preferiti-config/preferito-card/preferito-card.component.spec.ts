import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PreferitoCardComponent } from './preferito-card.component';

describe('PreferitiConfigComponent', () => {
  let component: PreferitoCardComponent;
  let fixture: ComponentFixture<PreferitoCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PreferitoCardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PreferitoCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

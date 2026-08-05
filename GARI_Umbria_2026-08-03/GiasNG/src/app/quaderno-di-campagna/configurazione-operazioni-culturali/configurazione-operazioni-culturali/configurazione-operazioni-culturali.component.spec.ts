import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurazioneOperazioniCulturaliComponent } from './configurazione-operazioni-culturali.component';

describe('ConfigurazioneOperazioniCulturaliComponent', () => {
  let component: ConfigurazioneOperazioniCulturaliComponent;
  let fixture: ComponentFixture<ConfigurazioneOperazioniCulturaliComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConfigurazioneOperazioniCulturaliComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConfigurazioneOperazioniCulturaliComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

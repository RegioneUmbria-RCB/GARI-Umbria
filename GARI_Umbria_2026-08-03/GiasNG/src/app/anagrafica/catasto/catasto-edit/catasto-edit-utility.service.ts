import {FormBuilder, FormControl, FormGroup, Validators} from '@angular/forms';
import {AGRODATAFINE, AGRODATAINIZIO} from '../../../Model/CostantiPersonalizzate';
import {Injectable} from '@angular/core';

@Injectable()
export class CatastoEditUtilityService {

  constructor(
    private fb: FormBuilder
  ) {

  }
  public getFormParticella(): FormGroup{
    return this.fb.group({
      primaryKey: this.getFormParticellaPK(),
      Area: [0, [Validators.required, Validators.min(0.0001)]],
      macrousi: this.fb.array([]),
      classamento: this.fb.array([]),
      metodoProduzione: this.fb.array([]),
      zonizzazione: this.fb.array([]),
      proprietario: ''
    });
  }

  public getFormParticellaPK(): FormGroup {
    return this.fb.group({
      Prov: ['', Validators.required],
      Com: ['', Validators.required],
      Sezione: ['', Validators.maxLength(2)],
      Foglio: [undefined, [Validators.required, Validators.min(1)]],
      Numero: [undefined, [Validators.required, Validators.min(1)]],
      Subalterno: ['', Validators.maxLength(3)]
    });
  }

  public getRowPossessiParticella() {
    return this.fb.group({
      Area: [0],
      codice: [0],
      titolo_Di_Possesso: new FormControl({ codice: 0, descrizione: '' }),
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      }),
      codice_particella: ['']
    });
  }

  public getRowMetodoProduzioneParticella() {
    return this.fb.group({
      metodoProduzione: new FormControl({ codice: 0, descrizione: '' }),
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      })
    });
  }

  public getRowMacrousoParticella() {
    return this.fb.group({
      macrouso: new FormControl({ codice: 0, descrizione: '' }),
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      }),
      Area: [0],
      Piva: [''],
      NumeroFascicolo: [''],
      DataValidazioneFascicolo: [AGRODATAINIZIO]
    });
  }

  public getRowClassamentoParticella() {
    return this.fb.group({
      qualita: new FormControl({ codice: 0, descrizione: '' }),
      Area: [0],
      porzione: [''],
      classe: [''],
      redditoDomiciliare: [''],
      redditoAgrario: ['']
    });
  }

  public getRowZonaParticella() {
    return this.fb.group({
      zona: new FormControl({ codice: 0, descrizione: '' }),
      Area: [0],
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      }),
    });
  }
}

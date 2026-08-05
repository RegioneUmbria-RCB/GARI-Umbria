import { Injectable } from "@angular/core";
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { Immagine } from "app/Model/anagrafiche/Immagine";
import { CaratteristicaDettagli, MacchinaGerarchia, ParcoMacchine } from "app/Model/anagrafiche/ParcoMacchine";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { AGRODATAFINE, AGRODATAINIZIO, LAVCOD_MIETITREBBIATURA } from "app/Model/CostantiPersonalizzate";
import { UnitaDiMisura } from "app/Model/metaschema/UnitaDiMisura";
import {Contatto} from '../../../Model/anagrafiche/Contatto';
import {IntervalloTemporale} from '../../../Model/anagrafiche/IntervalloTemporale';

export function forbiddenBrandCodeValidator(control: AbstractControl) {
  if (control.value.codice == undefined || control.value.codice == -1) {
    return { 'brandCode': true };
  }
  return null;
}

export function forbiddenCLASS_CODEValidator(control: AbstractControl) {
  if (control.value.codice == undefined || control.value.codice == '') {
    return { 'CLASS_CODE': true };
  }
  return null;
}

export function forbiddenPurposeCodeValidator(control: AbstractControl) {
  if (control.value.codice == undefined || control.value.codice == -1) {
    return { 'brandCode': true };
  }
  return null;
}

export function checkDatesValidator(control: AbstractControl) {
    const dataInizio = control?.parent?.get('Data_Inizio_Installazione')?.value;
    const dataFine = control.parent?.get('Data_Fine_Installazione')?.value;
    if (dataFine && dataInizio && new Date(dataFine) < new Date(dataInizio)) {
        return { invalidDateRange: true };
    }
    // console.log(control);
    control?.parent?.get('Data_Inizio_Installazione')?.setErrors(null);
    control?.parent?.get('Data_Fine_Installazione')?.setErrors(null);
    return null; // Validazione riuscita
}

export function latitudineValidator(control: AbstractControl) {
    const value = control?.parent?.get('Latitudine_Installazione')?.value;

    if (value == null || value === '') {
      return null; // non valido "vuoto", lascia ad un eventuale required
    }

    const num = parseFloat(value);
    if (isNaN(num) || num < -90 || num > 90) {
      return { latitudine: true };
    }

    return null;
}

export enum enumVisibilitaMacchina {
  PRIVATA = 0,
  PUBBLICA = -1
}

export function longitudineValidator(control: AbstractControl) {

  const value = control?.parent?.get('Longitudine_Installazione')?.value;

    if (value == null || value === '') {
      return null;
    }

    const num = parseFloat(value);
    if (isNaN(num) || num < -180 || num > 180) {
      return { longitudine: true };
    }

    return null;
}

@Injectable({
  providedIn: 'root'
})
export class MacchinaEditFormsService {

  getMacchinaForm() {
    return this.fb.group({
      // Dati Macchina
      partitaIva: [''],
      codice: 0,
      centroPK: this.fb.group({
        codice: 0,
        partitaIva: ''
      }),

      visibilitaPubblica: [{
        value: {
          codice: false,
          descrizione: 'Macchina Aziendale'
        }
      }],
      visibileControlloGestione: [false],
      descrizione: [''],
      marca: new FormControl({codice: 0, descrizione: ''}),
      modello: [''],

      finalita: new FormControl({codice: 0, descrizione: ''},forbiddenPurposeCodeValidator),

      tipo: new FormControl({codice: '01', descrizione: ''},forbiddenCLASS_CODEValidator),
      dettaglio_1: new FormControl({codice: '', descrizione: ''}),
      dettaglio_2: new FormControl({codice: '', descrizione: ''}),

      ageaCod: new FormControl({codice: '', descrizione: ''}),

      codice_stringa: [''],
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      }),
      Data_Carico: AGRODATAINIZIO,
      Data_Scarico: AGRODATAFINE,
      titolo_Possesso: new FormControl({codice: 0, descrizione: ''}),
      proprietario: [''],
      CUAA_Proprietario: [''],

      // Dati Tecnici
      targa: ['', Validators.maxLength(10)],
      tipo_Targa: new FormControl({codice: 0, descrizione: ''}),

      telaio: [''],
      n_Immatricolazione: [''],
      data_Immatricolazione: AGRODATAINIZIO,
      n_Immatricolazione_Rimorchio: [''],
      N_Autorizzazione_Trasporto: [''],
      data_Rilascio_Autorizzazione: AGRODATAINIZIO,
      alimentazione: new FormControl({codice: 0, descrizione: ''}),

      data_Ultima_Revisione: AGRODATAINIZIO,
      data_Ultima_Manutenzione: AGRODATAINIZIO,

      immagineGrande: new Immagine("","",""),
      immaginePiccola: new Immagine("","",""),

      VIN: "",
      BTM_Serial: "",
      ExternalAPIKey: "",

      HubIoT_PlatformDestination: new FormControl({codice: 0, descrizione: ''}),

      caratteristiche: this.fb.array([]),
      gerarchiaFigli: this.fb.array([]),
      gerarchiaPadre: new MacchinaGerarchia(0, 0, null, 0, '', '', 0, '', 0, new IntervalloTemporale()),

      rateiTempo: this.fb.array([]),

      // 'Taratura Ugello
      taratura_Ugello: 0,
      numero_certificato: ['', Validators.maxLength(20)],
      data_Ultima_Taratura: AGRODATAFINE,
      scadenza_Taratura: AGRODATAFINE,
      stato_Utilizzo: [''],
      potenza: [''],
      unita_Misura: new FormControl({codice: 0, descrizione: ''}),
      note: [''],
      contatto: new Contatto(),
      // Costi e ammortamento
      costi: this.fb.array([]),
      efficienza: 0,
      portata: 0,
      codice_impianto: '',

      //stazioni meteo
      Distinta_Installazione: "",
      Contratto_Installazione: "",
      Tipologia_Installazione: "",
      Data_Inizio_Installazione: new FormControl(new Date(), checkDatesValidator),
      Data_Fine_Installazione: new FormControl(new Date(), checkDatesValidator),
      Stato_Installazione: null,
      Provincia_Istat_Installazione: null,
      Comune_Istat_Installazione: null,
      Indirizzo_Installazione: "",
      Latitudine_Installazione: [0, latitudineValidator],
      Longitudine_Installazione: [0, longitudineValidator]
    });
  }

  getCostoUnitarioChiaveForm() {
    return this.fb.group({
      chiave: 0,
      codice: 0,
      prezzo: 0,
      flag_cancellazione: false,
      unitaDiMisura: new FormControl({codice: 0, descrizione: ''}),
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      })
    })
  }

  getCaratteristicaForm() {
    return this.fb.group({
      codice: 0,
      caratteristica: new CaratteristicaDettagli(0, ""),
      valore: "",
      validita_inizio: [AGRODATAINIZIO],
      validita_fine: [AGRODATAFINE]
    })
  }

  getMacchinaGerarchiaForm() {
    return this.fb.group({
      ID: 0,
      codice: 0,
      macchina: new ParcoMacchine(),
      legame: new BaseCodeDescr(0, "Legame_Des"),
      desclegame:  "",
      udm: new UnitaDiMisura(0, "UdM_Des"),
      qta : 0,
      validita_inizio: [AGRODATAINIZIO],
      validita_fine: [AGRODATAFINE]
    })
  }

  getRateoTempoForm() {
    return this.fb.group({
        Rateo_Cod: 0,
        DataInizio: new Date(),
        DataFine: new Date(),
        OraInizio: new Date(),
        OraFine: new Date(),
        Rotazione: 0,
        ValiditaInizio: AGRODATAINIZIO,
        ValiditaFine: AGRODATAFINE
    });
  }

  constructor(private fb: FormBuilder){}

}

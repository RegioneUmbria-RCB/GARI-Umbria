import { Injectable } from "@angular/core";
import {AbstractControl, FormBuilder, FormControl, ValidationErrors, ValidatorFn, Validators} from '@angular/forms';
import { disableDebugTools } from "@angular/platform-browser";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { DestinazioneUso } from "app/Model/metaschema/utilizzi/DestinazioneUso";
import { Varieta } from "app/Model/metaschema/utilizzi/Varieta";
import { BaseCodeDescr, ValiditaValidator } from 'gias-ui-kit';
import {BehaviorSubject} from "rxjs";
import {LinkedContribute} from '../../../Model/metaschema/Contribute';
import {Pair} from '@progress/kendo-data-query/dist/npm/utils';
import { ParcoMacchine } from "app/Model/anagrafiche/ParcoMacchine";
import { SharedDataService } from "app/GIS/services/shared-data.service";
import { Configurazione_Siti, ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from "app/Service/configurazione-siti.service";

export function forbiddenUtilizzoTerrenoValidator(control: AbstractControl) {
  switch (control.value?.classType) {
    case 'Varieta':
      let varieta = <Varieta>control.value;
      if (varieta.specie.codice == 0) {
        return { UtilizzoTerrenoNoSpecie: true };
      }
      if (varieta.codice == 0) {
        return { UtilizzoTerrenoNoVarieta: true };
      }
      break;
    case 'DestinazioneUso':
      let destinazioneUso = <DestinazioneUso>control.value;
      if (destinazioneUso.codice == 0) {
        return { UtilizzoTerrenoNoDestinazione: true };
      }
      break;
  }
  if (control.value.codice == undefined || control.value.codice == 0 || control.value.classType == '') {
    return { 'UtilizzoTerreno': true };
  }
  return null;
}

export function validateBaseCodeDescr(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value as BaseCodeDescr;
    return (value && value.codice && value.codice != 0) ? null : { 'required': true };
  };
}

@Injectable({
  providedIn: 'root'
})
export class AppezzamentoEditService {
  public savings: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  getAppezzamentoForm() {
    let _ValiditaValidator = new ValiditaValidator();
    return this.fb.group({
      primaryKey: this.fb.group({
        codice: 0,
        centroAziendalePK: this.fb.group({
          codice: 0,
          partitaIva: ''
        })
      }),
      campoPK: this.fb.group({
        centroAziendalePK: this.fb.group({
          codice: 0,
          partitaIva: ''
        }),
        codice: [0]
      }),
      immagineBase64: [''],
      metodo_Produzione: new FormControl({codice: 1, descrizione: 'Convenzionale'}),
      descrizione: new FormControl(''),
      superficie: new FormControl(0, [Validators.required, Validators.min(0.0001)]),
      superficieGis:[0],
      validita: this.fb.group(
        {
          inizio: [AGRODATAINIZIO],
          fine: [AGRODATAFINE]
        }, {
          validator: _ValiditaValidator.validate.bind(this)
        }
      ),
      rif_Appezzamento:[''],
      isola:[''],
      coltura_Precedente_1_Anno: new FormControl({codice: 0, descrizione: ''}),
      coltura_Precedente_2_Anno: new FormControl({codice: 0, descrizione: ''}),
      coltura_Precedente_3_Anno: new FormControl({codice: 0, descrizione: ''}),
      coltura_Precedente_4_Anno: new FormControl({codice: 0, descrizione: ''}),
      pendenza:[0],
      esposizione: new FormControl({codice: '', descrizione: ''}),
      ubicazione: new FormControl({codice: '', descrizione: ''}),
      lat:[0],
      lng:[0],
      altitudine:[0],
      distBZ_CorpiIdrici:[0],
      distBZ_AreeResPub:[0],
      distBZ_Allevamenti:[0],
      distBZ_VegNatNonColt:[0],
      supBZ_Riduzione: [0],
      confini_A_Rischio: [''],
      fine_Impiego_Prod_Non_Conformi: [AGRODATAINIZIO],
      n_App_Bio: [''],
      utilizzo_Terreno: [],
      impianti: this.fb.array([]),
      indirizzi: this.fb.array([]),
      catastoAppezzamento: new FormControl([]),
      codici: new FormControl([]),
      terrenoInutilizzato: false,
      terrenoDegradato: false,
      lowILUC: false,
      sabbia: [],
      limo: [],
      argilla: [],
      classeTessitura: this.fb.group({
        codice: new FormControl(0),
        descrizione: new FormControl({value: '', disabled: true})
      }),
      Agea_idSchedaValidazione: '',
      Agea_identificativoPianoColtivazione: '',
      Agea_codiBarrScheVali: '',
      Agea_identificativoIsola: '',
      Agea_identificativoAppezzamento: '',
      Agea_idAppezzamentoOrig: '',
    });
  }

  getImpiantoForm(id: number, appezzamentoKey: any, validitaInizio: Date, validitaFine: Date, isSementieri: boolean) {

    let _ValiditaValidator = new ValiditaValidator();

    const impiantoForm = this.fb.group({
      id: id,
      primaryKey: {
        codice: 0,
        appezzamentoPK: appezzamentoKey
      },
      descrizione: [''],
      immagineBase64: [''],
      utilizzoTerreno: new FormControl({codice: 0, descrizione: '', specie: { codice: 0, descrizione: '' }, classType:'Varieta' }, forbiddenUtilizzoTerrenoValidator),
      superficie:[0, [Validators.required, Validators.min(0.0001)]],
      superficieGis:[0],
      gruppoFinalita: new FormControl({ codice: 0, descrizione: '' }),
      gruppoVarietale: new FormControl({ codice: 0, descrizione: '' }),
      data_Innesto_Varieta: [AGRODATAINIZIO],
      data_Inizio_Produzione: [AGRODATAINIZIO],
      data_Inizio_Impianto: [AGRODATAINIZIO],
      algoritmoCodifica: '',
      codiceImpianto: [''],
      cover_Crops: [false],
      monitorato: [false],
      // imp_Cod: [0],
      irrigazione: new FormControl({codice: 0, descrizione: ''}),
      macchineIrrigazione: new FormControl<Array<ParcoMacchine>>([]),
      formaAllevamento: new FormControl({codice: 0, descrizione: ''}),
      portinnesto: new FormControl({ codice: 0, descrizione: '' }),
      data_Inizio_Portinnesto: [AGRODATAINIZIO],
      seminaTrapianto: new FormControl({codice: 0, descrizione: ''}),
      provenienzaSeme: new FormControl({codice: 0, descrizione: ''}),
      tecnicaConduzioneTraFila: new FormControl({codice: 0, descrizione: ''}),
      tecnicaConduzioneSuFila: new FormControl({codice: 0, descrizione: ''}),
      // infoAgg_Cod: [0],
      dettaglio_varieta_personalizzato: new FormControl({codice: 0, descrizione: ''}),
      codiceZona: new FormControl({codice: '', descrizione: ''}),
      maschi_in_Sesto: [false],
      sup_Netta_Coltivata: [0],
      tra_Fila_M: [0],
      su_Fila_M: [0],
      copertura: new FormControl({codice: 0, descrizione: ''}),
      cop_Data_Inizio: [AGRODATAINIZIO],
      cop_Data_Fine: [AGRODATAFINE],
      unita_Vitata: [0],
      impiantoConsociato: [false],
      resaStoricaPrevista: [0],
      resaStoricaCorretta: [0],
      impianto_Ibrido: [{value: false, disabled: true}],
      codBMBDBT_M: [''],
      codBMBDBT_F: [''],
      genetica_M: [''],
      genetica_F: [''],
      offType_M: [''],
      offType_F: [''],
      distanzaSuFila_F: [0],
      distanzaTraFila_F: [0],
      partiTuberi: [0],
      tagliatoIntero: new FormControl({codice: 0, descrizione: ''}),
      interbina: [0],
      germinabilita: [100],
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      }, {
        validator: _ValiditaValidator.validate.bind(this)
      }),
      piante_Ha: [{value: 0, disabled: true}],
      piante_Impianto: [{value: 0, disabled: true}],
      // esercizi: this.fb.array([this.getEsercizioForm(0, {codice: 0, appezzamentoPK: appezzamentoKey}, validitaInizio, validitaFine)])
      esercizi: this.fb.array([]),
      unitaMisuraAlternativa: new FormControl({ codice: 0, descrizione: '', simbolo: null, tassoConversione: 0 }),
      superficieAlternativa: [0],
      Agea_idColt: '',
      usaMacchineIrrigazioneAnagrafica : false, // Indica se leggere anche l'anagrafioca macchine alla lettura degli impianti di irrigazione o meno
      flagImpiantoIsMacchina : false // Definisce se l'impianto d'irrigazione è stato scelto dall'anagrafica macchine
    });

    if (isSementieri) {
      impiantoForm.get('gruppoVarietale')?.setValidators([validateBaseCodeDescr()]);
      impiantoForm.get('gruppoVarietale')?.updateValueAndValidity();
    }
    return impiantoForm;
  }

  getEsercizioForm(id: number, impiantoKey: any, validitaInizio: Date, validitaFine: Date) {

    let _ValiditaValidator = new ValiditaValidator();

    return this.fb.group({
      id: id,
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      }, {
        validator: _ValiditaValidator.validate.bind(this)
      }),
      impiantoPK: impiantoKey,
      lotto: [''],
      piante_Ha: new FormControl(0, [Validators.nullValidator]),
      piante_Impianto: new FormControl(0, [Validators.nullValidator]),
      piante_Ha_Femmine: [0],
      Piante_Ha_Impianto_Femmine: [0],
      Piante_Ha_Maschi: [0],
      Piante_Ha_Impianto_Maschi: [0],
      data_Semina_Trapianto_Prevista: AGRODATAINIZIO,
      data_Raccolta_Prevista: AGRODATAFINE,
      data_Fioritura_Prevista: AGRODATAINIZIO,
      resa_prevista: [0],
      resa_totale_prevista: [0],
      regolamento: new FormControl({codice: 1, descrizione: 'Nessuno'}),
      //disciplinare: new FormControl({codice: '0', descrizione: '', disciplinarePrivato: false}),
      disciplinare: new FormControl({
        codice: 0,
        descizione: '',
        disciplinarePrivato: false,
        idTr: 3,
        regolamentoConcimazione: {
          codice: 0,
          descrizione: ''
        }
      }),
      id_tr: [3],
      iaf: [],
      capitolato_Privato: new FormControl({codice: 0, descrizione: ''}),
      organismo_Referente: new FormControl({codice: 0, descrizione: ''}),
      modalita_liquidazione: new FormControl({codice: 0, descrizione: ''}),
      origine_prodotto: new FormControl({codice: 0, descrizione: ''}),
      residuo: new FormControl({codice: 0, descrizione: ''}),
      certificazioneAziendale: new FormControl([]),
      certificazioneProdotto: new FormControl({codice: '', descrizione: ''}),
      contributi: new FormControl([]),
      tecnico: new FormControl([]),
      licenza_Coltivazione: new FormControl({codice: 0, descrizione: ''}),
      riferimento_Trasferimento_Dati: new FormControl({codice: 0, descrizione: ''}),
      magazzino_Conferimento: new FormControl({codice: 0, descrizione: ''}),
      piano_Semina: new FormControl({codice: 0, descrizione: ''}),
      esercizio_Chiuso: {value: false, disabled: true},
      flagSecondoRaccolto: [false],
      apportiMassimiMacroelementi: this.fb.group({
        pianoConcimazione: new FormControl({codice: 0, descrizione: ''}),
        tipologia: new FormControl({codice: 0, descrizione: ''}),
        fase: new FormControl({codice: 0, descrizione: ''}),
        trasformati: new FormControl({codice: 0, descrizione: ''}),
        n: [0],
        p2o5: [0],
        k2o: [0],
        mgo: [0]
      }),
      lavorazione: new FormControl({codice: "", descrizione: ''}),
      specifica: new FormControl({codice: "", descrizione: ''}),
      vincolo: [],
      catastoEsercizio: [],
      codici: new FormControl([]),
      codice: [{value: 0, disabled: true}],
      descrizione: [''],
      prodotto: new FormControl({codice: 0, descrizione: ''}),
      gruppoRaccolta: new FormControl([]),
      acaContributes: new FormControl<LinkedContribute<Pair<number, string>>[]>([])
    });
  }

  getIndirizzoAssociatoForm() {
    return this.fb.group({
      tipo_indirizzo: [1],
      indirizzo: this.getIndirizzoForm()
    });
  }

  private getIndirizzoForm() {
    return this.fb.group({
      codice: [0],
      via: [''],
      frazione: [''],
      cap: ['00000'],
      note: [''],
      stato: new FormControl({ codice: 0, descrizione: '' }),
      istatComune: this.fb.group({
        prov: ['000'],
        com: ['000'],
        comuni_prov: [''],
        localita: [''],
        cap: ['']
      })
    });
  }

  constructor(
    private fb: FormBuilder,
    private configurazioneSitiService: ConfigurazioneSitiService
  ) {}
}
import { Injectable, InjectionToken } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import {Appezzamento, AppezzamentoJoinDescrizioni, PKAppezzamento} from 'app/Model/anagrafiche/Appezzamento';
import { PKCentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Impianto, PKImpianto } from 'app/Model/anagrafiche/Impianto';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { MetodoProduzione } from 'app/Model/metaschema/MetodoProduzione';
import { UtilizzoTerreno } from 'app/Model/metaschema/utilizzi/UtilizzoTerreno';
import { BehaviorSubject, Observable } from 'rxjs';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { ConversionService } from '../conversion.service';
import { MasterService, rispostaStandard, RispostaStandard } from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {AppezzamentoFiltroTemporale, Parametri_ObjParametriAgenda_NG} from '../api.service';
import {AGRODATAFINE, AGRODATAINIZIO} from '../../Model/CostantiPersonalizzate';
import {LeggiInvestimentoCatastale} from './investimento-catastale.factory.service';
import {Esercizio} from '../../Model/anagrafiche/Esercizio';
import {LinkedMachine} from '../../Model/anagrafiche/ParcoMacchine';
import {AppezzamentoXParcoMacchine} from '../../Model/anagrafiche/appezzamento-x-parco-macchine';
import { LeggiClasseTessitura } from 'app/Model/metaschema/LeggiClasseTessitura';
import { ClasseTessitura } from 'app/Model/anagrafiche/ClasseTessitura';

export class CaricaCatastoSettings {
  Piva: string;
  Sa_Cod: number;
  Campo_Cod: number;
  chkMacrousi: boolean;
  chkUtilizzi: boolean;
  chkVarieta: boolean;
  Validita_Inizio: Date;
  Validita_Fine: Date;
}

export class CaricaDatiCatastali {
  idBudget?: number = 0;
  parametri_ObjParametriAgenda_NG?: Parametri_ObjParametriAgenda_NG;
  Campo_Cod?: number = 0;
  flag_Macrousi?: boolean = false;
  flag_Utilizzi?: boolean = false;
  flag_Varieta?: boolean = false;
  ValiditaInizio?: Date = AGRODATAINIZIO;
  ValiditaFine?: Date = AGRODATAFINE;
}

export class EserciziRequest {
  impresa: Impresa;
  esercizi: any[];
  requestDate: Date;
}

export class EserciziRequestManager {
  impresexEserciziList: EserciziRequest[]
}

export const IMPIANTI_SERVICE_TOKEN = new InjectionToken<ImpiantiFactoryService>('app.impianti.service');

@Injectable()
export abstract class ImpiantiFactoryService {

  constructor(
    protected masterService: MasterService,
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected conversionService: ConversionService,
    protected intlService: IntlService
  ) {
    const centroPK = { partitaIva: '', codice: 0 } as PKCentroAziendale;
    const appezzamentoPK = { codice: 0, centroAziendalePK: centroPK };
  }

  protected centroPK = { partitaIva: '', codice: 0 } as PKCentroAziendale;
  protected appezzamentoPK = { codice: 0, centroAziendalePK: this.centroPK } as PKAppezzamento;

  protected Appezzamento: Appezzamento = new Appezzamento({
    codice: 0,
    centroAziendalePK: {
      codice: 0, partitaIva: ''
    }
  } as PKAppezzamento);

  protected AppezzamentoSource = new BehaviorSubject(this.Appezzamento);
  currentAppezzamento: Observable<Appezzamento> = this.AppezzamentoSource.asObservable();

  protected Array_Metodo_Produzione: Array<MetodoProduzione>=[
    {descrizione: 'Integrato',  codice: 1},
    {descrizione: 'In conversione', codice: 2},
    {descrizione: 'Biologico', codice: 3}
  ];

  protected impresexEsercizi: EserciziRequestManager = { impresexEserciziList: [] }

  abstract setAppezzamento(appezza: Appezzamento);

  abstract getAppezzamento(): Appezzamento;

  abstract getArray_Metodo_Produzione(): Array<any>;

  abstract leggiAppezzamenti(objParametri: ObjParametriAgenda): Promise<string>;

  abstract leggiAppezzamento(
    objParametri: ObjParametriAgenda,
    data: Date,
    filtroData: boolean,
    leggiIndirizzi: boolean,
    leggiCatasto: boolean,
    leggiImpianti: boolean,
    leggiDistinte: boolean,
    leggiCartografia: boolean,
    setIsLoading: boolean
  ): Promise<Appezzamento>;

  abstract leggiAppezzamentoObs(
    objParametri: ObjParametriAgenda,
    data: Date,
    filtroData: boolean,
    leggiIndirizzi: boolean,
    leggiCatasto: boolean,
    leggiImpianti: boolean,
    leggiDistinte: boolean,
    leggiCartografia: boolean
  ): Observable<Appezzamento>;

  /**
   * Reads a light version of an agricultural plot
   */
  abstract readAgriculturalPlotLight(objParametri: ObjParametriAgenda): Observable<AppezzamentoJoinDescrizioni>;

  abstract readAppezzamentiXParcoMacchine(params: AppezzamentoXParcoMacchine): Observable<LinkedMachine<PKAppezzamento>[]>;
  abstract writeAppezzamentiXParcoMacchine(app: Appezzamento[]): Observable<rispostaStandard<boolean>>;
  abstract editAppezzamentiXParcoMacchine(machines: LinkedMachine<PKAppezzamento>[]): Observable<rispostaStandard<boolean>>;
  abstract deleteAppezzamentiXParcoMacchine(axp: AppezzamentoXParcoMacchine): Observable<rispostaStandard<boolean>>;
  abstract deleteAppezzamentiXParcoMacchineRecords(plots: Appezzamento[]): Observable<rispostaStandard<boolean>>;

  abstract updateWeaving(plots: Appezzamento[]): Observable<rispostaStandard<boolean>>;
  abstract updateSlope(plots: Appezzamento[]): Observable<rispostaStandard<boolean>>;
  abstract updateConstrain(plots: Appezzamento[]): Observable<rispostaStandard<boolean>>;

  abstract leggiAppezzamentoUtilizzoTerreno(pkImpianto: PKImpianto): Promise<UtilizzoTerreno>;

  abstract CaricaDatiCatastaliObs(settings: CaricaCatastoSettings): Observable<any[]>;

  abstract leggiInvestimentoCatastale(settings: LeggiInvestimentoCatastale): Observable<any[]>;

  abstract CaricaDatiCatastali(
    objParametri: ObjParametriAgenda,
    Campo_Cod: number,
    flag_Macrousi: boolean,
    flag_Utilizzi: boolean,
    flag_Varieta: boolean, ValiditaInizio: Date,
    ValiditaFine: Date
  ): Promise<string>;

  abstract leggiClasseTessitura(leggiClasseTessitura: LeggiClasseTessitura): Promise<ClasseTessitura>;

  abstract leggiClasseTessituraObs(leggiClasseTessitura: LeggiClasseTessitura): Observable<ClasseTessitura>;

  abstract LeggiParticelle_Per_Appezzamento(objParametri: ObjParametriAgenda): Promise<string>;

  abstract Controlla_Validita_Appezzamenti(objParametri: ObjParametriAgenda): Promise<string>;

  abstract ScriviAppezzamento(appezzamento: Appezzamento): Promise<rispostaStandard<Appezzamento>>;

  abstract ScriviAppezzamento_Obs(appezzamento: Appezzamento, mostraErrori: boolean, deleteRibaltamento: boolean): Observable<rispostaStandard<Appezzamento>>;

  abstract ScriviAppezzamentoFiltroTemporale(appezzamentoFiltroTemporale: AppezzamentoFiltroTemporale): Observable<rispostaStandard<Appezzamento>>;

  abstract bloccaSbloccaAppezzamenti(appezzamenti: Appezzamento[], blocca: boolean): Observable<rispostaStandard<Appezzamento>>;

  abstract verifica_Superficie(impianto: Impianto, newSup: number): Observable<rispostaStandard<string>>;

  abstract verifica_OperazioniAgenda(impianto: Impianto): Observable<rispostaStandard<RispostaStandard>>;

  abstract leggi(objParametri: ObjParametriAgenda): Observable<Array<any>>;

  testLettura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {

    const parametri: CoreWS_Generic<ObjParametriAgenda> = {
      objP: this.masterService.getCoreWSGenericObjP(),
      InData: objParametriAgenda
    };

    return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
      this.masterService.link_CoreWS + '/Anagrafica/Appezzamento.asmx/Test_Appezzamenti_Archivio_Lettura',
      parametri);
  }

  testScrittura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {

    const parametri: CoreWS_Generic<ObjParametriAgenda> = {
      objP: this.masterService.getCoreWSGenericObjP(),
      InData: objParametriAgenda
    };

    return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
      this.masterService.link_CoreWS + '/Anagrafica/Appezzamento.asmx/Test_Appezzamenti_Archivio_Scrittura',
      parametri)
  }

  protected ripartoCatastoPresente: boolean = false;
  protected ripartoCatastoPresenteSource = new BehaviorSubject <boolean>(this.ripartoCatastoPresente);
  currentRipartoCatastoPresente: Observable<boolean> = this.ripartoCatastoPresenteSource.asObservable();

  abstract setRipartoCatastoPresente(flag: boolean);

  abstract getRipartoCatastoPresente(): boolean;

  abstract leggiGenerazionePoligoniDefaultValue(objParametri: ObjParametriAgenda): Observable<string>

  abstract getCodiceImpianto(piva: string, year: number): Observable<any>;

  abstract generateDescriptions(piva: string): Observable<any>;

  abstract closePlant(esercizio: Esercizio): Observable<rispostaStandard<string>>;
}

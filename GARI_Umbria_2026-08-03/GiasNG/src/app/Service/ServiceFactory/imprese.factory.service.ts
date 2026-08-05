import { InjectionToken } from "@angular/core";
import { CodiceAnagrafe } from "app/Model/anagrafiche/CodiceAnagrafe";
import { Contatto } from "app/Model/anagrafiche/Contatto";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { RisorseUmane } from "../../Model/anagrafiche/RisorseUmane";
import { CoreWS_Generic } from "app/Model/CoreWS/CoreWS_Generic";
import { AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { Codice } from "app/Model/ImpresaModel";
import { JsonKendoResult } from 'gias-kendo-grid';
import { NgxIndexedDBService } from "ngx-indexed-db";
import { BehaviorSubject, map, Observable, switchMap } from "rxjs";
import { AjaxAgronicaAPIService } from "../ajax-agronica.api.service";
import { AjaxAgronicaService } from "../ajax-agronica.service";
import { AnagraficaNGClient } from "../api.service";
import { MasterService, RispostaStandard, rispostaStandard } from "../master.service";
import { ObjParametriAgenda } from 'gias-ui-kit';
import {GruppoRaccolta} from '../../Model/metaschema/GruppoRaccolta';

export const IMPRESE_SERVICE_TOKEN = new InjectionToken<ImpreseFactoryService>('app.imprese.service');

export class Leggi_Tecnici_Request {
  piva: string;
  objP_server: string;
}

export class Leggi_Odc_Request {
  piva: string;
  objP_server: string;
}

export class Leggi_Padri {
}

export class Leggi_Imprese_Codici_Request {
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
}

export class Data {
  data: Date
}

export abstract class ImpreseFactoryService {

  protected unmodifiedData: any[];
  protected data: any[];
  protected id: string;
  protected saveOnRequest = true;
  protected imprese_codici: Array<any>;
  protected impresa: Impresa;

  protected padri: Impresa[] = [];

  protected imprese: JsonKendoResult;
  protected maxData: Date = AGRODATAINIZIO;

  protected caricaTutte: boolean = false;
  protected caricaTutteSource: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.caricaTutte);

  constructor(protected masterService: MasterService,
              protected ajaxAgronicaService: AjaxAgronicaService,
              protected ajaxApiService: AjaxAgronicaAPIService,
              protected dbService: NgxIndexedDBService,
              protected anagraficaNG: AnagraficaNGClient
  ) {
  }

  abstract leggiImprese(piva?: string, data?: Date): Observable<JsonKendoResult>;

  //abstract leggiCombo_Tecnici_Old(piva: string): Promise<rispostaStandard<Contatto[]>>;

  abstract leggiCombo_Tecnici(piva: string): Promise<rispostaStandard<Contatto[]>>;

  //abstract leggiCombo_OrganismiDiControllo_Old(piva: string): Promise<rispostaStandard<RisorseUmane[]>>;

  abstract leggiCombo_OrganismiDiControllo(piva: string): Promise<rispostaStandard<RisorseUmane[]>>;

  //abstract leggiPadri_Old(): Promise<Impresa[]>;

  abstract leggiPadri(): Promise<Impresa[]>;

  //abstract leggiImpresa_Old(objParametri: ObjParametriAgenda): Promise<rispostaStandard<Impresa>>;

  abstract leggiImpresa(objParametri: ObjParametriAgenda): Promise<rispostaStandard<Impresa>>;

  //abstract leggiImpresaObservable_Old(objParametri: ObjParametriAgenda): Observable<rispostaStandard<Impresa>>;

  abstract leggiImpresaObservable(objParametri: ObjParametriAgenda): Observable<rispostaStandard<Impresa>>;
  abstract setupLeggiImprese(agenda: ObjParametriAgenda);

  abstract leggi_Imprese_Codici();

  abstract leggi_Certificazioni();

  //abstract leggiImpreseCodici(agenda: ObjParametriAgenda): Observable<CodiceAnagrafe[]>;

  abstract leggiImpreseCodici(agenda: ObjParametriAgenda): Observable<CodiceAnagrafe[]>;

  abstract controlloPresenzaPiva(piva: string): Observable<string>;

  abstract impresaBiologica(piva: string): Observable<boolean>;

  abstract ScriviImpresa(impresa: Impresa, setLoading: boolean): Observable<rispostaStandard<Impresa>>;

  testLettura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {
    return this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, boolean>(
      'AnagraficaNG/Test_Imprese_Archivio_Lettura',
      objParametriAgenda);
  }

  /*testScrittura_Old(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {

      const parametri: CoreWS_Generic<ObjParametriAgenda> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: objParametriAgenda
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
          this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Test_Imprese_Archivio_Scrittura',
          parametri)
  }*/

  testScrittura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {
    return this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, boolean>(
      'AnagraficaNG/Test_Imprese_Archivio_Scrittura',
      objParametriAgenda)
  }

  setCaricaTutteImprese(carica: boolean) {
    this.caricaTutteSource.next(carica);
  }

  getCaricaTutteImprese(): boolean {
    return this.caricaTutteSource.getValue();
  }

  resetMaxData() {
    this.maxData = AGRODATAINIZIO;
  }

}

import { Injectable } from '@angular/core';
import { BehaviorSubject, lastValueFrom, Observable, of } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, RispostaStandard, rispostaStandard } from '../master.service';
import { ObjParametriAgendaService} from '../obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { JsonKendoResult } from 'gias-kendo-grid';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { RisorseUmane } from '../../Model/anagrafiche/RisorseUmane';
import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { NgxIndexedDBService } from 'ngx-indexed-db';
import { ImpreseFactoryService } from '../ServiceFactory/imprese.factory.service';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import {AnagraficaNGClient, ImpresaDto} from '../api.service';
import { ImpresaPadre } from 'app/Model/anagrafiche/ImpresaPadre';
import { GruppoRaccolta } from 'app/Model/metaschema/GruppoRaccolta';

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
  data: Date;
}

@Injectable({providedIn: 'root'})
export class ImpreseService extends ImpreseFactoryService {

  constructor(protected masterService: MasterService,
              protected ajaxAgronicaService: AjaxAgronicaService,
              protected ajaxApiService: AjaxAgronicaAPIService,
              protected dbService: NgxIndexedDBService,
              protected anagraficaNG: AnagraficaNGClient
  ) {
    super(masterService, ajaxAgronicaService, ajaxApiService, dbService, anagraficaNG)
  }

  leggiImprese(piva?: string, data?: Date): Observable<JsonKendoResult> {
    // if (this.maxData == AGRODATAINIZIO) {
    //   return this.fetchImprese(piva);
    // } else {
    //   const parametri: CoreWS_Generic<Object> = {
    //     objP: this.masterService.getCoreWSGenericObjP(),
    //     InData: ''
    //   };
    //   return this.Leggi_Max_DataModifica()
    //     .pipe(switchMap((maxDataModifica) => {
    //       if (this.maxData > maxDataModifica && this.imprese != null) {
    //         return of(this.imprese);
    //       } else {
    //         return this.fetchImprese(piva);
    //       }
    //     }))
    // }
    return this.fetchImprese(piva, data);
  }

  public leggiImpreseNoSuperfici(piva: string = '', data?: Date): Observable<JsonKendoResult> {
    return this.fetchImprese(piva, data, false);
  }

  public leggiImpreseDistinct(leggiSuperfici: boolean = false): Observable<JsonKendoResult> {
    return this.fetchImprese('', null, leggiSuperfici, true);
  }

  caricaCmbImprese(): Observable<ImpresaDto[]> {
    return this.ajaxApiService.ajaxAPIGet('AnagraficaNG/Carica_Cmb_Imprese','')
      .pipe(map(R => {
        return R.RispostaOK ? R.RispostaStringa as ImpresaDto[] : [];
      }))
  }

  private fetchImprese(piva: string, data: Date, leggiSuperfici: boolean = true, distinctPiva:boolean = false): Observable<JsonKendoResult> {
    this.maxData = new Date();
    let objParams = new ObjParametriAgenda();
    objParams.Piva = piva;

    if (data){
      objParams.Data = data;
    }

    objParams.GenericObj_string = JSON.stringify({
      leggiSuperfici: leggiSuperfici,
      distinctPiva: distinctPiva
    });

    return this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, any>(
      'AnagraficaNG/Imprese', objParams
    ).pipe(
        map((data) => {
        let imprese = new JsonKendoResult();
        const datiParse = data.RispostaStringa;

        imprese.kendo_columns = [];
        imprese.kendo_model = {};
        imprese.kendo_rows = datiParse;
        // this.dbService.clear('imprese');
        // this.dbService.add('imprese', imprese, 1).subscribe();
        this.imprese = imprese;
        return <JsonKendoResult>imprese;
      })
    );
  }

  private Leggi_Max_DataModifica(): Observable<Date> {
    return this.ajaxApiService.ajaxAPIPost<string, { data: Date }>(
      'AnagraficaNG/GetImpreseMaxDataModifica',
      '').pipe(map(
      (data) => {
        return data.RispostaStringa.data;
      })
    );
  }

  private customizeParametri(objParametri: ObjParametriAgenda) {
    objParametri.Piva = '';
    return objParametri;
  }

  leggiCombo_Tecnici(piva: string): Promise<rispostaStandard<Contatto[]>> {
    return new Promise<rispostaStandard<Contatto[]>>(async (resolve, reject) => {
      this.ajaxApiService.ajaxAPIPost<any, Contatto[]>('AnagraficaNG/CaricaComboCmbTecniciModello', {Ricerca: piva}).pipe(map(
        (data) => {
          resolve(data);
        })).subscribe();
    });
  }

  leggiCombo_OrganismiDiControllo(piva: string): Promise<rispostaStandard<RisorseUmane[]>> {
    return new Promise<rispostaStandard<RisorseUmane[]>>(async (resolve, reject) => {
      this.ajaxApiService.ajaxAPIPost<any, RisorseUmane[]>('AnagraficaNG/CaricaComboCmbOrganismiODCModello', {Ricerca: piva}).pipe(map(
        (data) => {
          resolve(data);
        })).subscribe();
    });
  }

  leggiPadri(): Promise<ImpresaPadre[]> {
    return new Promise<ImpresaPadre[]>(async (resolve, reject) => {
      this.ajaxApiService.ajaxAPIPost<Leggi_Padri, ImpresaPadre[]>('AnagraficaNG/LeggiImpresePadri',
        {}, false).pipe(map(data => {
        resolve(data.RispostaStringa)
      })).subscribe();
    });
  }

  leggiPadreSementieri(): Promise<ImpresaPadre> {
    return new Promise<ImpresaPadre>(async (resolve, reject) => {
      this.ajaxApiService.ajaxAPIPost<Leggi_Padri, ImpresaPadre>('AnagraficaNG/LeggiImpresaPadreSementieri', {}, false)
        .pipe(map(data => {
          resolve(data.RispostaStringa);
        })).subscribe();
    });
  }

  leggi_Certificazioni(): Observable<BaseCodeDescr[]> {

    return (this.ajaxApiService.ajaxAPIPost<object, BaseCodeDescr[]>(
      'AnagraficaNG/Leggi_Certificazioni',
      {},
      false).pipe(map((risp) => {
      return (risp.RispostaStringa);
    })));
  }

  async leggiImpresa(objParametri: ObjParametriAgenda): Promise<rispostaStandard<Impresa>> {
    return lastValueFrom(this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, Impresa>('AnagraficaNG/LeggiImpresa', objParametri));
  }

  leggiImpresaObservable(objParametri: ObjParametriAgenda): Observable<rispostaStandard<Impresa>> {
    return this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, Impresa>('AnagraficaNG/LeggiImpresa', objParametri);
  }

  setupLeggiImprese(agenda: ObjParametriAgenda) {
    return agenda;
  }

  leggi_Imprese_Codici() {
    return new Promise<Array<any>>(async (resolve, reject) => {
      if (this.imprese_codici == null || this.imprese_codici == undefined || this.imprese_codici.length == 0){

        this.ajaxApiService.ajaxAPIGet<any, any>('AnagraficaNG/LeggiImpreseCodici', "").pipe(map(r => {
          this.imprese_codici = JSON.parse(r.RispostaStringa);
          resolve(this.imprese_codici);
        })).subscribe();

      } else {
        resolve(this.imprese_codici);
      }
    });
  }

  leggiImpreseCodici(agenda: ObjParametriAgenda): Observable<CodiceAnagrafe[]> {
    const obs: Observable<CodiceAnagrafe[]> = this.ajaxApiService.ajaxAPIGet<any, CodiceAnagrafe[]>('AnagraficaNG/LeggiImpreseCodici', this.setupLeggiImprese(agenda)).pipe(
      map(
        (data: rispostaStandard<CodiceAnagrafe[]>) => {
          this.imprese_codici = data.RispostaStringa;
          return this.imprese_codici.slice();
        }));
    return obs;
  }

  controlloPresenzaPiva(piva: string): Observable<string> {
    return this.ajaxApiService.ajaxAPIPost<string, string>(
      'AnagraficaNG/ControlloPresenzaPiva',
      piva).pipe(map((data) => data.RispostaStringa)
    );
  }

  impresaBiologica(piva: string): Observable<boolean> {
    let impresa = new Impresa;
    impresa.partitaIva = piva;

    return this.ajaxApiService.ajaxAPIPost<Impresa, boolean>('AnagraficaNG/ImpresaBiologica',
      impresa).pipe(map((data) => data.RispostaStringa)
    );

  }

  public ScriviImpresa(impresa: Impresa, setLoading: boolean): Observable<rispostaStandard<Impresa>> {
    return this.ajaxApiService.ajaxAPIPost<Impresa, Impresa>('AnagraficaNG/ScriviImpresa', impresa, setLoading);
  }

  public LeggiImpreseConFiltroUtente(): Observable<Impresa[]> {
    return this.ajaxApiService.ajaxAPIGet("AnagraficaNG/LeggiImpreseConFiltroUtente_Modello",'').pipe(
      map((
      data: rispostaStandard<Impresa[]>) => data.RispostaStringa
      ));
  }

  public LeggiImpreseConFiltroUtenteHubAgea(filters: FarmFilters): Observable<Impresa[]> {
    return this.ajaxApiService.ajaxAPIPost<FarmFilters, Impresa[]>("AnagraficaNG/LeggiImpreseConFiltroUtenteHubAgea_Modello", filters)
      .pipe(map(data => data.RispostaStringa));
  }

  public exportImpreseExcel(filters: FarmFilters) {
    return this.ajaxApiService.ajaxAPIPost<FarmFilters, any>("AnagraficaNG/ReportImpreseExcelFiltroAgea", filters)
      .pipe(map(data => data.RispostaStringa));
  }

  // testLettura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {

  //     const parametri: CoreWS_Generic<ObjParametriAgenda> = {
  //         objP: this.masterService.getCoreWSGenericObjP(),
  //         InData: objParametriAgenda
  //     };

  //     return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
  //         this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Test_Imprese_Archivio_Lettura',
  //         parametri);
  // }

  // testScrittura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {

  //     const parametri: CoreWS_Generic<ObjParametriAgenda> = {
  //         objP: this.masterService.getCoreWSGenericObjP(),
  //         InData: objParametriAgenda
  //     };

  //     return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
  //         this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Test_Imprese_Archivio_Scrittura',
  //         parametri)
  // }
}

export interface FarmFilters {
  withBundles: boolean;
  afterDate: Date | null;
  beforeDate: Date | null;
  withoutSubmissions: boolean;
  submissionError: boolean;
  submissionCompleted: boolean;
}

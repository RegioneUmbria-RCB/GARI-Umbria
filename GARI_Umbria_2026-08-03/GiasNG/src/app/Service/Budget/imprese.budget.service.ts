import {Injectable} from '@angular/core';
import {firstValueFrom, lastValueFrom, Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {AjaxAgronicaService} from '../ajax-agronica.service';
import {MasterService, RispostaStandard, rispostaStandard} from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {JsonKendoResult} from 'gias-kendo-grid';
import {Impresa} from 'app/Model/anagrafiche/Impresa';
import {RisorseUmane} from '../../Model/anagrafiche/RisorseUmane';
import {Contatto} from 'app/Model/anagrafiche/Contatto';
import {CodiceAnagrafe} from 'app/Model/anagrafiche/CodiceAnagrafe';
import {NgxIndexedDBService} from 'ngx-indexed-db';
import {ImpreseFactoryService} from '../ServiceFactory/imprese.factory.service';
import {AjaxAgronicaAPIService} from '../ajax-agronica.api.service';
import {BaseCodeDescr} from 'app/Model/baseClass/baseCodeDescr';
import {AnagraficaNGClient} from '../api.service';

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

export class LeggiFiltro
{
  Ricerca:string;
}

@Injectable()
export class ImpreseBudgetService extends ImpreseFactoryService {


  constructor(protected masterService: MasterService,
              protected ajaxAgronicaService: AjaxAgronicaService,
              protected ajaxApiService: AjaxAgronicaAPIService,
              protected dbService: NgxIndexedDBService,
              protected anagraficaNG: AnagraficaNGClient
  ) {
    super(masterService, ajaxAgronicaService, ajaxApiService, dbService, anagraficaNG)
    //console.log('Imprese Budget Service');
  }

  // private unmodifiedData: any[];
  // private data: any[];
  // private id: string;
  // private saveOnRequest = true;
  // private imprese_codici: Array<any>;
  // private impresa: Impresa;

  // private padri: Impresa[] = [];

  // private imprese: JsonKendoResult;
  // private maxData: Date = AGRODATAINIZIO;

  // leggiImprese_Old(): Observable<JsonKendoResult> {
  //     if (this.maxData == AGRODATAINIZIO) {
  //         return this.fetchImprese();
  //     } else {
  //         const parametri: CoreWS_Generic<Object> = {
  //             objP: this.masterService.getCoreWSGenericObjP(),
  //             InData: ''
  //         };
  //         return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<Data, Object>(
  //             this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Leggi_Max_DataModifica',
  //             parametri).pipe(
  //                 map(r => { return r.RispostaStringa.data }),
  //                 switchMap((maxDataModifica) => {
  //                     if (this.maxData > maxDataModifica) {
  //                         // this.dbService.getByID('imprese', 1).subscribe((val) => {
  //                         //     console.log(val);
  //                         // });
  //                         // return this.dbService.getByID('imprese', 1).pipe(
  //                         //     map((val) => {
  //                         //         return <JsonKendoResult>val;
  //                         //     })
  //                         // )
  //                         return of(this.imprese);
  //                     } else {
  //                         return this.fetchImprese();
  //                     }
  //                 })
  //         )
  //
  //     }
  // }

  leggiImprese(piva?: string, data?: Date): Observable<JsonKendoResult> {
    // if (this.maxData == AGRODATAINIZIO) {
    //     return this.fetchImprese(piva);
    // } else {
    //     const parametri: CoreWS_Generic<Object> = {
    //         objP: this.masterService.getCoreWSGenericObjP(),
    //         InData: ''
    //     };
    //     return this.Leggi_Max_DataModificaAPI().pipe(
    //         switchMap((maxDataModifica) => {
    //             if (this.maxData > maxDataModifica && this.imprese != null) {
    //                 return of(this.imprese);
    //             } else {
    //                 return this.fetchImprese(piva);
    //             }
    //         })
    //     )
    // }
    return this.fetchImprese(piva, data);
  }



  private fetchImpreseAPI(): Observable<JsonKendoResult> {
    this.maxData = new Date();
    return this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, any>(
      'AnagraficaNG/Imprese',
      new ObjParametriAgenda()).pipe(map(
      (data) => {
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

  private Leggi_Max_DataModificaAPI(): Observable<Date> {
    return this.ajaxApiService.ajaxAPIPost<string, { data: Date }>(
      'AnagraficaNG/GetImpreseMaxDataModifica',
      '').pipe(map(
      (data) => {
        return data.RispostaStringa.data;
      })
    );
  }

  // private fetchImprese_OLD(): Observable<JsonKendoResult> {
  //     const parametri: CoreWS_Generic<ObjParametriAgenda> = {
  //         objP: this.masterService.getCoreWSGenericObjP(),
  //         InData: new ObjParametriAgenda()
  //     };
  //     this.maxData = new Date();
  //     return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<any, ObjParametriAgenda>(
  //         this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Leggi_Imprese_Anagrafica',
  //         parametri).pipe(map(
  //             (data) => {
  //                 new JsonKendoResult()
  //                 let imprese = new JsonKendoResult();
  //                 const datiParse = (<any>data.RispostaStringa);
  //                 imprese.kendo_columns = datiParse.kendo_columns;
  //                 imprese.kendo_model = datiParse.kendo_model;
  //                 imprese.kendo_rows = datiParse.kendo_rows;
  //                 // this.dbService.clear('imprese');
  //                 // this.dbService.add('imprese', imprese, 1).subscribe();
  //                 this.imprese = imprese;
  //                 return <JsonKendoResult>imprese;
  //             })
  //     );
  //
  // }

  private fetchImprese(piva: string, data: Date): Observable<JsonKendoResult> {
    this.maxData = new Date();
    let objParams = new ObjParametriAgenda();
    objParams.Piva = piva;
    if (data){
      objParams.Data = data;
    }
    return this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, any>(
      'AnagraficaNG/Imprese',
      objParams).pipe(map(
      (data) => {
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

  private customizeParametri(objParametri: ObjParametriAgenda) {
    objParametri.Piva = '';
    return objParametri;
  }

  /*leggiCombo_Tecnici_Old(piva: string): Promise<rispostaStandard<Contatto[]>> {
      const parametri: Leggi_Tecnici_Request = {
          piva: piva,
          objP_server: this.masterService.ObjParametri_Server
      };
      // return this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + "/Anagrafica/Contatti.asmx/CaricaComboCmb_Tecnici", parametri);
      return this.ajaxAgronicaService.ajaxAgronicaG<Contatto[]>(this.masterService.link_CoreWS + '/Anagrafica/Contatti.asmx/CaricaComboCmb_Tecnici_Modello', parametri);
  }*/

  leggiCombo_Tecnici(piva: string): Promise<rispostaStandard<Contatto[]>> {
    return new Promise<rispostaStandard<Contatto[]>>(async (resolve, reject) => {
      this.ajaxApiService.ajaxAPIPost<LeggiFiltro, Contatto[]>(
        'AnagraficaNG/CaricaComboCmbTecniciModello',
        {Ricerca: piva}
      ).pipe(map(
        (data) => {
          resolve(data);
        })).subscribe();
    });
  }

  /*leggiCombo_OrganismiDiControllo_Old(piva: string): Promise<rispostaStandard<RisorseUmane[]>> {
      const parametri: Leggi_Odc_Request = {
          piva: piva,
          objP_server: this.masterService.ObjParametri_Server
      };
      // return this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + "/Anagrafica/Contatti.asmx/CaricaComboCmb_OrganismiODC", parametri);
      return this.ajaxAgronicaService.ajaxAgronicaG<RisorseUmane[]>(this.masterService.link_CoreWS + '/Anagrafica/Contatti.asmx/CaricaComboCmb_OrganismiODC_Modello', parametri);
  }*/

  leggiCombo_OrganismiDiControllo(piva: string): Promise<rispostaStandard<RisorseUmane[]>> {
    return firstValueFrom(this.ajaxApiService.ajaxAPIPost<any, RisorseUmane[]>('MetaschemaNG/CaricaComboCmbOrganismiODCModello', {Ricerca: piva}).pipe(map(data => {
      return data
    })));
  }


  /*leggiPadri_Old(): Promise<Impresa[]> {
      return new Promise<Impresa[]>(async (resolve, reject) => {
          if (this.padri.length > 0) {
              resolve(this.padri);
              return;
          }
          const parametri: CoreWS_Generic<Leggi_Padri> = new CoreWS_Generic(
              this.masterService.getCoreWSGenericObjP(),
              {}
          );

          this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Impresa[], Leggi_Padri>(this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Leggi_Imprese_Padri',
              parametri, false).then((val) => {
                  this.padri = val.RispostaStringa;
                  resolve(val.RispostaStringa);
          });

      });
  }*/

  leggiPadri(): Promise<Impresa[]> {
    return new Promise<Impresa[]>(async (resolve, reject) => {
      this.ajaxApiService.ajaxAPIPost<Leggi_Padri, Impresa[]>('AnagraficaNG/Leggi_Imprese_Padri',
        {}, false).pipe(map(data => {
        resolve(data.RispostaStringa)
      })).subscribe();
    });
  }

  /*leggi_Certificazioni_Old(): Observable<BaseCodeDescr[]> {

          const parametri: CoreWS_Generic<object> = new CoreWS_Generic(
              this.masterService.getCoreWSGenericObjP(),
              {}
          );

          return (this.ajaxAgronicaService.ajaxCoreWSPost<object, BaseCodeDescr[]>(
              this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Leggi_Certificazioni',
              parametri,
              false).pipe(map((risp) => {
              return (risp.RispostaStringa);
              })));
  }*/

  leggi_Certificazioni(): Observable<BaseCodeDescr[]> {

    return (this.ajaxApiService.ajaxAPIPost<object, BaseCodeDescr[]>(
      'AnagraficaNG/Leggi_Certificazioni',
      {},
      false).pipe(map((risp) => {
      return (risp.RispostaStringa);
    })));
  }

  /*async leggiImpresa_Old(objParametri: ObjParametriAgenda): Promise<rispostaStandard<Impresa>> {
     const parametri: CoreWS_Generic<ObjParametriAgenda> = {
         objP: this.masterService.getCoreWSGenericObjP(),
         InData: objParametri
     };
     // this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, Impresa>('AnagraficaNG/LeggiImpresa', objParametri, true).subscribe((val) => {
     //     console.log(val);
     // })
     // this.anagraficaNG.anagraficaNGGetImpresa(objParametri as any).subscribe((val) => {
     //     console.log(val)
     // });
     //return lastValueFrom(this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, Impresa>('AnagraficaNG/LeggiImpresa', objParametri, true))
     return lastValueFrom(this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<Impresa, ObjParametriAgenda>(this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Leggi_Impresa_Anagrafica', parametri));
 }*/

  async leggiImpresa(objParametri: ObjParametriAgenda): Promise<rispostaStandard<Impresa>> {
    return lastValueFrom(this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, Impresa>('AnagraficaNG/LeggiImpresa', objParametri));
  }

  /*leggiImpresaObservable_Old(objParametri: ObjParametriAgenda): Observable<rispostaStandard<Impresa>> {
      const parametri: CoreWS_Generic<ObjParametriAgenda> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: objParametri
      };
      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<Impresa, ObjParametriAgenda>(this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Leggi_Impresa_Anagrafica', parametri);
      //return this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, Impresa>('AnagraficaNG/LeggiImpresa', objParametri, true);
  }*/

  leggiImpresaObservable(objParametri: ObjParametriAgenda): Observable<rispostaStandard<Impresa>> {
    return this.ajaxApiService.ajaxAPIPost<ObjParametriAgenda, Impresa>('AnagraficaNG/LeggiImpresa', objParametri);
  }
  setupLeggiImprese(agenda: ObjParametriAgenda) {
    return agenda;
  }

  /*leggi_Imprese_Codici_Old() {
      return new Promise<Array<any>>(async (resolve, reject) => {
          if (this.imprese_codici == null || this.imprese_codici == undefined || this.imprese_codici.length == 0){
              const parametri: Leggi_Imprese_Codici_Request = {
                  objP_super_server: this.masterService.ObjParametri_Super_Server,
                  objP_server: this.masterService.ObjParametri_Server,
                  objP_utenti: this.masterService.ObjParametri_Utenti
              };
              const r = await this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Leggi_Imprese_Codici', parametri);
              this.imprese_codici = JSON.parse(r.RispostaStringa);
              resolve(this.imprese_codici);
          } else {
              resolve(this.imprese_codici);
          }
      });
  }*/

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

  /*leggiImpreseCodici_Old(agenda: ObjParametriAgenda): Observable<CodiceAnagrafe[]> {
      const obs: Observable<CodiceAnagrafe[]> = this.httpService.getUsingAgenda(
          '/Anagrafica/Imprese.asmx/Leggi_Imprese_Codici',
          this.setupLeggiImprese).pipe(
          map(
              (data: RispostaStandard) => {
                  this.imprese_codici = JSON.parse(data.RispostaStringa);
                  return this.imprese_codici.slice();
              }));

      return obs;
  }*/

  leggiImpreseCodici(agenda: ObjParametriAgenda): Observable<CodiceAnagrafe[]> {
    const obs: Observable<CodiceAnagrafe[]> = this.ajaxApiService.ajaxAPIGet<any, CodiceAnagrafe[]>('AnagraficaNG/LeggiImpreseCodici', this.setupLeggiImprese(agenda)).pipe(
      map(
        (data: rispostaStandard<CodiceAnagrafe[]>) => {
          this.imprese_codici = data.RispostaStringa;
          return this.imprese_codici.slice();
        }));
    return obs;
  }


  /*controlloPresenzaPiva_Old(piva: string): Observable<RispostaStandard> {
      const parametri: CoreWS_Generic<string> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: piva
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<RispostaStandard, string>(
          this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/Controllo_Presenza_Piva',
          parametri).pipe(map((data) => data.RispostaStringa)
      );

  }*/

  controlloPresenzaPiva(piva: string): Observable<string> {

    return this.ajaxApiService.ajaxAPIGet<string, string>(
      'AnagraficaNG/ControlloPresenzaPiva',
      piva).pipe(map((data) => data.RispostaStringa)
    );

  }


  /*impresaBiologica_Old(piva: string): Observable<boolean> {
      let impresa = new Impresa;
      impresa.partitaIva = piva;

      const parametri: CoreWS_Generic<Impresa> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: impresa
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, Impresa>(
          this.masterService.link_CoreWS + '/Anagrafica/Imprese.asmx/ImpresaBiologica',
          parametri).pipe(map((data) => data.RispostaStringa)
      );

  }*/

  impresaBiologica(piva: string): Observable<boolean> {
    let impresa = new Impresa;
    impresa.partitaIva = piva;

    return this.ajaxApiService.ajaxAPIPost<Impresa, boolean>('AnagraficaNG/ImpresaBiologica',
      impresa).pipe(map((data) => data.RispostaStringa)
    );

  }


  // public ScriviImpresa_Old(impresa: Impresa): Observable<rispostaStandard<Impresa>> {
  //     const parametri: CoreWS_Generic<Impresa> = {
  //         objP: this.masterService.getCoreWSGenericObjP(),
  //         InData: impresa
  //     };
  //
  //     return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<Impresa, Impresa>(
  //         this.masterService.link_CoreWS + "/Anagrafica/Imprese.asmx/Scrivi_Impresa_Anagrafica",
  //         parametri).pipe(map((data) => {
  //             return data
  //         })
  //     )
  //
  // }

  public ScriviImpresa(impresa: Impresa): Observable<rispostaStandard<Impresa>> {
    return this.ajaxApiService.ajaxAPIPost<Impresa, Impresa>(
      "AnagraficaNG/ScriviImpresa", impresa)
      .pipe(map((data) => {
          return data
        })
      )
  }

}

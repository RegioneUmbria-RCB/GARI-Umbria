import { Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { AppezzamentiRequest, CampiKendoServerResult } from 'app/anagrafica/campi/campi.model';
import { Campo, PKCampo } from 'app/Model/anagrafiche/Campo';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { CoreWSRequest } from 'app/Model/AppConfig';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { JsonKendoResult, KendoGridColumn } from 'gias-kendo-grid';
import {firstValueFrom, from, lastValueFrom, map, of} from 'rxjs';
import { Observable } from 'rxjs/internal/Observable';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, RispostaStandard, rispostaStandard } from '../master.service';
import { SpecieVegetaliService } from '../Metaschema/specie-vegetali.service';
import { ObjParametriAgendaService } from '../obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { CampiFactoryService } from '../ServiceFactory/campi.factory.service';
import {UtilizzoTerreno} from "../../Model/metaschema/utilizzi/UtilizzoTerreno";
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import {LeggiInvestimentoCatastaleCampo} from '../../anagrafica/campi/investimento-catastale-campo/investimento-catastale-campo.service';
import {IntlService} from '@progress/kendo-angular-intl';
import {BudgetAnagrafica} from '../Budget/budget.service';
import {tap} from 'rxjs/operators';
import {Specie} from '../../Model/metaschema/utilizzi/Specie';

export class  LeggiCampi {
  centro: CentroAziendale
  utilizzoTerreno: UtilizzoTerreno;
  data: Date;
}

// export class Aggiorna_Campo_Edit_Request {
//     campo: Campo;
//     tipoOperazione: enum_TipoOperazioneDB;
//     objP_super_server: string;
//     objP_server: string;
//     objP_utenti: string;
// }

export class Aggiorna_Campo_Edit_Request {
  campo: Campo;
  tipoOperazione: enum_TipoOperazioneDB;
}

// export class Aggiorna_Campo_inLine_Edit_Request {
//     InData: CoreWS_Generic<ObjParametriAgenda>;
//     campo: Campo;
//     tipoOperazione: enum_TipoOperazioneDB;
//     objP_super_server: string;
//     objP_server: string;
//     objP_utenti: string;
// }

export class Aggiorna_Campo_inLine_Edit_Request {
  InData: CoreWS_Generic<ObjParametriAgenda>;
  campo: Campo;
  tipoOperazione: enum_TipoOperazioneDB;
}


export class Leggi_Campi_SpecieVegetali_Request {
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
}

export class Leggi_AppezzamentiCampo_Request {
  Piva: string;
  Sa_Cod: number;
  Campo_Cod: number;
  Validita_Inizio: Date;
  Validita_Fine: Date;
  flag_ConCatasto: boolean;
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
}

export class Leggi_ParticelleCampo_Request {
  Piva: string;
  Sa_Cod: number;
  Campo_Cod: number;
  Validita_Inizio: Date;
  Validita_Fine: Date;
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
}

@Injectable({
  providedIn: 'root'
})
export class CampiService extends CampiFactoryService {

  private objParametriAgenda: ObjParametriAgenda;
  private campi: CampiKendoServerResult;

  private specieVegetali: Array<any>;
  private campi_codici: Array<any>;
  private appezzamenti_campo: Array<any>;

  constructor(
    protected masterService: MasterService,
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    protected translocoService: TranslocoService,
    protected specievegetaliservice: SpecieVegetaliService,
    protected intlService: IntlService
  ) {
    super(masterService, ajaxAgronicaService, ajaxAgronicaAPIService, translocoService, specievegetaliservice)
  }

  private AggiungiRigaVuotaCampi(flagPrimaRiga: boolean,descrizioneRigaVuota: string, campi: Campo[],centro: CentroAziendale){
    if(flagPrimaRiga && campi){

      const CampoPrimaRiga=new Campo({ centroAziendalePK:  centro.primaryKey, codice: 0 } as PKCampo);

      if(descrizioneRigaVuota !== ""){
        CampoPrimaRiga.descrizione=this.translocoService.translate(descrizioneRigaVuota);
      }else{
        CampoPrimaRiga.descrizione="";
      }


      campi.unshift(CampoPrimaRiga);
    }
  }

  /*LeggiCampi_Old(p: LeggiCampi, flagPrimaRiga: boolean, descrizioneRigaVuota: string){
      return new Promise<Campo[]>(async (resolve, reject) => {
          const parametri: CoreWS_Generic<LeggiCampi> = new CoreWS_Generic(this.masterService.getCoreWSGenericObjP(), p);

          const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Campo[], LeggiCampi>(
              this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/LeggiCampi',
              parametri, false
          );

          this.AggiungiRigaVuotaCampi(flagPrimaRiga,descrizioneRigaVuota,R.RispostaStringa,p.centro);

          resolve(R.RispostaStringa);
      });
  }*/

  LeggiCampi(p: LeggiCampi, flagPrimaRiga: boolean, descrizioneRigaVuota: string){
    return new Promise<Campo[]>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiCampi, Campo[]>(
        'AnagraficaNG/LeggiCampi',
        p, false
      ).pipe(map(R => {
        this.AggiungiRigaVuotaCampi(flagPrimaRiga,descrizioneRigaVuota,R.RispostaStringa,p.centro);
        resolve(R.RispostaStringa);
      })).subscribe();
    });
  }

  /*LeggiCampi_perSpecieImpianti_Old(p: LeggiCampi, flagPrimaRiga: boolean, descrizioneRigaVuota: string){
      return new Promise<Campo[]>(async (resolve, reject) => {
          const parametri: CoreWS_Generic<LeggiCampi> = new CoreWS_Generic(this.masterService.getCoreWSGenericObjP(), p);

          const R = await this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Campo[], LeggiCampi>(
              this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/LeggiCampi_perSpecieImpianti',
              parametri, false
          );

          this.AggiungiRigaVuotaCampi(flagPrimaRiga,descrizioneRigaVuota,R.RispostaStringa,p.centro);

          resolve(R.RispostaStringa);
      });
  }*/

  LeggiCampi_perSpecieImpianti(p: LeggiCampi, flagPrimaRiga: boolean, descrizioneRigaVuota: string){
    return new Promise<Campo[]>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiCampi, Campo[]>(
        'AnagraficaNG/LeggiCampi_perSpecieImpianti',
        p, false
      ).pipe(map(data => {
        this.AggiungiRigaVuotaCampi(flagPrimaRiga,descrizioneRigaVuota,data.RispostaStringa,p.centro);
        resolve(data.RispostaStringa);
      })).subscribe();
    });
  }


  public getCampi(): CampiKendoServerResult {
    return this.campi;
  }

  /*public getCampi1_Old(objParametri: ObjParametriAgenda) {
      const parametri: CoreWSRequest<ObjParametriAgenda> = {
          objP_super_server: this.masterService.ObjParametri_Super_Server,
          objP_server: this.masterService.ObjParametri_Server,
          objP_utenti: this.masterService.ObjParametri_Utenti,
          InData: objParametri
      };

      return this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/Leggi_Campi_Anagrafica', parametri);
  }*/
  //
  // public getCampi1(objParametri: ObjParametriAgenda) {
  //     return this.ajaxAgronicaAPIService.ajaxAPIPost('AnagraficaNG/LeggiCampiAnagrafica', objParametri);
  // }

  /*LeggiCampiAnagrafica_Old(objParametriAgenda: ObjParametriAgenda): Observable<JsonKendoResult> {

      const objPAgenda = objParametriAgenda;

      const parametri: CoreWS_Generic<ObjParametriAgenda> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: objPAgenda
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<any, ObjParametriAgenda>(
          this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/Leggi_Campi_Anagrafica',
          parametri).pipe(map((data) => <JsonKendoResult>data.RispostaStringa));
  }*/

  LeggiCampiAnagrafica(objParametriAgenda: ObjParametriAgenda): Observable<any> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any>('AnagraficaNG/LeggiCampiAnagrafica',
      objParametriAgenda).pipe(map((data) => data.RispostaStringa));
  }


  // private async doAjaxCall(): Promise<RispostaStandard> {
  //     const parametri = this.masterService.getCoreWSRequest(this.objParametriAgenda);
  //     return this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS +
  //     '/Anagrafica/Campi.asmx/Leggi_Campi_Anagrafica', parametri);
  // }

  public setDates(column: KendoGridColumn): void {
    if(column.field === 'Validita_Inizio') {
      column.date = { defaultValue: new Date(), min: new Date(1900,1,1), max: new Date(2100,12,31)};
    }
    if(column.field === 'Validita_Fine') {
      column.date = { defaultValue: new Date(), min: new Date(1900,1,1), max: new Date(2100,12,31)};
    }
    if(column.field === 'Data_Creazione') {
      column.date = { defaultValue: new Date(), min: new Date(1900,1,1), max: new Date(2100,12,31)};
    }
    if(column.field === 'Data_Modifica') {
      column.date = { defaultValue: new Date(), min: new Date(1900,1,1), max: new Date(2100,12,31)};
    }
  }
  public setNumericFields(column: KendoGridColumn): void {
    if(column.field === 'Superficie_Totale') {
      column.numeric = { defaultValue: 1, min: 0, max: 10 };
    }
    if(column.field === 'Superficie_Biologico') {
      column.numeric = { defaultValue: 1, min: 0, max: 10 };
    }
    if(column.field === 'Superficie_Convenzionale') {
      column.numeric = { defaultValue: 1, min: 0, max: 10 };
    }
    if(column.field === 'Superficie_Conversione') {
      column.numeric = { defaultValue: 1, min: 0, max: 10 };
    }
    if(column.field === 'Superficie_Catastale') {
      column.numeric = { defaultValue: 1, min: 0, max: 10 };
    }

  }

  /*async leggiCampo_Old(objParametri: ObjParametriAgenda): Promise<rispostaStandard<Campo>> {
      const parametri: CoreWS_Generic<ObjParametriAgenda> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: objParametri
      };
      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_Promise<Campo, ObjParametriAgenda>(
              this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/Leggi_Campo_Anagrafica',
              parametri, false
      );
  }*/

  async leggiCampo(objParametri: ObjParametriAgenda): Promise<rispostaStandard<Campo>> {
    return firstValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, Campo>('AnagraficaNG/LeggiCampoAnagrafica',
      objParametri, false
    ).pipe(map(data => {
      return data;
    })));
  }

  /*async leggiAppezzamentiCampo_Old(objParametri: ObjParametriAgenda){

      const parametri: AppezzamentiRequest = {
          objP_super_server: this.masterService.ObjParametri_Super_Server,
          objP_server: this.masterService.ObjParametri_Server,
          objP_utenti: this.masterService.ObjParametri_Utenti,
          InData: objParametri
      };

      let app = this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/Leggi_AppezzamentiCampo', parametri);
      return app;
  }*/

  async leggiAppezzamentiCampo(objParametri: ObjParametriAgenda){
    return await lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost('AnagraficaNG/LeggiAppezzamentiCampo', objParametri));
  }

  /*async leggiParticelleCampo_Old(objParametri: ObjParametriAgenda) {
      const parametri: CoreWSRequest<ObjParametriAgenda> = {
          objP_super_server: this.masterService.ObjParametri_Super_Server,
          objP_server: this.masterService.ObjParametri_Server,
          objP_utenti: this.masterService.ObjParametri_Utenti,
          InData: objParametri
      };

      return this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/Leggi_ParticelleCampo', parametri);
  }*/

  leggiParticellePerCentro(objParametri: ObjParametriAgenda) {
    return (this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any>('AnagraficaNG/LeggiParticellePerCentro', objParametri));
  }

  leggi_SpecieVegetali(): Promise<Specie[]> {
    return this.specievegetaliservice.leggi_FiltroUtente();
  }

  /*leggiCampiCodici_Old(agenda: ObjParametriAgenda): Observable<CodiceAnagrafe[]> {
      const obs: Observable<CodiceAnagrafe[]> = this.httpService.getUsingAgenda(
          '/Anagrafica/Campi.asmx/Leggi_Campi_Codici',
          this.setupLeggiCampi).pipe(
          map(
              (data: RispostaStandard) => {
                  this.campi_codici = JSON.parse(data.RispostaStringa);
                  return this.campi_codici.slice();
              }));

      return obs;
  }*/

  leggiCampiCodici(agenda: ObjParametriAgenda): Observable<CodiceAnagrafe[]> {
    const obs: Observable<CodiceAnagrafe[]> = this.ajaxAgronicaAPIService.ajaxAPIGet<any, any[]>(
      'AnagraficaNG/Leggi_Campi_Codici',
      this.setupLeggiCampi).pipe(
      map(
        (data) => {
          this.campi_codici = data.RispostaStringa;
          return this.campi_codici.slice();
        }));

    return obs;
  }


  setupLeggiCampi(agenda: ObjParametriAgenda) {
    return agenda;
  }

  // aggiornaCampo(campo: Campo, tipoOperazione: enum_TipoOperazioneDB) {

  //     return new Promise<RispostaStandard>(async (resolve, reject) => {
  //         const parametri: Aggiorna_Campo_Edit_Request = {
  //             campo: campo,
  //             tipoOperazione: tipoOperazione,
  //             objP_super_server: this.masterService.ObjParametri_Super_Server,
  //             objP_server: this.masterService.ObjParametri_Server,
  //             objP_utenti: this.masterService.ObjParametri_Utenti
  //         };


  //         const r = await this.ajaxAgronicaService.ajaxAgronica(this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/ScriviCampiAnagrafica', parametri);
  //         resolve(r);

  //     });
  // }

  aggiornaCampo(campo: Campo, tipoOperazione: enum_TipoOperazioneDB) {

    return new Promise<RispostaStandard>(async (resolve, reject) => {
      const parametri: Aggiorna_Campo_Edit_Request = {
        campo: campo,
        tipoOperazione: tipoOperazione
      };

      resolve(await lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost('AnagraficaNG/ScriviCampiAnagrafica', parametri)));
    });
  }

  // aggiornaCampoObs(campo: Campo, tipoOperazione: enum_TipoOperazioneDB) {

  //     const parametri: Aggiorna_Campo_Edit_Request = {
  //         campo: campo,
  //         tipoOperazione: tipoOperazione,
  //         objP_super_server: this.masterService.ObjParametri_Super_Server,
  //         objP_server: this.masterService.ObjParametri_Server,
  //         objP_utenti: this.masterService.ObjParametri_Utenti
  //     };



  //     return this.ajaxAgronicaService.ajaxAgronicaObs(
  //         this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/ScriviCampiAnagrafica',
  //         parametri);
  // }

  aggiornaCampoObs(campo: Campo, tipoOperazione: enum_TipoOperazioneDB) {

    const parametri: Aggiorna_Campo_Edit_Request = {
      campo: campo,
      tipoOperazione: tipoOperazione
    };

    return this.ajaxAgronicaAPIService.ajaxAPIPost('AnagraficaNG/ScriviCampiAnagrafica', parametri);
  }

  /*aggiornaCampo_inLine_Old(campo: Campo, tipoOperazione: enum_TipoOperazioneDB, agenda: ObjParametriAgenda): Observable<RispostaStandard> {
          const parametri: Aggiorna_Campo_inLine_Edit_Request = {
              InData: {
                  objP: this.masterService.getCoreWSGenericObjP(),
                  InData: agenda
              },
              campo: campo,
              tipoOperazione: tipoOperazione,
              objP_super_server: this.masterService.ObjParametri_Super_Server,
              objP_server: this.masterService.ObjParametri_Server,
              objP_utenti: this.masterService.ObjParametri_Utenti
          };

          return this.ajaxAgronicaService.ajaxAgronicaObs(this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/ScriviCampiAnagrafica_inLine', parametri);
  }*/

  aggiornaCampo_inLine(campo: Campo, tipoOperazione: enum_TipoOperazioneDB, agenda: ObjParametriAgenda): Observable<RispostaStandard> {
    const parametri: Aggiorna_Campo_inLine_Edit_Request = {
      InData: {
        objP: this.masterService.getCoreWSGenericObjP(),
        InData: agenda
      },
      campo: campo,
      tipoOperazione: tipoOperazione,
    };

    return this.ajaxAgronicaAPIService.ajaxAPIPost('AnagraficaNG/ScriviCampiAnagraficaInLine', parametri);
  }


  /*testLettura_Old(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {

      const parametri: CoreWS_Generic<ObjParametriAgenda> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: objParametriAgenda
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
          this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/Test_Campi_Archivio_Lettura',
          parametri);
  }*/

  testLettura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, boolean>(
      'AnagraficaNG/Test_Campi_Archivio_Lettura',
      objParametriAgenda);
  }

  /*testScrittura_Old(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {

      const parametri: CoreWS_Generic<ObjParametriAgenda> = {
          objP: this.masterService.getCoreWSGenericObjP(),
          InData: objParametriAgenda
      };

      return this.ajaxAgronicaService.ajaxAgronicaCoreWS_GenericsObs<boolean, ObjParametriAgenda>(
          this.masterService.link_CoreWS + '/Anagrafica/Campi.asmx/Test_Campi_Archivio_Scrittura',
          parametri)
  }*/

  testScrittura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, boolean>(
      'AnagraficaNG/Test_Campi_Archivio_Scrittura',
      objParametriAgenda)
  }

  leggiInvestimentoCatastale(filtro: LeggiInvestimentoCatastaleCampo): Observable<rispostaStandard<any[]>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiInvestimentoCatastaleCampo, any[]>(
      '/AnagraficaNG/LeggiInvestimentoCatastaleCampo',
      filtro
    );
  }

}

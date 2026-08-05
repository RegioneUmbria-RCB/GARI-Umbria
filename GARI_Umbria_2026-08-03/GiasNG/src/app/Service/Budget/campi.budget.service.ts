import { Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { CampiKendoServerResult } from 'app/anagrafica/campi/campi.model';
import { Campo, PKCampo } from 'app/Model/anagrafiche/Campo';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { KendoGridColumn } from 'gias-kendo-grid';
import {lastValueFrom, map, Observable, of} from 'rxjs';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { MasterService, RispostaStandard, rispostaStandard } from '../master.service';
import { SpecieVegetaliService } from '../Metaschema/specie-vegetali.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { CampiFactoryService } from '../ServiceFactory/campi.factory.service';
import { BudgetAnagrafica, BudgetService } from './budget.service';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import {CoreWS_Generic} from '../../Model/CoreWS/CoreWS_Generic';
import {LeggiInvestimentoCatastaleCampo} from '../../anagrafica/campi/investimento-catastale-campo/investimento-catastale-campo.service';
import {IntlService} from '@progress/kendo-angular-intl';
import {tap} from 'rxjs/operators';

export class  LeggiCampi {
  centro: CentroAziendale
  data: Date;
}

export class ScriviCampiAnagrafica {
  public InData: BudgetAnagrafica<Campo>;
  public tipoOperazione: enum_TipoOperazioneDB;
}

@Injectable({
  providedIn: 'root'
})
export class CampiBudgetService extends CampiFactoryService {

  private objParametriAgenda: ObjParametriAgenda;
  private campi: CampiKendoServerResult;

  private campi_codici: Array<any>;

  constructor(
    protected masterService: MasterService,
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    protected translocoService: TranslocoService,
    protected specievegetaliservice: SpecieVegetaliService,
    protected budgetService: BudgetService,
    protected intlService: IntlService
  ) {
    super(masterService, ajaxAgronicaService, ajaxAgronicaAPIService, translocoService, specievegetaliservice)
  }

  LeggiCampi(p: LeggiCampi, flagPrimaRiga: boolean, descrizioneRigaVuota: string){
    let bdgAnagrafica: BudgetAnagrafica<LeggiCampi> = new BudgetAnagrafica<LeggiCampi>(this.budgetService.getBudget().budgetId, p);
    return new Promise<Campo[]>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<LeggiCampi>, Campo[]>(
        'Budget/LeggiCampi',
        bdgAnagrafica,
        false
      ).pipe(map(R => {
        this.AggiungiRigaVuotaCampi(flagPrimaRiga,descrizioneRigaVuota,R.RispostaStringa,p.centro);
        resolve(R.RispostaStringa);
      })).subscribe();
    });
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

  public getCampi(): CampiKendoServerResult {
    return this.campi;
  }

  LeggiCampiAnagrafica(objParametriAgenda: ObjParametriAgenda): Observable<any> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<ObjParametriAgenda>, any>(
      'Budget/Leggi_Campi_Anagrafica',
      new BudgetAnagrafica(
        this.budgetService.getBudget().budgetId,
        objParametriAgenda
      )
    ).pipe(map((data) => {
      return data.RispostaStringa;
    }));
  }

  private async doAjaxCall(): Promise<RispostaStandard> {
    return lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      'Budget/Leggi_Campi_Anagrafica', this.masterService.getCoreWSRequest(this.objParametriAgenda)));
  }

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

  async leggiCampo(objParametri: ObjParametriAgenda): Promise<rispostaStandard<Campo>> {
    return lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<ObjParametriAgenda>, Campo>(
      'Budget/Leggi_Campo_Anagrafica',
      new BudgetAnagrafica(
        this.budgetService.getBudget().budgetId,
        objParametri
      ),
      false
    ));
  }

  async leggiAppezzamentiCampo(objParametri: ObjParametriAgenda){
    //console.log("QUI")
    let parametri: BudgetAnagrafica<ObjParametriAgenda> = new BudgetAnagrafica<ObjParametriAgenda>(
      this.budgetService.getBudget().budgetId,
      objParametri
    );
    let app = await lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost('Budget/Leggi_AppezzamentiCampo', parametri));
    return app;
  }

  leggiParticellePerCentro(objParametri: ObjParametriAgenda) {
    let parametri: BudgetAnagrafica<ObjParametriAgenda> = new BudgetAnagrafica(
      this.budgetService.getBudget().budgetId,
      objParametri
    );
    return this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<ObjParametriAgenda>, any>('Budget/Leggi_ParticelleCampo', parametri);
  }

  leggi_SpecieVegetali() {
    return this.specievegetaliservice.leggi_FiltroUtente();
  }

  leggiCampiCodici(agenda: ObjParametriAgenda): Observable<CodiceAnagrafe[]> {
    const obs: Observable<CodiceAnagrafe[]> = this.ajaxAgronicaAPIService.ajaxAPIGet<any, CodiceAnagrafe[]>(
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

  aggiornaCampo(campo: Campo, tipoOperazione: enum_TipoOperazioneDB) {

    return new Promise<RispostaStandard>(async (resolve, reject) => {
      const r = await lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('Budget/ScriviCampiAnagrafica', {InData: {
          Id_Budget: this.budgetService.getBudget().budgetId,
          ElementoAnagrafico: campo
        },
        tipoOperazione: tipoOperazione}));
      resolve(r);
    });
  }

  aggiornaCampoObs(campo: Campo, tipoOperazione: enum_TipoOperazioneDB, deleteRibaltamento: boolean = false) {
    let params: ScriviCampiAnagrafica = new ScriviCampiAnagrafica();
    params.InData = new BudgetAnagrafica<Campo>(this.budgetService.getBudget().budgetId, campo, deleteRibaltamento);
    params.tipoOperazione = tipoOperazione;
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      'Budget/ScriviCampiAnagrafica',
      params
    );
  }

  aggiornaCampo_inLine(campo: Campo, tipoOperazione: enum_TipoOperazioneDB, agenda: ObjParametriAgenda): Observable<RispostaStandard> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      'Budget/ScriviCampiAnagrafica',
      {InData: {
          Id_Budget: this.budgetService.getBudget().budgetId,
          ElementoAnagrafico: campo
        },
        tipoOperazione: tipoOperazione});
  }

  testLettura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, boolean>(
      'AnagraficaNG/Test_Campi_Archivio_Lettura',
      objParametriAgenda);
  }

  testScrittura(objParametriAgenda: ObjParametriAgenda): Observable<rispostaStandard<boolean>>  {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, boolean>(
      'AnagraficaNG/Test_Campi_Archivio_Scrittura',
      objParametriAgenda
    );
  }

  leggiInvestimentoCatastale(filtro: LeggiInvestimentoCatastaleCampo): Observable<rispostaStandard<any[]>> {
    let filtroBudget: BudgetAnagrafica<LeggiInvestimentoCatastaleCampo> = new BudgetAnagrafica<LeggiInvestimentoCatastaleCampo>(
      this.budgetService.getBudget().budgetId,
      filtro
    );

    return this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<LeggiInvestimentoCatastaleCampo>, any[]>(
      '/Budget/Leggi_Investimento_Catastale_Campo',
      filtroBudget
    );
  }

}

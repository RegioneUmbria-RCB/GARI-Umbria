import { Injectable } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import {Appezzamento, AppezzamentoJoinDescrizioni, PKAppezzamento} from 'app/Model/anagrafiche/Appezzamento';
import { PKCentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Impianto, PKImpianto } from 'app/Model/anagrafiche/Impianto';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { UtilizzoTerreno } from 'app/Model/metaschema/utilizzi/UtilizzoTerreno';
import { from, map, Observable, of, take} from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { Data } from '../Anagrafica/imprese.service';
import { ConversionService } from '../conversion.service';
import { MasterService, rispostaStandard, RispostaStandard} from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {
  ImpiantiFactoryService,
  CaricaCatastoSettings,
  CaricaDatiCatastali
} from '../ServiceFactory/impianti.factory.service';
import { BudgetAnagrafica, BudgetService } from './budget.service';
import {LeggiInvestimentoCatastale} from '../ServiceFactory/investimento-catastale.factory.service';
import {Esercizio} from '../../Model/anagrafiche/Esercizio';
import {LinkedMachine} from '../../Model/anagrafiche/ParcoMacchine';
import {AppezzamentoXParcoMacchine} from '../../Model/anagrafiche/appezzamento-x-parco-macchine';
import { LeggiClasseTessitura } from 'app/Model/metaschema/LeggiClasseTessitura';
import { ClasseTessitura } from 'app/Model/anagrafiche/ClasseTessitura';
import { AnagraficaNGClient, AppezzamentoFiltroTemporale } from '../api.service';

class BloccaSbloccaAppezzamenti{
  appezzamenti: Appezzamento[];
  blocca: boolean;
}

export class LeggiAppezzamento {
  public appezzamento: Appezzamento;
  public data: Date;
  public filtroData: Boolean;
  public leggiIndirizzi: Boolean;
  public leggiCatasto: Boolean;
  public leggiImpianti: Boolean;
  public leggiDistinte: Boolean;
  public leggiCartografia: Boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AppezzamentiBudgetService extends ImpiantiFactoryService {

  constructor(
    protected masterService: MasterService,
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    protected conversionService: ConversionService,
    protected intlService: IntlService,
    protected budgetService: BudgetService,
    private anagraficaNGClient: AnagraficaNGClient
  ) {
    super(masterService, ajaxAgronicaService, conversionService, intlService)
  }

  setAppezzamento(appezza: Appezzamento){
    this.AppezzamentoSource.next(appezza);
  }

  getAppezzamento(): Appezzamento {
    return this.AppezzamentoSource.getValue();
  }

  getArray_Metodo_Produzione(): Array<any> {
    return this.Array_Metodo_Produzione;
  }

  leggiAppezzamenti(objParametri: ObjParametriAgenda) {
    return new Promise<string>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('Budget/Leggi_Esercizi_Anagrafica', {InData: {
          Id_Budget: this.budgetService.getBudget().budgetId,
          ElementoAnagrafico: objParametri
        }}).pipe(map(r => {
        resolve(r.RispostaStringa);
      })).subscribe();
    });
  }

  leggiAppezzamento(
    objParametri: ObjParametriAgenda,
    data: Date,
    filtroData: boolean,
    leggiIndirizzi: boolean,
    leggiCatasto: boolean,
    leggiImpianti: boolean,
    leggiDistinte: boolean,
    leggiCartografia: boolean,
    setIsLoading: boolean
  ) : Promise<Appezzamento>{
    const pkCentro: PKCentroAziendale = { codice:objParametri.Sa_Cod , partitaIva:objParametri.Piva };
    const pkApp: PKAppezzamento = { codice: objParametri.Appezza, centroAziendalePK: pkCentro };

    const leggiAppezzamento: LeggiAppezzamento = new LeggiAppezzamento();
    leggiAppezzamento.appezzamento = new Appezzamento(pkApp);
    leggiAppezzamento.data = data;
    leggiAppezzamento.filtroData = filtroData;
    leggiAppezzamento.leggiIndirizzi = leggiIndirizzi;
    leggiAppezzamento.leggiCatasto = leggiCatasto;
    leggiAppezzamento.leggiImpianti = leggiImpianti;
    leggiAppezzamento.leggiDistinte = leggiDistinte;
    leggiAppezzamento.leggiCartografia = leggiCartografia;

    let bdgAnagrafica: BudgetAnagrafica<LeggiAppezzamento> = new BudgetAnagrafica<LeggiAppezzamento>(
      this.budgetService.getBudget().budgetId,
      leggiAppezzamento
    );

    return new Promise((resolve) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<LeggiAppezzamento>, Appezzamento>(
        'Budget/Leggi_Appezzamento_Anagrafica', bdgAnagrafica, setIsLoading
      ).pipe(map(r => {
        resolve(r.RispostaStringa);
      })).subscribe();
    })
  }

  readAgriculturalPlotLight(objParametri: ObjParametriAgenda): Observable<AppezzamentoJoinDescrizioni> {
    throw new Error('Method not yet implemented');
  }

  readAppezzamentiXParcoMacchine(params: AppezzamentoXParcoMacchine): Observable<LinkedMachine<PKAppezzamento>[]> {
    throw new Error('Method not yet implemented');
  }

  writeAppezzamentiXParcoMacchine(app: Appezzamento[]): Observable<rispostaStandard<boolean>> {
    throw new Error('Method not yet implemented');
  }

  editAppezzamentiXParcoMacchine(machines: LinkedMachine<PKAppezzamento>[]): Observable<rispostaStandard<boolean>> {
    throw new Error('Method not yet implemented');
  }

  deleteAppezzamentiXParcoMacchine(axp: AppezzamentoXParcoMacchine): Observable<rispostaStandard<boolean>> {
    throw new Error('Method not yet implemented');
  }

  deleteAppezzamentiXParcoMacchineRecords(plots: Appezzamento[]): Observable<rispostaStandard<boolean>> {
    throw new Error('Method not yet implemented');
  }

  updateWeaving(app: Appezzamento[]): Observable<rispostaStandard<boolean>> {
    throw new Error('Method not yet implemented');
  }

  updateSlope(app: Appezzamento[]): Observable<rispostaStandard<boolean>> {
    throw new Error('Method not yet implemented');
  }

  updateConstrain(plots: Appezzamento[]): Observable<rispostaStandard<boolean>> {
    throw new Error('Method not yet implemented');
  }

  leggiAppezzamentoObs(
    objParametri: ObjParametriAgenda,
    data: Date,
    filtroData: boolean,
    leggiIndirizzi: boolean,
    leggiCatasto: boolean,
    leggiImpianti: boolean,
    leggiDistinte: boolean,
    leggiCartografia: boolean
  ): Observable<Appezzamento> {

    return from(this.leggiAppezzamento(objParametri,
      data, filtroData,
      leggiIndirizzi, leggiCatasto,
      leggiImpianti, leggiDistinte,
      leggiCartografia, true))
  }

  leggiAppezzamentoUtilizzoTerreno(pkImpianto: PKImpianto) {
    return new Promise<UtilizzoTerreno>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<PKImpianto>, UtilizzoTerreno>(
        'AnagraficaNG/LeggiAppezzamentoUtilizzoTerreno',
        new BudgetAnagrafica<PKImpianto>(
          this.budgetService.getBudget().budgetId,
          pkImpianto
        )
      ).pipe(map(appResp => {
        return appResp.RispostaStringa;
      })).subscribe();
    });
  }

  CaricaDatiCatastaliObs(settings: CaricaCatastoSettings) {
    const appezzamento: Appezzamento = this.getAppezzamento();

    let objAgenda: ObjParametriAgenda = new ObjParametriAgenda();
    objAgenda.Piva = settings.Piva;
    objAgenda.Sa_Cod = settings.Sa_Cod;
    const campo_Cod: number = settings.Campo_Cod;
    const flag_Macrousi: boolean = settings.chkMacrousi;
    const flag_Utilizzi: boolean = settings.chkUtilizzi;
    const flag_Varieta: boolean = settings.chkVarieta;

    if (objAgenda.Sa_Cod == 0) {
      return of([]);
    }

    if (appezzamento.validita != undefined) {
      return this.caricaDatiCatastali({
        parametri_ObjParametriAgenda_NG: objAgenda,
        idBudget: this.budgetService.getBudget().budgetId,
        Campo_Cod: campo_Cod,
        flag_Macrousi: flag_Macrousi,
        flag_Utilizzi:flag_Utilizzi,
        flag_Varieta: flag_Varieta,
        ValiditaInizio: appezzamento.validita.inizio,
        ValiditaFine: appezzamento.validita.fine,
      });
    }
  }

  private caricaDatiCatastali(settings: CaricaDatiCatastali): Observable<any[]> {
    let bdgParams: BudgetAnagrafica<CaricaDatiCatastali> = new BudgetAnagrafica<CaricaDatiCatastali>(settings.idBudget, settings);

    return this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<CaricaDatiCatastali>, any[]>(
      'Budget/CaricaDatiCatastali',
      bdgParams
    ).pipe(map(r => {
      return r.RispostaStringa;
    }));
  }

  // TODO Salvo: da correggere: deve chiamare LeggiInvestimentoCatastale
  public leggiInvestimentoCatastale(settings: LeggiInvestimentoCatastale): Observable<any[]> {
    let params: BudgetAnagrafica<LeggiInvestimentoCatastale> = new BudgetAnagrafica<LeggiInvestimentoCatastale>(
      this.budgetService.getBudget().budgetId,
      settings
    );
    return this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<LeggiInvestimentoCatastale>, any[]>(
      'Budget/LeggiInvestimentoCatastale',
      params
    ).pipe(map(r => {
      return r.RispostaStringa;
    }));
  }

  CaricaDatiCatastali(
    objParametri: ObjParametriAgenda,
    Campo_Cod: number,
    flag_Macrousi: boolean,
    flag_Utilizzi: boolean,
    flag_Varieta: boolean,
    ValiditaInizio: Date,
    ValiditaFine: Date
  ) {

    return new Promise<string>(async (resolve, reject) => {

      return this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>('AnagraficaNG/CaricaDatiCatastali', {
        parametri_ObjParametriAgenda_NG: objParametri,
        Campo_Cod: Campo_Cod,
        flag_Macrousi: flag_Macrousi,
        flag_Utilizzi: flag_Utilizzi,
        flag_Varieta: flag_Varieta,
        ValiditaInizio: ValiditaInizio,
        ValiditaFine: ValiditaFine
      }).pipe(map(r => {
        resolve(r.RispostaStringa);
      }));
    });

  }

  LeggiParticelle_Per_Appezzamento(objParametri: ObjParametriAgenda) {
    return new Promise<string>(async (resolve, reject) => {

      this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, string>('AnagraficaNG/LeggiParticellePerAppezzamento', objParametri).pipe(map(r => {
        resolve(r.RispostaStringa);
      })).subscribe();

    });
  }

  leggiClasseTessitura(leggiClasseTessitura: LeggiClasseTessitura) {
    return new Promise<ClasseTessitura>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiClasseTessitura, ClasseTessitura>('MetaschemaNG/LeggiClasseTessitura', leggiClasseTessitura).pipe(map(r => {
        resolve(r.RispostaStringa);
      })).subscribe();
    });
  }

  leggiClasseTessituraObs(leggiClasseTessitura: LeggiClasseTessitura) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiClasseTessitura, ClasseTessitura>('MetaschemaNG/LeggiClasseTessitura', leggiClasseTessitura).pipe(
      map(r => r.RispostaStringa)
    );
  }

  Controlla_Validita_Appezzamenti(objParametri: ObjParametriAgenda) {
    return new Promise<string>(async (resolve, reject) => {

      this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, string>('AnagraficaNG/ControllaValiditaAppezzamenti', objParametri).pipe(map(r => {
        resolve(r.RispostaStringa);
      })).subscribe();

    });
  }

  ScriviAppezzamento(appezzamento: Appezzamento) {
    return new Promise<rispostaStandard<Appezzamento>>(async (resolve, reject) => {
      const parametri: BudgetAnagrafica<Appezzamento> = new BudgetAnagrafica<Appezzamento>(
        this.budgetService.getBudget().budgetId,
        appezzamento
      );
      return this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<Appezzamento>, Appezzamento>(
        'Budget/Scrivi_Appezzamento_Anagrafica',
        parametri,
        false
      ).pipe(take(1)).subscribe((r) => {
        resolve(r);
      });
    })
  }

  ScriviAppezzamento_Obs(appezzamento: Appezzamento, mostraErrori: boolean = true, deleteRibaltamento: boolean = false): Observable<rispostaStandard<Appezzamento>> {
    let bdgParametri: BudgetAnagrafica<Appezzamento> = new BudgetAnagrafica<Appezzamento>(
      this.budgetService.getBudget().budgetId,
      appezzamento,
      deleteRibaltamento
    );
    return this.ajaxAgronicaAPIService.ajaxAPIPost<BudgetAnagrafica<Appezzamento>, Appezzamento>(
      'Budget/Scrivi_Appezzamento_Anagrafica',
      bdgParametri, false, true, mostraErrori
    );
  }

  ScriviAppezzamentoFiltroTemporale(appezzamentoFiltroTemporale: AppezzamentoFiltroTemporale): Observable<rispostaStandard<Appezzamento>> {
    return this.ScriviAppezzamento_Obs(appezzamentoFiltroTemporale.Appezzamento as any as Appezzamento);
  }

  bloccaSbloccaAppezzamenti(appezzamenti: Appezzamento[], blocca: boolean) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<BloccaSbloccaAppezzamenti, Appezzamento>(
      'AnagraficaNG/BloccaSbloccaAppezzamenti',
      {
        appezzamenti: appezzamenti,
        blocca: blocca
      }
    );
  }

  verifica_Superficie(impianto: Impianto, newSup: number) {
    let imp = impianto;
    imp.superficie = newSup;
    let rispostaStandard: rispostaStandard<string> = {
      Sessione: true,
      RispostaOK: true,
      RispostaConferma: true,
      ParametroDue: false,
      ParametroDue_stringa: "",
      Tipo: "",
      RispostaCompressa: null,
      Compressa: false,
      Errore: "",
      RispostaStringa: "",
      ErroriGias: []
    }
    return of(rispostaStandard)
    // return this.ajaxAgronicaAPIService.ajaxAPIPost<Impianto, RispostaStandard>(
    //     'AnagraficaNG/VerificaSuperficie',
    //     imp
    // )
  }

  verifica_OperazioniAgenda(impianto: Impianto) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Impianto, RispostaStandard>(
      'AnagraficaNG/VerificaOperazioniAgenda',
      impianto
    );
  }

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

  leggi(objParametri: ObjParametriAgenda): Observable<Array<any>> {
    return this.fetchEsercizi(objParametri);
    // let eserciziIndex = this.impresexEsercizi.impresexEserciziList.findIndex((val) => val.impresa.partitaIva == objParametri.Piva);
    // const requestDate = new Date();
    // if (eserciziIndex >= 0) {
    //   const esercizioObjSaved = this.impresexEsercizi.impresexEserciziList[eserciziIndex];
    //   return this.leggiMaxDataModifica(objParametri).pipe(
    //     switchMap((maxEditDate) => {
    //       if (esercizioObjSaved.requestDate > maxEditDate) {
    //         return of(esercizioObjSaved.esercizi);
    //       } else {
    //         return this.fetchEsercizi(objParametri).pipe(
    //           tap((val) => {
    //             const newEsercizioObj: EserciziRequest = {
    //               impresa: esercizioObjSaved.impresa,
    //               requestDate: requestDate,
    //               esercizi: val
    //             };
    //             //this.impresexEsercizi.impresexEserciziList[eserciziIndex] = newEsercizioObj;
    //           })
    //         )
    //       }
    //     })
    //   );
    // } else {
    //   return this.fetchEsercizi(objParametri).pipe(
    //     tap((val) => {
    //       let impresa: Impresa = new Impresa();
    //       impresa.partitaIva = objParametri.Piva;
    //       const esercizioObj: EserciziRequest = {
    //         impresa: impresa,
    //         esercizi: val,
    //         requestDate: requestDate
    //       }
    //       //this.impresexEsercizi.impresexEserciziList.push(esercizioObj);
    //     })
    //   );
    // }
  }

  private fetchEsercizi(objParametri: ObjParametriAgenda): Observable<Array<any>> {
    let parametri: BudgetAnagrafica<ObjParametriAgenda> = new BudgetAnagrafica<ObjParametriAgenda>(
      this.budgetService.getBudget().budgetId,
      objParametri
    );
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any ,any[]>(
      'Budget/Leggi_Esercizi_Anagrafica',
      parametri).pipe(
      map((data) => {
        let Resp = data.RispostaStringa;
        Resp.forEach(el => {
          el.Data_Creazione = this.intlService.parseDate(el.Data_Creazione);
          el.Data_Modifica = this.intlService.parseDate(el.Data_Modifica);
          el.Validita_Fine = this.intlService.parseDate(el.Validita_Fine);
          el.Validita_Inizio = this.intlService.parseDate(el.Validita_Inizio);
          if (el.Data_Inizio_Portinnesto == null || el.Data_Inizio_Portinnesto == '') {
            el.Data_Inizio_Portinnesto = AGRODATAINIZIO;
          } else {
            el.Data_Inizio_Portinnesto = this.intlService.parseDate(el.Data_Inizio_Portinnesto);
          }
        });
        return Resp;
      })
    );
  }

  private leggiMaxDataModifica(objParametri: ObjParametriAgenda): Observable<Date> {
    let parametri: BudgetAnagrafica<string> = new BudgetAnagrafica<string>(
      this.budgetService.getBudget().budgetId,
      objParametri.Piva
    );
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Object, Data>(
      'Budget/Leggi_Max_DataModifica',
      parametri).pipe(
      map(r => { return r.RispostaStringa.data }),
    )
  }

  setRipartoCatastoPresente(flag: boolean): void {
    this.ripartoCatastoPresenteSource.next(flag);
  }

  getRipartoCatastoPresente(): boolean {
    return this.ripartoCatastoPresenteSource.getValue();
  }

  public leggiGenerazionePoligoniDefaultValue(objParametri: ObjParametriAgenda): Observable<string> {
    return null;
  }

  getCodiceImpianto(piva: string, year: number): Observable<any> {
    return of({codiceImpianto: '', algoritmoCodifica: ''});
  }

  generateDescriptions(piva: string): Observable<any> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<string, any[]>(
      'AnagraficaNG/GenerateDescriptions',
      piva
    ).pipe(
      map(r => {
        return r.RispostaStringa;
      }),
    );
  }

  closePlant(esercizio: Esercizio): Observable<rispostaStandard<string>> {
    return of(undefined);
  }
}

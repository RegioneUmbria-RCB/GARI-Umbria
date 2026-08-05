import { Injectable } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import {Appezzamento, AppezzamentoJoinDescrizioni, PKAppezzamento} from 'app/Model/anagrafiche/Appezzamento';
import { PKCentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { Impianto, PKImpianto } from 'app/Model/anagrafiche/Impianto';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { UtilizzoTerreno } from 'app/Model/metaschema/utilizzi/UtilizzoTerreno';
import { lastValueFrom, map, Observable, of } from 'rxjs';
import { AjaxAgronicaAPIService } from '../ajax-agronica.api.service';
import { AjaxAgronicaService } from '../ajax-agronica.service';
import { ConversionService } from '../conversion.service';
import { MasterService, rispostaStandard, RispostaStandard } from '../master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {CaricaCatastoSettings, CaricaDatiCatastali, ImpiantiFactoryService} from '../ServiceFactory/impianti.factory.service';
import {Data} from './imprese.service';
import {LeggiInvestimentoCatastale} from '../ServiceFactory/investimento-catastale.factory.service';
import {Esercizio} from '../../Model/anagrafiche/Esercizio';
import {LinkedMachine} from '../../Model/anagrafiche/ParcoMacchine';
import {AppezzamentoXParcoMacchine} from '../../Model/anagrafiche/appezzamento-x-parco-macchine';
import { LeggiClasseTessitura } from 'app/Model/metaschema/LeggiClasseTessitura';
import { ClasseTessitura } from 'app/Model/anagrafiche/ClasseTessitura';
import { AnagraficaNGClient, AppezzamentoFiltroTemporale } from '../api.service';

class BloccaSbloccaAppezzamenti {
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
export class AppezzamentiService extends ImpiantiFactoryService {


  constructor(
    protected masterService: MasterService,
    protected ajaxAgronicaService: AjaxAgronicaService,
    protected ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    protected conversionService: ConversionService,
    protected intlService: IntlService,
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
      this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, string>('AnagraficaNG/LeggiAppezzamentiAnagrafica', objParametri).pipe(map(r => {
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
  ) {

    const pkCentro: PKCentroAziendale = { codice:objParametri.Sa_Cod , partitaIva:objParametri.Piva };
    const pkApp: PKAppezzamento = { codice: objParametri.Appezza, centroAziendalePK: pkCentro };


    // let appResp = await this.ajaxAgronicaService.ajaxAgronicaG<Appezzamento>(this.masterService.link_CoreWS + "/Anagrafica/Appezzamento.asmx/Leggi_Appezzamento_Global_Anagrafica", parametri);
    return lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiAppezzamento, Appezzamento>(
      'AnagraficaNG/LeggiAppezzamentoAnagrafica', {
        appezzamento: new Appezzamento(pkApp),
        data: data,
        filtroData: filtroData,
        leggiIndirizzi: leggiIndirizzi,
        leggiCatasto: leggiCatasto,
        leggiImpianti: leggiImpianti,
        leggiDistinte: leggiDistinte,
        leggiCartografia: leggiCartografia
      }, setIsLoading).pipe(
      map(resp => resp.RispostaStringa)
    ));
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

    const pkCentro: PKCentroAziendale = { codice:objParametri.Sa_Cod , partitaIva:objParametri.Piva };
    const pkApp: PKAppezzamento = { codice: objParametri.Appezza, centroAziendalePK: pkCentro };

    // let appResp = await this.ajaxAgronicaService.ajaxAgronicaG<Appezzamento>(this.masterService.link_CoreWS + "/Anagrafica/Appezzamento.asmx/Leggi_Appezzamento_Global_Anagrafica", parametri);
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiAppezzamento, Appezzamento>(
      'AnagraficaNG/LeggiAppezzamentoAnagrafica', {
        appezzamento: new Appezzamento(pkApp),
        data: data,
        filtroData: filtroData,
        leggiIndirizzi: leggiIndirizzi,
        leggiCatasto: leggiCatasto,
        leggiImpianti: leggiImpianti,
        leggiDistinte: leggiDistinte,
        leggiCartografia: leggiCartografia
      }).pipe(
      map(resp => resp.RispostaStringa)
    );
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

  readAgriculturalPlotLight(objParametri: ObjParametriAgenda): Observable<AppezzamentoJoinDescrizioni> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, AppezzamentoJoinDescrizioni>(
      'AnagraficaNG/ReadAgriculturalPlotLight', objParametri).pipe(
      map(resp => resp.RispostaStringa)
    );
  }

  readAppezzamentiXParcoMacchine(params: AppezzamentoXParcoMacchine): Observable<LinkedMachine<PKAppezzamento>[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<AppezzamentoXParcoMacchine, LinkedMachine<PKAppezzamento>[]>(
      'AnagraficaNG/ReadAppezzamentiXParcoMacchine', params).pipe(
      map(resp => resp.RispostaStringa)
    );
  }

  writeAppezzamentiXParcoMacchine(app: Appezzamento[]): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Appezzamento[], rispostaStandard<boolean>>(
      'AnagraficaNG/WriteAppezzamentiXParcoMacchine', app).pipe(
      map(resp => resp.RispostaStringa)
    );
  }

  editAppezzamentiXParcoMacchine(machines: LinkedMachine<PKAppezzamento>[]): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LinkedMachine<PKAppezzamento>[], rispostaStandard<boolean>>(
      'AnagraficaNG/EditAppezzamentiXParcoMacchine', machines).pipe(
      map(resp => resp.RispostaStringa)
    );
  }

  deleteAppezzamentiXParcoMacchine(axp: AppezzamentoXParcoMacchine): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<AppezzamentoXParcoMacchine, rispostaStandard<boolean>>(
      'AnagraficaNG/DeleteAppezzamentiXParcoMacchine', axp).pipe(
      map(resp => resp.RispostaStringa)
    );
  }

  deleteAppezzamentiXParcoMacchineRecords(plots: Appezzamento[]): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Appezzamento[], rispostaStandard<boolean>>(
      'AnagraficaNG/DeleteAppezzamentiXParcoMacchineRecords', plots).pipe(
      map(resp => resp.RispostaStringa)
    );
  }

  updateWeaving(app: Appezzamento[]): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Appezzamento[], rispostaStandard<boolean>>(
      'AnagraficaNG/UpdatePlotsWeaving', app).pipe(
      map(resp => resp.RispostaStringa)
    );
  }

  updateSlope(app: Appezzamento[]): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Appezzamento[], rispostaStandard<boolean>>(
      'AnagraficaNG/UpdatePlotsSlope', app).pipe(
      map(resp => resp.RispostaStringa)
    );
  }

  updateConstrain(plots: Appezzamento[]): Observable<rispostaStandard<boolean>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Appezzamento[], rispostaStandard<boolean>>(
      'AnagraficaNG/UpdatePlotsConstrain', plots).pipe(
      map(resp => resp.RispostaStringa)
    );
  }

  leggiAppezzamentoUtilizzoTerreno(pkImpianto: PKImpianto) {
    return new Promise<UtilizzoTerreno>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<PKImpianto, UtilizzoTerreno>(
        'AnagraficaNG/LeggiAppezzamentoUtilizzoTerreno', pkImpianto).pipe(map(appResp => {
        return appResp.RispostaStringa;
      })).subscribe();
    });
  }

  CaricaDatiCatastaliObs(settings: CaricaCatastoSettings): Observable<any[]> {
    const appezzamento = this.getAppezzamento();

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
        Campo_Cod: campo_Cod,
        flag_Macrousi: flag_Macrousi,
        flag_Utilizzi: flag_Utilizzi,
        flag_Varieta: flag_Varieta,
        ValiditaInizio: settings.Validita_Inizio,
        ValiditaFine: settings.Validita_Fine
      });
    }
  }

  private caricaDatiCatastali(settings: CaricaDatiCatastali): Observable<any[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<CaricaDatiCatastali, any[]>(
      'AnagraficaNG/CaricaDatiCatastali',
      settings
    ).pipe(map(r => {
      return r.RispostaStringa;
    }));
  }

  public leggiInvestimentoCatastale(settings: LeggiInvestimentoCatastale): Observable<any[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<LeggiInvestimentoCatastale, any[]>(
      'AnagraficaNG/LeggiInvestimentoCatastale',
      settings
    ).pipe(map(r => {
      return r.RispostaStringa;
    }));
  }

  CaricaDatiCatastali(objParametri: ObjParametriAgenda, Campo_Cod: number, flag_Macrousi: boolean, flag_Utilizzi: boolean, flag_Varieta: boolean, ValiditaInizio: Date,
                      ValiditaFine: Date) {

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

  Controlla_Validita_Appezzamenti(objParametri: ObjParametriAgenda) {
    return new Promise<string>(async (resolve, reject) => {
      this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, string>('AnagraficaNG/ControllaValiditaAppezzamenti', objParametri).pipe(map(r => {
        resolve(r.RispostaStringa);
      })).subscribe();
    });
  }

  ScriviAppezzamento(appezzamento: Appezzamento) {
    return lastValueFrom(this.ajaxAgronicaAPIService.ajaxAPIPost<Appezzamento, Appezzamento>(
      'AnagraficaNG/ScriviAppezzamentoAnagrafica',
      appezzamento, false
    ));
  }

  ScriviAppezzamentoFiltroTemporale(appezzamentoFiltroTemporale: AppezzamentoFiltroTemporale): Observable<rispostaStandard<Appezzamento>> {
    return this.anagraficaNGClient
      .anagraficaNGScriviAppezzamentoAnagraficaFiltroTemporale(appezzamentoFiltroTemporale)
      .pipe(map(r => r as any as rispostaStandard<Appezzamento>));
  }

  ScriviAppezzamento_Obs(appezzamento: Appezzamento, mostraErrori: boolean = true, deleteRibaltamento: boolean = false): Observable<rispostaStandard<Appezzamento>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Appezzamento, Appezzamento>(
      'AnagraficaNG/ScriviAppezzamentoAnagrafica',
      appezzamento, false, true, mostraErrori
    );
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

    return this.ajaxAgronicaAPIService.ajaxAPIPost<Impianto, string>(
      'AnagraficaNG/VerificaSuperficie',
      imp
    )
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
  }

  private fetchEsercizi(objParametri: ObjParametriAgenda): Observable<Array<any>> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<ObjParametriAgenda, any[]>(
      'AnagraficaNG/LeggiEserciziAnagrafica',
      objParametri
    ).pipe(
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
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Object, Data>(
      'AnagraficaNG/LeggiMaxDataModifica',
      {Ricerca: objParametri.Piva}).pipe(
      map(r => { return r.RispostaStringa.data }),
    );
  }

  public leggiGenerazionePoligoniDefaultValue(objParametri: ObjParametriAgenda): Observable<string> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Object, string>(
      'AnagraficaNG/leggiGenerazionePoligoniDefaultValue',
      objParametri.Piva).pipe(
      map(r => { return r.RispostaStringa }),
    )
  }

  setRipartoCatastoPresente(flag: boolean){
    this.ripartoCatastoPresenteSource.next(flag);
  }

  getRipartoCatastoPresente(): boolean {
    return this.ripartoCatastoPresenteSource.getValue();
  }

  getCodiceImpianto(piva: string, year: number): Observable<any> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, string>(
      'AnagraficaNG/getCodiceImpianto',
      {piva: piva, year: year}
    ).pipe(
      map(r => r.RispostaStringa),
    );
  }

  generateDescriptions(piva: string): Observable<any[]> {
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
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Esercizio, string>(
      'AnagraficaNG/ClosePlant',
      esercizio
    )
  }
}

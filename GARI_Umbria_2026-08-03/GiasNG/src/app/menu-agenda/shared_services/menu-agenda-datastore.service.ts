import {Injectable} from '@angular/core';
import {ImpresaKendoServerResult} from 'app/anagrafica/imprese/imprese.model';
import {MasterService} from 'app/Service/master.service';
import {parseServerResult} from 'app/Service/utils';
import { KendoGridColumn, KendoServerResult, ServerResult } from 'gias-kendo-grid';
import {cloneDeep} from 'lodash';
import {CookieService} from 'ngx-cookie-service';
import {BehaviorSubject, Observable, of, Subject} from 'rxjs';
import {switchMap, takeUntil} from 'rxjs/operators';
import {FiltersService} from '../components/filters/filters.service';
import {OperationsService} from '../components/operations-list/operations.service';
import {
  BrogliaccioServerResult,
  CreateRecipeData,
  enum_menuAgendaGridCommands,
  Filters,
  GridCommandItem,
  GridZooResult,
  IMenuAgendaGridCommand,
  ModificaListaRicetteQdC,
  RicettaRow,
  RicetteServerResult,
  TableQdCLink,
  TabTypes
} from '../components/utils';
import {faTrashAlt} from "@fortawesome/free-solid-svg-icons";
import {AjaxAgronicaAPIService} from 'app/Service/ajax-agronica.api.service';
import {APP_Ricetta_Operazione} from 'app/Service/Agenda/Agenda.service';
import {BaseCodeDescr} from '../../Model/baseClass/baseCodeDescr';
import {ObjParametriAgendaService} from "../../Service/obj-parametri-agenda.service";
import {FiltersConfig} from "../components/models";


//const importaAgendaImpresaSelezionata_Old = "/Contab/Ricette.asmx/ImportaAgendaAziendaDaTabelleAPP";
const importaAgendaImpresaSelezionata = "Contab/ImportaAgendaAziendaDaTabelleAPP";
//const importaTuttaLAgenda_Old = "/Contab/Ricette.asmx/ImportaAgendaDaTabelleAPP";
const importaTuttaLAgenda = "/Contab/Ricette.asmx/ImportaAgendaDaTabelleAPP";
//const importaRicetteAgenda = "Contab/Ricette.asmx/ImportaRicetteAziendaDaTabelleAPP";
const importaRicetteAgenda = "Contab/ImportaRicetteAziendaDaTabelleAPP";
//const importaRicette_Old = "Contab/Ricette.asmx/ImportaRicetteDaTabelleAPP";
const importaRicette = "Contab/ImportaRicetteDaTabelleAPP";

const activeTabCacheId = "QDC_MenuAgenda_TabAttivo";
//const caricaAgendaLink_Old = "/GiasApp/SincroDatiApp.asmx/CaricaDatiApp";
const caricaAgendaLink = "/GiasApp/CaricaDatiApp";


//export const LinkRicette_Old = '/Agenda/MenuBS_WS.asmx/CaricaRicette';
export const LinkRicette = 'Agenda/CaricaRicette';
//const CaricaLavcodOperazioniVisibili_Old = "Agenda/Agenda.asmx/CaricaGrigliaOperazioni";
const CaricaLavcodOperazioniVisibili = "Agenda/CaricaGrigliaOperazioni";


@Injectable({providedIn: 'root'})
export class MenuAgendaDataStore {
  isMenuAgendaCalledFromGis = false;

  recipeModalData: BehaviorSubject<CreateRecipeData> = new BehaviorSubject(null);
  selectedTabstrip: BehaviorSubject<TabTypes> = new BehaviorSubject(TabTypes.Initializer);
  commandExecuted: BehaviorSubject<IMenuAgendaGridCommand> = new BehaviorSubject(null);

  public get currentTab() {
    return this.selectedTabstrip.value;
  }

  public set currentTab(index: TabTypes) {
    this.selectedTabstrip.next(index);
  }

  private _lav_cod_ricettabili: number[];

  public set lav_cod_ricettabili(value: number[]) {
    this._lav_cod_ricettabili = value;
  }

  public get lav_cod_ricettabili() {
    return this._lav_cod_ricettabili;
  }

  public setupShowDDT: boolean = false;

  public gridDataBrogliaccio: BrogliaccioServerResult;
  public gridDataRicette: RicetteServerResult;
  public gridDataZoo: GridZooResult;

  private _gridDataQdc: ImpresaKendoServerResult;
  public set gridDataQdc(value: ImpresaKendoServerResult) {
    this._gridDataQdc = value;
  }

  public get gridDataQdc() {
    return this._gridDataQdc;
  }

  /** Section Gestione Costi */
  public _lavCodOperazioniVisibili: number[];

  get lavCodOpVisibili() {
    return this._lavCodOperazioniVisibili;
  }

  set lavCodOpVisibili(values: number[]) {
    this._lavCodOperazioniVisibili = values;
  }

  /** Items in drop-down button */
  private _gridCommandList: GridCommandItem[] = [
    {
      actionName: "Informazioni",
      action: enum_menuAgendaGridCommands.INFO,
      iconClass: "btn-Info k-grid-Info k-rounded-md",
      isStatic: true,
      paramsforTrasloco: null
    },
    {
      actionName: "Modifica",
      action: enum_menuAgendaGridCommands.MODIFICA,
      iconClass: 'faEditFull',
      isStatic: true,
      paramsforTrasloco: null
    },
    {
      actionName: "Cancella",
      action: enum_menuAgendaGridCommands.CANCELLA,
      iconClass: '',
      fontawesomeIcon: faTrashAlt,
      isStatic: true,
      paramsforTrasloco: null
    }, {
      actionName: "Copia",
      iconClass: 'faCopySingle02',
      action: enum_menuAgendaGridCommands.COPIA,
      isStatic: true,
      paramsforTrasloco: null
    }
  ];
  get gridCommandList(): GridCommandItem[] {
    return cloneDeep(this._gridCommandList);
  }

  public addBaseCommand(btn: GridCommandItem) {
    this._gridCommandList.push(btn);
  }

  constructor(private cookies: CookieService,
              private filters: FiltersService,
              private operations: OperationsService,
              private APIService: AjaxAgronicaAPIService,
              private masterService: MasterService,
              private objParametriAgendaService: ObjParametriAgendaService) {

  }


  /*public ImportaAgendaDaTabelleAPP_Old(importaSoloAziendaSelezionata: boolean) {
      let link = importaSoloAziendaSelezionata
          ? importaAgendaImpresaSelezionata: importaTuttaLAgenda;

      return this.http.post_legacy(
          link,
          this.ImportaTabelleAppParams(importaSoloAziendaSelezionata),
          true
      );
  }*/

  public ImportaAgendaDaTabelleAPP(importaSoloAziendaSelezionata: boolean) {
    const piva = importaSoloAziendaSelezionata
      ? this.objParametriAgendaService.getObjParamValue().Piva : "";
    let link = importaSoloAziendaSelezionata
      ? importaAgendaImpresaSelezionata : importaTuttaLAgenda;
    return this.APIService.ajaxAPIPost<any, any>(link, piva);
  }

  /*public ImportaRicetteDaTabelleAPP(importaSoloAziendaSelezionata: boolean) {
      let link = importaSoloAziendaSelezionata
          ? importaRicetteAgenda: importaRicette;

      return this.http.post_legacy(
          link,
          this.ImportaTabelleAppParams(importaSoloAziendaSelezionata),
          true
      );
  }*/

  public ImportaRicetteDaTabelleAPP(importaSoloAziendaSelezionata: boolean) {
    const piva = importaSoloAziendaSelezionata
      ? this.objParametriAgendaService.getObjParamValue().Piva
      : "";
    if (importaSoloAziendaSelezionata) {
      return this.APIService.ajaxAPIPost<any, any>(importaRicetteAgenda, piva, true);
    } else {
      return this.APIService.ajaxAPIGet<any, any>(importaRicette, piva, true);
    }
  }


  public GetQdCGridData(filters: Filters): Observable<unknown> {
    return this.APIService.ajaxAPIPost<any, any>(TableQdCLink, {
      filtro: JSON.stringify(filters),
      piva: this.objParametriAgendaService.getObjParamValue().Piva
    })
  }


  memorizeCurActiveTab(tabAttivoIndx: any) {
    this.cookies.set(
      activeTabCacheId, JSON.stringify(tabAttivoIndx), {path: '/'}
    );
  }

  getTabIndexFromCache() {
    return this.cookies.get(activeTabCacheId);
  }


  initializeOperations(signal: Subject<void>) {
    this.selectedTabstrip.pipe(takeUntil(signal)).subscribe((selectedIndx: TabTypes) => {
      if (selectedIndx === TabTypes.Initializer)
        return;

      switch (selectedIndx) {
        case TabTypes.QuadernoDiCampagna:
          this.operations.LoadFavorites();
          break;
        case TabTypes.Ricette:
          this.operations.LoadFavoritesPerRicette();
          break;
        case TabTypes.Brogliaccio:
          this.operations.LoadFavoritesBrogliaccio();
          break;
        default:
          throw new Error('Undefined tab type!');
      }
    })
  }

  setPrevSelectedTab() {
    const cacheIndx = this.getTabIndexFromCache();
    let index = parseInt(cacheIndx);
    if (isNaN(index)) {
      index = 0;
    }

    this.currentTab = index;
  }

  CaricaZoo(tabType: TabTypes) {

    const providesArgs = (...args) => {
      let agenda = args[0];
      let master = args[1];

      const filters = this.filters.getFiltriRicette();

      return {
        filtri: JSON.stringify(filters),

        objParam_server: master.ObjParametri_Server,
        objParam_utenti: master.ObjParametri_Utenti
      };
    }

    const providesArgs_NG = (...args) => {
      const filters = this.filters.getFiltriRicette();

      return filters;
    }

    return this.APIService.ajaxAPIPost<any, any>(LinkRicette, providesArgs_NG.bind(this))
      .pipe(switchMap((data) => {
        let serverResult = data.RispostaStringa as any as ServerResult;
        switch (tabType) {
          case TabTypes.Ricette:
            this.gridDataRicette = parseServerResult(serverResult);
            return of(this.gridDataRicette);
          case TabTypes.Brogliaccio:
            this.gridDataBrogliaccio = parseServerResult(serverResult);
            return of(this.gridDataBrogliaccio);
          default:
            throw Error("Something went terribly wrong");
        }
      }));

    /*return this.http.post2<RicetteServerResult>(LinkRicette, providesArgs.bind(this))
    .pipe(switchMap((data) => {
        let serverResult = JSON.parse(data as any) as ServerResult;
        switch (tabType) {
            case TabTypes.Ricette:
                this.gridDataRicette = parseServerResult(serverResult);
                return of(this.gridDataRicette);
            case TabTypes.Brogliaccio:
                this.gridDataBrogliaccio = parseServerResult(serverResult);
                return of(this.gridDataBrogliaccio);
            default:
                throw Error("Something went terribly wrong");
        }
    }));*/
  }

  /**
   * Questo metodo viene chiamato in Gias2010 sia per le ricette che per il
   * brogliaccio.
   */
  CaricaRicette(tabType: TabTypes) {

    const providesArgs = (...args) => {
      let agenda = args[0];
      let master = args[1];

      let filters = null;
      switch (tabType) {
        case TabTypes.Ricette:
          filters = this.filters.getFiltriRicette();
          break;

        case TabTypes.Brogliaccio:
          filters = this.filters.getFiltriBrogliaccio();
          break;

        default:
          throw Error("Tab type not handled");
      }

      let params = agenda.getObjParamValue();
      return {
        piva: params.Piva,
        filtri: JSON.stringify(filters),

        objParam_server: master.ObjParametri_Server,
        objParam_utenti: master.ObjParametri_Utenti
      };
    };

    const providesArgs_NG = (...args) => {
      let agenda = args[0];
      let master = args[1];

      let filters = null;
      switch (tabType) {
        case TabTypes.Ricette:
          filters = this.filters.getFiltriRicette();
          break;

        case TabTypes.Brogliaccio:
          filters = this.filters.getFiltriBrogliaccio();
          break;

        default:
          throw Error("Tab type not handled");
      }

      let params = agenda.getObjParamValue();
      return {
        piva: params.Piva,
        filtri: JSON.stringify(filters),

        objParam_server: master.ObjParametri_Server,
        objParam_utenti: master.ObjParametri_Utenti
      };
    };

    return this.APIService.ajaxAPIPost<any, any>(LinkRicette, providesArgs_NG(this.objParametriAgendaService, this.masterService))
      .pipe(switchMap((data) => {
        if (!data) {
          throw Error("Oh no! The output data is " + data + "!");
        }

        let serverResult = data.RispostaStringa as any as ServerResult;
        switch (tabType) {
          case TabTypes.Ricette:
            this.gridDataRicette = parseServerResult(serverResult);
            return of(this.gridDataRicette);
          case TabTypes.Brogliaccio:
            this.gridDataBrogliaccio = parseServerResult(serverResult);
            return of(this.gridDataBrogliaccio);
          default:
            throw Error("Something went terribly wrong");
        }
      }));

    /*return this.http.post2<RicetteServerResult>(LinkRicette, providesArgs.bind(this))
    .pipe(switchMap((data) => {
        if (!data) {
            throw Error("Oh no! The output data is "+ data +"!");
        }

        let serverResult = JSON.parse(data as any) as ServerResult;
        switch (tabType) {
            case TabTypes.Ricette:
                this.gridDataRicette = parseServerResult(serverResult);
                return of(this.gridDataRicette);
            case TabTypes.Brogliaccio:
                this.gridDataBrogliaccio = parseServerResult(serverResult);
                return of(this.gridDataBrogliaccio);
            default:
                throw Error("Something went terribly wrong");
        }
    }));*/
  }

  forceRefreshBrogliaccio() {
    this.gridDataBrogliaccio = null;
  }

  /**
   * Azzera i dati delle 3 giiglie. Questo porta al ricaricamento
   * dei dati (tramite una chiamata http) quando un tab viene cambiato.
   */
  public resetGridData() {
    this.azzeraDatiGrigliaQdc();
    this.azzeraDatiGrigliaBrogliaccio();
    this.azzeraDatiGrigliaRicette();
    this.azzeraDatiGrigliaZoo();
  }

  public azzeraDatiGrigliaQdc() {
    this.gridDataQdc = null;
  }

  public azzeraDatiGrigliaBrogliaccio() {
    this.gridDataBrogliaccio = null;
  }

  public azzeraDatiGrigliaRicette() {
    this.gridDataRicette = null;
  }

  public azzeraDatiGrigliaZoo() {
    this.gridDataZoo = null;
  }

  public async getStampePreferite(): Promise<GridCommandItem[]> {
    let stampe: GridCommandItem[] = [];
    const R = await this.APIService.ajaxAPIGet<string, object>('Agenda/getStampePreferite', "", false, true).toPromise();

    (R.RispostaStringa as Array<BaseCodeDescr>).forEach((item: BaseCodeDescr) => {
      stampe.push(new GridCommandItem(item.descrizione, item.codice));
    })
    return stampe;
  }

  /*public CaricaAgendaDaTabelleAPP_Old(importaSoloAziendaSelezionata: boolean, tipo: string) {
      const params = (...args: HttpArgs) => {
          const agenda = args[0];
          const master = args[1];
          let piva = "";
          if(importaSoloAziendaSelezionata)
              piva = agenda.getObjParamValue().Piva;

          return {
              tipo: tipo,
              piva: piva,
              objP_super_server: master.ObjParametri_Super_Server,
              objP_server: master.ObjParametri_Server,
              objP_utenti: master.ObjParametri_Utenti
          };
      };

      return this.http.post2(caricaAgendaLink, params, true, false);
  }*/

  public CaricaAgendaDaTabelleAPP(importaSoloAziendaSelezionata: boolean, tipo: string) {
    let objAgenda = this.objParametriAgendaService.getObjParamValue();
    const params = {
      tipo: tipo,
      piva: objAgenda.Piva
    };

    return this.APIService.ajaxAPIPost<any, any>(caricaAgendaLink, params, true);
  }

  public async InviaRicettaApp(listaRicette: RicettaRow[]) {
    return new Promise((resolve, reject) => {
      let lst: Array<APP_Ricetta_Operazione> = listaRicette.filter(r => r.Invia_App !== '1').map(r => {
        let ricetta = new APP_Ricetta_Operazione();
        ricetta.Ricetta_Cod = r.Ricetta_Cod;
        ricetta.Ricetta_Operazione_Cod = r.Ricetta_Operazione_Cod;
        return ricetta;
      })

      let inData = new ModificaListaRicetteQdC();
      inData.ListaRicette = lst;

      this.APIService.ajaxAPIPost<ModificaListaRicetteQdC, any>(
        'Agenda/GestioneFlagRicettaInviaApp', inData
      ).GiasSubscribe(R => {
        console.log('done', R)
        resolve(R);
      });

    });
  }

}

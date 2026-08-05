import { Injectable, Injector, Renderer2 } from '@angular/core';
import { RowClassArgs } from '@progress/kendo-angular-grid';
import {
  EditingMode,
  KendoGridColumn,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  RendererGridEvent
} from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { filter, forkJoin, map, Observable, of, skip, switchMap, take, takeUntil } from 'rxjs';
import { FiltersService } from '../filters/filters.service';
import { enum_menuAgendaGridCommands, GridCommandItem, RibaltamentoTypes, RicettaRow, TabTypes } from '../utils';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { DialogResult } from '@progress/kendo-angular-dialog';
import { MenuAgendaDataStore } from '../../shared_services/menu-agenda-datastore.service';
import { isNullOrUndefined, NumToStr } from 'app/Service/utils';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { BussinessMenuAgendaService } from '../../shared_services/bussiness-logic.service';
import {
  GestioneRichiesteService,
  KeyValuePair,
  ParametriAggiuntivi_QueryString
} from 'app/Service/gestione-richieste.service';
import { enum_PagineAgenda_2010, enum_PaginePianoConcimazione_2017, Enum_SiteRedirector } from 'app/Model/siti.enum';
import { MenuContestualeService } from 'app/Master/menu-contestuale/menu-contestuale.service';
import { Gias2010Redirector } from '../grid-qdc/Gias2010Redirector.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import {
  CommandsColumnSettings,
  CommandsDropDownSettings,
  DettagliColumnSettings
} from 'gias-kendo-grid';
import { UtilityFunctions } from '../../../Utility/UtilityFunctions';
import { AjaxAgronicaAPIService } from '../../../Service/ajax-agronica.api.service';
import { Ricetta_Operazione } from '../../../Service/Agenda/Agenda.service';
import { RicetteService } from './ricette.service';
import { enum_Tipo_Operazione_Agenda_Target, Tipo_Ricetta } from "../../../Model/attivita/Attivita";
import { MasterService, rispostaStandard } from '../../../Service/master.service';
import { Elimina_Ricetta_Brogliaccio } from "../models";
import { ImpostazioniAziendeCentriService } from 'app/profilazione/services/impostazioni/impostazioni-aziende-centri.service';
import { Tipo_Attivita, Stati } from 'gias-ui-kit';
import {RicetteSmartTractorService} from './ricette-smart-tractor.service';

export const LinkRicette = 'Agenda/CaricaRicette';

export const HasAuthorizationToModify = (elem: KendoGridRow) => elem['PermessoModifica'] != "False";
export const InUse = (elem: KendoGridRow) => elem['in_uso'] === "1";
export const TipoRicettaEPianoDistribuzioneConcimi = (elem: KendoGridRow) => elem['Tipo_Ricetta'] === 6;
export const InviataAdApp = (elem: KendoGridRow) => elem['Invia_App'] === "1";

@Injectable()
export class RicetteGridConfig extends AbstractGridConfigService<GridRicetteResult> {
  editingMode: EditingMode = EditingMode.IN_LINE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = "chiave";
  gridId: string = "RicetteGridConfig";
  applicaFiltri: boolean;

  private permessoOrdiniLavoroRicette_W: boolean;
  private permessoNuovoDocumento: boolean;
  private permessoRicercaDocumenti: boolean;
  private rows = [];

  constructor(
    private renderer: Renderer2,
    private APIService: AjaxAgronicaAPIService,
    private filters: FiltersService,
    injector: Injector,
    private giasDialogService: GiasDialogService,
    private store: MenuAgendaDataStore,
    private agenda: ObjParametriAgendaService,
    private business: BussinessMenuAgendaService,
    private gestioneRichieste: GestioneRichiesteService,
    private redirector: Gias2010Redirector,
    private menu: MenuContestualeService,
    private permessiUtenteService: PermessiUtenteService,
    private agendaService: ObjParametriAgendaService,
    private impostazioiImprese: ImpostazioniAziendeCentriService,
    private ricetteService: RicetteService,
    private ricetteSmartTractorService: RicetteSmartTractorService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private masterService: MasterService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.permessoOrdiniLavoroRicette_W = this.permessiUtenteService.canWritePermesso(enum_Security_Attivita.Gest_Ricette);
    this.permessoNuovoDocumento = this.permessiUtenteService.canWritePermesso(enum_Security_Attivita.Documentale_Inser);
    this.permessoRicercaDocumenti = this.permessiUtenteService.canReadPermesso(enum_Security_Attivita.Documentale_Lista);

    this.configureGrid();
    this.registerObservers();

    this.handleGridCommand();
  }

  private get selected() {
    return this.rows.filter(r => r['Selected']);
  }

  private configureGrid() {
    this.resizable.autoFitColumns = true;
    this.selectable.selectable.enabled = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.selectable.drag = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.columnSettings.showSelectAll = true;
    this.selectable.columnSettings.width = 35;
    this.generalSettings.height = "100%";
    this.behavior.excelSettings.enabled = !this.store.isMenuAgendaCalledFromGis;

    this.setDdlCommandMenu();
    this.setCommandsVisibility();
  }

  private setDdlCommandMenu() {
    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, removeBtn: false });
    this.dettagliColumn = new DettagliColumnSettings({
      editBtn: this.permessoOrdiniLavoroRicette_W && !this.store.isMenuAgendaCalledFromGis,
      edit: (data) => this.handle_Info_Edit_Ribaltamento_Buttons('edit', data)
    })
    if (this.store.isMenuAgendaCalledFromGis) return;

    this.cmdDropDown = new CommandsDropDownSettings({
      fullEditBtn: false, removeBtn: true, infoBtn: true
    });
    this.cmdDropDown.addCommand(new GridCommandItem(
      "RicercaDocumenti",
      enum_menuAgendaGridCommands.RICERCA_DOCUMENTI,
      'faQdCSearchDocument'
    ));
    this.cmdDropDown.addCommand(new GridCommandItem(
      "AggiungiNuovoAllegato",
      enum_menuAgendaGridCommands.NUOVO_ALLEGATO,
      'faQdCUploadFile'
    ));
    this.cmdDropDown.addCommand(new GridCommandItem(
      "Copia",
      enum_menuAgendaGridCommands.COPIA,
      'faCopySingle02'
    ));
    this.cmdDropDown.addCommand(new GridCommandItem(
      "QdC",
      enum_menuAgendaGridCommands.QUADERNO_DI_CAMPAGNA,
      'faQdCRowGotoQdC'
    ));
    this.cmdDropDown.addCommand(new GridCommandItem(
      "ApplicaRicettaDaFareBrogliaccio",
      enum_menuAgendaGridCommands.BROGLIACCIO,
      'faQdCRowGotoBrogliaccio'
    ));
    this.cmdDropDown.addCommand(new GridCommandItem(
      "StampaRicetta",
      enum_menuAgendaGridCommands.STAMPA_RICETTA,
      'faQdCRowPrintReceipt'
    ));
    this.cmdDropDown.addCommand(new GridCommandItem(
      "StampaOdL",
      enum_menuAgendaGridCommands.STAMPA_ORDINE_LAVORO,
      'faQdCRowPrintOdL'
    ));
    this.cmdDropDown.addCommand(new GridCommandItem(
      "VaiAlPianoConcimazione",
      enum_menuAgendaGridCommands.VAI_AL_PIANO_CONCIMAZIONE,
      ''
    ));
  }

  private setCommandsVisibility() {
    this.gridPublicService.openCommands.GiasSubscribe(ricetta => {
      //Per ora inibisco il tasto di ribaltamento ricetta->brogliaccio
      this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.BROGLIACCIO);

      if (!ricetta) return;

      if (!TipoRicettaEPianoDistribuzioneConcimi(ricetta)) {
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.VAI_AL_PIANO_CONCIMAZIONE);
      }

      if (InUse(ricetta)) {
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.BROGLIACCIO);
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.QUADERNO_DI_CAMPAGNA);
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.CANCELLA);
      }

      //Le ricette che sono state inviate all'APP non possono essere ribaltate in agenda, copiate o modificate
      if (InviataAdApp(ricetta)) {
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.COPIA);
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.QUADERNO_DI_CAMPAGNA);
      }

      if (TipoRicettaEPianoDistribuzioneConcimi(ricetta)) {
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.INFO);
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.COPIA);
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.CANCELLA);
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.STAMPA_ORDINE_LAVORO);
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.STAMPA_RICETTA);
      }

      if (!this.permessoOrdiniLavoroRicette_W) {
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.CANCELLA);
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.QUADERNO_DI_CAMPAGNA);
      }

      if (!HasAuthorizationToModify(ricetta)) {
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.CANCELLA);
      }

      if (this.permessoRicercaDocumenti) {
        this.business.CheckAttachedDocumentsRecipes(ricetta.piva, ricetta.Ricetta_Cod)
          .pipe(filter(hasAttachments => !hasAttachments))
          .subscribe(() => this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.RICERCA_DOCUMENTI));
      } else {
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.RICERCA_DOCUMENTI);
      }

      if (!this.permessoNuovoDocumento) {
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.NUOVO_ALLEGATO);
      }
    })
  }

  public registerObservers() {
    this.handleFilters();
  }

  private ribaltamentoDaRicettaAdAgenda(row: RicettaRow) {
    //Se il tipo ricetta è Piano Concimazione per il ribaltamento faccio il redirect alla trattamenti_2
    if (TipoRicettaEPianoDistribuzioneConcimi(row)) {
      this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
      const Parametri: Array<ParametriAggiuntivi_QueryString> = [
        { key: "r", value: row.Ricetta_Cod.toString(), codifica: true },
        { key: "operazione_ricetta", value: row.Ricetta_Operazione_Cod.toString(), codifica: true }
      ];

      const objAgenda = this.agenda.getObjParamValue();
      objAgenda.Piva = row.piva;
      objAgenda.Sa_Cod = 0;
      objAgenda.SaNome = "";
      objAgenda.Veg_Cod = row.veg_cod_r;
      objAgenda.Data = row.Ricetta_Operazione_Data;
      objAgenda.Id_Agenda = 0;
      objAgenda.Lav_Cod = row.lav_cod;
      objAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
      objAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
      objAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale;
      objAgenda.Programmazione_Cod = 0;
      objAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;

      this.gestioneRichieste.gestionePassaggioAltroSito(
        Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
        enum_PagineAgenda_2010.Pagina_Trattamenti,
        Parametri,
        objAgenda).then(resp => {
          this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
          window.location.href = resp
        });
    } else {
      this.handle_Info_Edit_Ribaltamento_Buttons('new', row, RibaltamentoTypes.Da_Ricetta_ad_Agenda);
    }
  }

  private ribaltamentoDaRicettaABrogliaccio(row: RicettaRow) {
    //this.store.forceRefreshBrogliaccio();
    this.handle_Info_Edit_Ribaltamento_Buttons('new', row, RibaltamentoTypes.Da_Ricetta_a_Brogliaccio);
  }

  private handleGridCommand() {
    this.gridPublicService.commandEvent.GiasSubscribe(ev => {
      if (!ev) return;
      switch (ev.command.action) {
        case enum_menuAgendaGridCommands.COPIA:
          this.copiaRicetta(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.BROGLIACCIO:
          this.ribaltamentoDaRicettaABrogliaccio(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.QUADERNO_DI_CAMPAGNA:
          this.ribaltamentoDaRicettaAdAgenda(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.STAMPA_RICETTA:
          this.apriRicettaDaStampare(ev.dataItem.Ricetta_Cod);
          break;
        case enum_menuAgendaGridCommands.STAMPA_ORDINE_LAVORO:
          this.apriPianoLavoriDaStampare(ev.dataItem.Ricetta_Cod);
          break;
        case enum_menuAgendaGridCommands.INFO:
          this.handle_Info_Edit_Ribaltamento_Buttons('info', ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.MODIFICA:
          this.handle_Info_Edit_Ribaltamento_Buttons('edit', ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.RICERCA_DOCUMENTI:
          this.business.ApriKendoWindowRicercaDocumenti(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.NUOVO_ALLEGATO:
          this.business.ApriKendoWindowAggiungiNuovoAllegato(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.VAI_AL_PIANO_CONCIMAZIONE:
          this.redirectToMenuBS_PianoConcimazione_2017(ev.dataItem);
          break;
        // case enum_menuAgendaGridCommands.CANCELLA:
        //     let kendoGrid = this.qdcGridRef as unknown as GiasKendoGridComponent;
        //     this.business.Cancella(kendoGrid, ev.dataItem);
        //     break;
      }
    })
  }

  private handleFilters() {
    this.filters.subscribeToFiltersChange().pipe(takeUntil(this.signal), skip(1)).subscribe(() => {
      this.applicaFiltri = true;
      this.gridPublicService.refresh(true);
    });
  }

  public handle_Info_Edit_Ribaltamento_Buttons(action: string, dataItem: any, tipo_ribaltamento: RibaltamentoTypes = RibaltamentoTypes.Nessuno) {
    if (action !== 'info' && dataItem.Invia_App === '1') return;

    let tipoOperazione = 0;
    switch (action) {
      case 'info':
        tipoOperazione = Enum_DBTypeOperation.Read;
        break;
      case 'edit':
        tipoOperazione = Enum_DBTypeOperation.Update;
        break;
      case 'new':
        tipoOperazione = Enum_DBTypeOperation.Write;
        break;
    }

    let ObjParametriAgenda = this.agenda.getObjParamValue();
    ObjParametriAgenda.TipoOperazioneDB = tipoOperazione;
    ObjParametriAgenda.Lav_Cod = dataItem.lav_cod;
    ObjParametriAgenda.Lav_Des = dataItem.lav_des;
    ObjParametriAgenda.Sa_Cod = dataItem.Sa_Cod;
    ObjParametriAgenda.SaNome = dataItem.sa_nome;
    ObjParametriAgenda.Id_Agenda = 0;
    ObjParametriAgenda.Ricetta_Cod = dataItem.Ricetta_Cod;
    ObjParametriAgenda.Ricetta_Operazione_Cod = dataItem.Ricetta_Operazione_Cod;
    ObjParametriAgenda.Veg_Cod = dataItem.veg_cod_op;
    ObjParametriAgenda.Veg_Des = dataItem.veg_des_unificato;
    ObjParametriAgenda.Data = dataItem.Ricetta_Operazione_Data;

    switch (tipo_ribaltamento) {
      case RibaltamentoTypes.Da_Brogliaccio_ad_Agenda:
      case RibaltamentoTypes.Da_Ricetta_ad_Agenda:
        ObjParametriAgenda.TipoRicetta = 0;
        ObjParametriAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
        ObjParametriAgenda.Stato = 0;
        break;
      case RibaltamentoTypes.Da_Ricetta_a_Brogliaccio:
        ObjParametriAgenda.TipoRicetta = Tipo_Ricetta.Standard_Destinazioni;
        ObjParametriAgenda.TipoOperazioneAgenda = Tipo_Attivita.Ricetta;
        ObjParametriAgenda.Stato = Stati.Eseguita;
        break;
      default:
        ObjParametriAgenda.TipoRicetta = dataItem.Tipo_Ricetta;
        ObjParametriAgenda.TipoOperazioneAgenda = Tipo_Attivita.Ricetta;
        ObjParametriAgenda.Stato = dataItem.WAnagraficaStati_Cod;
        break;
    }

    ObjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;
    this.redirector.gestisciRedirectToQdC(ObjParametriAgenda, tipo_ribaltamento).then();
  }

  read(options?: any): Observable<KendoServerResult> {
    const ricette = this.store.gridDataRicette;
    if (!isNullOrUndefined(ricette) && !this.applicaFiltri && !this.menu.pendingWorkDone) {
      return of(ricette);
    }
    this.applicaFiltri = false;

    this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
    return this.store.CaricaRicette(TabTypes.Ricette)
      .pipe(map((result: KendoServerResult) => {
        let columns = new Array<KendoGridColumn>();
        //Se non sono state impostate le colonne della grid allora le prendo dalla
        //lettura lato server
        if (!this.gridPublicService?.giasGridComponent?.columns) {
          columns = result.columns;
        } else {
          columns = this.gridPublicService?.giasGridComponent?.columns;
        }

        this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

        this.ricetteService.reset();
        this.menu.markPendingWorkAsDone();
        this.rows = result.rows;
        return {
          columns: columns,
          model: result.model,
          rows: result.rows
        };
      }));
  }


  applyRendererRules(opts: RendererGridEvent): void {
    this.hideButtons(opts);
  }

  hideButtons(opts: RendererGridEvent) {

    const applyRules = (ricetta: KendoGridRow, domElement: any) => {
      const TipoRicettaEPuaa = (elem: KendoGridRow) => elem['Tipo_RIcetta'] == 2;

      if (InviataAdApp(ricetta) || InUse(ricetta) || TipoRicettaEPianoDistribuzioneConcimi(ricetta)) {
        UtilityFunctions.setStyle(this.renderer, domElement, ".divEditFull", 'display', 'none');
      } else {
        UtilityFunctions.setStyle(this.renderer, domElement, ".divEditFull", 'display', 'block');
      }

      // if (!HasAuthorizationToModify(ricetta)) {
      // UtilityFunctions.setStyle(this.renderer, domElement, ".btnCopia", 'display', 'none');
      // }

      // if (InUse(ricetta)) {
      // UtilityFunctions.setStyle(this.renderer, domElement, ".btnApplicaRicettaDaFareBrogliaccio", 'display', 'none');
      // UtilityFunctions.setStyle(this.renderer, domElement, ".btnApplicaRicettaDaFareAgenda", 'display', 'none');
      // }

      // if (!TipoRicettaELineaTecnica(ricetta)) {
      // if (ricetta['Raccoglitore_Cod'] !== '0') {
      //    UtilityFunctions.setStyle(this.renderer, domElement, ".btnCopia", 'display', 'none');
      // }

      if (TipoRicettaEPuaa(ricetta)) {
        alert('Tipo ricetta è PUAA');
        // $(this.querySelector("td:nth-child(1)")).html("<div class='btn btn-success' style='display:block;width:70px;border:0px;word-wrap:break-word;white-space:normal;' onclick='Gestione_Operazione_Menu(37)'>Vai al PUA Zootecnico</div>");
        // $(this.querySelector("td:nth-child(2)")).html("");
      }
    }

    const { grid, gridElRef } = { ...opts };
    let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
    grid.view.forEach((el, index) => {
      applyRules(el, visibleRows[index]);
    });
  }

  onRowClass = (e: RowClassArgs) => {
    let item = e.dataItem;
    let result: { [k: string]: boolean } = {};
    result.ricettaSalvataInAgenda = false;
    result.ricettaBloccata = false;
    result.ricettaPubblica = false;

    if (item.in_uso === "1") {
      result.ricettaSalvataInAgenda = true;
    } else if (item.blocco_flag === "1") {
      result.ricettaBloccata = true;
    } else if (item.piva === "") {
      result.ricettaPubblica = true;
    }
    return result;
  }

  public copiaRicetta(dataItem: RicettaRow) {
    let recipe = new Ricetta_Operazione();
    recipe.ricetta_cod = dataItem.Ricetta_Cod;
    this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
    this.ajaxAgronicaAPIService.ajaxAPIPost<Ricetta_Operazione, string>('Agenda/Ricette_Copia', recipe, false)
      .pipe(take(1))
      .subscribe((result: rispostaStandard<string>) => {
        this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

        if (result.RispostaStringa.toString().toLowerCase() === "true") {
          let messaggio = this.transloco.translate('RicettaCopiataCorrettamente', {});
          this.giasDialogService.baseSuccess('', messaggio, false);
          this.applicaFiltri = true;
          this.gridPublicService.refresh(true);
        } else {
          let messaggio = this.transloco.translate('ErroreDuranteCopia', {});
          this.giasDialogService.baseError('', messaggio, false);
        }
      });
  }

  private deleting = new Set<string>();

  perform(actionType: HttpAction, row: RicettaRow | RicettaRow[], massive = false): Observable<any[]> {
    if (actionType === HttpAction.REMOVE && massive) {
      this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
      const obs = new Array<Observable<any>>();
      const ricette = row as RicettaRow[];
      ricette.forEach(ricetta => obs.push(this.deleteItems(ricetta, massive)));
      forkJoin(obs).pipe(take(1))
        .subscribe(r => this.handleDeletioResult(r))
    } else if (actionType === HttpAction.REMOVE) {
      this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
      this.deleteItems(row as RicettaRow, massive).pipe(take(1))
        .subscribe(r => this.handleDeletioResult([r]))
    }
    return of([]);
  }

  private handleDeletioResult(esiti: any[]) {
    this.deleting.clear();
    this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
    console.log(esiti);
    if (!esiti.every(e => e.cancelOperation)) {
      // Esiste almeno un'operazione di eliminazione che non è stata cancellata
      this.showEsito(esiti)
    }
  }

  private deleteItems(row: RicettaRow, massive = false) {
    return this.business.getDialogEliminaRicettaBrogliaccio(
      row, this.needDeleteConfirmDialog(row, massive)
    ).pipe(take(1),
      switchMap((dialogResponse: DialogResult) => {
        if (dialogResponse['returnObj']) {
          return this.cancellaRicetta(row);
        }
        return of({
          cancelOperation: true,
          RispostaOK: false,
          ricetta: row
        });
      })
    );
  }

  private needDeleteConfirmDialog(row: RicettaRow, massive = false): boolean {
    const massiveHasOnlyOne = massive && this.selected.length === 1;
    const singleRecipeNotMassive = +row['Raccoglitore_Cod'] === 0 && !massive;
    if (+row['Raccoglitore_Cod'] > 0 && !this.deleting.has(row['Raccoglitore_Cod'])) {
      this.deleting.add(row['Raccoglitore_Cod']);
      return true;
    }
    return singleRecipeNotMassive || massiveHasOnlyOne;
  }

  private showEsito(esiti: any[]): void {
    if (esiti.filter(e => !e.cancelOperation).every(e => e.RispostaOK)) {
      let messaggio = this.transloco.translate('RicettaRimossaConSuccesso');
      this.giasDialogService.baseSuccess('', messaggio, false);
      this.store.resetGridData();
      this.gridPublicService.refresh(true)
    } else {
      let messaggio = this.transloco.translate('RimozioneRicettaFallita');
      messaggio += "\n" + this.transloco.translate('Ricette') + ": ";
      messaggio += esiti.filter(e => !e.RispostaOK)
        .map(e => '- ' + e.ricetta['Descrizione_Unica'])
        .reduce((a, b) => a + '<BR/>' + b)
      this.giasDialogService.baseError('', messaggio, false);
    }
  }

  cancellaRicetta(row: RicettaRow) {
    let siblings = [];
    if (row['Raccoglitore_Cod'] !== '0') {
      siblings = this.store.gridDataRicette.rows.filter(r => r['Raccoglitore_Cod'] === row['Raccoglitore_Cod'])
    } else if (this.deleting.has('0')) {
      siblings = this.selected.filter(r => r['Raccoglitore_Cod'] === '0');
    }
    if (!siblings.length) siblings = [row];

    const recipes: Ricetta_Operazione[] = siblings.map(r => {
      let recipe = new Ricetta_Operazione();
      recipe.ricetta_cod = r.Ricetta_Cod;
      recipe.ricetta_operazione_cod = r.Ricetta_Operazione_Cod;
      recipe.in_uso = +r.in_uso;
      return recipe;
    });

    const elimina_Ricetta_Brogliaccio = <Elimina_Ricetta_Brogliaccio>{
      ricette: recipes,
      variabiliInSessione_NG: this.masterService.variabiliInSessione
    };

    return this.APIService.ajaxAPIPost<Elimina_Ricetta_Brogliaccio, any>(
      'Agenda/Ricette_Cancella', elimina_Ricetta_Brogliaccio
    ).pipe(map(res => ({
      cancelOperation: false,
      RispostaOK: res.RispostaOK,
      ricetta: row
    })))
  }

  public apriRicettaDaStampare(ricettaCod: number) {
    const queryStr = [
      KeyValuePair.Create("ricetta_cod", NumToStr(ricettaCod), true),
      KeyValuePair.Create("ricetta_stampa_tipo", NumToStr(1), true)
    ]
    const objAgenda = this.agenda.getObjParamValue();
    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_RicetteStampa,
      queryStr,
      objAgenda).then(resp =>
        window.open(resp, '_blank', 'location=yes,height=768,width=1024,scrollbars=yes,status=yes')
      );
  }


  public apriPianoLavoriDaStampare(ricettaCod: number) {
    const queryStr = [
      KeyValuePair.Create("ricetta_cod", NumToStr(ricettaCod), true),
      KeyValuePair.Create("ricetta_stampa_tipo", NumToStr(3), true)
    ]
    const objAgenda = this.agenda.getObjParamValue();
    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_RicetteStampa,
      queryStr,
      objAgenda).then(resp =>
        window.open(resp, '_blank', 'location=yes,height=768,width=1024,scrollbars=yes,status=yes')
      );
  }

  public redirectToMenuBS_PianoConcimazione_2017(row: RicettaRow) {
    const objAgenda = this.agenda.getObjParamValue();
    objAgenda.Piva = row.piva;
    objAgenda.Sa_Cod = 0;
    objAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;

    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_PianoConcimazione_2017,
      enum_PaginePianoConcimazione_2017.MenuBS,
      [],
      objAgenda).then(resp => {
        window.location.href = resp
      });
  }
}

export class GridRicetteResult extends KendoServerResult {
  constructor(model, columns, rows) {
    super(model, columns, rows);
  }
}

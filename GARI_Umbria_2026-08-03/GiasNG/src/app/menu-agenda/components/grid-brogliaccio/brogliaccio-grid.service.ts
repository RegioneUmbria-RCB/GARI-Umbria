import {Injectable, Injector, Renderer2} from '@angular/core';
import {DialogResult, WindowRef} from '@progress/kendo-angular-dialog';
import {RowClassArgs} from '@progress/kendo-angular-grid';
import {MenuContestualeService} from 'app/Master/menu-contestuale/menu-contestuale.service';
import {GiasDialogAction, Dialog_Type, GiasDialogService} from 'app/Service/gias-dialog.service';
import {MasterService, RispostaStandard} from 'app/Service/master.service';
import {isNullOrUndefined} from 'app/Service/utils';
import {GiasIFrameWindowService} from 'gias-ui-kit';
import {
  AgrSelectableSettings,
  CommandsColumnSettings,
  CommandsDropDownSettings,
  DettagliColumnSettings
} from 'gias-kendo-grid';
import {
  EditingMode,
  KendoGridColumn,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  RendererGridEvent
} from 'gias-kendo-grid';
import {ConfigTemplate} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {UtilityFunctions} from 'app/Utility/UtilityFunctions';
import {forkJoin, lastValueFrom, Observable, of} from 'rxjs';
import {filter, map, skip, switchMap, take, takeUntil} from 'rxjs/operators';
import {BrogliaccioRow, enum_menuAgendaGridCommands, GridCommandItem, RibaltamentoTypes, TabTypes} from '../utils';
import {FiltersService} from '../filters/filters.service';
import {MenuAgendaDataStore} from '../../shared_services/menu-agenda-datastore.service';
import {enum_PagineGiasNG, enum_Security_Attivita} from 'app/Model/TipiEnumerativi';
import {PermessiUtenteService} from 'app/Service/permessi-utente.service';
import {BussinessMenuAgendaService} from 'app/menu-agenda/shared_services/bussiness-logic.service';
import {ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {Gias2010Redirector} from '../grid-qdc/Gias2010Redirector.service';
import {Ricetta_Operazione} from 'app/Service/Agenda/Agenda.service';
import {
  ImpostazioniAziendeCentriService
} from '../../../profilazione/services/impostazioni/impostazioni-aziende-centri.service';
import {AjaxAgronicaAPIService} from 'app/Service/ajax-agronica.api.service';
import {enum_PagineAgronicaSincro, Enum_SiteRedirector} from "../../../Model/siti.enum";
import {GestioneRichiesteService, ParametriAggiuntivi_QueryString} from "../../../Service/gestione-richieste.service";
import {Elimina_Ricetta_Brogliaccio} from '../models';
import { Tipo_Attivita} from 'gias-ui-kit';
import {enum_Impostazioni_Utenti} from "../../../Model/Impostazioni_Utenti.enum";
import { ObjParametriAgenda } from 'gias-ui-kit';

const AttemptRemoval = "Agenda/Ricette_VerificaSeCostiCollegatiECancella";
const RemovalWithAllConstraints = "Agenda/Ricette_CancellaRicettaCancellaCosti";
const PartialRemoval = "Agenda/Ricette_CancellaRicettaConvertiCosti";

const DO_NOTHING = of(false);
const RICARICA_LA_GRIGLIA = of(true);
const CONTINUE = of({});

const InUse = (elem: KendoGridRow) => elem['in_uso'] === "1";

@Injectable()
export class BrogliaccioGridConfig extends AbstractGridConfigService<KendoServerResult> {
  editingMode: EditingMode = EditingMode.IN_LINE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = "chiave";
  gridId: string = "BrogliaccioGridId";
  data: KendoServerResult;
  selectable: AgrSelectableSettings = new AgrSelectableSettings();
  applicaFiltri: boolean;

  window: WindowRef;
  ObjParametriAgenda: ObjParametriAgenda;
  public publicService;
  public SUPERUSER_IMPEDISCI_INS_MOD_BROGLIACCIO: boolean = false;

  private dialogAlreadyShown = false;

  private permessoBrogliaccio_W: boolean;
  private permessoQdC_W: boolean;
  private permessoNuovoDocumento: boolean;
  private permessoRicercaDocumenti: boolean;

  constructor(
    injector: Injector,
    private filters: FiltersService,
    private datastore: MenuAgendaDataStore,
    private menu: MenuContestualeService,
    public objParametriAgendaService: ObjParametriAgendaService,
    private renderer: Renderer2,
    private windowService: GiasIFrameWindowService,
    private dialog: GiasDialogService,
    private business: BussinessMenuAgendaService,
    private redirector: Gias2010Redirector,
    private permessiUtenteService: PermessiUtenteService,
    private impostazioiImprese: ImpostazioniAziendeCentriService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private masterService: MasterService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.publicService = this.gridPublicService;

    this.filters.subscribeToFiltersChange().pipe(takeUntil(this.signal), skip(1)).subscribe((val) => {
      this.applicaFiltri = true;
      this.gridPublicService.refresh(true);
    });

    this.ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.permessoBrogliaccio_W = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Brogliaccio, 2);
    this.permessoQdC_W = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG, 2);
    this.permessoNuovoDocumento = this.permessiUtenteService.canWritePermesso(enum_Security_Attivita.Documentale_Inser);
    this.permessoRicercaDocumenti = this.permessiUtenteService.canReadPermesso(enum_Security_Attivita.Documentale_Lista);

    let impostazioneSuperUser = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_IMPEDISCI_INS_MOD_BROGLIACCIO);

    if (impostazioneSuperUser && impostazioneSuperUser.Valore && impostazioneSuperUser.Valore !== "" && +impostazioneSuperUser.Valore === 1) {
      this.SUPERUSER_IMPEDISCI_INS_MOD_BROGLIACCIO = true;
    }

    this.handleCustomizations();
    this.handleCommandsVisibility();
    this.handleGridCommand();
  }

  private get selected() {
    return this.data.rows.filter(r => r['Selected']);
  }

  read(options?: any): Observable<KendoServerResult> {
    const data = this.datastore.gridDataBrogliaccio;
    if (!isNullOrUndefined(data) && !this.applicaFiltri && !this.menu.pendingWorkDone) {
      return of(data);
    }
    this.applicaFiltri = false;

    this.loadingService.set_isLoading({isLoading: true, component: this.gridPublicService.gridElRef});

    return this.datastore.CaricaRicette(TabTypes.Brogliaccio)
      .pipe(map((result: KendoServerResult) => {

        let columns = new Array<KendoGridColumn>();

        //Se non sono state impostate le colonne della grid allora le prendo dalla
        //lettura lato server
        if (!this.gridPublicService?.giasGridComponent?.columns) {
          columns = result.columns;
        } else {
          columns = this.gridPublicService?.giasGridComponent?.columns;
        }

        this.loadingService.set_isLoading({isLoading: false, component: this.gridPublicService.gridElRef});

        this.menu.markPendingWorkAsDone();
        return {
          columns: columns,
          model: result.model,
          rows: result.rows
        };
      }));

  }

  onRowClass = (event: RowClassArgs) => {
    let result: { [k: string]: boolean } = {};
    let row = event.dataItem;

    result.ricettaSalvataInAgenda = false;
    result.ricettaBloccata = false;
    result.ricettaPubblica = false;

    if (row.in_uso === "1") {
      result.ricettaSalvataInAgenda = true;
    } else if (row.blocco_flag === "1") {
      result.ricettaBloccata = true;
    } else if (row.piva === "") {
      result.ricettaPubblica = true;
    }
    return result;
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const {grid, gridElRef} = {...opts};
    let rows: [] = grid.data['data'];
    let domElems = gridElRef.nativeElement.querySelectorAll('tbody tr');

    rows.forEach((dataItem: any, index: number) => {
      let currDomRow = domElems[index];
      if (InUse(dataItem)) { // Ricetta salvata in agenda
        UtilityFunctions.setStyle(this.renderer, currDomRow, ".divEditFull", 'display', 'none');
      } else {
        UtilityFunctions.setStyle(this.renderer, currDomRow, ".divEditFull", 'display', 'block');
      }
    });
  }

  private handleCommandsVisibility() {
    this.gridPublicService.openCommands
      .pipe(takeUntil(this.signal), filter(ricetta => !!ricetta))
      .subscribe(ricetta => {
        //Il tasto di modifica ora è in una colonna dedicata
        this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.MODIFICA);

        if (!this.permessoBrogliaccio_W || ricetta.PermessoModifica === "False") {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.CANCELLA);
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.COPIA);
        }

        if (InUse(ricetta)) {
          this.cmdDropDown.removeCommand(enum_menuAgendaGridCommands.QUADERNO_DI_CAMPAGNA);
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

  public perform(actionType: HttpAction, row: BrogliaccioRow | BrogliaccioRow[]): Observable<any[]> {
    if (actionType == HttpAction.REMOVE) {
      return this.delete(row);
    }
  }

  private ribaltamentoDaBrogliaccioAdAgenda(row: BrogliaccioRow) {
    this.datastore.forceRefreshBrogliaccio();
    this.handle_Info_Edit_Ribaltamento_Buttons('new', row, RibaltamentoTypes.Da_Brogliaccio_ad_Agenda);
  }

  private handleGridCommand() {
    this.gridPublicService.commandEvent.GiasSubscribe(ev => {
      if (!ev) return;
      switch (ev.command.action) {
        case enum_menuAgendaGridCommands.COPIA:
          this.copiaBrogliaccio(ev.dataItem);
          break;
        case enum_menuAgendaGridCommands.QUADERNO_DI_CAMPAGNA:
          this.ribaltamentoDaBrogliaccioAdAgenda(ev.dataItem);
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
        // case enum_menuAgendaGridCommands.CANCELLA:
                //     let kendoGrid = this.qdcGridRef as unknown as GiasKendoGridComponent;
        //     this.business.Cancella(kendoGrid, ev.dataItem);
        //     break;
      }
    })
  }

  public copiaBrogliaccio(dataItem: any) {
    this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      'Agenda/Ricette_Copia', new Ricetta_Operazione(dataItem.Ricetta_Cod)
    ).pipe(take(1))
      .subscribe(res => {
        if (res.RispostaStringa) {
          let messaggio = this.transloco.translate('qdc.BrogliaccioCopiataCorrettamente', {});
          this.dialog.baseSuccess('', messaggio, false);
          this.applicaFiltri = true;
          this.gridPublicService.refresh(true)
        } else {
          let messaggio = this.transloco.translate('qdc.ErroreDuranteCopiaBrogliaccio', {});
          this.dialog.baseError('', messaggio, false);
        }
      });
  }

  /**
   * I vari casi:
   * Attempt to remove the operation:
   *  1. The chosen operation contains constraints:
   *    - Cancel operation
   *    - Remove all
   *    - Remove everything besides ore/costi
   *  2. Operation was successfully removed.
   *  3. Error thrown during cancellation attempt.
   */
  public delete(brogliaccioRow: BrogliaccioRow | BrogliaccioRow[]): Observable<any> {
    this.loadingService.set_isLoading({isLoading: true, component: this.gridPublicService.gridElRef});
    const rows: BrogliaccioRow[] = (brogliaccioRow['length'] ? brogliaccioRow : [brogliaccioRow]) as BrogliaccioRow[];
    const showDialog = rows['length'] === 1;
    const obs: Observable<any>[] = [];
    for (let row of rows)
      obs.push(this.business.getDialogEliminaRicettaBrogliaccio(row, showDialog));

    return forkJoin(obs).pipe(switchMap(async (dialogResponse: DialogResult[]) => {
      // Cerca di eliminare l'operazione
      if (dialogResponse.every(d => d['returnObj'])) {
        const risposteServer: {
          risposta: RispostaStandard,
          riga: BrogliaccioRow
        }[] = await lastValueFrom(this.tryDelete(rows))
        if (risposteServer.some(R => !R.risposta.RispostaOK)) {
          this.showErrorMessage("ErroreCancellazioneBrogliaccio");
        } else if (risposteServer.every(R => R.risposta.RispostaOK && !rowContainsConstraints(R.risposta))) {
          return CONTINUE;
        } else if (risposteServer.some(R => rowContainsConstraints(R.risposta))) {
          return await this.showRemovalDecisionDialog(risposteServer);
        }
      }
      return {returnObj: RemovalDecision.CANCEL};
    }), switchMap((result: GiasDialogAction) => {
      // Gestisce eventuali costi/ore collegate
      if (result.returnObj == RemovalDecision.CANCEL)
        return DO_NOTHING;
      if (result.returnObj == RemovalDecision.REMOVE_ALL)
        return this.removeMultiOperationWithAllConstraints(rows);
      if (result.returnObj == RemovalDecision.REMOVE_INTERVENTO_KEEP_ORE_COSTI)
        return this.removeMultiOperationPreservingOreCosti(rows);
      return CONTINUE; // in case the modal was not shown
    }), switchMap((risposte: { risposta: RispostaStandard, riga: BrogliaccioRow }[]) => {
      // Fine operazione, mostra messaggio finale
      this.loadingService.set_isLoading({isLoading: false, component: this.gridPublicService.gridElRef});
      if (!risposte) return DO_NOTHING; // modal not shown or cancel pressed
      if (!risposte.length || risposte.every(r => r.risposta.RispostaOK)) {
        this.showSuccessMessage("OperazioneRiuscita");
        this.datastore.resetGridData();
      }
      return RICARICA_LA_GRIGLIA; // because of resetGridData()
    }))
  }

  private tryDelete(rows: BrogliaccioRow[]): Observable<{ risposta: RispostaStandard, riga: BrogliaccioRow }[]> {
    const obs: Observable<any>[] = [];
    for (let row of rows)
      obs.push(this.attemptToRemoveBrogliaccio(row))
    return forkJoin(obs);
  }

  private attemptToRemoveBrogliaccio(row: BrogliaccioRow) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost(AttemptRemoval, this.removalParams_NG(row))
      .pipe(
        map(r => {
          return {risposta: r, riga: row};
        }));
  }

  public refresh() {
    this.gridPublicService.refresh(true);
  }

  private showRemovalDecisionDialog(risposteServer?: {
    risposta: RispostaStandard,
    riga: BrogliaccioRow
  }[]): Promise<any> {
    const title = this.transloco.translate("CancellaBrogliaccio");
    let body = this.transloco.translate("CancellaBrogliaccioBody")
    if (risposteServer.length > 1) {
      const descrizioni = risposteServer.filter(R => rowContainsConstraints(R.risposta))
        .map(R => R.riga.Descrizione_Unica)
        .reduce((a, b) => a + '<br>' + b)
      body = this.transloco.translate("CancellaBrogliaccioBodyMulti", [descrizioni])
    }

    return lastValueFrom(this.dialog.dialogMessageObs_Result(title, body, this.getRemovalDecisionBtns(), "auto",
      "auto", null, Dialog_Type.warning).pipe(take(1)));
  }

  private showErrorMessage = (translocoKey: string) => {
    this.dialog.baseError('Error', translocoKey);
  }

  private showSuccessMessage = (translocoKey: string) => {
    return this.dialog.baseSuccess('', translocoKey);
  }

  private getRemovalDecisionBtns(): Array<GiasDialogAction> {
    const removeAll = this.transloco.translate('RemoveAll')
    const partialRemoval = this.transloco.translate('RemoveInterventoKeepOreCosti')
    const cancel = this.transloco.translate('Cancel');
    return [
      {text: removeAll, primary: true, returnObj: RemovalDecision.REMOVE_ALL},
      {
        text: partialRemoval, primary: true,
        returnObj: RemovalDecision.REMOVE_INTERVENTO_KEEP_ORE_COSTI
      },
      {text: cancel, returnObj: RemovalDecision.CANCEL}
    ]
  }

  private removeMultiOperationWithAllConstraints(rows: BrogliaccioRow[]): any {
    const obs: Observable<any>[] = [];
    for (let row of rows)
      obs.push(this.httpCall(RemovalWithAllConstraints, this.removalParams(row))
        .pipe(map(r => {
          return {risposta: r, riga: row};
        })));
    return forkJoin(obs);
  }

  private removeMultiOperationPreservingOreCosti(rows: BrogliaccioRow[]): any {
    const obs: Observable<any>[] = [];
    for (let row of rows)
      obs.push(this.httpCall(PartialRemoval, this.removalParams(row))
        .pipe(map(r => {
          return {risposta: r, riga: row};
        })));
    return forkJoin(obs);
  }

  /** Se ha un raccoglitore_cod il brogliaccio che devo eliminare allora elimino anche
   * gli altri brogliacci con lo stesso raccoglitore_cod
   */
  private removalParams_NG(row: BrogliaccioRow): Elimina_Ricetta_Brogliaccio {
    let elimina_ricetta_brogliaccio = new Elimina_Ricetta_Brogliaccio();

    let siblings = [];
    if (row['Raccoglitore_Cod'] !== '0') {
      siblings = this.datastore.gridDataBrogliaccio.rows.filter(r => r['Raccoglitore_Cod'] === row['Raccoglitore_Cod'])
    }
    if (!siblings.length) siblings = [row];

    const recipes: Ricetta_Operazione[] = siblings.map(r => {
      let recipe = new Ricetta_Operazione();
      recipe.ricetta_cod = r.Ricetta_Cod;
      recipe.ricetta_operazione_cod = r.Ricetta_Operazione_Cod;
      recipe.in_uso = +row.in_uso;
      recipe.app_ricetta_operazione_id = row.APP_Ricetta_Operazione_ID;
      return recipe;
    })

    elimina_ricetta_brogliaccio.ricette = recipes;
    elimina_ricetta_brogliaccio.variabiliInSessione_NG = this.masterService.variabiliInSessione;
    return elimina_ricetta_brogliaccio;
  }

  /** Se ha un raccoglitore_cod il brogliaccio che devo eliminare allora elimino anche
   * gli altri brogliacci con lo stesso raccoglitore_cod
   */
  private removalParams(row: BrogliaccioRow): Elimina_Ricetta_Brogliaccio {
    let elimina_ricetta_brogliaccio = new Elimina_Ricetta_Brogliaccio();
    let siblings = [];

    if (row['Raccoglitore_Cod'] !== '0') {
      siblings = this.datastore.gridDataBrogliaccio.rows.filter(r => r['Raccoglitore_Cod'] === row['Raccoglitore_Cod'])
    }
    if (!siblings.length) siblings = [row];

    const recipes: Ricetta_Operazione[] = siblings.map(r => {
      let recipe = new Ricetta_Operazione();
      recipe.ricetta_cod = r.Ricetta_Cod;
      recipe.ricetta_operazione_cod = r.Ricetta_Operazione_Cod;
      recipe.in_uso = +row.in_uso;
      recipe.app_ricetta_operazione_id = row.APP_Ricetta_Operazione_ID;
      return recipe;
    })

    elimina_ricetta_brogliaccio.variabiliInSessione_NG = this.masterService.variabiliInSessione;
    elimina_ricetta_brogliaccio.ricette = recipes;
    return elimina_ricetta_brogliaccio;
  }


  private httpCall(link: string, params) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost(link, params).pipe(take(1))
  }

  public ReindirizzaACodificaProdottiAPP() {
    let Parametri: Array<ParametriAggiuntivi_QueryString> = [{
      key: "win",
      value: "1",
      codifica: false
    }];

    return this.gestioneRichiesteService.gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaSincronizzatore,
      enum_PagineAgronicaSincro.Codifica_ProdottiAPP,
      Parametri,
      this.ObjParametriAgenda,
      false
    ).then(link => {
      this.showDialogCodificaProdotti(link);
    });
  }

  private showDialogCodificaProdotti(link: string) {
    this.window = this.windowService.open({
      title: this.transloco.translate("CodificaProdottiAPP"),
      content: link,
      height: window.innerHeight * 0.9,
      width: window.innerWidth * 0.9
    });
  }

  private handleCustomizations() {
    this.resizable.autoFitColumns = true;
    this.selectable.selectable.enabled = !this.datastore.isMenuAgendaCalledFromGis;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.selectable.drag = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.columnSettings.showSelectAll = true;
    this.selectable.columnSettings.width = 35;
    this.generalSettings.height = "100%";
    this.behavior.excelSettings.enabled = !this.datastore.isMenuAgendaCalledFromGis;
    this.setDdlCommandMenu();
  }

  private setDdlCommandMenu() {
    this.cmdColumn = new CommandsColumnSettings({editBtn: false, removeBtn: false});
    this.dettagliColumn = new DettagliColumnSettings({
      editBtn: this.ShowEditBtn(),
      edit: (data) => this.handle_Info_Edit_Ribaltamento_Buttons('edit', data)
    })
    if (this.datastore.isMenuAgendaCalledFromGis) return;

    this.cmdDropDown = new CommandsDropDownSettings({
      infoBtn: true,
      fullEditBtn: this.permessoQdC_W,
      removeBtn: this.permessoQdC_W
    });
    this.cmdDropDown.addCommand(new GridCommandItem(
      "Copia",
      enum_menuAgendaGridCommands.COPIA,
      'faCopySingle02'
    ));
    if (this.permessoQdC_W) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        "QdC",
        enum_menuAgendaGridCommands.QUADERNO_DI_CAMPAGNA,
        'faQdCRowGotoQdC',
      ));
    }
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
  }

  private handle_Info_Edit_Ribaltamento_Buttons(action: string, dataItem: any, tipo_ribaltamento: RibaltamentoTypes = RibaltamentoTypes.Nessuno) {
    if (action === 'edit' && !this.ShowEditBtn())
      return;

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

    let ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
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

    if (tipo_ribaltamento === RibaltamentoTypes.Da_Brogliaccio_ad_Agenda) {
      ObjParametriAgenda.TipoRicetta = 0;
      ObjParametriAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
      ObjParametriAgenda.Stato = 0;
    } else if (tipo_ribaltamento === RibaltamentoTypes.Nessuno) {
      ObjParametriAgenda.TipoRicetta = dataItem.Tipo_Ricetta;
      ObjParametriAgenda.TipoOperazioneAgenda = Tipo_Attivita.Ricetta;
      ObjParametriAgenda.Stato = dataItem.WAnagraficaStati_Cod;
    }

    ObjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;
    this.redirector.gestisciRedirectToQdC(ObjParametriAgenda, tipo_ribaltamento).then();
  }

  ShowEditBtn(): boolean {
    return !this.datastore.isMenuAgendaCalledFromGis
      && this.permessoBrogliaccio_W
      && !this.SUPERUSER_IMPEDISCI_INS_MOD_BROGLIACCIO;
  }

}


const enum RemovalDecision {
  REMOVE_ALL,
  REMOVE_INTERVENTO_KEEP_ORE_COSTI,
  CANCEL
}

function rowContainsConstraints(risposta: RispostaStandard) {
  return risposta.RispostaStringa !== ""
}

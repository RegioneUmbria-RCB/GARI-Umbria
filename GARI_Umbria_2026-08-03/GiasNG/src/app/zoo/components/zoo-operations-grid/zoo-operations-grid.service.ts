import { Injectable, Injector } from "@angular/core";
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  CommandsDropDownEvents,
  CommandsDropDownSettings,
  EditingMode,
  GridCommandItem,
  GridPublicService,
  HttpAction,
  KendoGridColumn,
  KendoGridRow,
  KendoServerResultImpl,
  LoaderType,
  ToolbarSettings
} from 'gias-kendo-grid';
import { catchError, filter, map, Observable, of, switchMap, takeUntil, tap, throwError } from "rxjs";
import { MasterService } from "../../../Service/master.service";
import { ObjParametriAgendaService } from "../../../Service/obj-parametri-agenda.service";
import { Link_ElimOpMultipla } from "../../../menu-agenda/components/utils";
import { OPERATIONS_MODEL_4, OPERAZIONI_ZOO_ALIMENTAZIONE, ZooActivityForRedirect, ZooRedirectorService } from "../../services/zoo-redirector.service";
import { OperazioniAgendaInput, OperazioniZooClient } from "../../../Service/net-core6-api.service";
import { ZooOperationsFilters } from "../../models/zoo-operations-filters.model";
import { ZooOperationGridFlatItem, ZooOperationGridModel } from "../../models/zoo-operation-grid-item.model";
import { cloneDeep } from "lodash";
import { elimina_operazione_multipla } from "app/menu-agenda/components/grid-qdc/qdc-config.service";
import { AjaxAgronicaAPIService } from "../../../Service/ajax-agronica.api.service";
import { GiasDialogService } from "../../../Service/gias-dialog.service";
import { enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { RowClassArgs } from "@progress/kendo-angular-grid";
import { faFileCircleExclamation, faFileCirclePlus } from "@fortawesome/free-solid-svg-icons";
import { CookieService } from "ngx-cookie-service";
import { ConversionService } from "gias-ui-kit";

enum ZooGridCommands {
  GENERETE_MODEL_4,
  REGISTER_MODEL
}

@Injectable()
export class ZooOperationsGridConfigService extends AbstractGridConfigService<KendoServerResultImpl> {
  editingMode = EditingMode.IN_PAGE;
  loader = LoaderType.SERVICE;
  rowId = "Id_Agenda";
  gridId = "zooOpGrid";

  private canEdit = true;
  private canEditModel4 = true;

  private faGenerateModel = faFileCirclePlus;
  private faRegisterModel = faFileCircleExclamation;
  private _filters: ZooOperationsFilters | null = null;
  private _gridRows: ZooOperationGridFlatItem[] = [];
  private readonly _gridModel = ZooOperationGridModel;
  private _gridColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'Id_Agenda', title: 'ID' }, { editable: false, width: 80 }),
    new KendoGridColumn({ field: 'Data', title: this.transloco.translate('Data') }, { editable: false, width: 120 }),
    new KendoGridColumn({ field: 'lav_des', title: this.transloco.translate('Operazione') }, { editable: false, width: 160 }),
    new KendoGridColumn({ field: 'Lav_Cod', title: this.transloco.translate('Codice') }, {
      hidden: true,
      editable: false
    }),
    new KendoGridColumn({ field: 'sa_nome', title: this.transloco.translate('CentroAziendale') }, { editable: false, width: 160 }),
    new KendoGridColumn({ field: 'STA_DES', title: this.transloco.translate('Stalla') }, { editable: false, width: 160 }),
    // new KendoGridColumn({field: 'stables', title: this.transloco.translate('Stalla')}, {editable: false}),
    // new KendoGridColumn({field: 'groups', title: this.transloco.translate('Gruppi')}, {editable: false}),
    new KendoGridColumn({ field: 'Dettaglio_Tecnico', title: this.transloco.translate('DettaglioTecnico') }, { editable: false, width: 800 }),
    new KendoGridColumn({ field: 'Prodotti', title: this.transloco.translate('Prodotti') }, { editable: false, width: 160 }),
    new KendoGridColumn({ field: 'Segnalazioni', title: this.transloco.translate('Segnalazioni') }, { editable: false, width: 160 }),
    new KendoGridColumn({
      field: 'Creatore_Intervento',
      title: this.transloco.translate('UtenteCreazione')
    }, { editable: false, width: 120 })
  ];

  constructor(
    injector: Injector,
    public gridpublicService: GridPublicService,
    private master: MasterService,
    private dialog: GiasDialogService,
    private agenda: ObjParametriAgendaService,
    private zooRedirector: ZooRedirectorService,
    private zooClient: OperazioniZooClient,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
    private permissions: PermessiUtenteService,
    private cookies: CookieService,
    private conversionService: ConversionService
  ) {
    super(injector);
    this.canEdit = this.permissions.canWritePermesso(enum_Security_Attivita.MenuZooNG);
    this.canEditModel4 = this.permissions.canWritePermesso(enum_Security_Attivita.ManutenzioneArchivi_ImportaAnagraficheAnimali_XLS2GIAS);
    this.handleCustomizations();
  }

  private get currentPiva(): string {
    const agenda = this.agenda.getObjParamValue();
    return agenda.Piva ?? '';
  }

  read(options?: ZooOperationsFilters): Observable<KendoServerResultImpl> {
    if (this.reuseLoadedData(options) || this.cookies.get('zooTab') != "0") {
      return of(new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows));
    }
    this._filters = cloneDeep(options);
    this.master.set_isLoading({ isLoading: true });
    return this.zooClient.operazioniZooGetOperazioniAgenda(this.getLoaderParams()).pipe(
      map(r => r.RispostaOK ? JSON.parse(r.RispostaStringa) : []),
      map(r => this.conversionService.ConversionDateInObject(r)),
      map((items: ZooOperationGridFlatItem[]) => {
        const opFiltered = this._filters?.operations.map(x => +x) ?? [];
        if (opFiltered.length > 0) return items.filter(x => opFiltered.includes(x.Lav_Cod));
        else return items;
      }),
      map(items => items.sort((a, b) => new Date(a.Data) > new Date(b.Data) ? -1 : 1)),
      tap((items: ZooOperationGridFlatItem[]) => this._gridRows = items),
      tap(() => this.master.set_isLoading({ isLoading: false })),
      map(() => new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows)),
      catchError(() => {
        this.master.set_isLoading({ isLoading: false });
        return of(new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows));
      })
    );
  }

  perform(actionType: HttpAction, items: any, oldRow?: any) {
    if (actionType === HttpAction.REMOVE) {
      return this.deleteItem(items, false).pipe(map(() => []));
    }
    return of(null);
  }

  onRowClass = (event: RowClassArgs) => this.coloraRigheOperazioni(event.dataItem);

  coloraRigheOperazioni(row: KendoGridRow) {
    const result: { [k: string]: boolean } = {};
    result.agendaOperazPianificata = row['contabilizzato'] < 0;
    result.agendaOperazBloccata = row['Blocco_Flag'] === 1;
    result.agendaOperazDaRevisionare = row['Blocco_Flag'] === 2;
    return result;
  }

  private getLoaderParams(): OperazioniAgendaInput {
    let param = {
      Piva: this.currentPiva,
      Sa_Cod: this._filters?.center ? +this._filters.center : 0,
      Sta_Num: this._filters?.stable ? +this._filters.stable : 0,
      ValiditaInizio: this._filters?.from,
      ValiditaFine: this._filters?.to
    } as OperazioniAgendaInput;
    if (isNaN(param.Sta_Num)) param.Sta_Num = 0;
    return param;
  }

  private handleCustomizations(): void {
    this.selectable.selectable.enabled = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.showSelectAll = true;
    this.columnMenu.kendoGridColumnChooser = true;
    this.views.enabled = true;
    this.resizable.autoFitColumns = true;
    this.groups.groupable.enabled = false;
    this.resizable.isResizable = true;
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.gridIsEditable = true;
    this.setupCommands();
  }

  private setupCommands() {
    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });
    this.cmdDropDown = new CommandsDropDownSettings({ fullEditBtn: true, infoBtn: true });
    this.cmdDropDown.addCommand(new GridCommandItem(
      'GeneraModello4', ZooGridCommands.GENERETE_MODEL_4, '', this.faGenerateModel
    ));
    this.cmdDropDown.width = 30;
    // Pulsante "Registra Uscita" aggiunto dinamicamente nella funzione setButtonVisibility
    this.handleCommandEvent();
    this.setButtonsVisibility();
  }

  private handleCommandEvent() {
    this.gridPublicService.commandEvent.pipe(
      takeUntil(this.signal),
      filter(cmdEvent => !!cmdEvent)
    ).subscribe(cmd => {
      console.debug(cmd);
      switch (cmd.command.action) {
        case CommandsDropDownEvents.INFO:
          this.zooRedirector.redirectOpenOperationInfo(cmd.dataItem, this.gridPublicService.gridElRef);
          break;
        case CommandsDropDownEvents.FULL_EDIT:
          this.zooRedirector.redirectOpenOperationEdit(cmd.dataItem, this.gridPublicService.gridElRef);
          break;
        case ZooGridCommands.GENERETE_MODEL_4:
          this.zooRedirector.redirectToModel4Generation(cmd.dataItem);
          break;
        case ZooGridCommands.REGISTER_MODEL:
          this.zooRedirector.redirectToModel4Registration(cmd.dataItem);
          break;
      }
    });
  }

  private setButtonsVisibility() {
    this.gridPublicService.openCommands.pipe(
      takeUntil(this.signal),
      filter(activity => !!activity)
    ).subscribe(activity => {
      //Se l'operazione è bloccata nascondo modifica, genera modello e mostro registrazione
      if (!this.canEdit || activity.Blocco_Flag === 1) {
        this.cmdDropDown.removeCommand(CommandsDropDownEvents.FULL_EDIT);
        this.cmdDropDown.removeCommand(ZooGridCommands.GENERETE_MODEL_4);
        this.cmdDropDown.addCommand(new GridCommandItem(
          'RegistraUscita', ZooGridCommands.REGISTER_MODEL, '', this.faRegisterModel, false
        ));
      } else if (OPERAZIONI_ZOO_ALIMENTAZIONE.includes(activity.Lav_Cod)) {
        this.cmdDropDown.removeCommand(CommandsDropDownEvents.FULL_EDIT);
      }
      // Se il modello è già stato registrato, nascondo registra
      if (activity.Tipo_Accettazione === 1) {
        this.cmdDropDown.removeCommand(ZooGridCommands.REGISTER_MODEL);
      }
      

      if (!OPERATIONS_MODEL_4.includes(activity.Lav_Cod) || !this.canEditModel4) {
        this.cmdDropDown.removeCommand(ZooGridCommands.GENERETE_MODEL_4);
        this.cmdDropDown.removeCommand(ZooGridCommands.REGISTER_MODEL);
      }
    });
  }

  private findRowWithId(id: number): ZooActivityForRedirect {
    return this._gridRows.find(r => r.Id_Agenda === id);
  }

  /**
   * @param options the filters passed to the {@link read} function
   * @private
   * @returns `true` if the filters are not set and there are no specified options or,
   * if the new reading options are the same as the filters. `false` otherwise.
   */
  private reuseLoadedData(options: ZooOperationsFilters): Boolean {
    return (!this._filters && !options) || (!options && !!this._filters) || !options.forceReload && (
      !!this._filters && !!options
      && options.center === this._filters.center
      && options.stable === this._filters.stable
      && options.from === this._filters.from
      && options.to === this._filters.to
      && options.operations.reduce((x1, x2) => x1 + "|" + x2) === this._filters.operations.reduce((x1, x2) => x1 + "|" + x2)
    );
  }

  private getDeletionParams(
    rows: ZooOperationGridFlatItem[],
    proseguiInCasoDiAlert: boolean
  ): elimina_operazione_multipla {
    const chiavi = this.getCompositeKeys(rows);
    return {
      strChiaviComposite: "del_elem|" + chiavi,
      proseguiInCasoDiAlert: proseguiInCasoDiAlert,
      variabiliInSessione_NG: this.master.variabiliInSessione
    };
  }

  private getCompositeKeys(rows: ZooOperationGridFlatItem[]) {
    return rows.map(r => {
      const opDate = new Date(r.Data);
      const dateStr = opDate.getDate() + '/' + (opDate.getMonth() + 1).toString().padStart(2, '0') + '/'
        + opDate.getFullYear();
      return dateStr + "_" + r.Id_Agenda + "_" + r.Lav_Cod + "_" + r.Piva + "_" + r.Sa_Cod + "_" + r.Blocco_Flag
        + "_" + r.Veg_Cod + "_" + r.Piva;
    }).join(",");
  }

  private asRowArray(rowsIn: ZooOperationGridFlatItem[] | ZooOperationGridFlatItem) {
    if (!rowsIn['length']) {
      const rows: ZooOperationGridFlatItem[] = [];
      rows.push(rowsIn as ZooOperationGridFlatItem);
      return rows;
    } else {
      return rowsIn as ZooOperationGridFlatItem[];
    }
  }

  /**
   * @param rowsIn rows to delete
   * @param proseguiInCasoDiAlert Richiede all'utente la conferma prima di cancellare operazioni che hanno altri vincoli.
   * @returns {Observable<boolean>} true if the delete was successful and a refresh is needed, false otherwise
   */
  private deleteItem(rowsIn: ZooOperationGridFlatItem[] | ZooOperationGridFlatItem, proseguiInCasoDiAlert: boolean): Observable<boolean> {
    let rows: ZooOperationGridFlatItem[] = this.asRowArray(rowsIn);
    let params = this.getDeletionParams(rows, proseguiInCasoDiAlert);
    if (rows.some(r => r.Blocco_Flag === 1)) {
      this.dialog.baseError('Errore_', 'LockedActivityError');
      return throwError(() => new Error());
    }
    return this.ajaxAgronicaAPIService.ajaxAPIPost<elimina_operazione_multipla, string>(Link_ElimOpMultipla, params, true, false, false, false)
      .pipe(
        switchMap((risposta) => {
          if (risposta.RispostaOK) {
            this.dialog.baseSuccess('', risposta.RispostaStringa, false);
            return of({ refresh: true });
          } else if (risposta.RispostaConferma) {
            return this.dialog.warningObs('', risposta.Errore, false);
          } else {
            const errMsg = risposta.Errore ? risposta.Errore : this.transloco.translate("ImpossibileEliminareOperazione");
            this.dialog.baseError('', errMsg, false);
            return of({ refresh: false });
          }
        }),
        switchMap((result: boolean | { refresh: boolean }) => {
          if (result === true) return this.deleteItem(rows, true);
          else if (result === false) return of(false);
          else return of((result as { refresh: boolean }).refresh);
        })
      );
  }

}

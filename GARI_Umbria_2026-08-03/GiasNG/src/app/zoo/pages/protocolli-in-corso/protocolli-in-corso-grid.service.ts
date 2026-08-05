import { Injectable, Injector } from "@angular/core";
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  CommandsDropDownSettings,
  CustomColumnSettings,
  DropdownListItem,
  DropdownListWithForm,
  EditingMode,
  GridCommandItem,
  HttpAction,
  KendoGridColumn,
  KendoServerResultImpl,
  LoaderType,
  NumericSettings,
  ToolbarSettings
} from "gias-kendo-grid";
import { catchError, filter, finalize, forkJoin, from, map, mergeMap, Observable, of, switchMap, take, takeUntil, tap } from "rxjs";
import { cloneDeep } from "lodash";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { BaseCodeDescr } from "app/Service/api.service";
import { ZooPrescriptionGridFlatItem, ZooPrescriptionGridModel } from "../../models/zoo-prescription-grid-item.model";
import { LeggiPrescrizioni } from "../../models/leggi-prescrizioni.model";
import { ZooOperationsFilters, ZooPrescriptionsFilters } from "../../models/zoo-operations-filters.model";
import { TUTTI_CENTRI_AZIENDALI } from "app/Model/CostantiPersonalizzate";
import { DeleteSomministrazione, OperazioniZooClient, PrescrizioniClient } from "app/Service/net-core6-api.service";
import { ZooFiltersHelperService } from "../../services/zoo-filters-helper.service"
import { enum_PagineGiasNG, enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { enum_menuZooGridCommands } from "app/zoo/zoo.utils";
import { ConversionService, Enum_DBTypeOperation, GiasDialogService, ObjParametriAgenda } from "gias-ui-kit";
import { CookieService } from "ngx-cookie-service";
import { IntervalloTemporale } from "app/Model/anagrafiche/IntervalloTemporale";
import { enum_TypeTab_Zootecnia } from "app/zoo/models/tipi-enumerativi-zoo";

@Injectable()
export abstract class ZooPrescriptionsInProgressGridConfigService extends AbstractGridConfigService<KendoServerResultImpl> {
  editingMode = EditingMode.IN_LINE;
  gridId = "zooProtocolsInProgressGrid";
  loader = LoaderType.SERVICE;
  rowId = "IdRicetta";

  protected canEdit = true;

  protected _postReadProcessing = (rows) => rows;
  protected _filters: ZooPrescriptionsFilters | null = null;
  protected _gridRows: ZooPrescriptionGridFlatItem[] = [];
  protected _gridColumns = [
    new KendoGridColumn({ field: 'IdRicetta', title: this.transloco.translate('ID') }, { editable: false }),
    new KendoGridColumn({ field: 'DataInizioTrattamento', title: this.transloco.translate('Data') }, { editable: false }),
    new KendoGridColumn({ field: 'Numero', title: this.transloco.translate('Numero') }, { editable: false }),
    new KendoGridColumn({ field: 'Denominazione', title: this.transloco.translate('Denominazione') }, { editable: false }),
    new KendoGridColumn({ field: 'Capi', title: this.transloco.translate('Capi') }, { editable: false }),
    new KendoGridColumn({ field: 'Note', title: this.transloco.translate('Note') }, { editable: false }),
    new KendoGridColumn({ field: 'rag_soc', title: this.transloco.translate('RagioneSociale') }, { editable: false }),
    new KendoGridColumn({ field: 'sa_nome', title: this.transloco.translate('CentroAziendale') }, { editable: false }),
    new KendoGridColumn({ field: 'STA_DES', title: this.transloco.translate('Stalla') }, { editable: false }),
    new KendoGridColumn({ field: 'Posologia', title: this.transloco.translate('Posologia') }, { editable: false }),
    new KendoGridColumn({ field: 'Note_Agenda', title: this.transloco.translate('NSomministrazione') }, { editable: false }),
    new KendoGridColumn({ field: 'Username_Creazione', title: this.transloco.translate('UtenteCreazione') }, { editable: false }),
    new KendoGridColumn({ field: 'Username_Modifica', title: this.transloco.translate('UtenteModifica') }, { editable: false, width: 150 }),
    // new KendoGridColumn({ field: 'Numero_Somm', title: this.transloco.translate('NSomministrazioni') }, { editable: true, format: '{0:n0}', numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n0', step: 1, decimals: 0 }) }),
    // new KendoGridColumn({ field: 'Intervallo_Somm', title: this.transloco.translate('IntSomministrazioni') }, { editable: true, format: '{0:n4}', numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n4', step: 0.0001, decimals: 4 }) }),
    new KendoGridColumn({ field: 'Qta_Dose', title: this.transloco.translate('DoseProdotto.text') }, { editable: true, format: '{0:n4}', numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n4', step: 0.0001, decimals: 4 }) }),
    new KendoGridColumn({ field: 'Udm_Dose', title: this.transloco.translate('Unita_Misura') }, { editable: true })
  ];
  private _gridModel = ZooPrescriptionGridModel;
  private _centers: BaseCodeDescr[];
  private _stables: BaseCodeDescr[];
  private lastFilterUsed: ZooPrescriptionsFilters;

  constructor(
    injector: Injector,
    protected agenda: ObjParametriAgendaService,
    protected prescriptions: PrescrizioniClient,
    protected helper: ZooFiltersHelperService,
    private permissions: PermessiUtenteService,
    private gestioneRichieste: GestioneRichiesteService,
    private zooClient: OperazioniZooClient,
    private dialog: GiasDialogService,
    private cookies: CookieService,
    private conversionService: ConversionService
  ) {
    super(injector);
    this.handleCustomizations();
    this.setupCommands();
    this.handleCommandEvent();
  }

  private get currentPiva(): string {
    const agenda = this.agenda.getObjParamValue();
    return agenda.Piva ?? '';
  }

  read(options?: ZooPrescriptionsFilters): Observable<KendoServerResultImpl> {

    if (this.reuseLoadedData(options) || this.cookies.get('zooTab') != "2") {
      return of(new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows));
    }
    this._filters = cloneDeep(options);
    this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
    // this.gridPublicService
    return forkJoin([
      this.loadAdditionalData(),
      this.prescriptions.prescrizioniLeggiPrescrizioni(this.getLoadParams())
    ]).pipe(
      map(results => results[1]),
      map(r => r.RispostaOK ? JSON.parse(r.RispostaStringa) : []),
      map(r => this.conversionService.ConversionDateInObject(r)),
      tap(rows => this._gridRows = rows),
      map(() => new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows)),
      tap(() => { console.debug(this._gridRows); }),
      tap(() => this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef })),
      catchError(() => {
        this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
        return of(new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows));
      })
    );
  }

  perform(actionType: HttpAction, items: any, oldRow: any): Observable<any> {
    switch (actionType) {
      case HttpAction.REMOVE:
        return this.deleteItem(items);
      default:
        return of();
    }
  }

  private deleteItem(items: any): Observable<any> {

      const arrParams = this.getParamsForDelete(items);

      this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

      return forkJoin(
        arrParams.map(item =>
          this.zooClient.operazioniZooEliminaSomministrazione(item).pipe(
            catchError(error => {
              return of({ RispostaOK: false, Errore: this.transloco.translate("zoo.ErroreDuranteEliminazioneProtocolloInCorso", [item.Id_Ricetta]) });
            })
          )
        )
      ).pipe(
        tap(() => this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef })),
        map(responses => {
          const successCount = responses.filter(r => r.RispostaOK).length;
          const errorMessages = responses
            .filter(r => !r.RispostaOK)
            .map(r => r.Errore)
            .join('\n');
    
          return { successCount, errorMessages };
        }),
        tap(({ successCount, errorMessages }) => {
          if (successCount > 0) {
            this.dialog.baseSuccess(
              '',
              this.transloco.translate('EliminazioneEffettuataCorrettamente', { count: successCount }),
              false
            );
          }
          if (errorMessages) {
            // this.dialog.baseError('', errorMessages, false);
            console.log(errorMessages);
          }
        }),
        switchMap(() => {
          // const options = new ZooPrescriptionsFilters(0, 0, AGRODATAINIZIO, AGRODATAFINE, []);
          // options.prescriptionType = enum_TipoPrescrizione.Da_Protocollo_GIAS;
          // options.forceReload = true;
          // return this.read(options);
          //this.lastFilterUsed.forceReload = true;
          return this.read(this.lastFilterUsed);
        })
      );
  }

  private getParamsForDelete(items: any): DeleteSomministrazione[] {
    const arrParams: DeleteSomministrazione[] = [];

    items.forEach((item: any) => {
      arrParams.push({ 
        Id_Ricetta: item.IdRicetta,
        Id_Riga_Ricetta: item.IdAgenda,
        Id_Agenda: 0,
        /** Indica se si tratta di una Somministrazione futura o confermata */
        Programmata: true
      });
    });

    return arrParams;
  }

  protected handleCustomizations(): void {

    //leggiamo il permesso di scrittura per poter aggiungere il pulsante di cancellazione
    this.canEdit = this.permissions.canWritePermesso(enum_Security_Attivita.MenuZooNG);

    this.selectable.selectable.enabled = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.showSelectAll = true;

    this.gridIsEditable = false;
    this.columnMenu.kendoGridColumnChooser = true;
    this.views.enabled = true;
    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });
    this.groups.groupable.enabled = false;
    this.customColumn = new CustomColumnSettings({
      title: '',
      showColumn: true,
      useCustomColumnCellTemplate: true
    });
    this.setUpDdlCols();
    this.setCommandsVisibility();
  }

  private setUpDdlCols() {
    const col = this._gridColumns.find(s => s.field === 'Udm_Dose');
    let data: DropdownListItem[] = [];
    col.ddl = new DropdownListWithForm('id', 'Udm_Dose', 'name', data);
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'Udm_Sim';
    //col.ddl.loadFunction = (dataItem: any) => {return of([]);};
  }

  private setupCommands() {
    // this.cmdDropDown = new CommandsDropDownSettings({ fullEditBtn: false, infoBtn: false, removeBtn: true });
    // this.cmdDropDown.addCommand(new GridCommandItem(
    //   'EseguiTrattamento', 
    //   enum_menuZooGridCommands.RegisterOperationToAgenda, 
    //   'faQdCRowGotoQdC'
    // ));
    // Setup dynamic commands on ddl menu's open
    this.gridPublicService.openCommands
      .pipe(takeUntil(this.signal), filter(activity => !!activity))
      .subscribe(activity => {
        /* TODO */
      });
  }

  private setCommandsVisibility() {
  
      this.gridPublicService.openCommands.GiasSubscribe(protocollo => {

      })
    }

  private handleCommandEvent() {
    this.gridPublicService.commandEvent.pipe(
      takeUntil(this.signal),
      filter(cmdEvent => !!cmdEvent)
    ).subscribe(cmd => {
      switch (cmd.command.action) {
        case enum_menuZooGridCommands.RegisterOperationToAgenda:
          this.handleRegisterOperation(cmd.dataItem);
          // this.createOperationFromProtocol(cmd.dataItem);
      }
    });
  }

  public handleRegisterOperation(item: any) {
    this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

    this.prescriptions.prescrizioniCheckPreviousSomministration(item.IdRicetta, item.IdAgenda)
      .pipe(
        finalize(() => this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef })),
        map(r => r.RispostaOK ? JSON.parse(r.RispostaStringa) : false),
        switchMap((hasPreviousUnconfirmed: boolean) => {
          if (hasPreviousUnconfirmed) {
            return this.dialog.dialogMessageObs_Result(
              this.transloco.translate('Attenzione'),
              this.transloco.translate('zoo.CiSonoSomministrazioniPrecedentiNonConfermate')
            ).pipe(
              map(result => !!result['returnObj'])
            );
          }
          return of(true);
        }),
        filter(shouldProceed => shouldProceed === true)
      )
      .subscribe(() => {
        this.createOperationFromProtocol(item);
      });
  }

  private createOperationFromProtocol(protocol: ZooPrescriptionGridFlatItem) {
    forkJoin([
      from(this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Trattamento_Zoo)),
      this.prescriptions.prescrizioniGetAttivitaFromPrescrizione(protocol.IdRicetta)
        .pipe(map(r => r.RispostaOK ? r.RispostaStringa as any : null))
    ]).subscribe(([redirectUrl, attivita]) => {
      const objP = this.agenda.getObjParamValue() as ObjParametriAgenda;
      objP.TipoOperazioneDB = Enum_DBTypeOperation.Write;
      objP.GenericObj_string = JSON.stringify(attivita);
      let queryParams: { [key:string] : string | string[] } = {"t_Tab":enum_TypeTab_Zootecnia.FuturePrescriptions.toString()};
      this.agenda.navigateTo(redirectUrl, queryParams, objP, false);
    });
  }

  private loadAdditionalData(): Observable<boolean> {
    if (!!this._centers && !!this._stables) return of(true);

    return forkJoin([
      this.helper.loadBusinessCenters(this.currentPiva).pipe(
        tap(c => this._centers = c),
      ),
      this.helper.loadStables(this.currentPiva, TUTTI_CENTRI_AZIENDALI).pipe(
        tap(s => this._stables = s),
      )
    ]).pipe(take(1), map(() => true));
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

  private getLoadParams() {
    return new LeggiPrescrizioni(
      this.currentPiva,
      this._filters.center,
      this._filters.stable,
      new IntervalloTemporale(
        this._filters.from,
        this._filters.to
      ),
      null,
      enum_TypeTab_Zootecnia.FuturePrescriptions
    );
  }

}

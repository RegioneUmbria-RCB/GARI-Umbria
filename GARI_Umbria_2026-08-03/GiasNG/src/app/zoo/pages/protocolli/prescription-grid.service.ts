import { Injectable, Injector } from "@angular/core";
import {
  AbstractGridConfigService,
  BooleanSettings,
  CommandsColumnSettings,
  CustomColumnSettings,
  DropdownListItem,
  DropdownListWithForm,
  EditingMode,
  HttpAction,
  KendoGridColumn,
  KendoServerResultImpl,
  LoaderType,
  NumericSettings,
  ToolbarSettings
} from "gias-kendo-grid";
import { catchError, filter, forkJoin, from, map, Observable, of, switchMap, take, takeUntil, tap } from "rxjs";
import { cloneDeep } from "lodash";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { BaseCodeDescr } from "app/Service/api.service";
import { ZooPrescriptionGridFlatItem, ZooPrescriptionGridModel } from "../../models/zoo-prescription-grid-item.model";
import { LeggiPrescrizioni } from "../../models/leggi-prescrizioni.model";
import { ZooOperationsFilters, ZooPrescriptionsFilters } from "../../models/zoo-operations-filters.model";
import { TUTTI_CENTRI_AZIENDALI } from "app/Model/CostantiPersonalizzate";
import { MetaschemaClient, PrescrizioniClient, UpdatePrescrizione } from "app/Service/net-core6-api.service";
import { ZooFiltersHelperService } from "../../services/zoo-filters-helper.service";
import { enum_PagineGiasNG, enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { enum_menuZooGridCommands } from "app/zoo/zoo.utils";
import { enum_TypeTab_Zootecnia } from "app/zoo/models/tipi-enumerativi-zoo";
import { ConversionService, Enum_DBTypeOperation, ObjParametriAgenda } from "gias-ui-kit";
import { CookieService } from "ngx-cookie-service";
import { Validators } from "@angular/forms";
import { GiasMessageService } from "app/Service/gias-message.service";
import { IntervalloTemporale } from "app/Model/anagrafiche/IntervalloTemporale";

@Injectable()
export abstract class ZooPrescriptionsGridConfigService extends AbstractGridConfigService<KendoServerResultImpl> {
  editingMode = EditingMode.IN_LINE;
  gridId = "zooPrescriptionsGrid";
  loader = LoaderType.SERVICE;
  rowId = "IdRicetta";

  protected _postReadProcessing = (rows) => rows;
  protected _filters: ZooPrescriptionsFilters | null = null;
  protected _gridRows: ZooPrescriptionGridFlatItem[] = [];
  protected _gridColumns = [
    new KendoGridColumn({ field: 'IdRicetta', title: this.transloco.translate('ID') }, { editable: false, width: 150 }),
    // new KendoGridColumn({ field: 'IdAgenda', title: this.transloco.translate('IdAgenda') }, { editable: false, hidden: true }),
    // new KendoGridColumn({ field: 'IdDettaglio', title: this.transloco.translate('IdDettaglio') }, { editable: false, hidden: true }),
    // new KendoGridColumn({ field: 'IdMov', title: this.transloco.translate('IdMov') }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'DataEmissione', title: this.transloco.translate('DataEmissione') }, { editable: false, width: 150 }),
    new KendoGridColumn({ field: 'Numero', title: this.transloco.translate('Numero') }, { editable: false, width: 150 }),
    new KendoGridColumn({ field: 'Denominazione', title: this.transloco.translate('Denominazione') }, { editable: false, width: 250 }),
    new KendoGridColumn({ field: 'Note', title: this.transloco.translate('Note') }, { editable: false, width: 250 }),
    new KendoGridColumn({ field: 'rag_soc', title: this.transloco.translate('RagioneSociale') }, { editable: false, width: 250 }),
    new KendoGridColumn({ field: 'sa_nome', title: this.transloco.translate('CentroAziendale') }, { editable: false, width: 250 }),
    new KendoGridColumn({ field: 'STA_DES', title: this.transloco.translate('Stalla') }, { editable: false, width: 250 }),
    new KendoGridColumn({ field: 'Posologia', title: this.transloco.translate('Posologia') }, { editable: false, width: 200 }),
    new KendoGridColumn({ field: 'ProprietarioIdFiscale', title: this.transloco.translate('Proprietario') }, { editable: false, width: 150 }),
    new KendoGridColumn({ field: 'VeterinarioIdFiscale', title: this.transloco.translate('Veterinario') }, { editable: false, width: 150 }),
    new KendoGridColumn({ field: 'DetentoreIdFiscale', title: this.transloco.translate('Detentore') }, { editable: false, width: 150 }),
    new KendoGridColumn({ field: 'Username_Creazione', title: this.transloco.translate('UtenteCreazione') }, { editable: false, width: 150 }),
    new KendoGridColumn({ field: 'Username_Modifica', title: this.transloco.translate('UtenteModifica') }, { editable: false, width: 150 }),
    new KendoGridColumn({ field: 'Numero_Somm', title: this.transloco.translate('NSomministrazioni') }, { editable: true, width: 150, format: '{0:n0}', numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n0', step: 1, decimals: 0 }) }),
    new KendoGridColumn({ field: 'Intervallo_Somm', title: this.transloco.translate('IntSomministrazioni') }, { editable: true, width: 150, format: '{0:n0}', numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n0', step: 1, decimals: 0 }) }),
    new KendoGridColumn({ field: 'Qta_Dose', title: this.transloco.translate('DoseProdotto.text') }, { editable: true, width: 150, format: '{0:n2}', numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n2', step: 0.01, decimals: 2 }) }),
    new KendoGridColumn({ field: 'Udm_Dose', title: this.transloco.translate('Unita_Misura') }, { editable: true, width: 150, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Arrotondamento_Peso', title: this.transloco.translate('Arrotondamento_Peso') }, { editable: true, format: '{0:n0}', numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n0', step: 1, decimals: 0 }) }),
    new KendoGridColumn({ field: 'Massivo', title: this.transloco.translate('Massivo') }, {editable: true, hidden: false, boolean: new BooleanSettings({ defaultValue: false, leftLabel: this.transloco.translate('No'), rightLabel: this.transloco.translate('Sì') }) }),
  ];
  private _gridModel = ZooPrescriptionGridModel;
  private _centers: BaseCodeDescr[];
  private _stables: BaseCodeDescr[];
  private _udm: BaseCodeDescr[];
  private lastFilterUsed: ZooPrescriptionsFilters;

  constructor(
    injector: Injector,
    protected agenda: ObjParametriAgendaService,
    protected prescriptions: PrescrizioniClient,
    protected helper: ZooFiltersHelperService,
    private permissions: PermessiUtenteService,
    private gestioneRichieste: GestioneRichiesteService,
    private cookies: CookieService,
    private conversionService: ConversionService,
    private giasMessageService: GiasMessageService,
    private metaschemaClient: MetaschemaClient
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
    if (this.reuseLoadedData(options) || this.cookies.get('zooTab') != "1") {
      return of(new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows));
    }
    this._filters = cloneDeep(options);
    this.lastFilterUsed = cloneDeep(options);
    this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
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
    // console.debug("perform", actionType, items, oldRow);

    switch (actionType) {
      case HttpAction.UPDATE:

        return this.updateItems(items);

      case HttpAction.REMOVE:

        return this.deleteItem(items);

      default:
        return of();
    }

  }

  protected handleCustomizations(): void {
    this.gridIsEditable = this.permissions.canWritePermesso(enum_Security_Attivita.ZooProtocolliTerapeutici);
    this.columnMenu.kendoGridColumnChooser = true;
    this.views.enabled = true;
    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;
    this.groups.groupable.enabled = false;
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.cmdColumn = new CommandsColumnSettings({ editBtn: true, infoBtn: false, removeBtn: false });
    this.cmdColumn.width = 30;
    this.groups.groupable.enabled = false;

    this.selectable.selectable.enabled = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.showSelectAll = true;

    this.customColumn = new CustomColumnSettings({
          title: '',
          showColumn: true,
          useCustomColumnCellTemplate: true
        });
    this.setUpDdlCols();
  }

  private setUpDdlCols() {


        const col = this._gridColumns.find(s => s.field === 'Udm_Dose');
        if (col != undefined) {
          let data = [];
          col.ddl = new DropdownListWithForm('id', 'Udm_Dose', 'name', data);
          col.ddl.loadFunction = this.loadUdm.bind(this);
          col.ddl.descriptionField = 'Udm_Sim';
          col.ddl.loadOnEdit = true;
          col.ddl.defaultValue = { id: 0, name: ''};
        }

  }

  private updateItems(items: any): Observable<any> {

    const params = this.getParamsForUpdateOrDelete(items);

    return this.prescriptions.prescrizioniUpdatePrescription(params as UpdatePrescrizione)
        .pipe(
          filter(r => r.RispostaOK),
          map(() => {
            this.lastFilterUsed.forceReload = true;
            return this.lastFilterUsed;
          }),
          switchMap((options) => this.read(options)),
          map(() => [])
        );
  }

  private deleteItem(items: any): Observable<any> {

      const arrParams = this.getParamsForUpdateOrDelete(items) as UpdatePrescrizione[];

      this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

      return forkJoin(
        arrParams.map(item =>
          this.prescriptions.prescrizioniDeletePrescriptions(item).pipe(
            catchError(error => {
              return of({ RispostaOK: false, Errore: this.transloco.translate("zoo.ErroreDuranteEliminazioneProtocollo", [item.IdRicetta]) });
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
            this.giasMessageService.infoMessagge(this.transloco.translate('EliminazioneEffettuataCorrettamente', { count: successCount }));
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
          this.lastFilterUsed.forceReload = true;
          return this.read(this.lastFilterUsed);
        })
      );
  }

  private getParamsForUpdateOrDelete(items: any): UpdatePrescrizione[] | UpdatePrescrizione {

      let paramSolo: UpdatePrescrizione;
      let arrParams: UpdatePrescrizione[] = [];

      let udm_dose: number = 0;

      if (typeof items.Udm_Dose == 'number') {
        udm_dose = items.Udm_Dose;
      }

      if (Array.isArray(items)) {
          items.forEach(item => {
            arrParams.push({
                          Piva: item.Piva,
                          SaCod: item.Sa_Cod,
                          IdRicetta: item.IdRicetta,
                          IdAgenda: item.IdAgenda,
                          IdMov: item.IdMov,
                          IdDettaglio: item.IdDettaglio,
                          NumSomm: item.Numero_Somm,
                          IntSomm: item.Intervallo_Somm,
                          QtaDose: item.Qta_Dose,
                          UdmDose: udm_dose,
                          ArrotondamentoPeso: item.Arrotondamento_Peso,
                          Massivo: item.Massivo
                        } as UpdatePrescrizione); 
          });

          return arrParams;
      } else {
        paramSolo = {
          Piva: items.Piva,
          SaCod: items.Sa_Cod,
          IdRicetta: items.IdRicetta,
          IdAgenda: items.IdAgenda,
          IdMov: items.IdMov,
          IdDettaglio: items.IdDettaglio,
          NumSomm: items.Numero_Somm,
          IntSomm: items.Intervallo_Somm,
          QtaDose: items.Qta_Dose,
          UdmDose: udm_dose,
          ArrotondamentoPeso: items.Arrotondamento_Peso,
          Massivo: items.Massivo
        } as UpdatePrescrizione;

        return paramSolo;
      }
  }

  private loadUdm(): Observable<DropdownListItem[]>{
    if (this._udm ==  null || this._udm.length == 0){
      return this.metaschemaClient.metaschemaGetUnitaMisuraProtocolli().pipe(
        map((el) => {
          return el.RispostaStringa;
        }),
        tap((el) => {
          this._udm = el;
        }),
        map((el) => {
          return el.map((el) => { return new DropdownListItem(el.codice, el.descrizione) });
        })
      )
    } else {
      return of(this._udm.map((el) => { return new DropdownListItem(el.codice, el.descrizione) }));
    }
  }

  private setupCommands() {
    // this.cmdDropDown = new CommandsDropDownSettings({ fullEditBtn: false, infoBtn: false });
    // this.cmdDropDown.addCommand(new GridCommandItem(
    //   'IniziaProtocollo', enum_menuZooGridCommands.StartNewProtocol, 'faMenuZoo', ''
    // ));
    // Setup dynamic commands on ddl menu's open
    this.gridPublicService.openCommands
      .pipe(takeUntil(this.signal), filter(activity => !!activity))
      .subscribe(activity => {
        /* TODO */
      });
  }

  private handleCommandEvent() {
    this.gridPublicService.commandEvent.pipe(
      takeUntil(this.signal),
      filter(cmdEvent => !!cmdEvent)
    ).subscribe(cmd => {
      switch (cmd.command.action) {
        case enum_menuZooGridCommands.StartNewProtocol:
          this.createOperationFromProtocol(cmd.dataItem);
      }
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
      let queryParams: { [key:string] : string | string[] } = {"t_Tab":enum_TypeTab_Zootecnia.Prescriptions.toString()};
      this.agenda.navigateTo(redirectUrl, queryParams, objP, false);
    })
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
      this._filters.prescriptionType
    );
  }

}

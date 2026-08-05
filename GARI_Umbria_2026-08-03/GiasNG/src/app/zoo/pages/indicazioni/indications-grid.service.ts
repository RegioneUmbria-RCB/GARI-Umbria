import {Injectable, Injector} from "@angular/core";
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  CommandsDropDownSettings,
  CustomColumnSettings,
  EditingMode,
  GiasMessageService,
  HttpAction,
  KendoGridColumn,
  KendoServerResultImpl,
  LoaderType,
  NumericSettings,
  ToolbarSettings
} from "gias-kendo-grid";
import {PermessiUtenteService} from "../../../Service/permessi-utente.service";
import {catchError, filter, forkJoin, map, Observable, of, switchMap, take, tap} from "rxjs";
import {enum_Security_Attivita} from "../../../Model/TipiEnumerativi";
import {PrescrizioniClient, UpdatePrescrizione} from "../../../Service/net-core6-api.service";
import {ZooOperationsFilters} from "../../models/zoo-operations-filters.model";
import {ObjParametriAgendaService} from "../../../Service/obj-parametri-agenda.service";
import {ZooPrescriptionGridFlatItem, ZooPrescriptionGridModel} from "../../models/zoo-prescription-grid-item.model";
import {BaseCodeDescr} from "../../../Service/api.service";
import {ZooFiltersHelperService} from "../../services/zoo-filters-helper.service";
import {cloneDeep} from "lodash";
import {TUTTI_CENTRI_AZIENDALI} from "../../../Model/CostantiPersonalizzate";
import {LeggiPrescrizioniVeterinarie} from "../../models/leggi-prescrizioni.model";
import { ConversionService } from "gias-ui-kit";
import { IntervalloTemporale } from "app/Model/anagrafiche/IntervalloTemporale";

@Injectable()
export class ZooIndicationsGridConfigService extends AbstractGridConfigService<KendoServerResultImpl> {
  editingMode = EditingMode.IN_LINE;
  gridId = "zooIndicationsGrid";
  loader = LoaderType.SERVICE;
  rowId = "IdRicetta";

  protected canEdit = true;
  private _centers: BaseCodeDescr[];
  private _stables: BaseCodeDescr[];
  private _filters: ZooOperationsFilters | null = null;
  private lastFilterUsed: ZooOperationsFilters | null = null;
  private _gridModel = ZooPrescriptionGridModel;
  private _gridRows: ZooPrescriptionGridFlatItem[] = [];
  private _gridColumns = [
    new KendoGridColumn({ field: 'IdRicetta', title: this.transloco.translate('ID') }, { editable: false }),
    new KendoGridColumn({ field: 'DataEmissione', title: this.transloco.translate('Data') }, { editable: false }),
    new KendoGridColumn({ field: 'Numero', title: this.transloco.translate('Numero') }, { editable: false }),
    new KendoGridColumn({ field: 'Denominazione', title: this.transloco.translate('Denominazione') }, { editable: false }),
    new KendoGridColumn({ field: 'Capi', title: this.transloco.translate('Capi') }, { editable: false }),
    new KendoGridColumn({ field: 'TipoDes', title: this.transloco.translate('Tipo') }, { editable: false }),
    new KendoGridColumn({ field: 'Note', title: this.transloco.translate('Note') }, { editable: false }),
    new KendoGridColumn({ field: 'Posologia', title: this.transloco.translate('Posologia') }, { editable: false }),
    new KendoGridColumn({ field: 'sa_nome', title: this.transloco.translate('CentroAziendale') }, { editable: false }),
    new KendoGridColumn({ field: 'STA_DES', title: this.transloco.translate('Stalla') }, { editable: false }),
    new KendoGridColumn({ field: 'Quantitativo', title: this.transloco.translate('Quantitativo') }, { editable: false }),
    new KendoGridColumn({ field: 'Numero_Somm', title: this.transloco.translate('NSomministrazioni') }, { editable: true, width: 150, format: '{0:n0}', numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n0', step: 1, decimals: 0 }) }),
    new KendoGridColumn({ field: 'Intervallo_Somm', title: this.transloco.translate('IntSomministrazioni') }, { editable: true, width: 150, format: '{0:n0}', numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n0', step: 1, decimals: 0 }) }),
    new KendoGridColumn({ field: 'ProprietarioIdFiscale', title: this.transloco.translate('Proprietario') }, { editable: false }),
    new KendoGridColumn({ field: 'VeterinarioIdFiscale', title: this.transloco.translate('Veterinario') }, { editable: false }),
    new KendoGridColumn({ field: 'DetentoreIdFiscale', title: this.transloco.translate('Detentore') }, { editable: false }),
    new KendoGridColumn({ field: 'Username_Creazione', title: this.transloco.translate('UtenteCreazione') }, { editable: false }),
    new KendoGridColumn({ field: 'Username_Modifica', title: this.transloco.translate('UtenteModifica') }, { editable: false, width: 150 })
  ];

  constructor(
    injector: Injector,
    private agenda: ObjParametriAgendaService,
    private prescriptions: PrescrizioniClient,
    private permissions: PermessiUtenteService,
    private helper: ZooFiltersHelperService,
    private giasMessageService: GiasMessageService,
    private conversionService: ConversionService
  ) {
    super(injector);
    this.handleCustomizations();
  }

  private get currentPiva(): string {
    const agenda = this.agenda.getObjParamValue();
    return agenda.Piva ?? '';
  }

  read(options?: ZooOperationsFilters): Observable<KendoServerResultImpl> {
    if (this.reuseLoadedData(options))
      return of(new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows));

    this._filters = cloneDeep(options);
    this.lastFilterUsed = cloneDeep(options);
    this.loadingService.set_isLoading({isLoading: true, component: this.gridPublicService.gridElRef});

    return forkJoin([
      this.loadAdditionalData(),
      this.prescriptions.prescrizioniLeggiPrescrizioni(this.getLoadParams())
    ]).pipe(
      map(results => results[1]),
      map(r => r.RispostaOK ? JSON.parse(r.RispostaStringa) : []),
      map(r => this.conversionService.ConversionDateInObject(r)),
      tap(rows => this._gridRows = rows.filter(r =>
        new Date(r.DataEmissione) >= options.from && new Date(r.DataEmissione) <= options.to
      )),
      map(() => new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows)),
      tap(() => this.loadingService.set_isLoading({isLoading: false, component: this.gridPublicService.gridElRef}))
    );
  }

  perform(actionType: HttpAction, items: any, oldRow: any): Observable<any> {
    switch (actionType) {
      case HttpAction.UPDATE:
        return this.updateItems(items);
      case HttpAction.REMOVE:
        return this.deleteItem(items);
      default:
        return of();
    }
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

  private handleCustomizations(): void {
    this.gridIsEditable = this.permissions.canWritePermesso(enum_Security_Attivita.ZooProtocolliTerapeutici);
    this.canEdit = this.permissions.canWritePermesso(enum_Security_Attivita.ZooProtocolliTerapeutici);
    this.groups.groupable.enabled = false;
    this.columnMenu.kendoGridColumnChooser = true;
    this.views.enabled = true;
    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.cmdColumn = new CommandsColumnSettings({ editBtn: true, infoBtn: false, removeBtn: false });
    this.cmdColumn.width = 30;
    this.groups.groupable.enabled = false;
    this.customColumn = new CustomColumnSettings({
      title: '',
      showColumn: true,
      useCustomColumnCellTemplate: true
    });

    this.selectable.selectable.enabled = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.showSelectAll = true;
  }

  private getParamsForUpdateOrDelete(items: any): UpdatePrescrizione[] | UpdatePrescrizione {
    let paramSolo: UpdatePrescrizione;
    let arrParams: UpdatePrescrizione[] = [];

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
          IntSomm: item.Intervallo_Somm
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
        IntSomm: items.Intervallo_Somm
      } as UpdatePrescrizione;

      return paramSolo;
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

  private getLoadParams() {
    return new LeggiPrescrizioniVeterinarie(
      this.currentPiva,
      this._filters.center,
      this._filters.stable,
      new IntervalloTemporale(
        this._filters.from,
        this._filters.to
      )
    );
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

}

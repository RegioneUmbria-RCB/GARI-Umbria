import { Injectable, Injector } from "@angular/core";
import { AbstractGridConfigService, CommandsColumnSettings, CommandsDropDownSettings, CustomColumnSettings, DropdownListItem, DropdownListWithForm, EditingMode, GridCommandItem, HttpAction, KendoGridColumn, KendoServerResultImpl, LoaderType, NumericSettings, ToolbarSettings } from "gias-kendo-grid";
import { ModelEntry, CommandsDropDownEvents } from 'gias-kendo-grid';
import { CELL_TYPES, ConversionService, AGRODATAFINE, AGRODATAINIZIO, Enum_DBTypeOperation, GiasDialogService, ObjParametriAgenda } from 'gias-ui-kit';
import { catchError, filter, forkJoin, from, map, mergeMap, Observable, of, switchMap, take, takeUntil, tap } from "rxjs";
import { BaseCodeDescr } from "app/Service/api.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { TerapieClient, OperazioniZooClient, PrescrizioniClient } from 'app/Service/net-core6-api.service';
import { ZooFiltersHelperService } from "../../services/zoo-filters-helper.service";
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";
import { CookieService } from "ngx-cookie-service";
import { enum_PagineGiasNG, enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { enum_menuZooGridCommands } from "app/zoo/zoo.utils";
import { cloneDeep } from "lodash";
import { TUTTI_CENTRI_AZIENDALI } from "app/Model/CostantiPersonalizzate";
import { ZooTherapiesGridFlatItem, ZooTherapiesGridModel, ZooTherapiesFilters } from "app/zoo/models/zoo-therapies.model";
import { ZooOperationsFilters } from "app/zoo/models/zoo-operations-filters.model";
import { ZooRedirectorService } from "app/zoo/services/zoo-redirector.service";

@Injectable()
export abstract class ZooTherapiesGridConfigService extends AbstractGridConfigService<KendoServerResultImpl> {
  gridId = "zooTherapiesGrid";
  rowId = "Id_Terapia";
  editingMode = EditingMode.IN_LINE;
  loader = LoaderType.SERVICE;

  protected canEdit = false;
  protected _postReadProcessing = (rows) => rows;
  protected _filters: ZooOperationsFilters | null = null;
  protected _gridRows: ZooTherapiesGridFlatItem[] = [];
  protected _gridColumns = [
    new KendoGridColumn({
        field: "Id_Terapia",
        title: "ID"
    }, {
        width: 100,
        editable: false,
        filterable: true,
    }),
    new KendoGridColumn({
        field: "Terapia_Des",
        title: this.transloco.translate('Terapia')
    }, {
        width: 100,
        editable: false,
        filterable: true,
    }),
    // new KendoGridColumn({
    //     field: "Data",
    //     title: this.transloco.translate('Data')
    // }, {
    //     width: 100,
    //     editable: false,
    //     filterable: true,
    // }),
    new KendoGridColumn({
        field: "Impresa",
        title: this.transloco.translate('Impresa'),
    }, {
        width: 100,
        editable: false,
        filterable: true,
    }),
    new KendoGridColumn({
        field: "Centro",
        title: this.transloco.translate('CentroAziendale'),
    }, {
        width: 100,
        editable: false,
        filterable: true,
    }),
    new KendoGridColumn({
        field: "Stalla",
        title: this.transloco.translate('Stalla'),
    }, {
        width: 100,
        editable: false,
        filterable: true,
    }),
    new KendoGridColumn({
        field: "Interventi",
        title: this.transloco.translate('Interventi'),
    }, {
        width: 200,
        editable: false,
        filterable: true,
    })
  ];

  private _gridModel = ZooTherapiesGridModel;
  private _centers: BaseCodeDescr[];
  private _stables: BaseCodeDescr[];

  constructor(
    injector: Injector,
    protected agenda: ObjParametriAgendaService,
    protected therapiesClient: TerapieClient,
    protected helper: ZooFiltersHelperService,
    private permissions: PermessiUtenteService,
    private gestioneRichieste: GestioneRichiesteService,
    private zooRedirector: ZooRedirectorService,
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

  private setUpDdlCols() {
    // const col = this._gridColumns.find(s => s.field === 'Udm_Dose');
    // let data: DropdownListItem[] = [];
    // col.ddl = new DropdownListWithForm('id', 'Udm_Dose', 'name', data);
    // col.ddl.loadOnEdit = true;
    // col.ddl.descriptionField = 'Udm_Sim';
    // col.ddl.loadFunction = (dataItem: any) => { return of([]); };
  }

  private setupCommands() {
    // this.cmdDropDown = new CommandsDropDownSettings({ fullEditBtn: false, infoBtn: false, removeBtn: true });
    // this.cmdDropDown.addCommand(new GridCommandItem(
    //   'EseguiTrattamento' | transloco,
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
    this.gridPublicService.openCommands.GiasSubscribe(terapia => {

    });
  }

  private handleCommandEvent() {
    this.gridPublicService.commandEvent.pipe(
      takeUntil(this.signal),
      filter(cmdEvent => !!cmdEvent)
    ).subscribe(cmd => {
      switch (cmd.command.action) {
        case CommandsDropDownEvents.INFO:
          this.zooRedirector.redirectToTherapy(cmd.dataItem.Id_Terapia, Enum_DBTypeOperation.Read, this.gridPublicService.gridElRef);
          break;
        case CommandsDropDownEvents.FULL_EDIT:
          this.zooRedirector.redirectToTherapy(cmd.dataItem.Id_Terapia, Enum_DBTypeOperation.Update, this.gridPublicService.gridElRef);
          break;
      }
    });
  }

  private reuseLoadedData(options: ZooOperationsFilters): Boolean {
    return (!this._filters && !options) || (!options && !!this._filters) && (
      !!this._filters && !!options
      && options.center === this._filters.center
      && options.stable === this._filters.stable
      && options.from === this._filters.from
      && options.to === this._filters.to
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

  private getLoadParams() {
    if (!this._filters) {
      return new ZooTherapiesFilters(this.currentPiva, 0, 0, new Date());
    }
    else {
      return new ZooTherapiesFilters(
        this.currentPiva,
        this._filters.center ,
        this._filters.stable,
        this._filters.from ?? AGRODATAINIZIO
      );
    }
  }

  private deleteItem(items: any): Observable<any> {
      const arrParams = items.map((item: any) => ({
        Id_Terapia: item.Id_Terapia
      }));

      this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

      return forkJoin(
        arrParams.map(item =>
          this.therapiesClient.terapieDeleteTerapia(item.Id_Terapia).pipe(
            catchError(() => of({
                  RispostaOK: false,
                  Errore: this.transloco.translate("zoo.ErroreDuranteEliminazioneTerapia", [item.Id_Terapia])
                })
            )
          )
        )
      ).pipe(
        tap(() =>
          this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef })
        ),
        map((responses: any[]) => {
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
          if (errorMessages) console.log(errorMessages);
        }),
        switchMap(() => {
          const options = new ZooOperationsFilters(0, 0, AGRODATAINIZIO, AGRODATAFINE, []);
          return this.read(options);
        })
      );
  }

  protected handleCustomizations(): void {
    // Permesso scrittura per aggiungere pulsante cancellazione
    this.canEdit = this.permissions.canWritePermesso(enum_Security_Attivita.ZooTerapie);

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
    this.cmdDropDown = new CommandsDropDownSettings({ fullEditBtn: true, infoBtn: true });
    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });
    this.groups.groupable.enabled = false;
    // this.customColumn = new CustomColumnSettings({
    //   title: '',
    //   showColumn: true,
    //   useCustomColumnCellTemplate: true
    // });
    this.setUpDdlCols();
    this.setCommandsVisibility();
  }

  read(options?: ZooOperationsFilters): Observable<KendoServerResultImpl> {
    if (this.reuseLoadedData(options) || this.cookies.get('zooTab') != "4") {
      return of(new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows));
    }
    this._filters = cloneDeep(options);
    this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
    // this.gridPublicService
    return forkJoin([
      this.loadAdditionalData(),
      this.therapiesClient.terapieReadTerapieGrid(this.getLoadParams())
    ]).pipe(
      map(results => results[1]),
      map(r => r.RispostaOK ? JSON.parse(r.RispostaStringa) : []),
      map(r => this.conversionService.ConversionDateInObject(r)),
      tap(rows => this._gridRows = rows),
      map(() => new KendoServerResultImpl(this._gridModel, this._gridColumns, this._gridRows)),
      tap(() => console.debug(this._gridRows)),
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
}

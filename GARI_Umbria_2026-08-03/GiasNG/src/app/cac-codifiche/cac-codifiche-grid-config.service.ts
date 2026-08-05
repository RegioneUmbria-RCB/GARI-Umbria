import { AbstractGridConfigService, CommandsColumnSettings, ConfigTemplate, DropdownListWithForm, EditingMode, GiasMessageService, HttpAction, KendoGridColumn, KendoGridModel, KendoGridRow, LoaderType, ModelEntry, ResizableSettings, ToolbarSettings } from "gias-kendo-grid";
import { CacCodificaModel, CacConvKendoGridModel, CacStatusCodMapping, CodificheKendoServerResult } from "./cac-codifiche.component.model";
import { Observable, of, switchMap, map, catchError } from "rxjs";
import { TranslocoService } from "@jsverse/transloco";
import { CELL_TYPES, DropdownListItem, Enum_DBTypeOperation } from "gias-ui-kit";
import { SMARTPHONE_WIDTH } from "app/Model/CostantiPersonalizzate";
import { Injectable, Injector } from "@angular/core";
import { throwError as _observableThrow, of as _observableOf } from 'rxjs';
// import { CacCodificheClient } from "app/Service/net-core6-api.service";
import { Validators } from "@angular/forms";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { enum_Security_Attivita } from "app/Model/TipiEnumerativi";
import { CodificaCACClient } from "app/Service/net-core6-api.service";

@Injectable({ providedIn: 'root' })
export class CacCodificheGridConfigervice extends AbstractGridConfigService<CodificheKendoServerResult> {

  gridId = 'cac-grid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'chiave';
  public permessoEdit: boolean;


  kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'Sistema_Cod', title: this.translocoService.translate('SistemaCod') }, { resizable: true, editable: true, width: 100, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Codice_Esterno', title: this.translocoService.translate('CodiceEsterno') }, { resizable: true, editable: true, width: 100, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Descrizione_Esterno', title: this.translocoService.translate('DescrizioneEsterno') }, { resizable: true, editable: true, width: 250, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Tabella_Gias', title: this.translocoService.translate('TabellaGias') }, { resizable: true, editable: true, width: 140, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Codice_Gias', title: this.translocoService.translate('CodiceGias') }, { resizable: true, editable: true, width: 100, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 80 }),
    new KendoGridColumn({ field: 'Data_Modifica', title: this.translocoService.translate('DataModifica') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 105 }),
    new KendoGridColumn({ field: 'Username_Creazione', title: this.translocoService.translate('UsernameCreazione') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 175 }),
    new KendoGridColumn({ field: 'Username_Modifica', title: this.translocoService.translate('UsernameModifica') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100 }),
    new KendoGridColumn({ field: 'Validita_Inizio', title: this.translocoService.translate('ValiditaInizio') }, { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 180 }),
    new KendoGridColumn({ field: 'Validita_Fine', title: this.translocoService.translate('ValiditaFine') }, { resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 180 }),
  ];

  gridModel: CacConvKendoGridModel = {
    chiave: new ModelEntry(CELL_TYPES.STRING, false),
    ID: new ModelEntry(CELL_TYPES.STRING, false),
    Sistema_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Codice_Esterno: new ModelEntry(CELL_TYPES.STRING),
    Descrizione_Esterno: new ModelEntry(CELL_TYPES.STRING),
    Tabella_Gias: new ModelEntry(CELL_TYPES.STRING),
    Codice_Gias: new ModelEntry(CELL_TYPES.STRING),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATE),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATE),
    Username_Creazione: new ModelEntry(CELL_TYPES.STRING),
    Username_Modifica: new ModelEntry(CELL_TYPES.STRING),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE),
    Sistema: new ModelEntry(CELL_TYPES.STRING),
  };

  constructor(
    private CAClient: CodificaCACClient,
    protected injector: Injector,
    protected translocoService: TranslocoService,
    private giasMessageService: GiasMessageService,
    private permessiUtenteService: PermessiUtenteService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.handleCustomization();
    this.handleDropdowns();
  }

  read(options?: any): Observable<CodificheKendoServerResult> {
    this.isLoading(true);

    return this.CAClient.codificaCACGetCacCodifiche().pipe(catchError((err) => {
      this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
      return of();
    }), map(r => {

      this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
      let objResult = JSON.parse(r.RispostaStringa) as CacCodificaModel[];

      let gridVisiteServerResult = new CodificheKendoServerResult(
        this.gridModel,
        this.kendoColumns,
        objResult
      );
      return gridVisiteServerResult;
    }));
  }

  perform(actionType: HttpAction, item: any): Observable<any[]> {

    let httpRisp: Observable<any>;
    httpRisp = this.handleHttpAction(actionType, item);

    if (httpRisp) {
      return httpRisp.pipe(map((risposta) => {
        if (risposta.RispostaOK) {
          this.giasMessageService.successMessage(this.translocoService.translate('cacSalvataggioEffettuato'));
          return [];
        }
      }), switchMap((e) => of([])));
    }
    return of([]);
  }

  private handleHttpAction(actionType: HttpAction, item: any): Observable<any> {
    const translate = (key: string) => this.translocoService.translate(key);

    switch (actionType) {

      case HttpAction.REMOVE:
        return this.writeCodifica(item, Enum_DBTypeOperation.Delete).pipe(map(response => {
          if (!response.RispostaOK) {
            this.giasMessageService.errorMessage(translate('cacErroreSalvataggio'));
            return [];
          } else {
            this.giasMessageService.successMessage(translate('cacCancellazioneEffettuata'));
          }
          return [];
        }));

      case HttpAction.UPDATE:
        return this.writeCodifica(item, Enum_DBTypeOperation.Update);

      case HttpAction.CREATE:
        return this.writeCodifica(item, Enum_DBTypeOperation.Write);

      default:
        console.warn('Azione HTTP non riconosciuta:', actionType);
        return of([]);
    }
  }

  private writeCodifica(item: any, operation: Enum_DBTypeOperation): Observable<any> {
    item.TipoOperazioneDB = operation;

    return this.CAClient.codificaCACWriteCacCodifiche(
      item
    );
  }

  private handleDropdowns(): void {

    let col: KendoGridColumn;
    col = this.kendoColumns.find(s => s.field === 'Sistema_Cod');
    let data = [
      new DropdownListItem(CacStatusCodMapping.Agea, 'Agea'),
      new DropdownListItem(CacStatusCodMapping.Artea, 'Artea'),
      new DropdownListItem(CacStatusCodMapping.CAI, 'CAI'),
      new DropdownListItem(CacStatusCodMapping.Demetra, 'Demetra'),
      new DropdownListItem(CacStatusCodMapping.Enogis, 'Enogis'),
      new DropdownListItem(CacStatusCodMapping.Gias, 'Gias'),
      new DropdownListItem(CacStatusCodMapping.Smarttractors, 'Smarttractors'),
    ];
    col.ddl = new DropdownListWithForm('CAC_Codice', 'Sistema_Cod', 'Sistema', data);
    col.ddl.valuePrimitive = true;
    col.ddl.descriptionField = "Sistema";

  }

  private handleCustomization() {
    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.ManutenzioneArchivi_GestioneSistemi_Esterni, 2);

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: this.permessoEdit,
      infoBtn: false,
      removeBtn: this.permessoEdit
    });

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = this.permessoEdit;
    this.toolbar.resetChanges = false;
    this.behavior.excelSettings.enabled = true;
    this.behavior.showDeletionConfirmation = true;
    this.columnMenu.kendoGridColumnChooser = false;
    this.views.enabled = true;

    // impostazioni paginazione
    this.pagination.gridState.sort = [
      { field: 'Data_Modifica', dir: 'desc' },
    ];

    this.pagination.gridState.take = 20;
    this.pagination.navigable = false;
    this.pagination.pageable = {
      buttonCount: 4,
      pageSizes: [20, 30, 50, 100, {
        text: this.translocoService.translate('Tutti'),
        value: "all",
      } as any as number]
    };
  }
}
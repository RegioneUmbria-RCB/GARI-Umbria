import {Injectable, Injector} from "@angular/core";
import {CommandsColumnSettings, PaginationSettings} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {filter, from, Observable, of, switchMap, takeUntil} from "rxjs";
import {FormGroup} from "@angular/forms";
import {ProfilazioneDataShareService} from "../../../services/profilazione-data-share.service";
import {ImpostazioniFormService} from "../../../services/impostazioni/impostazioni-form.service";
import {enum_PaginaImpostazioni} from "../../impostazioni.model";

export class GridCategorieMagazzinoServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class GridCategorieMagazzinoService extends AbstractGridConfigService<GridCategorieMagazzinoServerResult> {
  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = "Elem_Cod";
  gridId = 'gridCategorieMagazzinoID';

  private kendoRows = [];
  private kendoModel: KendoGridModel = {
    NomeComune: new ModelEntry(CELL_TYPES.NUMBER),
    Elem_Cod: new ModelEntry(CELL_TYPES.STRING),
    ValueCod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    ValueDes: new ModelEntry(CELL_TYPES.STRING),
    Impostazione: new ModelEntry(CELL_TYPES.NUMBER)
  };

  constructor(
    injector: Injector,
    private datashare: ProfilazioneDataShareService,
    private settingsFormService: ImpostazioniFormService,
  ) {
    super(injector);
    this.handleCustomization();
    this.handleEvents();
  }

  private get form(): FormGroup {
    if (this.kendoRows.length) {
      let impostazioneCod = this.kendoRows[0].Impostazione;
      return this.datashare.formsMap.get(impostazioneCod);
    } else return null;
  }

  private get filtroStr(): string {
    return this.kendoRows.filter(u => u.ValueCod !== undefined)
      .map(u => u.Elem_Cod + "_" + u.ValueCod)
      .reduce((a, b) => a + "|" + b);
  }

  read(options?: any): Observable<GridCategorieMagazzinoServerResult> {
    if (this.gridPublicService.value) {
      this.kendoRows = this.gridPublicService.value.data.rows;
      // Set proprieties in the Abstract class
      this.model = this.gridPublicService.value.data.model;
      this.columns = this.gridPublicService.value.data.columns;
      return of(this.gridPublicService.value.data);
    }
    return of(new GridCategorieMagazzinoServerResult([], [], this.kendoModel));
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return from([]);
  }

  private handleCustomization() {
    if (this.settingsFormService.USAGE_AREA === enum_PaginaImpostazioni.AZIENDE_CENTRI) {
      this.gridIsEditable = this.datashare.userPermissions.canEditBusinessSettings;
    }
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false,
    });
    this.behavior.excelSettings.enabled = false;
    this.behavior.pdfSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.views.enabled = false;
    this.groups.groupable.enabled = false;
    this.pagination = this.getPaginationSettings();
  }

  private getPaginationSettings(): PaginationSettings {
    const result: PaginationSettings = new PaginationSettings();
    result.gridState = {
      sort: [],
      skip: 0,
      group: [],
      take: 100,
      filter: {
        logic: 'and',
        filters: [],
      },
    };
    result.pageable = {
      buttonCount: 4,
      info: true,
      type: 'input',
      pageSizes: [5, 8, 10, 25, 50, 100, {
        text: this.transloco.translate('Tutti'),
        value: "all",
      } as any as number]
    };
    result.navigable = false;
    return result;
  }

  /**
   * Handles events related to the grid form group and updates the corresponding Kendo grid rows.
   * 
   * This method subscribes to the `valueChanges` observable of the `formGroup` from `gridPublicService`,
   * and performs the following actions:
   * - Updates the `ValueCod` property of the corresponding row in `kendoRows` based on the `Elem_Cod` value.
   * - Checks if the current form's `valoreCorrente` value differs from `filtroStr`, and if so:
   *   - Updates the `valoreCorrente` form control with the value of `filtroStr`.
   *   - Marks the form as touched.
   * 
   * The subscription is automatically unsubscribed when the `signal` observable emits a value.
   * 
   * @private
   */
  private handleEvents() {
    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      filter(inlineForm => !!inlineForm),
      switchMap(inlineForm => inlineForm.valueChanges)
    ).subscribe((val) => {
      this.kendoRows.find(r => r.Elem_Cod === val.Elem_Cod).ValueCod = val.ValueCod;
      if (this.form.get('valoreCorrente').value !== this.filtroStr) {
        this.form.get('valoreCorrente').patchValue(this.filtroStr);
        this.form.markAsTouched();
      }
    });
  }
}

import { Injectable, Injector } from '@angular/core';
import {
  EditingMode, LoaderType, AbstractGridConfigService, HttpAction, KendoServerResult, KendoGridColumn, ModelEntry,
  MasterDetailSettings, CommandsColumnSettings, GridCustomizations, RendererGridEvent, KendoGridRow
} from 'gias-kendo-grid';
import { RowClassArgs } from '@progress/kendo-angular-grid';
import { process } from '@progress/kendo-data-query';
import { CELL_TYPES } from 'gias-ui-kit';
import { Observable, of, takeUntil, map, Subscription } from 'rxjs';
import { GridServerResult } from 'app/qualita-tracciabilita/models/grid-server-result.model';
import { VerificaDisciplinariService } from 'app/qualita-tracciabilita/services/verifica-disciplinari.service';
import { RelatedOperationsCellComponent } from 'app/qualita-tracciabilita/components/custom-cells/related-operations-cell/related-operations-cell.component';

@Injectable({
  providedIn: 'root'
})
export class DettaglioAnalisiService extends AbstractGridConfigService<GridServerResult> {
  editingMode: EditingMode = EditingMode.IN_PAGE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'Row_Key';
  gridId = 'DettaglioAnalisi';

  private _columns = [
    new KendoGridColumn({ field: 'Descrizione', title: this.transloco.translate('ControlloEffettuato') }, { editable: false, width: 200 }),
    new KendoGridColumn({ field: 'Esito', title: this.transloco.translate('NonConforme') }, { editable: false, width: 150 }),
    new KendoGridColumn({ field: 'Messaggio', title: this.transloco.translate('DettaglioNonConformita') }, { editable: false, showHTMLAsString: true }),
    new KendoGridColumn({ field: 'AgendeCorrelate', title: this.transloco.translate('OperazioniCorrelate') }, { editable: false, component: RelatedOperationsCellComponent }),
  ];
  private _model = {
    CodiceControllo: new ModelEntry(CELL_TYPES.NUMBER),
    Descrizione: new ModelEntry(CELL_TYPES.STRING),
    Esito: new ModelEntry(CELL_TYPES.BOOLEAN), // from 26/03/2024: true if non-compliant, false otherwise
    Messaggio: new ModelEntry(CELL_TYPES.STRING),
    AgendeCorrelate: new ModelEntry(CELL_TYPES.CUSTOM),
  };
  /** The rows displayed in the grid. */
  private _controls = [];
  private _processedControls = [];
  private _stateChangeSub: Subscription = null;

  constructor(
    injector: Injector,
    private analisiService: VerificaDisciplinariService
  ) {
    super(injector);
    this.handleCustomization();
  }

  /**
   * Retrieves detailed analysis data and maps it to a `GridServerResult` observable.
   *
   * @param dataItem - the dataItem in the master grid from which the detail is generated.
   * @returns An `Observable` that emits a `GridServerResult` containing the model, columns, and fetched rows.
   */
  read(dataItem?: any): Observable<GridServerResult> {
    this._controls = Array.from(this.getDetailsMap(dataItem.DettagliVerifica).values());
    this._controls.forEach(x => x.Esito = !x.Esito);

    const agende = dataItem.DettagliVerifica.flatMap(x => x.AgendeCorrelate);
    if (agende.length == 0) {
      this._columns.find(x => x.field == 'AgendeCorrelate').hidden = true;
    }
    if (this._stateChangeSub == null) {
      this._processedControls = process(this._controls, this.pagination.gridState).data;
      this._stateChangeSub = this.gridPublicService.gridComp.dataStateChange
        .subscribe((state) => {
          this._processedControls = process(this._controls, state).data;
          this.applyRendererRules();
        });
    }
    return of(new GridServerResult(this._model, this._columns, this._processedControls));
  }

  perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
    return null;
  }

  override applyRendererRules(opts?: RendererGridEvent): void {
    const DOMRows = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr.k-master-row');
    const notCompliant = [];
    const compliant = [];
    for (let i = 0; i < DOMRows.length; i++) {
      if (!this._processedControls[i].Esito) { compliant.push({ idx: i, row: DOMRows[i] }); }
      else { notCompliant.push({ idx: i, row: DOMRows[i] }); }
    }
    for (let rows of notCompliant) {
      const style = `background-color: ${(rows.idx % 2 != 0) ? '#ffebee' : '#ffdbe1'} !important; color: #c62828 !important;`;
      rows.row.setAttribute('style', style);
    }
    for (let rows of compliant) {
      const style = ``;
      rows.row.setAttribute('style', style);
    }
  }

  // Non osno sicura che il metodo sotto funzioni...
  onRowClass = (event: RowClassArgs) => this.coloraRigheOperazioni(event.dataItem);

  coloraRigheOperazioni(row: KendoGridRow): { [k: string]: boolean } {
    return {
      rowNotCompliant: row['Esito'],
    };
  }
  // fine.

  private handleCustomization() {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      removeBtn: false,
    });
    this.masterdetailSettings = new MasterDetailSettings(true, {
      flag_grid_detail: true,
      flag_grid_master: false,
    });
    this.groups.groupable.enabled = false;
    this.views = new GridCustomizations({ enabled: false });
    this.behavior.excelSettings.enabled = false;
    this.columnMenu.columnMenu = false;

    this.pagination.gridState.take = 100;
    this.pagination.gridState.sort = [
      { field: 'Esito', dir: 'desc' },
      { field: 'Descrizione', dir: 'asc' }
    ];
  }

  private getDetailsMap(details: any[]): Map<number, any> {
    const set = new Map<number, any>();
    for (let item of details) {
      if (set.has(item.CodiceControllo)) {
        const existing = set.get(item.CodiceControllo);
        if (!item.Esito && !existing.Messaggio.includes(item.Messaggio)) {
          existing.Messaggio += (existing.Messaggio == "") ? "" : '<br/>';
          existing.Messaggio += item.Messaggio;
        }
        existing.Descrizione = existing.Descrizione || item.CodiceControllo;
        existing.Esito = existing.Esito && item.Esito;
        set.set(item.CodiceControllo, existing);
      } else {
        item.Messaggio = item.Esito ? "" : `${item.Messaggio}`;
        item.Descrizione = item.Descrizione || item.CodiceControllo;
        set.set(item.CodiceControllo, item);
      }
    }
    return set;
  }

}


import { Injectable, Injector } from '@angular/core';
import { process } from '@progress/kendo-data-query';
import { Observable, of, takeUntil, map, Subscription, tap } from 'rxjs';
import {
  EditingMode, LoaderType, AbstractGridConfigService, HttpAction, KendoServerResult, KendoGridColumn, ModelEntry,
  MasterDetailSettings, CommandsColumnSettings, GridCustomizations, RendererGridEvent, KendoGridRow
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { GridServerResult } from 'app/qualita-tracciabilita/models/grid-server-result.model';
import { VerificaDisciplinariService } from 'app/qualita-tracciabilita/services/verifica-disciplinari.service';

@Injectable({
  providedIn: 'root'
})
export class DettaglioMagazzinoService extends AbstractGridConfigService<GridServerResult> {
  editingMode: EditingMode = EditingMode.IN_PAGE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'Row_Key';
  gridId = 'DettaglioMagazzino';

  private _columns = [
    new KendoGridColumn({ field: 'NomeProdotto', title: this.transloco.translate('Prodotto') }, { editable: false }),
    new KendoGridColumn({ field: 'NomeMagazzino', title: this.transloco.translate('Magazzino') }, { editable: false }),
    new KendoGridColumn({ field: 'Lotto', title: this.transloco.translate('Lotto2') }, { editable: false }),
    new KendoGridColumn({ field: 'QtaScaricata', title: this.transloco.translate('QtaScaricata') }, { editable: false }),
    new KendoGridColumn({ field: 'QtaPresenteInMagazzino', title: this.transloco.translate('QtaPresenteInMagazzino') }, { editable: false }),
    new KendoGridColumn({ field: 'UnitaDiMisuraDes', title: this.transloco.translate('UDM') }, { editable: false }),
    new KendoGridColumn({ field: 'Conforme', title: this.transloco.translate('Conforme') }, { editable: false }),
    new KendoGridColumn({ field: 'Messaggio', title: this.transloco.translate('RisultatoVerifica') }, { editable: false }),
  ];
  private _model = {
    CategoriaProdotto: new ModelEntry(CELL_TYPES.NUMBER),
    ChiaveMagazzino: new ModelEntry(CELL_TYPES.STRING),
    NomeMagazzino: new ModelEntry(CELL_TYPES.STRING),
    Esito: new ModelEntry(CELL_TYPES.BOOLEAN),
    Conforme: new ModelEntry(CELL_TYPES.STRING),
    IdProdotto: new ModelEntry(CELL_TYPES.NUMBER),
    NomeProdotto: new ModelEntry(CELL_TYPES.STRING),
    Lotto: new ModelEntry(CELL_TYPES.STRING),
    Messaggio: new ModelEntry(CELL_TYPES.STRING),
    QtaPresenteInMagazzino: new ModelEntry(CELL_TYPES.NUMBER),
    QtaScaricata: new ModelEntry(CELL_TYPES.NUMBER),
    UnitaDiMisura: new ModelEntry(CELL_TYPES.NUMBER),
    UnitaDiMisuraDes: new ModelEntry(CELL_TYPES.STRING),
  };
  /** The rows displayed in the grid. */
  private _details = [];
  private _processedDetails = [];
  private _stateChangeSub: Subscription;
  constructor(
    injector: Injector,
    private analisiService: VerificaDisciplinariService
  ) {
    super(injector);
    this.handleCustomization();
  }

  read(dataItem?: any): Observable<GridServerResult> {
    this._details = dataItem.DettagliVerifica.sort((a, b) => a.NomeProdotto.localeCompare(b.NomeProdotto)) ?? [];
    this._details.forEach(x => {
      x.Conforme = x.Esito ? this.transloco.translate("Si") : this.transloco.translate("No");
    });

    if (this._stateChangeSub == null) {
      this._processedDetails = this._details;
      this._stateChangeSub = this.gridPublicService.gridComp.dataStateChange
        .pipe(takeUntil(this.signal), tap((state) => this._processedDetails = process(this._details, state).data))
        .subscribe(() => this.applyRendererRules());
    }

    return of(new GridServerResult(this._model, this._columns, this._processedDetails));
  }

  perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
    return null;
  }

  override applyRendererRules(opts?: RendererGridEvent): void {
    const DOMRows = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr.k-master-row');
    const notCompliant = [];
    const compliant = [];
    for (let i = 0; i < DOMRows.length; i++) {
      if (this._processedDetails[i].Esito) { compliant.push({ idx: i, row: DOMRows[i] }); }
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

  private handleCustomization() {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      removeBtn: false,
    });
    this.masterdetailSettings = new MasterDetailSettings(true, {
      flag_grid_detail: true,
      flag_grid_master: false,
    });
    this.pagination.gridState.take = 100;
    this.groups.groupable.enabled = false;
    this.views = new GridCustomizations({ enabled: false });
    this.behavior.excelSettings.enabled = false;
    this.columnMenu.columnMenu = false;
  }

}

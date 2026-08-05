import { Injectable, Injector } from '@angular/core';
import { Observable, map, of, switchMap, tap } from 'rxjs';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import {
  CommandsColumnSettings,
  CustomColumnSettings,
  ResizableSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';
import { AllegatoLayer, AllegatoLayerModifica, GisClient, MisuraPerAvversitaAnagrafica } from 'app/Service/api.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';

export class GISAttributiExportResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class GISAttributiExportGridModel extends KendoGridModel {
  allegati_Documenti_Cod: ModelEntry;
  descrizione: ModelEntry;
  data_estrazione: ModelEntry;
  formato: ModelEntry;
  Utente_Esportazione: ModelEntry;
}

@Injectable()
export class GISAttributiExportGridConfig extends AbstractGridConfigService<GISAttributiExportResult> {
  gridId = 'GISAttributiExportGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'allegati_Documenti_Cod';

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'allegati_Documenti_Cod', title: '' },
      { hidden: true }
    ),
    new KendoGridColumn(
      { field: 'descrizione', title: this.transloco.translate('Descrizione') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 200,
      },
    ),
    new KendoGridColumn(
      { field: 'data_estrazione', title: this.transloco.translate('gis.DataEstrazione') },
      {
        resizable: true,
        filterable: true,
        editable: false,
        width: 100
      },
    ),
    new KendoGridColumn(
      { field: 'formato', title: this.transloco.translate('gis.Formato') },
      {
        resizable: true,
        filterable: true,
        editable: false,
        width: 100
      },
    ),
    new KendoGridColumn(
      { field: 'Utente_Esportazione', title: this.transloco.translate('Utente') },
      {
        resizable: true,
        filterable: true,
        editable: false,
        width: 100
      },
    ),
  ];

  GISAttributiExportGridModel: GISAttributiExportGridModel = {
    allegati_Documenti_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    descrizione: new ModelEntry(CELL_TYPES.STRING),
    data_estrazione: new ModelEntry(CELL_TYPES.DATETIME),
    formato: new ModelEntry(CELL_TYPES.STRING),
    Utente_Esportazione: new ModelEntry(CELL_TYPES.STRING),
  };

  constructor(
    protected injector: Injector,
    protected transloco: TranslocoService,
    private gisClient: GisClient,
    private layerService: LayerService,
    private giasMessageService: GiasMessageService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings();
    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: true,
      infoBtn: false,
      removeBtn: false
    });

    this.behavior.excelSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.cmdColumn['widthSet'] = 50;

    this.customColumn = new CustomColumnSettings({showColumn: true,width: 25,useCustomColumnCellTemplate: true});
  }

  public read(): Observable<GISAttributiExportResult> {
    this.isLoading(true);

    return this.readData()
      .pipe(
        map(res => new GISAttributiExportResult(res, this.columns, this.GISAttributiExportGridModel)),
        tap(() => this.isLoading(false))
      );
  }

  public perform(actionType: HttpAction, row: AllegatoLayer): Observable<KendoGridRow[]> {
    if (actionType != HttpAction.UPDATE || row == null || row.descrizione == '') {
      this.giasMessageService.errorMessage('gis.SpecificareUnaDescrizione', false, true);
      return of([row]);
    }

    const payload = {
      allegati_Documenti_Cod: row.allegati_Documenti_Cod,
      descrizione: row.descrizione,
      inizio_validita: AGRODATAINIZIO,
      fine_validita: AGRODATAFINE
    } as AllegatoLayerModifica;

    return this.gisClient
      .gisModificaEstrazioneShape(payload)
      .pipe(
        switchMap(() => this.readData()),
        tap(() => this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true))
      );
  }

  private readData(): Observable<AllegatoLayer[]> {
    const layer = this.layerService.layerItemSelected;
    const layerType = this.layerService.LayerSelected.value;
    return this.gisClient
      .gisLeggiAllegatiLayer({
        LayerElementiGrafici_Cod: layer[1] ? +layer[0].id : null,
        TipologiaLayer_Cod: +layerType.Option_Value
      }).pipe(
        map(res => res.RispostaStringa.elencoAllegatiLayer),
        map(data => data.sort((a, b) => (new Date(a.data_estrazione).getTime() - new Date(b.data_estrazione).getTime()) * -1))
      );
  }
}

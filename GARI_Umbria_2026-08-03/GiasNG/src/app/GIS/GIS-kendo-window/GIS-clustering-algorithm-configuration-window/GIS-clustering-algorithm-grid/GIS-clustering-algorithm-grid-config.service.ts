import { Injectable, Injector } from '@angular/core';
import { catchError, map, Observable, of, Subject, tap } from 'rxjs';
import { CommandsDropDownSettings, DropdownListWithForm, EditingMode, GridCommandItem, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, StringSettings } from 'gias-kendo-grid';
import { CELL_TYPES, DropdownListItem } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GisClient as NetCoreGisClient, GisClusterConfigSave_InData } from 'app/Service/net-core6-api.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { MasterService } from 'app/Service/master.service';

export class GISClusteringAlgorithmConfigurationResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class GISClusteringAlgorithmConfigurationGridModel extends KendoGridModel {
  id: ModelEntry;
  LayerElementiGrafici_Config_Type: ModelEntry;
  LayerElementiGrafici_Cod: ModelEntry;
  Utente: ModelEntry;
  Livello_Zoom_Massimo_Visualizzazione_Raggruppata: ModelEntry;
}


@Injectable()
export class GISClusteringAlgorithmGridConfigService extends AbstractGridConfigService<GISClusteringAlgorithmConfigurationResult> {
  gridId = 'GISClusteringAlgorithmConfigurationGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'id';
  onParameterUpdate$ = new Subject<GisClusterConfigSave_InData>();

  availableAlgorithm = {
    1: this.transloco.translate('Distanza')
  };

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'id', title: '' },
      {
        hidden: true
      },
    ),
    new KendoGridColumn(
      { field: 'LayerElementiGrafici_Config_Type', title: this.transloco.translate('Algoritmo') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 150,
      },
    ),
    new KendoGridColumn(
      { field: 'LayerElementiGrafici_Cod', title: this.transloco.translate('Layer') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 150,
      },
    ),
    new KendoGridColumn(
      { field: 'Utente', title: this.transloco.translate('Utente') },
      {
        resizable: true,
        filterable: true,
        editable: false,
        width: 75,
        string: new StringSettings({ defaultValue: this.masterService.getCurrentUserUsername() })
      },
    ),
    new KendoGridColumn(
      { field: 'Livello_Zoom_Massimo_Visualizzazione_Raggruppata', title: 'Zoom' },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 75,
        numeric: { max: 20, min: 1, defaultValue: 10, format: 'n0' }
      },
    )
  ];

  GISClusteringAlgorithmConfigurationGridModel: GISClusteringAlgorithmConfigurationGridModel = {
    id: new ModelEntry(CELL_TYPES.NUMBER),
    LayerElementiGrafici_Config_Type: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    LayerElementiGrafici_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    Utente: new ModelEntry(CELL_TYPES.STRING),
    Livello_Zoom_Massimo_Visualizzazione_Raggruppata: new ModelEntry(CELL_TYPES.NUMBER),
  };

  constructor(
    protected injector: Injector,
    protected transloco: TranslocoService,
    private netCoreGisClient: NetCoreGisClient,
    private giasMessageService: GiasMessageService,
    private layerService: LayerService,
    private masterService: MasterService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings(true);
    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: true,
      infoBtn: false,
      removeBtn: true,
    });

    this.behavior.excelSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;

    this.cmdDropDown = new CommandsDropDownSettings();
    this.cmdDropDown.addCommand(new GridCommandItem(this.transloco.translate('ModificaParametri'), 1, 'k-icon k-i-edit'));
    this.gridPublicService.commandEvent.GiasSubscribe(ev => {
      if (!ev || ev.command.action != 1) return;
      this.onParameterUpdate$.next(ev.dataItem as GisClusterConfigSave_InData);
    });
  }

  public read(): Observable<GISClusteringAlgorithmConfigurationResult> {
    this.isLoading(true);
    this.handleDropdowns();
    return this.netCoreGisClient.gisGetGisClusterConfigs()
      .pipe(
        map(res => JSON.parse(res.RispostaStringa) as GisClusterConfigSave_InData[]),
        map(data => new GISClusteringAlgorithmConfigurationResult(
          data.map((x, i) => ({
            ...x,
            id: i,
            LayerElementiGrafici_Config_Type_Des: this.availableAlgorithm[x.LayerElementiGrafici_Config_Type],
            LayerElementiGrafici_Des: x.LayerElementiGrafici_Des || this.layerService.ListLayerItemVisible.value.find(l => +l.TipologiaLayer.id == x.LayerElementiGrafici_Cod)?.TipologiaLayer.nome || ''
          })),
          this.columns,
          this.GISClusteringAlgorithmConfigurationGridModel)
        ),
        tap(() => this.isLoading(false))
      );
  }

  public perform(actionType: HttpAction, configuration: GisClusterConfigSave_InData): Observable<KendoGridRow[]> {
    if (actionType == HttpAction.CREATE) {
      return this.netCoreGisClient
        .gisCreateGisClusterConfig(configuration)
        .pipe(
          catchError(err => {
            this.giasMessageService.errorMessage(FunzioniComuniService.getResponseError(err, this.transloco), false);
            return of(null);
          }),
          map(res => {
            if (res != null) {
              this.giasMessageService.successMessage("gis.ConfigurazioneAlgoritmoClusteringSalvatoCorrettamente", false, true);
            }

            return [configuration];
          })
        );
    } else if (actionType == HttpAction.UPDATE) {
      return this.netCoreGisClient
        .gisUpdateGisClusterConfig(configuration)
        .pipe(
          catchError(err => {
            this.giasMessageService.errorMessage(FunzioniComuniService.getResponseError(err, this.transloco), false);
            return of(null);
          }),
          map(res => {
            if (res != null) {
              this.giasMessageService.successMessage("gis.ConfigurazioneAlgoritmoClusteringSalvatoCorrettamente", false, true);
            }

            return [configuration];
          })
        );
    } else if (actionType == HttpAction.REMOVE) {
      return this.netCoreGisClient
        .gisDeleteGisClusterConfig({
          LayerElementiGrafici_Cod: configuration.LayerElementiGrafici_Cod,
          LayerElementiGrafici_Config_Type: configuration.LayerElementiGrafici_Config_Type,
          Utente: configuration.Utente
        })
        .pipe(
          catchError(err => {
            this.giasMessageService.errorMessage(FunzioniComuniService.getResponseError(err, this.transloco), false);
            return of(null);
          }),
          map(res => {
            if (res != null) {
              this.giasMessageService.successMessage("gis.ConfigurazioneAlgoritmoClusteringEliminatoCorrettamente", false, true);
            }

            return [configuration];
          })
        );
    }

    return of([configuration]);
  }

  private handleDropdowns(): void {
    let col: KendoGridColumn;

    col = this.columns.find(s => s.field === 'LayerElementiGrafici_Cod');
    col.ddl = new DropdownListWithForm('id', 'LayerElementiGrafici_Cod', 'name', []);
    col.ddl.loadFunction = this.loadLayers.bind(this);
    col.ddl.valuePrimitive = true;
    col.ddl.descriptionField = 'LayerElementiGrafici_Des';
    col.ddl.loadOnEdit = true;

    col = this.columns.find(s => s.field === 'LayerElementiGrafici_Config_Type');
    col.ddl = new DropdownListWithForm('id', 'LayerElementiGrafici_Config_Type', 'name', []);
    col.ddl.loadFunction = this.loadAlgorithms.bind(this);
    col.ddl.valuePrimitive = true;
    col.ddl.descriptionField = 'LayerElementiGrafici_Config_Type_Des';
    col.ddl.loadOnEdit = true;
  }

  private loadLayers(_: any): Observable<DropdownListItem[]> {
    return this.layerService.ListLayerItemVisible.pipe(
      map(layers => layers.map(x => ({ id: x.TipologiaLayer.id, name: x.TipologiaLayer.nome })))
    );
  }

  private loadAlgorithms(): Observable<DropdownListItem[]> {
    return of(Object.entries(this.availableAlgorithm).map(x => ({ id: x[0], name: x[1] })));
  }
}
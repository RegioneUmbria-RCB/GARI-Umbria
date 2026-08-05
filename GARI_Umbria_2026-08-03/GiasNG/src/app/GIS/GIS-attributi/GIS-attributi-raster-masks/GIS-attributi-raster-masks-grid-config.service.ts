import { Injectable, Injector } from '@angular/core';
import { Observable, catchError, map, of, switchMap, tap, withLatestFrom } from 'rxjs';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  DateSettings,
  DropdownListItem,
  DropdownListWithForm,
  EditingMode,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import {
  CommandsColumnSettings,
  CustomColumnSettings,
  ResizableSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';
import { GisClient, MascheraLayerRaster, OperazioneMascheraLayerRaster_In, TipoOperazioneMaschera, TipologiaLayer } from 'app/Service/api.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { enum_TipologiaLayer } from 'app/GIS/GIS-enum/GIS-tipologia-layer';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GISRasterConfigurationWindowService } from 'app/GIS/GIS-raster-configuration-window/GIS-raster-configuration-window.service';

export class GISAttributiRasterMasksResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class GISAttributiRasterMasksGridModel extends KendoGridModel {
  mask_cod: ModelEntry;
  mask_des: ModelEntry;
  layer: ModelEntry;
  inizio_validita: ModelEntry;
  fine_validita: ModelEntry;
}

@Injectable()
export class GISAttributiRasterMasksGridConfig extends AbstractGridConfigService<GISAttributiRasterMasksResult> {
  gridId = 'GISAttributiRasterMasksGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'mask_cod';

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'mask_cod', title: '' },
      { hidden: true,editable: false }
    ),
    new KendoGridColumn(
      { field: 'mask_des', title: this.transloco.translate('Descrizione') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 200,
      },
    ),
    new KendoGridColumn(
      { field: 'layer', title: this.transloco.translate('gis.Layer') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 200
      },
    ),
    new KendoGridColumn(
      { field: 'inizio_validita', title: this.transloco.translate('gis.ValiditaInizio') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 100,
        date: new DateSettings()
      },
    ),
    new KendoGridColumn(
      { field: 'fine_validita', title: this.transloco.translate('gis.ValiditaFine') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 100,
        date: new DateSettings()
      }
    )
  ];

  GISAttributiRasterMasksGridModel: GISAttributiRasterMasksGridModel = {
    mask_cod: new ModelEntry(CELL_TYPES.NUMBER),
    mask_des: new ModelEntry(CELL_TYPES.STRING),
    layer: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    inizio_validita: new ModelEntry(CELL_TYPES.DATE),
    fine_validita: new ModelEntry(CELL_TYPES.DATE)
  };

  constructor(
    protected injector: Injector,
    protected transloco: TranslocoService,
    private gisClient: GisClient,
    private layerService: LayerService,
    private gisRasterConfigurationWindowService: GISRasterConfigurationWindowService,
    private giasMessageService: GiasMessageService,
    private translocoService: TranslocoService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings(true);
    this.resizable = new ResizableSettings(true,false);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: true,
      infoBtn: false,
      removeBtn: true
    });

    this.behavior.excelSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.cmdColumn['widthSet'] = 50;

    this.customColumn = new CustomColumnSettings({showColumn: true,width: 100,useCustomColumnCellTemplate: true});
  }

  public read(): Observable<GISAttributiRasterMasksResult> {
    this.setLayerList(this.columns.find(c => c.field === 'layer'));
    this.isLoading(true);

    return this.readData()
      .pipe(
        map(res => new GISAttributiRasterMasksResult(res, this.columns, this.GISAttributiRasterMasksGridModel)),
        tap(() => this.isLoading(false))
      );
  }

  public perform(actionType: HttpAction, row: RasterMask): Observable<KendoGridRow[]> {
    if (row.mask_cod == -1) {
      this.giasMessageService.errorMessage("gis.NonHaiIPermessiPerModificareQuestaMaschera", false, true);
      return this.readData();
    }

    const rasterId = +this.layerService.layerItemSelected[0].id;
    const payload = {
      Maschera_Cod: row.mask_cod,
      Maschera_Des: row.mask_des,
      TipologiaLayer_Cod: +enum_TipologiaLayer.Entita,
      LayerElementiGrafici_Cod: Number.isInteger(row.layer) ? row.layer : +row.layer.id,
      TipologiaLayer_Raster_Cod: +enum_TipologiaLayer.Entita,
      LayerElementiGrafici_Raster_Cod: rasterId,
      Validita_Inizio: row.inizio_validita ?? new Date('1900-01-01'),
      Validita_Fine: row.fine_validita ?? new Date('2100-12-31')
    } as OperazioneMascheraLayerRaster_In;

    if (actionType == HttpAction.CREATE) {
      payload.codice_operazione = TipoOperazioneMaschera.INSERT;
    } else if (actionType == HttpAction.UPDATE) {
      payload.codice_operazione = TipoOperazioneMaschera.UPDATE;
    } else if (actionType == HttpAction.REMOVE) {
      payload.codice_operazione = TipoOperazioneMaschera.DELETE;
    }

    return this.gisClient
      .gisOperazioniMaschereLayerRaster(payload)
      .pipe(
        catchError(error => {
          return of({ RispostaOK: false, Errore: FunzioniComuniService.getResponseError(error, this.translocoService, 'SiÈVerificatoUnErroreDuranteLaFaseDiSalvat') + '\n' });
        }),
        tap(res => {
          if (res.RispostaOK) {
            this.giasMessageService.successMessage('OperazioneCompletataSuccesso', false, true);
            return;
          }

          this.giasMessageService.errorMessage(res.Errore, false, false);
        }),
        switchMap(() => this.gisRasterConfigurationWindowService.fetchMasks(rasterId)),
        switchMap(() => this.readData()),
      )
  }

  private readData(): Observable<RasterMask[]> {
    const layer = this.layerService.layerItemSelected;
    if (!layer[1]) {
      return of([]);
    }

    return this.gisRasterConfigurationWindowService
      .getMasks$(+layer[0].id)
      .pipe(
        withLatestFrom(this.layerService.ListLayerItemVisible), // ObservableLayer is not a BehaviorSubject...
        map(([masks, layers]) => this.parseData(masks))
      );
  }

  private parseData(input: MascheraLayerRaster[]): RasterMask[] {
    return input.map(mask => ({
      mask_cod: mask.maschera_cod,
      mask_des: mask.maschera_des,
      layer: mask.LayerElementiGrafici_cod,
      inizio_validita: mask.inizio_validita ?? new Date('1900-01-01'),
      fine_validita: mask.fine_validita ?? new Date('2100-12-31')
    }) as RasterMask);
  }

  private setLayerList(column: KendoGridColumn): void {
    if (column.field !== 'layer') {
      return;
    }

    const data: DropdownListItem[] = this.layerService.ListLayerItemVisible.value.map(x => new DropdownListItem(+x.TipologiaLayer.id, x.TipologiaLayer.nome));
    column.ddl = new DropdownListWithForm('ddl_layer_cod', 'layer', '', data, null);
    column.ddl.valuePrimitive = false;
  }
}

export interface RasterMask {
  mask_cod: number;
  mask_des: string;
  layer: TipologiaLayer;
  inizio_validita: Date;
  fine_validita: Date;
}

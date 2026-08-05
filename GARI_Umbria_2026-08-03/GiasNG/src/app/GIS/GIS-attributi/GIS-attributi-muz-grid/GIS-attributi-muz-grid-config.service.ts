import { Injectable, Injector } from '@angular/core';
import { combineLatest, filter, map, Observable, of, share, startWith, tap } from 'rxjs';
import { EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import {
  AgrSelectableSettings,
  CommandsColumnSettings,
  CustomColumnSettings,
  ResizableSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { DatiMUZVisibili_Out, GisClient, MisuraPerAvversitaAnagrafica } from 'app/Service/api.service';
import { GISAttributiMuzService } from './GIS-attributi-muz.service';
import { Plot } from './GIS-attributi-muz-grid.component';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { FeatureService } from 'app/GIS/services/feature.service';
import { WKTService } from 'app/GIS/services/wkt.service';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';

export class GISAttributiMuzGridResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class GISAttributiMuzGridConfigService extends AbstractGridConfigService<GISAttributiMuzGridResult> {
  gridId = 'GISAttributiMuzGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_PAGE;
  rowId = 'Area_Cod';

  constructor(
    protected injector: Injector,
    private gisClient: GisClient,
    private gisAttributiMuzService: GISAttributiMuzService,
    private featureInformationService: FeatureInformationService,
    private wktService: WKTService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings();
    this.resizable = new ResizableSettings(true, false);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false
    });

    this.behavior.excelSettings.enabled = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.toolbar.customToolbar = true;

    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = false;
    this.selectable.selectable.enabled = true;
    this.selectable.shouldShowCheckbox = false;

    this.customColumn = new CustomColumnSettings({showColumn: true,width: 200,useCustomColumnCellTemplate: true});
  }

  public read(): Observable<GISAttributiMuzGridResult> {
    this.isLoading(true);

    return combineLatest({
      gridModel: this.gisClient.gisLeggiElencoStrutturaAttributiLayer(enum_LayerElementiGraficiStd.Muz).pipe(share()),
      plot: this.gisAttributiMuzService.selectedPlot$,
      muzs: this.gisAttributiMuzService.muzs$.pipe(filter(muz => muz != null)),
      _: this.gisAttributiMuzService.gridLoadRequest$.pipe(startWith(null))
    })
      .pipe(
        map(data => [GISAttributiMuzGridConfigService.parseMuz(data.plot, data.muzs), JSON.parse(data.gridModel.RispostaStringa.KendoGridAttributiLayer)]),
        map(([data, gridModel]: [DatiMUZVisibili_Out[], any]) => {
          this.model ??= gridModel.kendo_model;
          this.columns ??= gridModel.kendo_columns.map(c =>
            new KendoGridColumn(
              { field: c.field, title: c.title },
              { resizable: true, editable: false,width: 150 }
            ));

          return new GISAttributiMuzGridResult(this.completeMuzDataWithFeatures(data), this.columns, this.model);
        }),
        tap(() => this.isLoading(false)),
        share()
      );
  }

  public perform(_: HttpAction, rows: Array<MisuraPerAvversitaAnagrafica>): Observable<KendoGridRow[]> {
    return of(rows);
  }

  public selectRow(key: string): void {
    const fiters = this.gridPublicService.filters.getValue();
    if (fiters != null && fiters.filters.length == 0) {
      return;
    }

    const row = this.gridPublicService.giasGridComponent.rows.findIndex(r => r[this.rowId] == key);
    this.gridPublicService.selection.setSelected.next({ keys: [key], resetPreviousSelection: true });
    this.gridPublicService.giasGridComponent?.grid.scrollTo({ row: row });
  }

  private static parseMuz(plot: Plot, muzs: DatiMUZVisibili_Out[]): DatiMUZVisibili_Out[] {
    if (plot == null || plot.Appezza == -1) {
      return muzs;
    }

    return muzs.filter(muz => GISAttributiMuzService.plotInMuz(muz, plot));
  }

  private completeMuzDataWithFeatures(data: DatiMUZVisibili_Out[]): DatiMUZVisibili_Out[] {
    const features = this.featureInformationService.getByLayer(enum_LayerElementiGraficiStd.Muz);

    for (const element of data) {
      features.forEach(f => {
        if (f.properties.Area_Cod != element.Area_Cod) {
          return;
        }

        const dataString = f.properties.AppIdRate;

        dataString.split('|').forEach(d => {
          const dd = d.split('§');
          const field = dd[0];
          const value = dd[1];

          element[field.trim()] = value;
        });

        element['chiave'] = f.properties.id;
        element['Geometry'] = this.wktService.geometryToWKT(this.featureInformationService.getGeometry(f.properties.id));
      });
    }

    return data;
  }
}

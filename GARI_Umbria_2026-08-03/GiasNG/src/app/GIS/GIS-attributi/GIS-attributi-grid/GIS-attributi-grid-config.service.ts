import { process } from '@progress/kendo-data-query';
import { Injectable, Injector } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { enum_GISDrawingOperations } from 'app/GIS/GIS-enum/GIS-drawing-operations';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { DrawWindowOperationService } from 'app/GIS/GIS-kendo-window/draw-window/draw-window-operation.service';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { EditFeatureService } from 'app/GIS/services/edit-feature.service';
import { PermessiFeature, FeatureService } from 'app/GIS/services/feature.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { FeatureType } from 'app/Model/GIS/GisDataReadRval_New';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { TipologiaLayer, GisClient, GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from 'app/Service/api.service';
import { getServiceIdAndLog } from 'app/Service/utils';
import { AgrSelectableSettings, ToolbarSettings, CommandsColumnSettings, PaginationSettings, CustomColumnSettings } from 'gias-kendo-grid';
import { KendoGridColumn, ModelEntry,  KendoGridRow, EditingMode, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { TreeGisService } from 'app/Utility/Template/kendo-tree/services/tree-gis.service';
import { Subscription, Subject, Observable, tap, forkJoin, map } from 'rxjs';
import { GISAttributiKendoServerResult, KendoGISAppuntiModel } from './GIS-attributi-grid.model';
import { GISAttributiEventsService } from './GIS-attributi-grid-events.service';
import { FormControl } from '@angular/forms';
import { TreeNode } from '../../../Utility/Template/kendo-tree/model';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';
import { GISAttributiComponent } from '../GIS-attributi.component';

export function forbiddenCharactersValidator(input: FormControl): any {
  if (input.getRawValue().indexOf('§') != -1 || input.getRawValue().indexOf('|') != -1)
    return { 'character': true };
  else return null;
}

@Injectable()
export class GISAttributiConfigService extends AbstractGridConfigService<GISAttributiKendoServerResult>{

  gridId: string = `GISAttributi`;
  rowId: string = 'chiave';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;

  public readIsLoading: Subject<boolean> = new Subject<boolean>();

  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'Descrizione', title: this.translocoService.translate('Descrizione') },
      { resizable: true, editable: true, validators: [forbiddenCharactersValidator] }
    )
  ];

  private kendoModel: KendoGISAppuntiModel = {
    chiave: new ModelEntry(CELL_TYPES.STRING, false),
    Descrizione: new ModelEntry(CELL_TYPES.STRING, false)
  };

  private kendoRows: KendoGridRow[] = [];

  private subs: Subscription = new Subscription();
  private eventListener_Remove: google.maps.MapsEventListener;

  private kendoLayer: any;
  private layerItemSelected: [TipologiaLayer, boolean];
  private layerTypeSelected: string = '';
  private permessi: PermessiFeature = new PermessiFeature();

  private ultimaFeatureSelezionata: GeoJson_Feature_New_1OfGeoJSONAgroGisProp;

  private serviceId = null;

  constructor(
    injector: Injector,
    private featureService: FeatureService,
    private sharedDataService: SharedDataService,
    private layerService: LayerService,
    private translocoService: TranslocoService,
    private gisClient: GisClient,
    private gisAttributiGridEventsService: GISAttributiEventsService,
    private editFeatureService: EditFeatureService,
    private kendoWindowService: KendoWindowsService,
    private drawWindowOperationService: DrawWindowOperationService,
    private treeGisService: TreeGisService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private featureInformationService: FeatureInformationService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.serviceId = getServiceIdAndLog('GISAttributiConfigService', 'constructor');

    let featureSelezionate = this.featureService.getFeatureSelezionate();
    this.ultimaFeatureSelezionata = featureSelezionate[featureSelezionate.length - 1];

    this.subs.add(this.featureService.getFeatureSelezionate$()
      .subscribe(featureSelezionate => {
        const indiceUltimaFeature = featureSelezionate.length - 1;
        this.ultimaFeatureSelezionata = featureSelezionate[indiceUltimaFeature];

        if (this.ultimaFeatureSelezionata != undefined) {
          this.permessi = this.featureService.getPermessiFeature(this.ultimaFeatureSelezionata);

          if (this.layerTypeSelected != this.ultimaFeatureSelezionata?.properties.layer) {
            this.layerTypeSelected = this.ultimaFeatureSelezionata?.properties.layer;

            // this.gridId = `GISAttributi${this.layerTypeSelected}`;
            this.gridPublicService.refresh(true);
            this.handleCustomizations();
          }
        }

        // la riga sottostante fa si che alla selezione di una feature venga selezionata la riga corrispondente
        this.selectRow(<string>this.ultimaFeatureSelezionata?.properties.id);
      })
    );

    this.layerItemSelected = this.layerService.layerItemSelected;
    this.layerTypeSelected = this.layerItemSelected[0]?.id;
    this.permessi = this.sharedDataService.getPermessiLayer(this.layerItemSelected[0]);

    this.subs.add(this.gridPublicService.changeDetected.subscribe((c) => {
      if (c['action'] == 'cancel') {
        this.drawWindowOperationService.setOperation(enum_GISDrawingOperations.none);

        if (Number.parseInt(this.layerService.layerItemSelected[0].FeatureTypeId) == FeatureType.Polygon)
          this.gisAttributiGridEventsService.cancelPolygon();
        if (Number.parseInt(this.layerService.layerItemSelected[0].FeatureTypeId) == FeatureType.Point)
          this.gisAttributiGridEventsService.cancelMarker();
        if (Number.parseInt(this.layerService.layerItemSelected[0].FeatureTypeId) == FeatureType.LineString)
          this.gisAttributiGridEventsService.cancelPolyline();
      }
    }));

    this.subs.add(this.sharedDataService.geoJsonLoaded.subscribe(v => {
      this.gridPublicService.refresh(true);
    }));

    this.subs.add(this.editFeatureService.editFeature.subscribe(l => {
      this.selectRow(<string>l.getId());
      this.onEditFeature();
    }));

    this.subs.add(this.gridPublicService.selection.getSelectedValue.subscribe(r => {
      this.gisAttributiGridEventsService.selectFeature(r?.value.selectedRows[0].dataItem.chiave);
    }));

    this.subs.add(this.gridPublicService.filters.subscribe(c => {
      let filteredRows: any[] = process(this.kendoRows, { filter: c }).data;

      if (filteredRows.length > 0)
        this.googleMapGeoJsonService.setMapCenterOfPassedFeatures(filteredRows.map(fr => fr.chiave));
    }));

    this.handleCustomizations();
  }

  perform(action: HttpAction, items: any): Observable<any[]> {
    if (action == HttpAction.CREATE) {
      this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
      return this.gisAttributiGridEventsService
        .addItem(items)
        .pipe(tap((risultato) => {
          if (risultato == null) {
            return;
          }
          let datiFeature = this.sharedDataService.getDatiFeatureConAttributi();
          datiFeature.EntitaCod = risultato.RispostaStringa.ListaEntitaInserite[0];
          datiFeature.DatiCompleti = true;
          this.sharedDataService.setDatiFeatureConAttributi(datiFeature);
          this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
          this.sharedDataService.closeAttributiTable.next('');
          this.kendoWindowService.close(WindowTypes.MarkerWindow);
          this.drawWindowOperationService.setOperation(enum_GISDrawingOperations.none);
        }));
    }
    if (action == HttpAction.UPDATE) {
      this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
      return this.gisAttributiGridEventsService.updateItem(items).pipe(tap((risultato) => {
        let datiFeature = this.sharedDataService.getDatiFeatureConAttributi();
        datiFeature.DatiCompleti = true;
        this.sharedDataService.setDatiFeatureConAttributi(datiFeature);
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
        this.sharedDataService.closeAttributiTable.next('');
        this.kendoWindowService.close(WindowTypes.MarkerWindow);
        this.drawWindowOperationService.setOperation(enum_GISDrawingOperations.none);
      }));
    }
  }

  read(options?: any): Observable<GISAttributiKendoServerResult> {
    this.readIsLoading.next(true);
    let obs: Observable<any>[] = [];
    if (this.layerTypeSelected != undefined && GISAttributiComponent.isDatiVisible(this.layerTypeSelected as enum_LayerElementiGraficiStd)) {
      this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
      obs = [this.gisClient.gisLeggiElencoStrutturaAttributiLayer(this.layerTypeSelected)];
    }

    return forkJoin(obs).pipe(map(result => {
      this.kendoLayer = JSON.parse(result[0].RispostaStringa.KendoGridAttributiLayer);
      this.kendoRows = <Array<any>>(this.kendoLayer?.kendo_rows);

      let tableData;
      if (this.kendoLayer?.kendo_model == undefined || this.kendoLayer?.kendo_columns == undefined) {
        this.extractDescrizioneFromFeatures();

        tableData = new GISAttributiKendoServerResult(
          this.kendoModel,
          this.kendoColumns,
          this.kendoRows
        );
      } else {
        this.kendoLayer.kendo_model['chiave'] = { editable: false, type: 'string' };

        this.kendoModel = this.kendoLayer.kendo_model;
        this.kendoColumns = this.kendoLayer.kendo_columns.map(c =>
          new KendoGridColumn(
            { field: c.field, title: c.title },
            { resizable: true, editable: true, width: 150, validators: [forbiddenCharactersValidator] }
          )
        );
        this.extractDataFromFeatures();

        tableData = new GISAttributiKendoServerResult(
          this.kendoModel,
          this.kendoColumns,
          this.kendoRows
        );
      }

      this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
      this.readIsLoading.next(false);
      return tableData;
    }));
  }

  public selectRow(chiave: string): void {
    if (this.gridPublicService.filters.getValue() == undefined || this.gridPublicService.filters.getValue().filters.length == 0) {
      this.gridPublicService.selection.setSelected.next({ keys: [chiave], resetPreviousSelection: true });
      this.gridPublicService.giasGridComponent?.grid.scrollTo({
        row: this.gridPublicService.giasGridComponent.rows.findIndex(r => r['chiave'] == chiave)
      });
    }
  }

  public addItem(): void {
    this.gridPublicService.gridComp.add.next({ sender: this.gridPublicService.gridComp, dataItem: undefined, isNew: true, rowIndex: 0 })
  }

  public onComponentDestroy(): void {
    google.maps.event.removeListener(this.eventListener_Remove);
    this.subs.unsubscribe();
  }

  public refreshGrid(keepModel?: boolean): void {
    if (this.kendoLayer?.kendo_model == undefined || this.kendoLayer?.kendo_columns == undefined)
      this.extractDescrizioneFromFeatures();
    else
      this.extractDataFromFeatures();

    if (keepModel)
      this.gridPublicService.refresh(
        false,
        {
          model: this.kendoModel,
          columns: this.kendoColumns,
          rows: this.kendoRows
        }
      );
    else
      this.gridPublicService.refresh(true);
  }

  private getTreeKey(): string {
    let checkedKeys: Array<any> = this.sharedDataService.getTreeViewCheckedKeys();
    if (checkedKeys.length < 2) {
      if (this.ultimaFeatureSelezionata) {
        return this.ultimaFeatureSelezionata?.properties.chiavealbero;
      } else if (checkedKeys.length == 1) {
        let node: TreeNode = this.treeGisService.getTreeNodeFromCheckedKey(checkedKeys[0]);
        return node.id;
      }
    }
    return '';
  }

  private handleCustomizations(): void {
    this.pagination = this.handlePagination();

    const permessoEdit: boolean = this.permessi.modifica;
    const permessoRemove: boolean = false; //this.permessi.cancellazione;
    const permessoInserimento: boolean = false; //this.permessi.inserimento;

    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = false;
    this.selectable.selectable.enabled = true;
    this.selectable.shouldShowCheckbox = false;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false; //permessoInserimento;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: permessoEdit,
      infoBtn: false,
      removeBtn: permessoRemove,
      onDisableInfoBtn: () => false
    });

    this.resizable.autoFitColumns = false;
    this.resizable.isResizable = true;

    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.toolbar.newItem = false; //permessoInserimento;
      this.cmdColumn.editBtn = permessoEdit;
      this.groups.groupable.enabled = false;
      this.views.enabled = false;
    }

    this.customColumn = new CustomColumnSettings({ showColumn: true, width: 50, useCustomColumnCellTemplate: true });
  }

  private handlePagination(): PaginationSettings {
    const result: PaginationSettings = new PaginationSettings();
    result.gridState = {
      sort: [],
      skip: 0,
      group: [],
      take: 5,
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
        text: this.translocoService.translate('Tutti'),
        value: "all",
      } as any as number]
    };
    result.navigable = false;

    return result;
  }

  private extractDataFromFeatures(): void {
    const features = this.featureInformationService.getByLayer(this.layerTypeSelected);
    let rows: Array<any> = [];

    features.forEach(f => {
      let row = <any>{};
      let dataString: string = f.properties.AppIdRate;

      dataString.split('|').forEach(d => {
        let dd: Array<string> = d.split('§');
        let field: string = dd[0];
        let value: string = dd[1];

        row[field] = value;
      });
      row['chiave'] = f.properties.id;

      rows.push(row);
    });

    this.kendoRows = rows;
  }

  private extractDescrizioneFromFeatures(): void {
    let features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[];
    if (
      this.ultimaFeatureSelezionata?.properties.layer == enum_LayerElementiGraficiStd.CAMPIONAMENTI ||
      this.layerService.layerItemSelected[0].id == enum_LayerElementiGraficiStd.CAMPIONAMENTI
    ) {
      features = this.featureInformationService.getByNode(this.layerTypeSelected, this.getTreeKey());
    } else {
      features = this.featureInformationService.getByLayer(this.layerTypeSelected);
    }

    let rows: Array<any> = [];
    features.forEach(f => {
      let row = <any>{};
      let dataString: string = f.properties.Testo;

      row['Descrizione'] = dataString;
      row['chiave'] = f.properties.id;

      rows.push(row);
    });

    this.kendoRows = rows;
  }

  private onEditFeature(): void {
    const dataItem: KendoGridRow = this.gridPublicService.giasGridComponent.rows.find(
      r => r['chiave'] == this.ultimaFeatureSelezionata.properties.id
    );

    const idx: number = this.gridPublicService.giasGridComponent.rows.findIndex(
      r => r['chiave'] == this.ultimaFeatureSelezionata.properties.id
    );

    this.gridPublicService.giasGridComponent.editHandler(
      { dataItem: dataItem, isNew: false, rowIndex: idx, sender: this.gridPublicService.giasGridComponent.grid }
    );
  }
}

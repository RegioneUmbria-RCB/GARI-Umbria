import { Component, OnInit, OnDestroy, Inject } from "@angular/core";
import { enum_FeatureProperty, enum_FeatureGeometryType } from "app/GIS/GIS-enum/GIS-feature";
import { enum_LayerElementiGraficiStd } from "app/GIS/GIS-enum/GIS-layer-elementi-grafici";
import { GoogleMapGeoJsonService } from "app/GIS/google-map/google-map-geojson.service";
import { DrawingManagerService } from "app/GIS/services/drawing-manager.service";
import { DrawingService } from "app/GIS/services/drawing.service";
import { FeatureService } from "app/GIS/services/feature.service";
import { GisAttributiService, ReadTecniciParams } from "app/GIS/services/gis-attributi.service";
import { LayerService } from "app/GIS/services/layer.service";
import { PermessiLayer, SharedDataService } from "app/GIS/services/shared-data.service";
import { FeatureType } from "app/Model/GIS/GisDataReadRval_New";
import { FiltroTemporale, FiltroTemporale_enum_OperatoreFiltroTemporale, FiltroTemporale_enum_TipoFiltroTemporale, GisClient, TipologiaLayer } from "app/Service/api.service";
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { TreeGisFiltersService } from "app/Utility/Template/kendo-tree/filters/gis-tree-filters.service";
import { Subscription, combineLatest, debounceTime, filter, map, of, switchMap } from "rxjs";
import { GISAttributiConfigService } from "./GIS-attributi-grid-config.service";
import { GISAttributiEventsService } from "./GIS-attributi-grid-events.service";
import { Dialog_Type, GiasDialogService } from "app/Service/gias-dialog.service";
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { TranslocoService } from "@jsverse/transloco";
import { enum_TipologiaLayer } from "app/GIS/GIS-enum/GIS-tipologia-layer";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { enum_OrigineChiamataLoadGeoJson } from "app/GIS/GIS-enum/GIS-origine-chiamata";
import { AGRODATAFINE, AGRODATAINIZIO } from "gias-ui-kit";

@Component({
  standalone: false,
  selector: 'GIS-attributi-grid',
  templateUrl: './GIS-attributi-grid.component.html',
  styleUrls: ['./GIS-attributi-grid.component.css'],
  providers: [
    ...generateGridProviders(GISAttributiConfigService, GISAttributiGridComponent),
    GisAttributiService,
    GISAttributiEventsService
  ]
})
export class GISAttributiGridComponent implements OnInit, OnDestroy {
  public permessi: PermessiLayer;
  public enableDrawing: boolean;
  public isTecnici: boolean;
  public filtroTemporale = {
    TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale,
    DataInizio: new Date(AGRODATAINIZIO),
    DataFine: new Date(AGRODATAFINE),
    TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
    TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,
  } as FiltroTemporale;

  public isDeleteAllLayerDataButtonVisible$ = combineLatest([
    this.layerService.layerItemSelected$,
    this.layerService.LayerSelected.asObservable()
  ])
    .pipe(
      debounceTime(100),
      switchMap(([[layer, selected], layerType]) => {
        if (
          !selected
          || layer == null
          || layerType.Option_Value != enum_TipologiaLayer.Entita
          || +layer.FeatureTypeId == FeatureType.Raster
          || !FunzioniComuniService.isCustomLayer(+layer.id)
        ) {
          return of(false);
        }

        const user = this.permessiUtenteService.getCurrentUser();
        return this.gisClient.gisPermessiUtenteSuSingoloLayer({ LayerElementiGrafici_Cod: +layer.id, user: user.Username })
          .pipe(map(res => res.RispostaStringa.utenti_permessi.find(p => p.Flag_Cancellazione == 1) != null));
      }),
    );

  private subs: Subscription = new Subscription();

  constructor(
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private featureService: FeatureService,
    private drawingManagerService: DrawingManagerService,
    private drawingService: DrawingService,
    private layerService: LayerService,
    private sharedDataService: SharedDataService,
    @Inject(GRID_HTTP_TOKEN) private gridhttpService: GISAttributiConfigService,
    private gisAttributiEventsService: GISAttributiEventsService,
    private gisTreeFilterService: TreeGisFiltersService,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService,
    private permessiUtenteService: PermessiUtenteService,
    private gisClient: GisClient
  ) { }

  ngOnInit(): void {
    this.permessi = this.sharedDataService.getPermessiLayer(this.getLayer());
    this.enableDrawing = this.isDrawingEnabled();
    this.isTecnici = this.sharedDataService.getLayerSelezionatoSourceAsValue()?.id == enum_LayerElementiGraficiStd.TECNICIINCAMPO;

    this.subs.add(this.gridhttpService.readIsLoading.subscribe(r => {
      if (!r) this.setFilterDates();
    }));

    this.subs.add(this.drawingService.addEvent.subscribe(d => {
      this.gridhttpService.addItem();
    }));

    let centri = this.gisTreeFilterService.getValue();
    if (!centri || centri.length == 0) {
      this.gisTreeFilterService.caricaInteroAlberoConFiltri();
    }

    this.gridhttpService.gridId = `GISAttributi${this.layerService.layerItemSelected[0]?.id}|${this.layerService.layerItemSelected[0]?.FeatureTypeId}`;
  }

  private getLayer(): TipologiaLayer {
    if (this.layerService.layerItemSelected[0])
      return this.layerService.layerItemSelected[0];
    return this.sharedDataService.getTipologiaLayerById(
      this.featureService.getFeatureSelezionate()[this.featureService.getFeatureSelezionate().length - 1]
        .properties.layer
    );
  }

  private isDrawingEnabled(): boolean {
    if (this.layerService.layerItemSelected[0])
      return (
        Number.parseInt(this.getLayer().FeatureTypeId) == FeatureType.Polygon ||
        Number.parseInt(this.getLayer().FeatureTypeId) == FeatureType.Point ||
        Number.parseInt(this.getLayer().FeatureTypeId) == FeatureType.LineString
      );
    if (this.featureService.getFeatureSelezionate().length > 0)
      return (
        this.featureService.getFeatureSelezionate()[this.featureService.getFeatureSelezionate().length - 1]
          .properties.TipologiaGML == enum_FeatureGeometryType.Polygon ||
        this.featureService.getFeatureSelezionate()[this.featureService.getFeatureSelezionate().length - 1]
          .properties.TipologiaGML == enum_FeatureGeometryType.Point ||
        this.featureService.getFeatureSelezionate()[this.featureService.getFeatureSelezionate().length - 1]
          .properties.TipologiaGML == enum_FeatureGeometryType.LineString
      );
    return false;
  }

  ngOnDestroy(): void {
    if (this.isTecnici)
      this.googleMapGeoJsonService.removeFeaturesOfSameLayer(enum_LayerElementiGraficiStd.TECNICIINCAMPO);
    this.gridhttpService.onComponentDestroy();
    this.subs.unsubscribe();
  }

  public get getGridId(): string {
    return this.gridhttpService.gridId;
  }

  public focusOnFeature(e: Event, dataItem: any): void {
    this.gisAttributiEventsService.focusOnFeature(dataItem.chiave);
  }

  public findSelecetedItem(): void {
    const feature = this.featureService.getUltimaFeatureSelezionata();
    if (feature != null) {
      this.gridhttpService.selectRow(feature.properties.id);
    }
  }

  public disegnaLayer(): void {
    let layer = this.getLayer();

    if (Number.parseInt(layer.FeatureTypeId) == FeatureType.Polygon)
      this.drawingManagerService.setDrawing_Modes(google.maps.drawing.OverlayType.POLYGON);
    if (Number.parseInt(layer.FeatureTypeId) == FeatureType.Point)
      this.drawingManagerService.setDrawing_Modes(google.maps.drawing.OverlayType.MARKER);
    if (Number.parseInt(layer.FeatureTypeId) == FeatureType.LineString)
      this.drawingManagerService.setDrawing_Modes(google.maps.drawing.OverlayType.POLYLINE);

    this.drawingManagerService.setLayer(layer);
    this.drawingManagerService.startDrawingMode();
  }

  public setStartDate(d: Date): void {
    this.filtroTemporale.DataInizio = d;
  }

  public setEndDate(d: Date): void {
    this.filtroTemporale.DataFine = d;
  }

  public loadTecnici(): void {
    this.gisAttributiEventsService
      .readTecnici(this.getReadParamsTecnici())
      .subscribe(r => {
        this.removeFeatureOfLayer(this.layerService.layerItemSelected[0]);
        if (r.RispostaStringa.myGeoJson?.geoJsonCaricato != null) {
          this.googleMapGeoJsonService.addGeoJsonPolyLabelsAndClusterer(r.RispostaStringa.myGeoJson, false);
        }

        this.gridhttpService.refreshGrid(true);
      });
  }

  public deleteAllLayerData(): void {
    const [layer, selected] = this.layerService.layerItemSelected;
    if (!selected || layer == null) {
      return;
    }

    const title = this.translocoService.translate("gis.EliminazioneDatiLayer");
    this.giasDialogService
      .dialogMessageObs_Result(title, this.translocoService.translate("gis.EliminareTuttiIDatiRelativiAQuestoLayer"), undefined, undefined, undefined, undefined, Dialog_Type.warning)
      .pipe(
        filter((x: any) => x.returnObj),
        switchMap(() => this.gisClient.gisEliminazioneTotaleDatiLayer(+layer.id))
      )
      .subscribe({
        next: () => this.giasDialogService.baseSuccess(title, this.translocoService.translate("gis.TuttiIDatiSonoStatiEliminatiCorrettamente"), false),
        error: error => this.giasDialogService.baseError(title, FunzioniComuniService.getResponseError(error, this.translocoService), false),
        complete: () => this.reloadData()
      });
  }

  private getReadParamsTecnici(): ReadTecniciParams {
    return new ReadTecniciParams(this.filtroTemporale.DataInizio, this.filtroTemporale.DataFine, true);
  }

  private setFilterDates() {
    if (this.isTecnici) {
      this.setStartDate(new Date(new Date().setDate(new Date().getDate() - 1)));
      this.setEndDate(new Date());
      this.loadTecnici();
    }
  }

  private removeFeatureOfLayer(layer: TipologiaLayer) {
    this.googleMapGeoJsonService.removeFeaturesOfSameLayer(layer.id)
  }

  private reloadData(): void {
    this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.CentraSuAzienda);
    this.googleMapGeoJsonService.loadGeoJsonForzato(true);
  }
}

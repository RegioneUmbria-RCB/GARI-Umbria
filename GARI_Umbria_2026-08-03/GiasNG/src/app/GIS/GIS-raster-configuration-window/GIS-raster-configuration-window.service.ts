import { Injectable } from '@angular/core';
import { GisClient, MascheraLayerRaster, RispostaStandard_1OfElencoMaschereLayerRaster } from 'app/Service/api.service';
import { BehaviorSubject, Observable, Subject, Subscription, combineLatest, filter, map, startWith, tap, withLatestFrom } from 'rxjs';
import { enum_TipologiaLayer } from '../GIS-enum/GIS-tipologia-layer';
import { GoogleMapService } from '../google-map/google-map.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { RasterOverlayService, RasterParameterVisualizationLayer, RasterParameterVisualizationType } from '../services/raster-overlay.service';
import { FeatureService } from '../services/feature.service';
import { TranslocoService } from '@jsverse/transloco';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { NotificationRef } from '@progress/kendo-angular-notification';
import { GoogleMapUtils } from '../utils/google-map.utils';
import { FeatureInformationService } from '../services/feature-information.service';
import { LayerService } from '../services/layer.service';
import { SharedDataService } from '../services/shared-data.service';

const DEFAULT_VIEW = 'DefaultView';

@Injectable()
export class GISRasterConfigurationWindowService {
  private created = new Set<number>();
  private visibilitySubject = new Map<number, BehaviorSubject<boolean>>();
  private opacitySubject = new Map<number, BehaviorSubject<number>>();
  private clickInfoSubject = new Map<number, BehaviorSubject<boolean>>();
  private masksSubject = new Map<number, BehaviorSubject<MascheraLayerRaster[]>>();
  private loaderSubscritions = new Map<number, Subscription>();
  private mapClickEventSubject = new Map<number, Subject<google.maps.Data.MouseEvent>>();
  private currentVisibleSubject = new BehaviorSubject<number | null>(null);

  private rasterActiveTitleBarRef: NotificationRef | null = null;

  constructor(
    private gisClient: GisClient,
    private googleMapService: GoogleMapService,
    private permessiUtenteService: PermessiUtenteService,
    private rasterOverlayService: RasterOverlayService,
    private featureService: FeatureService,
    private translocoService: TranslocoService,
    private giasMessageService: GiasMessageService,
    private featureInformationService: FeatureInformationService,
    private layerService: LayerService,
    private sharedDataService: SharedDataService
  ) { }

  public get isAnyMapInfoActive(): boolean {
    for (const rasterId of this.created) {
      if (this.visibilitySubject.get(rasterId).value && this.clickInfoSubject.get(rasterId).value) {
        return true;
      }
    }

    return false;
  }

  public getVisibility$(rasterId: number): Observable<boolean> {
    this.createDefaultSubjects(rasterId);
    return this.visibilitySubject.get(rasterId).asObservable();
  }

  public nextVisibility(rasterId: number, value: boolean): void {
    this.createDefaultSubjects(rasterId);
    this.visibilitySubject.get(rasterId).next(value);

    if (!value) {
      this.currentVisibleSubject.next(null);
      if (!this.loaderSubscritions.has(rasterId)) {
        return;
      }

      RasterOverlayService.clearRasterOverlays(rasterId, this.googleMapService.googleMapWrapper.data.getMap());
      const subscription = this.loaderSubscritions.get(rasterId);
      this.stopLoader(subscription);
      this.loaderSubscritions.delete(rasterId);

      return;
    }

    const title = this.translocoService.translate('gis.VisualizzazioneDeiDatiRasterAttiva');
    this.rasterActiveTitleBarRef = this.giasMessageService.customMessage(title, '', '', () => {
      this.nextVisibility(rasterId, false);
      const layer = this.sharedDataService.getTipologiaLayerById(rasterId.toString());
      this.layerService.toggleLayerItemVisible(layer, false);
    }, false);

    const subscription = this.runLoader(rasterId);
    this.loaderSubscritions.set(rasterId, subscription);
    this.currentVisibleSubject.next(rasterId);
  }

  public getCurrentVisible(): number | null {
    return this.currentVisibleSubject.value;
  }

  public isSomeVisible(): boolean {
    const visibles = this.visibilitySubject.values();
    for (const visible of visibles) {
      if (visible.value) {
        return true;
      }
    }

    return false;
  }

  public getOpacity$(rasterId: number): Observable<number> {
    this.createDefaultSubjects(rasterId);
    return this.opacitySubject.get(rasterId).asObservable();
  }

  public nextOpacity(rasterId: number, value: number): void {
    this.createDefaultSubjects(rasterId);
    this.opacitySubject.get(rasterId).next(value);
  }

  public getClickInfo$(rasterId: number): Observable<boolean> {
    this.createDefaultSubjects(rasterId);
    return this.clickInfoSubject.get(rasterId).asObservable();
  }

  public nextMapClickInfo(rasterId: number, value: boolean) {
    this.createDefaultSubjects(rasterId);
    this.clickInfoSubject.get(rasterId).next(value);
  }

  public getMasks$(rasterId: number): Observable<MascheraLayerRaster[]> {
    this.createDefaultSubjects(rasterId);
    return this.masksSubject.get(rasterId).asObservable();
  }

  public fetchMasks(rasterId: number): Observable<RispostaStandard_1OfElencoMaschereLayerRaster> {
    return this.gisClient
      .gisLeggiMaschereLayerRaster({ TipologiaLayer_Raster_Cod: +enum_TipologiaLayer.Entita, LayerElementiGrafici_Raster_Cod: rasterId })
      .pipe(tap(res => {
        this.createDefaultSubjects(rasterId);
        this.masksSubject.get(rasterId).next(res.RispostaStringa.elencoMaschere);
      }));
  }

  public isFeatureMaskerable(layerId: number): boolean {
    // For all masks in rasters
    const allMasks = this.masksSubject.entries();
    for (const [rasterId, masks] of allMasks) {

      // Check only visible rasters
      if (!this.visibilitySubject.get(rasterId).value) {
        continue;
      }

      for (const mask of masks.value) {

        // Return true if features is in maskerable layer
        if (mask.LayerElementiGrafici_cod == layerId && mask.isAttivaPerUtenteCorrente) {
          return true;
        }
      }
    }
    return false;
  }

  public stopAllLoders(): void {
    for (const [rasterId, subscription] of this.loaderSubscritions.entries()) {
      RasterOverlayService.clearRasterOverlays(rasterId, this.googleMapService.googleMapWrapper.data.getMap());
      this.stopLoader(subscription);
    }

    this.loaderSubscritions.clear();
  }

  public nextMapClickEvent(event: google.maps.Data.MouseEvent): void {
    const rasterId = this.getCurrentVisible();
    if (rasterId != null) {
      this.mapClickEventSubject.get(rasterId).next(event);
    }
  }

  public getMapClickEvent$(rasterId: number): Observable<google.maps.Data.MouseEvent> {
    return this.mapClickEventSubject.get(rasterId).asObservable()
      .pipe(
        withLatestFrom(this.getVisibility$(rasterId)),
        filter(([_, visible]) => visible),
        withLatestFrom(this.getClickInfo$(rasterId)),
        filter(([_, enabled]) => enabled),
        map(([[event, _1], _2]) => event)
      );
  }

  public get currentVisible$(): Observable<number | null> {
    return this.currentVisibleSubject.asObservable();
  }

  private stopLoader(subscription: Subscription): void {
    subscription.unsubscribe();
    if (this.rasterActiveTitleBarRef != null) {
      this.rasterActiveTitleBarRef.hide();
    }
  }

  private createDefaultSubjects(rasterId: number): void {
    if (!this.created.has(rasterId)) {
      this.visibilitySubject.set(rasterId, new BehaviorSubject<boolean>(false));
      this.opacitySubject.set(rasterId, new BehaviorSubject<number>(1));
      this.clickInfoSubject.set(rasterId, new BehaviorSubject<boolean>(false));
      this.masksSubject.set(rasterId, new BehaviorSubject<MascheraLayerRaster[]>([]));
      this.mapClickEventSubject.set(rasterId, new Subject<google.maps.Data.MouseEvent>());
      this.created.add(rasterId);

      this.fetchMasks(rasterId).subscribe();
    }
  }

  private runLoader(rasterId: number): Subscription {
    const features$ = this.featureService.getFeatureSelezionate$();
    const opacity$ = this.getOpacity$(rasterId).pipe(tap(opacity => this.rasterOverlayService.setOpacity(rasterId, opacity)));

    return combineLatest([
      this.googleMapService.googleMapWrapper.idle.pipe(startWith(null)),
      features$
    ])
      .pipe(
        map(([_1, features]) => [features]),
        withLatestFrom(this.getMasks$(rasterId), opacity$)
      )
      .subscribe({
        next: ([[features], masks, opacity]) => {
          const hasPermissions = this.permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_Gestione_Parametri_Maschere_Raster, 0);
          const gmap = this.googleMapService.googleMapWrapper.data.getMap();
          const layerParams = this.getLayerParams(gmap, rasterId);
          this.rasterOverlayService.load(rasterId, masks, layerParams, features, hasPermissions, opacity);
        }
      });
  }

  private getLayerParams(gmap: google.maps.Map, rasterId: number): RasterParameterVisualizationLayer[] {
    const layerParams: RasterParameterVisualizationLayer[] = [];

    this.featureInformationService.getByLayer(rasterId.toString()).forEach(feature => {
      const geometry = this.featureInformationService.getGeometry(feature.properties.id);

      if (!GoogleMapUtils.isPolygonInViewport(gmap, geometry as google.maps.Data.Polygon)) {
        return;
      }

      const params = feature.properties.ParametriVisualizzazioneLayer;
      if (params == null || params == "") {
        return;
      }

      // ParametriVisualizzazioneLayer example
      // const params = '{ "type" : 5 , "baseUrl" : "https://geoproxy-qvrusott6a-ew.a.run.app/t/xyz/services/srv-cog-3857", "bucket": "hub-cloud-staging_test_bucket", "obj": "mapTilesForEE","views":["DefaultView"],"gsUriFile":"gs://gee_objects_private/mapTilesForEE/7419ac39-b881-4105-9f3b-e7290fbf9e32/7419ac39-b881-4105-9f3b-e7290fbf9e32.tif"}';

      const guid = feature.properties.Entita_GUID;
      if (guid == null || guid == '') {
        return;
      }

      const param = JSON.parse(params) as RasterParameterVisualizationLayer;
      const view = param.views == null || param.views.length == 0 ? DEFAULT_VIEW : param.views[0]; // NDVI, NDWI...
      if (param.type == RasterParameterVisualizationType.PRIVATE) {
        layerParams.push({ type: param.type, baseUrl: null, bucket: param.bucket, obj: `${param.obj}/${guid}/${view}` } as RasterParameterVisualizationLayer);
      } else if (param.type == RasterParameterVisualizationType.SATELLITE) {
        layerParams.push({ type: param.type, baseUrl: `${param.baseUrl.replace(/\/$/, '')}/${param.bucket}/${param.obj}/${guid}/${view}`, bucket: null, obj: null } as RasterParameterVisualizationLayer);
      } else if (param.type == RasterParameterVisualizationType.PUBLIC) {
        layerParams.push({ type: param.type, baseUrl: `${param.baseUrl.replace(/\/$/, '')}/${param.bucket}/${param.obj}/${guid}/${view}`, bucket: null, obj: null } as RasterParameterVisualizationLayer);
      } else if (param.type == RasterParameterVisualizationType.ABACO_PRIVATE) {
        layerParams.push({ type: param.type, baseUrl: null, bucket: param.bucket, obj: `${param.obj}/${guid}/3857/${guid}`, views: param.views, guid: guid } as RasterParameterVisualizationLayer);
      } else if (param.type == RasterParameterVisualizationType.ABACO_PUBLIC) {
        layerParams.push({ type: param.type, baseUrl: `${param.baseUrl.replace(/\/$/, '')}/${param.bucket}/${param.obj}/${guid}/3857/${guid}.tif`, bucket: null, obj: null, views: param.views, guid: guid } as RasterParameterVisualizationLayer);
      }
    });

    return layerParams;
  }
}

import { Injectable, Optional } from '@angular/core';
import { FeatureType } from 'app/Model/GIS/GisDataReadRval_New';
import { guid } from '@progress/kendo-angular-common';
import { BackendCluster, Cluster, CLUSTER_ICON_SIZE, CLUSTER_MARKER_URL, FrontendCluster } from '../../google-map/cluster';
import { GoogleMapUtils } from '../../utils/google-map.utils';
import { PolygonLabelInfowindowService } from '../polygon-label-infowindow.service';
import { PolygonLabelService } from '../polygon-label.service';
import { InfowindowClustererService } from '../../infowindow-clusterer/infowindow-clusterer.service';
import { GoogleMapFeatureService } from './google-map-feature.service';
import { FeatureInformationService } from '../feature-information.service';
import { CfgAlbero_CfgGisUtente, GeoJson_Feature_New_1OfGeoJSONAgroGisProp, GeoJson_Shape_New_1OfGeoJSONAgroGisProp, TipologiaLayer } from 'app/Service/api.service';
import { Observable, ReplaySubject } from 'rxjs';
import { EditFeatureWindowService } from '../edit-feature-window.service';
import { GoogleMapHeatmapService } from './google-map-heatmap.service';
import { SharedDataService } from '../shared-data.service';
import { enum_ClusteringLevel } from 'app/GIS/GIS-enum/GIS-clustering-level';
import { BackEndColor } from 'app/Model/GIS/BackEndColor';

export const GEOJSON_TYPE = 'FeatureCollection';
const MIN_PIXEL_CLUSTER_DISTANCE = 100;
const MIN_RENDERABLE_AREA_SIZE = 5_000_000_000;

@Injectable()
export class GoogleMapGeoJsonLazyService {
  private geoJson: GeoJson_Shape_New_1OfGeoJSONAgroGisProp = { features: [], type: GEOJSON_TYPE };
  private clusters = new Map<string, Cluster>();
  private featureInClusters = new Map<string, string>();
  private featuresLoaded = new Map<string, boolean>();
  private layerVisible = new Map<string, boolean>();
  private previousZoom: number | null = null;
  private loadedSubject = new ReplaySubject<void>(1);
  private clusteringLevel: number;
  private isHeatmapShown: boolean;
  private cfgAlberoGisUtente: CfgAlbero_CfgGisUtente;

  constructor(
    private polygonLabelInfoWindowService: PolygonLabelInfowindowService,
    private polygonLabelService: PolygonLabelService,
    private infoWindowClustererService: InfowindowClustererService,
    private googleMapFeatureService: GoogleMapFeatureService,
    private featureInformationService: FeatureInformationService,
    private editFeatureWindowService: EditFeatureWindowService,
    private sharedDataService: SharedDataService,
    @Optional() private googleMapHeatmapService: GoogleMapHeatmapService
  ) {
    this.cfgAlberoGisUtente = this.sharedDataService.getCfgAlberoGisUtente()[0];
    this.loadClusteringLevel();
    this.initHeatmap();
  }

  public get loaded$(): Observable<void> {
    return this.loadedSubject.asObservable();
  }

  public loadGeoJson(geoJson: GeoJson_Shape_New_1OfGeoJSONAgroGisProp, googleMap: google.maps.Map, fitBounds: boolean, reset: boolean): void {
    const zoom = googleMap.getZoom();
    if (reset) {
      this.reset();
    }

    this.geoJson.features = this.geoJson.features.concat(geoJson.features);
    this.featureInformationService.addFeatures(googleMap.getProjection(), false, ...geoJson.features);
    this.clearClusteringFeatures();

    this.previousZoom = zoom;

    if (fitBounds) {
      const bounds = GoogleMapUtils.getInitialBounds(this.geoJson.features.map(f => this.featureInformationService.getGeometry(f.properties.id)));
      googleMap.fitBounds(bounds);
      googleMap.setCenter(bounds.getCenter());
    } else {
      this.forceMapRefresh(googleMap);
    }

    this.loadedSubject.next();
  }

  public addFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, googleMap: google.maps.Map): void {
    const id = feature.properties.id;

    const alreadyExistingIndex = this.geoJson.features.findIndex(f => f.properties.id == id);
    if (alreadyExistingIndex != -1) {
      this.geoJson.features.splice(alreadyExistingIndex, 1);
      this.googleMapFeatureService.removeFeatures(id);
      this.featuresLoaded.set(id, false);
    }

    this.geoJson.features.push(feature);
    this.featureInformationService.addFeatures(googleMap.getProjection(), false, feature);
    this.editFeatureWindowService.load(feature);
    this.renderGeoJson(googleMap);
  }

  public removeFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, googleMap: google.maps.Map): void {
    const id = feature.properties.id;

    const alreadyExistingIndex = this.geoJson.features.findIndex(f => f.properties.id == id);
    if (alreadyExistingIndex != -1) {
      this.geoJson.features.splice(alreadyExistingIndex, 1);
      this.googleMapFeatureService.removeFeatures(id);
      this.featuresLoaded.set(id, false);
    }

    this.renderGeoJson(googleMap);
  }

  public renderGeoJson(googleMap: google.maps.Map): void {
    const zoom = googleMap.getZoom();
    const viewPort = googleMap.getBounds();

    const visibleFeatures = this.getVisibleFeatures(viewPort);
    const visibleClusters = this.getVisibleClusters(viewPort);
    this.clearLocalClusters(visibleClusters);
    this.googleMapHeatmapService?.clearFeatures();

    if (this.isZoomIn(zoom)) {
      this.handleZoomIn(visibleFeatures, visibleClusters, zoom, googleMap);
    } else if (this.isZoomOut(zoom)) {
      this.handleZoomOut(visibleFeatures, visibleClusters, zoom, googleMap);
    } else {
      this.handlePanning(visibleFeatures, visibleClusters, zoom, googleMap);
    }

    if (this.isHeatmapShown) {
      this.googleMapHeatmapService?.displayHeatmap();
    } else {
      visibleClusters.forEach(cluster => cluster.setMarker(googleMap));
    }

    // This probably can be improved if we have performance issues
    this.setPolyLabelsFromGoogleMapData(zoom, [...this.googleMapFeatureService.getCurrentFeatures()]);
  }

  public renderOnLayerVisibilityChanged(layers: { TipologiaLayer: TipologiaLayer; visible: boolean; }[], config: CfgAlbero_CfgGisUtente, googleMap: google.maps.Map) {
    this.layerVisible.clear();
    this.cfgAlberoGisUtente = config;
    layers.forEach(layer => this.layerVisible.set(layer.TipologiaLayer.id, layer.visible));

    const zoom = googleMap.getZoom();
    const viewPort = googleMap.getBounds();

    const visibleFeatures = this.getVisibleFeatures(viewPort);
    const visibleClusters = this.getVisibleClusters(viewPort);
    this.clearLocalClusters(visibleClusters);
    this.handlePanning(visibleFeatures, visibleClusters, zoom, googleMap);

    // This probably can be improved if we have performance issues
    this.setPolyLabelsFromGoogleMapData(zoom, [...this.googleMapFeatureService.getCurrentFeatures()]);
  }

  private isZoomIn(zoom: number): boolean {
    return zoom > this.previousZoom;
  }

  private isZoomOut(zoom: number): boolean {
    return zoom < this.previousZoom;
  }

  private handleZoomIn(features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], localClusters: Cluster[], zoom: number, googleMap: google.maps.Map): void {
    // While zooming in we can render new features but never remove old ones
    // And we can lose old clusters and create new ones
    const toBeLoaded: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] = [];

    for (const feature of features) {
      const id = feature.properties.id;
      const layerId = feature.properties.layer;

      // Don't handle features already loaded (it cannot be removed since we are zooming in)
      // Don't handle features hidden
      if (this.featuresLoaded.get(id) || (!this.isRasterVisible(layerId) && !this.layerVisible.get(layerId))) {
        continue;
      }

      // If feature is not big enough and we have not zoom in enough, place it in cluster, otherwise render it
      const area = this.featureInformationService.getArea(id);
      const pixelSize = Math.pow(2, -zoom);
      if (zoom < this.clusteringLevel && (area / pixelSize) <= MIN_RENDERABLE_AREA_SIZE) {
        if (this.isHeatmapShown) {
          this.placeInLocalHeatMap(id);
        } else {
          this.placeInLocalCluster(id, zoom, localClusters);
        }
      } else {
        toBeLoaded.push(feature);
        this.featuresLoaded.set(id, true);
      }
    }

    // render features and clusters
    this.loadFeatures(toBeLoaded);

    this.previousZoom = zoom;
  }

  private handleZoomOut(features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], localClusters: Cluster[], zoom: number, googleMap: google.maps.Map): void {
    // While zooming out we cannot render new features but we can remove old ones
    // And we can lose old clusters and create new one

    for (const feature of features) {
      const id = feature.properties.id;
      const layerId = feature.properties.layer;

      // Don't handle features hidden
      if (!this.isRasterVisible(layerId) && !this.layerVisible.get(layerId)) {
        continue;
      }

      // If feature is not big enough and we have not zoom in enough, remove it (if rendered) and cluster it
      const area = this.featureInformationService.getArea(id);
      const pixelSize = Math.pow(2, -zoom)
      if (zoom < this.clusteringLevel && (area / pixelSize) <= MIN_RENDERABLE_AREA_SIZE) {
        if (this.featuresLoaded.get(id)) {
          this.googleMapFeatureService.removeFeatures(id);
          this.featuresLoaded.set(id, false);
        }

        if (this.isHeatmapShown) {
          this.placeInLocalHeatMap(id);
        } else {
          this.placeInLocalCluster(id, zoom, localClusters);
        }
      }
    }

    // only render clusters because we cannot add new features while zooming out

    this.previousZoom = zoom;
  }

  private handlePanning(features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], localClusters: Cluster[], zoom: number, googleMap: google.maps.Map): void {
    // While panning we can both render new features and remove old ones (because zoom-in / out was done somewhere else)
    // And we can lose old clusters and create new ones
    const toBeLoaded: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] = [];

    for (const feature of features) {
      const featureId = feature.properties.id;
      const layerId = feature.properties.layer;

      // Don't handle features hidden
      if (!this.isRasterVisible(layerId) && !this.layerVisible.get(layerId)) {
        continue;
      }

      // If feature is not big enough and we have not zoom in enough, remove it (if rendered) and cluster it
      const area = this.featureInformationService.getArea(featureId);
      const pixelSize = Math.pow(2, -zoom);

      // cluster the feature only if
      // - it is not clustered backend side
      // - the zoom is less than clustering level
      // - the area is less than minimum renderable area size
      if (feature.properties.Clustered == 'True') {
        if (this.isHeatmapShown) {
          this.placeInLocalHeatMap(featureId);
        } else {
          this.placeInBackendCluster(featureId, feature, localClusters);
        }
      } else if (zoom < this.clusteringLevel && (area / pixelSize) <= MIN_RENDERABLE_AREA_SIZE) {
        if (this.featuresLoaded.get(featureId)) {
          this.googleMapFeatureService.removeFeatures(featureId);
          this.featuresLoaded.set(featureId, false);
        }
        if (this.isHeatmapShown) {
          this.placeInLocalHeatMap(featureId);
        } else {
          this.placeInLocalCluster(featureId, zoom, localClusters);
        }
      } else {
        // only render feature if it has not been loaded
        if (!this.featuresLoaded.get(featureId)) {
          toBeLoaded.push(feature);
          this.featuresLoaded.set(featureId, true);
        }
      }
    }

    // render features and clusters
    this.loadFeatures(toBeLoaded);
  }

  private loadFeatures(features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]): void {
    this.googleMapFeatureService.loadByGeoJson({
      type: GEOJSON_TYPE,
      features: features
    });

    for (const feature of features) {
      this.editFeatureWindowService.load(feature);
    }
  }

  private getVisibleFeatures(viewPort: google.maps.LatLngBounds): GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] {
    const visibleFeatures: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] = [];
    for (const feature of this.geoJson.features) {
      const featureId = feature.properties.id;
      const position = this.featureInformationService.getPosition(featureId);
      if (viewPort.contains(position)) {
        visibleFeatures.push(feature);
      }
    }

    return visibleFeatures;
  }

  private getVisibleClusters(viewPort: google.maps.LatLngBounds): Cluster[] {
    const visibleClusters: Cluster[] = [];
    for (const cluster of this.clusters.values()) {
      if (viewPort.contains(cluster.getClusterPosition())) {
        visibleClusters.push(cluster);
      }
    }

    return visibleClusters;
  }

  private placeInLocalCluster(featureId: string, zoom: number, clusters: Cluster[]): void {
    const positionLatLng = this.featureInformationService.getPosition(featureId);
    const positionPoint = this.featureInformationService.getPositionPoint(featureId);

    const hasFeatureBeenPlaced = this.couldPlaceFeatureInCluster(featureId, positionLatLng, positionPoint, clusters, zoom);
    if (hasFeatureBeenPlaced) {
      return;
    }

    // Could not add the feature to an existing claster, create a new one
    const clusterId = guid();
    const cluster = new FrontendCluster(clusterId);
    cluster.addFeature(featureId, positionLatLng, positionPoint);
    clusters.push(cluster);

    this.featureInClusters.set(featureId, clusterId);
    this.clusters.set(clusterId, cluster);
  }

  private placeInLocalHeatMap(featureId: string) {
    const positionLatLng = this.featureInformationService.getPosition(featureId);
    const weight = this.featureInformationService.getTotalOriginalArea(featureId) + this.featureInformationService.getTotalOriginalFeatures(featureId);
    this.googleMapHeatmapService?.addFeature(positionLatLng, weight);
  }

  private placeInBackendCluster(featureId: string, feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, localClusters: Cluster[]) {
    if (this.featuresLoaded.get(featureId)) {
      this.googleMapFeatureService.removeFeatures(featureId);
      this.featuresLoaded.set(featureId, false);
    }

    // Could not add the feature to an existing claster, create a new one
    const clusterId = guid();
    const cluster = new BackendCluster(clusterId);

    // we assume we always get a Point geometry for backend clustered features
    const center = new google.maps.LatLng(feature.geometry.coordinates[1], feature.geometry.coordinates[0]);
    cluster.setFeature(featureId, center, feature.properties.Testo);
    localClusters.push(cluster);

    this.featureInClusters.set(featureId, clusterId);
    this.clusters.set(clusterId, cluster);
  }

  private couldPlaceFeatureInCluster(featureId: string, positionLatLng: google.maps.LatLng, positionPoint: google.maps.Point, clusters: Cluster[], zoom: number): boolean {
    const pixelSize = Math.pow(2, -zoom);
    for (const cluster of clusters.filter(c => c instanceof FrontendCluster) as FrontendCluster[]) {
      const distanceFromClusterCenter = GoogleMapUtils.getPixelDistanceFromPoint(positionPoint, cluster.positionPoint, pixelSize);
      if (distanceFromClusterCenter < MIN_PIXEL_CLUSTER_DISTANCE) {
        cluster.addFeature(featureId, positionLatLng, positionPoint);
        this.googleMapHeatmapService?.addFeature(positionLatLng);
        this.featureInClusters.set(featureId, cluster.id);
        return true;
      }
    }

    return false;
  }

  private reset(): void {
    const ids = this.googleMapFeatureService.getCurrentFeatures().map(feature => feature.getId().toString());
    this.googleMapFeatureService.removeFeatures(...ids);
    this.featureInformationService.clear();
    this.featuresLoaded.clear();
    this.clearClusteringFeatures();
    this.googleMapFeatureService.removeAll();
    this.geoJson.features = [];
  }

  private clearClusters(): void {
    for (const cluster of this.clusters.values()) {
      cluster.remove();
    }

    this.clusters.clear();
    this.featureInClusters.clear();
  }

  private clearLocalClusters(clusters: Cluster[]): void {
    for (const cluster of clusters) {
      cluster.remove();
      this.clusters.delete(cluster.id);
      for (const featureId of cluster.features) {
        this.featureInClusters.delete(featureId);
      }
    }

    clusters.splice(0, clusters.length);
  }

  private setPolyLabelsFromGoogleMapData(zoom: number, features: google.maps.Data.Feature[]): void {
    this.infoWindowClustererService?.resetPolyLabels();

    // Start rendering labels only after "clusteringLevel"
    if (zoom >= this.clusteringLevel) {
      features.forEach(feature => {
        const f = this.featureInformationService.getById(feature.getId().toString());
        this.polygonLabelInfoWindowService.createPolyLabelAssociatedInfoWindow(f);
      });

      this.polygonLabelService.renderPolyLabels(true);
      this.infoWindowClustererService?.recompute();
    }
  }

  private loadClusteringLevel(): void {
    this.clusteringLevel = enum_ClusteringLevel.default;
    this.sharedDataService.getCfgAlberoGisUtente$().subscribe((cfgAlberoGisUtente) => {
      this.clusteringLevel = cfgAlberoGisUtente[0]?.CfgGisUtente?.LivelloClusterizzazione ?? enum_ClusteringLevel.default;
    });
  }

  private initHeatmap(): void {
    this.googleMapHeatmapService?.isShown$?.subscribe((isShown) =>
      this.switchClusteringMode(isShown)
    );
  }

  private switchClusteringMode(mode: boolean) {
    this.isHeatmapShown = mode;
    let googleMap: google.maps.Map | null;
    if (this.isHeatmapShown) {
      googleMap = this.clusters.values().next().value?.marker?.getMap();
      this.clearClusters();
    } else {
      googleMap = this.googleMapHeatmapService.getHeatmap()?.getMap();
      this.googleMapHeatmapService.removeHeatMap();
    }
    if (googleMap) {
      this.renderGeoJson(googleMap);
    }
  }

  private clearClusteringFeatures() {
    this.clearClusters();
    this.googleMapHeatmapService?.removeHeatMap();
  }

  public forceMapRefresh(googleMap: google.maps.Map): void {
    google.maps.event.trigger(googleMap, 'idle');
  }

  private isRasterVisible(layerId: string) {
    const isGoogleGridVisible = this.cfgAlberoGisUtente?.CfgGisUtente?.ckGrigliaTiles_Sviluppo ?? false;
    const isRaster = this.sharedDataService.getTipologiaLayerById(layerId)?.FeatureTypeId === FeatureType.Raster.toString();
    const isRasterVisible = isRaster && isGoogleGridVisible && this.layerVisible.get(layerId);
    return isRasterVisible;
  }
}

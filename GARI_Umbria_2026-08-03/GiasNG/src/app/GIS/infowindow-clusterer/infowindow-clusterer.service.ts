import { Injectable } from '@angular/core';
import { GoogleMapService } from '../google-map/google-map.service';
import { GiasInfoWindow } from './models/gias-infowindow';
import { GiasCluster, GiasClusterOptions } from './models/gias-cluster';
import { MFSet } from 'app/Model/MFSet.model';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from 'app/Service/api.service';

const RADIUS: number = 150;

export interface InfoWindowClustererOptions {
  infoWindows?: GiasInfoWindow[];
  map?: google.maps.Map | null;
}

@Injectable()
export class InfowindowClustererService {
  private clusters: GiasCluster[] = [];
  private infoWindows: GiasInfoWindow[] = [];
  private infoWindowsMFSet: MFSet<string> = new MFSet<string>([]);
  private infoWindowsIdMap: Map<string, GiasInfoWindow> = new Map<string, GiasInfoWindow>();
  private lastClusterId: number = 0;

  constructor(private googleMapService: GoogleMapService) { }

  private get map() {
    return this.googleMapService.googleMapWrapper.googleMap
  }

  public get polyLabelsClusterer() {
    return this.infoWindows.filter(pl => pl.clusterable && pl.visible);
  }

  // Aggiunge la InfoWindow, prova ad inserirla in un cluster pre-esistente, altrimenti prova a crearne di nuovi
  public addInfowindow(infoWindow: GiasInfoWindow): void {
    const key = infoWindow.getInfoWindowTitle();
    if (this.infoWindowsIdMap.has(key)) {
      return;
    }

    this.infoWindowsIdMap.set(key, infoWindow);
    this.infoWindows.push(infoWindow);
    this.infoWindowsMFSet.add(key);
  }

  public recompute(): void {
    this.clearClusterer();

    // Use MFSet to prepare clusters
    const windows = this.getInfoWindowsInsideMapBounds(this.getVisibleInfoWindows(this.infoWindows));
    this.processWindows(windows, this.infoWindowsMFSet);

    const result: Map<string, GiasClusterOptions> = new Map<string, GiasClusterOptions>();
    for (const w of windows) {
      const parent = this.infoWindowsMFSet.find(w.getInfoWindowTitle());
      if (!result.has(parent)) {
        result.set(parent, { position: w.infoWindow.getPosition(), infoWindows: [], map: this.map, id: ++this.lastClusterId });
      }

      const cluster = result.get(parent);
      cluster.infoWindows.push(w);
    }

    this.clusters = [...result.values()].map(data => new GiasCluster(data));
    for (const cluster of this.clusters) {
      cluster.renderCluster();
    }
  }

  /// Resets and delete all clusters
  public clearClusterer(): void {
    this.clusters?.forEach(c => {
      c.delete();
      c = null;
    });

    this.clusters = [];
    this.infoWindowsMFSet.reset();
  }

  public resetPolyLabels(): void {
    this.clearClusterer();

    this.infoWindows.forEach(iw => {
      iw.hideLabel();
      iw = null;
    });

    this.infoWindows = [];
    this.infoWindowsIdMap.clear();
  }

  public recomputeForPanEvent(): void {
    // Implement this if performance are an issue
    // this.hideOffScreenClusters();

    const infoWindows = this.getInfoWindowsInsideMapBounds(this.getVisibleInfoWindows(this.infoWindows));
    const positions = new Map<string, google.maps.Point>(infoWindows.map(w => [w.getInfoWindowTitle(), this.getPxlPosition(w.infoWindow.getPosition())]));
    const clustersToBeReloaded = new Set<GiasCluster>();
    for (const infoWindow of infoWindows) {
      const key = infoWindow.getInfoWindowTitle();
      if (!this.infoWindowsMFSet.has(key)) {
        // This should not happen by panning the view
        this.infoWindowsMFSet.add(key);
      }

      // The window is already clustered
      const parentWindow = this.infoWindowsIdMap.get(this.infoWindowsMFSet.find(key));
      if (parentWindow.isClustered) {
        continue;
      }

      // Create new cluster if not clusterable or not mergable
      if (!infoWindow.clusterable || !this.tryMergeInExistingCluster(infoWindows, key, positions, this.infoWindowsMFSet)) {
        const newCluster = new GiasCluster({ position: infoWindow.infoWindow.getPosition(), infoWindows: [infoWindow], map: this.map, id: ++this.lastClusterId });
        this.clusters.push(newCluster);
        clustersToBeReloaded.add(newCluster);
        continue;
      }

      const existingCluster = this.clusters.find(c => c.id == parentWindow.clusterId);
      if (existingCluster != null) {
        existingCluster.push(infoWindow);
        clustersToBeReloaded.add(existingCluster);
      }
    }

    for (const cluster of clustersToBeReloaded) {
      cluster.renderCluster();
    }
  }

  public removePolyLabelAssociatedWithFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): void {
    const key = feature.properties.id;
    if (!this.infoWindowsIdMap.has(key)) {
      return;
    }

    const infoWindow = this.infoWindowsIdMap.get(key);
    const cluster = this.clusters.find(c => c.id == infoWindow.clusterId);
    if (cluster != null) {
      cluster.removeInfoWindow(key);
    }

    const index = this.infoWindows.findIndex(iw => iw.getInfoWindowTitle() == key);
    this.infoWindows.splice(index, 1);
    this.infoWindowsIdMap.delete(key);
    infoWindow.hideLabel();
  }

  public changePositionOfPolyLabel(title: string, pos: google.maps.LatLng): void {
    this.infoWindows.find(iw => iw.getInfoWindowTitle() == title)?.infoWindow.setPosition(pos);
  }

  public changeClusterByLayer(toCluster: boolean, layer: string): void {
    this.infoWindows.filter(iw => iw.layer == layer).forEach(iw => iw.clusterable = toCluster);
    this.recompute();
  }

  public changeVisibleByLayer(visible: boolean, layer: string): void {
    this.infoWindows.filter(iw => iw.layer == layer).forEach(iw => iw.visible = visible);
    this.recompute();
  }

  public changeClusterAllLayer(toCluster: boolean): void {
    this.infoWindows.forEach(iw => iw.clusterable = toCluster);
    this.recompute();
  }

  public changeVisibleAllLayer(visible: boolean): void {
    this.infoWindows.forEach(iw => iw.visible = visible);
    this.recompute();
  }

  private processWindows(windows: GiasInfoWindow[], set: MFSet<string>): void {
    const positions = new Map<string, google.maps.Point>(windows.map(w => [w.getInfoWindowTitle(), this.getPxlPosition(w.infoWindow.getPosition())]));
    for (const windowA of windows) {
      if (!windowA.clusterable) {
        continue;
      }

      const key = windowA.getInfoWindowTitle();
      this.tryMergeInExistingCluster(windows, key, positions, set);
    }
  }

  private getInfoWindowsInsideMapBounds(infoWindows: GiasInfoWindow[]): GiasInfoWindow[] {
    return infoWindows.filter(iw => this.map?.getBounds()?.contains(iw.infoWindow.getPosition()));
  }

  private getVisibleInfoWindows(infoWindows: GiasInfoWindow[]): GiasInfoWindow[] {
    return infoWindows.filter(iw => iw.visible);
  }

  private getPxlPosition(pos: google.maps.LatLng): google.maps.Point {
    const gMapsUtility = require('../../GiasJSLibraries/GIS-js-libraries/gMapsUtility');
    return gMapsUtility.gMapsUtility?.fromLatLngToPoint(pos, this.map);
  }

  private fitInClusterBuonds(distance: number): boolean {
    return distance <= RADIUS;
  }

  private tryMergeInExistingCluster(infoWindows: GiasInfoWindow[], key: any, positions: Map<string, google.maps.Point>, set: MFSet<string>): boolean {
    for (const windowB of infoWindows) {
      const idB = windowB.getInfoWindowTitle();
      const thisParent = set.find(key);
      const otherParent = set.find(idB);
      if (thisParent == otherParent || positions.get(thisParent) == null || positions.get(otherParent) == null) {
        continue;
      }

      const distance = FunzioniComuniService.getPointsDistance(positions.get(thisParent), positions.get(otherParent));
      if (this.fitInClusterBuonds(distance)) {
        set.merge(key, otherParent);
        return true;
      }
    }

    return false;
  }
}

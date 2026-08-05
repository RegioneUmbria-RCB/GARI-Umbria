import { BehaviorSubject, Observable } from "rxjs";
import { SharedDataService } from "../shared-data.service";
import { Injectable } from "@angular/core";
import { GoogleMapService } from "app/GIS/google-map/google-map.service";
import {
  Cluster,
  ClusterStats,
  MarkerClusterer,
  Renderer,
} from "@googlemaps/markerclusterer";

const RADIUS = 20;
const ICON_SIZE = 32;
const ICON_CENTER = 16;

@Injectable()
export class GoogleMapHeatmapService {
  private heatmap: google.maps.visualization.HeatmapLayer | null;
  private clusterer: MarkerClusterer | null;
  private map: google.maps.Map | null;
  private positions: google.maps.visualization.WeightedLocation[];
  private isShown: BehaviorSubject<boolean>;

  constructor(
    sharedDataService: SharedDataService,
    googleMapService: GoogleMapService
  ) {
    this.positions = [];
    this.isShown = new BehaviorSubject<boolean>(false);
    googleMapService.loaded$.subscribe((loaded) => {
      this.map = googleMapService.googleMapWrapper.googleMap;
      this.heatmap = new google.maps.visualization.HeatmapLayer({
        radius: RADIUS,
      });
      this.clusterer = new MarkerClusterer({
        renderer: new InvisibleRenderer(),
      });
    });
    sharedDataService
      .getCfgAlberoGisUtente$()
      .subscribe((cfgAlberoGisUtente) => {
        this.isShown.next(
          cfgAlberoGisUtente[0]?.CfgGisUtente?.chkMostraHeatmap ?? false
        );
      });
  }

  public getHeatmap(): google.maps.visualization.HeatmapLayer | null {
    return this.heatmap;
  }

  public addFeature(position: google.maps.LatLng, weight: number = 0): void {
    this.positions.push({ location: position, weight: weight });
  }

  public clearFeatures(): void {
    this.positions = [];
  }

  public displayHeatmap(): void {
    this.heatmap?.setMap(this.map);
    this.heatmap?.setData(this.positions);
    this.clusterer?.setMap(this.map);
    this.clusterer?.clearMarkers();
    this.clusterer?.addMarkers(
      this.positions.map((position) => {
        const marker = new google.maps.Marker({
          position: position.location,
          opacity: 0,
          icon: {
            url: "https://maps.google.com/mapfiles/ms/icons/purple.png",
            scaledSize: new google.maps.Size(ICON_SIZE, ICON_SIZE),
            anchor: new google.maps.Point(ICON_CENTER, ICON_CENTER),
          },
          optimized: true,
        });
        marker.addListener("click", () => {
          const bounds = new google.maps.LatLngBounds();
          bounds.extend(position.location);
          this.map.fitBounds(bounds);
          this.map.setCenter(bounds.getCenter());
        });
        return marker;
      })
    );
  }

  public removeHeatMap(): void {
    this.heatmap?.setMap(null);
    this.clearFeatures();
    this.heatmap?.setData(this.positions);
    this.clusterer?.setMap(null);
    this.clusterer?.clearMarkers();
  }

  public switchHeatmapDisplay(): void {
    this.isShown.next(!this.isShown.value);
  }

  public get isShown$(): Observable<boolean> {
    return this.isShown.asObservable();
  }
}

class InvisibleRenderer implements Renderer {
  render(cluster: Cluster, stats: ClusterStats) {
    return new google.maps.Marker({
      position: cluster.position,
      opacity: 0,
      icon: {
        url: "https://maps.google.com/mapfiles/ms/icons/purple.png",
        scaledSize: new google.maps.Size(ICON_SIZE, ICON_SIZE),
        anchor: new google.maps.Point(ICON_CENTER, ICON_CENTER),
      },
      optimized: true,
    });
  }
}

import { GiasInfoWindow } from './gias-infowindow';
import { GiasDefaultRenderer } from './gias-renderer';

export interface GiasClusterOptions {
  position?: google.maps.LatLng | google.maps.LatLngLiteral;
  infoWindows?: Array<GiasInfoWindow>;
  map: google.maps.Map;
  id: number;
}

export class GiasCluster {
  marker: google.maps.Marker | null = null;

  private _infoWindows?: Array<GiasInfoWindow>;
  private _position: google.maps.LatLng;
  private _renderedInfoWindow: GiasInfoWindow | null = null;
  readonly map: google.maps.Map;
  readonly id: number;

  constructor({ infoWindows, position, map, id }: GiasClusterOptions) {
    this._infoWindows = infoWindows;
    this.map = map;
    this.id = id;

    if (position != null) {
      if (position instanceof google.maps.LatLng) {
        this._position = position;
      } else {
        this._position = new google.maps.LatLng(position);
      }
    }

    this._infoWindows.forEach(_iw => _iw.clusterId = this.id);
  }

  public get infoWindows(): Array<GiasInfoWindow> {
    return this._infoWindows;
  }

  public get bounds(): google.maps.LatLngBounds | null {
    if (this._infoWindows.length === 0 && this._position == null) {
      return null;
    }

    const bounds = new google.maps.LatLngBounds(this._position, this._position);
    for (const iw of this._infoWindows) {
      bounds.extend(iw.infoWindow.getPosition());
    }

    return bounds;
  }

  public get position(): google.maps.LatLng {
    return this._position || this.bounds?.getCenter();
  }

  public push(infoWindow: GiasInfoWindow): void {
    infoWindow.clusterId = this.id;
    this._infoWindows.push(infoWindow);
  }

  public delete(): void {
    this.marker?.setMap(null);
    this.marker = null;

    this._renderedInfoWindow?.hideLabel()
    this._renderedInfoWindow = null;

    this._infoWindows.forEach(iw => iw.clusterId = NaN);
    this._infoWindows = [];
  }

  public renderCluster(): void {
    this.marker?.setMap(null);
    this.marker = null;

    this._renderedInfoWindow?.hideLabel();
    this._renderedInfoWindow = null;

    const iwsVisible: Array<GiasInfoWindow> = this.infoWindows.filter(iw => iw.visible);
    if (iwsVisible.length == 1) {
      iwsVisible[0].showLabel();
      this._renderedInfoWindow = iwsVisible[0];
      return;
    }

    if (iwsVisible.length > 1) {
      this.marker = new GiasDefaultRenderer().render(this, null);
      this.marker?.addListener('click', () => this.handleClickOnCluster());
    }
  }

  public removeInfoWindow(key: string): boolean {
    const index = this._infoWindows.findIndex(iw => iw.getInfoWindowTitle() == key);
    if (index == -1) {
      return false;
    }

    const removedElements = this._infoWindows.splice(index, 1);
    removedElements.forEach(e => e.clusterId = NaN);

    return removedElements.length == 1;
  }

  private handleClickOnCluster(): void {
    const latlngAutoFit: google.maps.LatLngBounds = this.calculateMapLimits(this.infoWindows);
    if (latlngAutoFit) {
      const centroMappa: google.maps.LatLng = latlngAutoFit.getCenter();
      this.map.setCenter(centroMappa);
      this.map.fitBounds(latlngAutoFit);
    }
  }

  private calculateMapLimits(infoWindows: Array<GiasInfoWindow>): google.maps.LatLngBounds {
    let latlngAutoFit = new google.maps.LatLngBounds();
    infoWindows.forEach((iw) => {
      let coords: google.maps.LatLng = iw.infoWindow.getPosition();
      latlngAutoFit.extend(coords);
    });

    return latlngAutoFit;
  }
}

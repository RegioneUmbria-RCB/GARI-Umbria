import { enum_InfoWindowCustomProperties } from '../../services/polygon-label-infowindow.service';

export class GiasInfoWindow {
  private _infoWindow: google.maps.InfoWindow;
  private _visible: boolean = false;
  private _clusterable: boolean = false;
  private _clusterId: number = NaN;
  private _defaultOptions: google.maps.InfoWindowOpenOptions = {};

  constructor(
    opts: google.maps.InfoWindowOptions,
    map: google.maps.Map,
    public readonly layer: string
  ) {
    this._infoWindow = new google.maps.InfoWindow(opts);
    this._defaultOptions = {
      anchor: null,
      map: map,
      shouldFocus: false
    };
  }

  public get clusterId() {
    return this._clusterId;
  }

  public set clusterId(value: number) {
    this._clusterId = value;
  }

  get visible(): boolean {
    return this._visible;
  }

  set visible(value: boolean) {
    this._visible = value;
  }

  get clusterable(): boolean {
    return this._clusterable;
  }

  set clusterable(value: boolean) {
    this._clusterable = value;
  }

  get isClustered(): boolean {
    return !Number.isNaN(this.clusterId);
  }

  public get infoWindow(): google.maps.InfoWindow {
    return this._infoWindow;
  }

  public getInfoWindowTitle(): string {
    return this.infoWindow.get(enum_InfoWindowCustomProperties.title);
  }

  public setInfoWindowTitle(title: string): void {
    this.infoWindow.set(enum_InfoWindowCustomProperties.title, title);
  }

  public showLabel(): void {
    if (this.visible) {
      this.infoWindow.open(this._defaultOptions);
    }
  }

  public hideLabel(): void {
    this.infoWindow.close();
  }
}

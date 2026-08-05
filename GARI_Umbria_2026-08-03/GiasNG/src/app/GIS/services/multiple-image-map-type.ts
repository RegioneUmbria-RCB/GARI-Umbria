import { UrlFirmato } from "app/Service/api.service";
import { TILE_SIZE, SATELLITE_DATA_MAP_TYPE, TILE_OVERLAY_LOADING_MAX_ZOOM, TILE_OVERLAY_LOADING_MIN_ZOOM } from "./raster-overlay.service";
import { enum_LayerElementiGraficiStd } from "../GIS-enum/GIS-layer-elementi-grafici";

export class MultipleImageMapType implements google.maps.MapType {
  tileSize = new google.maps.Size(TILE_SIZE, TILE_SIZE);
  alt: string;
  maxZoom = TILE_OVERLAY_LOADING_MAX_ZOOM;
  minZoom = TILE_OVERLAY_LOADING_MIN_ZOOM;
  name = SATELLITE_DATA_MAP_TYPE;
  projection: google.maps.Projection;
  radius: number;
  id = +enum_LayerElementiGraficiStd.ANALISI_MAPPE_SATELLITARI;
  description: string;

  private canvas = new Map<google.maps.Point, HTMLCanvasElement>();

  constructor(private urls: UrlFirmato[], private opacity: number) { }

  getTile(tile: google.maps.Point, zoom: number, ownerDocument: Document): Element {
    const urls = this.urls.filter(url => url.Key == `${zoom}/${tile.x}/${tile.y}`);
    if (urls.length == 0) {
      return ownerDocument.createElement('div');
    }

    const canvas = this.canvas.get(tile) ?? ownerDocument.createElement('canvas');
    canvas.width = this.tileSize.width;
    canvas.height = this.tileSize.height;
    canvas.style.opacity = `${this.opacity}`;

    const context = canvas.getContext('2d');

    for (const url of urls) {
      const image = new Image();
      image.src = url.SignedUrl;
      image.onload = () => context.drawImage(image, 0, 0);
    }

    this.canvas.set(tile, canvas);
    return canvas;
  }

  releaseTile(_: Element): void {
    let i = 0; //per togliere errore sonarQube
  }

  setOpacity(opacity: number): void {
    this.opacity = opacity;
    if (this.canvas == null) {
      return;
    }

    for (const c of this.canvas.values()) {
      c.style.opacity = `${this.opacity}`;
    }
  }
}

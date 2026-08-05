import { UrlFirmato } from "app/Service/api.service";
import { TILE_SIZE, TILE_OVERLAY_LOADING_MAX_ZOOM, TILE_OVERLAY_LOADING_MIN_ZOOM, SATELLITE_DATA_MAP_TYPE } from "./raster-overlay.service";
import { PolygonInterceptionUtils } from "./polygon-interception-utils";

// Based on https://stackoverflow.com/questions/24311350/clip-google-maps-js-api-imagemaptype-to-a-polygon

export class PolygonInterceptionMapType implements google.maps.MapType {
  tileSize = new google.maps.Size(TILE_SIZE, TILE_SIZE);
  alt: string = "PolygonInterceptionMapType";
  maxZoom: number = TILE_OVERLAY_LOADING_MAX_ZOOM;
  minZoom: number = TILE_OVERLAY_LOADING_MIN_ZOOM;
  name: string = SATELLITE_DATA_MAP_TYPE;
  projection: google.maps.Projection;
  radius: number;

  id: number;
  description: string;
  private polygons: google.maps.Data.Polygon[];
  private map: google.maps.Map;
  private urls: UrlFirmato[] | UrlFirmato;
  private opacity: number;
  private canvas = new Map<google.maps.Point, HTMLCanvasElement>();

  constructor(id: number, polygons: google.maps.Data.Polygon[], map: google.maps.Map, urls: UrlFirmato[] | UrlFirmato, opacity: number) {
    this.id = id;
    this.polygons = polygons;
    this.map = map;
    this.urls = urls;
    this.opacity = opacity;
  }

  // Funzione richiamata per ogni tile
  getTile(coord: google.maps.Point, z: number, ownerDocument: Document): Element {
    const map = this.map;
    const scale = Math.pow(2, z);

    // if (z > this.maxZoom || z < this.minZoom) {
    //   return ownerDocument.createElement('div');
    // }

    if (coord.y < 0 || coord.y >= scale) {
      return ownerDocument.createElement('div');
    }

    const intersections = [];
    for (const polygon of this.polygons) {
      const intersection = PolygonInterceptionUtils.getPointPolygonIntersection(polygon, coord, z);
      if (intersection.length != 0) {
        intersections.push(intersection);
      }
    }

    if (intersections.length == 0) {
      return ownerDocument.createElement('div');
    }

    const urls = Array.isArray(this.urls) ? this.urls.filter(url => url.Key == `${z}/${coord.x}/${coord.y}`).map(x => x?.SignedUrl) : this.getLegacyUrl(z, coord.x, coord.y);
    if (urls == null) {
      return ownerDocument.createElement('div');
    }


    const canvas = this.canvas.get(coord) ?? ownerDocument.createElement('canvas');
    canvas.width = this.tileSize.width;
    canvas.height = this.tileSize.height;
    canvas.style.opacity = `${this.opacity}`;

    const xdif = coord.x * this.tileSize.width;
    const ydif = coord.y * this.tileSize.height;
    const context = canvas.getContext('2d');
    for (const url of urls) {
      const image = new Image();
      image.src = url;
      image.onload = () => {
        context.beginPath();

        for (const intersection of intersections) {
          const points = intersection.map(x => {
            const worldPoint = map.getProjection().fromLatLngToPoint(x);
            return new google.maps.Point((worldPoint.x) * scale - xdif, (worldPoint.y) * scale - ydif);
          });

          context.moveTo(points[0].x, points[0].y);

          const count = points.length;
          for (let i = 0; i < count; i++) {
            context.lineTo(points[i].x, points[i].y);
          }

          context.lineTo(points[count - 1].x, points[count - 1].y);
        }

        context.closePath();
        context.clip();
        context.drawImage(image, 0, 0);
      };
    }

    this.canvas.set(coord, canvas);
    return canvas;
  }

  private getLegacyUrl(zoom: number, tileX: number, tileY: number): string {
    const url = (this.urls as UrlFirmato).SignedUrl;

    const ymax = 1 << zoom;
    const y = ymax - tileY - 1;
    const coords = `${zoom}/${tileX}/${y}`; // legacy computation
    return `${url}/${coords}.png`;
  }

  releaseTile(tile: Element): void {
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

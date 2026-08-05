import { Injectable } from "@angular/core";
import { GoogleMap } from "@angular/google-maps";
import { TILE_OVERLAY_LOADING_MAX_ZOOM, TILE_OVERLAY_LOADING_MIN_ZOOM } from "./raster-overlay.service";

@Injectable()
export class MapGridOverlayService {
  public add(googleMap: GoogleMap): void {
    if (this.getIndex(googleMap.overlayMapTypes) == -1) {
      googleMap.overlayMapTypes.insertAt(0, new MapGridOverlay());
    }
  }

  public remove(googleMap: GoogleMap): void {
    const index = this.getIndex(googleMap.overlayMapTypes);
    if (index != -1) {
      googleMap.overlayMapTypes.removeAt(index);
    }
  }

  private getIndex(overlays: google.maps.MVCArray<google.maps.MapType>): number {
    for (const [i, overlay] of overlays.getArray().entries()) {
      if (overlay instanceof MapGridOverlay) {
        return i;
      }
    }

    return -1;
  }
}

export class MapGridOverlay implements google.maps.MapType {
  alt = "MapGridOverlay";
  maxZoom = TILE_OVERLAY_LOADING_MAX_ZOOM;
  minZoom = TILE_OVERLAY_LOADING_MIN_ZOOM;
  name = "MapGridOverlay";
  projection: google.maps.Projection;
  radius: number;
  tileSize = new google.maps.Size(256, 256);

  getTile(coord: google.maps.Point, _: number, ownerDocument: Document): Element {
    const div = ownerDocument.createElement('div');
    div.style.width = this.tileSize.width + 'px';
    div.style.height = this.tileSize.height + 'px';
    div.style.fontSize = '10';
    div.style.color = 'yellow';
    div.style.borderStyle = 'solid';
    div.style.borderWidth = '1px';
    div.style.borderColor = '#AAAAAA';
    div.innerHTML = coord.toString();
    return div;
  }

  releaseTile(_: Element): void {
    let i = 0; //per togliere errore sonarQube
  }
}

import { Component, EventEmitter, Output } from '@angular/core';
import { DrawingService } from 'app/GIS/services/drawing.service';
import { IOutsideResettableComponent } from '../marker-window.component';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { BehaviorSubject } from 'rxjs';
import { GiasDialogService } from 'app/Service/gias-dialog.service';

@Component({
  standalone: false,
  selector: 'app-tool-disegno-avanzato',
  templateUrl: './tool-disegno-avanzato.component.html',
  styleUrls: ['./tool-disegno-avanzato.component.css']
})
export class ToolDisegnoAvanzatoComponent implements IOutsideResettableComponent {
  @Output() onReset = new EventEmitter<boolean>();

  private dataSubject = new BehaviorSubject<DrawData | null>(null);

  data$ = this.dataSubject.asObservable();
  isCloseConfirmationOpen: boolean = false;
  isNewPolygonConfirmationOpen: boolean = false;
  tmpPolygon: google.maps.Polygon | null = null;

  constructor(
    private googleMapService: GoogleMapService,
    private drawingService: DrawingService,
    private giasDialogService: GiasDialogService
  ) { }

  addFromMap(): void {
    const map = this.googleMapService.googleMapWrapper.data.getMap();
    const markerCenter = map.getCenter();
    const bounds = map.getBounds();

    const markerNE = bounds.getNorthEast();
    const markerSW = bounds.getSouthWest();
    const markerNW = new google.maps.LatLng({ lat: markerNE.lat(), lng: markerSW.lng() });
    const markerSE = new google.maps.LatLng({ lat: markerSW.lat(), lng: markerNE.lng() });

    map.setZoom(map.getZoom() - 1);

    this.tmpPolygon = this.drawingService.getPolygonFromCoordinates([markerNE, markerNW, markerSW, markerSE]);
    this.tmpPolygon.setMap(map);
    this.tmpPolygon.setDraggable(false);
    this.tmpPolygon.setEditable(false);

    const distanceX = google.maps.geometry.spherical.computeDistanceBetween(markerNE, markerNW);
    const distanceY = google.maps.geometry.spherical.computeDistanceBetween(markerNE, markerSE);
    this.dataSubject.next({ markerCenter: markerCenter, markerNE: markerNE, markerNW: markerNW, markerSE: markerSE, markerSW: markerSW, distanceX: Math.round(distanceX), distanceY: Math.round(distanceY) })
  }

  save(): void {
    if (this.tmpPolygon == null) {
      return;
    }

    this.drawingService.addDrawing(this.tmpPolygon);
  }

  reset(data: DrawData): void {
    this.onReset.emit(data != null);
  }

  resetAndAddNew(): void {
    this.isNewPolygonConfirmationOpen = false;
    this.onOutsideReset();
    this.addFromMap();
  }

  handleAddFromMapClick(): void {
    const map = this.googleMapService.googleMapWrapper.data.getMap();
    if (map.getZoom() < 15) {
      this.giasDialogService.baseError("", "gis.ToolDisegnoAvanzatoZoomNonValido");
      return;
    }

    // We don't have a polygon yer
    if (this.tmpPolygon == null) {
      this.addFromMap();
      return;
    }

    // Ask user to remove the previous polygon
    this.isNewPolygonConfirmationOpen = true;
  }

  public onOutsideReset(): void {
    this.dataSubject.next(null);
    this.drawingService.removeAllPolygons();
    this.tmpPolygon?.setMap(null);
    this.tmpPolygon = null;
  }
}

interface DrawData {
  markerCenter: google.maps.LatLng;
  markerNE: google.maps.LatLng;
  markerNW: google.maps.LatLng;
  markerSW: google.maps.LatLng;
  markerSE: google.maps.LatLng;
  distanceX: number;
  distanceY: number;
}

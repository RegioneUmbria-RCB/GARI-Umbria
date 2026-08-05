import { Component, EventEmitter, OnDestroy, Output } from '@angular/core';
import { enum_GISDrawingOperations } from 'app/GIS/GIS-enum/GIS-drawing-operations';
import { GiasMarker } from 'app/GIS/models/gias-drawings.model';
import { DrawingService } from 'app/GIS/services/drawing.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { FeatureType } from 'app/Model/GIS/GisDataReadRval_New';
import { TipologiaLayer } from 'app/Service/api.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { Subscription } from 'rxjs';
import { DrawWindowOperationService } from '../../draw-window/draw-window-operation.service';
import { IOutsideResettableComponent } from '../marker-window.component';

@Component({
  standalone: false,
  selector: 'app-tool-disegna-punti',
  templateUrl: './tool-disegna-punti.component.html',
  styleUrls: ['./tool-disegna-punti.component.css']
})
export class ToolDisegnaPuntiComponent implements OnDestroy, IOutsideResettableComponent {
  @Output() onReset = new EventEmitter<boolean>();

  markers: Marker[] = [];
  isGpsActive: boolean = null;
  isCloseConfirmationOpen: boolean = false;

  private layerSelected: TipologiaLayer | null = null;
  private subscriptions: Subscription[] = [];

  constructor(
    private drawingService: DrawingService,
    private layerService: LayerService,
    private giasDialogService: GiasDialogService,
    private drawWindowOperationService: DrawWindowOperationService
  ) {
    this.subscriptions.push(
      this.drawingService
        .markers$
        .subscribe(() => this.updateMarkers())
    );

    this.subscriptions.push(
      this.layerService
        .layerItemSelected$
        .subscribe(([layer, selected]) => {
          if (selected) {
            this.layerSelected = layer;
          }
        })
    );

    window.navigator
      .geolocation
      .getCurrentPosition(
        () => this.isGpsActive = true,
        () => this.isGpsActive = false
      );
  }

  ngOnDestroy(): void {
    for (const sub of this.subscriptions) {
      sub.unsubscribe();
    }
  }

  save(): void {
    if (this.markers.length == 0) {
      this.giasDialogService.baseError('gis.StrumentoDisegnoPunti', 'gis.NessunPuntoInserito');
      return;
    }

    const selectedLayer = this.layerService.layerItemSelected[0];
    if (selectedLayer.FeatureTypeId == `${FeatureType.Point}`) {
      this.drawingService.addEvent.next(this.markers[0].marker);
      return;
    }

    if (selectedLayer.FeatureTypeId == `${FeatureType.LineString}`) {
      this.drawingService.fromPointsToPolyline(this.markers.map(x => x.marker.marker.getPosition()), true);
      return;
    }

    if (this.markers.length < 3) {
      this.giasDialogService.baseError('gis.StrumentoDisegnoPunti', 'gis.NonAbbastanzaPunti');
      return;
    }

    this.drawingService.fromPointsToPolygon(this.markers.map(x => x.marker.marker.getPosition()), true);
  }

  reset(): void {
    this.onReset.emit(this.markers.length > 0);
  }

  addPoint(): void {
    if (this.layerSelected != null && this.layerSelected.FeatureTypeId == `${FeatureType.Point}` && this.markers.length >= 1) {
      this.giasDialogService.baseError('gis.StrumentoDisegnoPunti', 'gis.GpsDisabledSinglePoint');
      return;
    }

    window.navigator
      .geolocation
      .getCurrentPosition(pos => {
        const coords = new google.maps.LatLng(pos.coords.latitude, pos.coords.longitude);
        this.drawingService.fromPointToMarker(coords, true, true);
      });
  }

  removePoint(marker: Marker): void {
    this.drawingService.removeDrawing(marker.marker.id);

    // Re-enable marker operation if we are in single-point mode and the selected point is removed
    if (this.layerSelected.FeatureTypeId == `${FeatureType.Point}`) {
      this.drawWindowOperationService.setOperation(enum_GISDrawingOperations.marker);
    }
  }

  public onOutsideReset(): void {
    // Nothing to do
  }

  private updateMarkers(): void {
    const result: Marker[] = [];

    for (const marker of this.drawingService.markers) {
      const position = marker.marker.getPosition();
      result.push({ lat: position.lat(), lng: position.lng(), marker: marker });
    }

    this.markers = result;
  }

}

class Marker {
  lat: number;
  lng: number;
  marker: GiasMarker;
}

import { GoogleMapMovementControlService } from 'app/GIS/services/google-map-movement-control.service';
import { UndoableCommand } from '../GIS-layer-window-toolbar.component';

export class ZoomAreaCommand implements UndoableCommand {
  private eventListener: google.maps.MapsEventListener | null = null;
  private drawingManagerOptions: google.maps.drawing.DrawingManagerOptions = {
    drawingControlOptions: {
      position: google.maps.ControlPosition.BOTTOM_CENTER,
      drawingModes: [google.maps.drawing.OverlayType.RECTANGLE]
    },
    rectangleOptions: {
      strokeColor: '#B07E4F',
      fillColor: '#B07E4F',
      fillOpacity: 0.4
    },
    drawingMode: google.maps.drawing.OverlayType.RECTANGLE
  };

  constructor(
    private drawingManager: google.maps.drawing.DrawingManager,
    private googleMap: google.maps.Map,
    private googleMapMovementControlService: GoogleMapMovementControlService
  ) { }

  do(): void {
    if (this.eventListener != null) {
      return;
    }

    this.eventListener = google.maps.event.addListener(this.drawingManager, 'rectanglecomplete', (event: google.maps.Rectangle) => {
      const latlngAutoFit = event.getBounds();
      this.googleMap.setCenter(latlngAutoFit.getCenter());
      this.googleMap.fitBounds(latlngAutoFit);
      event.setMap(null);
    });

    this.drawingManager.setMap(this.googleMap);
    this.drawingManager.setOptions(this.drawingManagerOptions);
  }

  undo(): void {
    if (this.eventListener == null) {
      return;
    }

    google.maps.event.removeListener(this.eventListener);
    this.drawingManager.setMap(null);
    this.drawingManager.setDrawingMode(null);
    this.drawingManager.setOptions({ drawingControlOptions: { drawingModes: [] } });
    this.eventListener = null;
  }
}

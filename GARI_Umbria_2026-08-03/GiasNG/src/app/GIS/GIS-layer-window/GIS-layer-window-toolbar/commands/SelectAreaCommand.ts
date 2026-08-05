import { LayerService } from 'app/GIS/services/layer.service';
import { enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { enum_OrigineChiamata } from 'app/GIS/GIS-enum/GIS-origine-chiamata';
import { UndoableCommand } from '../GIS-layer-window-toolbar.component';
import { GoogleMapUtils } from 'app/GIS/utils/google-map.utils';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';

export class SelectAreaCommand implements UndoableCommand {
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
    private layerService: LayerService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private featureInformationService: FeatureInformationService
  ) { }

  do(): void {
    if (this.eventListener != null) {
      return;
    }

    this.eventListener = google.maps.event.addListener(this.drawingManager, 'rectanglecomplete', (event: google.maps.Rectangle) => {
      const polygon = GoogleMapUtils.rectangleToPolygon(event);
      this.googleMapGeoJsonService.seDeselezionaFeatureSelezionate(enum_OrigineChiamata.Mappa);

      const selectedLayer = this.layerService.layerItemSelected[0]?.id;
      this.featureInformationService.getAll().forEach(feature => {
        if (feature.properties.layer != selectedLayer) {
          return;
        }

        let select = false;
        this.featureInformationService.getPath(feature.properties.id).forEach(latlng => {
          if (google.maps.geometry.poly.containsLocation(latlng, polygon)) {
            select = true;
          }
        });

        if (select) {
          this.googleMapGeoJsonService.selezionaFeature(feature, true, false, enum_OrigineChiamata.Mappa);
        }
      });

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

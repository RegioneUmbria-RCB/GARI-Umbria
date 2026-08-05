import { Component, EventEmitter, OnDestroy, Output } from '@angular/core';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { enum_OrigineChiamata } from 'app/GIS/GIS-enum/GIS-origine-chiamata';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { DrawingOptionsService } from 'app/GIS/services/drawing-options.service';
import { DrawingService } from 'app/GIS/services/drawing.service';
import { FeatureService } from 'app/GIS/services/feature.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp, TipologiaLayer } from 'app/Service/api.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { Subscription } from 'rxjs';
import { IOutsideResettableComponent } from '../marker-window.component';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';

@Component({
  standalone: false,
  selector: 'app-tool-scomponi-punti',
  templateUrl: './tool-scomponi-punti.component.html',
  styleUrls: ['./tool-scomponi-punti.component.css']
})
export class ToolScomponiPuntiComponent implements OnDestroy, IOutsideResettableComponent {
  @Output() onReset = new EventEmitter<boolean>()

  selectedMarkers: Marker[] = [];
  selectedLayer: TipologiaLayer;
  layers: TipologiaLayer[] = [];

  private markers: Marker[] = [];
  private subscriptions: Subscription[] = [];

  constructor(
    private featureService: FeatureService,
    private googleMapService: GoogleMapService,
    private drawingOptionsService: DrawingOptionsService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private sharedDataService: SharedDataService,
    private giasDialogService: GiasDialogService,
    private drawingService: DrawingService,
    private layerService: LayerService,
    private featureInformationService: FeatureInformationService
  ) {
    this.subscriptions.push(
      this.featureService
        .getFeatureSelezionate$()
        .subscribe(features => this.parseFeature(features[0] ?? null))
    );

    this.layers = [this.sharedDataService.getTipologiaLayerById(enum_LayerElementiGraficiStd.IMPIANTI)]; // For now IMPIANTI is the default layer
    this.selectedLayer = this.layers[0];
  }

  ngOnDestroy(): void {
    for (const sub of this.subscriptions) {
      sub.unsubscribe();
    }
  }

  save(): void {
    if (this.selectedMarkers.length == 0) {
      this.giasDialogService.baseError('gis.StrumentoScomponiModificaPunti', 'gis.NessunPuntoInserito');
      return;
    }

    if (this.selectedMarkers.length < 3) {
      this.giasDialogService.baseError('gis.StrumentoScomponiModificaPunti', 'gis.NonAbbastanzaPunti');
      return;
    }

    this.layerService.setLayerItemSelected([this.selectedLayer, true]);
    this.drawingService.fromPointsToPolygon(this.selectedMarkers.map(x => x.marker.getPosition()), true);
  }

  reset(): void {
    this.onReset.emit(this.selectedMarkers.length > 0);
  }

  parseFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp | null): void {
    if (feature == null) {
      return;
    }

    this.resetMarkers();
    this.featureInformationService.getPath(feature.properties.id).forEach(coords => this.addMarker(coords));
    this.googleMapGeoJsonService.seDeselezionaFeatureSelezionate(enum_OrigineChiamata.Mappa);
  }

  removeSelectedMarker(marker: Marker): void {
    const index = this.selectedMarkers.indexOf(marker);
    if (index == -1) {
      return;
    }

    const markers = [...this.selectedMarkers];
    markers.splice(index, 1);
    if (markers.length > 0 && !this.checkPolygonValidity(markers)) {
      return;
    }

    marker.selected = false;
    marker.marker.setLabel(null);
    if (index > -1) {
      this.selectedMarkers.splice(index, 1);
      this.selectedMarkers.forEach((m, i) => {
        m.id = (i + 1).toString();
        m.marker.setLabel(m.id)
      });
    }
  }

  onOutsideReset(): void {
    this.resetMarkers();
  }

  private resetMarkers(): void {
    for (const marker of this.markers) {
      marker.marker.setMap(null);
      // this.googleMapDataService.removeGoogleFeature(marker.feature);
    }

    this.markers = [];
    this.selectedMarkers = [];
  }

  private addMarker(coordinates: google.maps.LatLng): void {
    const markerOptions = this.drawingOptionsService.getMarkerDrawingOptions(enum_LayerElementiGraficiStd.DEEFAULT);
    markerOptions.position = new google.maps.LatLng(coordinates);
    markerOptions.map = this.googleMapService.googleMapWrapper.googleMap;
    markerOptions.visible = true;

    const gMarker = new google.maps.Marker(markerOptions);
    const feature = new google.maps.Data.Feature({ geometry: new google.maps.Data.Point(gMarker.getPosition()) });
    const marker = {
      marker: gMarker,
      selected: false,
      feature: feature,
      lat: gMarker.getPosition().lat(),
      lng: gMarker.getPosition().lng()
    } as Marker;
    gMarker.addListener('click', () => this.clickMarker(marker));

    this.markers.push(marker);
  }

  private clickMarker(marker: Marker): void {
    if (!marker.selected) {
      this.addSelectedMarker(marker);
      return;
    }

    this.removeSelectedMarker(marker);
  }

  private addSelectedMarker(marker: Marker): void {
    const markers = [...this.selectedMarkers];
    markers.push(marker);
    if (!this.checkPolygonValidity(markers)) {
      return;
    }

    marker.selected = true;
    marker.id = (this.selectedMarkers.length + 1).toString();
    marker.marker.setLabel(marker.id);
    this.selectedMarkers.push(marker);
  }

  private checkPolygonValidity(markers: Marker[]): boolean {
    const drawingManager = require('../../../../GiasJSLibraries/GIS-js-libraries/DrawingManager');

    const polygonOptions = this.drawingOptionsService.getPolygonDrawingOptions(enum_LayerElementiGraficiStd.DEEFAULT);
    polygonOptions.paths = markers.map(x => x.marker.getPosition());
    const polygon = new google.maps.Polygon(polygonOptions);

    const polygonValidator = new drawingManager.PolygonValidator(polygon.getPath());
    if (!polygonValidator.isValid) {
      drawingManager.DisplayErrorPolyLines(polygonValidator.intersection, this.googleMapService.googleMapWrapper.googleMap);
      return false;
    }

    return true;
  }
}

interface Marker {
  id: string;
  lat: number;
  lng: number;
  selected: boolean;
  marker: google.maps.Marker;
  feature: google.maps.Data.Feature;
}

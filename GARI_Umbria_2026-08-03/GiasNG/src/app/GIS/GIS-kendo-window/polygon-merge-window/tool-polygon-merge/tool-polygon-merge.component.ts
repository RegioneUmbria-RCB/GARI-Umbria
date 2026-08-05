import { Component, EventEmitter, Output } from '@angular/core';
import { DrawingService } from 'app/GIS/services/drawing.service';
import { FeatureService } from 'app/GIS/services/feature.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp, TipologiaLayer } from 'app/Service/api.service';
import { filter, map, tap } from 'rxjs';
import Polygon from 'jsts/org/locationtech/jts/geom/Polygon';
import GeometryFactory from 'jsts/org/locationtech/jts/geom/GeometryFactory';
import UnionOp from 'jsts/org/locationtech/jts/operation/union/UnionOp';
import { GoogleMapDataService } from 'app/GIS/services/google.maps-services/google-map-data.service';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { enum_OrigineChiamata } from 'app/GIS/GIS-enum/GIS-origine-chiamata';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { JstsService } from 'app/GIS/services/jsts.service';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';

@Component({
  standalone: false,
  selector: 'app-tool-polygon-merge',
  templateUrl: './tool-polygon-merge.component.html',
  styleUrls: ['./tool-polygon-merge.component.css']
})
export class ToolPolygonMergeComponent {
  @Output() onReset = new EventEmitter<boolean>()

  selectedLayer: TipologiaLayer | null = null;
  layers$ = this.layerService.ObservableLayer.asObservable();

  inputLayer$ = this.layerService.layerItemSelected$
    .pipe(
      filter(([_, visible]) => visible),
      map(([layer, _]) => layer),
      tap(layer => {
        // Close if an invalid layer is selected
        if (!isLayerMergeable(layer.id)) {
          this.kendoWindowsService.close(WindowTypes.PolygonMergeWindow);
        }
      })
    );

  features$ = this.featureService.getFeatureSelezionate$()
    .pipe(map(features => features.map(f => ({
      id: f.properties.id,
      geometry: this.featureInformationService.getGeometry(f.properties.id),
      name: this.getEtichetta(f),// only for layer 51 f.getProperty(enum_FeatureProperty.etichetta),
      code: +f.properties.Entita_Cod
    } as Feature))));

  constructor(
    private featureService: FeatureService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private googleMapDataService: GoogleMapDataService,
    private drawingService: DrawingService,
    private layerService: LayerService,
    private kendoWindowsService: KendoWindowsService,
    private giasMessageService: GiasMessageService,
    private featureInformationService: FeatureInformationService
  ) { }

  save(features: Feature[]): void {
    const polygons: google.maps.Polygon[] = this.getPolygons(features);
    let mergePolygonPath: Polygon = this.mergePolygons(polygons);
    mergePolygonPath = JstsService.deflate(mergePolygonPath);

    const coordinates = JstsService.toGoogleMaps(mergePolygonPath);
    coordinates.pop(); // Remove last coord from polygon due to JTST conversion

    const polygon = this.drawingService.getPolygonFromCoordinates(coordinates);

    const DrawingManager = require('../../../../GiasJSLibraries/GIS-js-libraries/DrawingManager');
    const polygonValidator = new DrawingManager.PolygonValidator(polygon.getPath());
    if (!polygonValidator.isValid) {
      this.giasMessageService.errorMessage("gis.ImpossibileAggiungereElemento", false, true);
      return;
    }

    this.minimize();
    this.drawingService.addDrawing(polygon, true);
  }

  reset(): void {
    this.onReset.emit(true);
  }

  unselectFeature(feature: Feature): void {
    const googleFeature = this.featureInformationService.getById(feature.id.toString());
    this.googleMapGeoJsonService.deselezionaFeature(googleFeature, true, false, false, enum_OrigineChiamata.Mappa);
  }

  private minimize(): void {
    const args = { ...this.kendoWindowsService.getWindowArgs(WindowTypes.PolygonMergeWindow) };
    args.startMinimized = true;
    this.kendoWindowsService.open(WindowTypes.PolygonMergeWindow, args);
  }

  private mergePolygons(polygons: google.maps.Polygon[]): Polygon {
    const geometryFactory = new GeometryFactory();
    let result: Polygon = geometryFactory.createPolygon(geometryFactory.createLinearRing(JstsService.fromGoogleMaps(polygons[0].getPath())));
    result = JstsService.inflate(result);
    result.normalize();

    for (let i = 1; i < polygons.length; i++) {
      let current = geometryFactory.createPolygon(geometryFactory.createLinearRing(JstsService.fromGoogleMaps(polygons[i].getPath())));
      current = JstsService.inflate(current);
      current.normalize();
      result = UnionOp.union(result, current);
    }

    return result;
  }

  private getPolygons(features: Feature[]): google.maps.Polygon[] {
    const polygons: google.maps.Polygon[] = [];
    for (const geometry of features.map(feature => feature.geometry)) {
      const paths = [];
      geometry.forEachLatLng(point => paths.push({ lat: point.lat(), lng: point.lng() }));
      // Add starting point as last point to "close" the polygon
      polygons.push(new google.maps.Polygon({ paths: [...paths, paths[0]] }));
    }
    return polygons;
  }

  private getEtichetta(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): string { 
    const text = feature.properties.Testo;
    const split = text.match(/CAP_N\+\§(.*)\|/);
    if (split == null) {
      return '';
    }

    return split[1]?.split('|')[0] ?? '';
  }
}

interface Feature {
  geometry: google.maps.Data.Geometry;
  id: string | number;
  name: string;
  code: number;
}

export function isLayerMergeable(layerId: string): boolean {
  return layerId === enum_LayerElementiGraficiStd.Mappe_Prescrizione; // || +layer.id > 1000000
}

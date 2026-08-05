import { Injectable } from "@angular/core";
import { FeatureType, GeoJson_Geometry_New } from "app/Model/GIS/GisDataReadRval_New";
import { enum_FeatureGeometryType } from "../GIS-enum/GIS-feature";
import Feature = google.maps.Data.Feature;
import { FeatureInformationService } from "./feature-information.service";
import {GeoJsonUtils} from '../utils/geo-json.utils';
import {GoogleMapUtils} from '../utils/google-map.utils';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from "app/Service/api.service";

@Injectable()
export class WKTService {

  private Terraformer = require('@terraformer/wkt');

  constructor(private featureInformationService: FeatureInformationService) {}

  public geoJsonToWKT(geoJson: GeoJson_Geometry_New): string {
    let wkt: string = this.Terraformer.geojsonToWKT(geoJson);
    return wkt;
  }

  public wktToGeoJson(wkt: string): GeoJson_Geometry_New {
    let geoJson: GeoJson_Geometry_New = this.Terraformer.wktToGeoJSON(wkt);
    return geoJson;
  }

  public featureGeometryToWKT(feature: Feature): string {
    const featureGeometry = feature.getGeometry();
    return this.geometryToWKT(featureGeometry);
  }

  public geometryToWKT(featureGeometry: google.maps.Data.Geometry): string {
    const featureGeometryType = featureGeometry.getType();

    let featureWkt = "";
    let featureGeometryNew: GeoJson_Geometry_New = null;

    let featureGeometryNewType = 0;
    let featureGeometryNewData = "";

    let vettoreCoordinate1D: number[];
    let matriceCoordinate2D: number[][];
    let matriceCoordinate3D: number[][][];

    switch (featureGeometryType) {
      case enum_FeatureGeometryType.Point:
        featureGeometryNewType = FeatureType.Point;
        featureGeometry.forEachLatLng(latlng => {
          vettoreCoordinate1D = [];
          vettoreCoordinate1D.push(latlng.lng());
          vettoreCoordinate1D.push(latlng.lat());
        });
        featureGeometryNewData = JSON.stringify(vettoreCoordinate1D);
        break;
      case enum_FeatureGeometryType.LineString:
        featureGeometryNewType = FeatureType.LineString;
        matriceCoordinate2D = [];
        featureGeometry.forEachLatLng(latlng => {
          vettoreCoordinate1D = [];
          vettoreCoordinate1D.push(latlng.lng());
          vettoreCoordinate1D.push(latlng.lat());
          matriceCoordinate2D.push(vettoreCoordinate1D);
        });
        featureGeometryNewData = JSON.stringify(matriceCoordinate2D);
        break;
      case enum_FeatureGeometryType.Polygon:
        featureGeometryNewType = FeatureType.Polygon;
        matriceCoordinate2D = [];
        matriceCoordinate3D = [];
        featureGeometry.forEachLatLng(latlng => {
          vettoreCoordinate1D = [];
          vettoreCoordinate1D.push(latlng.lng());
          vettoreCoordinate1D.push(latlng.lat());
          matriceCoordinate2D.push(vettoreCoordinate1D);
        });
        // --------- chiudo l'anello delle coordinate ---------
        matriceCoordinate2D.push(matriceCoordinate2D[0]);
        // ----------------------------------------------------
        matriceCoordinate3D.push(matriceCoordinate2D);
        featureGeometryNewData = JSON.stringify(matriceCoordinate3D);
        break;
    }

    featureGeometryNew = new GeoJson_Geometry_New(featureGeometryNewType, featureGeometryNewData);

    featureWkt = this.geoJsonToWKT(featureGeometryNew);
    return featureWkt;
  }

  public geometryToHiddenPunti(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): string {
    const vettoreCoordinate: string[] = [];
    this.featureInformationService.getPath(feature.properties.id).forEach(latlng => {
      const coordinatePunto = `(${latlng.lat()}, ${latlng.lng()})`
      vettoreCoordinate.push(coordinatePunto);
    });

    return  vettoreCoordinate.join(",");
  }

  public polygonToDataGeometry(polygon: google.maps.Polygon): google.maps.Data.Geometry {

    let coordinatePoligono: google.maps.LatLng[] = [];

    polygon.getPath().getArray().forEach(ll => {
      coordinatePoligono.push(ll);
    });

    let dataLinearRing = new google.maps.Data.LinearRing(coordinatePoligono);
    let dataPolygon = new google.maps.Data.Polygon([dataLinearRing]);
    let dataGeometry: google.maps.Data.Geometry = dataPolygon;

    return dataGeometry;

  }

  public markerToDataGeometry(marker: google.maps.Marker): google.maps.Data.Geometry {
    let cooordinatePoint: google.maps.LatLng = marker.getPosition();

    let datamarker = new google.maps.Data.Point(cooordinatePoint);
    let dataGeometry: google.maps.Data.Geometry = datamarker;

    return dataGeometry;
  }

  public polylineToDataGeometry(polyline: google.maps.Polyline): google.maps.Data.Geometry {
    let coordinatePolyline: google.maps.LatLng[] = [];

    polyline.getPath().getArray().forEach(ll => {
      coordinatePolyline.push(ll);
    });

    let dataLinearString = new google.maps.Data.LineString(coordinatePolyline);
    // let dataPolygon = new google.maps.Data.LineString([dataLinearRing]);
    let dataGeometry: google.maps.Data.Geometry = dataLinearString;

    return dataGeometry;
  }

  public centerGMapOnWKT(wkt: string, gMap: google.maps.Map): void {
    let gj = this.wktToGeoJson(wkt);
    let geometry = GeoJsonUtils.getGeometryFromGeoJsonGeometry(gj.type, gj.coordinates);
    let bounds = GoogleMapUtils.getInitialBounds([geometry]);
    gMap.fitBounds(bounds);
    gMap.setCenter(bounds.getCenter());
  }
}

import { FeatureType, FeatureTypeUtil, GeoJson_Geometry_New } from "app/Model/GIS/GisDataReadRval_New";
import { enum_FeatureProperty } from "../GIS-enum/GIS-feature";
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp, GeoJSONAgroGisProp } from "app/Service/api.service";

export class GeoJsonUtils {
  public static getGeometryFromGeoJsonGeometry(stringType: string, data: any): google.maps.Data.Geometry {
    const type = FeatureTypeUtil.fromString(stringType);
    switch (type) {
      case FeatureType.Point:
        const point = data as number[];
        return new google.maps.Data.Point(new google.maps.LatLng(point[1], point[0]));
      case FeatureType.MultiPoint:
        const points = data as number[][];
        return new google.maps.Data.MultiPoint(points.map(coord => new google.maps.LatLng(coord[1], coord[0])));
      case FeatureType.LineString:
        const linePoints = data as number[][];
        return new google.maps.Data.LineString(linePoints.map(coord => new google.maps.LatLng(coord[1], coord[0])));
      case FeatureType.MultiLineString:
        const stringPoints = data as number[][][];
        return new google.maps.Data.MultiLineString(stringPoints.map(coords => coords.map(coord => new google.maps.LatLng(coord[1], coord[0]))));
      case FeatureType.Polygon:
        const polygonPoints = data as number[][][];
        return new google.maps.Data.Polygon(polygonPoints.map(coords => coords.map(coord => new google.maps.LatLng(coord[1], coord[0]))));
      case FeatureType.MultiPolygon:
        const multiElements = data as number[][][][];
        return new google.maps.Data.MultiPolygon(multiElements.map(elements => elements.map(coords => coords.map(coord => new google.maps.LatLng(coord[1], coord[0])))));
    }
  }

  public static getGeoJsonGeometryFromGeometry(stringType: string, data: google.maps.Data.Geometry): any {
    const type = FeatureTypeUtil.fromString(stringType);
    switch (type) {
      case FeatureType.Point:
        const point = data as google.maps.Data.Point;
        return GeoJsonUtils.getGeoJsonLatLng(point.get());
      case FeatureType.MultiPoint:
        const points = data as google.maps.Data.MultiPoint;
        return points.getArray().map(p => GeoJsonUtils.getGeoJsonLatLng(p));
      case FeatureType.LineString:
        const linePoints = data as google.maps.Data.LineString;
        return linePoints.getArray().map(p => GeoJsonUtils.getGeoJsonLatLng(p));
      case FeatureType.MultiLineString:
        const stringPoints = data as google.maps.Data.MultiLineString;
        return stringPoints.getArray().map(ps => ps.getArray().map(p => GeoJsonUtils.getGeoJsonLatLng(p)));
      case FeatureType.Polygon:
        const polygonPoints = data as google.maps.Data.Polygon;
        return polygonPoints.getArray().map(ps => {
          const result = ps.getArray().map(p => GeoJsonUtils.getGeoJsonLatLng(p));
          result.push(result[0]);
          return result;
        });
      case FeatureType.MultiPolygon:
        const multiElements = data as google.maps.Data.MultiPolygon;
        return multiElements.getArray().map(polygon => polygon.getArray().map(ps => {
          const result = ps.getArray().map(p => GeoJsonUtils.getGeoJsonLatLng(p));
          result.push(result[0]);
          return result;
        }));
    }
  }

  public static computeArea(stringType: string, coordinates: object): number {
    const type = FeatureTypeUtil.fromString(stringType);
    switch (type) {
      case FeatureType.Point:
      case FeatureType.MultiPoint:
      case FeatureType.LineString:
        return 1;
      case FeatureType.MultiLineString:
      case FeatureType.Polygon:
        const coords = (coordinates as number[][][])[0].map(x => new google.maps.LatLng(x[1], x[0]));
        return google.maps.geometry.spherical.computeArea(coords);
    }

    return 0;
  }

  public static computePosition(stringType: string, coordinates: object): google.maps.LatLng {
    const type = FeatureTypeUtil.fromString(stringType);
    const bounds = new google.maps.LatLngBounds();
    const coords: google.maps.LatLng[] = [];

    switch (type) {
      case FeatureType.Point:
        coords.push(new google.maps.LatLng(coordinates[1], coordinates[0]));
        break;
      case FeatureType.MultiPoint:
      case FeatureType.LineString:
        for (const coord of (coordinates as number[][])) {
          coords.push(new google.maps.LatLng(coord[1], coord[0]));
        }
        break;
      case FeatureType.MultiPolygon:
        // TODO: Vanni 22/12/2023, intervenire qui per gestire il multipolygon
        for (const coord1 of (coordinates as object[])) {
          for (const coord of (coord1 as number[][][])[0]) {
            coords.push(new google.maps.LatLng(coord[1], coord[0]));
          }
        }
        break;
      case FeatureType.MultiLineString:
      case FeatureType.Polygon:
        for (const coord of (coordinates as number[][][])[0]) {
          coords.push(new google.maps.LatLng(coord[1], coord[0]));
        }
        break;
    }

    for (let i = 0; i < coords.length; i++) {
      bounds.extend(coords[i]);
    }

    return bounds.getCenter();
  }

  public static featureToGeoJson(feature: google.maps.Data.Feature): GeoJson_Feature_New_1OfGeoJSONAgroGisProp {
    const properties: GeoJSONAgroGisProp = {
      id: feature.getId().toString(),
      layer: feature.getProperty(enum_FeatureProperty.layer) as string,
      StandardEntita_layerDiAppartenenza: feature.getProperty(enum_FeatureProperty.layerAppartenenza) as string,
      StandardEntita_layerDiAppartenenza_Des: feature.getProperty(enum_FeatureProperty.layerAppartenenzaDes) as string,
      StandardEntita_layerDiAppartenenza_Icona32: feature.getProperty(enum_FeatureProperty.layerAppartenenzaIcona32) as string,
      ParametriVisualizzazioneLayer: feature.getProperty(enum_FeatureProperty.parametriVisualizzazioneLayer) as string,
      tipoicona: feature.getProperty(enum_FeatureProperty.tipoIcona) as string,
      zindex: feature.getProperty(enum_FeatureProperty.zIndex) as string,
      Entita_Cod: feature.getProperty(enum_FeatureProperty.entitaCod) as string,
      veg_cod: feature.getProperty(enum_FeatureProperty.vegCod) as string,
      flag_gps: feature.getProperty(enum_FeatureProperty.flagGps) as string,
      etichetta: feature.getProperty(enum_FeatureProperty.etichetta) as string,
      inserimento: feature.getProperty(enum_FeatureProperty.inserimento) as string,
      modifica: feature.getProperty(enum_FeatureProperty.modifica) as string,
      cancellazione: feature.getProperty(enum_FeatureProperty.cancellazione) as string,
      informazioni: feature.getProperty(enum_FeatureProperty.informazioni) as string,
      chiavealbero: feature.getProperty(enum_FeatureProperty.chiaveAlbero) as string,
      Testo: feature.getProperty(enum_FeatureProperty.testo) as string,
      AppIdRate: feature.getProperty(enum_FeatureProperty.appIdRate) as string,
      TipologiaGML: feature.getProperty(enum_FeatureProperty.tipologiaGML) as string,
      InOsservazione: feature.getProperty(enum_FeatureProperty.inOsservazione) as number,
      Colore_Primario: feature.getProperty(enum_FeatureProperty.colorePrimario) as string,
      Colore_Retinatura: feature.getProperty(enum_FeatureProperty.coloreRetinatura) as string,
      Trasparenza: feature.getProperty(enum_FeatureProperty.trasparenza) as number,
      Area_Cod: feature.getProperty(enum_FeatureProperty.Area_Cod) as number,
      Entita_GUID: feature.getProperty(enum_FeatureProperty.entitaGuid) as string,
      GMapsZoomLevel: feature.getProperty(enum_FeatureProperty.GMapsZoomLevel) as number,
      TotalOriginalArea: feature.getProperty(enum_FeatureProperty.TotalOriginalArea) as number,
      TotalOriginalFeatureNumber: feature.getProperty(enum_FeatureProperty.TotalOriginalFeatureNumber) as number,
      Clustered: feature.getProperty(enum_FeatureProperty.Clustered) as string
    };

    const type = feature.getGeometry().getType();
    return {
      properties: properties,
      geometry: {
        type: type,
        coordinates: this.getGeoJsonGeometryFromGeometry(type, feature.getGeometry())
      },
      type: 'Feature'
    } as GeoJson_Feature_New_1OfGeoJSONAgroGisProp;
  }

  public static polygonToGeoJson(polygon: google.maps.Polygon): GeoJson_Geometry_New {
    let coordinates: number[][][] = [polygon.getPath().getArray().map(ll => [ll.lng(), ll.lat()])];
    //Chiudo l'anello delle coordinate inserendo sul fondo copia della prima
    coordinates[0].push(coordinates[0][0]);

    return new GeoJson_Geometry_New(FeatureType.Polygon, JSON.stringify(coordinates));
  }

  public static markerToGeoJson(marker: google.maps.Marker): GeoJson_Geometry_New {
    let coordinates: number[] = [marker.getPosition().lng(), marker.getPosition().lat()];
    return new GeoJson_Geometry_New(FeatureType.Point, JSON.stringify(coordinates));
  }

  public static polylineToGeoJson(polyline: google.maps.Polyline) {
    let coordinates: number[][] = polyline.getPath().getArray().map(ll => [ll.lng(), ll.lat()]);
    return new GeoJson_Geometry_New(FeatureType.LineString, JSON.stringify(coordinates));
  }

  private static getGeoJsonLatLng(latLng: google.maps.LatLng): [number, number] {
    return [latLng.lng(), latLng.lat()];
  }
}

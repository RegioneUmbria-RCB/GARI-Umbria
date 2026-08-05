import { JstsService } from "./jsts.service";
import GeometryFactory from 'jsts/org/locationtech/jts/geom/GeometryFactory';
import OverlayOp from 'jsts/org/locationtech/jts/operation/overlay/OverlayOp';
import { Mercator } from "../utils/mercator.utils";

export class PolygonInterceptionUtils {
  static getPointPolygonIntersection(polygon: google.maps.Data.Polygon, coord: google.maps.Point, zoom: number) {
    const tile = Mercator.getTileBounds({ x: coord.x, y: coord.y, zoom: zoom });
    return PolygonInterceptionUtils.getTilePolygonIntersection(tile, polygon);
  }

  static getTilePolygonIntersection(tile: google.maps.LatLng[], polygon: google.maps.Data.Polygon) {
    const geometryFactory = new GeometryFactory();
    const p2 = geometryFactory.createPolygon(geometryFactory.createLinearRing(JstsService.fromGoogleMaps(new google.maps.MVCArray(tile))));

    const path = polygon.getArray()[0];
    const p1 = geometryFactory.createPolygon(geometryFactory.createLinearRing(JstsService.fromGoogleMaps(path)));

    const intersection = JstsService.toGoogleMaps(OverlayOp.intersection(p1, p2));
    return intersection;
  }
}

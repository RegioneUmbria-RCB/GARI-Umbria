export class GoogleMapUtils {
  // Coords for Rome, used to extend bounds if the center is at (0,0)
  public static ROME_COORDS: [number, number][] = [
    [42.430981, 11.454983],
    [41.580960, 13.622036]
  ];

  public static getInitialBounds(geometries: google.maps.Data.Geometry[]) {
    const bounds = new google.maps.LatLngBounds();
    for (const geometry of geometries) {
      GoogleMapUtils.extendInitialBounds(geometry, bounds);
    }

    const center = bounds.getCenter();
    if ((center.lat() % 180) == 0 && (center.lng() % 180) == 0) {
      GoogleMapUtils.ROME_COORDS.forEach(coord => bounds.extend(new google.maps.LatLng(coord[0], coord[1])));
    }

    return bounds;
  }

  public static extendInitialBounds(geometry: google.maps.Data.Geometry, bounds: google.maps.LatLngBounds): void {
    const coords = GoogleMapUtils.getPointsFromGeometry(geometry);
    for (const coord of coords) {
      bounds.extend(coord);
    }
  }

  public static getPointFromLatLng(latLng: google.maps.LatLng, projection: google.maps.Projection): google.maps.Point {
    return projection.fromLatLngToPoint(latLng);
  }

  public static getPixelDistanceFromLatLng(a: google.maps.LatLng, b: google.maps.LatLng, projection: google.maps.Projection, zoom: number): number {
    const p1 = GoogleMapUtils.getPointFromLatLng(a, projection);
    const p2 = GoogleMapUtils.getPointFromLatLng(b, projection);
    const pixelSize = Math.pow(2, -zoom);
    return GoogleMapUtils.getPixelDistanceFromPoint(p1, p2, pixelSize);
  }

  public static getPixelDistanceFromPoint(p1: google.maps.Point, p2: google.maps.Point, pixelSize: number): number {
    return Math.sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y)) / pixelSize;
  }

  public static isPolygonInViewport(map: google.maps.Map, inputPolygon: google.maps.Data.Polygon): boolean {
    const polygon = new google.maps.Polygon({ paths: inputPolygon.getArray().flatMap(path => path.getArray()) });
    const isPolygonVisible = polygon.getPath().getArray().some(coord => map.getBounds().contains(coord));
    const isViewportInPolygon = GoogleMapUtils.getViewportAsPolygon(map).getPaths().getArray().some(path => path.getArray().some(coord => google.maps.geometry.poly.containsLocation(coord, polygon)));

    return isPolygonVisible || isViewportInPolygon;
  }

  public static getViewportAsPolygon(map: google.maps.Map): google.maps.Polygon {
    const bounds = map.getBounds();
    return GoogleMapUtils.boundsToPolygon(bounds);
  }

  public static getMapBoundsWKT(bounds: google.maps.LatLngBounds): string {
    const neLat = bounds.getNorthEast().lat();
    const neLng = bounds.getNorthEast().lng();
    const swLat = bounds.getSouthWest().lat();
    const swLng = bounds.getSouthWest().lng();

    return `POLYGON((` +
      `${swLng} ${swLat},` +
      `${neLng} ${swLat},` +
      `${neLng} ${neLat},` +
      `${swLng} ${neLat},` +
      `${swLng} ${swLat}))`;
  }

  public static rectangleToPolygon(rectangle: google.maps.Rectangle): google.maps.Polygon {
    const bounds = rectangle.getBounds();
    return GoogleMapUtils.boundsToPolygon(bounds);
  }

  public static boundsToPolygon(bounds: google.maps.LatLngBounds): google.maps.Polygon {
    const markerNE = bounds.getNorthEast();
    const markerSW = bounds.getSouthWest();
    const markerNW = new google.maps.LatLng({ lat: markerNE.lat(), lng: markerSW.lng() });
    const markerSE = new google.maps.LatLng({ lat: markerSW.lat(), lng: markerNE.lng() });

    return new google.maps.Polygon({ paths: [markerNE, markerNW, markerSW, markerSE] });
  }

  public static getPointsFromGeometry(data: google.maps.Data.Geometry): google.maps.LatLng[] {
    const result: google.maps.LatLng[] = [];
    data.forEachLatLng(latlng => result.push(latlng));
    return result;
  }
}
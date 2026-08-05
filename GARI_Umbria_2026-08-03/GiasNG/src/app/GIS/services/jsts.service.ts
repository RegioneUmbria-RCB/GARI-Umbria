import { Injectable } from '@angular/core';
import Polygon from 'jsts/org/locationtech/jts/geom/Polygon';
import Coordinate from 'jsts/org/locationtech/jts/geom/Coordinate';
import BufferParameters from 'jsts/org/locationtech/jts/operation/buffer/BufferParameters';
import BufferOp from 'jsts/org/locationtech/jts/operation/buffer/BufferOp';

@Injectable()
export class JstsService {
  public static fromGoogleMaps(points: google.maps.MVCArray<google.maps.LatLng> | google.maps.Data.LinearRing): Coordinate[] {
    const coordinates = [];
    for (const point of points.getArray()) {
      coordinates.push(new Coordinate(point.lat(), point.lng()));
    }

    if (coordinates[0] != coordinates[coordinates.length - 1]) {
      coordinates.push(coordinates[0]);
    }

    return coordinates;
  }

  public static toGoogleMaps(geometry: Polygon): google.maps.LatLng[] {
    const result = [];
    for (const coordinate of geometry.getCoordinates()) {
      result.push(new google.maps.LatLng(coordinate.x, coordinate.y));
    }

    return result;
  }

  public static deflate(geometry: Polygon): Polygon {
    const bufferParameters = JstsService.getBufferParameters();
    const buffered = BufferOp.bufferOp(geometry, -.0001, bufferParameters);
    buffered.setUserData(geometry.getUserData());
    return buffered;
  }

  public static inflate(geometry: Polygon): Polygon {
    const bufferParameters = JstsService.getBufferParameters();
    const buffered = BufferOp.bufferOp(geometry, .0001, bufferParameters);
    buffered.setUserData(geometry.getUserData());
    return buffered;
  }

  private static getBufferParameters() {
    const bufferParameters = new BufferParameters();
    bufferParameters.setEndCapStyle(BufferParameters.CAP_ROUND);
    bufferParameters.setJoinStyle(BufferParameters.JOIN_MITRE);
    return bufferParameters;
  }
}

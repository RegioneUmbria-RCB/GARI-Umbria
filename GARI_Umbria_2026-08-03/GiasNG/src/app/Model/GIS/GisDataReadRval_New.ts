export enum FeatureType {
  Point = 1,
  MultiPoint = 2,
  LineString = 3,
  MultiLineString = 4,
  Polygon = 5,
  MultiPolygon = 6,
  Raster = 100,
}

export class GeoJson_Geometry_New {
  _type: FeatureType;
  public coordinates: object;

  //get & set
  get type(): string {
    return FeatureType[this._type];
  }

  set type(value: string) {
    this._type = (<any>FeatureType)[value];
  }

  //costruttori
  constructor(type: FeatureType, data: string) {
    this._type = type;
    switch (type) {
      case FeatureType.Point: {
        let vettoreCoordinate: number[] = JSON.parse(data);
        this.coordinates = vettoreCoordinate;
        break;
      }
      case FeatureType.MultiPoint:
      case FeatureType.LineString: {
        let matriceCoordinate: number[][] = JSON.parse(data);
        this.coordinates = matriceCoordinate;
        break;
      }
      case FeatureType.MultiLineString:
      case FeatureType.Polygon: {
        let matriceCoordinate3D: number[][][] = JSON.parse(data);
        this.coordinates = matriceCoordinate3D;
        break;
      }
      case FeatureType.MultiPolygon: {
        let matriceCoordinate4D: number[][][][] = JSON.parse(data);
        this.coordinates = matriceCoordinate4D;
        break;
      }
    }
  }
}

export class FeatureTypeUtil {
  public static fromString(type: string): FeatureType {
    switch (type) {
      case 'Point':
        return FeatureType.Point;
      case 'MultiPoint':
        return FeatureType.MultiPoint;
      case 'LineString':
        return FeatureType.LineString;
      case 'MultiLineString':
        return FeatureType.MultiLineString;
      case 'Polygon':
        return FeatureType.Polygon;
      case 'MultiPolygon':
        return FeatureType.MultiPolygon;
      case 'Raster':
        return FeatureType.Raster;
    }
    return null;
  }
}

export class GeoJSONAgroGisPropTreeNode {
  public id: string;
  public imageUrl: string;
  public style: string;
  public text: string;
  public type: string;
  public startDate: Date;
  public endDate: Date
}

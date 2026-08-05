import { Injectable } from "@angular/core";
import { GeoJsonUtils } from "../utils/geo-json.utils";
import { GoogleMapUtils } from "../utils/google-map.utils";
import { Observable, Subject, takeUntil } from 'rxjs';
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { FeatureService } from './feature.service';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from "app/Service/api.service";

@Injectable()
export class FeatureInformationService {
  private featuresChangedSubject = new Subject<void>();
  private featuresById = new Map<string, GeoJson_Feature_New_1OfGeoJSONAgroGisProp>();
  private areas = new Map<string, number>();
  private totalOriginalAreas = new Map<string, number>();
  private totalOriginalFeatures = new Map<string, number>();
  private paths = new Map<string, google.maps.LatLng[]>();
  private geometries = new Map<string, google.maps.Data.Geometry>();
  private positionsLatLng = new Map<string, google.maps.LatLng>();
  private positionsPoint = new Map<string, google.maps.Point>();
  private tmpFeatures = new Set<string>();

  private _signal$: Subject<void> = new Subject<void>();

  public get signal$() {
    return this._signal$;
  }

  constructor(
    private featureService: FeatureService
  ) {
    this.featureService.featureDeleted$.pipe(
      takeUntil(this._signal$)
    ).subscribe(id => {
      //this.featuresById;
      console.log(this.getById(id));
      console.log(id);
      this.removeById(id);
    });
  }

  public get featuresChanged$(): Observable<void> {
    return this.featuresChangedSubject.asObservable();
  }

  public addFeatures(projection: google.maps.Projection, isTmp: boolean, ...features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]): void {
    for (const feature of features) {
      const id = feature.properties.id;
      const position = GeoJsonUtils.computePosition(feature.geometry.type, feature.geometry.coordinates);
      const area = GeoJsonUtils.computeArea(feature.geometry.type, feature.geometry.coordinates);
      const geometry = GeoJsonUtils.getGeometryFromGeoJsonGeometry(feature.geometry.type, feature.geometry.coordinates);
      const totalOriginalAreas = feature.properties.Clustered == 'True' ? +feature.properties.Testo : feature.properties.TotalOriginalArea;
      const totalOriginalFeatures = feature.properties.Clustered == 'True' ? +feature.properties.Testo : feature.properties.TotalOriginalFeatureNumber;
      this.geometries.set(id, geometry);
      this.paths.set(id, GoogleMapUtils.getPointsFromGeometry(geometry));
      this.positionsLatLng.set(id, position);
      this.positionsPoint.set(id, GoogleMapUtils.getPointFromLatLng(position, projection));
      this.areas.set(id, area);
      this.totalOriginalFeatures.set(id, totalOriginalFeatures);
      this.totalOriginalAreas.set(id, totalOriginalAreas);
      this.featuresById.set(id, feature);

      if (isTmp) {
        this.tmpFeatures.add(id);
      }
    }

    this.featuresChangedSubject.next();
  }

  public getById(id: string): GeoJson_Feature_New_1OfGeoJSONAgroGisProp | null {
    return this.featuresById.get(id);
  }

  public getPosition(id: string): google.maps.LatLng | null {
    return this.positionsLatLng.get(id);
  }

  public getPositionPoint(id: string): google.maps.Point | null {
    return this.positionsPoint.get(id);
  }

  public getArea(id: string): number | null {
    return this.areas.get(id);
  }

  public getTotalOriginalArea(id: string): number | null {
    return this.totalOriginalAreas.get(id);
  }

  public getTotalOriginalFeatures(id: string): number | null {
    return this.totalOriginalFeatures.get(id);
  }

  public getGeometry(id: string): google.maps.Data.Geometry | null {
    return this.geometries.get(id);
  }

  public getPath(id: string): google.maps.LatLng[] | null {
    return this.paths.get(id);
  }

  public getByLayer(layerId: string): GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] {
    return [...this.featuresById.values()].filter(x => x.properties.layer == layerId);
  }

  public getByNode(layerId: string, nodeKey: string): GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] {
    if (nodeKey == null || nodeKey == '') {
      return [];
    }

    return this.getByLayer(layerId).filter(f => f.properties.chiavealbero == FunzioniComuniService.chiaveAlberoBigToRidotta(nodeKey) || f.properties.chiavealbero == nodeKey);
  }

  public getAll(): GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] {
    const result = [];
    for (const [id, feature] of this.featuresById.entries()) {
      if (!this.tmpFeatures.has(id)) {
        result.push(feature);
      }
    }

    return result;
  }

  public getNumber(): number {
    return this.featuresById.size;
  }

  public clear(): void {
    this.featuresById.clear();
    this.areas.clear();
    this.positionsLatLng.clear();
    this.positionsPoint.clear();
    this.featuresChangedSubject.next();
  }

  private removeById(id: string) {
    if (!this.featuresById.delete(id)) {
      console.error('Feature non rimossa: ' + id);
    }
  }
}

import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, Subject } from "rxjs";

@Injectable()
export class GoogleMapFeatureService {
  private addingFeaturesSubject = new Subject<google.maps.Data.Feature[]>();
  private addFeaturesSubject = new Subject<google.maps.Data.Feature[]>();
  private addedFeaturesSubject = new Subject<google.maps.Data.Feature[]>();

  private removingFeaturesSubject = new Subject<string[]>(); // Just need ids
  private removeFeaturesSubject = new Subject<google.maps.Data.Feature[]>();
  private removedFeaturesSubject = new Subject<google.maps.Data.Feature[]>();

  private loadingGeoJsonSubject = new Subject<object>();
  private loadGeoJsonSubject = new Subject<object>();
  private loadedGeoJsonSubject = new Subject<object>();

  private featuresLoadedSubject = new BehaviorSubject<google.maps.Data.Feature[]>([]);
  private featuresMap = new Map<string, google.maps.Data.Feature>();

  public addFeatures(...features: google.maps.Data.Feature[]): void {
    this.addingFeaturesSubject.next(features);
    this.addFeaturesSubject.next(features);
  }

  public addedFeatures(...features: google.maps.Data.Feature[]): void {
    const newFeatures = [...this.featuresLoadedSubject.value];
    for (const feature of features) {
      this.featuresMap.set(feature.getId().toString(), feature);
      newFeatures.push(feature);
    }

    this.featuresLoadedSubject.next(newFeatures);
    this.addedFeaturesSubject.next(features);
  }

  public get addingFeatures$(): Observable<google.maps.Data.Feature[]> {
    return this.addingFeaturesSubject.asObservable();
  }

  public get addFeatures$(): Observable<google.maps.Data.Feature[]> {
    return this.addFeaturesSubject.asObservable();
  }

  public get addedFeatures$(): Observable<google.maps.Data.Feature[]> {
    return this.addedFeaturesSubject.asObservable();
  }

  public removeFeatures(...featuresIds: string[]): void {
    this.removingFeaturesSubject.next(featuresIds);
    const features = featuresIds.map(id => this.featuresMap.get(id)).filter(x => x != null);
    this.removeFeaturesSubject.next(features);
  }

  public removedFeatures(...features: google.maps.Data.Feature[]): void {
    const newFeatures = this.featuresLoadedSubject.value.filter(feature => !features.some(f => f.getId().toString() == feature.getId().toString()));
    this.featuresLoadedSubject.next(newFeatures);
    this.removedFeaturesSubject.next(features);
  }

  public removeAll(): void {
    const features = this.featuresLoadedSubject.value;
    const ids = features.map(f => f.getId().toString());
    this.removingFeaturesSubject.next(ids);
    this.removeFeaturesSubject.next(features);
  }

  public get removingFeatures$(): Observable<string[]> {
    return this.removingFeaturesSubject.asObservable();
  }

  public get removeFeatures$(): Observable<google.maps.Data.Feature[]> {
    return this.removeFeaturesSubject.asObservable();
  }

  public get removedFeatures$(): Observable<google.maps.Data.Feature[]> {
    return this.removedFeaturesSubject.asObservable();
  }

  public loadByGeoJson(geoJson: object): void {
    this.loadingGeoJsonSubject.next(geoJson);
    this.loadGeoJsonSubject.next(geoJson);
  }

  public loadedByGeoJson(geoJson: object, ...features: google.maps.Data.Feature[]): void {
    this.loadedGeoJsonSubject.next(geoJson);
    this.addedFeatures(...features);
  }

  public get loadingGeoJson$(): Observable<object> {
    return this.loadingGeoJsonSubject.asObservable();
  }

  public get loadGeoJson$(): Observable<object> {
    return this.loadGeoJsonSubject.asObservable();
  }

  public get loadedGeoJsonSubject$(): Observable<object> {
    return this.loadedGeoJsonSubject.asObservable();
  }

  public getCurrentFeatures(): google.maps.Data.Feature[] {
    return this.featuresLoadedSubject.value;
  }

  public getByid(featureId: string): google.maps.Data.Feature | null {
    return this.featuresMap.get(featureId);
  }
}
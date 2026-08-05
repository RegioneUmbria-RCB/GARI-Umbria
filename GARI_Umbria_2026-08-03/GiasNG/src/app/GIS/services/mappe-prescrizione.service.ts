import { Injectable } from "@angular/core";
import { GeoJson_New_1OfGeoJSONAgroGisProp } from "app/Service/api.service";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable()
export class MappePrescrizioneService {
  private lastGeoJsonLoadedSubject = new BehaviorSubject<MappePrescrizioneGeoJson | null>(null);

  get lastGeoJsonLoaded$(): Observable<MappePrescrizioneGeoJson> {
    return this.lastGeoJsonLoadedSubject.asObservable();
  }

  get lastGeoJsonLoaded(): MappePrescrizioneGeoJson {
    return this.lastGeoJsonLoadedSubject.value;
  }

  public setGeoJsonLoaded(geoJson: MappePrescrizioneGeoJson): void {
    this.lastGeoJsonLoadedSubject.next(geoJson);
  }
}

interface MappePrescrizioneGeoJson {
  allegatoCode: number;
  geoJson: GeoJson_New_1OfGeoJSONAgroGisProp;
}
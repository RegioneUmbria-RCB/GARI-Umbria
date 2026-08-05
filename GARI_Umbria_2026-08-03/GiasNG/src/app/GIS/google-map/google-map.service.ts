import { Injectable } from "@angular/core";
import { GoogleMap } from "@angular/google-maps";
import { BehaviorSubject, Observable, Subject, filter } from "rxjs";

@Injectable()
export class GoogleMapService {
  private loadedSubject = new BehaviorSubject<boolean>(false);
  private idleSubject = new Subject<[google.maps.LatLng, number]>();
  private _googleMapWrapper: GoogleMap;

  public get googleMapWrapper(): GoogleMap {
    return this._googleMapWrapper;
  }

  public set googleMapWrapper(value: GoogleMap) {
    this._googleMapWrapper = value;
    this.loadedSubject.next(true);
  }

  public get loaded$(): Observable<boolean> {
    return this.loadedSubject.asObservable()
      .pipe(filter(loaded => loaded));
  }

  public get idle$(): Observable<[google.maps.LatLng, number]> {
    return this.idleSubject.asObservable();
  }

  public nextIdle(position: google.maps.LatLng, zoom: number): void {
    this.idleSubject.next([position, zoom]);
  }
}

import { Injectable } from "@angular/core";
import { Subject, debounceTime } from "rxjs";

@Injectable()
export class GoogleMapMovementControlService {
  private idleSubject = new Subject<HistoryEntry>();
  private history: HistoryEntry[] = [];
  private current = -1;

  constructor() {
    this.idleSubject
      .pipe(debounceTime(100))
      .subscribe(entry => this.setIdle(entry));
  }

  public nextIdle(center: google.maps.LatLng, zoom: number): void {
    if (this.current != -1) {
      const current = this.history[this.current];
      if (current.center.equals(center) && current.zoom == zoom) {
        return;
      }
    }

    this.idleSubject.next({ center: center, zoom: zoom });
  }

  public goToNextPosition(googleMap: google.maps.Map): void {
    if (this.isLast()) {
      return null;
    }

    const latlng = this.history[++this.current];
    this.goToPosition(googleMap, latlng.center, latlng.zoom);
  }

  public goToPrevPosition(googleMap: google.maps.Map): void {
    if (this.isFirst()) {
      return null;
    }

    const latlng = this.history[--this.current];
    this.goToPosition(googleMap, latlng.center, latlng.zoom);
  }

  public goToPosition(googleMap: google.maps.Map, center: google.maps.LatLng, zoom: number): void {
    googleMap.setCenter(center);
    googleMap.setZoom(zoom);
  }

  public isLast(): boolean {
    return this.current == this.history.length - 1;
  }

  public isFirst(): boolean {
    return this.current == 0;
  }

  private setIdle(entry: HistoryEntry): void {
    this.history = [...this.history.slice(0, ++this.current), entry];
  }
}

interface HistoryEntry {
  zoom: number;
  center: google.maps.LatLng;
}
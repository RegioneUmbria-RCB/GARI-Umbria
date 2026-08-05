import { Injectable } from '@angular/core';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp, Gis_Sat_Sentinel_Overlay, MascheraLayerRaster, UrlFirmato } from 'app/Service/api.service';
import { BehaviorSubject, filter, map, Observable, of, Subject, tap, withLatestFrom } from 'rxjs';

export const DEFAULT_SENSOR = 'NDVI';

export interface MappeSatellitariData {
  overlay: Gis_Sat_Sentinel_Overlay;
  sensor: string;
  urls: UrlFirmato[] | UrlFirmato;
  opacity: number;
  features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[];
  applyToPolygon: boolean;
}

export interface AnimationData {
  sensor: string;
  overlays: MappeSatellitariData[];
  status: AnimationStatus;
  step: number;
  opacity: number;
}

export enum AnimationStatus {
  ToBeLoaded,
  Loading,
  Started,
  Stopped
}

export enum SatelliteDataVisualizationModality {
  Daily,
  Animation
}

@Injectable()
export class GISAnalisiMappeSatellitariWindowService {

  private isAnimationActiveSubject = new BehaviorSubject<boolean>(false);
  private dateFromSubject = new BehaviorSubject<Date | null>(null);
  private dateToSubject = new BehaviorSubject<Date | null>(null);
  private currentDateSubject = new BehaviorSubject<Date | null>(null);
  private sensorSubject = new BehaviorSubject<string>(DEFAULT_SENSOR);
  private opacitySubject = new BehaviorSubject<number>(1);
  private mapInfoSubject = new BehaviorSubject<boolean>(null);
  private mapIdleSubject = new Subject<void>();
  private externalLoadOnPolygonsSubject = new Subject<boolean>();
  private mapClickEventSubject = new Subject<google.maps.Data.MouseEvent>();
  private animationStepSubject = new BehaviorSubject<number>(0);
  private animationStatusSubject = new BehaviorSubject<AnimationStatus>(AnimationStatus.ToBeLoaded);
  private satelliteDataSubject = new BehaviorSubject<Gis_Sat_Sentinel_Overlay[]>([]);
  private calendarSatelliteDataSubject = new BehaviorSubject<Gis_Sat_Sentinel_Overlay[]>([]);
  private overlaysSubject = new BehaviorSubject<MappeSatellitariData[]>([]);
  private masksSubject = new BehaviorSubject<MascheraLayerRaster[]>([]);
  private modalitySubject = new BehaviorSubject<SatelliteDataVisualizationModality>(SatelliteDataVisualizationModality.Daily);

  public get isAnimationActive$(): Observable<boolean> {
    return this.isAnimationActiveSubject.asObservable();
  }

  public get isAnimationActive(): boolean {
    return this.isAnimationActiveSubject.value;
  }

  public nextIsAnimationActive(value: boolean): void {
    this.isAnimationActiveSubject.next(value);
  }

  public get dateFrom$(): Observable<Date | null> {
    return this.dateFromSubject.asObservable();
  }

  public nextDateFrom(value: Date | null): void {
    this.dateFromSubject.next(value);
  }

  public get dateTo$(): Observable<Date | null> {
    return this.dateToSubject.asObservable();
  }

  public nextDateTo(value: Date | null): void {
    this.dateToSubject.next(value);
  }

  public get currentDate$(): Observable<Date | null> {
    return this.currentDateSubject.asObservable();
  }

  public nextCurrentDate(value: Date | null): void {
    this.currentDateSubject.next(value);
  }

  public get sensor$(): Observable<string> {
    return this.sensorSubject.asObservable();
  }

  public nextSensor(value: string): void {
    this.sensorSubject.next(value);
  }

  public get opacity$(): Observable<number> {
    return this.opacitySubject.asObservable();
  }

  public nextOpacity(value: number): void {
    this.opacitySubject.next(value);
  }

  public get mapInfo$(): Observable<boolean> {
    return this.mapInfoSubject.asObservable();
  }

  public nextMapInfo(value: boolean): void {
    this.mapInfoSubject.next(value);
  }

  public get isMapInfoActive(): boolean {
    return this.mapInfoSubject.value;
  }

  public get mapIdle$(): Observable<void> {
    return this.mapIdleSubject.asObservable();
  }

  public nextMapIdle(): void {
    this.mapIdleSubject.next();
  }

  public get externalLoadOnPolygons$(): Observable<boolean> {
    return this.externalLoadOnPolygonsSubject.asObservable();
  }

  public nextExternalLoadOnPolygons(value: boolean): void {
    this.externalLoadOnPolygonsSubject.next(value);
  }

  public get animationStep$(): Observable<number> {
    return this.animationStepSubject.asObservable();
  }

  public nextAnimationStep(value: number): void {
    this.animationStepSubject.next(Math.max(Math.min(value, this.overlaysSubject.value.length - 1), 0));
  }

  public goNextAnimationStep(previous: boolean = false): void {
    const newValue = this.animationStepSubject.value + (previous ? -1 : 1);
    this.nextAnimationStep(newValue);
  }

  public goLastAnimationStep(): void {
    this.nextAnimationStep(this.overlaysSubject.value.length - 1);
  }

  public get animationStatus$(): Observable<AnimationStatus> {
    return this.animationStatusSubject.asObservable();
  }

  public nextAnimationStatus(value: AnimationStatus): void {
    this.animationStatusSubject.next(value);
  }

  public get satelliteData$(): Observable<Gis_Sat_Sentinel_Overlay[]> {
    return this.satelliteDataSubject.asObservable();
  }

  public nextSatelliteData(value: Gis_Sat_Sentinel_Overlay[]): void {
    this.satelliteDataSubject.next(value);
  }

  public get calendarSatelliteData$(): Observable<Gis_Sat_Sentinel_Overlay[]> {
    return this.calendarSatelliteDataSubject.asObservable();
  }

  public nextCalendarSatelliteData(value: Gis_Sat_Sentinel_Overlay[]): void {
    this.calendarSatelliteDataSubject.next(value);
  }

  public get overlays$(): Observable<MappeSatellitariData[]> {
    return this.overlaysSubject.asObservable();
  }

  public nextOverlays(value: MappeSatellitariData[]): void {
    this.overlaysSubject.next(value);
  }

  public hasOverlays(): boolean {
    return this.overlaysSubject.value.length > 0;
  }

  public get masks$(): Observable<MascheraLayerRaster[]> {
    return this.masksSubject.asObservable();
  }

  public nextMasks(value: MascheraLayerRaster[]): void {
    this.masksSubject.next(value);
  }

  public get animationMapClickEvent$(): Observable<google.maps.Data.MouseEvent> {
    return this.mapClickEventSubject.asObservable().pipe(
      withLatestFrom(this.mapInfoSubject, this.modalitySubject),
      filter(([_, enabled, modality]) => enabled && modality === SatelliteDataVisualizationModality.Animation),
      map(([event, _, __]) => event)
    );
  }

  public get dailyMapClickEvent$(): Observable<google.maps.Data.MouseEvent> {
    return this.mapClickEventSubject.asObservable().pipe(
      withLatestFrom(this.mapInfoSubject, this.modalitySubject),
      filter(([_, enabled, modality]) => enabled && modality === SatelliteDataVisualizationModality.Daily),
      map(([event, _, __]) => event)
    );
  }

  public nextMapClick(clickEvent: google.maps.Data.MouseEvent): void {
    this.mapClickEventSubject.next(clickEvent);
  }

  public static dateToString(date: Date | null): string | null {
    if (date == null) {
      return null;
    }

    const yyyy = date.getFullYear();
    const mmInt = date.getMonth() + 1;
    const ddInt = date.getDate();

    const dd = ddInt < 10 ? `0${ddInt}` : `${ddInt}`;
    const mm = mmInt < 10 ? `0${mmInt}` : `${mmInt}`;
    const formattedToday = dd + '/' + mm + '/' + yyyy;

    return formattedToday;
  }

  public static stringToDate(date: string): Date {
    if (date.includes('/')) {
      // formato dd/MM/yyyy
      const split = date.split('/');
      return new Date(+split[2], +split[1] - 1, +split[0]);
    }

    if (date.includes('-')) {
      // formato yyyy-MM-dd
      const split = date.split('-');
      return new Date(+split[0], +split[1] - 1, +split[2]);
    }

    return new Date(date);
  }

  public static groupLayersByDate(layers: Gis_Sat_Sentinel_Overlay[]): Gis_Sat_Sentinel_Overlay[] {
    const result: Gis_Sat_Sentinel_Overlay[] = [];
    for (const layer of layers) {
      const current = result.find(x => x.DataRiferimento == layer.DataRiferimento);
      if (current == null) {
        result.push(layer);
      } else {
        current.Passaggi = current.Passaggi.concat(layer.Passaggi);
      }
    }

    return result;
  }

  public get modality$(): Observable<SatelliteDataVisualizationModality> {
    return this.modalitySubject.asObservable();
  }

  public nextModality(value: SatelliteDataVisualizationModality): void {
    this.modalitySubject.next(value);
  }
}

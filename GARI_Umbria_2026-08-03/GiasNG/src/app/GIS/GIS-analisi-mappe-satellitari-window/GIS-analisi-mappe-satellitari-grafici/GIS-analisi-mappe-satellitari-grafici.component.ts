import { Component, ViewChild } from '@angular/core';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { GisClient, LetturaDatiElaboratiSuSensoreListaValori_In } from 'app/Service/api.service';
import { catchError, combineLatest, debounceTime, filter, merge, Observable, of, Subject, tap, withLatestFrom } from 'rxjs';
import { map, switchMap } from 'rxjs';
import { GISAnalisiMappeSatellitariWindowService, MappeSatellitariData } from '../GIS-analisi-mappe-satellitari-window.service';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { SelectionRange } from '@progress/kendo-angular-dateinputs';
import { GISAnalisiMappeSatellitariCalendarioComponent } from '../GIS-analisi-mappe-satellitari-calendario/GIS-analisi-mappe-satellitari-calendario.component';

export const DEFAULT_CHART_WIDTH = 550;

@Component({
  standalone: false,
  selector: 'gis-analisi-mappe-satellitari-grafici',
  templateUrl: './GIS-analisi-mappe-satellitari-grafici.component.html',
  styleUrls: ['./GIS-analisi-mappe-satellitari-grafici.component.css']
})
export class GISAnalisiMappeSatellitariGraficiComponent {
  @ViewChild('calendar') gisAnalisiMappeSatellitariCalendarioComponent: GISAnalisiMappeSatellitariCalendarioComponent;

  private latestLatLng: google.maps.LatLng | null = null;
  private localDateToSubject = new Subject<Date>();
  private localDateFromSubject = new Subject<Date>();
  private reloadDataSubject = new Subject<google.maps.Data.MouseEvent>();

  dateTo$ = merge(
    this.gisAnalisiMappeSatellitariWindowService.dateTo$,
    this.localDateToSubject.asObservable()
  );
  dateFrom$ = merge(
    this.gisAnalisiMappeSatellitariWindowService.dateFrom$,
    this.localDateFromSubject.asObservable()
  );

  reloader$ = this.gisAnalisiMappeSatellitariWindowService.overlays$
    .pipe(
      debounceTime(100),
      map(() => this.latestLatLng),
      filter(value => value != null),
      tap(value => this.reloadDataSubject.next({ latLng: value } as google.maps.Data.MouseEvent))
    );

  clickEvent$ = merge(
    this.gisAnalisiMappeSatellitariWindowService.dailyMapClickEvent$,
    this.gisAnalisiMappeSatellitariWindowService.animationMapClickEvent$
  ).pipe(tap(click => this.latestLatLng = click.latLng));

  result$: Observable<{ title: string, dates: string[], values: number[] }> = merge(
    this.clickEvent$,
    this.reloadDataSubject.asObservable()
  )
    .pipe(
      debounceTime(100),
      withLatestFrom(
        this.gisAnalisiMappeSatellitariWindowService.overlays$,
        this.gisAnalisiMappeSatellitariWindowService.currentDate$,
        this.dateFrom$,
        this.dateTo$
      ),
      map(([click, overlays, currentDate, dateFrom, dateTo]) => this.getPayload(
        click,
        overlays.find(o => GISAnalisiMappeSatellitariWindowService.stringToDate(o.overlay.DataRiferimento).toDateString() == currentDate.toDateString()),
        dateFrom,
        dateTo)
      ),
      filter(payload => payload != null),
      switchMap(payload => this.getResult(payload))
    );

  dates$ = combineLatest([this.dateFrom$, this.dateTo$])
    .pipe(map(([from, to]) => ({ start: from, end: to } as SelectionRange)));

  loading = false;

  constructor(
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    private googleMapService: GoogleMapService,
    private gisClient: GisClient,
    private gisMessageService: GiasMessageService,
    private featureInformationService: FeatureInformationService
  ) {
  }

  openCalendar(): void {
    this.gisAnalisiMappeSatellitariCalendarioComponent?.openDialog();
  }

  changeDates(dates: SelectionRange): void {
    this.localDateFromSubject.next(dates.start);
    this.localDateToSubject.next(dates.end);

    if (this.latestLatLng != null) {
      this.reloadDataSubject.next({ latLng: this.latestLatLng } as google.maps.Data.MouseEvent);
    }
  }

  private getPayload(ev: google.maps.Data.MouseEvent, data: MappeSatellitariData | null, dateFrom: Date | null, dateTo: Date | null): LetturaDatiElaboratiSuSensoreListaValori_In | null {
    if (data?.overlay == null || dateFrom == null || dateTo == null) {
      return null;
    }

    const zoom = this.googleMapService.googleMapWrapper.data.getMap().getZoom();
    const sensor = data.sensor;
    const position = ev.latLng;

    if (data.applyToPolygon) {
      let inSelectedPolygon = false;

      for (const feature of data.features) {
        const path = this.featureInformationService.getPath(feature.properties.id);
        const polygon = new google.maps.Polygon();
        polygon.setPath(path);
        if (google.maps.geometry.poly.containsLocation(position, polygon)) {
          inSelectedPolygon = true;
          break;
        }
      }

      if (!inSelectedPolygon) {
        if (ev.feature == null) {
          // clicked on the map, not on the feature
          this.gisMessageService.errorMessage("gis.DatiSatellitariDisponibiliSoloNeiPoligonoDoveLaMappaEStataCaricata", false, true);
        }

        return null;
      }
    }

    return {
      DataFine: `${GISAnalisiMappeSatellitariGraficiComponent.getFormattedValue(dateTo.getDate() + 1)}/${GISAnalisiMappeSatellitariGraficiComponent.getFormattedValue(dateTo.getMonth() + 1)}/${dateTo.getFullYear()}`,
      DataInizio: `${GISAnalisiMappeSatellitariGraficiComponent.getFormattedValue(dateFrom.getDate())}/${GISAnalisiMappeSatellitariGraficiComponent.getFormattedValue(dateFrom.getMonth() + 1)}/${dateFrom.getFullYear()}`,
      PoligonoWKT: `POINT(${position.lng()} ${position.lat()})`,
      Sensore: sensor,
      Zoom: zoom
    } as LetturaDatiElaboratiSuSensoreListaValori_In;
  }

  private getResult(payload: LetturaDatiElaboratiSuSensoreListaValori_In | null): Observable<{ title: string, dates: string[], values: number[] }> {
    if (payload == null) {
      return of(null);
    }

    this.loading = true;
    return this.gisClient
      .gisLetturaDatiElaboratiSuSensoreListaValori(payload)
      .pipe(
        map(result => JSON.parse(result.RispostaStringa)),
        map((result: Result[]) => result.sort((a, b) => new Date(a.DataRiferimento).getTime() - new Date(b.DataRiferimento).getTime())),
        map((result: Result[]) => {
          this.loading = false;
          return ({
            title: `${payload.DataInizio} - ${payload.Sensore}`,
            dates: result.map(x => GISAnalisiMappeSatellitariWindowService.dateToString(new Date(x.DataRiferimento))),
            values: result.map(x => x.Valore)
          });
        }),
        catchError(() => {
          this.loading = false;
          // this.gisMessageService.baseError("Errore", "gis.DatiSatellitariNonDisponibili");
          return of(null);
        }),
      );
  }

  private static getFormattedValue(value: number): string {
    return `${value}`.padStart(2, '0');
  }
}

interface Result {
  DataRiferimento: string
  Valore: number;
}

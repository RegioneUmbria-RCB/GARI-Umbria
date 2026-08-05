import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { BehaviorSubject, combineLatest, map } from 'rxjs';
import { GISAnalisiMappeSatellitariWindowService } from '../GIS-analisi-mappe-satellitari-window.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { Gis_Sat_Sentinel_Overlay } from 'app/Service/api.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { CalendarSelection } from '@progress/kendo-angular-dateinputs/calendar/models/selection';
import { SelectionRange } from '@progress/kendo-angular-dateinputs';

@Component({
  standalone: false,
  selector: 'gis-analisi-mappe-satellitari-calendario',
  templateUrl: './GIS-analisi-mappe-satellitari-calendario.component.html',
  styleUrls: ['./GIS-analisi-mappe-satellitari-calendario.component.css']
})
export class GISAnalisiMappeSatellitariCalendarioComponent implements OnChanges {

  @Input() selectedDate: Date | SelectionRange | null = null;
  @Input() disabled: boolean = false;
  @Input() featureCode: string | null = null;
  @Input() focusedDate: Date | null = null;
  @Input() selection: CalendarSelection = 'single';
  @Input() onlyDialog: boolean = false;
  @Output() selectedDateChange = new EventEmitter<Date | SelectionRange>();

  private featureCodeSubject = new BehaviorSubject<string | null>(null);

  calendarOpen = false;
  dates$ = combineLatest([
    this.gisAnalisiMappeSatellitariWindowService.calendarSatelliteData$,
    this.featureCodeSubject.asObservable()
  ])
    .pipe(
      map(([data, code]) => code == null || this.isLegacy ? data : this.filter(data, code)),
      map(data => data.map(y => [GISAnalisiMappeSatellitariWindowService.stringToDate(y.DataRiferimento), y.Passaggi.every(x => x.url == '')]))
    );

  constructor(
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    private sharedDataService: SharedDataService
  ) { }

  private get isLegacy(): boolean {
    return this.sharedDataService.getCfgAlberoGisUtente()[0].CfgGisUtente.ckGestioneAnalisiMappeLegacy;
  }

  ngOnChanges(): void {
    this.featureCodeSubject.next(this.featureCode);
  }

  loadData(): void {
    this.gisAnalisiMappeSatellitariWindowService.nextExternalLoadOnPolygons(true);
  }

  changeDate(date: Date | SelectionRange): void {
    this.selectedDate = date;
    this.selectedDateChange.emit(date);
  }

  getDayEvent(values: [Date, boolean][], date: Date): string {
    const value = values.find(d => FunzioniComuniService.areDatesEqual(d[0], date));
    return value == null ? '' : value[1] ? 'event-no-data' : 'event-data'; // value[1] => is cloudy
  }

  getMonthEvent(values: [Date, boolean][], date: Date): string {
    const value = values.find(d => `${d[0].getMonth()}/${d[0].getFullYear()}` == `${date.getMonth()}/${date.getFullYear()}`);
    return value == null ? '' : value[1] ? 'event-no-data' : 'event-data'; // value[1] => is cloudy
  }

  getYearEvent(values: [Date, boolean][], date: Date, difference: number): string {
    const value = values.find(d => Math.floor(d[0].getFullYear() / difference) * difference == Math.floor(date.getFullYear() / difference) * difference);
    return value == null ? '' : value[1] ? 'event-no-data' : 'event-data'; // value[1] => is cloudy
  }

  openDialog(): void {
    this.calendarOpen = true;
  }

  private filter(data: Gis_Sat_Sentinel_Overlay[], code: string): Gis_Sat_Sentinel_Overlay[] {
    const result: Gis_Sat_Sentinel_Overlay[] = [];
    for (const element of data) {
      if (element.Passaggi.some(p => p.url.includes(code) || p.url == '')) {
        result.push(element);
      }
    }
    return result;
  }
}

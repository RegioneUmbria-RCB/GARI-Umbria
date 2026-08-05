import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { WidgetsClient,Widget_MeteoImpresaLatLng } from 'app/Service/api.service';
import { finalize, Observable, map, switchMap, tap, take, of } from 'rxjs';
import { HubMeteoData, HubMeteoObservation, MeteoData } from './meteo-widget.model';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ConfigurazioneSitiService, Configurazione_Siti } from "../../../Service/configurazione-siti.service";
import { filter } from "rxjs/operators";


export const BASE_METEO_IMG_URL = 'agronica/Scripts/AgronicaControlli_2010/AgroMeteo/AgroMeteoIcons'; // this should be a backend info
export const HOURS_24 = 86400000;
export const HOURS_48 = 172800000;
export const HOURS_72 = 259200000;
export const HOURS_96 = 345600000;

const chiaveConfigurazone = 'servizioAutorizzazioneHubMeteo';

interface HubMeteoParsedData {
  date: string;
  weekDay: string;
  average: HubMeteoObservation;
  nextPrevisions: HubMeteoObservation[];
}

interface AgroMeteoConfig {
  baseUrl: string;
  user: string;
  password: string;
}

@Component({
  standalone: false,
  selector: 'app-meteo-widget',
  templateUrl: './meteo-widget.component.html',
  styleUrls: ['./meteo-widget.component.css']
})
export class MeteoWidgetComponent implements OnInit {
  @Input() params: string | null = null;

  loading: boolean = true;
  data: HubMeteoParsedData | null = null;

  protected weekMeteo: HubMeteoParsedData[] = [];
  protected city: string;
  protected todayIndex: number;
  protected displayIndex: number;
  protected noLatLng: boolean = false;

  constructor(
    private widgetsClient: WidgetsClient,
    private translocoService: TranslocoService,
    private httpClient: HttpClient,
    private objParametriAgendaService: ObjParametriAgendaService,
    private config: ConfigurazioneSitiService,
  ) {  }

  get currHourPrevision(): HubMeteoObservation {
    const today = this.todaysPrevisions;
    const now: number = new Date().getHours();
    return today.nextPrevisions.find((x) => +x.dataOra.split(' ')[1].split(':')[0] == now)
      ?? today.nextPrevisions[0];
  }

  get todaysPrevisions(): HubMeteoParsedData {
    return this.weekMeteo.at(this.todayIndex);
  }

  get displayedPrevision(): HubMeteoParsedData {
    return this.weekMeteo.at(this.displayIndex);
  }

  get nextPrevisions(): HubMeteoParsedData[] {
    return this.weekMeteo.filter((x, i) => i >= this.todayIndex);
  }

  ngOnInit(): void {
    this.loading = true;

    this.widgetsClient.widgetsWeatherLatLng(this.objParametriAgendaService?.getObjParamValue()?.Piva)
      .pipe(
        switchMap(config => this.getWeatherDataHubMeteo(config.RispostaStringa)),
        filter(val => val != undefined),
        tap(data => this.parseHubData(data)),
        finalize(() => this.loading = false)
      )
      .subscribe();
  }

  private getWeatherDataHubMeteo(config: Widget_MeteoImpresaLatLng): Observable<HubMeteoData> {
    let forecastUrl: string = '';
    return this.config.leggiChiave(chiaveConfigurazone).pipe(
      switchMap((R: Configurazione_Siti) => {
        if (!!R && !!config && config.Country.toUpperCase().trim() === 'IT') {
          const params: AgroMeteoConfig = JSON.parse(R.Valore);
          this.city = config.Location?.replace(",", ", ");
          forecastUrl = `${params.baseUrl}/Measure/ForecastIcons?Lat=${config.Lat}&Lng=${config.Lng}`;
          return this.httpClient.post<any>(params.baseUrl + '/Users/Authenticate', {
            username: params.user,
            password: params.password
          }, { headers: { 'skipWithCredentials': 'true' } });
        } else {
          this.noLatLng = true;
          return of(undefined);
        }
      }),
      filter((val) => {
        return val != undefined;
      }),
      take(1),
      map(R => R['accessToken']),
      switchMap(token => this.httpClient.get<HubMeteoData>(forecastUrl, {
        headers: { Authorization: 'Bearer ' + token, 'skipWithCredentials': 'true' }
      }))
    );
  }

  private parseHubData(rawData: HubMeteoData): void {
    this.tryParse(rawData);
    if (this.weekMeteo.length > 0) {
      this.setDayAverage();
      this.setDateOffset();
      this.data = this.weekMeteo[0];
    }
  }

  private tryParse(rawData: HubMeteoData): void {
    if (!rawData || !rawData.icons) {
      return;
    }

    for (let data of rawData?.icons) {
      const date: string = data.dataOra.split(" ")[0];
      const time: string = data.dataOra.split(" ")[1]
        .split(":")
        .filter((x, i) => i < 2) // Rimuovo indicazione secondi
        .reduce((a, b) => a + ":" + b);
      data.dataOra = date + " " + time;

      const dateObj = new Date(data.dataOra);
      data.dataOraLocale = this.getDateTimeStr(dateObj);

      let dayPrevision = this.weekMeteo.find(day => day.date === date);
      if (dayPrevision) {
        dayPrevision.nextPrevisions.push(data);
      } else {
        let dateStr = this.translocoService.translate("ShortWeekDay" + dateObj.toDateString().split(" ")[0]);
        dayPrevision = {
          date: date,
          weekDay: dateStr,
          average: undefined,
          nextPrevisions: [data]
        };
        this.weekMeteo.push(dayPrevision);
      }
    }
  }

  private groupBy(xs: any[], key: string) {
    return xs.reduce(function (rv, x) {
      (rv[x[key]] = rv[x[key]] || []).push(x);
      return rv;
    }, {});
  }

  /**
   * Set the day average based on the weekMeteo data.
   * @private
   * To find each day's average weather, it first filters the weather for the hours between 6am and 7pm (daytime previsions).
   * If no daytime previsions are found, it uses all the available previsions for the day. It groups the previsions by
   * weather value and finds the one with most occurrences. It then sets the average weather for the day.
   * If also the grouping has failed, it sets the first prevision found as average for the day.
   */
  private setDayAverage(): void {
    for (let value of this.weekMeteo) {
      let dayPrevisions = value.nextPrevisions.filter(x => {
        let h = +x.dataOra.split(" ")[1].split(':')[0];
        return h > 6 && h < 19;
      });

      if (dayPrevisions.length === 0)
        dayPrevisions = value.nextPrevisions;

      const groups = this.groupBy(dayPrevisions, 'valore');
      let maxLength = { key: '-1', length: 0 };
      for (let v in groups) {
        if (groups[v].length > maxLength.length) {
          maxLength.length = groups[v].length;
          maxLength.key = v;
        }
      }

      value.average = (+maxLength.key >= 0) ? groups[+maxLength.key][0] : value.nextPrevisions[0];
    }
  }

  private setDateOffset(): void {
    const today = new Date();
    const mm = today.getMonth() + 1 < 10 ? '0' + (today.getMonth() + 1) : (today.getMonth() + 1);
    const dd = today.getDate() < 10 ? '0' + today.getDate() : today.getDate();
    const todayStr = today.getFullYear() + '-' + mm + '-' + dd;
    this.todayIndex = this.weekMeteo.findIndex(d => d.date === todayStr);
    this.displayIndex = this.todayIndex;
  }

  private getDateTimeStr(date: Date): string {
    let dStr = date.toDateString().split(" ")[0];
    let mStr = date.toDateString().split(" ")[1];

    return this.translocoService.translate("ShortWeekDay" + dStr)
      + " " + date.getDate() + " "
      + this.translocoService.translate("ShortMonth" + mStr)
      + ", " + date.toTimeString().substring(0, 5);
  }
}

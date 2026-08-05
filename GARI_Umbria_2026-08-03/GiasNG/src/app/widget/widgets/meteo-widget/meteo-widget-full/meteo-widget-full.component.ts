import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { MasterService } from 'app/Service/master.service';
import { BASE_METEO_IMG_URL, HOURS_24, HOURS_48, HOURS_72, HOURS_96 } from '../meteo-widget.component';
import { MeteoData } from '../meteo-widget.model';

@Component({
  standalone: false,
  selector: 'app-meteo-widget-full',
  templateUrl: './meteo-widget-full.component.html',
  styleUrls: ['./meteo-widget-full.component.css'],
})
export class MeteoWidgetFullComponent implements OnInit {
  @Input() meteoData: MeteoData | null = null;
  @Output() closeEvent = new EventEmitter<void>();

  // Kendo dialog sizes
  height = window.innerHeight;
  width = window.innerWidth;

  // Data
  today: MeteoWidgetFullData | null = null;
  forecasts: MeteoWidgetFullForecast[] = [];

  // Charts variables
  mmRainCorrentTime: number[] = [];
  pressureCurrentTime: number[] = [];
  groundPressureCurrentTime: number[] = [];
  weekDate: string[] = [];
  mmRainWeek: number[] = [];
  pressureWeek: number[] = [];
  groundPressureWeek: number[] = [];
  weekDay: string[] = [];
  temperatureWeek: number[] = [];
  maxTemperatureWeek: number[] = [];
  minTemperatureWeek: number[] = [];
  currentChartTime: number | null = null;

  constructor(
    private translocoService: TranslocoService,
    private masterService: MasterService
  ) { }

  ngOnInit(): void {
    if (this.meteoData == null) {
      throw new Error("Error in MeteoWidgetFullComponent. Parameter meteoData cannot be null");
    }

    this.today = this.getTodayData();
    this.computeForecast();
  }

  onClose(): void {
    this.closeEvent.emit();
  }

  private computeForecast(): void {
    const split_current_date = this.meteoData.list[0].dt_txt.split(" ");
    const split_current = split_current_date[0].split("-");
    const utc = Date.UTC(+split_current[0], +split_current[1] - 1, +split_current[2]);

    // salto il primo rilevamento perchè è quello già mostrato e vado a dividere gli altri a seconda della data
    for (let i = 1; i < this.meteoData.list.length; i++) {
      const tempo_temp = this.meteoData.list[i].dt_txt.split(" ");
      const data_temp = tempo_temp[0];
      const ora_temp = tempo_temp[1];

      const split_ril = data_temp.split("-")
      const utc_conf = Date.UTC(+split_ril[0], +split_ril[1] - 1, +split_ril[2]);
      const diff = utc_conf - utc;

      const ora = ora_temp.split(":");
      const data_conforme = parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + "/" + split_ril[0];

      const dayIndex = this.getDayIndex(diff);
      if (dayIndex == -1) {
        continue;
      }

      if (dayIndex == 0) {
        this.currentChartTime = new Date().getHours();
      }

      this.forecasts[dayIndex] ??= { date: data_conforme, forecasts: [] };
      this.forecasts[dayIndex].forecasts.push({
        hour: ora[0],
        imageSrc: `${this.masterService.link_GiasBase}/${BASE_METEO_IMG_URL}/${this.meteoData.list[i].weather[0].icon}.png`,
        temperature: Math.round(this.meteoData.list[i].main.temp),
        description: this.meteoData.list[i].weather[0].description,
        windSpeed: Math.round(this.meteoData.list[i].wind.speed),
        windDirection: this.getWindDirection(this.meteoData.list[i].wind.deg)
      });

      if (this.meteoData.list[i].rain == undefined || this.meteoData.list[i].rain["3h"] == undefined) {
        this.mmRainCorrentTime[dayIndex] = parseFloat("0");
      } else {
        const mmPioggia = parseFloat(this.meteoData.list[i].rain["3h"]).toFixed(2);
        this.mmRainCorrentTime[dayIndex] = mmPioggia == "NaN" ? parseFloat("0") : +mmPioggia;
      }

      // inserisco la pressione per l'orario dell'array
      this.pressureCurrentTime[dayIndex] = this.meteoData.list[i].main.pressure;
      this.groundPressureCurrentTime[dayIndex] = this.meteoData.list[i].main.grnd_level;
      this.weekDate[dayIndex] = data_conforme;

      // per i grafici orari della settimana
      if (this.meteoData.list[i].rain == undefined || this.meteoData.list[i].rain["3h"] == undefined) {
        this.mmRainWeek.push(parseFloat("0"));
      } else {
        const mmPioggia = parseFloat(this.meteoData.list[i].rain["3h"]).toFixed(2);
        this.mmRainWeek.push(mmPioggia == "NaN" ? parseFloat("0") : +mmPioggia);
      }

      this.pressureWeek.push(this.meteoData.list[i].main.pressure);
      this.groundPressureWeek.push(this.meteoData.list[i].main.grnd_level);
      this.weekDay.push(parseInt(split_ril[2]) + "/" + parseInt(split_ril[1]) + " \n " + ora[0]);
      this.temperatureWeek.push(this.meteoData.list[i].main.temp);
      this.maxTemperatureWeek.push(this.meteoData.list[i].main.temp_max);
      this.minTemperatureWeek.push(this.meteoData.list[i].main.temp_min);
    }
  }

  private getDayIndex(diff: number) {
    let dayIndex = -1;
    if (diff >= 0 && diff < HOURS_24) {
      dayIndex = 0;
    }
    else if (diff == HOURS_24) {
      dayIndex = 1;
    }
    else if (diff == HOURS_48) {
      dayIndex = 2;
    }
    else if (diff == HOURS_72) {
      dayIndex = 3;
    }
    else if (diff == HOURS_96) {
      dayIndex = 4;
    }
    return dayIndex;
  }

  private getTodayData(): MeteoWidgetFullData {
    const today = {} as MeteoWidgetFullData;
    today.city = this.meteoData.city.name;
    today.imageSrc = `${this.masterService.link_GiasBase}/${BASE_METEO_IMG_URL}/${this.meteoData.list[0].weather[0].icon}.png`;
    today.windSpeed = Math.round(this.meteoData.list[0].wind.speed);
    today.windDirection = this.getWindDirection(this.meteoData.list[0].wind.deg);
    today.temperature = Math.round(this.meteoData.list[0].main.temp);
    today.description = this.meteoData.list[0].weather[0].description;
    today.humidity = this.meteoData.list[0].main.humidity;

    today.rain = parseFloat("0");
    if (this.meteoData.list[0].rain == undefined || this.meteoData.list[0].rain["3h"] == undefined) {
      today.rain = parseFloat("0");
    } else {
      today.rain = +parseFloat(this.meteoData.list[0].rain["3h"]).toFixed(2);
      if (isNaN(today.rain)) {
        today.rain = parseFloat("0");
      }
    }

    return today;
  }

  private getWindDirection(direction: number): string {
    if (direction >= 0 && direction <= 22.5) {
      return this.translocoService.translate('VentoNord');
    }

    if (direction <= 67.5 && direction > 22.5) {
      return this.translocoService.translate('VentoNordEst');
    }

    if (direction <= 112.5 && direction > 67.5) {
      return this.translocoService.translate('VentoEst');
    }

    if (direction <= 157.5 && direction > 112.5) {
      return this.translocoService.translate('VentoSudEst');
    }

    if (direction <= 202.5 && direction > 157.5) {
      return this.translocoService.translate('VentoSud');
    }

    if (direction <= 247.5 && direction > 202.5) {
      return this.translocoService.translate('VentoSudOvest');
    }

    if (direction <= 292.5 && direction > 247.5) {
      return this.translocoService.translate('VentoOvest');
    }

    if (direction <= 337.5 && direction > 292.5) {
      return this.translocoService.translate('VentoNordOvest');
    }

    if (direction <= 360 && direction > 337.5) {
      return this.translocoService.translate('VentoNordOvest');
    }

    return this.translocoService.translate('VentoValoreErrato');
  }
}


interface MeteoWidgetFullForecast {
  date: string;
  forecasts: MeteoWidgetFullData[];
}

interface MeteoWidgetFullData {
  city?: string;
  hour?: string;
  imageSrc: string;
  temperature: number;
  description: string;
  windSpeed: number;
  windDirection: string;
  humidity?: number;
  rain?: number;
}

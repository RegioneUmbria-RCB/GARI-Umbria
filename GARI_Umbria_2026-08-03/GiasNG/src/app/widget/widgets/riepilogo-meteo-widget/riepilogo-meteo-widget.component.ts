import { DatePipe } from '@angular/common';
import { AfterViewChecked, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { WidgetsClient } from 'app/Service/api.service';
import { finalize } from 'rxjs';
import { Axis, MeteoChart, Series } from './riepilogo-meteo-widget.models';
import { RiepilogoMeteoWidgetService } from './riepilogo-meteo-widget.service';

@Component({
  standalone: false,
  selector: 'app-riepilogo-meteo-widget',
  templateUrl: './riepilogo-meteo-widget.component.html',
  styleUrls: ['./riepilogo-meteo-widget.component.css']
})
export class RiepilogoMeteoWidgetComponent implements OnInit, AfterViewChecked {
  @ViewChild('container') container: ElementRef;

  @Input() params: string | null = null;
  @Input() piva: string | null = null;

  loading = false;
  widgetData: WidgetData[] = [];
  page = 0;
  height: number | null = null;

  arrayStazioni = [];

  private language: string;

  constructor(
    private widgetsClient: WidgetsClient,
    private translocoService: TranslocoService,
    private riepilogoMeteoWidgetService: RiepilogoMeteoWidgetService
  ) {
    this.language = this.translocoService.getActiveLang();
  }

  ngOnInit(): void {
    if (this.piva == null) {
      this.loading = false;
      return;
    }

    this.loading = true;
    this.widgetsClient
      .widgetsDatiMeteoElaboraRiepilogo(this.piva)
      .pipe(finalize(() => this.loading = false))
      .subscribe(response => {
        this.arrayStazioni = response.RispostaStringa.Stazioni;
        this.widgetData = this.processResponse(response.RispostaStringa);
        this.ngAfterViewChecked();
      });
  }


  ngAfterViewChecked(): void {
    this.height = this.container?.nativeElement?.clientHeight / 2;
  }

  navigate(delta: number): void {
    const length = this.widgetData.length ?? 0;
    let page = this.page + delta;

    if (page >= length) {
      page -= length;
    }

    if (page < 0) {
      page += length;
    }

    this.page = page;
  }

  getCategoryAsDate(input: string): string {
    const pipe = new DatePipe(this.language);
    if (!isNaN(Date.parse(input))) {
      return pipe.transform(new Date(input), 'dd MMMM - HH');
    }

    const datetime = input.split(' ');
    const date = datetime[0].split(/\/|-/);
    const time = datetime[1].split(':');
    const result = new Date(+date[2], +date[1] - 1, +date[0], +time[0], +time[1], +time[2]);
    return pipe.transform(result, 'dd MMMM - HH');
  }

  onChartClick(event: any, stazione: any): void {
    this.riepilogoMeteoWidgetService.ApriPlugInMeteo(this.arrayStazioni, stazione);
  }

  private isSameYear(date: Date): boolean {
    return date.getFullYear() == new Date().getFullYear();
  }

  private processResponse(response: any): WidgetData[] {
    const pipe = new DatePipe(this.language);
    const result: WidgetData[] = [];

    for (const station of response.Stazioni) {
      const charts = JSON.parse(station.Meteo.Meteo_Charts);
      const table = JSON.parse(station.Meteo.Meteo_Table);
      const ultimoAggiornamento = new Date(station.UltimoAggiornamento);

      result.push({
        Descrizione: station.Descrizione,
        UltimoAggiornamento: this.isSameYear(ultimoAggiornamento) ? pipe.transform(ultimoAggiornamento, 'dd MMM - HH:mm') : pipe.transform(ultimoAggiornamento, 'dd MMM yyyy'),
        Charts: this.parseChartsFromResponse(charts, table, 2),
        Meteo_Table: table
      });
    }

    return result;
  }

  private parseChartsFromResponse(charts: MeteoChart[], data: any, maxNumber: number): Chart[] {
    const result: Chart[] = [];
    for (let i = 0; i < maxNumber && i < charts.length; i++) {
      const series = this.parseSeriesFromChart(charts[i], data);
      const axis = this.parseAxisFromChart(charts[i]);
      const categories = this.parseCategoriesFromChart(charts[i], data);

      result.push({ Series: series, Axis: axis, Categories: categories } as Chart);
    }

    return result;
  }

  private parseSeriesFromChart(chart: MeteoChart, data: any): Series[] {
    const series = [];
    for (const serie of chart.series) {
      const patchSerie = Object.assign({}, { missingValues: "interpolate", gap: 0.25, aggregate: "", data: [] }, serie);
      if (serie.FunAggreg === "avg" || serie.FunAggreg === "sum" || serie.FunAggreg === "max" || serie.FunAggreg === "min") {
        patchSerie.aggregate = serie.FunAggreg;
      } else if (serie.FunAggreg !== "") {
        patchSerie.aggregate = "avg";
      }
      patchSerie.data = data.kendo_rows;

      for (const row of patchSerie.data) {
        for (const key in row) {
          if (key.includes("DataOra")) {
            row[key] = this.parseDate(row[key]);
          }
        }
      }

      series.push(patchSerie);
    }

    return series;
  }

  private parseAxisFromChart(chart: MeteoChart): Axis[] {
    const axis = [];

    for (const ax of chart.axis) {
      axis.push({ name: ax.name });
    }

    return axis;
  }

  private parseCategoriesFromChart(chart: MeteoChart, data: any): string[] {
    const field = typeof chart.horizAxis === "string" ? chart.horizAxis : chart.horizAxis.field
    return data.kendo_rows.map(x => x[field]);
  }

  private parseDate(input: string): string {
    // test for 2025-01-01T01:00:00.000Z
    const isoRegex = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}Z$/;
    if (isoRegex.test(input)) {
      return input;
    }

    // test for 2025-01-01T01:00:00
    const dateRegex = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$/;
    if (dateRegex.test(input)) {
      const localDate = new Date(input);
      const utcDate = new Date(Date.UTC(
        localDate.getFullYear(),
        localDate.getMonth(),
        localDate.getDate(),
        localDate.getHours(),
        localDate.getMinutes(),
        localDate.getSeconds()
      ));
      return utcDate.toISOString();
    }

    // input is '06/05/2025 10:00:00', meaning that it is in the format 'dd/MM/yyyy HH:mm:ss'
    // we need to turn it into iso string
    const [datePart, timePart] = input.split(' ');
    const [day, month, year] = datePart.split('/').map(Number);
    const [hours, minutes, seconds] = timePart.split(':').map(Number);
    return new Date(Date.UTC(year, month - 1, day, hours, minutes, seconds)).toISOString();
  }
}

interface Chart {
  Series: Series[];
  Axis: Axis[];
  Categories: string[];
}

interface WidgetData {
  Descrizione: string;
  UltimoAggiornamento: string;
  Charts: Chart[];
  Meteo_Table: any;
}

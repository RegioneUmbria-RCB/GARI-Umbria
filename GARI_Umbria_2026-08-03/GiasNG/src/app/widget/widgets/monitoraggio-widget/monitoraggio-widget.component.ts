import { Component, Input, OnInit } from '@angular/core';
import { Series } from '@progress/kendo-angular-charts';
import { WidgetsClient } from 'app/Service/api.service';
import { finalize } from 'rxjs';
import { AlertSerie, MonitoraggioStation } from './monitoraggio-widget.models';
import { MonitoraggioWidgetService } from './monitoraggio-widget.service';

@Component({
  standalone: false,
  selector: 'app-monitoraggio-widget',
  templateUrl: './monitoraggio-widget.component.html',
  styleUrls: ['./monitoraggio-widget.component.css']
})
export class MonitoraggioWidgetComponent implements OnInit {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;

  rawData: MonitoraggioStation[] = [];
  widgetData: WidgetData[] = []
  loading = false;
  page = 0;

  arrayStazioni = [];

  constructor(private widgetsClient: WidgetsClient,
              private monitoraggioService: MonitoraggioWidgetService) {}

  ngOnInit(): void {
    if (this.piva == null) {
      this.loading = false;
      return;
    }

    this.loading = true;

    this.widgetsClient
      .widgetsElaboraMonitoraggioSuolo(this.piva)
      .pipe(finalize(() => this.loading = false))
      .subscribe(result => {
        this.arrayStazioni = result.RispostaStringa.Stazioni;
        this.parseResponse(result.RispostaStringa);
        this.computeWidgetData(this.rawData);
      });
  }

  navigate(delta: number): void {
    const length = this.widgetData.length
    let page = this.page + delta;

    if (page >= length) {
      page -= length;
    }

    if (page < 0) {
      page += length;
    }

    this.page = page;
  }

  onClickWidget(event: any, station: any) {
    this.monitoraggioService.ApriPlugInMonitoraggio(this.arrayStazioni, station);
  }

  private parseResponse(response: any): void {
    const result: MonitoraggioStation[] = [];

    for(const station of response.Stazioni) {
      result.push( {
        AlertSerie: JSON.parse(station.AlertSerie),
        Descrizione: station.Descrizione,
        SogliaInf: station.SogliaInf,
        SogliaSup: station.SogliaSup,
        Meteo: {} // This will be used for inner charts
      } as MonitoraggioStation);
    }

    this.rawData = result;
  }

  private computeWidgetData(rawData: MonitoraggioStation[]): void {
    const widgetData: WidgetData[] = [];

    for (const station of rawData) {
      const series: Series[] = [];
      const data: number[] = [];

      for (const e of station.AlertSerie) {
        let idx = e.value + 1; // -1 se non ho valori per la data...
        while (idx >= series.length) {
          series.push({
            line: { style: "step" },
            data: data.slice(),
            color: ""
          });
        }

        series[idx].color = e.color;

        data.push(0);

        for (let j = 0; j < series.length; j++) {
          const s = series[j];
          if (j === idx) {
            s.data.push((e.value === 0 ? 1 : (e.value === 1 ? 1.2 : (e.value === 2 ? 0.8 : 0))));
          } else {
            s.data.push(0);
          }
        }
      }

      let s = 1
      while (s < series.length) {
        if (series[s].color === "") {
          series.splice(s, 1);
        } else {
          s++;
        }
      }

      if (series.length > 1 && series[0].color === "") {
        series.splice(0, 1);
      }

      widgetData.push({
        AlertSerie: station.AlertSerie,
        Descrizione: station.Descrizione,
        Series: series
      } as WidgetData)
    }

    this.widgetData = widgetData;
  }
}

interface WidgetData {
  AlertSerie: AlertSerie[];
  Descrizione: string;
  Series: Series[];
}

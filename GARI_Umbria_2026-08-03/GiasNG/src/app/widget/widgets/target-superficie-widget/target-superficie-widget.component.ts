import { Component, OnDestroy, OnInit } from "@angular/core";
import { WidgetStatisticheClient, Widget_Statistics_IN } from "app/Service/net-core6-api.service";
import { GestioneMultiAziendaService } from "app/widget-config/gestione-multiazienda.service";
import { combineLatest, finalize, Subject, takeUntil } from "rxjs";

@Component({
  standalone: false,
  selector: 'app-target-superficie-widget',
  templateUrl: './target-superficie-widget.component.html',
  styleUrls: ['./target-superficie-widget.component.scss']
})
export class TargetSuperficieWidgetComponent implements OnInit, OnDestroy {

  loading = true;
  scale;
  value;   //test
  total;
  minorUnit;
  majorUnit;

  labelFormat: string = 'n0';

  public colors = [];

  private signal$ = new Subject<void>();

  constructor(
    private widgetsClient: WidgetStatisticheClient,
    private gestioneMultiAziendaService: GestioneMultiAziendaService
  ) { }

  ngOnInit(): void {
    combineLatest({
      nazione: this.gestioneMultiAziendaService.nazioneMultiAzienda$,
      anno: this.gestioneMultiAziendaService.annoMultiAzienda$
    })
      .pipe(takeUntil(this.signal$))
      .subscribe(({ nazione, anno }) => {
        this.loadData(nazione, anno);
      });
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }


  loadData(nazione: any, anno: any) {
    this.loading = true;
    if (nazione?.code != "") {

      const payload = {
        Year: anno?.text,
        Country: nazione?.code
      } as Widget_Statistics_IN;

      this.widgetsClient
        .widgetStatisticheGetTargetHA(payload)
        .pipe(finalize(() => this.loading = false))
        .subscribe(data => {
          const dataRisposta = JSON.parse(data.RispostaStringa);
          dataRisposta.forEach((val) => {
            this.value = val["TOTAL_HA"];
            this.total = val["TARGET_HA"];
          });

          // this.value = (this.value/this.total)*100;

          this.majorUnit = Math.floor(this.total / 5);

          //const limit: number = 20;

          // this.colors = [
          //   {
          //     to: limit,
          //     color: "	#FF0000",
          //   },
          //   {
          //     from: limit,
          //     to: 2*limit,
          //     color: "#DC143C",
          //   },
          //   {
          //     from: 2*limit,
          //     to: 3*limit,
          //     color: "#FFA500",
          //   },
          //   {
          //     from: 3*limit,
          //     to: 4*limit,
          //     color: "#FFFF00",
          //   },
          //   {
          //     from: 4*limit,
          //     to: 5*limit,
          //     color: "#ADFF2F",
          //   },
          //   {
          //     from: 5*limit,
          //     color: "#00FF00",
          //   },
          // ];

        });

    } else {
      this.total = -1;
    }

    this.loading = false;
  }
}

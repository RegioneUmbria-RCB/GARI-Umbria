import { Component, OnDestroy, OnInit } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { WidgetStatisticheClient, Widget_Statistics_IN } from "app/Service/net-core6-api.service";
import { GestioneMultiAziendaService } from "app/widget-config/gestione-multiazienda.service";
import { combineLatest, finalize, Subject, takeUntil } from "rxjs";

@Component({
  standalone: false,
  selector: 'app-dati-generali-farmers-widget',
  templateUrl: './dati-generali-farmers-widget.component.html',
  styleUrls: ['./dati-generali-farmers-widget.component.scss']
})
export class DatiGeneraliFarmersComponent implements OnInit, OnDestroy {

  loading = true;
  data = [];

  resultData: string;

  private signal$ = new Subject<void>();

  constructor(
    private widgetsClient: WidgetStatisticheClient,
    private transloco: TranslocoService,
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

      this.widgetsClient.widgetStatisticheGetGeneralStatistics(payload)
        .pipe(finalize(() => this.loading = false))
        .subscribe(res => {
          const dataRisposta = JSON.parse(res.RispostaStringa);
          dataRisposta.forEach((val) => {
            Object.entries(val).forEach(([key, value]) => {
              const num: number = value as number;
              this.data.push({ 'descrizione': this.transloco.translate(key), 'valore': num?.toLocaleString('it-IT', { useGrouping: true }) });
            });
          });
        });

    }
    this.loading = false;
  }
}

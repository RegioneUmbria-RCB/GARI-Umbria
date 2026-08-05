import { Component, Input, OnInit } from '@angular/core';
import { Gias2010Redirector } from 'app/menu-agenda/components/grid-qdc/Gias2010Redirector.service';
import { WidgetsClient, Widget_Operazione, Widget_Operazioni_IN } from 'app/Service/api.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { finalize } from 'rxjs';

const DEFAULT_NUMERO_MOVIMENTI = 5;

@Component({
  standalone: false,
  selector: 'app-ultime-attivita-widget',
  templateUrl: './ultime-attivita-widget.component.html',
  styleUrls: ['./ultime-attivita-widget.component.css']
})
export class UltimeAttivitaWidgetComponent implements OnInit {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;

  data: Widget_Operazione[] = [];
  loading = true;

  constructor(
    private widgetsClient: WidgetsClient,
    private gias2010Redirector: Gias2010Redirector
  ) { }

  ngOnInit(): void {
    if (this.piva == null) {
      this.loading = false;
      return;
    }

    this.loading = true;

    const body = {
      NumeroMovimenti: DEFAULT_NUMERO_MOVIMENTI,
      Piva: this.piva
    } as Widget_Operazioni_IN;

    this.widgetsClient
      .widgetsLeggiUltimeAttivita(body)
      .pipe(finalize(() => this.loading = false))
      .subscribe(res => this.data = res.RispostaStringa);
  }

  navigate(payload: [Widget_Operazione, Enum_DBTypeOperation]): void {
    const attivita = payload[0];
    const operation = payload[1];
    this.gias2010Redirector.redirectToQdCFromDashboard(attivita.IdAgenda, attivita.Piva, attivita.Rag_Soc, attivita.LavCod, operation, attivita.Data_Operazione);
  }
}

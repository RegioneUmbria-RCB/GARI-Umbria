import { Component, Input, OnInit } from '@angular/core';
import { Gias2010Redirector } from 'app/menu-agenda/components/grid-qdc/Gias2010Redirector.service';
import { WidgetsClient, Widget_Operazione, Widget_Operazioni_IN } from 'app/Service/api.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { finalize } from 'rxjs';

const DEFAULT_NUMERO_MOVIMENTI = 5;

@Component({
  standalone: false,
  selector: 'app-ultime-visite-widget',
  templateUrl: './ultime-visite-widget.component.html',
  styleUrls: ['./ultime-visite-widget.component.css']
})
export class UltimeVisiteWidgetComponent implements OnInit {
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
      .widgetsLeggiUltimeVisite(body)
      .pipe(finalize(() => this.loading = false))
      .subscribe(res => this.data = res.RispostaStringa);
  }

  navigate(payload: [Widget_Operazione, number]) {
    const visita = payload[0];
    const operation = payload[1];
    const agenda = this.getParametriAgenda(visita, operation);
    this.gias2010Redirector.gestisciRedirectToQdC(agenda);
  }

  private getParametriAgenda(attivita: Widget_Operazione, operation: number): ObjParametriAgenda {
    const agenda = new ObjParametriAgenda();
    agenda.Id_Agenda = attivita.IdAgenda;
    agenda.Lav_Cod = attivita.LavCod;
    agenda.Piva = attivita.Piva;
    agenda.TipoOperazioneAgenda = operation;
    agenda.Veg_Cod = -1;
    agenda.Veg_Des = " ";
    return agenda;
  }
}

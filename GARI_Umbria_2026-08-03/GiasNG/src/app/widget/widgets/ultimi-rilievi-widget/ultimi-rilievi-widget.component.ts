import { Component, Input, OnInit } from '@angular/core';
import { Gias2010Redirector } from 'app/menu-agenda/components/grid-qdc/Gias2010Redirector.service';
import { WidgetsClient, Widget_Operazione, Widget_Operazioni_IN } from 'app/Service/api.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { finalize } from 'rxjs';

const DEFAULT_LAV_COD = "113,110,109,108,79,119,126,169,125";
const DEFAULT_NUMERO_MOVIMENTI = 5;

@Component({
  standalone: false,
  selector: 'app-ultimi-rilievi-widget',
  templateUrl: './ultimi-rilievi-widget.component.html',
  styleUrls: ['./ultimi-rilievi-widget.component.css']
})
export class UltimiRilieviWidgetComponent implements OnInit {
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
      Filtro_Lav_Cod: DEFAULT_LAV_COD,
      NumeroMovimenti: DEFAULT_NUMERO_MOVIMENTI,
      Piva: this.piva
    } as Widget_Operazioni_IN;

    this.widgetsClient
      .widgetsLeggiUltimiRilievi(body)
      .pipe(finalize(() => this.loading = false))
      .subscribe(res => this.data = res.RispostaStringa);
  }

  navigate(payload: [Widget_Operazione, number]) {
    const rilievo = payload[0];
    const operation = payload[1];
    const agenda = this.getParametriAgenda(rilievo, operation);
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

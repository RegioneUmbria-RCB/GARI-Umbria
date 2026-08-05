import { Component, Input, OnInit } from '@angular/core';
import { WidgetsClient, Widget_GHGColture_IN, Widget_GHGColture } from 'app/Service/api.service';
import { finalize } from 'rxjs';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';

const DEFAULT_NUMERO_COLTURE = 10;

@Component({
  standalone: false,
  selector: 'app-ghg-colture-widget',
  templateUrl: './ghg-colture-widget.component.html',
  styleUrls: ['./ghg-colture-widget.component.scss']
})
export class GhgColtureWidgetComponent implements OnInit {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;

  data: Widget_GHGColture[] = [];
  loading = true;
  chartPie = true;
  total = false;

  constructor(
    private widgetsClient: WidgetsClient,
    private funzioniComuni: FunzioniComuniService
  ) { }

  ngOnInit(): void {
    if (this.piva == null) {
      this.loading = false;
      return;
    }

    this.loading = true;

    const payload = {
      NumeroMovimenti: DEFAULT_NUMERO_COLTURE,
      Piva: this.piva
    } as Widget_GHGColture_IN;

    this.widgetsClient
      .widgetsLeggiGHGColture(payload)
      .pipe(finalize(() => this.loading = false))
      .subscribe(data => this.processResponse(data.RispostaStringa));
  }

  private processResponse(response: Widget_GHGColture[]): void {
    const result = response ?? [];
    for (const element of result) {
      element.Eec_Ha = this.funzioniComuni.roundNumber(element.Eec_Ha, 2);
      element.Eec_Totale = this.funzioniComuni.roundNumber(element.Eec_Totale, 2);
    }

    this.data = result;
  }
}

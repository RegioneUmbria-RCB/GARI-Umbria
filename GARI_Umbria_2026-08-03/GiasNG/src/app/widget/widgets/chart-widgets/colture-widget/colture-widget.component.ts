import { Component, Input, OnInit } from '@angular/core';
import { WidgetsClient, Widget_Coltura, Widget_Culture_IN } from 'app/Service/api.service';
import { finalize } from 'rxjs';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';

const DEFAULT_NUMERO_COLTURE = 10;

@Component({
  standalone: false,
  selector: 'app-colture-widget',
  templateUrl: './colture-widget.component.html',
  styleUrls: ['./colture-widget.component.scss']
})
export class ColtureWidgetComponent implements OnInit {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;

  data: Widget_Coltura[] = [];
  loading = true;
  chartPie = true;

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
    } as Widget_Culture_IN;

    this.widgetsClient
      .widgetsLeggiColture(payload)
      .pipe(finalize(() => this.loading = false))
      .subscribe(data => this.processResponse(data.RispostaStringa));
  }

  private processResponse(response: Widget_Coltura[]): void {
    const result = response ?? [];
    for (const element of result) {
      //element.Superficie = Math.round(element.Superficie);
      element.Superficie = this.funzioniComuni.roundNumber(element.Superficie, 2);

    }

    this.data = result;
  }
}

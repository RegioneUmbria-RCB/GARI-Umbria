import { Component, Input, OnInit } from '@angular/core';
import { WidgetsClient, Widget_StimeProduzioneColture_IN, Widget_StimeProduzioneColture } from 'app/Service/api.service';
import { finalize } from 'rxjs';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';

const DEFAULT_NUMERO_COLTURE = 10;

@Component({
  standalone: false,
  selector: 'app-stime-produzione-colture-widget',
  templateUrl: './stime-produzione-colture-widget.component.html',
  styleUrls: ['./stime-produzione-colture-widget.component.scss']
})
export class StimeProduzioneColtureWidgetComponent implements OnInit {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;

  data: Widget_StimeProduzioneColture[] = [];
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
    } as Widget_StimeProduzioneColture_IN;

    this.widgetsClient
      .widgetsLeggiStimeProduzioneColture(payload)
      .pipe(finalize(() => this.loading = false))
      .subscribe(data => this.processResponse(data.RispostaStringa));
  }

  private processResponse(response: Widget_StimeProduzioneColture[]): void {
    const result = response ?? [];
    for (const element of result) {
      element.StimaProduzione_Ha = this.funzioniComuni.roundNumber(element.StimaProduzione_Ha, 2);
      element.StimaProduzione_Totale = this.funzioniComuni.roundNumber(element.StimaProduzione_Totale, 2);
    }

    this.data = result;
  }
}

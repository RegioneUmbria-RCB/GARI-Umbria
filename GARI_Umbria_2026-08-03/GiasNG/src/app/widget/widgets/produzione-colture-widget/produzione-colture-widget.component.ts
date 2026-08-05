import { Component, Input, OnInit } from '@angular/core';
import { NumericFilterCellComponent } from '@progress/kendo-angular-grid';
import { WidgetsClient, Widget_Culture_IN, Widget_ProduzioneColtura } from 'app/Service/api.service';
import { finalize } from 'rxjs';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';

@Component({
  standalone: false,
  selector: 'app-produzione-colture-widget',
  templateUrl: './produzione-colture-widget.component.html',
  styleUrls: ['./produzione-colture-widget.component.css']
})
export class ProduzioneColtureWidgetComponent implements OnInit {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;

  data: Widget_ProduzioneColtura[] = [];
  loading = true;

  constructor(private widgetsClient: WidgetsClient, private funzioniComuni: FunzioniComuniService) { }

  ngOnInit(): void {
    if (this.piva == null) {
      this.loading = false;
      return;
    }

    this.loading = true;

    const payload = {
      NumeroMovimenti: 5,
      Piva: this.piva
    } as Widget_Culture_IN;

    this.widgetsClient
      .widgetsLeggiProduzioneColture(payload)
      .pipe(finalize(() => this.loading = false))
      .subscribe(data => this.data = data.RispostaStringa);
  }

  getRoundedData(value: number): number {
    return this.funzioniComuni.roundNumber(value,2);
  }
}

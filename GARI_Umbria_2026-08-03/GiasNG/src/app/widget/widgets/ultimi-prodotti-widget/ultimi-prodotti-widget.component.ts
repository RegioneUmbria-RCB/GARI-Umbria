import { Component, Input, OnInit } from '@angular/core';
import { Enum_Codice_Widget, WidgetsClient } from 'app/Service/api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { UltimiProdottiWidgetGridConfig } from './ultimi-prodotti-widget-grid-config.service';


@Component({
  standalone: false,
  selector: 'app-ultimi-prodotti-widget',
  templateUrl: './ultimi-prodotti-widget.component.html',
  styleUrls: ['./ultimi-prodotti-widget.component.scss'],
  providers: [
    ...generateGridProviders(UltimiProdottiWidgetGridConfig, UltimiProdottiWidgetComponent)
  ]
})
export class UltimiProdottiWidgetComponent implements OnInit {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;

  private navigationUrl: string | null = null;

  constructor(
    private widgetsClient: WidgetsClient,
    private giasMessageService: GiasMessageService
  ) { }

  ngOnInit(): void {
    if (this.piva == null) {
      return;
    }

    this.widgetsClient
      .widgetsLinkGestioneCompleta(this.piva, Enum_Codice_Widget.ProdottiMovimentatiGiacenze)
      .subscribe(x => this.navigationUrl = x.RispostaStringa);
  }

  navigateToAllProducts(): void {
    if (this.navigationUrl == null) {
      this.giasMessageService.errorMessage('SiEVerificatoUnErrore', true, true);
      return;
    }

    window.location.assign(this.navigationUrl);
  }
}

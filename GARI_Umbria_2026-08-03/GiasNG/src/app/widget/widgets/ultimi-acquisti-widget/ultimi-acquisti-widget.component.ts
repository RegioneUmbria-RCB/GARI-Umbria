import { Component, Input } from '@angular/core';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { UltimiAcquistiWidgetGridConfig } from './ultimi-acquisti-widget-grid-config.service';


@Component({
  standalone: false,
  selector: 'app-ultimi-acquisti-widget',
  templateUrl: './ultimi-acquisti-widget.component.html',
  styleUrls: ['./ultimi-acquisti-widget.component.scss'],
  providers: [
    ...generateGridProviders(UltimiAcquistiWidgetGridConfig, UltimiAcquistiWidgetComponent)
  ]
})
export class UltimiAcquistiWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;

  private navigationUrl: string | null = null;

  constructor(
    private giasMessageService: GiasMessageService
  ) { }

  navigateToAllPurchases(): void {
    if (this.navigationUrl == null) {
      this.giasMessageService.errorMessage('SiEVerificatoUnErrore', true, true);
      return;
    }

    window.location.assign(this.navigationUrl);
  }

}

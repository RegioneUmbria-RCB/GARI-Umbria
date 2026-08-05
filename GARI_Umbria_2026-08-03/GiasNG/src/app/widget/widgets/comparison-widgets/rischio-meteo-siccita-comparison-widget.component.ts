import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-rischio-meteo-siccita-comparison-widget',
  template: '<app-comparison-widget [piva]="piva" index="IndiceRischioSiccita"></app-comparison-widget>',
})
export class RischioMeteoSiccitaComparisonWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;
}

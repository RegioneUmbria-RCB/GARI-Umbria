import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-rischio-meteo-allagamento-comparison-widget',
  template: '<app-comparison-widget [piva]="piva" index="IndiceRischioAllagamento"></app-comparison-widget>',
})
export class RischioMeteoAllagamentoComparisonWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;
}

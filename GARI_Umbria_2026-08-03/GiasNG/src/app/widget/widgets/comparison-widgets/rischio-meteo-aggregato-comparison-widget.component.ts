import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-rischio-meteo-aggregato-comparison-widget',
  template: '<app-comparison-widget [piva]="piva" index="IndiceRischioMeteoAggregato"></app-comparison-widget>',
})
export class RischioMeteoAggregatoComparisonWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;
}

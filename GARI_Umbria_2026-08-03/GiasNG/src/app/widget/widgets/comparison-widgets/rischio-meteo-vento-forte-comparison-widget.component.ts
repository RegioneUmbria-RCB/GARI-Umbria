import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-rischio-meteo-vento-forte-comparison-widget',
  template: '<app-comparison-widget [piva]="piva" index="IndiceRischioVentoForte"></app-comparison-widget>',
})
export class RischioMeteoVentoForteComparisonWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;
}

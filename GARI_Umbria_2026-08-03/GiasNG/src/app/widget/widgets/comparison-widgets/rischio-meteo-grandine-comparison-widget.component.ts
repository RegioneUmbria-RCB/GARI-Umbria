import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-rischio-meteo-grandine-comparison-widget',
  template: '<app-comparison-widget [piva]="piva" index="IndiceRischioGrandine"></app-comparison-widget>',
})
export class RischioMeteoGrandineComparisonWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;
}

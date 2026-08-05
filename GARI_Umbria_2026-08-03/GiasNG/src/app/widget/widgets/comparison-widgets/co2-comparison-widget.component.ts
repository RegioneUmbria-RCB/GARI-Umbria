import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-co2-comparison-widget',
  template: '<app-comparison-widget [piva]="piva" index="IndiceCO2"></app-comparison-widget>',
})
export class Co2ComparisonWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;
}

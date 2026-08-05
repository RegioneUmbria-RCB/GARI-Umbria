import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-rischio-meteo-gelata-comparison-widget',
  template: '<app-comparison-widget [piva]="piva" index="IndiceRischioGelata"></app-comparison-widget>',
})
export class RischioMeteoGelataComparisonWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;
}

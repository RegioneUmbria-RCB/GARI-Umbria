import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-plv-comparison-widget',
  template: '<app-comparison-widget [piva]="piva" index="PLV"></app-comparison-widget>',
})
export class PlvComparisonWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;
}

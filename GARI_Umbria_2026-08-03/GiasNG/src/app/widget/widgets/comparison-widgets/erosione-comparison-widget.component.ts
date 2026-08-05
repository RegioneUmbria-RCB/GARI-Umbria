import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-erosione-comparison-widget',
  template: '<app-comparison-widget [piva]="piva" index="IndiceErosione"></app-comparison-widget>',
})
export class ErosioneComparisonWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;
}

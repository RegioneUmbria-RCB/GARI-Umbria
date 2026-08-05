import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-produttivita-comparison-widget',
  template: '<app-comparison-widget [piva]="piva" index="Produttivita"></app-comparison-widget>',
})
export class ProduttivitaComparisonWidgetComponent {
  @Input() params: string | null = null;
  @Input() piva: string | null = null;
}

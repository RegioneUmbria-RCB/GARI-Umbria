import { Component, Input } from '@angular/core';
import { AxisLabelContentArgs } from '@progress/kendo-angular-charts';

@Component({
  standalone: false,
  selector: 'app-chart-widget',
  templateUrl: './chart-widget.component.html',
  styleUrls: ['./chart-widget.component.scss']
})
export class ChartWidgetComponent {
  @Input() data: Culture[] = [];
  @Input() field: string;
  @Input() unit?: string;
  @Input() chartPie: boolean;

  labelContent = (e: AxisLabelContentArgs): string => {
    return `${e.value} ${e.dataItem.Udm_Sim ?? this.unit}`;
  };
}

interface Culture {
  Veg_Cod?: number;
  Veg_Des?: string | null;
}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { WidgetData } from 'app/widget/widgets.component';

@Component({
  standalone: false,
  selector: 'app-widget-resizer',
  templateUrl: './widget-resizer.component.html',
  styleUrls: ['./widget-resizer.component.css']
})
export class WidgetResizerComponent implements OnInit {
  MAX_WIDTH = 5;
  MAX_HEIGHT = 5;

  @Input() widget: WidgetData | null = null;

  @Output() onSave = new EventEmitter<void>();

  isVisible = false;
  matrix: boolean[][] = [];
  minX: number = 0;
  minY: number = 0;

  constructor(private translocoService: TranslocoService) { }

  ngOnInit(): void {
    if (this.widget == null) {
      throw Error("Input widget cannot be null");
    }

    this.minX = this.widget.aspettoReale.minX;
    this.minY = this.widget.aspettoReale.minY;

    const area = this.widget.aspettoReale.gridArea;
    const { spanX, spanY } = getSpanAsNumbers(area);
    this.computeMatrix(spanX, spanY);
  }

  computeMatrix(spanX: number, spanY: number): void {
    if (spanX < this.minX || spanY < this.minY) {
      return;
    }

    const matrix: boolean[][] = [];
    for (let i = 0; i < this.MAX_HEIGHT; i++) {
      matrix[i] = [];
      for (let j = 0; j < this.MAX_WIDTH; j++) {
        matrix[i][j] = j < spanX && i < spanY;
      }
    }

    this.matrix = matrix;
  }

  saveWidget(spanX: number, spanY: number): void {
    if (spanX < this.minX || spanY < this.minY) {
      return;
    }

    this.computeMatrix(spanX, spanY);
    this.widget.aspettoReale.gridArea = `span ${spanY} / span ${spanX}`;
    this.onSave.emit();
    this.isVisible = false;
  }

  trackByIndex(index: number, obj: any): any {
    return index;
  }

  getTooltip(x: number, y: number): string {
    if (x < this.minX && y < this.minY) {
      return this.translocoService.translate("WidgetAltezzaMinima", { altezza: this.minY }) + ", " + this.translocoService.translate("WidgetLarghezzaMinima", { larghezza: this.minX });
    }

    if (x < this.minX) {
      return this.translocoService.translate("WidgetLarghezzaMinima", { larghezza: this.minX });
    }

    if (y < this.minY) {
      return this.translocoService.translate("WidgetAltezzaMinima", { altezza: this.minY });
    }

    return "";
  }
}

export function getSpanAsNumbers(gridArea: string): { spanX: number, spanY: number } {
  const regex = /span (\d) \/ span (\d)/g;
  const match = regex.exec(gridArea);

  const spanY = match == null ? 1 : +match[1];
  const spanX = match == null ? 1 : +match[2];
  return { spanX, spanY };
}

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Indicatore } from '../indicatori-widget.models';

@Component({
  standalone: false,
  selector: 'app-indicatori-widget-full',
  templateUrl: './indicatori-widget-full.component.html',
  styleUrls: ['./indicatori-widget-full.component.css'],
})
export class IndicatoriWidgetFullComponent {
  @Input() data: Indicatore[] = [];
  @Output() closeEvent = new EventEmitter<void>();

  // Kendo dialog sizes
  height = window.innerHeight;
  width = window.innerWidth;

  tabSelected: 1 | 2 = 1;

  getGridHeight(): number {
    let grid = document.getElementById("indicatori-widget-grid");
    if (grid == null) {
      return 0;
    }

    const toolbar = grid.querySelector(".k-grid-toolbar");
    if (toolbar == null) {
      return 0;
    }

    const header = grid.querySelector(".k-grid-header");
    if (header == null) {
      return 0;
    }

    const toolbarHeight = Math.ceil(toolbar.clientHeight);
    const headerHeight = Math.ceil(header.clientHeight);
    const parentHeight = grid.parentElement.clientHeight;
    return Math.floor(parentHeight - (toolbarHeight + headerHeight) - 4);
  }

  onClose(): void {
    this.closeEvent.emit();
  }
}

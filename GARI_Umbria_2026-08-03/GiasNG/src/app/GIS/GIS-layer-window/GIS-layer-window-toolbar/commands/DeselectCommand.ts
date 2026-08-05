import { enum_GISDrawingOperations } from 'app/GIS/GIS-enum/GIS-drawing-operations';
import { DrawWindowOperationService } from 'app/GIS/GIS-kendo-window/draw-window/draw-window-operation.service';
import { Command } from '../GIS-layer-window-toolbar.component';

export class DeselectCommand implements Command {
  constructor(private drawWindowOperationService: DrawWindowOperationService) { }

  do(): void {
    this.drawWindowOperationService.setOperation(enum_GISDrawingOperations.erase);
  }
}

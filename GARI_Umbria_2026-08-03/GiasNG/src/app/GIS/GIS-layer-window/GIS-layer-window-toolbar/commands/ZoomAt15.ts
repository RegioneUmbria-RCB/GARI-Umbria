import { Command } from '../GIS-layer-window-toolbar.component';

export class ZoomAt15Command implements Command {
  constructor(private googleMap: google.maps.Map) { }

  do(): void {
    this.googleMap.setZoom(15);
  }
}

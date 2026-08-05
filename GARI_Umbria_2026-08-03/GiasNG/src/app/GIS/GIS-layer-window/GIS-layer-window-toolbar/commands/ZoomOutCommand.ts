import { Command } from '../GIS-layer-window-toolbar.component';

export class ZoomOutCommand implements Command {
  constructor(private googleMap: google.maps.Map) { }

  do(): void {
    this.googleMap.setZoom(this.googleMap.getZoom() - 1);
  }
}

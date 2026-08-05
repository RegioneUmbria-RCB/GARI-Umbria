import { Command } from '../GIS-layer-window-toolbar.component';

export class ZoomInCommand implements Command {
  constructor(private googleMap: google.maps.Map) { }

  do(): void {
    this.googleMap.setZoom(this.googleMap.getZoom() + 1);
  }
}

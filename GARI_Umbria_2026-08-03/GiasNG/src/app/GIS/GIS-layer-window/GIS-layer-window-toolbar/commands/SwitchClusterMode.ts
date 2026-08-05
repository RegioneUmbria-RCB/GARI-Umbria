import { GoogleMapHeatmapService } from "app/GIS/services/google.maps-services/google-map-heatmap.service";
import { Command } from "../GIS-layer-window-toolbar.component";

export class SwitchClusterModeCommand implements Command {
  constructor(private googleMapHeatmapService: GoogleMapHeatmapService) {}

  do(): void {
    this.googleMapHeatmapService.switchHeatmapDisplay();
  }
}

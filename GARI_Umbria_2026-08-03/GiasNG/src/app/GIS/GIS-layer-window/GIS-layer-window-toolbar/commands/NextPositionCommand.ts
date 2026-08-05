import { GoogleMapMovementControlService } from 'app/GIS/services/google-map-movement-control.service';
import { Command } from '../GIS-layer-window-toolbar.component';

export class NextPositionCommand implements Command {
  constructor(
    private googleMap: google.maps.Map,
    private googleMapMovementControlService: GoogleMapMovementControlService
  ) { }

  do(): void {
    this.googleMapMovementControlService.goToNextPosition(this.googleMap);
  }
}

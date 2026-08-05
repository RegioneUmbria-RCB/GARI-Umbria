import { Component, OnDestroy, ViewChild } from '@angular/core';
import { enum_GISDrawingOperations } from 'app/GIS/GIS-enum/GIS-drawing-operations';
import { enum_OrigineChiamata } from 'app/GIS/GIS-enum/GIS-origine-chiamata';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { DrawingManagerService } from 'app/GIS/services/drawing-manager.service';
import { DrawingService } from 'app/GIS/services/drawing.service';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { Subscription } from 'rxjs';
import { DrawWindowOperationService } from '../draw-window/draw-window-operation.service';
import { ToolDisegnaPuntiComponent } from './tool-disegna-punti/tool-disegna-punti.component';
import { ToolScomponiPuntiComponent } from './tool-scomponi-punti/tool-scomponi-punti.component';
import { ToolDisegnoAvanzatoComponent } from './tool-disegno-avanzato/tool-disegno-avanzato.component';

@Component({
  standalone: false,
  selector: 'app-marker-window',
  templateUrl: './marker-window.component.html',
  styleUrls: ['./marker-window.component.css']
})
export class MarkerWindowComponent implements OnDestroy {
  @ViewChild(ToolDisegnaPuntiComponent) toolDisegnaPuntiComponent: IOutsideResettableComponent;
  @ViewChild(ToolScomponiPuntiComponent) toolScompoToolScomponiPuntiComponent: IOutsideResettableComponent;
  @ViewChild(ToolDisegnoAvanzatoComponent) toolScompoToolDisegnoAvanzatoComponent: IOutsideResettableComponent;

  windowArgs: WindowArgs;
  isCloseConfirmationOpen: boolean = false;
  protected modality: MarkerWindowModality = MarkerWindowModality.Marker;
  markerWindowModality = MarkerWindowModality;

  private subscriptions: Subscription[] = [];

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private drawingService: DrawingService,
    private drawingManagerService: DrawingManagerService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private drawWindowOperationService: DrawWindowOperationService
  ) {
    if (this.kendoWindowsService.getOpenState(WindowTypes.MarkerWindow)) {
      this.windowArgs = this.kendoWindowsService.getWindowArgs(WindowTypes.MarkerWindow);
    }

    this.subscriptions.push(
      this.kendoWindowsService
        .windowToggle$
        .subscribe(([windowTypes, args]) => {
          if (windowTypes === WindowTypes.MarkerWindow) {
            this.windowArgs = args;
            this.modality = args.additionalArgs?.modality ?? MarkerWindowModality.Marker;

            if (this.windowArgs.openState == false) {
              this.doReset(false);
            }
          }
        })
    );
  }

  ngOnDestroy(): void {
    this.kendoWindowsService.close(WindowTypes.MarkerWindow);

    for (const sub of this.subscriptions) {
      sub.unsubscribe();
    }
  }

  reset(askUser: boolean): void {
    if (askUser) {
      this.isCloseConfirmationOpen = true;
      return;
    }

    this.doReset();
  }

  doReset(emit: boolean = true): void {
    if (this.modality == MarkerWindowModality.Marker) {
      this.toolDisegnaPuntiComponent?.onOutsideReset();
    }

    if (this.modality == MarkerWindowModality.Scomponi) {
      this.toolScompoToolScomponiPuntiComponent?.onOutsideReset();
    }

    if (this.modality == MarkerWindowModality.DisegnoAvanzato) {
      this.toolScompoToolDisegnoAvanzatoComponent?.onOutsideReset();
    }

    this.isCloseConfirmationOpen = false;
    this.drawingManagerService.resetDrawingManager();
    this.drawingService.removeAllMarkers();
    this.kendoWindowsService.open(WindowTypes.DrawWindow);
    this.drawWindowOperationService.setOperation(enum_GISDrawingOperations.none);
    this.googleMapGeoJsonService.seDeselezionaFeatureSelezionate(enum_OrigineChiamata.Mappa);
    this.modality = MarkerWindowModality.None;

    if (emit) {
      this.kendoWindowsService.close(WindowTypes.MarkerWindow);
    }
  }
}


export enum MarkerWindowModality {
  None,
  Marker,
  Scomponi,
  DisegnoAvanzato
}

export interface IOutsideResettableComponent {
  onOutsideReset(): void;
}

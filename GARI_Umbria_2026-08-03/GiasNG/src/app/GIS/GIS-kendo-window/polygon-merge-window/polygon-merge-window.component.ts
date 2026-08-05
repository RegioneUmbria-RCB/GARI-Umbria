import { Component } from "@angular/core";
import { enum_GISDrawingOperations } from "app/GIS/GIS-enum/GIS-drawing-operations";
import { KendoWindowsService, WindowArgs, WindowTypes } from "app/Service";
import { Observable, filter, map, pairwise, startWith, tap } from "rxjs";
import { DrawWindowOperationService } from "../draw-window/draw-window-operation.service";

@Component({
  standalone: false,
  selector: 'gis-polygon-merge-window',
  templateUrl: './polygon-merge-window.component.html',
  styleUrls: ['./polygon-merge-window.component.css'],
})
export class PolygonMergeWindowComponent {
  windowArgs$: Observable<WindowArgs> = this.kendoWindowsService
    .getWindowArgs$(WindowTypes.PolygonMergeWindow)
    .pipe(
      startWith(null),
      pairwise(),
      tap(([oldArgs, newArgs]) => {
        if (oldArgs?.openState && !newArgs?.openState) {
          this.doReset(false);
        }
      }),
      map(([_, newArgs]) => newArgs)
    );

  isCloseConfirmationOpen: boolean = false;

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private drawWindowOperationService: DrawWindowOperationService
  ) { }

  reset(askUser: boolean): void {
    if (askUser) {
      this.isCloseConfirmationOpen = true;
      return;
    }

    this.doReset();
  }

  doReset(emit: boolean = true): void {
    this.isCloseConfirmationOpen = false;
    // this.drawingManagerService.resetDrawingManager();
    this.kendoWindowsService.open(WindowTypes.DrawWindow);
    this.drawWindowOperationService.setOperation(enum_GISDrawingOperations.none);
    // this.googleMapGeoJsonService.seDeselezionaFeatureSelezionate(enum_OrigineChiamata.Mappa);

    if (emit) {
      this.kendoWindowsService.close(WindowTypes.PolygonMergeWindow);
    }
  }
}

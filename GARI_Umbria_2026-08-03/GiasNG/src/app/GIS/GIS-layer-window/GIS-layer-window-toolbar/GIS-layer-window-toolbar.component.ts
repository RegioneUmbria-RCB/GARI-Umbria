import { Component, Input } from '@angular/core';
import { DrawWindowOperationService } from 'app/GIS/GIS-kendo-window/draw-window/draw-window-operation.service';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { BehaviorSubject, filter, map, pairwise, startWith, take, tap, withLatestFrom } from 'rxjs';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { GoogleMapMovementControlService } from 'app/GIS/services/google-map-movement-control.service';
import { DeselectCommand } from './commands/DeselectCommand';
import { ZoomAreaCommand } from './commands/ZoomAreaCommand';
import { ZoomInCommand } from './commands/ZoomInCommand';
import { ZoomOutCommand } from './commands/ZoomOutCommand';
import { SelectAreaCommand } from './commands/SelectAreaCommand';
import { PrevPositionCommand } from './commands/PrevPositionCommand';
import { NextPositionCommand } from './commands/NextPositionCommand';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { OpenBookmarksCommand } from './commands/OpenBookmarksCommand';
import { TranslocoService } from '@jsverse/transloco';
import { ZoomAt15Command } from './commands/ZoomAt15';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';
import { GoogleMapHeatmapService } from 'app/GIS/services/google.maps-services/google-map-heatmap.service';
import { SwitchClusterModeCommand } from './commands/SwitchClusterMode';
import { GISModality } from 'app/GIS/GIS-enum/GIS-feature';

@Component({
  standalone: false,
  selector: 'gis-layer-window-toolbar',
  templateUrl: './GIS-layer-window-toolbar.component.html',
  styleUrls: ['./GIS-layer-window-toolbar.component.css']
})
export class GISLayerWindowToolbarComponent {
  @Input() modality: GISModality | undefined = GISModality.Full;
  GISModality = GISModality;

  private operationSubject = new BehaviorSubject<LayerWindowToolbarOperation>(null);
  private operations = new Map<LayerWindowToolbarOperation, Command>();

  layerWindowToolbarOperations = LayerWindowToolbarOperation;

  operation$ = this.operationSubject.asObservable()
    .pipe(
      pairwise(),
      tap(([oldOperation, newOperation]) => this.handleOperation(oldOperation, newOperation)),
      map(([_, newOperation]) => newOperation)
    );

  layer$ = this.layerService.layerItemSelected$
    .pipe(
      map(([layer, selected]) => selected == false ? null : layer),
      startWith(null),
      withLatestFrom(this.operationSubject.asObservable()),
      map(([layer, operation]) => {
        if (layer == null && operation == LayerWindowToolbarOperation.SelectArea) {
          // Disable SelectArea if current layer is deselected
          this.selectOperation(null, false);
        }

        return layer;
      })
    );

  bookmarksOpen$ = this.kendoWindowsService.windowToggle$
    .pipe(
      filter(([windowType, _]) => windowType == WindowTypes.BookmarksWindow),
      map(([_, args]) => args.openState),
      startWith(false)
    );

  constructor(
    private googleMapService: GoogleMapService,
    private drawWindowOperationService: DrawWindowOperationService,
    private layerService: LayerService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private googleMapMovementControlService: GoogleMapMovementControlService,
    private kendoWindowsService: KendoWindowsService,
    private translocoService: TranslocoService,
    private featureInformationService: FeatureInformationService,
    private googleMapHeatmapService: GoogleMapHeatmapService
  ) {
    this.googleMapService.loaded$
      .pipe(take(1))
      .subscribe(() => this.initialize(this.googleMapService.googleMapWrapper.googleMap));
  }

  selectOperation(operation: LayerWindowToolbarOperation, toggle: boolean): void {
    const newOperation = toggle && this.operationSubject.value == operation ? null : operation;
    this.operationSubject.next(newOperation);
  }

  checkModality(...modality: GISModality[]): boolean {
    return modality.includes(this.modality);
  }

  setToolbarStyle(): { [p: string]: any } | null | undefined {
    if (this.checkModality(GISModality.Full)) {
      return { 'justify-content': `space-evenly` };
    } else {
      return { 'gap': `2%` };
    }
  }

  private initialize(googleMap: google.maps.Map): void {
    const drawingManager = new google.maps.drawing.DrawingManager();
    this.operations.set(LayerWindowToolbarOperation.Deselect, new DeselectCommand(this.drawWindowOperationService));
    this.operations.set(LayerWindowToolbarOperation.ZoomArea, new ZoomAreaCommand(drawingManager, googleMap, this.googleMapMovementControlService));
    this.operations.set(LayerWindowToolbarOperation.ZoomIn, new ZoomInCommand(googleMap));
    this.operations.set(LayerWindowToolbarOperation.ZoomOut, new ZoomOutCommand(googleMap));
    this.operations.set(LayerWindowToolbarOperation.ZoomAt15, new ZoomAt15Command(googleMap));
    this.operations.set(LayerWindowToolbarOperation.SelectArea, new SelectAreaCommand(drawingManager, googleMap, this.layerService, this.googleMapGeoJsonService, this.featureInformationService));
    this.operations.set(LayerWindowToolbarOperation.PrevPosition, new PrevPositionCommand(googleMap, this.googleMapMovementControlService));
    this.operations.set(LayerWindowToolbarOperation.NextPosition, new NextPositionCommand(googleMap, this.googleMapMovementControlService));
    this.operations.set(LayerWindowToolbarOperation.OpenBookmarks, new OpenBookmarksCommand(this.kendoWindowsService, this.translocoService));
    this.operations.set(LayerWindowToolbarOperation.SwitchClusterMode, new SwitchClusterModeCommand(this.googleMapHeatmapService));
  }

  private handleOperation(oldOperation: LayerWindowToolbarOperation, newOperation: LayerWindowToolbarOperation): void {
    if (oldOperation != null) {
      const command = this.operations.get(oldOperation);
      if (this.isUndoableCommand(command)) {
        (command as UndoableCommand).undo();
      }
    }

    if (newOperation != null) {
      this.operations.get(newOperation)?.do();
    }
  }

  private isUndoableCommand(command: Command | null): boolean {
    return command != null && 'undo' in command;
  }
}

enum LayerWindowToolbarOperation {
  Deselect,
  ZoomArea,
  ZoomIn,
  ZoomOut,
  ZoomAt15,
  SelectArea,
  PrevPosition,
  NextPosition,
  OpenBookmarks,
  SwitchClusterMode
}

export interface Command {
  do(): void;
}

export interface UndoableCommand extends Command {
  undo(): void;
}

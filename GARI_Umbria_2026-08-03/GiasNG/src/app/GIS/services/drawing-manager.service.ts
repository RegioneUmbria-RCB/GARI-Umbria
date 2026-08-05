import { Injectable } from "@angular/core";
import { GoogleMapService } from "../google-map/google-map.service";
import { DrawingOptionsService } from "./drawing-options.service";
import { DrawingService } from "./drawing.service";
import { enum_LayerElementiGraficiStd } from '../GIS-enum/GIS-layer-elementi-grafici';
import { TipologiaLayer } from "app/Service/api.service";
import { Subscription } from "rxjs";
import { FeatureType } from "app/Model/GIS/GisDataReadRval_New";
import {LayerService} from './layer.service';

export interface GiasDrawingManagerOptions extends google.maps.drawing.DrawingManagerOptions{
    multiPointsAllowed?: boolean;
}

@Injectable()
export class DrawingManagerService {
    private drawingManager: google.maps.drawing.DrawingManager;
    private eventListener_polygon: google.maps.MapsEventListener;
    private eventListener_marker: google.maps.MapsEventListener;
    private eventListener_polyline: google.maps.MapsEventListener;

    private _drawingControlOptions: google.maps.drawing.DrawingControlOptions;
    private _drawingManagerOptions: GiasDrawingManagerOptions;

    private readonly DEFAULT_LAYER: string = enum_LayerElementiGraficiStd.DEEFAULT;
    private layer: string = this.DEFAULT_LAYER;
    private selectedLayer: TipologiaLayer | null = null;
    private layerSub: Subscription;

    private DRAWING_MODES = [
        google.maps.drawing.OverlayType.POLYGON,
        google.maps.drawing.OverlayType.MARKER,
        google.maps.drawing.OverlayType.POLYLINE
    ];

    private get googleMapWrapper() {
        return this.googleMapService.googleMapWrapper;
    }

    constructor(
        private googleMapService: GoogleMapService,
        private drawingService: DrawingService,
        private drawingOptionsService: DrawingOptionsService,
        private layerService: LayerService
    ) {
        this.layerSub = this.layerService
            .layerItemSelected$
            .subscribe(([layer, selected]) => {
                if (selected)  {
                    this.setLayer(layer);
                }
            });
    }

    public setDrawing_Modes(value: google.maps.drawing.OverlayType) {
        this.DRAWING_MODES = [value];
    }

    public setLayer(layer: TipologiaLayer): void {
        this.layer = layer.id;

        if(this.selectedLayer?.FeatureTypeId != layer?.FeatureTypeId) {
            this.stopDrawingMode();
        }
        this.selectedLayer = layer;
    }

    public onDestroy(): void {
        this.layerSub.unsubscribe();
    }

    public startDrawingMode(validatePolygon: boolean = false) {
        this.drawingManager == undefined ? this.createDrawingManager() : this.setDrawingManager();
        this.addeventListener(validatePolygon)
    }

    private createDrawingManager() {
        this.drawingManager = new google.maps.drawing.DrawingManager();
        this.setDrawingManager();
    }

    private setDrawingManager() {
        this.drawingManager.setMap(this.googleMapWrapper.googleMap);
        this.setDrawingOptions();
        this.drawingManager.setOptions(this._drawingManagerOptions);
    }

    private setDrawingOptions() {
        this._drawingControlOptions = {};
        this._drawingControlOptions.position = google.maps.ControlPosition.BOTTOM_CENTER;
        this._drawingControlOptions.drawingModes = this.DRAWING_MODES;

        this._drawingManagerOptions = {};
        this._drawingManagerOptions.markerOptions = this.drawingOptionsService.getMarkerDrawingOptions(this.layer);
        this._drawingManagerOptions.polygonOptions = this.drawingOptionsService.getPolygonDrawingOptions(this.DEFAULT_LAYER);
        this._drawingManagerOptions.polylineOptions = this.drawingOptionsService.getPolylineDrawingOptions(this.layer);
        this._drawingManagerOptions.drawingMode = this.DRAWING_MODES[0];
        this._drawingManagerOptions.drawingControlOptions = this._drawingControlOptions;
    }

    private addeventListener(validatePolygon: boolean) {
        this.eventListener_polygon = google.maps.event.addListener(this.drawingManager, 'polygoncomplete', (event) => {
            // Polygon drawn
            this.drawingService.addDrawing(event);
            this.resetDrawingManager();
        });
        this.eventListener_marker = google.maps.event.addListener(this.drawingManager, 'markercomplete', (event) => {
            const isMultiplePointAllowed = this.isMultiplePointAllowed();

            // Marker drawn
            this.drawingService.addDrawing(event, isMultiplePointAllowed, validatePolygon);
            if (!isMultiplePointAllowed) {
                this.resetDrawingManager();
            }
        });
        this.eventListener_polyline = google.maps.event.addListener(this.drawingManager, 'polylinecomplete', (event) => {
            // Polyline drawn
            this.drawingService.addDrawing(event);
            this.resetDrawingManager();
        });
    }

    private isMultiplePointAllowed(): boolean {
        return this.selectedLayer.FeatureTypeId != `${FeatureType.Point}`;
    }

    public stopDrawingMode() {
        this.layer = this.DEFAULT_LAYER;
        this.resetDrawingManager();
    }

    public resetDrawingManager() {
        google.maps.event.removeListener(this.eventListener_polygon);
        google.maps.event.removeListener(this.eventListener_marker);
        google.maps.event.removeListener(this.eventListener_polyline);
        this.drawingManager?.setMap(null);
        this.drawingManager?.setDrawingMode(null);
        this.drawingManager?.setOptions({ drawingControlOptions: { drawingModes: [] } });
    }

    public isDrawingModeRunning(): boolean {
        return !(this.drawingManager?.getMap() == undefined || this.drawingManager?.getDrawingMode() == undefined);
    }
}

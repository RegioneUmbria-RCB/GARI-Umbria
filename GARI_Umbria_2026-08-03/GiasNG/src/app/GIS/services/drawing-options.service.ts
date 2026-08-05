import { Injectable } from "@angular/core";
import { enum_LayerElementiGraficiStd } from "../GIS-enum/GIS-layer-elementi-grafici";
import { LayerStyleService as LayerStyleService } from "./layer-style.service";

@Injectable()
export class DrawingOptionsService {

    constructor(
        private layerStyleService: LayerStyleService
    ) {  }

    public getPolygonDrawingOptions(layer: string): google.maps.PolygonOptions {
        let options: google.maps.PolygonOptions = {};

        options = this.layerStyleService.getPolygonLayerStyle(layer);
        options.zIndex = this.getLayerZIndex(layer);

        options.draggable = true;
        options.editable = true;

        return options;
    }

    public getPolylineDrawingOptions(layer: string): google.maps.PolylineOptions {
        let options: google.maps.PolylineOptions = {};

        options = this.layerStyleService.getPolylineLayerStyle(layer);
        options.zIndex = this.getLayerZIndex(layer);

        return options;
    }

    public getMarkerDrawingOptions(layer: string): google.maps.MarkerOptions {
        let options: google.maps.MarkerOptions = {};

        options = this.layerStyleService.getMarkerLayerStyle(layer);
        options.zIndex = this.getLayerZIndex(layer);

        return options;
    }

    private getLayerZIndex(layer: string): number {
        switch(layer) {
            case (enum_LayerElementiGraficiStd.APPEZZAMENTI) : {
                return 1;
            }
            case (enum_LayerElementiGraficiStd.IMPIANTI) : {
                return 2;
            }
            default: {
                return 0;
            }
        }
    }

}

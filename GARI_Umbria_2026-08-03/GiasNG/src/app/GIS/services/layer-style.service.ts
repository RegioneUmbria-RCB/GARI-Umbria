import { Injectable } from "@angular/core";
import { SharedDataService } from './shared-data.service';
import {enum_LayerElementiGraficiStd} from '../GIS-enum/GIS-layer-elementi-grafici';
import {LayerService} from './layer.service';

@Injectable()
export class LayerStyleService {

    constructor(
        private sharedDataService: SharedDataService,
        private layerService: LayerService
    ) {  }

    public getPolygonLayerStyle(layer: string, isRaster: boolean = false): google.maps.PolygonOptions {
        let options: google.maps.PolygonOptions = {};
        let layerStyle: any = this.getLayerColorAndOpacity(layer);
        let layerPresente: boolean = this.sharedDataService.getLayerPresente(layer);

        options.strokeColor = layerStyle.strokeColor;
        options.fillColor = layerStyle.fillColor;
        options.fillOpacity = layerStyle.fillOpacity;
        options.visible = layer == enum_LayerElementiGraficiStd.DEEFAULT ? true : layerPresente && this.layerService.isLayerVisible(layer);
        options.draggable = false;
        options.editable = false;

        if (isRaster) {
            // If layer is raster, its features are visible if google grid is visible
            options.visible = this.sharedDataService.getCfgAlberoGisUtente()[0].CfgGisUtente.ckGrigliaTiles_Sviluppo;
        }

        return options;
    }

    public getPolylineLayerStyle(layer: string): google.maps.PolylineOptions {
        let options: google.maps.PolylineOptions = {};
        let layerStyle: any = this.getLayerColorAndOpacity(layer);
        let layerPresente = this.sharedDataService.getLayerPresente(layer);

        options.strokeColor = layerStyle.strokeColor;
        options.strokeWeight = layerStyle.fillOpacity * 5;
        options.visible = layerPresente;
        options.draggable = true;
        options.editable = true;

        return options;
    }

    public getMarkerLayerStyle(layer: string): google.maps.MarkerOptions {
        let options: google.maps.MarkerOptions = {};

        let layerStyle: any = this.getLayerColorAndOpacity(layer);
        let layerPresente = this.sharedDataService.getLayerPresente(layer);

        options.title = 'Title';

        options.clickable = true;
        options.draggable = true;
        options.visible = layerPresente;
        options.icon = {
            path: google.maps.SymbolPath.CIRCLE,
            fillColor: layerStyle.fillColor,
            fillOpacity: layerStyle.fillOpacity,
            strokeColor: layerStyle.strokeColor,
            scale: 7,
            strokeWeight: 0.5
        };

        return options;
    }

    public getLayerColorAndOpacity(layer: string): any {
        let layerStyle: any = {};
        let coloreLayer = this.sharedDataService.getColoreLayerRGB(layer);
        layerStyle.fillColor = coloreLayer;
        layerStyle.fillOpacity = this.sharedDataService.getTrasparenzaLayer(layer);
        layerStyle.strokeColor = coloreLayer;
        return layerStyle;
    }

}

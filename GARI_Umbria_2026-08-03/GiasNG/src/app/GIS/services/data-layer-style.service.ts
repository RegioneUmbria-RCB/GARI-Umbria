import {LayerStyleService} from './layer-style.service';
import {Injectable} from '@angular/core';
import {enum_FeatureGeometryType} from '../GIS-enum/GIS-feature';
import {RetinaturaService} from './retinatura.service';
import {SharedDataService} from './shared-data.service';
import {FeatureType } from '../../Model/GIS/GisDataReadRval_New';
import { DatiColoreTema, TemaSelezionato } from '../GIS-kendo-window/theme-window/theme-window.component';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp, TipologiaLayer } from 'app/Service/api.service';

class Simbolo implements google.maps.Symbol {
    path: string | google.maps.SymbolPath;
    scale?: number;
    fillColor?: string;
    fillOpacity?: number;
    strokeWeight?: number;
    strokeColor?: string;

    constructor() {
        this.scale = 6;
        this.strokeWeight = 1;
    }
}

@Injectable()
export class DataLayerStyleService {

    private simboloIconaPuntoDeselezionato = new Simbolo();

    private simboloIconaPuntoSelezionato = new Simbolo();

    constructor(
        private layerStyleService: LayerStyleService,
        private retinaturaService: RetinaturaService,
        private sharedDataService: SharedDataService
    ) {  }

    public setIconaPuntoDeselezionato() {
        this.simboloIconaPuntoDeselezionato.path = google.maps.SymbolPath.CIRCLE;
    }

    public setIconaPuntoSelezionato() {
        this.simboloIconaPuntoSelezionato.path = google.maps.SymbolPath.CIRCLE;
        // this.simboloIconaPuntoSelezionato.fillColor = "#000000";
        this.simboloIconaPuntoSelezionato.fillOpacity = 0.5
    }

    public getIconaPuntoSelezionato(): Simbolo {
        // Ritorna il riferimento all'oggetto
        return Object.assign({}, this.simboloIconaPuntoSelezionato);
    }

    public getIconaPuntoDeselezionato(): Simbolo {
        // Ritorna una copia dell'oggetto
        return Object.assign({}, this.simboloIconaPuntoDeselezionato);
    }

    public getDataLayerStyle(
        feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp,
        coloreTema: string
    ): google.maps.Data.StyleOptions {
        let styleOpt: google.maps.Data.StyleOptions;
        if(feature.properties.InOsservazione == 1) {
            styleOpt = this.handleInOsservazione(feature, coloreTema);
        } else {
            styleOpt = this.handleDefault(feature, coloreTema);
        }

        // SALVATORE ZAMMATARO 05-06-2023: setto lo strokecolor a 0 nel caso si tratti di layer raster
        const isGoogleGridVisible = this.sharedDataService.getCfgAlberoGisUtente()[0]?.CfgGisUtente?.ckGrigliaTiles_Sviluppo ?? false;
        if(
            this.sharedDataService.selezionatoTipoLayerEntita() &&
            this.sharedDataService.getTipologiaLayerById(feature.properties.layer)?.FeatureTypeId === FeatureType.Raster.toString()
            && !isGoogleGridVisible
        )
            styleOpt.strokeOpacity = 0;

        return styleOpt;
    }

    public getFeatureStyle(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): google.maps.Data.StyleOptions {
        const temaSelezionato: TemaSelezionato = this.sharedDataService.getTemaSelezionato();
        let scalaColori = [];
        if (temaSelezionato?.applicaTema) {
            scalaColori = this.sharedDataService.getScalaColoriTema();
        }

        return this.getDataLayerStyleWithScalaColori(feature, temaSelezionato, scalaColori);
    }

    private getDataLayerStyleWithScalaColori(f: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, temaSelezionato: TemaSelezionato, scalaColori: DatiColoreTema[]): google.maps.Data.StyleOptions {
        let coloreTema = null;
        if (temaSelezionato?.applicaTema && f.properties.layer === temaSelezionato.layerId) {
            const layerSelezionato: TipologiaLayer = this.sharedDataService.getLayerSelezionato();
            coloreTema = this.sharedDataService.getColoreTemaFeature(layerSelezionato.nome, temaSelezionato.nome, f, scalaColori);
        }

        return this.getDataLayerStyle(f, coloreTema);
    }

    private handleDefault(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, coloreTema: string): google.maps.Data.StyleOptions {
        const isRaster = this.sharedDataService.getTipologiaLayerById(feature.properties.layer)?.FeatureTypeId === FeatureType.Raster.toString()
        const options: google.maps.Data.StyleOptions = this.layerStyleService.getPolygonLayerStyle(feature.properties.layer, isRaster);

        if (coloreTema) {
            options.fillColor = coloreTema;
            options.strokeColor = coloreTema;
        }

        let icona: Simbolo = new Simbolo();
        if (feature.geometry.type == enum_FeatureGeometryType.Point ) {
            icona = this.getIconaPuntoDeselezionato();
            icona.strokeColor = options.strokeColor;
        }

        options.clickable = true;
        options.icon = Object.assign({},icona);
        options.zIndex = +feature.properties.zindex;

        if (feature.geometry.type == enum_FeatureGeometryType.LineString) {
            options.strokeWeight = 5;
        }

        return options;
    }

    private handleInOsservazione(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, coloreTema: string): google.maps.Data.StyleOptions {
        let options = this.layerStyleService.getPolygonLayerStyle(feature.properties.layer);

        this.retinaturaService.generateRetinaturaOverLay(feature, options.visible);

        options.editable = false;
        options.clickable = true;
        options.draggable = false;
        options.fillColor = (coloreTema && coloreTema != '') ? coloreTema : '#' + feature.properties.Colore_Retinatura;
        options.fillOpacity = feature.properties.Trasparenza / 100;
        options.strokeColor = (coloreTema && coloreTema != '') ? coloreTema : '#' + feature.properties.Colore_Primario;
        options.zIndex = +feature.properties.zindex;

        return options;
    }

}

import { Injectable } from "@angular/core";
import { GoogleMapService } from "app/GIS/google-map/google-map.service";
import { DataLayerStyleService } from "app/GIS/services/data-layer-style.service";
import { FeatureInformationService } from "app/GIS/services/feature-information.service";
import { FeatureService } from "app/GIS/services/feature.service";
import { LayerStyleService } from "app/GIS/services/layer-style.service";
import { LayerService } from "app/GIS/services/layer.service";
import { SharedDataService } from "app/GIS/services/shared-data.service";
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp, TipologiaLayer } from 'app/Service/api.service';

@Injectable()
export class ThemeWindowService {
  layers: TipologiaLayer[] = [];

  constructor(
    private sharedDataService: SharedDataService,
    private googleMapService: GoogleMapService,
    private layerService: LayerService,
    private layerStyleService: LayerStyleService,
    private dataLayerStyleService: DataLayerStyleService,
    private featureService: FeatureService,
    private featureInformationService: FeatureInformationService
  ) {
    this.layerService.ObservableLayer.subscribe(layers => this.layers = layers);
  }

  public getIcon(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, icon: any = null): any {
    const layerId = feature.properties.layer
    const layer = this.layers.find(x => x.id == layerId)
    const selectedTheme = this.sharedDataService.getTemaSelezionato();
    const colorScale = this.sharedDataService.getScalaColoriTema();
    const layerStyle = this.layerStyleService.getPolygonLayerStyle(layerId);
    const selected = this.featureService.getFeatureSelezionate().find(x => x.properties.id == feature.properties.id) != null;

    let themeColor = null;
    if (selectedTheme?.applicaTema && layerId === selectedTheme?.layerId) {
      themeColor = this.sharedDataService.getColoreScalaFeature(feature, layer?.nome, selectedTheme?.nome, colorScale);
    }

    icon ??= this.dataLayerStyleService.getDataLayerStyle(feature, themeColor).icon;
    icon.fillColor = themeColor ?? layerStyle.fillColor;
    icon.strokeColor = themeColor ?? layerStyle.strokeColor;
    icon.fillOpacity = selected ? 1 : 0.3;
    icon.scale = this.getScale(feature);
    icon.strokeWeight = this.getStrokeWeight(feature);

    return icon;
  }

  private getScale(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): number {
    return this.computeScaleFactor(feature) * this.googleMapService.googleMapWrapper.getZoom();
  }

  private getStrokeWeight(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): number {
    return this.computeScaleFactor(feature) * this.googleMapService.googleMapWrapper.getZoom() * 0.2;
  }

  private computeScaleFactor(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): number {
    const layer = this.sharedDataService.getLayerSelezionato();
    const theme = this.sharedDataService.getTemaSelezionato();
    if (layer == null || theme?.id == null || layer.id != feature.properties.layer) {
      return 1;
    }

    const features = this.featureInformationService.getByLayer(layer.id);
    const limits = this.sharedDataService.getLimitiTematizzazioneLayer(features, layer, theme.nome);
    if (limits.valoreMin == limits.valoreMax) {
      return 1;
    }

    // Riscalo il punto per un fattore da 1 a 2 in base al tema selezionato
    const current = this.sharedDataService.getFeatureScaleByLayerAndTheme(feature, layer, theme.nome);
    return ((current - limits.valoreMin) / (limits.valoreMax - limits.valoreMin)) + 1;
  }
}

import { Injectable } from '@angular/core';
import { EditFeatureService } from './edit-feature.service';
import { enum_FeatureGeometryType, enum_FeatureProperty } from '../GIS-enum/GIS-feature';
import { TranslocoService } from '@jsverse/transloco';
import { FeatureInformationService } from './feature-information.service';
import {GisFeaturesUtils} from '../utils/gis-features.utils';
import {PermessiLayer, SharedDataService} from './shared-data.service';
import {TipologiaLayer} from '../../Service/api.service';
import {enum_TipologiaLayer} from '../GIS-enum/GIS-tipologia-layer';
import { LayerService } from './layer.service';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from 'app/Service/net-core6-api.service';

@Injectable()
export class EditFeatureWindowService {
  private features = new Map<string, google.maps.Data.LinearRing[]>();
  private geometries = new Map<string, google.maps.Data.Geometry>();
  private windows = new Map<string, google.maps.InfoWindow>();

  constructor(
    private editFeatureService: EditFeatureService,
    private translocoService: TranslocoService,
    private featureInformationService: FeatureInformationService,
    private sharedDataService: SharedDataService,
    private layerService: LayerService
  ) { }

  public load(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): void {
    const geometry = this.featureInformationService.getGeometry(feature.properties.id);
    if (geometry.getType() != enum_FeatureGeometryType.Polygon) {
      return;
    }

    // Vanni,02/01/2024, al momento i poligoni con innerBound non sono modificabili
    if (GisFeaturesUtils.isGeoJsonFeaturePolygonWithInnerBounds(feature) ) {
      return;
    }

    const path = [...(geometry as google.maps.Data.Polygon).getArray()];
    this.features.set(feature.properties.id, path);
    this.geometries.set(feature.properties.id, geometry);
  }

  public reset() {
    this.features.clear();
    this.closeAll();
    this.windows.clear();
  }

  public update(feature: google.maps.Data.Feature, gMap: google.maps.Map, isSementieri: boolean): void {
    const id = feature.getId().toString();
    if (feature.getGeometry().getType() != enum_FeatureGeometryType.Polygon) {
      return;
    }
    // Vanni,02/01/2024, al momento i poligoni con innerBound non sono modificabili
    if (GisFeaturesUtils.isGoogleMapsFeaturePolygonWithInnerBounds(feature)) {
      return;
    }

    this.closeAll();

    // la modifica è ammessa solo su tipologia standard.
    // mengarda 23/01/2026: modifica ammessa anche su 'imprese semtentiere'
    const tipologiaLayer = this.layerService.LayerSelected.getValue().Option_Value;
    if (tipologiaLayer != enum_TipologiaLayer.Entita && !(isSementieri && tipologiaLayer == enum_TipologiaLayer.OrganizzazioneAppartenenza)) {
      return;
    }

    //se il layer non è modificabile non mostro la finestra di edit
    const layerSelezionato: TipologiaLayer = this.sharedDataService.getTipologiaLayerById(feature.getProperty('StandardEntita_layerDiAppartenenza') as string);
    const permessiLayer: PermessiLayer = this.sharedDataService.getPermessiLayer(layerSelezionato);
    if (!permessiLayer.modifica) {
      return;
    }

    // se il poligono non è modificabile non mostro la fintesta di edit
    const vModifyPermission = (feature.getProperty(enum_FeatureProperty.modifica) as string).toLowerCase();
    if (vModifyPermission != 'true') {
      return;
    }

    const position = (feature.getGeometry() as google.maps.Data.Polygon).getArray()[0].getAt(0);
    const oldWindow = this.windows.get(id);
    if (oldWindow != null) {
      oldWindow.setPosition(position);
      oldWindow.open({ anchor: null, map: gMap, shouldFocus: false });
      return;
    }

    const contentString =
      `<div class="maps-polygon-label-infowindow edit-feature">` +
      `  <div>${this.translocoService.translate('Modifica')}</div>` +
      `  <input type="button" id="save-${id}" value="${this.translocoService.translate('Modifica')}" />` +
      `  <input type="button" id="undo-${id}" value="${this.translocoService.translate('Annulla')}" />` +
      `</div>`;

    const opts: google.maps.InfoWindowOptions = {
      position: position,
      content: contentString
    };

    const infoWindow = new google.maps.InfoWindow(opts);
    this.windows.set(id, infoWindow);
    infoWindow.open({ anchor: null, map: gMap, shouldFocus: false });

    google.maps.event.addListener(infoWindow, 'domready', () => {
      document.getElementById(`save-${id}`).onclick = () => {
        infoWindow.close();
        this.windows.delete(id);
        this.editFeatureService.editThisFeature(feature, isSementieri);
      };

      document.getElementById(`undo-${id}`).onclick = () => {
        infoWindow.close();
        this.windows.delete(id);
        feature.setGeometry(this.geometries.get(id));
      };
    });
  }

  public closeAll(): void {
    for (const infoWindow of this.windows.values()) {
      infoWindow.close();
    }
  }
}

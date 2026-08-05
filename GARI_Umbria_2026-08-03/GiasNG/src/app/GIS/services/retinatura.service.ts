import { Injectable } from '@angular/core';
import { GoogleMapService } from '../google-map/google-map.service';
import { enum_FeatureProperty } from '../GIS-enum/GIS-feature';
import { GoogleMapDataService } from './google.maps-services/google-map-data.service';
import { FeatureInformationService } from './feature-information.service';
import { GoogleMapFeatureService } from './google.maps-services/google-map-feature.service';
import { GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from 'app/Service/api.service';

@Injectable()
export class RetinaturaService {
    constructor(
        private googleMapService: GoogleMapService,
        private googleMapDataService: GoogleMapDataService,
        private featureInformationService: FeatureInformationService,
        private googleMapFeatureService: GoogleMapFeatureService
    ) {
        this.googleMapFeatureService.removingFeatures$.subscribe(ids => {
            for (const id of ids) {
                const feature = this.featureInformationService.getById(id);
                if (id == null) {
                    return;
                }

                this.hideRetinaturaOverlay(feature);
            }
        });

        this.googleMapDataService.overrideFeatureStyleEvent.subscribe(fs => {
            if (fs.style.visible != undefined) {
                const feature = this.featureInformationService.getById(fs.f.getId().toString());
                fs.style.visible ? this.showRetinaturaOverlay(feature) : this.hideRetinaturaOverlay(feature);
            }
        })
    }

    public generateRetinaturaOverLay(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, visible: boolean): void {
        if (visible && !feature.properties[enum_FeatureProperty.retinatura]) {
            console.log('Creazione retinatura')
            const Mappa = require('../../GiasJSLibraries/GIS-js-libraries/Mappa');

            let latLngArray = this.featureInformationService.getPath(feature.properties.id);
            let path: google.maps.MVCArray<google.maps.LatLng> = new google.maps.MVCArray<google.maps.LatLng>(latLngArray);

            let polyLineFill = new Mappa.PolyLineFill({
                poly: path,
                fill: '#' + feature.properties.Colore_Retinatura,
                stroke: '#' + feature.properties.Colore_Primario,
                opacity: feature.properties.Trasparenza / 100,
                zIndex: feature.properties.zindex
            });

            polyLineFill.setMap(this.googleMapService.googleMapWrapper.googleMap);

            feature.properties[enum_FeatureProperty.retinatura] = polyLineFill;
            polyLineFill = null;
        }
    }

    public hideRetinaturaOverlay(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): void {
        (feature.properties[enum_FeatureProperty.retinatura])?.setMap(null);
    }

    public showRetinaturaOverlay(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): void {
        feature.properties[enum_FeatureProperty.retinatura]?.setMap(this.googleMapService.googleMapWrapper.googleMap);
    }
}

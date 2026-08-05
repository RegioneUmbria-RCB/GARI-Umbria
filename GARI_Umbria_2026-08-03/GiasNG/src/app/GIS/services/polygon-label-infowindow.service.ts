import {GoogleMapService} from '../google-map/google-map.service';
import {SharedDataService} from './shared-data.service';
import {Injectable} from '@angular/core';
import {GiasInfoWindow} from '../infowindow-clusterer/models/gias-infowindow';
import {InfowindowClustererService} from '../infowindow-clusterer/infowindow-clusterer.service';
import {LayerService} from './layer.service';
import {Observable, of, Subscription, switchMap} from 'rxjs';
import { FeatureInformationService } from './feature-information.service';
import { AttributoLayer, GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from 'app/Service/api.service';
import { map } from 'rxjs';
import { take } from 'rxjs';
import { tap } from 'rxjs';

export enum enum_InfoWindowCustomProperties {
    title = 'title'
}

@Injectable()
export class PolygonLabelInfowindowService {

    private subs: Array<Subscription> = new Array<Subscription>();
    private get googleMapWrapper() {
        return this.googleMapService.googleMapWrapper;
    }

    public get polyLabelsClusterer() {
        return this.infoWindowClustererService.polyLabelsClusterer;
    }

    constructor(
        private googleMapService: GoogleMapService,
        private sharedDataService: SharedDataService,
        private infoWindowClustererService: InfowindowClustererService,
        private layerService: LayerService,
        private featureInformationService: FeatureInformationService
    ) {
        this.subs.push(this.layerService.ShowLabels.subscribe(sl => {
            // mostra o nascondi tutte la labels
            // console.log('mostra o nascondi tutte le labels');
            this.changeVisibleAllLayer(sl);
        }));

        this.subs.push(this.layerService.AllLayerGrouping.subscribe(ag => {
            // raggruppa o meno tutte la labels
            // console.log('raggruppa o meno tutte le labels');
            this.changeClusterAllLayer(ag);
        }));

        this.subs.push(this.layerService.AllLayersVisible.subscribe(av => {
            // mostra o nascondi tutti i layer
            // console.log('mostra o nascondi tutti i layer');
            this.changeVisibleAllLayer(av);
        }));

        this.subs.push(this.layerService.LayerItemLabelVisible.subscribe(lv => {
           // mostra o nascondi label del singolo layer;
           // console.log('mostra o nascondi label del singolo layer');
           this.changeVisibleByLayer(lv[1], lv[0]?.id);
        }));

        this.subs.push(this.layerService.LayerItemGrouping.subscribe(lg => {
            // raggruppa o meno label del singolo layer;
            // console.log('raggruppa o meno label del singolo layer');
            this.changeClusterByLayer(lg[1], lg[0]?.id);
        }));

        this.subs.push(this.layerService.LayerItemVisible.subscribe(lv => {
            // mostra o nascondi singolo layer;
            // console.log('mostra o nascondi singolo layer');
            this.changeVisibleByLayer(lv[1], lv[0]?.id);
        }));
    }

    private getEtichettaVisibileByLayer(layer: string): boolean {
        const mostraDescrizioneAssociataLayer: boolean = this.sharedDataService.getMostraDescrizioneAssociata(layer);
        const flagVisibileLayer: boolean = this.sharedDataService.getFlagVisibile(layer);
        return (mostraDescrizioneAssociataLayer && flagVisibileLayer);
    }

    private GestioneEtichettaEstraiTesto(layerId: number, etichetta: string, tipoEtichetta: number = 1): Observable<string> {
        if (etichetta == null || etichetta == '') {
            return of('');
        }

        const vEti = etichetta.split("|");
        const vRes = [];

        return this.layerService.getLayerAttributesById(layerId).pipe(
            map((attributes: AttributoLayer[]) => {
                for (let i = 0; i < vEti.length; i++) {
                    const etichettaParts = vEti[i].split("§");
                    const attribute = attributes.find(attr => attr.NomeAttributo === etichettaParts[0].trim());
                    if (attribute != null && attribute.EtichettaVisibile) {
                        vRes.push("§ " + etichettaParts[1]);
                    }
                }

                if (vRes.length == 0) {
                    return '';
                }

                etichetta = vRes.join("|");

                if (tipoEtichetta === 1) {
                    etichetta = this.decodeHtmlText(etichetta.replace(/§/g, "").replace(/\|/g, "<br/>"));
                } else {
                    etichetta = etichetta.replace(/§/g, "");
                }

                return this.formatLabel(etichetta.trim());
            }));
    }

    private decodeHtmlText(value: any): string {
        let htmlString: string = `<div><p>${value}</p></div>`;

        const temp = document.createElement("div");
        temp.innerHTML = htmlString;
        return temp.textContent || temp.innerText || '';
    }

    private formatLabel(etichetta: string): string {
        if (etichetta.length > 10) {
            let newEtichetta: string = '';
            let splitArray: string[] = etichetta.split(' ').sort(() => -1);
            let placeOnTheSameLine: boolean = false;
            newEtichetta = newEtichetta.concat(splitArray.pop());
            while (splitArray.length > 0) {
                let piece: string = splitArray.pop();
                if (piece.length <= 4 && !placeOnTheSameLine) {
                    newEtichetta = newEtichetta.concat(`<br/> ${piece}`);
                    placeOnTheSameLine = true;
                } else {
                    if (piece.includes('<br/>') || newEtichetta.trim() == '' || placeOnTheSameLine) {
                        newEtichetta = newEtichetta.trim().concat(` ${piece.trim()}`);
                        placeOnTheSameLine = false;
                    } else {
                        newEtichetta = newEtichetta.concat(`<br/> ${piece}`);
                        placeOnTheSameLine = true;
                    }
                }
            }
            return newEtichetta;
        }
        return etichetta;
    }

    public createPolyLabelAssociatedInfoWindow(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): void {
        const Mappa = require('../../GiasJSLibraries/GIS-js-libraries/Mappa');
        const path = this.featureInformationService.getPath(feature.properties.id);
        const position: google.maps.LatLng = Mappa.polylabel(path);
        const layer = feature.properties.layer;
        const etichettaVisibile: boolean = this.getEtichettaVisibileByLayer(layer);
        const raggruppaDescrzioneAssociata: boolean = this.sharedDataService.getRaggruppaDescrizioneAssociata(layer);

        let etichettaTestoObs: Observable<string>;
        if (feature.properties.etichetta == '' || feature.properties.etichetta == undefined) {
            etichettaTestoObs = this.GestioneEtichettaEstraiTesto(+feature.properties.layer, feature.properties.Testo);
        } else {
            etichettaTestoObs = of(this.formatLabel(feature.properties.etichetta));
        }

        etichettaTestoObs
            .pipe(take(1))
            .subscribe(etichettaTesto => {
                if (etichettaTesto && etichettaTesto != '' && etichettaTesto.trim() != '') {
                    const opts: google.maps.InfoWindowOptions = {
                        ariaLabel: 'maps-polygon-label',
                        zIndex: google.maps.Marker.MAX_ZINDEX,
                        position: position,
                        content: '<div class="maps-polygon-label-infowindow">' + etichettaTesto + '</div>',
                        disableAutoPan: true
                    };

                    const pl: GiasInfoWindow = new GiasInfoWindow(
                        opts,
                        this.googleMapWrapper.googleMap,
                        feature.properties.layer
                    );
                    pl.setInfoWindowTitle(feature.properties.id);
                    pl.visible = etichettaVisibile;
                    pl.clusterable = raggruppaDescrzioneAssociata;

                    this.infoWindowClustererService.addInfowindow(pl);
                }
            });
    }

    public deletePolylabel(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): void {
        this.infoWindowClustererService.removePolyLabelAssociatedWithFeature(feature);
    }

    public catchPolygonChange(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): void {
        let path = this.featureInformationService.getPath(feature.properties.id);

        const Mappa = require('../../GiasJSLibraries/GIS-js-libraries/Mappa');
        let position: google.maps.LatLng = Mappa.polylabel(path);

        this.infoWindowClustererService.changePositionOfPolyLabel(feature.properties.id, position);
    }

    public resetPolyLabels(): void {
        this.infoWindowClustererService.resetPolyLabels();
    }

    public recalculateClusterer(): void {
        this.infoWindowClustererService.recompute();
    }

    public changeClusterByLayer(toCluster: boolean, layer: string): void {
        this.infoWindowClustererService.changeClusterByLayer(toCluster, layer);
    }

    public changeVisibleByLayer(visible: boolean, layer: string): void {
        this.infoWindowClustererService.changeVisibleByLayer(visible, layer);
    }

    public changeClusterAllLayer(toCluster: boolean): void {
        this.infoWindowClustererService.changeClusterAllLayer(toCluster);
    }

    public changeVisibleAllLayer(visible: boolean): void {
        this.infoWindowClustererService.changeVisibleAllLayer(visible);
    }
}

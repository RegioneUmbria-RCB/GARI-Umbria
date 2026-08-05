import { Injectable } from '@angular/core';
import { GoogleMapService } from '../google-map/google-map.service';
import { SharedDataService } from './shared-data.service';
import Feature = google.maps.Data.Feature;
import { enum_FeatureProperty } from '../GIS-enum/GIS-feature';
import { FeatureInformationService } from './feature-information.service';
import { AttributoLayer, GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from 'app/Service/api.service';
import { Observable, of, switchMap, take } from 'rxjs';
import { LayerService } from './layer.service';
import { map } from 'rxjs';

@Injectable()
export class PolygonLabelService {

    private get googleMapWrapper() {
        return this.googleMapService.googleMapWrapper;
    }

    private polylabels: google.maps.Marker[] = new Array<google.maps.Marker>();
    public get PolyLabels() {
        return this.polylabels;
    }

    public get PolyLabelsVisible() {
        return this.polylabels.filter(pl => this.getEtichettaVisibileByPolylabel(pl));
    }

    public get PolyLabelsClusterer() {
        return this.polylabels.filter(pl => this.getEtichettaVisibileByPolylabelClusterer(pl));
    }

    public PolyLabelsLayer(layer: string) {
        return this.polylabels.filter(pl => this.filterPolylabelsByLayer(layer, pl));
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

                return etichetta;
            }));
    }

    private decodeHtmlText(value: any): string {
        let htmlString: string = `<div><p>${value}</p></div>`;

        const temp = document.createElement("div");
        temp.innerHTML = htmlString;
        return temp.textContent || temp.innerText || '';
    }

    constructor(
        private googleMapService: GoogleMapService,
        private sharedDataService: SharedDataService,
        private featureInformationService: FeatureInformationService,
        private layerService: LayerService
    ) { }

    public createPolyLabelFromGoogleMapData(feature: Feature): void {
        const Mappa = require('../../GiasJSLibraries/GIS-js-libraries/Mappa');
        const path = this.featureInformationService.getPath(feature.getId().toString());
        const position: google.maps.LatLng = Mappa.polylabel(path);
        const layer = feature.getProperty(enum_FeatureProperty.layer) as string;
        const etichettaVisibile = this.getEtichettaVisibileByLayer(layer);

        let etichettaTestoObs: Observable<string>;
        if (feature.getProperty(enum_FeatureProperty.etichetta) == '' || feature.getProperty(enum_FeatureProperty.etichetta) == undefined) {
            etichettaTestoObs = this.GestioneEtichettaEstraiTesto(feature.getProperty(enum_FeatureProperty.layer) as number, feature.getProperty(enum_FeatureProperty.testo) as string);
        } else {
            etichettaTestoObs = of(feature.getProperty(enum_FeatureProperty.etichetta) as string);
        }

        // console.log(`Etichetta: ${etichettaTesto}; Layer: ${layer}; Visibile: ${etichettaVisibile}`);

        etichettaTestoObs.pipe(take(1)).subscribe(etichettaTesto => {
            if (etichettaTesto && etichettaTesto != '') {
                let opts: google.maps.MarkerOptions = {
                    label: {
                        text: etichettaTesto,
                        color: 'white',
                        className: 'maps-polygon-label',
                    },
                    clickable: false,
                    draggable: false,
                    zIndex: google.maps.Marker.MAX_ZINDEX,
                    map: this.googleMapWrapper.googleMap,
                    position: position,
                    title: feature.getId().toString(),
                    icon: 'https://www.google.com/mapfiles/arrowtransparent.png',
                    visible: false // etichettaVisibile
                };

                let m: google.maps.Marker = new google.maps.Marker(opts);
                this.polylabels.push(m);
            }
        });
    }

    public catchPolygonChange(feature: Feature): void {
        let path = this.featureInformationService.getPath(feature.getId().toString());

        const Mappa = require('../../GiasJSLibraries/GIS-js-libraries/Mappa');
        let position: google.maps.LatLng = Mappa.polylabel(path);

        let marker: google.maps.Marker = this.polylabels.find(pl => pl.getTitle() == feature.getId());
        marker?.setPosition(position);
    }

    public resetPolyLabels(): void {
        this.polylabels.forEach(m => {
            m.setMap(null);
            m = null;
        });
        this.polylabels = [];
    }

    public setMapPolyLabels(): void {
        this.polylabels.forEach(m => {
            m.setMap(this.googleMapWrapper.googleMap,);
        });
    }

    public renderPolyLabels(flagVisible: boolean, layer?: string): void {
        if (this.googleMapWrapper.googleMap) {
            this.checkVisibleMarkerLabel(flagVisible, layer);
        }
    }

    public getEtichettaVisibileByPolylabel(pl: google.maps.Marker): boolean {
        const feature = this.getFeatureFromPolylabel(pl);
        const layer = feature.properties.layer;
        return this.getEtichettaVisibileByLayer(layer);
    }

    public getEtichettaVisibileByPolylabelClusterer(pl: google.maps.Marker): boolean {
        let etichettaVisibile: boolean = false;
        const feature = this.getFeatureFromPolylabel(pl);
        if (feature) {
            const layer = feature.properties.layer;
            const raggruppaDescrizioneAssociataLayer: boolean = this.sharedDataService.getRaggruppaDescrizioneAssociata(layer);
            etichettaVisibile = (raggruppaDescrizioneAssociataLayer && this.getEtichettaVisibileByLayer(layer));
        }
        return etichettaVisibile;
    }

    public getEtichettaVisibileByLayer(layer: string): boolean {
        const mostraDescrizioneAssociataLayer: boolean = this.sharedDataService.getMostraDescrizioneAssociata(layer);
        const flagVisibileLayer: boolean = this.sharedDataService.getFlagVisibile(layer);
        return (mostraDescrizioneAssociataLayer && flagVisibileLayer);
    }

    public overrideRenderLabel(feature: Feature, visible: boolean | null = null): void {
        const label: google.maps.Marker = this.polylabels.find(x => x.getTitle() === feature.getId());
        if (label != null) {
            visible ??= this.getEtichettaVisibileByPolylabelClusterer(label);
            visible ? this.showPolyLabel(label) : this.hidePolyLabel(label);
        }
    }

    private filterPolylabelsByLayer(layer: string, pl: google.maps.Marker): boolean {
        const feature = this.getFeatureFromPolylabel(pl);
        const featureLayer = feature.properties.layer;
        return layer === featureLayer;
    }

    private getFeatureFromPolylabel(pl: google.maps.Marker): GeoJson_Feature_New_1OfGeoJSONAgroGisProp {
        const idFeature = pl.getTitle();
        return this.featureInformationService.getById(idFeature);
    }

    private checkVisibleMarkerLabel(flagVisible: boolean, layer?: string): void {

        if (!flagVisible) { // || this.googleMapWrapper.googleMap.getZoom() < this.mappa_zoom_mostraEtichette) {

            if (layer === undefined) {
                this.polylabels.forEach(pl => {
                    this.hidePolyLabel(pl);
                });
            } else {
                this.PolyLabelsLayer(layer).forEach(pl => {
                    this.hidePolyLabel(pl);
                });
            }
            return;
        }

        const bounds: google.maps.LatLngBounds = this.googleMapWrapper.googleMap.getBounds();
        this.PolyLabelsVisible.forEach(pl => {
            if (bounds.contains(pl.getPosition()) && this.isLabelVisibleAccordingToSelectedTheme(pl)) {
                this.showPolyLabel(pl);
                return;
            }

            this.hidePolyLabel(pl);
        });

        // ------------------------------------ commenti per test ------------------------------------
        // let filtro = this.polylabels.filter(pl => pl.getVisible()==true);
        // console.log(`Zoom: ${ this.googleMapWrapper.googleMap.getZoom()}; EtichetteVisibili: ${filtro.length}; Layer: ${layer}`);

    }

    public hidePolyLabel(pl: google.maps.Marker): void {
        pl.setVisible(false);
        // this.polygonLabelInfoWindowService.hideLabel(pl.getTitle());
    }

    public showPolyLabel(pl: google.maps.Marker): void {
        //pl.setVisible(true);
        // this.polygonLabelInfoWindowService.showLabel(pl.getTitle());
    }

    private isLabelVisibleAccordingToSelectedTheme(pl: google.maps.Marker): boolean {
        // Controllo se ho un tema selezionato per cui l'etichetta non deve essere visibile
        const theme = this.sharedDataService.getTemaSelezionato()?.nome;
        const layer = this.sharedDataService.getLayerSelezionato()?.nome;
        if (theme == null || layer == null) {
            return true;
        }

        const scalaColori = this.sharedDataService.getScalaColoriTema();
        const feature = this.getFeatureFromPolylabel(pl);
        if (feature == null) {
            return true;
        }

        const nomeTema = this.sharedDataService.getNomeTemaTipoLayer(layer, theme);
        const valoreScalaFeature = this.sharedDataService.getValoreScalaFeature(feature, nomeTema);

        const elementoScala = scalaColori.find(elemento => elemento.valoreMax >= valoreScalaFeature && (elemento.valoreMin == null || elemento.valoreMin <= valoreScalaFeature));
        if (elementoScala == null) {
            return true;
        }

        return elementoScala.visible;
    }
}

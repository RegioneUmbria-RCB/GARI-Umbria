import { Injectable } from "@angular/core";
import { wmsCatasto, WmsUrlAE } from "app/Model/GIS/Wms";
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { SharedDataService } from "./shared-data.service";
import { enum_LayerElementiGraficiStd } from "../GIS-enum/GIS-layer-elementi-grafici";
import { GisClient } from "app/Service/api.service";
import { Subject, takeUntil } from "rxjs";
import { TranslocoService } from "@jsverse/transloco";
import { getServiceIdAndLog } from "app/Service/utils";
import { FixedLayerProperty } from "app/Model/GIS/FixedLayerProperty";

@Injectable()
export class WmsService {

    private get googleMapWrapper() {
        return this.googleMapService.googleMapWrapper;
    }

    private mappa = null;
    private customMapOverlayBase = null;
    private rainMapOverlayArray = null;
    private fixedLayerProperty = null;
    private signal = new Subject<void>();

    private serviceId = null;

    constructor(
        private googleMapService: GoogleMapService,
        private sharedDataService: SharedDataService,
        private gisClient: GisClient,
        private translocoService: TranslocoService
    ) {
        this.serviceId = getServiceIdAndLog('WmsService', 'constructor');
    }

    public gestioneFixedLayerProperty(fixedLayer: FixedLayerProperty) {
        if (this.sharedDataService.selezionatoTipoLayerEntita()) {
            const flagVisibile = this.sharedDataService.getFlagVisibile(enum_LayerElementiGraficiStd.WMS);
            if (flagVisibile) {
                this.getMappaAndFixedLayerProperty();
                this.overlayImpostaOpacity(this.fixedLayerProperty.getOpacitaPercentuale());
            }
        }
    }

    ngOnDestroy() {
        this.signal.next();
        this.signal.complete();
    }

    getMappaAndFixedLayerProperty() {
        this.mappa = this.googleMapWrapper?.data.getMap();
        this.fixedLayerProperty = this.sharedDataService.getFixedLayerProperty();
    }

    public wmsDdlSensoreElaborazioneChange(wmsVisibile: boolean) {
        this.getMappaAndFixedLayerProperty();
        this.wmsOverlayReset();
        this.overlayRielaboraWms(wmsVisibile); // Enum_TipoDiOverlay.wms
    }

    wmsOverlayReset() {

        this.customMapOverlayBase = new Array();
        this.rainMapOverlayArray = new Array();

        let opacityValue = this.fixedLayerProperty.getOpacitaPercentuale();

        let urlToInsert = "";

        this.customMapOverlayBase.push({
            Tile: wmsCatasto,
            SensoreElaborazione: wmsCatasto,
            DataRiferimento: "01/01/1900",
            Descrizione: wmsCatasto,
            url: urlToInsert,
            opacity: opacityValue
        });

        this.rainMapOverlayArray.push({ url: this.overlayWms(512), toggle: 0 });
    }

    private overlayRielaboraWms(wmsVisibile: boolean) {
        this.attivaSatelliteRipulisiciOverlays();

        if (wmsVisibile) {
            const opacity = this.fixedLayerProperty.getOpacitaPercentuale();
            this.attivaSatelliteNewSetOverlayInserisciPerDataSensore("01/01/1900", wmsCatasto, opacity);
        }
    }

    attivaSatelliteNewSetOverlayInserisciPerDataSensore(date: string, sensor: string, opacity: number): void {
        for (let i = 0; i < this.customMapOverlayBase.length; i++) {
            const cOver = this.customMapOverlayBase[i];
            if (cOver.SensoreElaborazione === sensor && cOver.DataRiferimento === date) {
                this.attivaSatelliteNewSetOverlayInserisciPerIndice(i, opacity);
            }
        }
    }

    private attivaSatelliteRipulisiciOverlays(): void {
        if (this.rainMapOverlayArray == null) {
            return;
        }

        for (let i = 0; i < this.rainMapOverlayArray.length; i++) {
            this.rainMapOverlayArray[i].toggle = 0;
            for (let j = 0; j < this.mappa.overlayMapTypes.getLength(); j++) {
                if (this.mappa.overlayMapTypes.getAt(j).constructor.name !== "CoordMapType_Tiles") {
                    this.mappa.overlayMapTypes.removeAt(j);
                }
            }
        }
    }

    private attivaSatelliteNewSetOverlayInserisciPerIndice(indice: number, opacity: number): void {
        //Overlays the rainfall map on top of the Google map
        this.mappa.overlayMapTypes.insertAt(0, this.rainMapOverlayArray[indice].url);

        if (opacity < 1) {
            this.overlayImpostaOpacity(opacity);
        }

        this.rainMapOverlayArray[indice].toggle = 1
    }

    overlayImpostaOpacity(opacity: number): void {
        const omap = this.mappa.overlayMapTypes.getAt(0);
        if (omap !== undefined) {
            omap.setOpacity(opacity);
        }
    }

    overlayWms(tileDimension: any) {
        return new google.maps.ImageMapType({

            getTileUrl: function (coord, zoom) {
                let projection = this.mappa.getProjection();

                var zfactor = Math.pow(2, zoom);

                // get Long Lat coordinates
                var top = projection.fromPointToLatLng(new google.maps.Point(coord.x * tileDimension / zfactor, coord.y * tileDimension / zfactor));
                var bot = projection.fromPointToLatLng(new google.maps.Point((coord.x + 1) * tileDimension / zfactor, (coord.y + 1) * tileDimension / zfactor));

                // corrections for the slight shift of the SLP (mapserver)
                var deltaX = 0.0;
                var deltaY = 0.0;

                var bbox = (bot.lat() + deltaX) + "," +
                    (top.lng() + deltaY) + "," +
                    (top.lat() + deltaX) + "," +
                    (bot.lng() + deltaY);

                // base WMS URL
                var url = WmsUrlAE(bbox);

                // console.log(url);

                return url; // return URL for the tile

            }.bind(this),
            tileSize: new google.maps.Size(tileDimension, tileDimension),
            minZoom: 17,
            maxZoom: 20
        });
    }

    //--------------------------------------------------------------------------------
    // Informazioni Su Click In Mappa
    //--------------------------------------------------------------------------------

    public seWmsInfoClickMappa(e: google.maps.Data.MouseEvent) {

        const fixedLayerProperty = this.sharedDataService.getFixedLayerProperty();
        if (fixedLayerProperty.InfoClickMappa) {
            this.mappa = this.googleMapWrapper.data.getMap();
            this.WmsGetDatiOggetto(e.latLng);
        }

    }

    // private WmsGetFeatureInfo(latLng: google.maps.LatLng) {
    //     const gMapsUtility = require('../../GiasJSLibraries/GIS-js-libraries/gMapsUtility');
    //     const infoGeoData = gMapsUtility.gMapsUtility.WMS_GetFeatureInfoGeoData(this.mappa, latLng);
    //     let url = "https://wms.cartografia.agenziaentrate.gov.it/inspire/wms/ows01.php?"
    //     url += "language=ita"
    //     url += "&SERVICE=WMS";
    //     url += "&VERSION=1.3.0";
    //     url += "&REQUEST=GetFeatureInfo";
    //     url += "&BBOX=" + infoGeoData.BBOX;
    //     url += "&CRS=EPSG:6706";
    //     url += "&WIDTH=" + infoGeoData.WIDTH;
    //     url += "&HEIGHT=" + infoGeoData.HEIGHT;
    //     url += "&LAYERS=CP.CadastralZoning,CP.CadastralParcel";
    //     url += "&STYLES=default";
    //     url += "&FORMAT=image/png";
    //     url += "&QUERY_LAYERS=CP.CadastralParcel";
    //     url += "&INFO_FORMAT=text/html";
    //     url += "&X=" + infoGeoData.X;
    //     url += "&Y=" + infoGeoData.Y;
    //     console.log(url);
    //     this.gisClient.gisWmsGetFeature(url)
    //         .pipe(takeUntil(this.signal)).subscribe(r => {
    //         this.WmsGetFeatureInfoPopup(r.RispostaStringa, latLng)
    //     });
    // }

    // private WmsGetFeatureInfoPopup(contenuto: string, latLng: google.maps.LatLng) {
    //     var infoWindow = new google.maps.InfoWindow({
    //         content: '<div class="custom-infobox">' + contenuto + '</div>',
    //         position: latLng
    //     });
    //     infoWindow.open(this.mappa);
    // }

    private WmsGetDatiOggetto(latLng: google.maps.LatLng) {

        const lat = latLng.lat();
        const lng = latLng.lng();

        let url = "https://wms.cartografia.agenziaentrate.gov.it/inspire/ajax/ajax.php?"
        url += `op=getDatiOggetto&lat=${lat}&lon=${lng}`;

        console.log(url);

        this.gisClient.gisWmsGetFeature(url)
            .pipe(takeUntil(this.signal)).subscribe(r => {
                this.WmsGetDatiOggettoInfoPopup(r.RispostaStringa, latLng)
            });

    }

    private WmsGetDatiOggettoInfoPopup(contenuto: string, latLng: google.maps.LatLng) {
        const contenutoFormattato = this.FormattaContenutoWms(contenuto);
        var infoWindow = new google.maps.InfoWindow({
            content: contenutoFormattato,
            position: latLng
        });
        infoWindow.open(this.mappa);
    }

    private FormattaContenutoWms(contenuto: string): string {
        let contenutoFormattato = {
            openTag: '<div class="custom-infobox">',
            content: '',
            closeTag: '</div>'
        }

        const objContenuto = this.ConvertiContenutoInOggetto(contenuto);

        this.SeAggiungiRigaContenutoWms(contenutoFormattato, objContenuto.SIGLA_PROV, 'Provincia');
        this.SeAggiungiRigaContenutoWms(contenutoFormattato, objContenuto.DENOM, 'Comune');
        this.SeAggiungiRigaContenutoWms(contenutoFormattato, objContenuto.SEZIONE, 'Sezione');
        this.SeAggiungiRigaContenutoWms(contenutoFormattato, objContenuto.FOGLIO, 'Foglio');
        this.SeAggiungiRigaContenutoWms(contenutoFormattato, objContenuto.NUM_PART, 'Particella');

        return contenutoFormattato.openTag +
            contenutoFormattato.content +
            contenutoFormattato.closeTag;
    }

    private ConvertiContenutoInOggetto(contenuto: string): any {
        return JSON.parse(contenuto);
    }

    private SeAggiungiRigaContenutoWms(
        contenutoFormattato: any,
        proprieta: string,
        codiceEtichetta: string
    ): void {
        if (proprieta === undefined || proprieta === null) {
            proprieta = '';
        }
        const etichetta = this.translocoService.translate(codiceEtichetta);
        contenutoFormattato.content += `<div>${etichetta}: ${proprieta}</div>`;
    }

}

import {Injectable} from '@angular/core';
import {ChiaveAlbero} from '../../Service/api.service';
import { ObjParametriAgendaService} from '../../Service/obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {enum_PagineGiasNG} from '../../Model/TipiEnumerativi';
import {GestioneRichiesteService, ParametriAggiuntivi_QueryString} from '../../Service/gestione-richieste.service';
import {Enum_SiteRedirector} from '../../Model/siti.enum';
import {GiasIFrameWindowService} from 'gias-ui-kit';
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {WKTService} from './wkt.service';
import {FeatureService} from './feature.service';
import {GoogleMapGeoJsonService} from '../google-map/google-map-geojson.service';
import {FunzioniComuniService} from 'app/Service/FunzioniComuni.service';
import {BehaviorSubject, Subject} from 'rxjs';
import {enum_FeatureProperty} from '../GIS-enum/GIS-feature';
import {enum_TipologiaLayer} from '../GIS-enum/GIS-tipologia-layer';
import {SharedDataService} from './shared-data.service';
import {GoogleMapService} from '../google-map/google-map.service';
import {enum_LayerElementiGraficiStd} from '../GIS-enum/GIS-layer-elementi-grafici';
import { GoogleMapFeatureService } from './google.maps-services/google-map-feature.service';

@Injectable()
export class EditFeatureService {

    objParametriAgenda: ObjParametriAgenda;
    public editFeature: Subject<google.maps.Data.Feature> = new Subject< google.maps.Data.Feature>();

    private area: number;
    private wkt: string;
    private ca: ChiaveAlbero;
    private modificaPerAssociazione: boolean = false;
    private readonly functionMessageEventListener: any;
    private layer: string;

    public chiusuraFinestraModifica = new BehaviorSubject<[boolean,MessageEvent<any>]>([undefined,undefined]);

    constructor(
        private gestioneRichiesteService: GestioneRichiesteService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private GiasIFrameWindowService: GiasIFrameWindowService,
        private wktService: WKTService,
        private featureService: FeatureService,
        private sharedDataService: SharedDataService,
        private googleMapService: GoogleMapService,
        private googleMapFeatureService: GoogleMapFeatureService
    ) {
        this.functionMessageEventListener = ((me) => this.messageEventHandler(me)).bind(this);
    }

    public modificaFeature(isSementieri: boolean) {
        let f = this.featureService.getUltimaFeatureSelezionata();
        const feature = this.googleMapFeatureService.getByid(f.properties.id);
        this.doEditFeature(feature, isSementieri);
    }

    public editThisFeature(feature: google.maps.Data.Feature, isSementieri: boolean) {
        this.doEditFeature(feature, isSementieri);
    }

    private doEditFeature(f: google.maps.Data.Feature, isSementieri: boolean) {
        if (this.featureConditions(f, isSementieri)) {
            this.modificaElementoAnagrafica(isSementieri);
        } else {
            this.editFeature.next(f);
        }
    }

    private featureConditions(f: google.maps.Data.Feature, isSementieri: boolean): boolean {
        let layer = f.getProperty(enum_FeatureProperty.layer) as string;
        if(layer == '-1') return false;
        let tipo = this.sharedDataService.getTipoLayerSelezionato();
        if (tipo == enum_TipologiaLayer.Cultivar || tipo == enum_TipologiaLayer.GruppoColturale) {
            return true;
        } else if (tipo == enum_TipologiaLayer.OrganizzazioneAppartenenza && isSementieri) {
            return true;
        } else {
            return layer == enum_LayerElementiGraficiStd.IMPIANTI || layer == enum_LayerElementiGraficiStd.APPEZZAMENTI || layer == enum_LayerElementiGraficiStd.CAMPI;
        }
    }

    private modificaElementoAnagrafica(isSementieri: boolean) {
        let f = this.featureService.getUltimaFeatureSelezionata();
        const feature = this.googleMapFeatureService.getByid(f.properties.id);

        if(this.isPolygonValid(feature)) {
            this.wkt = this.wktService.featureGeometryToWKT(feature);

            const area = Number.parseFloat((google.maps.geometry.spherical.computeArea(this.getCoordinateFeature(feature))).toFixed(7));
            this.area = GoogleMapGeoJsonService.calcolaAreaHa(area);

            let chiave = this.featureService.getChiaveAlberoCompletaByFeature(f);
            this.ca = FunzioniComuniService.scomponiChiaveAlbero(chiave);

            this.modificaPerAssociazione = false;
            this.modifica(isSementieri);
        }
    }

    private getCoordinateFeature(feature: google.maps.Data.Feature): google.maps.LatLng[] {
        let geometriaFeature = feature.getGeometry();
        let coordinateFeature: google.maps.LatLng[] = [];
        geometriaFeature.forEachLatLng(latlng => {
            coordinateFeature.push(latlng);
        });

        return coordinateFeature;

    }

    private isPolygonValid(f: google.maps.Data.Feature): boolean {
        const DrawingManager = require('../../GiasJSLibraries/GIS-js-libraries/DrawingManager');

        let ll: google.maps.LatLng[] = [];
        f.getGeometry().forEachLatLng(_ll => {
            ll.push(_ll);
        });
        let path: google.maps.MVCArray<google.maps.LatLng> = new google.maps.MVCArray<google.maps.LatLng>(ll)
        let polygonValidator = new DrawingManager.PolygonValidator(path);

        if(!polygonValidator.isValid)
            DrawingManager.DisplayErrorPolyLines(polygonValidator.intersection, this.googleMapService.googleMapWrapper.googleMap);
        return polygonValidator.isValid;
    }

    modificaElementoAnagraficaPerAssociazione(
        polygonWkt: string,
        area: number,
        chiaveAlbero: ChiaveAlbero,
        layer: string = undefined
    ) {
        this.wkt = polygonWkt;
        this.area = area;
        this.ca = chiaveAlbero;
        this.layer = layer;

        this.modificaPerAssociazione = true;
        this.modifica(false);
    }

    private modifica(isSementieri: boolean) {

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
        this.objParametriAgenda.Piva = this.ca.Piva;
        this.objParametriAgenda.Appezza = this.ca.Appezza;
        this.objParametriAgenda.Sa_Cod = this.ca.Sa_Cod;
        this.objParametriAgenda.Campo_Cod = this.ca.Campo_Cod;
        this.objParametriAgenda.Id_Reg = this.ca.Id_Imp;
        this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

        window.addEventListener('message', this.functionMessageEventListener);

        const newobjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgenda));
        newobjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_GIS;
        const parametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];

        const tipologiaLayerSelezionato = this.sharedDataService.getTipoLayerSelezionato();
        const feature = this.featureService.getUltimaFeatureSelezionata();

        let pagina;
        if(feature?.properties.layer == enum_LayerElementiGraficiStd.IMPIANTI ||
            feature?.properties.layer == enum_LayerElementiGraficiStd.APPEZZAMENTI ||
            this.layer == enum_LayerElementiGraficiStd.IMPIANTI ||
            this.layer == enum_LayerElementiGraficiStd.APPEZZAMENTI
        )
            pagina = enum_PagineGiasNG.Pagina_Edit_AppezzamentoGlobal;

        if (tipologiaLayerSelezionato == enum_TipologiaLayer.OrganizzazioneAppartenenza && isSementieri)
            pagina = enum_PagineGiasNG.Pagina_Edit_AppezzamentoGlobal;

        if(
            feature?.properties.layer == enum_LayerElementiGraficiStd.CAMPI ||
            this.layer == enum_LayerElementiGraficiStd.CAMPI
        )
            pagina = enum_PagineGiasNG.Pagina_Edit_Campo;

        this.gestioneRichiesteService.gestionePassaggioAltroSito(
            Enum_SiteRedirector.GiasNG,
            pagina,
            parametriAggiuntivi,
            newobjParametriAgenda
        ).then(resp => {
            resp += '?seFrame=1&wkt=' + this.wkt +
                '&area=' + this.area +
                '&cfgSementi=' + JSON.stringify(this.sharedDataService.getCfgSementiAsValue()) +
                '&entitaCod=' + feature?.properties.Entita_Cod;

            this.GiasIFrameWindowService.open({
                title: 'Modifica poligono',
                content: resp,
                height: window.innerHeight * 0.9,
                width: window.innerWidth * 0.9
            });
        });
    }

    private messageEventHandler(me: MessageEvent<any>): void {

        window.removeEventListener('message', this.functionMessageEventListener);

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
        this.objParametriAgenda.Appezza = 0;
        this.objParametriAgenda.Sa_Cod = 0;
        this.objParametriAgenda.Campo_Cod = 0;
        this.objParametriAgenda.Id_Reg = 0;
        this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

        this.chiusuraFinestraModifica.next([this.modificaPerAssociazione,me]);

    }

}

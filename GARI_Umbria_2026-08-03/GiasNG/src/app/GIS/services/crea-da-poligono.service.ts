import { Injectable } from '@angular/core';
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from '../../Service/gestione-richieste.service';
import { ObjParametriAgendaService } from '../../Service/obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { WKTService } from './wkt.service';
import { FeatureService } from './feature.service';
import { GoogleMapGeoJsonService } from '../google-map/google-map-geojson.service';
import { FunzioniComuniService } from '../../Service/FunzioniComuni.service';
import { ChiaveAlbero } from '../../Service/api.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { enum_PagineGiasNG } from '../../Model/TipiEnumerativi';
import { Enum_SiteRedirector } from '../../Model/siti.enum';
import { FeatureInformationService } from './feature-information.service';

@Injectable()
export class CreaDaPoligonoService {

    private objParametriAgenda: ObjParametriAgenda;

    private area: number;
    private wkt: string;
    private ca: ChiaveAlbero;
    private functionMessageEventListener: any;

    constructor(
        private gestioneRichiesteService: GestioneRichiesteService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private GiasIFrameWindowService: GiasIFrameWindowService,
        private wktService: WKTService,
        private featureService: FeatureService,
        private featureInformationService: FeatureInformationService
    ) { }

    public creaImpiantoDaPoligono() {
        const f = this.featureService.getUltimaFeatureSelezionata();
        const geometry = this.featureInformationService.getGeometry(f.properties.id);
        this.wkt = this.wktService.geometryToWKT(geometry);

        this.area = GoogleMapGeoJsonService.calcolaAreaHa(this.featureInformationService.getArea(f.properties.id));

        let chiave = this.featureService.getChiaveAlberoCompletaByFeature(f);
        this.ca = FunzioniComuniService.scomponiChiaveAlbero(chiave);

        this.crea();
    }

    private crea() {

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

        this.gestioneRichiesteService.gestionePassaggioAltroSito(
            Enum_SiteRedirector.GiasNG,
            enum_PagineGiasNG.Pagina_Edit_AppezzamentoGlobal,
            parametriAggiuntivi,
            newobjParametriAgenda
        ).then(resp => {
            resp += '?seFrame=1&wkt=' + this.wkt +
                '&area=' + this.area +
                '&createFrom=' + true;

            this.GiasIFrameWindowService.open({
                title: 'Crea Impianto',
                content: resp,
                height: window.innerHeight * 1.0,
                width: window.innerWidth * 0.9
            });
        });
    }
}

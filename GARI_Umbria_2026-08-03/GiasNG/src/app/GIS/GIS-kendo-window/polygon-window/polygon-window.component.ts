/* eslint @typescript-eslint/no-shadow: ["error", { "allow": ["Appezzamento", "Impianto"] }] */

import { DrawingManagerService } from 'app/GIS/services/drawing-manager.service';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import { enum_PagineGiasNG, enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { contestoPostMessage, ObjParametriAgenda } from 'gias-ui-kit';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { DrawingService } from '../../services/drawing.service';
import { WKTService } from '../../services/wkt.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Component, OnDestroy, OnInit, Optional, Predicate, TemplateRef, ViewChild } from '@angular/core';
import { PolygonWindowEventsService } from '../../services/polygon-window-events.service';
import { GoogleMapService } from '../../google-map/google-map.service';
import { TreeGisService } from '../../../Utility/Template/kendo-tree/services/tree-gis.service';
import { ChiaveAlbero, TipologiaLayer } from '../../../Service/api.service';
import { from, map, of, Subject, switchMap, takeUntil, tap, withLatestFrom } from 'rxjs';
import { Appezzamento } from '../../../Model/anagrafiche/Appezzamento';
import { TranslocoService } from '@jsverse/transloco';
import { SharedDataService } from '../../services/shared-data.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { EditFeatureService } from 'app/GIS/services/edit-feature.service';
import { GeoJson_Geometry_New, FeatureType, GeoJSONAgroGisPropTreeNode } from 'app/Model/GIS/GisDataReadRval_New';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { enum_TipologiaLayer } from 'app/GIS/GIS-enum/GIS-tipologia-layer';
import { LayerService } from '../../services/layer.service';
import { GiasPolygon } from 'app/GIS/models/gias-drawings.model';
import { DrawWindowOperationService } from '../draw-window/draw-window-operation.service';
import { enum_GISDrawingOperations } from 'app/GIS/GIS-enum/GIS-drawing-operations';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { enum_LayerElementiGraficiStd } from '../../GIS-enum/GIS-layer-elementi-grafici';
import { GoogleMapGeoJsonLazyService } from 'app/GIS/services/google.maps-services/google-map-geojson-lazy.service';
import { GeoJsonUtils } from 'app/GIS/utils/geo-json.utils';
import { GeoJson_New_1OfGeoJSONAgroGisProp } from 'app/Service/net-core6-api.service';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';
import { SementieriParametrizzazione } from 'app/Model/GIS/SementieriParametrizzazione';
import { SementieriService } from 'app/Service/sementieri.service';

export enum Enum_AssociazioneGIS {
  Nessuno,
  Appezzamento,
  Impianto,
  AppezzamentoEImpianto
}

export enum TipoOperazioneAnagrafica {
  Indefinito,
  Inserimento,
  Modifica,
  ModificaPerAssociazione
}

export enum tipoSalvataggioDdl {
  IMPIANTIAPPEZZAMENTI,
  CAMPI
}

@Component({
  standalone: false,
  selector: 'polygon-window',
  templateUrl: './polygon-window.component.html',
  styleUrls: ['./polygon-window.component.css'],
})
export class PolygonWindowComponent implements OnInit, OnDestroy {

  @ViewChild('associazioneTemplate') public associazioneTemplate: TemplateRef<any>;

  private polygonValidator: any;

  public signal$: Subject<void> = new Subject();

  objParametriAgenda: ObjParametriAgenda;
  polyWindowForm: FormGroup;
  associazioneWindowForm: FormGroup;
  area: number = 0;

  functionMessageEventListener: any;

  itemSelected: TipologiaLayer;

  public get ddlSupLabel(): string {
    return `Associa la superficie calcolata ${this.area} [Ha] a ?`;
  }

  ddlGISLabel: string = 'Genera elemento GIS sui layer:';

  get polygons() {
    // filtro solo i poligoni appartenenti ai layer APPEZZAMENTI o IMPIANTI o CAMPI
    return this.drawingService.polygons.filter(this.polygonCondition);
  }

  private polygonCondition: Predicate<GiasPolygon> = (p: GiasPolygon) => {
    if (p.tipologiaLayer?.id == enum_LayerElementiGraficiStd.WMS) return false;
    let tipo: string = this.sharedDataService.getTipoLayerSelezionato();
    if (tipo == enum_TipologiaLayer.Cultivar || tipo == enum_TipologiaLayer.GruppoColturale) {
      return true;
    } else if (tipo == enum_TipologiaLayer.OrganizzazioneAppartenenza && this.currentIsSementieriSportello) {
      return true;
    } else {
      return p.tipologiaLayer?.id == enum_LayerElementiGraficiStd.IMPIANTI ||
        p.tipologiaLayer?.id == enum_LayerElementiGraficiStd.APPEZZAMENTI ||
        p.tipologiaLayer?.id == enum_LayerElementiGraficiStd.CAMPI;
    }
  };

  currentIsSementieriSportello = false;
  isSementieri$ = this.sementieriService.isSementieriSportello()
    .pipe(
      takeUntil(this.signal$),
      tap(isSementieriSportello => this.currentIsSementieriSportello = isSementieriSportello)
    );

  tipoSalvataggio: Array<any> = [
    { descrizione: 'Impianto e Appezzamento', codice: tipoSalvataggioDdl.IMPIANTIAPPEZZAMENTI },
    // {descrizione: 'Impianti Pianificati', codice: 3},
    // {descrizione: 'Aree Omogenee', codice: 4},
    // {descrizione: 'Ettari Equivalenti', codice: 5},
    // {descrizione: 'Fasce di Rispetto', codice: 6},
    { descrizione: 'Campo', codice: tipoSalvataggioDdl.CAMPI },
  ];

  tipoSalvataggioSementieri: Array<any> = [
    { descrizione: 'Impianto e Appezzamento', codice: tipoSalvataggioDdl.IMPIANTIAPPEZZAMENTI },
  ];

  assiciazioneSup: Array<any> = [
    { descrizione: 'Nessuno', codice: Enum_AssociazioneGIS.Nessuno },
    { descrizione: 'Impianto', codice: Enum_AssociazioneGIS.Impianto },
    { descrizione: 'Impianti e Appezzamento', codice: Enum_AssociazioneGIS.AppezzamentoEImpianto }
  ];

  associazioneGIS: Array<any> = [
    { descrizione: 'Appezzamento', codice: Enum_AssociazioneGIS.Appezzamento },
    { descrizione: 'Impianto', codice: Enum_AssociazioneGIS.Impianto },
    { descrizione: 'Impianti e Appezzamento', codice: Enum_AssociazioneGIS.AppezzamentoEImpianto }
  ];

  private appezzamento: Appezzamento;
  private impianto: Impianto;

  constructor(
    private drawingService: DrawingService,
    private drawingManagerService: DrawingManagerService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private giasIFrameWindowService: GiasIFrameWindowService,
    private wktService: WKTService,
    private polygonWindowEventsService: PolygonWindowEventsService,
    private googleMapService: GoogleMapService,
    private treeGisService: TreeGisService,
    private transloco: TranslocoService,
    private sharedDataService: SharedDataService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private funzioniComuniService: FunzioniComuniService,
    private editFeatureService: EditFeatureService,
    private giasMessageService: GiasMessageService,
    private layerService: LayerService,
    private drawWindowOperationService: DrawWindowOperationService,
    private kendoWindowsService: KendoWindowsService,
    private googleMapGeoJsonLazyService: GoogleMapGeoJsonLazyService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    private sementieriService: SementieriService,
    @Optional() private fb: FormBuilder,
  ) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.polyWindowForm = this.fb.group({
      ddlTipoValue: [{
        codice: [0],
        descrizione: ['']
      }],
      polygonWKT: ['']
    });

    this.associazioneWindowForm = this.fb.group({
      ddlSupValue: [{
        codice: [Enum_AssociazioneGIS.Nessuno],
        descrizione: ['']
      }],
      ddlGISValue: [{
        codice: [Enum_AssociazioneGIS.AppezzamentoEImpianto],
        descrizione: ['']
      }]
    });

    this.functionMessageEventListener = ((me) => this.messageEventHandler(me)).bind(this);

    this.editFeatureService.chiusuraFinestraModifica
      .pipe(takeUntil(this.signal$)).subscribe(parametriChiusuraFinestraModifica => {
      if (parametriChiusuraFinestraModifica[0] !== undefined &&
        parametriChiusuraFinestraModifica[1] !== undefined) {
        this.gestioneChiusuraFinestraModifica(
          parametriChiusuraFinestraModifica[0],
          parametriChiusuraFinestraModifica[1]
        )
      }
    });

    this.sharedDataService.datiFeatureConAttributiSource
      .pipe(
        takeUntil(this.signal$),
        switchMap(datiFeature => {
          if (datiFeature.DatiCompleti) {
            if (datiFeature.TipoOperazione === enum_TipoOperazioneDB.Scrittura) {
              return this.googleMapGeoJsonService.inserisciFeatureConAttributi(datiFeature);
            } else {
              return this.googleMapGeoJsonService.modificaFeatureConAttributi(datiFeature);
            }
          }

          return of(null);
        })
      )
      .subscribe();

    this.layerService.layerItemSelected$
      .pipe(takeUntil(this.signal$)).subscribe(([item, selected]) => {
      if (selected) {
        this.itemSelected = item;
        this.setDdlValue();
      }
    });
  }

  ngOnInit(): void {
    this.polyWindowForm.controls['ddlTipoValue'].setValue({ codice: 2 });
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  private setDdlValue(): void {
    if (this.itemSelected.id == enum_LayerElementiGraficiStd.IMPIANTI || this.itemSelected.id == enum_LayerElementiGraficiStd.APPEZZAMENTI)
      this.polyWindowForm.controls['ddlTipoValue'].setValue({ codice: tipoSalvataggioDdl.IMPIANTIAPPEZZAMENTI });
    if (this.itemSelected.id == enum_LayerElementiGraficiStd.CAMPI)
      this.polyWindowForm.controls['ddlTipoValue'].setValue({ codice: tipoSalvataggioDdl.CAMPI });
  }

  calculateArea_Ha(polygon: GiasPolygon): number {
    this.area = Number.parseFloat((google.maps.geometry.spherical.computeArea(polygon.polygon.getPath()) / 10000).toFixed(7));
    return this.area;
  }

  private isParticella(ca: ChiaveAlbero): boolean {
    return ca.p_Part_Cod != '' && ca.p_Part_Cod != '0';
  }

  private isAppezzamentoImpianto(ca: ChiaveAlbero): boolean {
    return ca.Appezza != 0;
  }

  private isCampo(ca: ChiaveAlbero): boolean {
    return ca.Campo_Cod != 0;
  }

  private checkIfAssociazione(ca: ChiaveAlbero): boolean {
    let associazione = false;

    if (this.isParticella(ca) || this.isAppezzamentoImpianto(ca) || this.isCampo(ca)) {
      associazione = this.sharedDataService.getLastCheckedKeyFeatureId() == null ||
        this.sharedDataService.getLastCheckedKeyFeatureId().length == 0;
    }

    return associazione;
  }

  save(polygon: GiasPolygon) {
    let associazione: boolean = false;
    let ca: ChiaveAlbero;

    if (this.sharedDataService.getTreeViewCheckedKeys().length == 1) {
      let chiave = this.treeGisService.getChiaveAlberoFromCheckedKey(this.sharedDataService.getTreeViewCheckedKeys()[0]);
      ca = FunzioniComuniService.scomponiChiaveAlbero(chiave);
      associazione = this.checkIfAssociazione(ca);
    }

    const DrawingManager = require('../../../GiasJSLibraries/GIS-js-libraries/DrawingManager');
    this.polygonValidator = new DrawingManager.PolygonValidator(polygon.polygon.getPath());

    if (this.polygonValidator.isValid) {

      if (associazione) {
        let layer: string;
        if (this.polyWindowForm.controls['ddlTipoValue'].value.codice == tipoSalvataggioDdl.IMPIANTIAPPEZZAMENTI)
          layer = enum_LayerElementiGraficiStd.IMPIANTI;
        if (this.polyWindowForm.controls['ddlTipoValue'].value.codice == tipoSalvataggioDdl.CAMPI)
          layer = enum_LayerElementiGraficiStd.CAMPI;
        this.saveAssociazioneEstesa(polygon, ca, layer);
      } else {
        this.saveEntityGlobal(polygon, ca);
      }

    } else {
      DrawingManager.DisplayErrorPolyLines(this.polygonValidator.intersection, this.googleMapService.googleMapWrapper.googleMap);
    }
  }

  public cancel(polygon: GiasPolygon): void {
    polygon.drawingService.removeEvent.next(polygon);
    this.drawingManagerService.stopDrawingMode();

    this.kendoWindowsService.close(WindowTypes.MarkerWindow);
    this.drawWindowOperationService.setOperation(enum_GISDrawingOperations.none);
  }

  private prepareParametriAgenda(polygon: GiasPolygon): void {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    this.objParametriAgenda.Appezza = 0;
    this.objParametriAgenda.Id_Reg = 0;
    this.objParametriAgenda.Sa_Cod = 0;
    this.prepareParametriPolygon(polygon)

    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
  }

  private prepareParametriPolygon(polygon: GiasPolygon): void {
    let area = this.calculateArea_Ha(polygon);
    this.objParametriAgenda.GenericObj_string = JSON.stringify(area);
  }

  private gestioneChiusuraFinestraModifica(
    modificaPerAssociazione: boolean,
    me: MessageEvent<any>
  ) {
    let tipoOperazione = modificaPerAssociazione ?
      TipoOperazioneAnagrafica.ModificaPerAssociazione :
      TipoOperazioneAnagrafica.Modifica

    if (this.sharedDataService.getLayerSelezionatoSourceAsValue().id == enum_LayerElementiGraficiStd.CAMPI) {
      this.treeGisService.loadDataInternalFilters(true);
      this.polygonWindowEventsService.reloadFeatures.next('');
    } else
      this.aggiornaFeatureAlberoDaSalvaAppezzamento(
        tipoOperazione,
        me
      );
  }

  private messageEventHandler(me: MessageEvent<any>): void {
    window.removeEventListener('message', this.functionMessageEventListener);

    if (this.polyWindowForm.controls['ddlTipoValue'].value.codice == tipoSalvataggioDdl.CAMPI) {
      this.treeGisService.loadDataInternalFilters(true);
      this.polygonWindowEventsService.reloadFeatures.next('');

      if (this.isDisegnatoNuovoPoligono(TipoOperazioneAnagrafica.Inserimento)) {
        this.cancellaPoligono();
      }
    } else
      this.aggiornaFeatureAlberoDaSalvaAppezzamento(
        TipoOperazioneAnagrafica.Inserimento,
        me
      );
  }

  // TODO Salvo: gestire anche i campi
  aggiornaFeatureAlberoDaSalvaAppezzamento(
    tipoOperazione: TipoOperazioneAnagrafica,
    me: MessageEvent<any>
  ): void {
    if (this.isDisegnatoNuovoPoligono(TipoOperazioneAnagrafica.Inserimento)) {
      this.cancellaPoligono();
    }
    if (this.isPostMessageSalvaAppezzamento(me)) {
      let tipoLayerSelezionato = this.sharedDataService.getTipoLayerSelezionato();
      if (tipoLayerSelezionato !== enum_TipologiaLayer.Entita) {
        this.polygonWindowEventsService.reloadFeatures.next('');
      }
      this.appezzamento = <Appezzamento>me.data.inData;
      this.impianto = null;
      if (this.appezzamento.impianti?.length > 0) {
        this.impianto = this.appezzamento.impianti[0];
      }
      switch (tipoOperazione) {
        case TipoOperazioneAnagrafica.Inserimento:
          //--------------------------------------------------------------------------------
          // Inserimento anagrafica e poligono
          //--------------------------------------------------------------------------------
          if (tipoLayerSelezionato === enum_TipologiaLayer.Entita) {
            this.caricaFeatureAppezzaImpiantoDaGeoJson();
          }
          this.inserisciNodoAlberoAppezzaImpiantiDaInfo();
          break;
        case TipoOperazioneAnagrafica.Modifica:
          //--------------------------------------------------------------------------------
          // Modifica anagrafica e poligono
          //--------------------------------------------------------------------------------
          // Non serve cancellare le feature precedenti, in quando vengono sovrascritte
          // a parità di chiave
          if (tipoLayerSelezionato === enum_TipologiaLayer.Entita) {
            this.caricaFeatureAppezzaImpiantoDaGeoJson();
          }
          this.aggiornaNodoAlberoAppezzaImpiantiDaInfo();
          break;
        case TipoOperazioneAnagrafica.ModificaPerAssociazione:
          //--------------------------------------------------------------------------------
          // Modifica anagrafica con inserimento poligono
          //--------------------------------------------------------------------------------
          if (tipoLayerSelezionato === enum_TipologiaLayer.Entita) {
            this.caricaFeatureAppezzaImpiantoDaGeoJson();
          }
          this.aggiornaNodoAlberoAppezzaImpiantiDaInfo();
          break;
      }
    } else {
      //--------------------------------------------------------------------------------
      // Dato che si tratta di un errore che capita sporadicamente e non è bloccante
      // per l'utente, viene mostrato come una info
      //--------------------------------------------------------------------------------
      const messaggio = this.transloco.translate('gis.NecessarioCaricamento');
      this.giasMessageService.infoMessagge(messaggio);
      console.log('messageEventData', me.data);

      // Ricaricamento feature
      this.polygonWindowEventsService.reloadFeatures.next('');
    }
  }

  isDisegnatoNuovoPoligono(tipoOperazione: TipoOperazioneAnagrafica): boolean {
    return tipoOperazione === TipoOperazioneAnagrafica.Inserimento ||
      tipoOperazione === TipoOperazioneAnagrafica.ModificaPerAssociazione
  }

  isPostMessageSalvaAppezzamento(me: MessageEvent<any>): boolean {
    return me.data.contesto &&
      me.data.inData &&
      me.data.contesto === contestoPostMessage.SalvaAppezzamento
  }

  private cancellaPoligono(): void {
    if (this.polygons[0] !== undefined) {
      this.cancel(this.polygons[0]);
    }
  }

  caricaFeatureAppezzaImpiantoDaGeoJson() {
    // Appezzamento
    this.seCaricaFeatureDaGeoJson(this.appezzamento.obj_app.myGeoJson);
    // Impianto
    if (this.impianto) {
      this.seCaricaFeatureDaGeoJson(this.impianto.obj_imp.myGeoJson);
    }
  }

  private seCaricaFeatureDaGeoJson(geoJson: GeoJson_New_1OfGeoJSONAgroGisProp) {
    //--------------------------------------------------------------------------------
    // TODO Andrea (4): capire se possibile far funzionare il cast o
    //                  serve funzione sistemazioneGeoJson()
    //--------------------------------------------------------------------------------
    // let geoJsonTest = this.sharedDataService.getGeoJsonTest();
    // let geoJsonTest1 = JSON.stringify(geoJsonTest);
    // let geoJsonTest2 = JSON.parse(geoJsonTest1);
    // this.googleMapGeoJsonService.addGeoJsonOnly(geoJsonTest); // FUNZIONA
    // this.googleMapGeoJsonService.addGeoJsonOnly(geoJsonTest2); // NON FUNZIONA
    //--------------------------------------------------------------------------------
    if (geoJson?.geoJsonCaricato?.features?.length > 0) {
      this.sistemazioneGeoJson(geoJson);
      geoJson.geoJsonCaricato.features.forEach(feature => {
        this.googleMapGeoJsonLazyService.addFeature(feature, this.googleMapService.googleMapWrapper.data.getMap());
      });
    }
  }

  private sistemazioneGeoJson(geoJson: GeoJson_New_1OfGeoJSONAgroGisProp) {
    geoJson.geoJsonCaricato.features.forEach(f => {
      let coord = JSON.stringify(f.geometry.coordinates);
      let tipo = FeatureType[f.geometry.type];
      f.geometry = new GeoJson_Geometry_New(tipo, coord);
    });
  }

  private inserisciNodoAlberoAppezzaImpiantiDaInfo() {
    // Appezzamento
    this.seInserisciNodoAlberoDaInfo(this.appezzamento.nodeInfo_app);
    // Impianto
    if (this.impianto) {
      this.seInserisciNodoAlberoDaInfo(this.impianto.nodeInfo_imp);
    }
  }

  private seInserisciNodoAlberoDaInfo(infoNodo: GeoJSONAgroGisPropTreeNode) {
    if (infoNodo) {
      this.inserisciNodoAlberoDaInfo(infoNodo);
    }
  }

  private inserisciNodoAlberoDaInfo(infoNodo: GeoJSONAgroGisPropTreeNode) {
    let nodo = this.funzioniComuniService.getNewTreeNode();
    nodo.id = infoNodo.id;
    nodo.type = infoNodo.type;
    nodo.text = infoNodo.text;
    nodo.imageUrl = infoNodo.imageUrl;
    nodo.style = infoNodo.style;
    nodo.startDate = infoNodo.startDate;
    nodo.endDate = infoNodo.endDate;
    this.treeGisService.aggiungiNodoAlbero(nodo);
  }

  private aggiornaNodoAlberoAppezzaImpiantiDaInfo() {
    // Appezzamento
    this.seAggiornaNodoAlberoDaInfo(this.appezzamento.nodeInfo_app);
    // Impianto
    if (this.impianto) {
      this.seAggiornaNodoAlberoDaInfo(this.impianto.nodeInfo_imp);
    }
  }

  private seAggiornaNodoAlberoDaInfo(infoNodo: GeoJSONAgroGisPropTreeNode) {
    if (infoNodo) {
      this.aggiornaNodoAlberoDaInfo(infoNodo);
    }
  }

  private aggiornaNodoAlberoDaInfo(infoNodo: GeoJSONAgroGisPropTreeNode) {
    let nodo = this.funzioniComuniService.getNewTreeNode();
    nodo.id = infoNodo.id;
    nodo.text = infoNodo.text;
    nodo.startDate = infoNodo.startDate;
    nodo.endDate = infoNodo.endDate;
    this.treeGisService.modificaNodoAlbero(nodo);
  }

  private saveEntityGlobal(polygon: GiasPolygon, ca?: ChiaveAlbero) {
    window.addEventListener('message', this.functionMessageEventListener);

    this.prepareParametriAgenda(polygon);
    const newObjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgenda));
    newObjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_GIS;
    const parametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];

    this.polyWindowForm.value.polygonWKT = this.wktService.geoJsonToWKT(GeoJsonUtils.polygonToGeoJson(polygon.polygon));

    let pagina;
    if (this.polyWindowForm.controls['ddlTipoValue'].value.codice == tipoSalvataggioDdl.IMPIANTIAPPEZZAMENTI)
      pagina = enum_PagineGiasNG.Pagina_Edit_AppezzamentoGlobal;
    if (this.polyWindowForm.controls['ddlTipoValue'].value.codice == tipoSalvataggioDdl.CAMPI)
      pagina = enum_PagineGiasNG.Pagina_Edit_Campo;

    from(this.gestioneRichiesteService.gestionePassaggioAltroSito(
      Enum_SiteRedirector.GiasNG,
      pagina,
      parametriAggiuntivi,
      newObjParametriAgenda
    ))
      .pipe(withLatestFrom(this.configurazioneSitiService.leggiChiave(EnumChiaviConfigurazioneSiti.Is_Sementieri)))
      .subscribe(([resp, isSementieriKey]) => {
        let sementieri = this.sharedDataService.getCfgSementiAsValue();
        if (sementieri == null && isSementieriKey?.Valore?.toLowerCase() === 'true') {
          // force sportello with mappatura libera if we are in a sementieri site
          sementieri = new SementieriParametrizzazione({
            Sementi: `0|0|0|0|0|GIS|01/01/1900 00:00:00|31/12/2099 00:00:00`,
            DatiPassaggio: '',
            SementiMappaturaLibera: 'True'
          });
        }

        const filtroTemporale = this.sharedDataService.getFiltroTemporaleAvanzato();

        resp += '?seFrame=1&wkt=' + this.polyWindowForm.value.polygonWKT +
          '&area=' + this.area +
          '&tipoSalvataggio=' + this.polyWindowForm.value['ddlValue'] +
          '&cfgSementi=' + JSON.stringify(sementieri) +
          '&filtroTemporale=' + JSON.stringify(filtroTemporale);

        this.giasIFrameWindowService.open({
          title: this.transloco.translate('gis.SalvataggioPoligono'),
          content: resp,
          height: window.innerHeight,
          width: window.innerWidth * 0.9
        });
      });
  }

  private saveAssociazioneEstesa(objPolygon: GiasPolygon, chiaveAlbero: ChiaveAlbero, layer: string) {
    let polygonWKT: string = this.wktService.geoJsonToWKT(GeoJsonUtils.polygonToGeoJson(objPolygon.polygon));
    this.editFeatureService.modificaElementoAnagraficaPerAssociazione(polygonWKT, this.area, chiaveAlbero, layer);
  }

}

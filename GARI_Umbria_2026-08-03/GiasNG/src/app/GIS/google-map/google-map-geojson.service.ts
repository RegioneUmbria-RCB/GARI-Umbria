import { Injectable, Optional } from '@angular/core';
import { FeatureType } from 'app/Model/GIS/GisDataReadRval_New';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GoogleMapService } from './google-map.service';
import { DataLayerStyleService } from '../services/data-layer-style.service';
import { enum_FeatureGeometryType, enum_FeatureProperty, GISModality } from '../GIS-enum/GIS-feature';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { FeatureService } from 'app/GIS/services/feature.service';
import { MasterService } from 'app/Service/master.service';
import { LayerService } from '../services/layer.service';
import { LayerStyleService } from '../services/layer-style.service';
import { GisToolbarService } from '../GIS-toolbar/gis-toolbar.service';
import { enum_OrigineChiamata, enum_OrigineChiamataFilterService, enum_OrigineChiamataLoadGeoJson } from '../GIS-enum/GIS-origine-chiamata';
import { ConfigurazioneAlbero, FiltroTemporale, GeoJson_Feature_New_1OfGeoJSONAgroGisProp, GeoJson_New_1OfGeoJSONAgroGisProp, GeoJSONAgroGisProp, GetProprieta_In, GetProprieta_Out, GisClient, GisDataReadParam, LeggiDateImpiantiPerFiltroTemporale_In, TipologiaLayer } from 'app/Service/api.service';
import { TranslocoService } from '@jsverse/transloco';
import { GISGeometrySelectionService } from '../services/gis-geometry-selection.service';
import { GeoJsonFilterService } from '../services/geojson-filter.service';
import { WmsService } from '../services/wms.service';
import { BehaviorSubject, Observable, Subject, combineLatest, map, of, startWith, switchMap, take, takeUntil, withLatestFrom } from 'rxjs';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { enum_zIndex } from '../GIS-enum/GIS-zIndex';
import { trim } from 'lodash';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { FiltersService } from 'app/menu-agenda/components/filters/filters.service';
import { GeoJsonFilterServiceParam } from 'app/Model/GIS/GeoJsonFilterServiceParam';
import { enum_zoom, enum_zoomVisualizzazioneTotale } from '../GIS-enum/GIS-zoom';
import { enum_TipoNodo } from 'app/Model/TipiEnumerativi';
import { objTreeNode } from 'app/Utility/Template/kendo-tree/services/tree-gis.service';
import { enum_LayerElementiGraficiStd } from '../GIS-enum/GIS-layer-elementi-grafici';
import { getServiceIdAndLog } from 'app/Service/utils';
import { DatiFeatureConAttributi } from 'app/Model/GIS/FeatureConAttributi';
import { DatiColoreTema, TemaSelezionato } from '../GIS-kendo-window/theme-window/theme-window.component';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import Feature = google.maps.Data.Feature;
import { ThemeWindowService } from '../GIS-kendo-window/theme-window/theme-window.service';
import { GoogleMapDataService } from '../services/google.maps-services/google-map-data.service';
import { PolygonLabelInfowindowService } from '../services/polygon-label-infowindow.service';
import { GISAnalisiMappeSatellitariWindowService } from '../GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-window.service';
import { GISRasterConfigurationWindowService } from '../GIS-raster-configuration-window/GIS-raster-configuration-window.service';
import { EditFeatureWindowService } from '../services/edit-feature-window.service';
import { GoogleMapGeoJsonLazyService } from '../services/google.maps-services/google-map-geojson-lazy.service';
import { GoogleMapUtils } from '../utils/google-map.utils';
import { FeatureInformationService } from '../services/feature-information.service';
import { GeoJsonUtils } from '../utils/geo-json.utils';
import { ObjParametriAgenda, ImpiantiAgendaNG } from 'gias-ui-kit';
import { SementieriParametrizzazione } from 'app/Model/GIS/SementieriParametrizzazione';
import { SementieriService } from 'app/Service/sementieri.service';

class MessaggioFinestraInformativa {
  contenuto: string;

  constructor() {
    this.contenuto = ""
  }

  seAggiungiRigaTitolo(titolo: string) {
    const stile = "font-size: larger; font-weight: bolder; margin-bottom: 5px;";
    if (titolo === null) {
      titolo = "";
    }
    this.aggiungiRigaTesto(titolo, stile);
  }

  seAggiungiRigaEtichettaValore(etichetta: string, valore: string) {
    if (valore) {
      const messaggio = `${etichetta}: ${valore}`;
      this.aggiungiRigaTesto(messaggio);
    }
  }

  aggiungiRigaTesto(messaggio: string, stile?: string) {
    let attributoStyle = "";
    if (stile !== 'undefined') {
      attributoStyle = `style='${stile}'`;
    }
    this.contenuto += `<div ${attributoStyle}>${messaggio}</div>`;
  }

  aggiungiRigaCorsivo(messaggio: string) {
    this.contenuto += `<div><em>${messaggio}</em></div>`;
  }

  aggiungiRigaSottolineato(messaggio: string) {
    this.contenuto += `<div><u>${messaggio}</u></div>`;
  }

  aggiungiFineRiga() {
    this.contenuto += "<br>";
  }

}

@Injectable()
export class GoogleMapGeoJsonService {

  public set modality(modality: GISModality) {
    this.modalitySubject.next(modality);
  }

  public get modality(): GISModality {
    return this.modalitySubject.value;
  }

  public get modality$(): Observable<GISModality> {
    return this.modalitySubject.asObservable();
  }

  private get googleMapWrapper() {
    return this.googleMapService.googleMapWrapper;
  }

  private signal = new Subject<void>();

  // Interazione con QdC
  public geoJsonCaricato: boolean = false;
  public richiestaCaricamentoGeoJsonQdc: boolean = false;
  public impostaCentroMappaQdc: boolean = false;

  // Finestra informativa
  private finestraInformativa: google.maps.InfoWindow;
  private indicatoreAppoggio: google.maps.Marker;

  private btnsFinestraInformativa: string =
    '<br>' +
    '<div class="row">' +
    '<div class="col-lg-6 col-md-6 col-sm-6">' +
    `${this.transloco.translate('gis.CopiaGeoJson')}` +
    `<div gisTooltip title="${this.transloco.translate('gis.CopiaGeoJson')}" class="k-mt-3">` +
    '<button style="border: 0;background-color: white;border-radius: 5px;" kendoButton fillMode="outline" [primary]="false" onClick="document.getElementById(\'copyGeoJsonBtnInfoWindow\').click()"> <i aria-hidden="true" class="xi-draw-copy-object"></i> </button>' +
    '</div>' +
    '</div>' +
    '<div class="col-lg-6 col-md-6 col-sm-6 align-end">' +
    `${this.transloco.translate('gis.CopiaWKT')}` +
    `<div gisTooltip title="${this.transloco.translate('gis.CopiaWKT')}" class="k-mt-3">` +
    '<button style="border: 0;background-color: white;border-radius: 5px;" kendoButton fillMode="outline" [primary]="false" onClick="document.getElementById(\'copyWKTBtnInfoWindow\').click()"> <i aria-hidden="true" class="xi-draw-copy-object"></i> </button>' +
    '</div>' +
    '</div>' +
    '</div>';

  // GPS
  private indicatoreGps: google.maps.Marker;
  public lastClickedCoordinates: BehaviorSubject<google.maps.LatLng> = new BehaviorSubject<google.maps.LatLng>(null);

  // Ricerca indirizzo
  private geocoder: google.maps.Geocoder;

  // Altro
  private maxZIndex: number;
  private modalitySubject = new BehaviorSubject<GISModality>(GISModality.Full);
  private objParametriAgenda: ObjParametriAgenda;
  private gisDataReadParamOld: GisDataReadParam = null;
  private readonly currentAgenda: ObjParametriAgenda;
  private featureClickEventListenerHandle: any = null;
  private featureDoubleClickEventListenerHandle: any = null;

  private readonly appezzamentiDes: string;
  private readonly impiantiDes: string;
  private precisionDataFeaturesIds: (string | number)[] = [];

  public serviceId = null;

  /**
   * Usata per disabilitare la deselezione di un impianto quando specifico la posizione di un rilievo.
   */
  public disabilitaDeselezione: boolean = false;

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    private googleMapService: GoogleMapService,
    private googleMapDataService: GoogleMapDataService,
    private dataLayerStyleService: DataLayerStyleService,
    private polygonLabelInfoWindowService: PolygonLabelInfowindowService,
    private gisClient: GisClient,
    private sharedDataService: SharedDataService,
    private featureService: FeatureService,
    private masterService: MasterService,
    private transloco: TranslocoService,
    private layerStyleService: LayerStyleService,
    private gisGeometrySelectionService: GISGeometrySelectionService,
    private geoJsonFilterService: GeoJsonFilterService,
    private wmsService: WmsService,
    private giasDialogService: GiasDialogService,
    private funzioniComuniService: FunzioniComuniService,
    private filtersService: FiltersService,
    private giasMessageService: GiasMessageService,
    private kendoWindowsService: KendoWindowsService,
    private geoJsonLazyService: GoogleMapGeoJsonLazyService,
    private featureInformationService: FeatureInformationService,
    private sementieriService: SementieriService,
    @Optional() private themeWindowService: ThemeWindowService,
    @Optional() private gisToolbarService: GisToolbarService,
    @Optional() private layerService: LayerService,
    @Optional() private gisRasterConfigurationWindowService: GISRasterConfigurationWindowService,
    @Optional() private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    @Optional() private editFeatureWindowService: EditFeatureWindowService,
  ) {
    this.serviceId = getServiceIdAndLog('GoogleMapGeoJsonService', 'constructor');

    this.currentAgenda = this.objParametriAgendaService.getObjParamValue();
    this.appezzamentiDes = this.transloco.translate('Appezzamenti').toUpperCase();
    this.impiantiDes = this.transloco.translate('Impianti').toUpperCase();
  }

  //====================================================================================================
  // FUNZIONI LETTURA GEOJSON
  //====================================================================================================

  private prepareReadParameters(filterServiceParam: GeoJsonFilterServiceParam): GisDataReadParam {

    let gj_params: GisDataReadParam = {};
    gj_params.layerElementiGrafici_cod = [];

    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    const config = this.sharedDataService.getCfgAlberoGisUtente()[0];
    if (config?.CfgAlbero !== undefined &&
      config?.CfgAlbero?.FiltroImpiantiIdTestataTemp !== undefined &&
      config?.CfgAlbero?.FiltroImpiantiIdTestataTemp !== 0) {
      gj_params.cfgAlbero = config.CfgAlbero;
    }

    const gjFilterService = this.geoJsonFilterService.getGisDataReadParam();
    if (filterServiceParam.FlagLoadGeoJson && gjFilterService != null) {

      if (filterServiceParam.OrigineChiamata === enum_OrigineChiamataFilterService.QuadernoDiCampagna) {
        this.richiestaCaricamentoGeoJsonQdc = true;
      }

      gj_params.piva = gjFilterService.piva;
      gj_params.sa_cod = gjFilterService.sa_cod;
      gj_params.campo_cod = gjFilterService.campo_cod;
      gj_params.veg_cod = gjFilterService.veg_cod;
      gj_params.layerElementiGrafici_cod = gjFilterService.layerElementiGrafici_cod;
      gj_params.filtroTemporale = gjFilterService.filtroTemporale;
      gj_params.filtroTemporaleSingolaData = gjFilterService.filtroTemporaleSingolaData;
      gj_params.TipologiaLayerSelezionata = gjFilterService.TipologiaLayerSelezionata;
      gj_params.wktBoundaySTIntersects = gjFilterService.wktBoundaySTIntersects;

      if (gj_params.wktBoundaySTIntersects != null && gj_params.wktBoundaySTIntersects != '') {
        gj_params.zoomLevel = this.googleMapService.googleMapWrapper.getZoom();
      }

    } else {

      // Piva
      gj_params.piva = this.objParametriAgenda.Piva;

      // SaCod
      let cfgAlbero: ConfigurazioneAlbero = this.sharedDataService.getCfgAlberoGisUtente()[0].CfgAlbero;
      gj_params.sa_cod = '0';
      if (cfgAlbero?.Sa_Cod) {
        gj_params.sa_cod = cfgAlbero.Sa_Cod;
      }

      // Filtro temporale
      const filtroTemporaleAvanzato = this.sharedDataService.getFiltroTemporaleAvanzato();
      gj_params.filtroTemporale = filtroTemporaleAvanzato.filtroTemporalePeriodo;
      gj_params.filtroTemporaleSingolaData = filtroTemporaleAvanzato.filtroTemporaleSingolaData;

      // Tipologia layer
      gj_params.TipologiaLayerSelezionata = this.sharedDataService.getTipoLayerSelezionato();
    }

    return gj_params;

  }

  public loadGeoJsonBase(impostaCentroMappa: boolean) {
    if (!this.sementieriService.isSementieriSportello()) {
      this.sharedDataService.setResetVisualizzazioneTotale(true);
    }

    let filterServiceParam: GeoJsonFilterServiceParam = null;
    if (this.sementieriService.isSementieriSportello()) {
      filterServiceParam = new GeoJsonFilterServiceParam(
        enum_OrigineChiamataFilterService.Indefinito,
        impostaCentroMappa,
        true
      );
    }

    this.googleMapService.loaded$
      .pipe(take(1))
      .subscribe(() => this.loadGeoJson(impostaCentroMappa, filterServiceParam, false));

  }

  public loadGeoJsonFilterService(filterServiceParam: GeoJsonFilterServiceParam): void {
    this.loadGeoJson(filterServiceParam.ImpostaCentroMappa, filterServiceParam, true)
  }

  public loadGeoJsonForzato(impostaCentroMappa: boolean) {
    let visualizzazioneTotale = this.sharedDataService.getVisualizzazioneTotale();
    let origineChiamata = this.sharedDataService.getOrigineChiamataLoadGeoJson();

    if (visualizzazioneTotale && origineChiamata === enum_OrigineChiamataLoadGeoJson.FiltroTemporale) {
      this.impostaZoomMinimoVisualizzazioneTotale();
      this.updateGeoJsonFilterServiceVisualizzazioneTotale(enum_OrigineChiamataFilterService.FiltroTemporale);
    } else if (visualizzazioneTotale && origineChiamata === enum_OrigineChiamataLoadGeoJson.CambioTipologiaLayer) {
      this.impostaZoomMinimoVisualizzazioneTotale();
      this.updateGeoJsonFilterServiceVisualizzazioneTotale(enum_OrigineChiamataFilterService.CambioTipologiaLayer);
    } else {

      this.googleMapService.loaded$
        .pipe(
          take(1),
          withLatestFrom(this.sementieriService.isSementieriSportello())
        )
        .subscribe(([_, isSementieriSportello]) => {
          let filterServiceParam: GeoJsonFilterServiceParam = null;
          if (isSementieriSportello) {
            filterServiceParam = new GeoJsonFilterServiceParam(
              enum_OrigineChiamataFilterService.Indefinito,
              impostaCentroMappa,
              true
            );
          } else {
            // resetto visualizzazione totale se non sono sotto sportello
            this.sharedDataService.setResetVisualizzazioneTotale(true);
          }

          this.loadGeoJson(impostaCentroMappa, filterServiceParam, true);
        });
    }
  }

  updateGeoJsonFilterServiceVisualizzazioneTotale(origine: enum_OrigineChiamataFilterService) {
    const filtroTemporaleAvanzato = this.sharedDataService.getFiltroTemporaleAvanzato();
    const filtro = {
      filtroTemporale: filtroTemporaleAvanzato.filtroTemporalePeriodo,
      filtroTemporaleSingolaData: filtroTemporaleAvanzato.filtroTemporaleSingolaData,
      piva: '',
      sa_cod: '0',
      campo_cod: '0',
      TipologiaLayerSelezionata: this.sharedDataService.getTipoLayerSelezionato(),
      wktBoundaySTIntersects: this.getPolygonWktFromBoundsMappa(),
      zoomLevel: this.googleMapWrapper.getZoom()
    } as GisDataReadParam;

    // Set Observable per ricaricamento feature
    this.geoJsonFilterService.setGisDataReadParam(filtro);

    let filterServiceParam = new GeoJsonFilterServiceParam(
      origine,
      false,
      true
    );
    this.geoJsonFilterService.setGeoJsonFilterServiceParam(filterServiceParam);
  }

  private loadGeoJson(
    impostaCentroMappa: boolean,
    filterServiceParam: GeoJsonFilterServiceParam,
    forzaCaricamento: boolean
  ) {
    if (filterServiceParam === null) {
      filterServiceParam = new GeoJsonFilterServiceParam(
        enum_OrigineChiamataFilterService.Indefinito,
        impostaCentroMappa,
        false
      );
    }
    this.readGeoJson(impostaCentroMappa, filterServiceParam, forzaCaricamento);
  }

  private readGeoJson(
    impostaCentroMappa: boolean,
    filterServiceParam: GeoJsonFilterServiceParam,
    forzaCaricamento: boolean,
    addGlobal: boolean = true
  ): void {

    // Parametri lettura
    let readParameters: GisDataReadParam = this.prepareReadParameters(filterServiceParam);

    if (this.checkReadGeoJson(readParameters, forzaCaricamento)) {

      this.inizializzaVariabiliMappa();

      let origineChiamata = this.sharedDataService.getOrigineChiamataLoadGeoJson();

      if (origineChiamata === enum_OrigineChiamataLoadGeoJson.CambioTipologiaLayer) {
        this.readGeoJsonCore(impostaCentroMappa, readParameters, addGlobal);
      } else {
        this.layerService.endLoadLayers.pipe(take(1)).subscribe(paramLoadGeoJson => {
          this.readGeoJsonCore(paramLoadGeoJson[0], paramLoadGeoJson[1], addGlobal);
        })
        if (origineChiamata === enum_OrigineChiamataLoadGeoJson.WidgetMultiAzienda) {
          this.layerService.refreshLayers(
            { Option_Value: readParameters.TipologiaLayerSelezionata },
            { forzaCaricamento: impostaCentroMappa, readParameters: readParameters },
            false
          );
        } else {
          this.layerService.refreshFilterableLayers(impostaCentroMappa, readParameters);
        }
      }

    }

  }

  private checkReadGeoJson(
    readParameters: GisDataReadParam,
    forzaCaricamento: boolean
  ): boolean {
    let paramNew = JSON.stringify(this.clonaGisDataReadParam(readParameters));
    let paramOld = JSON.stringify(this.gisDataReadParamOld);

    this.gisDataReadParamOld = this.clonaGisDataReadParam(readParameters);

    return forzaCaricamento === true || paramNew !== paramOld;
  }

  private readGeoJsonCore(
    impostaCentroMappa: boolean,
    readParameters: GisDataReadParam,
    addGlobal: boolean = true
  ) {

    if (this.sharedDataService.getCfgSementiAsValue() != undefined) {
      readParameters.cfgSementi = this.sharedDataService.getCfgSementiAsValue();
      readParameters.wktBoundaySTIntersects = '';
      impostaCentroMappa = true;
    }

    this.geoJsonCaricato = false;
    this.masterService.set_isLoading({ isLoading: true });

    console.log("ricaricamento con subscribe");

    combineLatest([this.gisClient.gisLeggiElencoEntitaGeoJson(readParameters), this.googleMapService.loaded$])
      .pipe(takeUntil(this.signal))
      .subscribe(([r, _]) => {
        if (addGlobal)
          this.addGeoJsonGlobal(r.RispostaStringa.myGeoJson, impostaCentroMappa);
        else {
          this.addGeoJsonPolyLabelsAndClusterer(r.RispostaStringa.myGeoJson, impostaCentroMappa);
          this.impostaCentroMappaDaFeature(false, false);
        }

        this.sharedDataService.geoJsonLoaded.next(true);
        this.layerService.reloadPageNotifier.next(null);
        this.masterService.set_isLoading({ isLoading: false });
      });
  }

  private addGeoJsonGlobal(
    myGeoJson: GeoJson_New_1OfGeoJSONAgroGisProp,
    impostaCentroMappa: boolean
  ): void {
    if (!this.featureClickEventListenerHandle) {
      this.featureClickEventListenerHandle = this.googleMapWrapper.data.addListener('click', e => this.handleClickEvent(e));
    }

    if (!this.featureDoubleClickEventListenerHandle) {
      this.featureDoubleClickEventListenerHandle = this.googleMapWrapper.data.addListener('dblclick', e => this.handleDoubleClickEvent(e));
    }

    this.addGeoJsonOnly(myGeoJson, impostaCentroMappa, true);

    const layerConFeature: Array<string> = [];
    this.editFeatureWindowService?.reset();
    myGeoJson.geoJsonCaricato.features.forEach(feature => {
      const idLayer = feature.properties.layer;
      const datiLayer = this.sharedDataService.getTipologiaLayerById(idLayer);

      if (!layerConFeature.includes(idLayer)) {
        layerConFeature.push(idLayer);
      }
      const layerVisibile = this.sharedDataService.getFlagVisibileBoolean(datiLayer.flagvisibile);
      const isGoogleGridVisible = this.sharedDataService.getCfgAlberoGisUtente()[0]?.CfgGisUtente?.ckGrigliaTiles_Sviluppo ?? false;
      const isRaster = this.sharedDataService.getTipologiaLayerById(idLayer)?.FeatureTypeId === FeatureType.Raster.toString();

      // raster has always layerVisibile = false, we need to check if the google grid is inactive in case of rasters before hiding the features
      if ((!isRaster && !layerVisibile) || (isRaster && !isGoogleGridVisible)) {
        this.googleMapDataService.overrideFeatureStyle(feature, { visible: layerVisibile });
      }
    });

    this.seFiltraLayers(layerConFeature);
    if (this.modalitySubject.value === GISModality.Trattamento || this.modalitySubject.value === GISModality.AnalisiTerreno) {
      this.handleSelectedGeometries(this.gisGeometrySelectionService.getCurrentSelection());
    }

    this.geoJsonCaricato = true;
  }

  private overrideAvversitaLayerStyle(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, icon: any): void {
    if (this.sharedDataService.isFeatureAvversita(feature)) {
      this.themeWindowService?.getIcon(feature, icon);
    }
  }

  public addGeoJsonOnly(myGeoJson: GeoJson_New_1OfGeoJSONAgroGisProp, fitBounds: boolean, reset: boolean) {
    this.geoJsonLazyService.loadGeoJson(myGeoJson.geoJsonCaricato, this.googleMapWrapper.data.getMap(), fitBounds, reset);
  }

  public addPrecisionDataFeatures(myGeoJson: GeoJson_New_1OfGeoJSONAgroGisProp, fitBounds: boolean) {
    for (const id of this.precisionDataFeaturesIds.filter(x => x != null)) {
      const feature = this.featureInformationService.getById(id.toString());
      this.geoJsonLazyService.removeFeature(feature, this.googleMapWrapper.data.getMap());
    }

    this.precisionDataFeaturesIds = myGeoJson.geoJsonCaricato.features.map(x => x.properties.id);
    this.addGeoJsonPolyLabelsAndClusterer(myGeoJson, fitBounds);
  }

  public addGeoJsonPolyLabelsAndClusterer(myGeoJson: GeoJson_New_1OfGeoJSONAgroGisProp, fitBounds: boolean): void {
    this.addGeoJsonOnly(myGeoJson, fitBounds, false);
  }

  private seFiltraLayers(layerConFeature: string[]) {
    const tipoLayerSelezionato = this.sharedDataService.getTipoLayerSelezionato();

    if (this.sharedDataService.tipiLayerDaFiltrare.includes(tipoLayerSelezionato)) {
      const elencoLayer = this.sharedDataService.getTipologiaLayer();
      const elencoLayerConFeature = elencoLayer.filter(obj => layerConFeature.includes(obj.id))

      // Aggiorno observable elenco layers SOLO SE NECESSARIO, ALTRIMENTI PROVOCA UN LOOP DI CHIAMATE
      const layers = this.layerService.ListLayerItemVisible.value.map(x => x.TipologiaLayer);
      if (LayerService.areLayersToBeReloaded(layers, elencoLayerConFeature)) {
        this.layerService.reloadLayers(elencoLayerConFeature);
      }
    }
  }

  //====================================================================================================
  // FUNZIONI GESTIONE EVENTO ZOOM
  //====================================================================================================

  private onZoomListener: google.maps.MapsEventListener | null = null; 

  public startOnZoomFeatureReload(func: () => void): void {
    if (this.googleMapWrapper.googleMap == null || this.onZoomListener != null) {
      return;
    }

    this.onZoomListener = this.googleMapWrapper.googleMap.addListener('zoom_changed', func);
  }

  public stopOnZoomFeatureReload(): void {
    if (this.googleMapWrapper.googleMap != null && this.onZoomListener != null) {
      this.onZoomListener.remove();
      this.onZoomListener = null;
    }
  }

  //====================================================================================================
  // FUNZIONI EVENTO CLICK FEATURE
  //====================================================================================================

  private handleClickEvent(e: google.maps.Data.MouseEvent): void {
    if (this.kendoWindowsService.getOpenState(WindowTypes.MarkerWindow)) {
      return;
    }

    // handled separetely
    if (this.gisToolbarService.isMarkerPlacerActive) {
      google.maps.event.trigger(this.googleMapWrapper.data.getMap(), "click", e);
      return;
    }

    // if any raster map-info is active, handle polygon click in
    if (this.gisRasterConfigurationWindowService.isAnyMapInfoActive) {
      this.gisRasterConfigurationWindowService.nextMapClickEvent(e);
      return;
    }

    this.lastClickedCoordinates.next(e.latLng);

    const feature = this.featureInformationService.getById(e.feature.getId().toString());

    let selezioneMultipla = this.getCtrlKey(e);
    const selezionata = this.selezionaFeature(feature, selezioneMultipla, true, enum_OrigineChiamata.Mappa);

    if (selezionata === true && (this.modalitySubject.value == GISModality.Trattamento || this.modalitySubject.value == GISModality.AnalisiTerreno)) {
      const feat = this.featureInformationService.getById(e.feature.getId().toString());
      const chiaveAlberoFeatureCompleta = this.leggiChiaveAlberoFeatureCompleta(feat);
      this.gisGeometrySelectionService.addFromMap(chiaveAlberoFeatureCompleta);
    }

    this.wmsService.seWmsInfoClickMappa(e);


    // if satellite map-info is active, handle polygon click in
    if (this.kendoWindowsService.getOpenState(WindowTypes.AnalisiMappeSatellitariWindow)) {
      this.gisAnalisiMappeSatellitariWindowService.nextMapClick(e);
    }
  }

  private handleDoubleClickEvent(e: google.maps.Data.MouseEvent): void {
    if (this.kendoWindowsService.getOpenState(WindowTypes.MarkerWindow)
      || this.gisToolbarService.isMarkerPlacerActive
      || (this.kendoWindowsService.getOpenState(WindowTypes.AnalisiMappeSatellitariWindow) && this.gisAnalisiMappeSatellitariWindowService.isMapInfoActive)
      || this.gisRasterConfigurationWindowService.isAnyMapInfoActive
    ) {
      return;
    }

    const feature = this.featureInformationService.getById(e.feature.getId().toString());
    this.featureService.nextFeatureDoubleClicked(feature);
  }

  private getCtrlKey(e: google.maps.Data.MouseEvent): boolean {
    const mouseEvent = "mouseEvent";
    let ctrlKey = false;
    for (const key in e) {
      if (e[key] instanceof MouseEvent) {
        e[mouseEvent] = e[key];
        ctrlKey = e[mouseEvent].ctrlKey
        break;
      }
    }

    return ctrlKey
  }

  public selezionaFeature(
    feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp,
    inputSelezioneMultipla: boolean,
    inputDeselezioneAbilitata: boolean,
    origineChiamata: enum_OrigineChiamata
  ): boolean {
    let selezioneMultipla = inputSelezioneMultipla || this.modalitySubject.value == GISModality.Trattamento || this.modalitySubject.value == GISModality.AnalisiTerreno;

    let deselezionaSingolaFeature = false;

    if (selezioneMultipla && this.featureService.esistonoFeatureSelezionate()) {

      if (inputDeselezioneAbilitata) {
        // Controllo se la feature è già selezionata: in questo caso devo fare la deselezione
        const featureId = feature.properties.id;
        const featureDaDeselezionare = this.featureService.getFeatureSelezionataById(featureId);
        if (featureDaDeselezionare != null) {
          // Deselezione singola feature
          deselezionaSingolaFeature = true;
        }
      }

      if (!deselezionaSingolaFeature) {
        // Selezione multipla ammessa solo a parità di layer
        let layerFeatureSelezionata = feature.properties.layer;
        let layerPrimaFeatureSelezionata = this.featureService.getPrimaFeatureSelezionata().properties.layer;
        if (layerFeatureSelezionata !== layerPrimaFeatureSelezionata && this.modalitySubject.value !== GISModality.Trattamento && this.modalitySubject.value !== GISModality.AnalisiTerreno) {
          selezioneMultipla = false;
        }
      }
    }

    if (deselezionaSingolaFeature && this.disabilitaDeselezione) {
      return !deselezionaSingolaFeature;
    }

    if (deselezionaSingolaFeature) {
      if (feature.properties.id === this.featureService.getUltimaFeatureSelezionata().properties.id) {
        this.sharedDataService.setLastCheckedKeyFeatureId(null);
      }

      this.deselezionaFeature(feature, true, true, true, origineChiamata);

      // Aggiorno coordinate toolbar
      if (origineChiamata === enum_OrigineChiamata.Mappa) {
        let limitiFeature = this.determinaLimitiGruppoFeature(true);
        let centroFeature = limitiFeature.getCenter();
        this.gisToolbarService.setLatLng(centroFeature.lat(), centroFeature.lng());
      }

    } else {
      if (!selezioneMultipla) {
        // Imposto non modificabili le eventuali feature selezionate in precedenza
        this.seDeselezionaFeatureSelezionate(origineChiamata);
      }

      // Aggiorna feature selezionata
      this.featureService.setOrigineChiamata(origineChiamata)
      this.featureService.addFeatureSelezionata(feature);

      // Imposto modificabile o selezionato la feature selezionata attualmente
      const permessiFeature = this.featureService.getPermessiFeature(feature);
      if (permessiFeature.modifica && this.modality == GISModality.Full) {
        // this.googleMapWrapper.data.overrideStyle(feature, this.stileModificabile(feature));
        this.googleMapDataService.overrideFeatureStyle(feature, this.stileModificabile(feature));
      } else {
        // this.googleMapWrapper.data.overrideStyle(feature, this.stileSelezionato(feature));
        this.googleMapDataService.overrideFeatureStyle(feature, this.stileSelezionato(feature));
      }

      // Aggiorno coordinate toolbar
      if (origineChiamata === enum_OrigineChiamata.Mappa) {
        let limitiFeature = this.determinaLimitiGruppoFeature(true);
        let centroFeature = limitiFeature.getCenter();
        this.gisToolbarService.setLatLng(centroFeature.lat(), centroFeature.lng());
      }

      const layerSelezionato: string = feature.properties.layer;

      //----------------------------------------------------------------------------------------------------
      // Commentato: la funzione toggleLayerItemSelected scatena già sul GIS.service tale effetto
      //----------------------------------------------------------------------------------------------------
      // Porto in primo piano le feature dello stesso layer
      // this.settaLayerDoveDisegnare(layerSelezionato);
      //----------------------------------------------------------------------------------------------------

      // Comunico alla finestra layer quello che è stato selezionato
      let tipologiaLayerSelezionato = this.sharedDataService.getTipologiaLayerById(layerSelezionato);
      if (tipologiaLayerSelezionato != null) {
        this.layerService.toggleLayerItemSelected(tipologiaLayerSelezionato, true);
      }

    }

    return !deselezionaSingolaFeature
  }

  //====================================================================================================
  // FUNZIONI UTILITA' VARIA
  //====================================================================================================

  public inserisciFeatureConAttributi(datiFeature: DatiFeatureConAttributi): Observable<any> {

    // Feature
    const idFeature = this.funzioniComuniService.getIdFeature(
      this.masterService.objP_server.PivaSuperUser,
      datiFeature.EntitaCod.toString(),
      this.sharedDataService.getTipoLayerSelezionato(),
      datiFeature.LayerElementiGraficiCod.toString()
    );

    // Proprietà
    const valoreTrue = 'True';
    const layerDiAppartenenzaDes = this.sharedDataService.getLayerSelezionato().nome;
    const layerZindex = this.sharedDataService.getLayerSelezionato().zindex;
    const objProprieta: GeoJSONAgroGisProp = {
      layer: datiFeature.LayerElementiGraficiCod.toString(),
      StandardEntita_layerDiAppartenenza: datiFeature.LayerElementiGraficiCod.toString(),
      StandardEntita_layerDiAppartenenza_Des: layerDiAppartenenzaDes,
      StandardEntita_layerDiAppartenenza_Icona32: '',
      ParametriVisualizzazioneLayer: '',
      id: idFeature,
      tipoicona: '',
      zindex: layerZindex,
      Entita_Cod: datiFeature.EntitaCod.toString(),
      veg_cod: '-1',
      flag_gps: datiFeature.FlagGps.toString(),
      etichetta: '',
      inserimento: valoreTrue,
      modifica: valoreTrue,
      cancellazione: valoreTrue,
      informazioni: valoreTrue,
      chiavealbero: '',
      Testo: datiFeature.ElementoGraficoDes.search('§') == -1 ? datiFeature.ElementoGraficoDes : '',
      AppIdRate: datiFeature.ElementoGraficoDes.search('§') == -1 ? '' : datiFeature.ElementoGraficoDes,
      TipologiaGML: datiFeature.FeatureGeometryType,
      InOsservazione: 0,
      Colore_Primario: '',
      Colore_Retinatura: '',
      Trasparenza: 0,
      Area_Cod: 0,
      Entita_GUID: '',
      GMapsZoomLevel: 0,
      TotalOriginalArea: 0,
      TotalOriginalFeatureNumber: 0,
      Clustered: 'false'
    };

    // Inserimento feature e aggiornamento label
    return this.gisClient
      .gisLeggiChiaveAlbero(datiFeature.EntitaCod)
      .pipe(map(res => {
        const chiaveAlberoCompleta = res.RispostaStringa;
        const chiaveAlbero = FunzioniComuniService.chiaveAlberoBigToRidotta(chiaveAlberoCompleta);
        this.addFeatureFromFeatureOptions({
          geometry: datiFeature.GoogleMapsDataGeometry,
          id: idFeature,
          properties: { ...objProprieta, chiavealbero: chiaveAlbero }
        } as google.maps.Data.FeatureOptions);

        this.selezionaFeatureByChiaveAlbero(chiaveAlberoCompleta, true, false);
        return chiaveAlbero;
      }));
  }

  public addFeatureFromFeatureOptions(featureOptions: google.maps.Data.FeatureOptions): void {
    const geometry = featureOptions.geometry as google.maps.Data.Geometry;
    const type = geometry.getType();
    const coordinates = GeoJsonUtils.getGeoJsonGeometryFromGeometry(type, geometry);
    const geoJsonFeature = {
      geometry: {
        coordinates: coordinates,
        type: type
      },
      properties: featureOptions.properties,
      type: 'Feature'
    } as GeoJson_Feature_New_1OfGeoJSONAgroGisProp;

    this.geoJsonLazyService.addFeature(geoJsonFeature, this.googleMapWrapper.data.getMap());
  }

  public modificaFeatureConAttributi(datiFeature: DatiFeatureConAttributi): Observable<any> {
    const f = datiFeature.FeatureModified;
    if (f == null) {
      return;
    }

    const objElementoGraficoDes = this.getObjElementoGraficoDes(datiFeature.ElementoGraficoDes);

    let objProprieta: GeoJSONAgroGisProp = {
      layer: f.getProperty(enum_FeatureProperty.layer) as string,
      StandardEntita_layerDiAppartenenza: f.getProperty(enum_FeatureProperty.layerAppartenenza) as string,
      StandardEntita_layerDiAppartenenza_Des: f.getProperty(enum_FeatureProperty.layerAppartenenzaDes) as string,
      StandardEntita_layerDiAppartenenza_Icona32: f.getProperty(enum_FeatureProperty.layerAppartenenzaIcona32) as string,
      id: f.getId().toString(),
      ParametriVisualizzazioneLayer: f.getProperty(enum_FeatureProperty.parametriVisualizzazioneLayer) as string,
      tipoicona: f.getProperty(enum_FeatureProperty.tipoIcona) as string,
      zindex: f.getProperty(enum_FeatureProperty.zIndex) as string,
      Entita_Cod: f.getProperty(enum_FeatureProperty.entitaCod) as string,
      veg_cod: f.getProperty(enum_FeatureProperty.vegCod) as string,
      flag_gps: f.getProperty(enum_FeatureProperty.flagGps) as string,
      etichetta: f.getProperty(enum_FeatureProperty.etichetta) as string,
      inserimento: f.getProperty(enum_FeatureProperty.inserimento) as string,
      modifica: f.getProperty(enum_FeatureProperty.modifica) as string,
      cancellazione: f.getProperty(enum_FeatureProperty.cancellazione) as string,
      informazioni: f.getProperty(enum_FeatureProperty.informazioni) as string,
      chiavealbero: f.getProperty(enum_FeatureProperty.chiaveAlbero) as string,
      Testo: objElementoGraficoDes.Testo,
      AppIdRate: objElementoGraficoDes.AppIdRate,
      TipologiaGML: f.getProperty(enum_FeatureProperty.tipologiaGML) as string,
      InOsservazione: f.getProperty(enum_FeatureProperty.inOsservazione) as number,
      Colore_Primario: f.getProperty(enum_FeatureProperty.colorePrimario) as string,
      Colore_Retinatura: f.getProperty(enum_FeatureProperty.coloreRetinatura) as string,
      Trasparenza: f.getProperty(enum_FeatureProperty.trasparenza) as number,
      Area_Cod: f.getProperty(enum_FeatureProperty.Area_Cod) as number,
      Entita_GUID: f.getProperty(enum_FeatureProperty.entitaGuid) as string,
      GMapsZoomLevel: f.getProperty(enum_FeatureProperty.GMapsZoomLevel) as number,
      TotalOriginalArea: f.getProperty(enum_FeatureProperty.TotalOriginalArea) as number,
      TotalOriginalFeatureNumber: f.getProperty(enum_FeatureProperty.TotalOriginalFeatureNumber) as number,
      Clustered: f.getProperty(enum_FeatureProperty.Clustered) as string
    };

    // Opzioni
    let opzioniFeatureConAttributi: google.maps.Data.FeatureOptions = {
      geometry: datiFeature.GoogleMapsDataGeometry,
      id: objProprieta.id,
      properties: objProprieta
    };

    // Inserimento feature e aggiornamento label
    return of(this.addFeatureFromFeatureOptions(opzioniFeatureConAttributi));
  }

  private getObjElementoGraficoDes(elementoGraficoDes: string): { Testo: string; AppIdRate: string } {
    let objElementoGraficoDes = {
      Testo: '',
      AppIdRate: ''
    }

    if (elementoGraficoDes?.includes('§')) {
      objElementoGraficoDes.AppIdRate = elementoGraficoDes;
    } else {
      objElementoGraficoDes.Testo = elementoGraficoDes;
    }

    return objElementoGraficoDes;
  }

  public inserisciFeatureIncollata(
    featureDaCopiare: GeoJson_Feature_New_1OfGeoJSONAgroGisProp,
    objTreeNodeSelezionato: objTreeNode,
    piva: string,
    entitaCod: string
  ) {
    const nodoSelezionato = objTreeNodeSelezionato.treeNode;
    const padreNodoSelezionato = objTreeNodeSelezionato.treeNodeParent;

    let objChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(nodoSelezionato.id);
    let layerChiaveAlbero = '';
    if (objChiaveAlbero.TipoNodo === enum_TipoNodo.Appezzamento) {
      layerChiaveAlbero = enum_LayerElementiGraficiStd.APPEZZAMENTI;
    } else {
      layerChiaveAlbero = enum_LayerElementiGraficiStd.IMPIANTI;
    }


    // Feature
    const idFeature = this.funzioniComuniService.getIdFeature(
      piva,
      entitaCod,
      this.sharedDataService.getTipoLayerSelezionato(),
      layerChiaveAlbero
    );

    // Proprietà
    let layerDiAppartenenzaDes = '';
    let etichettaEstrapolata = '';
    if (objChiaveAlbero.TipoNodo === enum_TipoNodo.Appezzamento) {
      layerDiAppartenenzaDes = this.appezzamentiDes
      etichettaEstrapolata = this.getEtichettaAppezzamentoDaTestoNodo(nodoSelezionato.text);
    } else {
      layerDiAppartenenzaDes = this.impiantiDes
      etichettaEstrapolata = this.getEtichettaAppezzamentoDaTestoNodo(padreNodoSelezionato.text);
    }
    let objProprieta: GeoJSONAgroGisProp = {
      layer: layerChiaveAlbero,
      StandardEntita_layerDiAppartenenza: layerChiaveAlbero,
      StandardEntita_layerDiAppartenenza_Des: layerDiAppartenenzaDes,
      StandardEntita_layerDiAppartenenza_Icona32: '',
      ParametriVisualizzazioneLayer: '',
      id: idFeature,
      tipoicona: '',
      zindex: featureDaCopiare.properties.zindex,
      Entita_Cod: entitaCod,
      veg_cod: '-1', // TODO Andrea (2): valutare con Ricky introduzione dato in albero
      flag_gps: featureDaCopiare.properties.flag_gps,
      etichetta: etichettaEstrapolata,
      inserimento: featureDaCopiare.properties.inserimento,
      modifica: featureDaCopiare.properties.modifica,
      cancellazione: featureDaCopiare.properties.cancellazione,
      informazioni: featureDaCopiare.properties.informazioni,
      chiavealbero: FunzioniComuniService.chiaveAlberoBigToRidotta(nodoSelezionato.id),
      Testo: '',
      AppIdRate: '',
      TipologiaGML: featureDaCopiare.properties.TipologiaGML,
      InOsservazione: featureDaCopiare.properties.InOsservazione,
      Colore_Primario: '',
      Colore_Retinatura: '',
      Trasparenza: 0,
      Area_Cod: 0,
      Entita_GUID: '',
      GMapsZoomLevel: 0,
      TotalOriginalArea: 0,
      TotalOriginalFeatureNumber: 0,
      Clustered: 'false'
    }

    // Geometria
    let featureGeometry = this.featureInformationService.getGeometry(featureDaCopiare.properties.id);

    // Opzioni
    let opzioniFeatureIncollata: google.maps.Data.FeatureOptions = {
      geometry: featureGeometry,
      id: idFeature,
      properties: objProprieta
    }

    // Inserimento feature e aggiornamento label
    this.addFeatureFromFeatureOptions(opzioniFeatureIncollata);

    // Porto in primo piano il layer chiave albero
    this.googleMapService.loaded$
      .pipe(take(1))
      .subscribe(() => this.settaLayerDoveDisegnare(layerChiaveAlbero));
  }

  private getEtichettaAppezzamentoDaTestoNodo(testoNodo: string): string {
    let inizDes = testoNodo.indexOf('} : ');
    let fineDes = testoNodo.indexOf(' : {');
    return testoNodo.substring(inizDes + 4, fineDes);
  }

  public selezionaFeatureByChiaveAlbero(
    chiaveAlbero: string,
    seleziona: boolean,
    selezioneMultipla: boolean
  ) {
    let idFeatureSelezionata: any = null;
    let deselezionataFeature: boolean = false;

    if (chiaveAlbero.length > 0) {

      let opDone: boolean = false;
      this.featureInformationService.getAll().forEach(feature => {
        const chiaveAlberoFeatureCompleta = this.leggiChiaveAlberoFeatureCompleta(feature);
        if (chiaveAlberoFeatureCompleta === chiaveAlbero) {
          opDone = true;
          if (seleziona) {
            this.selezionaFeature(feature, selezioneMultipla, false, enum_OrigineChiamata.Albero);
            idFeatureSelezionata = feature.properties.id;
          } else {
            this.deselezionaFeature(feature, true, true, false, enum_OrigineChiamata.Albero);
            deselezionataFeature = true;
          }
          this.impostaCentroMappaDaFeature(true, true);
        }
      });
      if (!opDone) this.seDeselezionaFeatureSelezionate(enum_OrigineChiamata.Albero);
    }

    if (seleziona) {
      this.sharedDataService.setLastCheckedKeyFeatureId(idFeatureSelezionata);
    } else {
      if (deselezionataFeature) {
        let idUltimaFeatureSelezionata = null;
        const ultimaFeatureSelezionata = this.featureService.getUltimaFeatureSelezionata();
        if (ultimaFeatureSelezionata) {
          idUltimaFeatureSelezionata = ultimaFeatureSelezionata.properties.id;
        }
        this.sharedDataService.setLastCheckedKeyFeatureId(idUltimaFeatureSelezionata);
      }
    }
  }

  public selezionaFeatureById(
    idFeature: string,
    seleziona: boolean,
    selezioneMultipla: boolean,
    centraMappa: boolean = true
  ) {
    let deselezionaFeature: boolean = false;

    if (idFeature != undefined && idFeature != '') {
      const feature = this.featureInformationService.getById(idFeature);
      if (seleziona) {
        this.selezionaFeature(feature, selezioneMultipla, false, enum_OrigineChiamata.Mappa);
      } else {
        this.deselezionaFeature(feature, true, true, false, enum_OrigineChiamata.Mappa);
        deselezionaFeature = true;
      }
      if (centraMappa) {
        this.impostaCentroMappaDaFeature(true, true);
      }
    }

    if (seleziona) {
      this.sharedDataService.setLastCheckedKeyFeatureId(idFeature);
    } else {
      if (deselezionaFeature) {
        let idUltimaFeatureSelezionata = null;
        const ultimaFeatureSelezionata = this.featureService.getUltimaFeatureSelezionata();
        if (ultimaFeatureSelezionata) {
          idUltimaFeatureSelezionata = ultimaFeatureSelezionata.properties.id;
        }
        this.sharedDataService.setLastCheckedKeyFeatureId(idUltimaFeatureSelezionata);
      }
    }
  }

  public getFeatureByChiaveAlbero(chiaveAlbero: string): Feature {
    let featureNulla: Feature = null;

    if (chiaveAlbero.length > 0) {
      this.featureInformationService.getAll().forEach(feature => {
        const chiaveAlberoFeatureCompleta = this.leggiChiaveAlberoFeatureCompleta(feature);
        if (chiaveAlberoFeatureCompleta === chiaveAlbero) {
          return feature;
        }
      });
    }

    return featureNulla;
  }

  private leggiChiaveAlberoFeatureCompleta(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): string {
    let chiaveAlberoFeature: string = feature.properties.chiavealbero;
    return FunzioniComuniService.chiaveAlberoRidottaToBig(chiaveAlberoFeature);
  }

  private inizializzaVariabiliMappa() {
    this.maxZIndex = enum_zIndex.inizializzazionePrimoPianoFeature;
    this.featureService.inizializzaFeatureSelezionate();
    this.featureService.inizializzaFeatureDaCopiare();
    this.chiudiCreaFinestraInformativa();
    this.indicatoreAppoggio = new google.maps.Marker({
      map: this.googleMapWrapper.data.getMap(),
      draggable: false,
      visible: false
    });
    this.dataLayerStyleService.setIconaPuntoDeselezionato();
    this.dataLayerStyleService.setIconaPuntoSelezionato();
  }

  private chiudiCreaFinestraInformativa() {
    if (this.finestraInformativa) {
      this.finestraInformativa.close();
    }
    this.finestraInformativa = new google.maps.InfoWindow();
  }

  public handleEditPolygon(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, isSementieri: boolean): void {
    this.polygonLabelInfoWindowService.catchPolygonChange(feature);
    const googleFeature = this.googleMapWrapper.data.getFeatureById(feature.properties.id);
    if (googleFeature != null) {
      const gMap = this.googleMapWrapper.data.getMap();
      if (!this.kendoWindowsService.getOpenState(WindowTypes.AnalisiMappeSatellitariWindow)) {
        this.editFeatureWindowService?.update(googleFeature, gMap, isSementieri);
      }
    }
  }

  public impostaCentroMappaByLayer(layerId: string) {
    const latlngAutoFit: google.maps.LatLngBounds = this.determinaLimitiFeatureLayer(layerId);

    if (latlngAutoFit) {
      const centroMappa: google.maps.LatLng = latlngAutoFit.getCenter()
      this.googleMapWrapper.data.getMap().setCenter(centroMappa);
      this.googleMapWrapper.data.getMap().fitBounds(latlngAutoFit);
    } else {
      this.giasMessageService.infoMessagge(this.transloco.translate('gis.NessunDatoTrovatoLayer'));
    }
  }

  private determinaLimitiFeatureLayer(layerId: string): google.maps.LatLngBounds {
    const features = this.featureInformationService.getByLayer(layerId);
    if (features.length == 0) {
      return null;
    }

    const isRaster = this.sharedDataService.getTipologiaLayerById(layerId)?.FeatureTypeId === FeatureType.Raster.toString();
    if (isRaster) {
      // Since raster is often represented by a single, huge feature
      // Use all the points in this feature to compute the area
      const rasterBounds = new google.maps.LatLngBounds();
      features.forEach(feature => {
        const geometry = this.featureInformationService.getGeometry(feature.properties.id);
        geometry.forEachLatLng(x => rasterBounds.extend(x));
      });
      return rasterBounds;
    }

    const latlngAutoFit = new google.maps.LatLngBounds();
    features.forEach(feature => {
      const position = this.featureInformationService.getPosition(feature.properties.id);
      latlngAutoFit.extend(position);
    });

    return latlngAutoFit;
  }

  public impostaCentroMappaDaFeature(
    consideraSoloFeatureSelezionate: boolean,
    aggiornaCoordinateToolbar: boolean
  ) {
    const latlngAutoFit = this.determinaLimitiGruppoFeature(consideraSoloFeatureSelezionate);
    const centroMappa = latlngAutoFit.getCenter()

    this.googleMapWrapper.data.getMap().setCenter(centroMappa);
    this.googleMapWrapper.data.getMap().fitBounds(latlngAutoFit);

    if (aggiornaCoordinateToolbar) {
      this.gisToolbarService.setLatLng(centroMappa.lat(), centroMappa.lng());
    }
  }

  public impostaCentroMappaDaGeometrie(): void {
    const [found, latlngAutoFit] = this.determinaLimitiGruppoAnalisiTerreno();
    if (!found) {
      return;
    }

    const centroMappa = latlngAutoFit.getCenter()

    this.googleMapWrapper.data.getMap().setCenter(centroMappa);
    this.googleMapWrapper.data.getMap().fitBounds(latlngAutoFit);
    this.gisToolbarService.setLatLng(centroMappa.lat(), centroMappa.lng());
  }

  determinaLimitiGruppoFeature(consideraSoloFeatureSelezionate: boolean): google.maps.LatLngBounds {
    let latlngAutoFit = new google.maps.LatLngBounds();
    if (consideraSoloFeatureSelezionate && this.featureService.esistonoFeatureSelezionate()) {
      const featureSelezionate = this.featureService.getFeatureSelezionate();
      featureSelezionate.forEach(feature => {
        const position = this.featureInformationService.getPosition(feature.properties.id);
        latlngAutoFit.extend(position);
      });
    } else {
      this.featureInformationService.getAll().forEach(feature => {
        const position = this.featureInformationService.getPosition(feature.properties.id);
        latlngAutoFit.extend(position);
      });
    }

    return latlngAutoFit;
  }

  determinaLimitiGruppoAnalisiTerreno(): [boolean, google.maps.LatLngBounds] {
    let found = false;
    let latlngAutoFit = new google.maps.LatLngBounds();
    const allFeaturesByChiaveAlbero: [string, string][] = [];
    this.featureInformationService.getAll().forEach(feature => {
      const chiaveAlberoCompleta = FunzioniComuniService.chiaveAlberoRidottaToBig(feature.properties.chiavealbero);
      allFeaturesByChiaveAlbero.push([chiaveAlberoCompleta, feature.properties.id]);
    });

    const featureSelezionate = this.gisGeometrySelectionService.getCurrentSelection();
    featureSelezionate.filter(([_, selected]) => selected).forEach(([chiaveAlbero, _]) => {
      const id = allFeaturesByChiaveAlbero.find(x => x[0].includes(chiaveAlbero))?.[1];
      if (id == null) {
        return;
      }

      const position = this.featureInformationService.getPosition(id);
      if (position != null) {
        latlngAutoFit.extend(position);
        found = true;
      }
    });

    return [found, latlngAutoFit];
  }

  public setMapCenterOfPassedFeatures(ids: string[]) {
    const latlngAutoFit = this.determineFeaturesGroupLimit(ids);
    const centroMappa = latlngAutoFit.getCenter();

    this.googleMapWrapper.data.getMap().setCenter(centroMappa);
    this.googleMapWrapper.data.getMap().fitBounds(latlngAutoFit);
  }

  private determineFeaturesGroupLimit(ids: string[]): google.maps.LatLngBounds {
    const latlngAutoFit: google.maps.LatLngBounds = new google.maps.LatLngBounds();
    ids.forEach(id => {
      const position = this.featureInformationService.getPosition(id);
      latlngAutoFit.extend(position);
    });

    return latlngAutoFit;
  }

  public seDeselezionaFeatureSelezionate(origineChiamata: enum_OrigineChiamata) {
    if (this.featureService.esistonoFeatureSelezionate()) {
      const featureSelezionate = this.featureService.getFeatureSelezionate();
      featureSelezionate.forEach((featureSelezionata) => {
        this.deselezionaFeature(featureSelezionata, false, false, true, origineChiamata);
      });
      this.finestraInformativa.close();
      this.featureService.inizializzaFeatureSelezionate();
    }
  }

  public deselezionaFeature(
    feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp,
    rimuoviDaSelezionate: boolean,
    chiudiFinestraInformativa: boolean,
    rimuoviDaMappa: boolean,
    origineChiamata: enum_OrigineChiamata
  ) {
    if (rimuoviDaSelezionate && this.featureService.esistonoFeatureSelezionate()) {
      this.featureService.setOrigineChiamata(origineChiamata)
      this.featureService.removeFeatureSelezionata(feature);
    }

    if (chiudiFinestraInformativa) {
      this.finestraInformativa.close();
    }

    this.googleMapDataService.overrideFeatureStyle(feature, this.stileNonModificabile(feature));

    if (rimuoviDaMappa) {
      if (this.modalitySubject.value == GISModality.Trattamento || this.modalitySubject.value == GISModality.AnalisiTerreno) {
        const chiaveAlberoFeatureCompleta = this.leggiChiaveAlberoFeatureCompleta(feature);
        this.gisGeometrySelectionService.removeFromMap(chiaveAlberoFeatureCompleta);
      }
    }
  }

  private stilePrimoPiano(): google.maps.Data.StyleOptions {
    return {
      zIndex: this.maxZIndex
    };
  }

  private stileNonModificabile(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): google.maps.Data.StyleOptions {
    let layer = feature.properties.layer;

    let coloreTema = this.getColoreTemaFeaturePerLayer(layer, feature);

    let icona = this.getIconaNonModificabile(feature, layer, coloreTema);
    this.overrideAvversitaLayerStyle(feature, icona);

    return this.dataLayerStyleService.getDataLayerStyle(feature, coloreTema);
  }

  private getColoreTemaFeaturePerLayer(
    layer: string,
    feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp
  ): string {
    let coloreTema = null;
    const temaSelezionato = this.sharedDataService.getTemaSelezionato();
    if (this.isTemaApplicabile(layer, temaSelezionato)) {
      const layerSelezionato = this.sharedDataService.getLayerSelezionato();
      coloreTema = this.sharedDataService.getColoreTemaFeature(layerSelezionato.nome,
        temaSelezionato.nome,
        feature);
    }
    return coloreTema;
  }

  private isTemaApplicabile(layer: string, temaSelezionato: TemaSelezionato | null): boolean {
    if (temaSelezionato == null) {
      return false;
    }
    return layer === temaSelezionato.layerId && temaSelezionato.applicaTema
  }

  private getIconaNonModificabile(
    feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp,
    layer: string,
    coloreTema: string
  ): any {

    let icona = null;

    if (feature.geometry.type === enum_FeatureGeometryType.Point) {
      icona = this.dataLayerStyleService.getIconaPuntoDeselezionato()
      if (coloreTema) {
        icona.strokeColor = coloreTema;
      } else {
        let options = this.layerStyleService.getPolygonLayerStyle(layer);
        icona.strokeColor = options.strokeColor;
      }
    }

    return icona;

  }

  stileModificabile(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): google.maps.Data.StyleOptions {
    let trascinabile = false;
    let icona = null;

    if (feature.geometry.type === enum_FeatureGeometryType.Point) {
      trascinabile = true;
      icona = this.dataLayerStyleService.getIconaPuntoSelezionato();
      icona.fillColor = this.layerStyleService.getLayerColorAndOpacity(feature.properties.layer).fillColor;
      icona.strokeColor = this.layerStyleService.getLayerColorAndOpacity(feature.properties.layer).strokeColor;
      this.overrideAvversitaLayerStyle(feature, icona);
    }

    const layer = feature.properties.layer;
    const opacity = this.sharedDataService.getTrasparenzaLayer(layer);
    const isGoogleGridVisible = this.sharedDataService.getCfgAlberoGisUtente()[0].CfgGisUtente.ckGrigliaTiles_Sviluppo;
    const isRaster = this.sharedDataService.getTipologiaLayerById(feature.properties.layer)?.FeatureTypeId === FeatureType.Raster.toString();
    const isFeatureMaskerable = this.gisRasterConfigurationWindowService?.isFeatureMaskerable(+layer);
    const isAnimation = this.kendoWindowsService.getOpenState(WindowTypes.AnalisiMappeSatellitariWindow);
    const fillOpacity = (isGoogleGridVisible && isRaster) || isFeatureMaskerable || isAnimation ? 0 : opacity;

    return {
      editable: !isRaster,
      draggable: trascinabile,
      icon: icona,
      fillColor:
        feature.geometry.type === enum_FeatureGeometryType.Point ?
          this.layerStyleService.getLayerColorAndOpacity(feature.properties.layer).fillColor :
          'gray',
      fillOpacity: fillOpacity
    };

  }

  stileSelezionato(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): google.maps.Data.StyleOptions {
    let icona = null;

    if (feature.geometry.type === enum_FeatureGeometryType.Point) {
      icona = this.dataLayerStyleService.getIconaPuntoSelezionato();
      icona.fillColor = this.layerStyleService.getLayerColorAndOpacity(feature.properties.layer).fillColor;
      icona.strokeColor = this.layerStyleService.getLayerColorAndOpacity(feature.properties.layer).strokeColor;
      this.overrideAvversitaLayerStyle(feature, icona);
    }

    const layer = feature.properties.layer;
    const opacity = this.sharedDataService.getTrasparenzaLayer(layer);
    const isFeatureMaskerable = this.gisRasterConfigurationWindowService?.isFeatureMaskerable(+layer);
    const isAnimation = this.kendoWindowsService.getOpenState(WindowTypes.AnalisiMappeSatellitariWindow);
    const fillOpacity = isFeatureMaskerable || isAnimation ? 0 : opacity;

    return {
      editable: false,
      draggable: false,
      icon: icona,
      fillColor:
        feature.geometry.type === enum_FeatureGeometryType.Point ?
          this.layerStyleService.getLayerColorAndOpacity(feature.properties.layer).fillColor :
          'gray',
      fillOpacity: fillOpacity
    };

  }

  impostaZoomMinimoVisualizzazioneTotale() {
    if (!this.sharedDataService.getCfgSementiAsValue()) {
      let iAutoZoomSuVisualizzazioneTotale = this.sharedDataService.getCfgAlberoGisUtente()[0].CfgGisUtente.iAutoZoomSuVisualizzazioneTotale;
      if (iAutoZoomSuVisualizzazioneTotale !== undefined) {
        if (iAutoZoomSuVisualizzazioneTotale !== enum_zoomVisualizzazioneTotale.no) {
          this.impostaZoomMinimo(iAutoZoomSuVisualizzazioneTotale);
        }
      }
    }
  }

  impostaZoomMinimoVisualizzazioneWms() {
    this.impostaZoomMinimo(enum_zoom.minimoVisualizzazioneWms);
  }

  impostaZoom(livelloZoom: number) {
    const mappa = this.googleMapWrapper.data.getMap();
    mappa.setZoom(livelloZoom);
  }

  private impostaZoomMinimo(zoomMinimo: number) {
    const mappa = this.googleMapWrapper.data.getMap();
    if (mappa.getZoom() < zoomMinimo) {
      mappa.setZoom(zoomMinimo);
    }
  }

  getPolygonWktFromBoundsMappa(): string {
    const mappa = this.googleMapWrapper.data.getMap();
    return GoogleMapUtils.getMapBoundsWKT(mappa.getBounds());
  }

  loadFiltroTemporale() {
    // Parametri lettura
    let readParameters: LeggiDateImpiantiPerFiltroTemporale_In = this.prepareReadParameters_TempFilter();
    const objParams = this.objParametriAgendaService.getObjParamValue();
    if (objParams.QueryStringFiltrino != null &&
      objParams.QueryStringFiltrino != '' &&
      SementieriParametrizzazione.isInstanceOfSementieri(JSON.parse(objParams.QueryStringFiltrino))
    ) {
      // in caso di sementieri non carico il filtro temporale
      return Promise.resolve(true);
    }

    return new Promise((resolve, reject) => {
      combineLatest([this.gisClient.gisLeggiDateImpiantiFiltroTemporale(readParameters), this.googleMapService.loaded$])
        .pipe(takeUntil(this.signal))
        .subscribe(([r, _]) => {
          if (r.RispostaOK) {
            let dataElaborazione = r.RispostaStringa;
            this.sharedDataService.setCustomTimeFilter(dataElaborazione, dataElaborazione);

            // Alert errore in caso di mancanza di dati/impianti per l'azienda
            if (r.Errore) {
              this.giasDialogService.alertMessage(r.Errore)
            }
          }

          resolve(r.RispostaOK && !r.Errore);
        });
    });
  }

  prepareReadParameters_TempFilter(): LeggiDateImpiantiPerFiltroTemporale_In {
    let gt_params: LeggiDateImpiantiPerFiltroTemporale_In = {};

    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    if (this.objParametriAgenda != null) {
      // Piva
      gt_params.piva = this.objParametriAgenda.Piva;

      // SaCod
      gt_params.sa_cod = this.objParametriAgenda.Sa_Cod.toString();

      // Data
      gt_params.date = new Date();
    }

    return gt_params;
  }

  //====================================================================================================
  // FUNZIONI FINESTRA LAYER
  //====================================================================================================

  //----------------------------------------------------------------------------------------------------
  // Porta un layer in primo piano
  //----------------------------------------------------------------------------------------------------

  public settaLayerDoveDisegnare(layer: string) {
    this.incrementaIndicePrimoPiano();
    this.featureInformationService.getAll().forEach(f => {
      let layerMappa = f.properties.layer;
      if (layerMappa === layer) {
        // this.googleMapWrapper.data.overrideStyle(f, this.stilePrimoPiano());
        this.googleMapDataService.overrideFeatureStyle(f, this.stilePrimoPiano());
      }
    });
  }

  private incrementaIndicePrimoPiano() {
    this.maxZIndex += 1;
  }

  public getCurrentZoomLevel(): number {
    return this.googleMapWrapper.data.getMap().getZoom();
  }

  //----------------------------------------------------------------------------------------------------
  // Visualizza o meno le feature/etichette di un layer
  //----------------------------------------------------------------------------------------------------
  public clearOverlays(layer: string, flagVisible: boolean) {
    if (!flagVisible) {
      // Do it always
      this.overrideVisibility(layer, flagVisible);
    } else {
      // Do only if not raster or if google grid is on
      const isRaster = this.sharedDataService.getTipologiaLayerById(layer)?.FeatureTypeId === FeatureType.Raster.toString();
      const isGoogleGridVisible = this.sharedDataService.getCfgAlberoGisUtente()[0]?.CfgGisUtente?.ckGrigliaTiles_Sviluppo ?? false;
      if (!isRaster || isGoogleGridVisible) {
        this.overrideVisibility(layer, flagVisible);
      }
    }

    // necessario per il refresh della mapppa
    this.geoJsonLazyService.forceMapRefresh(this.googleMapWrapper.data.getMap());
  }

  private overrideVisibility(layer: string, flagVisible: boolean): void {
    this.featureInformationService.getAll().forEach(f => {
      if (f.properties.layer === layer) {
        // this.googleMapWrapper.data.overrideStyle(f, {
        //     visible: flagVisible
        // });
        this.googleMapDataService.overrideFeatureStyle(f, { visible: flagVisible });
      }
    });
  }

  public showHideAllFeature(
    flagVisible: boolean,
    elencoLayers: TipologiaLayer[]
  ) {
    this.featureInformationService.getAll().forEach(f => {
      let featureLayer = f.properties.layer;
      let layerIndex = elencoLayers.findIndex(layer => layer.id === featureLayer);
      if (layerIndex >= 0) {
        // this.googleMapWrapper.data.overrideStyle(f, {
        //     visible: flagVisible
        // });
        this.googleMapDataService.overrideFeatureStyle(f, { visible: flagVisible });
      }
    });
  }

  //----------------------------------------------------------------------------------------------------
  // Modifica i colori delle feature
  //----------------------------------------------------------------------------------------------------

  public changeColorLayer(layer: string, checkTema: boolean) {
    let coloreLayer = this.layerStyleService.getLayerColorAndOpacity(layer);
    const temaSelezionato = this.sharedDataService.getTemaSelezionato();
    this.featureInformationService.getAll().forEach(f => {
      const featureLayer = f.properties.layer;
      let temaApplicabile = false;
      if (checkTema) {
        temaApplicabile = this.isTemaApplicabile(layer, temaSelezionato);
      }
      if (featureLayer === layer && !temaApplicabile) {
        switch (f.geometry.type) {

          case enum_FeatureGeometryType.LineString:
            // this.googleMapWrapper.data.overrideStyle(f, {
            //     strokeColor: coloreLayer.strokeColor,
            //     strokeWeight: (coloreLayer.fillOpacity * 5)
            // });
            this.googleMapDataService.overrideFeatureStyle(
              f,
              {
                strokeColor: coloreLayer.strokeColor,
                strokeWeight: coloreLayer.fillOpacity * 5
              }
            );
            break;

          case enum_FeatureGeometryType.Point:
            let icona = this.getIconaNonModificabile(f, layer, null);
            this.overrideAvversitaLayerStyle(f, icona);
            // this.googleMapWrapper.data.overrideStyle(f, {
            //     icon: icona
            // });
            this.googleMapDataService.overrideFeatureStyle(f, { icon: icona });
            break;

          default:
            // this.googleMapWrapper.data.overrideStyle(f, {
            //     fillColor: coloreLayer.fillColor,
            //     fillOpacity: coloreLayer.fillOpacity,
            //     strokeColor: coloreLayer.strokeColor
            // });
            this.googleMapDataService.overrideFeatureStyle(
              f,
              {
                fillColor: coloreLayer.fillColor,
                fillOpacity: coloreLayer.fillOpacity,
                strokeColor: coloreLayer.strokeColor
              });
            break;

        }
      }
    });
  }

  //====================================================================================================
  // FUNZIONI INFORMATIVE FEATURE
  //====================================================================================================

  public getProprietaFeatureSelezionata() {
    if (this.featureService.esistonoFeatureSelezionate()) {
      if (Number.parseInt(this.featureService.getUltimaFeatureSelezionata().properties.Entita_Cod) < 0)
        this.getFeaturePropertyLocal(this.featureService.getUltimaFeatureSelezionata());
      else
        this.getProprietaFeature(this.featureService.getUltimaFeatureSelezionata());
    }
  }

  public getProprietaFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp) {

    let getProprietaIn: GetProprieta_In = {};
    getProprietaIn.Entita_Cod = feature.properties.Entita_Cod;

    // TODO Andrea (2): gestire chiamata Tipo_GetProp = 1/3
    getProprietaIn.Tipo_GetProp = 2;

    this.gisClient.gisGetProprieta(getProprietaIn)
      .pipe(takeUntil(this.signal)).subscribe(r => {
      let contenuto = this.componiContenutoDaProprieta(getProprietaIn, r.RispostaStringa, feature);
      this.aggiornaFinestraInformativa(feature, contenuto)
    });

  }

  private getFeaturePropertyLocal(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp) {
    let msg: MessaggioFinestraInformativa = new MessaggioFinestraInformativa();
    let dataString = feature.properties.AppIdRate;
    dataString.split('|').forEach(d => {
      let dd = d.split('§');
      let field = dd[0];
      let value = dd[1];

      msg.seAggiungiRigaEtichettaValore(field, value);
      msg.aggiungiFineRiga();
    });

    this.aggiornaFinestraInformativa(feature, msg.contenuto);
  }

  private componiContenutoDaProprieta(
    proprietaIn: GetProprieta_In,
    proprietaOut: GetProprieta_Out,
    feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp
  ): string {

    let messaggio = new MessaggioFinestraInformativa;

    messaggio.seAggiungiRigaTitolo(proprietaOut.App_Nome);

    messaggio.seAggiungiRigaEtichettaValore(this.transloco.translate("Azienda"), proprietaOut.Rag_Soc);

    messaggio.seAggiungiRigaEtichettaValore(this.transloco.translate("Centro"), proprietaOut.Sa_Nome);

    if (proprietaOut.Extra_Info) {
      //--------------------------------------------------------------------------------
      // TODO Andrea (4): approfondire utilizzo class='supCondottaSingoloCentro' in Gis.css
      //--------------------------------------------------------------------------------
      // .supCondottaSingoloCentro {
      //     color: orange
      // }
      //--------------------------------------------------------------------------------
      messaggio.aggiungiRigaCorsivo(proprietaOut.Extra_Info);
      messaggio.aggiungiFineRiga();
    }

    switch (proprietaOut.Flag_GPS) {
      case "1":
        messaggio.aggiungiRigaSottolineato(this.transloco.translate("DatiRilevatiConGPS"));
        break;
      case "2":
        messaggio.aggiungiRigaSottolineato(this.transloco.translate("DatiImportatiDaFonteEsternaAGIAS"));
        break;
    }

    messaggio.seAggiungiRigaEtichettaValore(this.transloco.translate("SuperficieCatastaleAbbr"), proprietaOut.Superficie);

    let areaHa = GoogleMapGeoJsonService.calcolaAreaHa(this.featureInformationService.getArea(feature.properties.id));
    if (areaHa > 0) {
      messaggio.seAggiungiRigaEtichettaValore(this.transloco.translate("SupCalcolataGoogle"), areaHa.toString());
    }

    messaggio.seAggiungiRigaEtichettaValore(this.transloco.translate("Indirizzo"), proprietaOut.Indirizzo);

    messaggio.seAggiungiRigaEtichettaValore(this.transloco.translate("Lotto2"), proprietaOut.Progetto_Nome);

    messaggio.aggiungiFineRiga();

    const labelSpecie = this.transloco.translate('Specie');
    const labelVarieta = this.transloco.translate('Varietà');
    const labelTipologia = this.transloco.translate('Tipologia');
    const labelFinalita = this.transloco.translate('Finalità');
    const labelDestinazioneUso = this.transloco.translate('DestinazioneDUso');
    const labelValiditaInizio = this.transloco.translate('ValiditàInizio');
    const labelValiditaFine = this.transloco.translate('ValiditàFine');
    const labelMostraDettagli = this.transloco.translate('MostraDettagli');

    messaggio.seAggiungiRigaEtichettaValore(labelSpecie, proprietaOut.Veg_Des);

    messaggio.seAggiungiRigaEtichettaValore(labelVarieta, proprietaOut.Cul_Des);

    messaggio.seAggiungiRigaEtichettaValore(labelTipologia, proprietaOut.Grva_Des);

    messaggio.seAggiungiRigaEtichettaValore(labelFinalita, proprietaOut.Grfi_Des);

    messaggio.seAggiungiRigaEtichettaValore(labelDestinazioneUso, proprietaOut.Destinazione_Uso);

    messaggio.seAggiungiRigaEtichettaValore(labelValiditaInizio, proprietaOut.Validita_Inizio);

    messaggio.seAggiungiRigaEtichettaValore(labelValiditaFine, proprietaOut.Validita_Fine);

    if (proprietaOut.GiasPalm.Descrizione) {

      // TODO Andrea (3): capire come implementare richiamo funzione GiasPalmDettagli

      const GiasPalmDettagli = "GiasPalmDettagli";
      const argomentiGiasPalmDettagli = `"${proprietaOut.GiasPalm.Piva}",` +
        `"${proprietaOut.GiasPalm.Sa_Cod}",` +
        `"${proprietaOut.GiasPalm.Id}"`

      let datiGPS = `${proprietaOut.GiasPalm.Descrizione} `
      // + `(<span id='span${GiasPalmDettagli}' style='color: blue'`
      // + ` onclick = '${GiasPalmDettagli}(${argomentiGiasPalmDettagli})'`
      // + `>${labelMostraDettagli}</span>)`
      // + `<div style='display: none; height: 250px; overflow: scroll;' id="div${GiasPalmDettagli}"></div>`

      messaggio.seAggiungiRigaEtichettaValore("GPS", datiGPS);

    }

    messaggio.seAggiungiRigaEtichettaValore("GIS", proprietaIn.Entita_Cod);

    return messaggio.contenuto;

  }

  public static calcolaAreaHa(area: number): number {
    return Number.parseFloat((area / 10000).toFixed(7));
  }

  private aggiornaFinestraInformativa(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, contenuto: string) {

    let centroFeature = this.determinaCentroApprossimativoFeature(feature);

    // Aggiorna Marker
    this.indicatoreAppoggio.setPosition(centroFeature);

    // Aggiorna InfoWindow
    this.finestraInformativa.close();
    this.finestraInformativa.setContent('<div class="custom-infobox">' + contenuto + '</div>' + this.btnsFinestraInformativa);
    let infoWindowOpenOptions = { map: this.googleMapWrapper.data.getMap() }
    this.finestraInformativa.open(infoWindowOpenOptions, this.indicatoreAppoggio);

  }

  private determinaCentroApprossimativoFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): google.maps.LatLng {
    const Mappa = require('../../GiasJSLibraries/GIS-js-libraries/Mappa');
    let coordinateFeature = this.featureInformationService.getPath(feature.properties.id);
    let position: google.maps.LatLng = Mappa.polylabel(coordinateFeature);
    return position;
  }

  //====================================================================================================
  // FUNZIONI CANCELLAZIONE FEATURE
  //====================================================================================================

  cancellaFeature(featureCancellata: GeoJson_Feature_New_1OfGeoJSONAgroGisProp) {
    // Deselezione
    this.deselezionaFeature(featureCancellata, true, true, false, enum_OrigineChiamata.Mappa);
    // Rimozione da ultimo elemento selezionato sull'albero
    this.sharedDataService.setLastCheckedKeyFeatureId(null);
    // Rimozione effettiva feature
    // this.googleMapDataService.removeFeature(featureCancellata);
    this.geoJsonLazyService.removeFeature(featureCancellata, this.googleMapWrapper.data.getMap());
    // Sistemazione label & clusterer
    // this.setPolyLabelsFromGoogleMapData();
    this.polygonLabelInfoWindowService.deletePolylabel(featureCancellata);
  }

  public removeFeaturesOfSameLayer(layerId: string): void {
    this.featureInformationService.getByLayer(layerId).forEach(f => this.cancellaFeature(f));
  }

  //====================================================================================================
  // FUNZIONI INTERAZIONE QDC
  //====================================================================================================

  public handleSelectedGeometries(keys: [string, boolean][]): void {
    this.featureInformationService.getAll().forEach(f => {
      for (const [key, selected] of keys) {
        let chiaveAlbero = f.properties.chiavealbero;
        let chiaveAlberoCompleta = FunzioniComuniService.chiaveAlberoRidottaToBig(chiaveAlbero);
        if (!(chiaveAlberoCompleta as string).includes(key)) {
          continue;
        }

        if (selected) {
          this.selezionaFeature(f, true, false, enum_OrigineChiamata.Albero);
          continue;
        }

        this.deselezionaFeature(f, true, false, false, enum_OrigineChiamata.Albero);
      }
    });
  }

  //====================================================================================================
  // FUNZIONI INTERAZIONE AGENDA
  //====================================================================================================

  public changeAziendaByFeatures(features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]): void {
    let visualizzazioneTotale = this.sharedDataService.getVisualizzazioneTotale();
    if (visualizzazioneTotale) {
      return;
    }

    let agenda = this.objParametriAgendaService.getObjParamValue();

    if (features.length === 0) {
      if (this.filtroAgendaImpostato(agenda)) {
        agenda.Sa_Cod = 0;
        agenda.Campo_Cod = 0;
        agenda.Appezza = 0;
        agenda.Id_Reg = 0;
        this.objParametriAgendaService.changeObjParametriAgenda(agenda);
      }
      return;
    }

    const indiceUltimaFeature = features.length - 1;
    const chiaveAlbero = this.featureService.getChiaveAlberoCompletaByFeature(features[indiceUltimaFeature]);
    const objChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(chiaveAlbero);

    if (!this.funzioniComuniService.isTipoNodoImpianto(objChiaveAlbero.TipoNodo)) {
      return;
    }

    //const objAgenda = new ObjParametriAgenda();
    //agenda.Piva = agenda.Piva;

    // Non è consentito che la feature selezionata possa appartenere ad un'altra impresa,
    // anche per evitare di cambiare implicitamente ObjParametriAgenda.Piva

    // Parametri da feature
    agenda.Sa_Cod = objChiaveAlbero.Sa_Cod;
    agenda.Campo_Cod = objChiaveAlbero.Campo_Cod;
    agenda.Appezza = objChiaveAlbero.Appezza;
    agenda.Id_Reg = objChiaveAlbero.Id_Imp;
    agenda.Veg_Cod = 0; // Specie
    agenda.Id_Cod = 0; // Destinazione d'uso

    agenda.Impianti = [];

    const objImpianto = new ImpiantiAgendaNG();
    objImpianto.Piva = objChiaveAlbero.Piva;
    objImpianto.Sa_Cod = objChiaveAlbero.Sa_Cod;
    objImpianto.Appezza = objChiaveAlbero.Appezza;
    objImpianto.Id_Reg = objChiaveAlbero.Id_Imp;
    agenda.Impianti.push(objImpianto);

    if (this.isNewAgenda(agenda)) {
      // Cambio parametri agenda
      this.objParametriAgendaService.changeObjParametriAgenda(agenda);
      // Aggiornamento oggetto agenda locale
      this.changeCurrentAgenda(agenda);
      // Aggiorno filtri scheda agenda
      this.filtersService.reloadFromAgenda.next(true);
    }

  }

  private filtroAgendaImpostato(agenda: ObjParametriAgenda) {
    return agenda.Sa_Cod !== 0 ||
      agenda.Campo_Cod !== 0 ||
      agenda.Appezza !== 0 ||
      agenda.Id_Reg !== 0
  }

  private isNewAgenda(agenda: ObjParametriAgenda): boolean {
    return this.currentAgenda == null
      || this.currentAgenda.Piva != agenda.Piva
      || this.currentAgenda.Sa_Cod != agenda.Sa_Cod
      || this.currentAgenda.Campo_Cod != agenda.Campo_Cod
      || this.currentAgenda.Appezza != agenda.Appezza
      || this.currentAgenda.Id_Reg != agenda.Id_Reg;
  }

  private changeCurrentAgenda(agenda: ObjParametriAgenda) {
    this.currentAgenda.Piva = agenda.Piva
    this.currentAgenda.Sa_Cod = agenda.Sa_Cod
    this.currentAgenda.Campo_Cod = agenda.Campo_Cod
    this.currentAgenda.Appezza = agenda.Appezza
    this.currentAgenda.Id_Reg = agenda.Id_Reg;
  }

  //====================================================================================================
  // FUNZIONI STRUMENTO GPS
  //====================================================================================================

  public gestioneStrumentoGps(attivo: boolean) {
    if (attivo) {
      this.getLocation()
        .pipe(takeUntil(this.signal)).subscribe(position => {
        const posizioneGps = new google.maps.LatLng(
          position.coords.latitude,
          position.coords.longitude
        );
        this.creaIndicatoreGps(posizioneGps);
        this.googleMapWrapper.data.getMap().setCenter(posizioneGps);
        this.gisToolbarService.setLatLng(posizioneGps.lat(), posizioneGps.lng());

        if (this.googleMapWrapper.getZoom() < 15)
          this.googleMapWrapper.googleMap.setZoom(15);
      });
    } else {
      if (this.indicatoreGps) {
        this.indicatoreGps.setVisible(false);
      }
    }
  }

  getLocation(): Observable<any> {
    return new Observable(observer => {
      if (window.navigator && window.navigator.geolocation) {
        window.navigator.geolocation.getCurrentPosition(
          (position) => {
            observer.next(position);
            observer.complete();
          },
          (error) => observer.error(error)
        );
      } else {
        observer.error(this.transloco.translate('GeolocalizzazioneNonSupportata'));
      }
    });
  }

  creaIndicatoreGps(coordinateGps: google.maps.LatLng, cancellaAltri: boolean = false) {
    if (cancellaAltri && this.indicatoreGps) {
      this.indicatoreGps.setVisible(false);
    }

    this.indicatoreGps = new google.maps.Marker({
      map: this.googleMapWrapper.data.getMap(),
      draggable: false,
      visible: true,
      position: coordinateGps
    });
  }

  rimuoviIndicatoriGps() {
    if (this.indicatoreGps) {
      this.indicatoreGps.setVisible(false);
    }
  }

  //====================================================================================================
  // FUNZIONI CERCA INDIRIZZO
  //====================================================================================================

  ricercaIndirizzo(indirizzo: string) {

    this.geocoder = new google.maps.Geocoder();

    if (!indirizzo || trim(indirizzo) === '') {
      let messaggio = this.transloco.translate("gis.IndirizzoNonImpostato");
      this.giasDialogService.alertMessage(messaggio)
      return;
    }

    this.geocoder.geocode({ 'address': indirizzo }, function (results, status) {
      if (status === 'OK') {
        let map = this.googleMapWrapper.data.getMap();
        map.setCenter(results[0].geometry.location);
        let marker = new google.maps.Marker({
          map: map,
          position: results[0].geometry.location
        });
      } else {
        let messaggio = this.transloco.translate("gis.GeocodificaNonRiuscita");
        let errore = this.transloco.translate("Errore_");
        let formatoMessaggio = `${messaggio} (${errore}: ${status})`;
        this.giasDialogService.alertMessage(formatoMessaggio);
      }
    }.bind(this));
  }

  //====================================================================================================
  // FUNZIONI NAVIGA VERSO
  //====================================================================================================

  apriGoogleMaps(indirizzo: string): void {
    let destinazione: string;
    const latLng = this.gisToolbarService.getLatLngGradiDecimali();

    if (latLng != undefined) {
      destinazione = `${latLng.lat()} ${latLng.lng()}`;
    } else {
      destinazione = indirizzo;
    }

    if (destinazione != undefined && destinazione != '') {
      const url: string = `https://www.google.it/maps/dir//${destinazione}/`;
      window.open(url);
    } else {
      let messaggio: string = this.transloco.translate("gis.NessunaCoordinataIndirizzoImpostato");
      this.giasDialogService.alertMessage(messaggio);
    }
  }

  //====================================================================================================
  // FUNZIONI TEMATIZZAZIONI
  //====================================================================================================

  public applicaTematizzazioneLayer(layerSelezionato: TipologiaLayer, temaSelezionato: TemaSelezionato, scalaColori: DatiColoreTema[]): void {
    this.featureInformationService.getAll().forEach(feature => {
      const layerFeature = feature.properties.layer;
      if (layerFeature !== layerSelezionato.id) {
        return;
      }

      const isInvisible = this.isFeatureInvisible(layerSelezionato, temaSelezionato, feature, scalaColori);
      if (isInvisible) {
        this.handleInvisibleFeature(feature, layerFeature);
        return;
      }

      const coloreTemaFeature = this.sharedDataService.getColoreScalaFeature(feature, layerSelezionato.nome, temaSelezionato.nome, scalaColori);
      if (coloreTemaFeature) {
        let icona = this.getIconaNonModificabile(feature, layerFeature, coloreTemaFeature);
        this.overrideAvversitaLayerStyle(feature, icona);
        // this.googleMapWrapper.data.overrideStyle(feature, { icon: icona, fillColor: coloreTemaFeature, strokeColor: coloreTemaFeature });
        this.googleMapDataService.overrideFeatureStyle(feature, { icon: icona, fillColor: coloreTemaFeature, strokeColor: coloreTemaFeature });
        //TODO Salvo
        // this.polygonLabelService.overrideRenderLabel(feature, null);
        // this.polygonLabelInfoWindowService.overrideRenderLabel(feature, null);
      }
    });
  }

  public focusThemeVisibleFeatures(layerSelezionato: TipologiaLayer, temaSelezionato: TemaSelezionato, scalaColori: DatiColoreTema[]): void {
    let latlngAutoFit: google.maps.LatLngBounds = new google.maps.LatLngBounds();

    this.featureInformationService.getAll().forEach(feature => {
      const layerFeature = feature.properties.layer as string;
      if (layerFeature !== layerSelezionato.id) {
        return;
      }

      const isInvisible = this.isFeatureInvisible(layerSelezionato, temaSelezionato, feature, scalaColori);
      if (isInvisible) {
        return;
      }

      const position = this.featureInformationService.getPosition(feature.properties.id);
      latlngAutoFit.extend(position);
    });

    const centroMappa = latlngAutoFit.getCenter()
    this.googleMapWrapper.data.getMap().setCenter(centroMappa);
    this.googleMapWrapper.data.getMap().fitBounds(latlngAutoFit);
  }

  private handleInvisibleFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, layerFeature: string): void {
    const color = '#ffffff00';
    const icona = this.getIconaNonModificabile(feature, layerFeature, color);
    this.overrideAvversitaLayerStyle(feature, icona);
    // this.googleMapWrapper.data.overrideStyle(feature, { icon: icona, fillColor: color, strokeColor: color });
    this.googleMapDataService.overrideFeatureStyle(feature, { icon: icona, fillColor: color, strokeColor: color });
    //TODO Salvo
    //this.polygonLabelService.overrideRenderLabel(feature, false);
  }

  private isFeatureInvisible(layerSelezionato: TipologiaLayer, temaSelezionato: TemaSelezionato, feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp, scalaColori: DatiColoreTema[]) {
    const nomeTema = this.sharedDataService.getNomeTemaTipoLayer(layerSelezionato.nome, temaSelezionato.nome);
    const valoreScalaFeature = this.sharedDataService.getValoreScalaFeature(feature, nomeTema);
    const elementoScala = scalaColori.find(elemento => elemento.valoreMax >= valoreScalaFeature && (elemento.valoreMin == null || elemento.valoreMin <= valoreScalaFeature));
    return elementoScala != null && !elementoScala.visible;
  }

  private clonaGisDataReadParam(params: GisDataReadParam): GisDataReadParam {
    const objClonato = {
      piva: params.piva,
      sa_cod: params.sa_cod,
      TipologiaLayerSelezionata: params.TipologiaLayerSelezionata,
      filtroTemporale: {
        TipoFiltroTemporale: params.filtroTemporale.TipoFiltroTemporale,
        DataInizio: new Date(params.filtroTemporale.DataInizio),
        DataFine: new Date(params.filtroTemporale.DataFine),
        TipoOperatoreDataInizio: params.filtroTemporale.TipoOperatoreDataInizio,
        TipoOperatoreDataFine: params.filtroTemporale.TipoOperatoreDataFine,
      } as FiltroTemporale,

      filtroTemporaleSingolaData: {
        TipoFiltroTemporale: params.filtroTemporaleSingolaData.TipoFiltroTemporale,
        DataInizio: new Date(params.filtroTemporaleSingolaData.DataInizio),
        DataFine: new Date(params.filtroTemporaleSingolaData.DataFine),
        TipoOperatoreDataInizio: params.filtroTemporaleSingolaData.TipoOperatoreDataInizio,
        TipoOperatoreDataFine: params.filtroTemporaleSingolaData.TipoOperatoreDataFine,
      } as FiltroTemporale
    } as GisDataReadParam;

    return objClonato;
  }
}

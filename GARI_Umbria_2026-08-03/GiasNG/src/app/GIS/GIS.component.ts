import { HttpClient, HttpContext } from '@angular/common/http';
import { AfterContentChecked, Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import {
  BehaviorSubject,
  catchError,
  combineLatest,
  debounceTime,
  filter,
  map,
  Observable,
  of,
  startWith,
  Subject,
  switchMap,
  take,
  takeUntil,
  tap,
  withLatestFrom
} from 'rxjs';
import { GoogleMapGeoJsonService } from './google-map/google-map-geojson.service';
import { GoogleMapService } from './google-map/google-map.service';
import { DrawingManagerService } from './services/drawing-manager.service';
import { DrawingOptionsService } from './services/drawing-options.service';
import { DrawingService } from './services/drawing.service';
import { LayerStyleService } from './services/layer-style.service';
import { ActivatedRoute } from '@angular/router';
import { GisService } from './GIS.service';
import { SharedDataService } from './services/shared-data.service';
import { FeatureService } from './services/feature.service';
import { WKTService } from './services/wkt.service';
import { GISConfigurationModalComponent } from './GIS-configuration-modal/gis-configuration-modal.component';
import { GISCalendarModalComponent } from './GIS-calendar-modal/gis-calendar-modal.component';
import { DataLayerStyleService } from './services/data-layer-style.service';
import { PolygonLabelService } from './services/polygon-label.service';
import { GisToolbarService } from './GIS-toolbar/gis-toolbar.service';
import { PositionService } from './services/position.service';
import { ConfigurazioneSitiService } from '../Service/configurazione-siti.service';
import { PolygonWindowEventsService } from './services/polygon-window-events.service';
import {
  FiltroTemporale,
  FiltroTemporale_enum_OperatoreFiltroTemporale,
  FiltroTemporale_enum_TipoFiltroTemporale,
  GisClient,
  ImpostaConfigurazioneAlbero,
  LeggiConfigurazioniGeneraliGis,
  RispostaStandard_1OfCfgAlbero_CfgGisUtente,
  RispostaStandard_1OfConfigurazioniGisGenerali
} from 'app/Service/api.service';
import { TreeGisService } from 'app/Utility/Template/kendo-tree/services/tree-gis.service';
import { MeasureDistanceService } from './services/measure-distance.service';
import { EditFeatureService } from './services/edit-feature.service';
import { WmsService } from './services/wms.service';
import { AnalisiTerrenoType, GISModality } from './GIS-enum/GIS-feature';
import { DialItemClickEvent } from '@progress/kendo-angular-buttons';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { GisLayerColorPickerService } from './GIS-layer-color-picker-window/GIS-layer-color-picker-window.service';
import { GisFixedLayerPropertyService } from './GIS-fixed-layer-property-window/GIS-fixed-layer-property-window.service';
import { CreaDaPoligonoService } from './services/crea-da-poligono.service';
import { MasterService } from 'app/Service/master.service';
import { LayerService } from './services/layer.service';
import { GeoJsonFilterService } from './services/geojson-filter.service';
import { enum_OrigineChiamata, enum_OrigineChiamataFilterService, enum_OrigineChiamataLoadGeoJson } from './GIS-enum/GIS-origine-chiamata';
import { enum_LayerElementiGraficiStd } from './GIS-enum/GIS-layer-elementi-grafici';
import { consoleLogDebug, consoleLogDebugMultiParam, getComponentIdAndLog } from 'app/Service/utils';
import { enum_logDebugArea, enum_logDebugTipo } from 'app/Model/log-debug';
import { GISGeometrySelectionService } from './services/gis-geometry-selection.service';
import { CentraMappa, LatLng } from 'app/Model/GIS/Utility';
import { DrawWindowOperationService } from './GIS-kendo-window/draw-window/draw-window-operation.service';
import { GISLayerPermissionsWindowService } from './GIS-layer-permissions-window/GIS-layer-permissions-window.service';
import { GISLayerAdvancedSettingsWindowService } from './GIS-layer-advanced-settings-window/GIS-layer-advanced-settings-window.service';
import { GISGestionePianoRateoService } from './GIS-kendo-window/GIS-gestione-piano-rateo/GIS-gestione-piano-rateo.service';
import { RicetteService } from 'app/menu-agenda/components/grid-ricette/ricette.service';
import { ThemeWindowService } from './GIS-kendo-window/theme-window/theme-window.service';
import { GISAnalisiMappeSatellitariWindowService } from './GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-window.service';
import { ObjParametriAgendaService } from '../Service/obj-parametri-agenda.service';
import { SementieriParametrizzazione } from '../Model/GIS/SementieriParametrizzazione';
import { FunzioniComuniService } from '../Service/FunzioniComuni.service';
import { enum_TipologiaLayer } from './GIS-enum/GIS-tipologia-layer';
import { GISAttributiFileUploadService } from './GIS-attributi/GIS-attributi-file-upload/GIS-attributi-file-upload.service';
import { RetinaturaService } from './services/retinatura.service';
import { GoogleMapDataService } from './services/google.maps-services/google-map-data.service';
import {
  AlgorithmConfigurationWindowService
} from './GIS-kendo-window/GIS-algorithm-configuration-window/GIS-algorithm-configuration-window.service';
import { MappePrescrizioneService } from './services/mappe-prescrizione.service';
import { MappePrescrizioneRasterService } from './services/mappe-prescrizione-raster.service';
import { PolygonLabelInfowindowService } from './services/polygon-label-infowindow.service';
import { InfowindowClustererService } from './infowindow-clusterer/infowindow-clusterer.service';
import { GISRasterConfigurationWindowService } from './GIS-raster-configuration-window/GIS-raster-configuration-window.service';
import { RasterOverlayService } from './services/raster-overlay.service';
import { SatelliteAnimationService } from './services/satellite-animation.service';
import { GoogleMapMovementControlService } from './services/google-map-movement-control.service';
import { GISBookmarksWindowService } from './GIS-kendo-window/GIS-bookmarks-window/GIS-bookmarks-window.service';
import { MapGridOverlayService } from './services/map-grid-overlay.service';
import { GISAttributiMuzService } from './GIS-attributi/GIS-attributi-muz-grid/GIS-attributi-muz.service';
import { GISParticelleCatastaliService } from './GIS-particelle-catastali/GIS-particelle-catastali.service';
import { SatelliteGlobalDataLoader } from './GIS-analisi-mappe-satellitari-window/satellite-global-data-loader';
import { SatelliteLocalDataLoader } from './GIS-analisi-mappe-satellitari-window/satellite-local-data-loader';
import { EditFeatureWindowService } from './services/edit-feature-window.service';
import { GoogleMapGeoJsonLazyService } from './services/google.maps-services/google-map-geojson-lazy.service';
import { GoogleMapFeatureService } from './services/google.maps-services/google-map-feature.service';
import { FeatureInformationService } from './services/feature-information.service';
import { GISUtility } from '../Service/utils/GIS-utils/GIS-utility';
import { GoogleMapHeatmapService } from './services/google.maps-services/google-map-heatmap.service';
import { AdvancedTimeFilterService } from './services/advanced-time-filter.service';
import { MultiAziendaService } from './services/multi-azienda.service';
import { enum_GISDrawingOperations } from './GIS-enum/GIS-drawing-operations';
import { DateUtils, FiltroTemporaleAvanzato } from 'app/Utility/date-utils';
import { SementieriService } from 'app/Service/sementieri.service';
import {ExportCartographyDataService} from './GIS-toolbar/services/export-cartography-data.service';
import {RicetteSmartTractorService} from '../menu-agenda/components/grid-ricette/ricette-smart-tractor.service';

@Component({
  standalone: false,
  selector: 'GIS',
  templateUrl: './GIS.component.html',
  styleUrls: ['./GIS.component.scss'],
  providers: [
    DrawingManagerService,
    DrawingService,
    DrawingOptionsService,
    WKTService,
    PositionService,
    PolygonWindowEventsService,
    TreeGisService,
    MeasureDistanceService,
    WmsService,
    SatelliteAnimationService,
    SatelliteLocalDataLoader,
    SatelliteGlobalDataLoader,
    EditFeatureService,
    GisService,
    GoogleMapGeoJsonService,
    GoogleMapGeoJsonLazyService,
    GoogleMapService,
    GoogleMapDataService,
    DataLayerStyleService,
    LayerStyleService,
    PolygonLabelService,
    PolygonLabelInfowindowService,
    FeatureService,
    GisToolbarService,
    ExportCartographyDataService,
    GisLayerColorPickerService,
    GisFixedLayerPropertyService,
    CreaDaPoligonoService,
    DrawWindowOperationService,
    GISLayerPermissionsWindowService,
    ThemeWindowService,
    GISLayerAdvancedSettingsWindowService,
    GISGestionePianoRateoService,
    RicetteService,
    RicetteSmartTractorService,
    GISAnalisiMappeSatellitariWindowService,
    GISAttributiFileUploadService,
    RetinaturaService,
    AlgorithmConfigurationWindowService,
    MappePrescrizioneService,
    MappePrescrizioneRasterService,
    InfowindowClustererService,
    GISRasterConfigurationWindowService,
    RasterOverlayService,
    GoogleMapMovementControlService,
    GISBookmarksWindowService,
    MapGridOverlayService,
    GISAttributiMuzService,
    GISParticelleCatastaliService,
    EditFeatureWindowService,
    GoogleMapFeatureService,
    FeatureInformationService,
    GoogleMapHeatmapService,
    AdvancedTimeFilterService,
    MultiAziendaService
  ]
})

export class GISComponent implements OnInit, OnDestroy, AfterContentChecked, OnChanges {
  @ViewChild(GISConfigurationModalComponent) gisConfigurationModalComponent: GISConfigurationModalComponent;
  @ViewChild(GISCalendarModalComponent) gisCalendarModalComponent: GISCalendarModalComponent;

  @Input() _modality: GISModality | undefined = GISModality.Full;
  @Input() showToolbar: boolean | undefined = false;
  @Input() analisiTerrenoEdit?: boolean;
  @Input() forRilievi: boolean | undefined = false;

  @Output() loaded = new EventEmitter<GISComponent>();

  public get toolbarService() {
    return this.gisToolbarService;
  }

  public get mapService() {
    return this.googleMapService;
  }

  public get icon(): string {
    return this.dialOpen ? "xi-drawer-close" : "xi-drawer-open";
  }

  apiLoaded: Observable<boolean>;
  signal: Subject<void> = new Subject();
  lastClickedCoords: BehaviorSubject<google.maps.LatLng> = new BehaviorSubject(null);
  geoJsonLazyLoaded$ = this.googleMapGeoJsonLazyService.loaded$;
  public dialOpen: boolean = false;

  satelliteFocused: boolean = true;

  public contacts = [
    {
      icon: "xi-satellite",
      window: "maps-satellite",
    },
    {
      icon: "xi-roadmap",
      window: "maps-roadmap",
    },
  ];

  private centraMappaInInizializzazione = false;

  private paramCentraMappa: CentraMappa = null;
  private showToolbarSubject = new BehaviorSubject<boolean>(false);

  private componentId = null;
  private isSatelliteWindowHidden$ = this.kendoWindowsService.windowToggle$.pipe(
    filter(([windowType, _]) => windowType == WindowTypes.AnalisiMappeSatellitariWindow),
    map(([_, args]) => !args.openState),
    startWith(true)
  );

  isToolbarDisplayed$: Observable<boolean> = combineLatest([
    this.showToolbarSubject.asObservable(), // this overrides the others
    this.googleMapGeoJsonService.modality$.pipe(map(modality => modality == GISModality.Full)), // Toolbar hidden for non full modalities
    this.isSatelliteWindowHidden$ // Layer window hidden if the satellite analysis window is open
  ])
    .pipe(map(([toolbarVisibleOverride, ...visibles]) => toolbarVisibleOverride || visibles.every(x => x)));

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService,
    httpClient: HttpClient,
    private route: ActivatedRoute,
    private gisToolbarService: GisToolbarService,
    private gis: GisService,
    private gisClient: GisClient,
    private sharedDataService: SharedDataService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    private kendoWindowService: KendoWindowsService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private masterService: MasterService,
    private googleMapService: GoogleMapService,
    private layerService: LayerService,
    private geoJsonFilterService: GeoJsonFilterService,
    private wmsService: WmsService,
    private featureService: FeatureService,
    private featureInformationService: FeatureInformationService,
    private gisGeometrySelectionService: GISGeometrySelectionService,
    private drawingService: DrawingService,
    private editFeatureWindowService: EditFeatureWindowService,
    private googleMapGeoJsonLazyService: GoogleMapGeoJsonLazyService,
    private multiAziendaService: MultiAziendaService,
    private drawOperationService: DrawWindowOperationService,
    private kendoWindowsService: KendoWindowsService,
    private sementieriService: SementieriService
  ) {
    this.componentId = getComponentIdAndLog('GISComponent', 'constructor');
    this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.Indefinito);

    // Subscribe spostati da GisService
    combineLatest([this.layerService.ObservableLayer, this.googleMapService.loaded$.pipe(take(1))])
      .pipe(takeUntil(this.signal))
      .subscribe(([layers, _]) => {
        consoleLogDebug(enum_logDebugArea.Gis, enum_logDebugTipo.TraceSubscribe, this.constructor.name, 'ObservableLayer');
        gis.gestisciModificaListaLayer(layers, this.googleMapGeoJsonService.modality);
        this.loaded.emit(this);
      });

    combineLatest([this.layerService.layerItemSelected$, this.googleMapService.loaded$])
      .pipe(takeUntil(this.signal))
      .subscribe(([layer, _]) => {
        // Funzione eseguita ogni volta che un layer viene selezionato/deselezionato
        consoleLogDebug(enum_logDebugArea.Gis, enum_logDebugTipo.TraceSubscribe, this.constructor.name, 'LayerItemSelected');
        if (layer[1]) {
          // Se selezionato, porto i rispettivi poligoni in primo piano
          this.googleMapGeoJsonService.settaLayerDoveDisegnare(layer[0].id);
          // this.sharedDataService.setLayerSelezionato(layer[0]);
          // Se ci sono feature selezionate e la prima appartiene ad un layer diverso,
          // deseleziono tutte le feature
          const featureSelezionate = this.featureService.getFeatureSelezionate();
          if (featureSelezionate.length > 0) {
            const layerIdUltimaFeature = featureSelezionate[0].properties.layer;
            if (layer[0].id !== layerIdUltimaFeature) {
              this.googleMapGeoJsonService.seDeselezionaFeatureSelezionate(enum_OrigineChiamata.Mappa);
            }
          }
        }
      });

    combineLatest([this.layerService.LayerItemVisible, this.googleMapService.loaded$.pipe(take(1))])
      .pipe(takeUntil(this.signal))
      .subscribe(([layer, _]) => {
        consoleLogDebug(enum_logDebugArea.Gis, enum_logDebugTipo.TraceSubscribe, this.constructor.name, 'LayerItemVisible');
        const idLayer = layer[0].id;
        const flagVisibile = layer[1];
        if (this.sharedDataService.selezionatoTipoLayerEntita() && idLayer === enum_LayerElementiGraficiStd.WMS) {
          if (flagVisibile) {
            this.googleMapGeoJsonService.impostaZoomMinimoVisualizzazioneWms();
          }
          this.wmsService.wmsDdlSensoreElaborazioneChange(flagVisibile);
        } else {
          // Funzione eseguita quando si preme l'icona occhio su un layer
          this.googleMapGeoJsonService.clearOverlays(idLayer, flagVisibile);
        }
      });

    combineLatest([this.layerService.ListLayerItemVisible, this.googleMapGeoJsonLazyService.loaded$, this.sharedDataService.getCfgAlberoGisUtente$()])
      .pipe(takeUntil(this.signal))
      .subscribe(([layers, _1, config]) => this.googleMapGeoJsonLazyService.renderOnLayerVisibilityChanged(layers, config[0], this.googleMapService.googleMapWrapper.data.getMap()));

    const layers$ = this.layerService.ObservableLayer.pipe(take(1));
    combineLatest([this.geoJsonFilterService.geoJsonFilterServiceParamSource, this.googleMapService.loaded$, layers$])
      .pipe(
        debounceTime(200),
        takeUntil(this.signal)
      )
      .subscribe(([filterParam, _1, _2]) => {
        consoleLogDebug(enum_logDebugArea.Gis, enum_logDebugTipo.TraceSubscribe, this.constructor.name, 'geoJsonFilterServiceParamSource');
        if (filterParam.FlagLoadGeoJson) {
          if (filterParam.OrigineChiamata === enum_OrigineChiamataFilterService.QuadernoDiCampagna) {
            const qdcConPoligoni = this.geoJsonFilterService.getQdcConPoligoni();
            if (qdcConPoligoni) {
              this.googleMapGeoJsonService.loadGeoJsonFilterService(filterParam);
            }
          } else {
            this.googleMapGeoJsonService.loadGeoJsonFilterService(filterParam);
          }
        }
      });

    this.layerService.LayerItemChangeColor
      .pipe(takeUntil(this.signal))
      .subscribe(layer => {
        consoleLogDebug(enum_logDebugArea.Gis, enum_logDebugTipo.TraceSubscribe, this.constructor.name, 'LayerItemChangeColor');
        // Funzione eseguita quando si cambia il colore di un layer
        this.googleMapGeoJsonService.changeColorLayer(layer.id, true);
      })

    this.layerService.LayerItemCenterMap
      .pipe(takeUntil(this.signal))
      .subscribe(layerId => {
        if (layerId !== '') {
          this.googleMapGeoJsonService.impostaCentroMappaByLayer(layerId);
        }
      })

    this.layerService.LayerSelected
      .pipe(takeUntil(this.signal))
      .subscribe(tipoLayerOptionHtml => {
        consoleLogDebug(enum_logDebugArea.Gis, enum_logDebugTipo.TraceSubscribe, this.constructor.name, 'LayerSelected');
        // Funzione eseguita quando si cambia la tipologia di layer selezionato
        // NB: Il caricamento iniziale delle feature viene scatenato in questo momento
        this.sharedDataService.setTipoLayerSelezionato(tipoLayerOptionHtml?.Option_Value);
      })

    // Subscribe spostati da GoogleMapGeoJsonService

    this.featureService
      .getFeatureSelezionate$()
      .pipe(takeUntil(this.signal))
      .subscribe(features => {

        //LC: Nel caso in cui ho il GIS aperto nel pannello del QDC
        //non cambio l'objParametriAgenda perchè dopo non serve e non so come resettarlo
        if(this._modality !== GISModality.Trattamento){

          this.googleMapGeoJsonService.changeAziendaByFeatures(features);

          if (features.length == 0) {
            this.editFeatureWindowService.closeAll();
            return;
          }
        }
      });

    this.featureService
      .featureDoubleClicked$
      .pipe(
        withLatestFrom(this.sementieriService.isSementieriServer()),
        takeUntil(this.signal)
      )
      .subscribe(([feature, isSementieri]) => {
        if (this.googleMapGeoJsonService.modality != GISModality.Full) {
          return;
        }

        this.googleMapGeoJsonService.handleEditPolygon(feature, isSementieri);
      });

    combineLatest([this.gisGeometrySelectionService.geometrySelectedFromOutside$, this.googleMapService.loaded$])
      .pipe(takeUntil(this.signal))
      .subscribe(([keys, _]) => {
        consoleLogDebugMultiParam(
          enum_logDebugArea.Gis,
          enum_logDebugTipo.TraceSubscribe,
          this.constructor.name,
          'geometrySelectedFromOutside$',
          [keys, this.googleMapGeoJsonService.serviceId]
        )
        this.googleMapGeoJsonService.handleSelectedGeometries(keys)
      });

    // Subscribe spostati da WmsService
    combineLatest([this.sharedDataService.FixedLayerPropertySource, this.googleMapService.loaded$])
      .pipe(takeUntil(this.signal))
      .subscribe(([fixedLayer, _]) => this.wmsService.gestioneFixedLayerProperty(fixedLayer));

    //--------------------------------------------------------------------------------

    combineLatest([this.geoJsonFilterService.centraMappaSource, this.googleMapService.loaded$])
      .pipe(takeUntil(this.signal))
      .subscribe(([param, _]) => {
        if (param) {
          this.seCentraMappaConCoordinate(param, 0);
        }
      });

    if (this.masterService.gmapsApiLoaded == false) {
      this.configurazioneSitiService.currentConfigurazione_Siti
        .pipe(take(1))
        .subscribe(cs => {
          let urlWithApiKey = '';
          let csGoogleMaps = cs.filter(cs_ => cs_.Chiave === 'googlemaps');
          if (csGoogleMaps.length > 0) {
            urlWithApiKey = csGoogleMaps[0].Valore;
          }
          let url = '';
          // TODO: la versione "forzata" deve ssere rimossa entro febbraio 2027
          if (urlWithApiKey !== '') {
            url = urlWithApiKey + ',places,visualization&v=3.64';
          } else {
            url = 'https://maps.googleapis.com/maps/api/js?libraries=drawing,geometry,places,visualization&v=3.64';
          }
          this.apiLoaded = httpClient.jsonp(url, 'callback')
            .pipe(
              // take(1),
              map(() => true),
              tap(() => this.masterService.gmapsApiLoaded = true),
              catchError(() => of(false)),
            );
        });
    } else {
      this.apiLoaded = of(true);
    }

    this.gisToolbarService.configurazioneBtnClick$
      .subscribe(() => this.gisConfigurationModalComponent.open());

    this.gisToolbarService.calendarioBtnClick$
      .subscribe(() => this.gisCalendarModalComponent.open());

    this.sharedDataService.loadGeoJsonForzato
      .pipe(takeUntil(this.signal))
      .subscribe(result => {
        if (result[0] === true) {
          this.googleMapGeoJsonService.loadGeoJsonForzato(result[1]);
        }
      });

  }

  ngOnInit(): void {

    this.route.queryParams.subscribe(params => {
      this.multiAziendaService.country = params["nazione"] ?? "";
      this.multiAziendaService.year = params["anno"] ?? 0;
      // In questa versione i parametri querystring hanno priorità su quelli in input
      this._modality = parseInt(params['modalita'] ?? this._modality);

      // const giasHeaderComponent = document.querySelector('gias-master-header');
      // if (giasHeaderComponent) {
      //   giasHeaderComponent.setAttribute('style', 'position: absolute; visibility: hidden; left: -9999px; top: -9999px;');
      //   giasHeaderComponent.setAttribute('style', 'height: 0 !important');
      // }
    });

    this.googleMapGeoJsonService.modality = this._modality;

    let objParams = this.objParametriAgendaService.getObjParamValue();
    if (
      objParams.QueryStringFiltrino &&
      objParams.QueryStringFiltrino != '' &&
      SementieriParametrizzazione.isInstanceOfSementieri(JSON.parse(objParams.QueryStringFiltrino))
    ) {
      this.handleIsSementieri();
    } else {
      this.setDefaultTimeFilter();
    }

    // Inizializzazione oggetti ingresso e uscita
    const leggiGisGenerali: LeggiConfigurazioniGeneraliGis = {};
    const impostaCfgAlbero: ImpostaConfigurazioneAlbero = {};

    this.gis.InizializzaParametriGis();

    this.route.queryParams.pipe(
      takeUntil(this.signal),
      switchMap((params) => {

        // Parametri Configurazioni Gis Generali

        if (this.seStringaValorizzata(params.ParametriFiltroPercorsi)) {
          leggiGisGenerali.ParametriFiltroPercorsi = params.ParametriFiltroPercorsi;
        }
        if (this.seStringaValorizzata(params.Filtrone)) {
          leggiGisGenerali.Filtrone = params.filtrone;
        }
        if (this.seStringaValorizzata(params.Impianti)) {
          leggiGisGenerali.Impianti = JSON.parse(params.Impianti);
        }
        if (this.seStringaNumerica(params.IDTestataTemp)) {
          leggiGisGenerali.IDTestataTemp = params.IDTestataTemp;
        }
        if (this.seStringaValorizzata(params.Sementi)) {
          leggiGisGenerali.Sementi = params.Sementi;
        }
        if (this.seStringaValorizzata(params.SementiMappaturaLibera)) {
          leggiGisGenerali.SementiMappaturaLibera = params.SementiMappaturaLibera;
        }

        // Parametri Configurazioni Albero e Gis Utente

        if (this.seStringaValorizzata(params.Elenco_Icone_SpecieVegetali)) {
          impostaCfgAlbero.Elenco_Icone_SpecieVegetali = params.Elenco_Icone_SpecieVegetali;
        }
        if (this.seStringaValorizzata(params.DatiSportelloSementieri)) {
          impostaCfgAlbero.DatiSportelloSementieri = params.DatiSportelloSementieri;
        }
        if (this.seStringaValorizzata(params.Sa_Cod)) {
          impostaCfgAlbero.Sa_Cod = params.Sa_Cod;
        }

        // Lettura Configurazioni Gis Generali
        let obs: (body?: LeggiConfigurazioniGeneraliGis | undefined, httpContext?: HttpContext) => Observable<RispostaStandard_1OfConfigurazioniGisGenerali>;
        switch (this.googleMapGeoJsonService.modality) {
          case GISModality.Full:
            obs = this.gisClient.gisCfgGISLeggiGenerali.bind(this.gisClient);
            break;
          default:
            obs = GISUtility.defaultGisCfgGISLeggiGenerali;
            break;
        }
        return obs(leggiGisGenerali, undefined);
      }),

      switchMap((risposta_CfgGIS_Leggi_Generali: any) => {

        // Memorizzazione Configurazioni Gis Generali

        this.sharedDataService.setCfgGisGenerali(risposta_CfgGIS_Leggi_Generali.RispostaStringa);

        // Lettura Configurazioni Albero e Gis Utente

        impostaCfgAlbero.LeggiConfigurazioneDaDatabase = true;
        impostaCfgAlbero.FiltroImpiantiIdTestataTemp = this.sharedDataService.getCfgGisGenerali().IDTestataTemp;
        impostaCfgAlbero.Piva = this.sharedDataService.getCfgGisGenerali().Piva;

        let obs: (body?: ImpostaConfigurazioneAlbero | undefined, httpContext?: HttpContext) => Observable<RispostaStandard_1OfCfgAlbero_CfgGisUtente>;
        switch (this.googleMapGeoJsonService.modality) {
          case GISModality.Full:
            obs = this.gisClient.gisConfiguraAlbero.bind(this.gisClient);
            break;
          case GISModality.PaesePoligoniMultiAzienda:
            obs = GISUtility.gisConfiguraAlberoHeatMapOn;
            break;
          default:
            obs = GISUtility.defaultGisConfiguraAlbero;
            break;
        }
        return obs(impostaCfgAlbero, undefined);
      }),
      switchMap((risposta_ConfiguraAlbero: any) => {

        // Memorizzazione Configurazioni Albero e Gis Utente

        // if (this.sharedDataService.iAutoZoomSuVisTotaleLocalStorage) {
        //     let iAutoZoomSuVisTotale = Number(localStorage.getItem("iAutoZoomSuVisualizzazioneTotale"));
        //     if (!isNaN(iAutoZoomSuVisTotale) && iAutoZoomSuVisTotale !== 0) {
        //         risposta_ConfiguraAlbero.RispostaStringa.CfgGisUtente.iAutoZoomSuVisualizzazioneTotale = iAutoZoomSuVisTotale;
        //     }
        // }

        this.sharedDataService.setCfgAlberoGisUtente(risposta_ConfiguraAlbero.RispostaStringa, true);

        return of(null);
      })

    ).subscribe();

    let documentWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;

    if (documentWidth < 992 || this._modality != GISModality.Full) {
      this.contacts = [
        {
          icon: "xi-layer-open",
          window: "layer",
        },
        {
          icon: "xi-design-open",
          window: "draw",
        },
        {
          icon: "xi-satellite",
          window: "maps-satellite",
        },
        {
          icon: "xi-roadmap",
          window: "maps-roadmap",
        },
      ];
      if (this._modality != GISModality.Full) {
        this.contacts = this.contacts.filter(c => c.window != 'draw');
      }
    }

    this.googleMapGeoJsonService.lastClickedCoordinates
      .pipe(takeUntil(this.signal))
      .subscribe(coords => this.lastClickedCoords.next(coords));

    // Inizializzo filtro temporale avanzato
    this.setUpFiltroTemporale();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.showToolbar && changes.showToolbar.currentValue != changes.showToolbar.previousValue) {
      this.showToolbarSubject.next(changes.showToolbar.currentValue);
    }
  }

  seStringaNumerica(stringa: string | undefined): boolean {
    return stringa !== undefined && !isNaN(Number(stringa));
  }

  setDrawOperation(operation: enum_GISDrawingOperations): void {
    this.drawOperationService.setOperation(operation);
  }

  setLayer(layerId: string): void {
    this.layerService.setLayerItemSelected([this.sharedDataService.getTipologiaLayerById(layerId), true]);
  }

  setCenter(center: CentraMappa): void {
    this.googleMapGeoJsonService.impostaZoom(center.livelloZoom);
    this.geoJsonFilterService.setCentraMappa(center);
  }

  seStringaValorizzata(stringa: string | undefined): boolean {
    return stringa !== undefined && stringa !== '';
  }

  ngAfterContentChecked(): void {
    if (this.googleMapGeoJsonService.modality === GISModality.Trattamento &&
      this.googleMapGeoJsonService.impostaCentroMappaQdc &&
      this.googleMapGeoJsonService.geoJsonCaricato) {

      this.googleMapGeoJsonService.impostaCentroMappaDaFeature(false, false);
      this.googleMapGeoJsonService.impostaCentroMappaQdc = false;
      this.googleMapGeoJsonService.geoJsonCaricato = false;

    }
    let toolbar = document.getElementsByClassName("gis-toolbar-container")
    if (toolbar?.length && this.forRilievi) {
      for (let i = 0; i < toolbar.length; i++) {
        let item = toolbar.item(i);
        item.setAttribute("style", "z-index: 10")
      }
    }
  }

  initMap(): void {
    let mod = this.googleMapGeoJsonService.modality;
    switch (mod) {
      case GISModality.Full:
        break;
      case GISModality.Trattamento:
        this.sharedDataService.setTipoLayerSelezionato(enum_TipologiaLayer.Entita); // TODO TIPO_LAYER CORRETTO
        this.gis.initFullMapTrattamento();
        if (this.centraMappaInInizializzazione) {
          this.googleMapService.loaded$
            .pipe(take(1))
            .subscribe(() => this.seCentraMappaConCoordinate(this.paramCentraMappa, 1000));
        }
        break;
      case GISModality.PaeseDistribuzioneMultiAzienda:
      case GISModality.PaesePoligoniMultiAzienda:
        this.multiAziendaService.windowSize = {
          width: window.innerWidth,
          height: window.innerHeight
        };
        this.multiAziendaService.initMapMultiAzienda(mod);
        break;
    }
  }

  private seCentraMappaConCoordinate(param: CentraMappa, timeout: number) {
    if (param?.punto?.isValid) {
      console.log('---seCentraMappaConCoordinate.setTimeout', timeout, new Date().getTime());
      setTimeout(() => {
        console.log('---seCentraMappaConCoordinate.inizio', param.punto.lat, param.punto.lng, new Date().getTime());
        this.gisToolbarService.goToNumericPosition(param.punto.lat, param.punto.lng);
        this.googleMapGeoJsonService.impostaZoom(param.livelloZoom);
        this.centraMappaInInizializzazione = false;
        this.paramCentraMappa = null;
        this.geoJsonFilterService.setCentraMappa(null);
        console.log('---seCentraMappaConCoordinate.fine', new Date().getTime());
      },
        timeout)
      // } else {
      //     this.centraMappaInInizializzazione = true;
      //     this.paramCentraMappa = param;
      // }
    }
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
    this.kendoWindowService.reset();
    this.featureInformationService.signal$.next()
    this.featureInformationService.signal$.complete();

    this.drawingService.deleteSubscriptions();
  }

  public isFullModality(): boolean {
    return this.googleMapGeoJsonService.modality === GISModality.Full;
  }

  public isTrattamento(): boolean {
    return this.googleMapGeoJsonService.modality === GISModality.Trattamento;
  }

  public isAnalisiTerreno(): boolean {
    return this._modality === GISModality.AnalisiTerreno;
  }

  public onDialOpen(): void {
    this.dialOpen = true;
    this.satelliteFocused = this.googleMapService.googleMapWrapper.googleMap.getMapTypeId() == google.maps.MapTypeId.HYBRID;
  }

  public onDialClose(): void {
    this.dialOpen = false;
  }

  public onDialItemClick(e: DialItemClickEvent): void {
    const args = { item: e.item, index: e.index };
    // console.log("dialItemClick", args);
    if (args.item.window == 'layer') {
      this.layerBtnToggle();
    } else if (args.item.window === 'draw') {
      this.drawBtnToggle();
    } else if (args.item.window === 'maps-satellite') {
      if (this.googleMapService.googleMapWrapper.googleMap.getMapTypeId() != google.maps.MapTypeId.HYBRID) {
        this.googleMapService.googleMapWrapper.googleMap.setMapTypeId(google.maps.MapTypeId.HYBRID);
      }
      this.satelliteFocused = this.googleMapService.googleMapWrapper.googleMap.getMapTypeId() == google.maps.MapTypeId.HYBRID;
    } else if (args.item.window === 'maps-roadmap') {
      if (this.googleMapService.googleMapWrapper.googleMap.getMapTypeId() != google.maps.MapTypeId.ROADMAP) {
        this.googleMapService.googleMapWrapper.googleMap.setMapTypeId(google.maps.MapTypeId.ROADMAP);
      }
      this.satelliteFocused = this.googleMapService.googleMapWrapper.googleMap.getMapTypeId() == google.maps.MapTypeId.HYBRID;
    }
  }

  public layerBtnToggle() {
    let width = window.innerWidth;
    let height = window.innerHeight / 2;
    let top = window.innerHeight / 2;
    let windowArgs = new WindowArgs(WindowTypes.LayerWindow, false, "Layers", height, width, 250, 0, top, true, true, true, false, true, false, false);
    this.kendoWindowService.open(windowArgs.windowType, windowArgs);
  }

  public drawBtnToggle() {
    let width = window.innerWidth;
    let windowArgs = new WindowArgs(WindowTypes.DrawWindow, false, "Disegna", null, width, 250, 0, null, true, true, true, false, true, false, false);
    this.kendoWindowService.open(windowArgs.windowType, windowArgs);
  }

  public setMarker(coordinate: google.maps.LatLng, center: boolean = false, timeout: number = 200) {
    this.gisToolbarService.strumentoGpsAttivo = true;
    this.googleMapGeoJsonService.creaIndicatoreGps(coordinate, true)
    this.gisToolbarService.setLatLng(coordinate.lat(), coordinate.lng());

    if (this.googleMapService.googleMapWrapper.getZoom() < 15) {
      this.googleMapService.googleMapWrapper.googleMap.setZoom(15);
      center = true;
    }

    if (center) {
      let params = new CentraMappa();
      params.punto = new LatLng();
      params.punto.lat = coordinate.lat();
      params.punto.lng = coordinate.lng();
      params.punto.isValid = true;
      params.livelloZoom = 19;

      this.googleMapService.loaded$
        .pipe(take(1))
        .subscribe(() => this.seCentraMappaConCoordinate(params, timeout));
    }

  }

  public removeMarkers() {
    this.googleMapGeoJsonService.rimuoviIndicatoriGps();
  }

  public disabilitaDeselezione() {
    this.googleMapGeoJsonService.disabilitaDeselezione = true;
  }

  public abilitaDeselezione() {
    this.googleMapGeoJsonService.disabilitaDeselezione = false;
  }

  public get isAnalisiTerrenoMarkerActive(): boolean {
    return this.gisToolbarService.isMarkerPlacerActive;
  }

  public centerMapOnSelectedFeatures(): void {
    this.googleMapGeoJsonService.impostaCentroMappaDaGeometrie();
  }

  //#region "Sementieri"
  // --------------------------------- GESTIONE SEMENTIERI ---------------------------------
  private handleIsSementieri(): void {
    const sementieri = new SementieriParametrizzazione(JSON.parse(this.objParametriAgendaService.getObjParamValue().QueryStringFiltrino));
    this.sharedDataService.setCfgSementi(sementieri);
    this.sharedDataService.setTipoLayerSelezionato(enum_TipologiaLayer.OrganizzazioneAppartenenza);

    if (sementieri.SementiMappaturaLibera == 'True') {
      this.setDefaultTimeFilter();
    } else {
      this.setTimeFilterSementieri();
      this.sharedDataService.setVisualizzazioneTotale(true);
    }
  }

  private setTimeFilterSementieri(): void {
    // Filtro Temporale
    let dataInizioQS: Date = FunzioniComuniService.convertStringDdMmYyyyToDate(this.sharedDataService.getCfgSementiAsValue().Sementi.split('|')[6]);
    let dataFineQS: Date = FunzioniComuniService.convertStringDdMmYyyyToDate(this.sharedDataService.getCfgSementiAsValue().Sementi.split('|')[7]);

    let filtroTemporaleAvanzatoDefault: FiltroTemporaleAvanzato = new FiltroTemporaleAvanzato();

    filtroTemporaleAvanzatoDefault.filtroTemporalePeriodo = {
      TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData,
      DataInizio: new Date(dataInizioQS),
      DataFine: new Date(dataFineQS),
      TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,
    } as FiltroTemporale;

    const startDateOnlyDate = new Date(dataInizioQS);
    startDateOnlyDate.setHours(0, 0, 0, 0);
    filtroTemporaleAvanzatoDefault.filtroTemporaleSingolaData = {
      TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale,
      DataInizio: DateUtils.calcolaDataInizioSingolaData(startDateOnlyDate),
      DataFine: DateUtils.calcolaDataFineSingolaData(startDateOnlyDate),
      TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,
    } as FiltroTemporale;

    this.sharedDataService.setFiltroTemporaleAvanzato(filtroTemporaleAvanzatoDefault);
  }
  //#endregion "Sementieri"

  private setDefaultTimeFilter() {
    // Filtro Temporale
    let filtroTemporaleAvanzatoDefault = new FiltroTemporaleAvanzato();

    const dataOggi = new Date();
    dataOggi.setHours(0, 0, 0, 0);
    filtroTemporaleAvanzatoDefault.filtroTemporalePeriodo = {
      TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData,
      DataInizio: new Date(dataOggi),
      DataFine: new Date(dataOggi),
      TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,
    } as FiltroTemporale;

    const dataInizio = new Date(filtroTemporaleAvanzatoDefault.filtroTemporalePeriodo.DataInizio);
    dataInizio.setHours(0, 0, 0, 0);
    filtroTemporaleAvanzatoDefault.filtroTemporaleSingolaData = {
      TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale,
      DataInizio: DateUtils.calcolaDataInizioSingolaData(dataInizio),
      DataFine: DateUtils.calcolaDataFineSingolaData(dataInizio),
      TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,
    } as FiltroTemporale;

    this.sharedDataService.setFiltroTemporaleAvanzato(filtroTemporaleAvanzatoDefault);
  }

  private setUpFiltroTemporale(): void {
    //--------------------------------------------------------------------------------
    // Funzione eseguita ogni volta che viene aperto il GIS:
    // impostazione filtro temporale per lettura iniziale
    //--------------------------------------------------------------------------------

    this.googleMapGeoJsonService.loadFiltroTemporale()
      .then(resolved => this.gisCalendarModalComponent.checkFiltroTemporaleAvanzato());
  }
}

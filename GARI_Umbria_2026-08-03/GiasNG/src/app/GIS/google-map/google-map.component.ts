import {
  AfterViewInit,
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
  Output,
  EventEmitter,
  Input,
  Optional,
  ChangeDetectorRef
} from '@angular/core';
import { GoogleMap } from '@angular/google-maps';
import { catchError, combineLatest, debounceTime, filter, forkJoin, map, of, skip, Subject, Subscription, switchMap, take, takeUntil, tap, throwError, withLatestFrom } from 'rxjs';
import { enum_GISDrawingOperations } from '../GIS-enum/GIS-drawing-operations';
import { DrawingManagerService } from '../services/drawing-manager.service';
import { GoogleMapGeoJsonService } from './google-map-geojson.service';
import { GoogleMapService } from './google-map.service';
import { PolygonWindowEventsService } from '../services/polygon-window-events.service';
import { enum_OrigineChiamata, enum_OrigineChiamataLoadGeoJson } from '../GIS-enum/GIS-origine-chiamata';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { FeatureService } from '../services/feature.service';
import { GisService } from '../GIS.service';
import { TranslocoService } from '@jsverse/transloco';
import { GISModality, enum_FeatureProperty } from '../GIS-enum/GIS-feature';
import { EditFeatureService } from '../services/edit-feature.service';
import { WmsService } from '../services/wms.service';
import { SharedDataService } from '../services/shared-data.service';
import { CreaDaPoligonoService } from '../services/crea-da-poligono.service';
import { LayerService } from '../services/layer.service';
import { FeatureType } from '../../Model/GIS/GisDataReadRval_New';
import { DEFAULT_TOP_POSITION, GisToolbarService } from '../GIS-toolbar/gis-toolbar.service';
import { DrawWindowOperationService } from '../GIS-kendo-window/draw-window/draw-window-operation.service';
import { GiasDialogService } from '../../Service/gias-dialog.service';
import { MarkerWindowModality } from '../GIS-kendo-window/marker-window/marker-window.component';
import { RicetteService } from 'app/menu-agenda/components/grid-ricette/ricette.service';
import { AllegatiDaRicettaDestinazione, GisClient, LeggiAllegatiDaRicettaDestinazione_In, RispostaStandard_1OfGisDataReadRval_New_1OfGeoJSONAgroGisProp } from 'app/Service/api.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { enum_LayerElementiGraficiStd } from '../GIS-enum/GIS-layer-elementi-grafici';
import { GISAnalisiMappeSatellitariWindowService } from '../GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-window.service';
import { GiasMessageService } from '../../Service/gias-message.service';
import { enum_TipologiaLayer } from '../GIS-enum/GIS-tipologia-layer';
import { NotificationRef } from '@progress/kendo-angular-notification';
import { GoogleMapDataService } from '../services/google.maps-services/google-map-data.service';
import { ClipboardService } from '../../Service/clipboard.service';
import { WKTService } from '../services/wkt.service';
import { MappePrescrizioneService } from '../services/mappe-prescrizione.service';
import { MappePrescrizioneRasterService } from '../services/mappe-prescrizione-raster.service';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';
import { GoogleMapMovementControlService } from '../services/google-map-movement-control.service';
import { MapGridOverlayService } from '../services/map-grid-overlay.service';
import { GISRasterConfigurationWindowService } from '../GIS-raster-configuration-window/GIS-raster-configuration-window.service';
import { GoogleMapGeoJsonLazyService } from '../services/google.maps-services/google-map-geojson-lazy.service';
import { GoogleMapFeatureService } from '../services/google.maps-services/google-map-feature.service';
import { FeatureInformationService } from '../services/feature-information.service';
import { DataLayerStyleService } from '../services/data-layer-style.service';
import { MasterService } from 'app/Service/master.service';
import { GISGeometrySelectionService } from '../services/gis-geometry-selection.service';
import { SementieriService } from 'app/Service/sementieri.service';
import { is } from 'app/Utility/Template/kendo-tree/utility/providers';

const DEFAULT_GEOJSON_OPTIONS = {
  idPropertyName: enum_FeatureProperty.id
};

@Component({
  standalone: false,
  selector: 'gias-google-map',
  templateUrl: './google-map.component.html',
  styleUrls: ['./google-map.component.css'],
})
export class GoogleMapComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild(GoogleMap) googleMap: GoogleMap;
  @Input() modality: GISModality | undefined = GISModality.Full;
  @Output() loaded = new EventEmitter<void>();

  signal: Subject<void> = new Subject();
  allegatiDaRicettaSubject = new Subject<AllegatiDaRicettaDestinazione | null>();
  dialogListaAllegatiOpen = false;
  listaAllegati: AllegatiDaRicettaDestinazione[] = [];
  allegatoSelected: number | null = null;

  public calcHeight = 'calc(100vh - 0px);';

  operation: enum_GISDrawingOperations;

  public readonly center: google.maps.LatLngLiteral = { lat: 0, lng: 0 };
  private options: google.maps.MapOptions;
  zoom: number = 3;
  private mapZoomListener: google.maps.MapsEventListener;
  labelZoom: string = '';
  GISModality = GISModality;

  private notificationRef: NotificationRef;

  constructor(
    private googleMapService: GoogleMapService,
    private masterService: MasterService,
    private drawingManagerService: DrawingManagerService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private polygonWindowEventsService: PolygonWindowEventsService,
    private kendoWindowsService: KendoWindowsService,
    private giasMessageService: GiasMessageService,
    private featureService: FeatureService,
    private gisService: GisService,
    private transloco: TranslocoService,
    private wmsService: WmsService,
    private editFeatureService: EditFeatureService,
    private sharedDataService: SharedDataService,
    private creaDaPoligonoService: CreaDaPoligonoService,
    private googleMapDataService: GoogleMapDataService,
    private layerService: LayerService,
    private drawWindowOperationService: DrawWindowOperationService,
    private giasDialogService: GiasDialogService,
    private gisGeometrySelectionService: GISGeometrySelectionService,
    private gisClient: GisClient,
    private googleMapMovementControlService: GoogleMapMovementControlService,
    private geoJsonLazyService: GoogleMapGeoJsonLazyService,
    private googleMapFeatureService: GoogleMapFeatureService,
    private featureInformationService: FeatureInformationService,
    private clipboardService: ClipboardService,
    private wktService: WKTService,
    private dataLayerStyleService: DataLayerStyleService,
    private gisToolbarService: GisToolbarService,
    private funzioniComuniService: FunzioniComuniService,
    private sementieriService: SementieriService,
    private changeDetectorRef: ChangeDetectorRef,
    @Optional() private ricetteService: RicetteService,
    @Optional() private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    @Optional() private mappePrescrizioneService: MappePrescrizioneService,
    @Optional() private mappePrescrizioneRasterService: MappePrescrizioneRasterService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    @Optional() private mapGridOverlayService: MapGridOverlayService,
    @Optional() private gisRasterConfigurationWindowService: GISRasterConfigurationWindowService,
  ) {
    // console.log('[google-map.component] constructor');
    this.polygonWindowEventsService.reloadFeatures
      .pipe(takeUntil(this.signal)).subscribe(() => {
        this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.DisegnoPoligono);
        this.googleMapGeoJsonService.loadGeoJsonForzato(false);
      });

    this.drawWindowOperationService
      .operation$
      .pipe(
        takeUntil(this.signal),
        withLatestFrom(this.sementieriService.isSementieriSportello())
      )
      .subscribe(([operation, isSementieri]) => this.setOperation(operation, isSementieri));

    combineLatest([
      this.sharedDataService.getCfgAlberoGisUtente$(),
      this.sharedDataService.geoJsonLoaded.pipe(take(1))
    ])
      .pipe(takeUntil(this.signal))
      .subscribe(([config, _]) => {
        if (config[0]?.CfgGisUtente?.ckGrigliaTiles_Sviluppo) {
          this.mapGridOverlayService.add(this.googleMap);
        } else {
          this.mapGridOverlayService.remove(this.googleMap);
        }
      });

    // TODO remove
    combineLatest([this.googleMapService.idle$, this.sharedDataService.geoJsonLoaded.pipe(take(1))])
      .pipe(
        skip(1), // We do not care about the first one because we will zoom on the available features
        takeUntil(this.signal),
        debounceTime(200),
        tap(() => this.geoJsonLazyService.renderGeoJson(this.googleMap.data.getMap()))
      ).subscribe();

    this.googleMapFeatureService
      .addFeatures$
      .pipe(takeUntil(this.signal))
      .subscribe(features => {
        const addedFeatures = features.map(feature => this.googleMap.data.add(feature));
        this.googleMapFeatureService.addedFeatures(...addedFeatures);
        this.setStyle();
      });

    this.googleMapFeatureService
      .removeFeatures$
      .pipe(takeUntil(this.signal))
      .subscribe(features => {
        features.forEach(feature => this.googleMap.data.remove(feature));
        this.googleMapFeatureService.removedFeatures(...features);
        this.setStyle();
      });

    this.googleMapFeatureService
      .loadGeoJson$
      .pipe(takeUntil(this.signal))
      .subscribe(geoJson => {
        const addedFeatures = this.googleMap.data.addGeoJson(geoJson, DEFAULT_GEOJSON_OPTIONS);
        this.googleMapFeatureService.loadedByGeoJson(geoJson, ...addedFeatures);
        this.handleSelectedFeaturesStyle();
      });
  }

  ngOnInit(): void {
    if (document.querySelector('header'))
      this.calcHeight = (window.innerHeight - document.querySelector('header').offsetHeight) + 'px';
    else
      this.calcHeight = (window.innerHeight) + 'px';

    if (this.sharedDataService.getCfgSementiAsValue() != undefined)
      this.notificationRef = this.giasMessageService.customMessage(this.getSementiMsg(), '', '', () => { return; })
  }

  ngAfterViewInit(): void {
    google.maps.event.addListenerOnce(this.googleMap.googleMap, 'projection_changed', () => {
      this.googleMapService.googleMapWrapper = this.googleMap;
      this.setMapOptions();

      this.googleMap.googleMap.addListener('click',
        (e) => {
          // handled separetely
          if (this.gisToolbarService.isMarkerPlacerActive) {
            return;
          }

          if (this.kendoWindowsService.getOpenState(WindowTypes.AnalisiMappeSatellitariWindow) && this.gisAnalisiMappeSatellitariWindowService.isMapInfoActive) {
            this.gisAnalisiMappeSatellitariWindowService?.nextMapClick(e);
            return;
          }

          if (this.gisRasterConfigurationWindowService.isAnyMapInfoActive) {
            this.gisRasterConfigurationWindowService.nextMapClickEvent(e);
            return;
          }

          this.googleMapGeoJsonService.seDeselezionaFeatureSelezionate(enum_OrigineChiamata.Mappa);
          this.wmsService.seWmsInfoClickMappa(e);
        });
      this.googleMap.googleMap.addListener(
        'idle',
        () => {
          const currentZoom = this.googleMapService.googleMapWrapper.getZoom();
          this.googleMapService.nextIdle(this.googleMapService.googleMapWrapper.getCenter(), currentZoom);
          this.gisAnalisiMappeSatellitariWindowService?.nextMapIdle();
          this.googleMapMovementControlService.nextIdle(this.googleMapService.googleMapWrapper.getCenter(), currentZoom);
          this.labelZoom = `Livello zoom: ${currentZoom}`;
          this.changeDetectorRef.markForCheck();
        }
      );

      // Imposto il tipo di mappa di default a HYBRID
      this.googleMap.googleMap.setMapTypeId(google.maps.MapTypeId.HYBRID);

      this.loaded.emit();
    });
  }

  private setStyle(): void {
    this.googleMap.data.setStyle((f: google.maps.Data.Feature) => {
      const feature = this.featureInformationService.getById(f.getId().toString());
      if (feature != undefined) {
        return this.dataLayerStyleService.getFeatureStyle(feature);
      } else {
        return;
      }
    });
  }

  private handleSelectedFeaturesStyle(): void {
    const selected = this.featureService
      .getFeatureSelezionate()
      .map(f => f.properties.id);

    if (selected.length == 0) {
      return;
    }

    this.googleMap.data.forEach(f => {
      const feature = this.featureInformationService.getById(f.getId().toString());
      if (feature == null || !selected.find(id => feature.properties.id == id)) {
        return;
      }

      const permessiFeature = this.featureService.getPermessiFeature(feature);
      if (permessiFeature.modifica && this.modality == GISModality.Full) {
        this.googleMapDataService.overrideFeatureStyle(feature, this.googleMapGeoJsonService.stileModificabile(feature));
      } else {
        this.googleMapDataService.overrideFeatureStyle(feature, this.googleMapGeoJsonService.stileSelezionato(feature));
      }
    });
  }

  private getSementiMsg(): string {
    if (this.sharedDataService.getCfgSementiAsValue().SementiMappaturaLibera != 'True')
      return this.sharedDataService.getCfgSementiAsValue().Sementi.split('|')[5];
    else
      return this.transloco.translate('gis.MappaturaLibera');
  }

  public mapInitializedHandler() {
    setTimeout(() => {
      google.maps.event.trigger(this.googleMap.googleMap, 'resize');
      // this.googleMap.googleMap.setCenter(new google.maps.LatLng(this.center?.lat, this.center?.lng));
    }, 900);
  }

  private setMapOptions() {
    this.options = {
      mapTypeControl: false,
      mapTypeControlOptions: {
        position: google.maps.ControlPosition.LEFT_TOP,
        style: google.maps.MapTypeControlStyle.DEFAULT,
        mapTypeIds: [google.maps.MapTypeId.HYBRID, google.maps.MapTypeId.TERRAIN, google.maps.MapTypeId.SATELLITE, google.maps.MapTypeId.ROADMAP]
      },
      disableDoubleClickZoom: true,
      fullscreenControlOptions: {
        position: google.maps.ControlPosition.RIGHT_CENTER
      },
      zoomControlOptions: {
        position: google.maps.ControlPosition.LEFT_CENTER
      },
      cameraControl: false,
      streetViewControl: false,
      disableDefaultUI: false,
      zoomControl: true,
      tilt: 0
      //draggableCursor: 'vertical-text',
      //draggingCursor: 'vertical-text'
    }
    this.googleMap.googleMap.setOptions(this.options);
  }

  setOperation(op: enum_GISDrawingOperations, isSementieri: boolean) {
    this.drawingManagerService.stopDrawingMode();
    this.operation = op;
    this.callOperation(isSementieri);
  }

  closeAllegatiDaRicettaDialog(result: AllegatiDaRicettaDestinazione | null): void {
    this.dialogListaAllegatiOpen = false;
    this.allegatiDaRicettaSubject.next(result);
  }

  private callOperation(isSementieri) {

    switch (this.operation) {

      case enum_GISDrawingOperations.marker: {
        this.manageLayer();
        this.drawingManagerService.setDrawing_Modes(google.maps.drawing.OverlayType.MARKER);
        this.drawingManagerService.startDrawingMode(true);
        this.showMarkerWindow(MarkerWindowModality.Marker);
        break;
      }

      case enum_GISDrawingOperations.scomponiModificaPunti: {
        this.showMarkerWindow(MarkerWindowModality.Scomponi);
        break;
      }

      case enum_GISDrawingOperations.strumentoDisegnoAvanzato: {
        this.showMarkerWindow(MarkerWindowModality.DisegnoAvanzato);
        break;
      }

      case enum_GISDrawingOperations.strumentoUnisci: {
        this.showPolygonMergeWindow();
        break;
      }

      case enum_GISDrawingOperations.polygon: {
        // this.kendoWindowsService.close(WindowTypes.MarkerWindow);
        this.startDrawingMode();
        break;
      }

      case enum_GISDrawingOperations.erase: {
        this.googleMapGeoJsonService.seDeselezionaFeatureSelezionate(enum_OrigineChiamata.Mappa);
        break;
      }

      case enum_GISDrawingOperations.delete: {
        if (this.featureService.getFeatureSelezionate().length === 0) {
          //this.giasMessageService.errorMessage(this.transloco.translate('gis.SelezionarePoligono'));
          this.giasDialogService.baseError('gis.EliminaFeature', 'gis.SelezionarePoligono');
          break;
        }
        // let altezzaFinestra = 225;
        // if (!this.sharedDataService.getCfgGisGenerali().permessoPrecisionFarming) {
        //   altezzaFinestra -= 50;
        // }
        const windowArgs = new WindowArgs(WindowTypes.DeleteLayerWindow, true, this.transloco.translate("gis.EliminaImpianto"), undefined, 350, undefined, undefined, 200, true, true, true, false, false, false);
        this.kendoWindowsService.open(WindowTypes.DeleteLayerWindow, windowArgs);
        break;
      }

      case enum_GISDrawingOperations.copy: {
        this.gisService.copiaFeature();
        break;
      }

      case enum_GISDrawingOperations.paste: {
        this.gisService.incollaFeature();
        break;
      }

      case enum_GISDrawingOperations.save: {
        if (this.featureService.getFeatureSelezionate().length === 0) {
          // this.giasMessageService.errorMessage("Selezionare prima un poligono da modificare");
          this.giasDialogService.baseError('Modifica feature', 'Selezionare prima un poligono da modificare', false);
          break;
        }
        if (this.featureService.getFeatureSelezionate().length > 1) {
          // this.giasMessageService.errorMessage("Selezionare un solo poligono da modificare");
          this.giasDialogService.baseError('Modifica feature', 'Selezionare un solo poligono da modificare', false);
          break;
        }
        this.editFeatureService.modificaFeature(isSementieri);
        break;
      }

      case enum_GISDrawingOperations.new: {
        if (this.featureService.getFeatureSelezionate().length === 0) {
          // this.giasMessageService.errorMessage("Selezionare prima un poligono");
          this.giasDialogService.baseError('Modifica feature', 'Selezionare prima un poligono', false);
          break;
        }
        if (this.featureService.getFeatureSelezionate().length > 1) {
          // this.giasMessageService.errorMessage("Selezionare un solo poligono");
          this.giasDialogService.baseError('Modifica feature', 'Selezionare un solo poligono', false);
          break;
        }
        this.creaDaPoligonoService.creaImpiantoDaPoligono();
        break;
      }

      case enum_GISDrawingOperations.lineeGuidaAB: {
        this.showLineeGuidaABWindow();
        break;
      }

      case enum_GISDrawingOperations.datiAgricolturaPrecisone: {
        this.loadPrecisionData();
        break;
      }

      default: console.log('Operation not yet implemented');

    }
  }

  private showMarkerWindow(modality: MarkerWindowModality): void {
    const width = 625;
    const title = this.getMarkerWindowTitle(modality);
    const windowArgs = new WindowArgs(WindowTypes.MarkerWindow, true, title, null, width, 250, 100, DEFAULT_TOP_POSITION, true, true, true, true, true, false, true);
    windowArgs.additionalArgs = { modality: modality };
    this.kendoWindowsService.open(windowArgs.windowType, windowArgs);
  }

  private showPolygonMergeWindow(): void {
    const width = 625;
    const title = this.transloco.translate('gis.StrumentoUnisciPoligoni');
    const windowArgs = new WindowArgs(WindowTypes.PolygonMergeWindow, true, title, null, width, 250, 100, DEFAULT_TOP_POSITION, false, false, true, true, true, false, true);
    this.kendoWindowsService.open(windowArgs.windowType, windowArgs);
  }

  private getMarkerWindowTitle(modality: MarkerWindowModality): string {
    switch (modality) {
      case MarkerWindowModality.Marker:
        return this.transloco.translate('gis.StrumentoDisegnoPunti');
      case MarkerWindowModality.Scomponi:
        return this.transloco.translate('gis.StrumentoScomponiModificaPunti');
      case MarkerWindowModality.DisegnoAvanzato:
        return this.transloco.translate('gis.StrumentoDisegnoAvanzato');
      default:
        return "";
    }
  }

  private showLineeGuidaABWindow(): void {
    const width = 250;
    const title = this.transloco.translate('gis.StrumentoLineeGuidaAB');
    const windowArgs = new WindowArgs(WindowTypes.LineeGuidaABWindow, true, title, null, width, 250, 100, DEFAULT_TOP_POSITION, true, true, true, true, true, false, true);
    this.kendoWindowsService.open(windowArgs.windowType, windowArgs);
  }

  private loadPrecisionData(): void {
    const ricetteOpCodes = this.ricetteService.getSelected().map(x => x.Ricetta_Operazione_Cod);
    if (ricetteOpCodes.length == 0) {
      return;
    }

    const feature = this.featureService.getUltimaFeatureSelezionata();
    const chiaveAlbero = this.featureService.getChiaveAlberoCompletaByFeature(feature);
    const objChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(chiaveAlbero);

    const body = {
      Allegati_Documenti_CatCod: 75,
      Appezza: objChiaveAlbero.Appezza,
      Id_Imp: objChiaveAlbero.Id_Imp,
      Piva: objChiaveAlbero.Piva,
      Sa_Cod: objChiaveAlbero.Sa_Cod,
      Ricetta_Operazione_Cod: ricetteOpCodes[0]
    } as LeggiAllegatiDaRicettaDestinazione_In;

    // sub is used to avoid multiple emit of this.allegatiDaRicettaSubject.asObservable()
    const sub: Subscription = this.gisClient
      .gisLeggiAllegatiDaRicettaDestinazione(body)
      .pipe(
        catchError(() => throwError(() => new Error("gis.DatiAgricolturaPrecisoneErrore"))),
        switchMap(res => {
          const lista = res.RispostaStringa.ListaAllegati;
          if (lista.length == 0) {
            this.giasDialogService.baseError("gis.DatiAgricolturaPrecisone", "gis.DatiAgricolturaPrecisoneListaAllegatiVuota");
            return of(null);
          }

          if (lista.length == 1) {
            return of(lista[0].Allegati_Documenti_Cod);
          }

          this.listaAllegati = lista;
          this.dialogListaAllegatiOpen = true;
          return this.allegatiDaRicettaSubject.asObservable().pipe(map(x => x?.Allegati_Documenti_Cod));
        }),
        tap(res => {
          if (res == null) {
            sub.unsubscribe();
          }
          this.listaAllegati = [];
          this.allegatoSelected = null;
        }),
        tap(() => this.masterService.set_isLoading({ isLoading: true })),
        filter(res => res != null),
        switchMap(code => {
          const isActive$ = this.configurazioneSitiService
            .leggiChiave(EnumChiaviConfigurazioneSiti.MappePrescrizioneEngineIsActive)
            .pipe(map(config => config?.Valore?.toLowerCase() === 'true'), take(1));
          return forkJoin([of(code), isActive$]);
        }),
        switchMap(([code, isActive]) => {
          if (isActive && this.mappePrescrizioneRasterService) {
            this.mappePrescrizioneRasterService.loadGroundOverlay(code);
            this.giasDialogService.baseSuccess('gis.DatiAgricolturaPrecisone', 'gis.DatiAgricolturaPrecisoneCaricato');
            this.masterService.set_isLoading({ isLoading: false });
            sub.unsubscribe();
            return of(null);
          }
          return forkJoin([of(code), this.gisClient.gisCaricaDatiPrecisionXmlDaAllegati({ Flag_TipoCod: 1, Lista_RicettaOp_Cod: [code] })])
            .pipe(switchMap(([c, res]) => res == null ? throwError(() => new Error('gis.DatiAgricolturaPrecisoneErrore')) : of([c, res])));
        }),
        filter(res => res != null)
      )
      .subscribe({
        next: ([code, res]: [number, RispostaStandard_1OfGisDataReadRval_New_1OfGeoJSONAgroGisProp]) => {
          this.mappePrescrizioneService.setGeoJsonLoaded({ allegatoCode: code, geoJson: res.RispostaStringa.myGeoJson as any });
          this.googleMapGeoJsonService.addPrecisionDataFeatures(res.RispostaStringa.myGeoJson as any, false);
          this.giasDialogService.baseSuccess("gis.DatiAgricolturaPrecisone", "gis.DatiAgricolturaPrecisoneCaricato");
          this.masterService.set_isLoading({ isLoading: false });
          sub.unsubscribe();
        },
        error: () => {
          this.giasDialogService.baseError("gis.DatiAgricolturaPrecisone", "gis.DatiAgricolturaPrecisoneErrore");
          this.masterService.set_isLoading({ isLoading: false });
          sub.unsubscribe();
        }
      });
  }

  private startDrawingMode() {
    this.manageLayer();
    if (this.layerService.layerItemSelected[0]?.id != '-1') {
      this.drawingManagerService.setDrawing_Modes(this.getDrawingMode());
      this.drawingManagerService.setLayer(this.layerService.layerItemSelected[0])
      this.drawingManagerService.startDrawingMode();
    }
  }

  private getDrawingMode(): google.maps.drawing.OverlayType {
    if (Number.parseInt(this.layerService.layerItemSelected[0]?.FeatureTypeId) == FeatureType.Polygon)
      return google.maps.drawing.OverlayType.POLYGON;
    if (Number.parseInt(this.layerService.layerItemSelected[0]?.FeatureTypeId) == FeatureType.Point)
      return google.maps.drawing.OverlayType.MARKER;
    if (Number.parseInt(this.layerService.layerItemSelected[0]?.FeatureTypeId) == FeatureType.LineString)
      return google.maps.drawing.OverlayType.POLYLINE;
  }

  private manageLayer() {
    if (
      !this.layerService.layerItemSelected[1] ||
      (
        this.sharedDataService.getCfgSementiAsValue() != undefined &&
        this.layerService.LayerSelected.getValue().Option_Value != enum_TipologiaLayer.OrganizzazioneAppartenenza
      )
    ) {
      this.layerService.setLayerItemSelected([this.sharedDataService.getTipologiaLayerById(enum_LayerElementiGraficiStd.IMPIANTI), true]);
    }
  }

  ngOnDestroy(): void {
    this.notificationRef?.hide();
    this.signal.next();
    this.signal.complete();
    this.drawingManagerService.onDestroy();
    google.maps.event.removeListener(this.mapZoomListener);
  }

  public quickGeoJson(): void {
    const geometry = this.featureInformationService.getGeometry(this.featureService.getUltimaFeatureSelezionata().properties.id);
    this.clipboardService.copyObjAsJsonToClipboard(this.wktService.wktToGeoJson(this.wktService.geometryToWKT(geometry)));
    this.giasMessageService.successMessage('gis.CopiatoCorrettamenteInClipboard', false, true);
  }

  public quickWKT(): void {
    const geometry = this.featureInformationService.getGeometry(this.featureService.getUltimaFeatureSelezionata().properties.id);
    this.clipboardService.copyStringToCliboard(this.wktService.geometryToWKT(geometry));
    this.giasMessageService.successMessage('gis.CopiatoCorrettamenteInClipboard', false, true);
  }
}

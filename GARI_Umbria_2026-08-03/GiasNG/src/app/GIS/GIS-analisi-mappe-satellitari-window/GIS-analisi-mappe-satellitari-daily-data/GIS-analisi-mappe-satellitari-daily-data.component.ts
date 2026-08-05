import { Component, OnDestroy } from '@angular/core';
import { catchError, combineLatest, filter, forkJoin, map, Observable, of, shareReplay, switchMap, tap, withLatestFrom } from 'rxjs';
import { DEFAULT_SENSOR, GISAnalisiMappeSatellitariWindowService, MappeSatellitariData, SatelliteDataVisualizationModality } from '../GIS-analisi-mappe-satellitari-window.service';
import { AttivazioneConfigurazioneAlgoritmiCartografici, Gis_Sat_Sentinel_Overlay, GisClient, MascheraLayerRaster, UrlFirmato, LetturaDatiElaboratiSuSensoreListaValori_In, GeoJson_Feature_New_1OfGeoJSONAgroGisProp, AttivaDisattivaConfigurazioneEnum } from 'app/Service/api.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { FeatureService } from 'app/GIS/services/feature.service';
import { BehaviorSubject, debounceTime, take } from 'rxjs';
import { GiasDialogService } from 'gias-ui-kit';
import { Tile } from 'app/GIS/utils/mercator.utils';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { FeatureInformationService } from 'app/GIS/services/feature-information.service';
import { RasterParameterVisualizationLayer, RasterParameterVisualizationType } from 'app/GIS/services/raster-overlay.service';
import { SatelliteOverlayService } from '../satellite-overlay.service';
import { startWith } from 'rxjs';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { SatelliteGlobalDataLoader } from '../satellite-global-data-loader';
import { SatelliteLocalDataLoader } from '../satellite-local-data-loader';
import { SatelliteAnimationService } from 'app/GIS/services/satellite-animation.service';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';

interface DailyData {
  date: Date;
  value: number | null;
  stringValue: string;
  isCloudy: boolean;
}

@Component({
  standalone: false,
  selector: 'gis-analisi-mappe-satellitari-daily-data',
  templateUrl: './GIS-analisi-mappe-satellitari-daily-data.component.html',
  styleUrls: ['./GIS-analisi-mappe-satellitari-daily-data.component.scss']
})
export class GISAnalisiMappeSatellitariDailyDataComponent implements OnDestroy {

  private selectionMarker: google.maps.Marker | null = null;
  private lastSelected: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] | null = null;

  featuresWithNoDataSubject = new BehaviorSubject<string[]>([]);
  errorSubject = new BehaviorSubject<string | null>(null);
  isLoadingDailyValues$ = new BehaviorSubject<boolean>(false);

  active$ = combineLatest([
    this.kendoWindowsService.getWindowArgs$(WindowTypes.AnalisiMappeSatellitariWindow).pipe(map(args => args?.openState ?? false)),
    this.gisAnalisiMappeSatellitariWindowService.modality$.pipe(map(m => m == SatelliteDataVisualizationModality.Daily))
  ])
    .pipe(
      map(([isWindowOpen, isOnAnimationTab]) => isWindowOpen && isOnAnimationTab),
      tap(active => {
        if (!active) {
          this.clearSelectionMarker();
        }
      })
    );

  currentDate$ = this.gisAnalisiMappeSatellitariWindowService.currentDate$;
  masks$ = this.gisAnalisiMappeSatellitariWindowService.masks$;
  sensor$ = this.gisAnalisiMappeSatellitariWindowService.sensor$;
  selectedFeatures$ = this.featureService.getFeatureSelezionate$()
    .pipe(
      debounceTime(100),
      filter(newValue => {
        if (this.lastSelected == null || this.lastSelected.length != newValue.length) {
          this.lastSelected = [...newValue]; // we can't use pairwise nor distinctUntilChanged because it's messing up the flow
          return true;
        }

        // emit ONLY if the selection has changed
        const oldValueSet = new Set(this.lastSelected.map(x => x.properties.id));
        const newValueSet = new Set(newValue.map(x => x.properties.id));

        this.lastSelected = [...newValue];

        for (const oldId of oldValueSet.values()) {
          if (!newValueSet.has(oldId)) {
            return true;
          }
        }

        for (const newId of newValueSet.values()) {
          if (!oldValueSet.has(newId)) {
            return true;
          }
        }

        return false;
      }),
      shareReplay(1)
    );

  mapClickActivator$ = this.selectedFeatures$
    .pipe(tap(features => {
      this.gisAnalisiMappeSatellitariWindowService.nextMapInfo(features.length > 0);
    }));

  calendarCurrentDateloader$ = this.gisAnalisiMappeSatellitariWindowService
    .calendarSatelliteData$
    .pipe(
      withLatestFrom(this.currentDate$),
      tap(([satelliteData, currentDate]: [Gis_Sat_Sentinel_Overlay[], Date | null]) => {
        if (currentDate == null && satelliteData.length > 0) {
          this.changeCurrentDate(GISAnalisiMappeSatellitariWindowService.stringToDate(satelliteData[satelliteData.length - 1].DataRiferimento));
        }
      })
    );

  // we subscribe to this only if active$ is true
  noFeaturesSelected$ = this.selectedFeatures$
    .pipe(
      tap(() => this.featuresWithNoDataSubject.next([])), // reset on change
      map(features => features == null || features.length == 0)
    );

  featureLoadedCentroids$ = this.selectedFeatures$
    .pipe(
      filter(features => features != null && features.length > 0),
      tap(features => this.gisAnalisiMappeSatellitariWindowService.nextMapClick({
        latLng: this.computePolygonCentroid(features[0]),
        feature: this.googleMapService.googleMapWrapper.data.getFeatureById(features[0].properties.id)
      } as google.maps.Data.MouseEvent)
      )
    );

  featureClicked$ = this.gisAnalisiMappeSatellitariWindowService.dailyMapClickEvent$
    .pipe(
      filter(ev => {
        const features = this.featureService.getFeatureSelezionate();
        if (!features || features.length === 0) {
          // if there is no selected feature or no overlays, ignore
          return false;
        }

        if (ev?.feature != null && features.find(f => f.properties.id == ev.feature.getId()) == null) {
          // user clicked on a different feature, this case will be handled after reloading the data
          return false;
        }

        return true;
      }),
      map(ev => [ev.latLng])
    );

  markerPosition$ = this.featureClicked$
    .pipe(
      map(latlngs => {
        const features = this.featureService.getFeatureSelezionate();
        if (features.length == 0 || latlngs.length == 0) {
          return null;
        }

        const latlng = latlngs[0];
        return latlng;
      }),
      filter(latlng => latlng != null)
    );

  marker$ = this.markerPosition$
    .pipe(tap(latlng => {
      const features = this.featureService.getFeatureSelezionate();
      if (features.length == 0 || latlng == null) {
        return;
      }

      for (const feature of features) {
        const path = this.featureInformationService.getPath(feature.properties.id);
        const polygon = new google.maps.Polygon();
        polygon.setPath(path);
        if (google.maps.geometry.poly.containsLocation(latlng, polygon)) {
          // move marker to clicked position
          if (this.selectionMarker == null) {
            this.selectionMarker = new google.maps.Marker({ map: this.googleMapService.googleMapWrapper.googleMap, position: latlng, draggable: false, clickable: false, title: feature.properties.id, zIndex: 10001 });
          } else {
            this.selectionMarker.setPosition(latlng);
          }
        }
      }
    }));

  dailyValues$: Observable<DailyData[]> =
    combineLatest({
      latlng: this.markerPosition$.pipe(startWith(this.selectionMarker?.getPosition())),
      sensor: this.gisAnalisiMappeSatellitariWindowService.sensor$,
      calendarData: this.gisAnalisiMappeSatellitariWindowService.calendarSatelliteData$,
      currentDate: this.currentDate$
    })
      .pipe(
        debounceTime(100),
        filter(() => this.kendoWindowsService.getOpenState(WindowTypes.AnalisiMappeSatellitariWindow)),
        tap(() => this.isLoadingDailyValues$.next(false)),
        filter(() => {
          const features = this.featureService.getFeatureSelezionate();
          return features.length > 0;
        }),
        switchMap(data => {
          const emptyRes = of([] as DailyData[]);
          if (data.calendarData.length == 0) {
            return emptyRes;
          }

          const gMap = this.googleMapService.googleMapWrapper?.data?.getMap();
          if (gMap == null) {
            return emptyRes;
          }

          const position = data.latlng;
          if (!position) {
            return emptyRes;
          }

          const zoom = gMap.getZoom();
          const toDDMMYYYY = (d: Date) => `${GISAnalisiMappeSatellitariDailyDataComponent.getFormattedValue(d.getDate())}/${GISAnalisiMappeSatellitariDailyDataComponent.getFormattedValue(d.getMonth() + 1)}/${d.getFullYear()}`;

          const allDates = data.calendarData
            .map(o => GISAnalisiMappeSatellitariWindowService.stringToDate(o.DataRiferimento));

          const dates = Array.from(new Map(allDates.map(date => [date.getTime(), date])).values())
            .filter((value, index, array) => array.indexOf(value) === index)
            .filter(d => d <= data.currentDate)
            .sort((a, b) => a.getTime() - b.getTime())
            .slice(-3);

          // Add 1 day to DataFine
          const dataFine = new Date(dates[dates.length - 1]);
          dataFine.setDate(dataFine.getDate() + 1);

          const payload: LetturaDatiElaboratiSuSensoreListaValori_In = {
            DataInizio: toDDMMYYYY(dates[0]),
            DataFine: toDDMMYYYY(dataFine),
            PoligonoWKT: `POINT(${position.lng()} ${position.lat()})`,
            Sensore: data.sensor,
            Zoom: zoom
          } as any;

          this.isLoadingDailyValues$.next(true);
          return this.gisClient
            .gisLetturaDatiElaboratiSuSensoreListaValori(payload)
            .pipe(
              map(result => JSON.parse(result.RispostaStringa)),
              catchError(() => of([] as DailyData[])),
              map((arr: { DataRiferimento: string, Valore: string, isCloudy: boolean }[]) => dates.map(d => {
                const element = arr.find(v => GISAnalisiMappeSatellitariWindowService.stringToDate(v.DataRiferimento).toDateString() === d.toDateString());
                const isCloudy = data.calendarData
                  .find(x => GISAnalisiMappeSatellitariWindowService.stringToDate(x.DataRiferimento).toDateString() === d.toDateString())
                  ?.Passaggi
                  ?.every(x => x.url == '') ?? true;
                return ({
                  date: d,
                  stringValue: element?.Valore?.toString() ?? 'NaN',
                  value: element?.Valore == null ? null : +element?.Valore,
                  isCloudy: isCloudy
                } as DailyData);
              }
              )),
            );
        }),
        catchError(() => of([] as DailyData[])),
        tap(() => this.isLoadingDailyValues$.next(false))
      );

  sensors$: Observable<string[]> = this.gisAnalisiMappeSatellitariWindowService.satelliteData$
    .pipe(
      withLatestFrom(this.gisAnalisiMappeSatellitariWindowService.sensor$),
      map(([data, sensor]) => this.buildSensors(data, sensor))
    );

  reloader$ = this.gisAnalisiMappeSatellitariWindowService.masks$
    .pipe(
      switchMap(masks => {
        const hasMasks = masks.filter(x => x.isAttivaPerUtenteCorrente).length > 0;
        if (hasMasks) {
          return this.satelliteLocalDataLoader.load();
        }

        return this.satelliteGlobalDataLoader.load();
      })
    );

  data$ = combineLatest({
    features: this.selectedFeatures$,
    layersOnTiles: this.reloader$,
    sensor: this.gisAnalisiMappeSatellitariWindowService.sensor$,
    currentDate: this.currentDate$,
    opacity: this.gisAnalisiMappeSatellitariWindowService.opacity$
  }).pipe(
    withLatestFrom(this.gisAnalisiMappeSatellitariWindowService.masks$),
    switchMap(([data, masks]) => this.loadMappeSatellitariData(
      data.layersOnTiles.layers as Gis_Sat_Sentinel_Overlay[],
      data.layersOnTiles.tiles as Tile[],
      data.currentDate,
      data.sensor,
      data.layersOnTiles.features as GeoJson_Feature_New_1OfGeoJSONAgroGisProp[],
      data.opacity,
      masks)
    )
  );

  imageLoader$ = this.gisAnalisiMappeSatellitariWindowService.overlays$
    .pipe(tap(overlays => this.satelliteAnimationService.loadAllFrames(overlays)));

  imageShower$ = combineLatest({
    loader: this.imageLoader$,
    sensor: this.sensor$,
    opacity: this.gisAnalisiMappeSatellitariWindowService.opacity$,
  }).pipe(
    withLatestFrom(this.currentDate$),
    tap(([data, currentDate]) => this.satelliteAnimationService.showAnimationStep(this.dateToString(currentDate), data.opacity)));

  constructor(
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    private featureService: FeatureService,
    private featureInformationService: FeatureInformationService,
    private giasDialogService: GiasDialogService,
    private googleMapService: GoogleMapService,
    private permessiUtenteService: PermessiUtenteService,
    private gisClient: GisClient,
    private satelliteAnimationService: SatelliteAnimationService,
    private satelliteOverlayService: SatelliteOverlayService,
    private satelliteLocalDataLoader: SatelliteLocalDataLoader,
    private satelliteGlobalDataLoader: SatelliteGlobalDataLoader,
    private kendoWindowsService: KendoWindowsService,
    private configurazioneSitiService: ConfigurazioneSitiService
  ) {
  }

  ngOnDestroy(): void {
    this.clearSelectionMarker();
  }

  private clearSelectionMarker(): void {
    this.gisAnalisiMappeSatellitariWindowService.nextMapInfo(false);
    if (this.selectionMarker != null) {
      this.selectionMarker.setMap(null);
      this.selectionMarker = null;
    }
  }

  private static getFormattedValue(value: number): string {
    return `${value}`.padStart(2, '0');
  }

  getSatelliteData(): void {
    if (this.featuresWithNoDataSubject.value.length == 0) {
      return;
    }

    const codes = this.featuresWithNoDataSubject.value.map(c => +c);
    this.satelliteOverlayService
      .loadSatelliteAlgorithm(codes[0])
      .pipe(
        map(res => ({
          configurazioneProiezione_Cod: res,
          layer_cod: 0,
          isAttivo: true,
          tipologia_layer_cod: 1,
          listaEntita: SatelliteOverlayService.getEntitiesFromFeatures(codes)
        } as AttivazioneConfigurazioneAlgoritmiCartografici)),
        switchMap(body => this.gisClient.gisAttivaDisattivaConfigurazione(body)))
      .subscribe({
        next: (res => {
          if (res.RispostaStringa.Risultato == AttivaDisattivaConfigurazioneEnum.AttivatoCorrettamente) {
            this.giasDialogService.baseSuccess('', 'gis.DatiSatellitariRichiestiCorrettamente', true);
          } else if (res.RispostaStringa.Risultato == AttivaDisattivaConfigurazioneEnum.GiaAttivato) {
            this.giasDialogService.baseInfo('', 'gis.RichiestaDeiDatiSatellitariGiaAccodata', true);
          } else if (res.RispostaStringa.Risultato == AttivaDisattivaConfigurazioneEnum.ErroreAttivazione) {
            this.giasDialogService.baseError('', 'gis.ErroreDuranteLaRichiestaDeiDatiSatellitari', true);
          } else if (res.RispostaStringa.Risultato == AttivaDisattivaConfigurazioneEnum.NonAutorizzato) {
            this.giasDialogService.baseError('', 'gis.NonHaiIPermessiPerRichiedereLAttivazioneDeiDatiSatellitari', true);
          }
        }),
        error: () => this.giasDialogService.baseError('', 'gis.ErroreDuranteLaRichiestaDeiDatiSatellitari', true)
      });
  }

  changeSensor(sensor: string): void {
    this.gisAnalisiMappeSatellitariWindowService.nextSensor(sensor);
  }

  changeCurrentDate(date: Date): void {
    this.gisAnalisiMappeSatellitariWindowService.nextCurrentDate(date);
  }

  private buildSensors(data: Gis_Sat_Sentinel_Overlay[], sensor: string): string[] {
    const sensors = data.flatMap(dataP => dataP.Passaggi)
      .flatMap(passaggio => passaggio.Sensore.map(sensorP => sensorP.CodiceSensore))
      .filter((value, index, array) => array.indexOf(value) === index);

    if (sensors.find(s => s == sensor) == null) {
      this.changeSensor(sensors[0] ?? DEFAULT_SENSOR);
    }

    return sensors;
  }

  private computeFeaturesWithNoData(features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]) {
    const codes = features.map(f => f.properties.Entita_Cod);

    forkJoin(codes.map(code => this.gisClient.gisLeggiLogEsecuzioniConfigurazione(+code)))
      .subscribe(logs => {
        const featuresWithNoData = [];
        for (let i = 0; i < codes.length; i++) {
          if (logs[i].RispostaStringa.elencoLogEsecuzioniConfigurazione.length == 0) {
            featuresWithNoData.push(codes[i]);
          }
        }

        this.featuresWithNoDataSubject.next(featuresWithNoData);
      });
  }

  private computePolygonCentroid(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): google.maps.LatLng {
    const Mappa = require('../../../GiasJSLibraries/GIS-js-libraries/Mappa');
    const path = this.featureInformationService.getPath(feature.properties.id);
    const position: google.maps.LatLng = Mappa.polylabel(path);
    return position;
  }

  private loadMappeSatellitariData(overlays: Gis_Sat_Sentinel_Overlay[], tiles: Tile[], currentDate: Date, sensor: string, features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[], opacity: number, masks: MascheraLayerRaster[]): Observable<any> {
    const applyToPolygon = SatelliteOverlayService.applyOverlaysOnPolygons(this.permessiUtenteService, masks);
    if (applyToPolygon && overlays.length == 0 && features.length > 0) {
      this.gisAnalisiMappeSatellitariWindowService.nextOverlays([]);
      this.featuresWithNoDataSubject.next(features.map(f => f.properties.Entita_Cod));
      return of(null);
    }

    if (currentDate == null) {
      this.gisAnalisiMappeSatellitariWindowService.nextOverlays([]);
      return of(null);
    }

    const groupedOverlays = overlays.filter(element => {
      const date = GISAnalisiMappeSatellitariWindowService.stringToDate(element.DataRiferimento);
      // Fix time to be sure it does not effect dates
      date.setHours(0);
      currentDate.setHours(23);
      return date <= currentDate;
    })
      // .filter(overlay => overlay.Passaggi.some(x => x.url != ''))
      .reduce((acc, current) => {
        const filteredCurrent = { ...current };
        filteredCurrent.Passaggi = filteredCurrent.Passaggi.filter(x => x.Sensore.find(s => s.CodiceSensore == sensor) != null);

        const existing = acc.find(x => x.DataRiferimento == current.DataRiferimento);
        if (existing == null) {
          acc.push(filteredCurrent);
        } else {
          existing.Passaggi = existing.Passaggi.concat(filteredCurrent.Passaggi);
        }
        return acc;
      }, [] as Gis_Sat_Sentinel_Overlay[]);

    const validOverlays = groupedOverlays
      // .filter(overlay => overlay.Passaggi.some(x => x.url != ''))
      .sort((a, b) => {
        const d1 = GISAnalisiMappeSatellitariWindowService.stringToDate(b.DataRiferimento);
        const d2 = GISAnalisiMappeSatellitariWindowService.stringToDate(a.DataRiferimento);

        if (d1 > d2) {
          return -1;
        } else if (d1 < d2) {
          return 1;
        }
        return 0;
      });

    const lastThreeOverlays = validOverlays.slice(Math.max(validOverlays.length - 3, 0)); // Take the last 3 available dates
    if (applyToPolygon && overlays.length > 0 && lastThreeOverlays.length == 0 && features.length > 0) {
      this.errorSubject.next('gis.NonCiSonoDatiSatellitariPrimaDellaDataSelezionata');
      this.gisAnalisiMappeSatellitariWindowService.nextOverlays([]);
      return of(null);
    }

    if (!applyToPolygon && lastThreeOverlays.length == 0) {
      this.gisAnalisiMappeSatellitariWindowService.nextOverlays([]);
      return of(null);
    }

    const isEngineActive$ = this.configurazioneSitiService.leggiChiave(EnumChiaviConfigurazioneSiti.SatEngineIsActive)
    .pipe(
      map(config => config?.Valore?.toLowerCase() === 'true'),
      take(1)
    );

    const lastOverlay = lastThreeOverlays[lastThreeOverlays.length - 1];
    return this.gisClient.gisLeggiBaseUrlMappeSatellitari()
      .pipe(
        withLatestFrom(isEngineActive$),
        switchMap(([res, isEngineActive]) => {
          if (res.RispostaStringa.legacy_endpoint != null && res.RispostaStringa.legacy_endpoint !== '') {
            const layerParam = {} as RasterParameterVisualizationLayer;
            layerParam.type = RasterParameterVisualizationType.SATELLITE;
            layerParam.baseUrl = `${res.RispostaStringa.legacy_endpoint}/${lastOverlay.Passaggi[0].url}/${sensor}/`;
            return this.satelliteOverlayService.handleVisualizationParameter([layerParam], tiles, lastOverlay, sensor);
          }

          const layerParams: RasterParameterVisualizationLayer[] = [];
          for (const passaggio of lastOverlay.Passaggi) {
            const layerParam = {} as RasterParameterVisualizationLayer;
            const data = res.RispostaStringa.datiEndpoint;
            layerParam.type = data.type;
            if (isEngineActive) {
              layerParam.obj = sensor;
            } else {
              layerParam.baseUrl = data.baseUrl;
              layerParam.bucket = data.bucket;
              layerParam.obj = `${data.obj}/${passaggio.url}/${sensor}`;
            }
            layerParams.push(layerParam);
          }

          return this.satelliteOverlayService.handleVisualizationParameter(layerParams, tiles, lastOverlay, sensor);
        }),
        withLatestFrom(isEngineActive$),
        tap(([result, isEngineActive]: [[UrlFirmato | UrlFirmato[], Gis_Sat_Sentinel_Overlay], boolean]) => {
          const resultOverlay = { sensor: sensor, overlay: result[1], urls: result[0], features: features, opacity: opacity, applyToPolygon: applyToPolygon } as MappeSatellitariData;
          if (applyToPolygon && features.length > 0 && !SatelliteOverlayService.hasValidFeatures(isEngineActive, [resultOverlay])) {
            this.computeFeaturesWithNoData(features);
            this.clearSelectionMarker();
            return;
          }

          this.gisAnalisiMappeSatellitariWindowService.nextOverlays([resultOverlay]);
          this.gisAnalisiMappeSatellitariWindowService.nextDateFrom(
            GISAnalisiMappeSatellitariWindowService.stringToDate(lastThreeOverlays[0].DataRiferimento)
          );

          this.gisAnalisiMappeSatellitariWindowService.nextDateTo(
            GISAnalisiMappeSatellitariWindowService.stringToDate(lastOverlay.DataRiferimento)
          );

          this.gisAnalisiMappeSatellitariWindowService.nextMapInfo(true);
        })
      );
  }

  private dateToString(date: Date | null): string | null {
    if (date == null) {
      return null;
    }

    const yyyy = date.getFullYear();
    const mmInt = date.getMonth() + 1;
    const ddInt = date.getDate();

    const dd = ddInt < 10 ? `0${ddInt}` : `${ddInt}`;
    const mm = mmInt < 10 ? `0${mmInt}` : `${mmInt}`;
    const formattedToday = yyyy + '-' + mm + '-' + dd;

    return formattedToday;
  }
}

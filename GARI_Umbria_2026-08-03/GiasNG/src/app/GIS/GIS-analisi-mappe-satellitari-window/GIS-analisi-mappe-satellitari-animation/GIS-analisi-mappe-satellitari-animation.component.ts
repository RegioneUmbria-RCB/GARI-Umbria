import { Component, Input } from '@angular/core';
import { BehaviorSubject, catchError, combineLatest, filter, forkJoin, interval, map, Observable, of, startWith, switchMap, take, tap, withLatestFrom } from 'rxjs';
import { AnimationData, AnimationStatus, DEFAULT_SENSOR, GISAnalisiMappeSatellitariWindowService, MappeSatellitariData, SatelliteDataVisualizationModality } from '../GIS-analisi-mappe-satellitari-window.service';
import { SatelliteAnimationService } from 'app/GIS/services/satellite-animation.service';
import { Gis_Sat_Sentinel_Overlay, MascheraLayerRaster, UrlFirmato, GisClient, AttivazioneConfigurazioneAlgoritmiCartografici, GeoJSONAgroGisProp, GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from 'app/Service/api.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { SatelliteOverlayService } from '../satellite-overlay.service';
import { SelectionRange } from '@progress/kendo-angular-dateinputs';
import { Tile } from 'app/GIS/utils/mercator.utils';
import { RasterParameterVisualizationLayer, RasterParameterVisualizationType } from 'app/GIS/services/raster-overlay.service';
import { SatelliteLocalDataLoader } from '../satellite-local-data-loader';
import { SatelliteGlobalDataLoader } from '../satellite-global-data-loader';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { FeatureService } from 'app/GIS/services/feature.service';
import { GiasDialogService } from 'gias-ui-kit';
import { TranslocoService } from '@jsverse/transloco';
import { NotificationRef } from '@progress/kendo-angular-notification';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { GISAnalisiMappeSatellitariWindowComponent } from '../GIS-analisi-mappe-satellitari-window.component';
import { MasterService } from 'app/Service/master.service';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';

@Component({
  standalone: false,
  selector: 'gis-analisi-mappe-satellitari-animation',
  templateUrl: './GIS-analisi-mappe-satellitari-animation.component.html',
  styleUrls: ['./GIS-analisi-mappe-satellitari-animation.component.scss']
})
export class GISAnalisiMappeSatellitariAnimationComponent {

  active$ = combineLatest([
    this.kendoWindowsService.getWindowArgs$(WindowTypes.AnalisiMappeSatellitariWindow).pipe(map(args => args?.openState ?? false)),
    this.gisAnalisiMappeSatellitariWindowService.modality$.pipe(map(m => m == SatelliteDataVisualizationModality.Animation))
  ])
    .pipe(map(([isWindowOpen, isOnAnimationTab]) => isWindowOpen && isOnAnimationTab));

  // we subscribe to this only if active$ is true
  noFeaturesSelected$ = this.featureService.getFeatureSelezionate$()
    .pipe(
      tap(features => {
        this.gisAnalisiMappeSatellitariWindowService.nextMapInfo(features.length > 0);
      }),
      map(features => {
        const noFeatureSelected = features == null || features.length == 0;

        if (noFeatureSelected) {
          this.featuresWithNoDataSubject.next([]);
        }

        return noFeatureSelected;
      })
    );

  featuresWithNoDataSubject = new BehaviorSubject<string[]>([]);
  errorSubject = new BehaviorSubject<string | null>(null);

  dates$ = combineLatest([this.gisAnalisiMappeSatellitariWindowService.dateFrom$, this.gisAnalisiMappeSatellitariWindowService.dateTo$])
    .pipe(map(([from, to]) => ({ start: from, end: to } as SelectionRange)));

  masks$ = this.gisAnalisiMappeSatellitariWindowService.masks$;
  sensors$: Observable<string[]> = this.gisAnalisiMappeSatellitariWindowService.satelliteData$
    .pipe(
      withLatestFrom(this.gisAnalisiMappeSatellitariWindowService.sensor$),
      map(([data, sensor]) => this.buildSensors(data, sensor)))

  animation$: Observable<AnimationData>;
  animationUpdater$: Observable<any>;
  animationLoader$: Observable<any>;
  overlaysLoader$: Observable<any>;
  animationStatus = AnimationStatus;
  isCloudy$: Observable<boolean> = combineLatest([
    this.gisAnalisiMappeSatellitariWindowService.animationStep$,
    this.gisAnalisiMappeSatellitariWindowService.overlays$,
    this.gisAnalisiMappeSatellitariWindowService.sensor$,
  ])
    .pipe(
      map(([step, overlays, sensor]) => {
        if (step < 0 || step >= overlays.length) {
          return false;
        }

        const passaggi = overlays[step].overlay.Passaggi;
        if (passaggi.length == 0) {
          return false;
        }

        return passaggi.find(x => x.Sensore.some(y => y.CodiceSensore == sensor))?.url == '';
      })
    );

  private notificationRef: NotificationRef | null = null;
  private dialogRef: DialogRef | null = null;

  constructor(
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    private satelliteAnimationService: SatelliteAnimationService,
    private permessiUtenteService: PermessiUtenteService,
    private gisClient: GisClient,
    private satelliteLocalDataLoader: SatelliteLocalDataLoader,
    private satelliteGlobalDataLoader: SatelliteGlobalDataLoader,
    private satelliteOverlayService: SatelliteOverlayService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private featureService: FeatureService,
    private translocoService: TranslocoService,
    private kendoWindowsService: KendoWindowsService,
    private masterService: MasterService,
    private configurazioneSitiService: ConfigurazioneSitiService
  ) {
    this.animationUpdater$ = interval(2500)
      .pipe(
        withLatestFrom(this.gisAnalisiMappeSatellitariWindowService.animationStatus$, this.gisAnalisiMappeSatellitariWindowService.overlays$),
        filter(([_, status, overlays]) => status == AnimationStatus.Started && overlays.length > 0),
        withLatestFrom(this.gisAnalisiMappeSatellitariWindowService.animationStep$),
        map(([[_1, _2, overlays], step]) => [overlays, step]),
        tap(([overlays, step]: [MappeSatellitariData[], number]) => {
          this.gisAnalisiMappeSatellitariWindowService.nextAnimationStep(step + 1);
          if (step >= overlays.length - 1) {
            this.gisAnalisiMappeSatellitariWindowService.nextAnimationStatus(AnimationStatus.Stopped);
          }
        }),
      );

    this.animationLoader$ = this.gisAnalisiMappeSatellitariWindowService.overlays$
      .pipe(tap(overlays => this.satelliteAnimationService.loadAllFrames(overlays)));

    this.animation$ = combineLatest({
      status: this.gisAnalisiMappeSatellitariWindowService.animationStatus$,
      sensor: this.gisAnalisiMappeSatellitariWindowService.sensor$,
      overlays: this.gisAnalisiMappeSatellitariWindowService.overlays$,
      step: this.gisAnalisiMappeSatellitariWindowService.animationStep$,
      opacity: this.gisAnalisiMappeSatellitariWindowService.opacity$,
    }).pipe(
      tap(data => {
        if (data.status != AnimationStatus.ToBeLoaded && data.overlays.length > 0) {
          this.computeAnimationStep(data.step, data.overlays, data.opacity);
        }
      })
    );

    // Handle external load and overlays retrieval for animation modality
    const calendarLoader$ = combineLatest([
      this.gisAnalisiMappeSatellitariWindowService.externalLoadOnPolygons$,
      this.gisAnalisiMappeSatellitariWindowService.masks$
    ])
      .pipe(
        tap(() => this.gisAnalisiMappeSatellitariWindowService.nextOverlays([])),
        switchMap(([_, masks]) => {
          const hasMasks = masks.filter(x => x.isAttivaPerUtenteCorrente).length > 0;
          return hasMasks
            ? this.satelliteLocalDataLoader.load()
            : this.satelliteGlobalDataLoader.load();
        }),
        withLatestFrom(this.gisAnalisiMappeSatellitariWindowService.animationStatus$),
        filter(([_, status]) => status != AnimationStatus.ToBeLoaded),
        map(([data, _]) => data)
      );

    this.overlaysLoader$ = calendarLoader$
      .pipe(
        withLatestFrom(
          this.gisAnalisiMappeSatellitariWindowService.dateFrom$,
          this.gisAnalisiMappeSatellitariWindowService.dateTo$,
          this.gisAnalisiMappeSatellitariWindowService.sensor$,
          this.gisAnalisiMappeSatellitariWindowService.opacity$,
          this.gisAnalisiMappeSatellitariWindowService.masks$
        ),
        tap(() => this.masterService.set_isLoading({ isLoading: true })),
        switchMap(([ data, dateFrom, dateTo, sensor, opacity, masks]) => this.loadMappeSatellitariData(data.layers, data.tiles, dateFrom, dateTo, sensor, data.features, opacity, masks)),
        catchError(() => of(null)),
        tap(() => this.masterService.set_isLoading({ isLoading: false }))
      );
  }

  changeSensor(sensor: string): void {
    this.gisAnalisiMappeSatellitariWindowService.nextSensor(sensor);
  }

  changeDates(dates: SelectionRange): void {
    this.gisAnalisiMappeSatellitariWindowService.nextDateFrom(dates.start);
    this.gisAnalisiMappeSatellitariWindowService.nextDateTo(dates.end);
  }

  nextStatus(status: AnimationStatus): void {
    this.gisAnalisiMappeSatellitariWindowService.nextAnimationStatus(status);
  }

  reset(): void {
    this.nextStatus(AnimationStatus.ToBeLoaded);
    this.gisAnalisiMappeSatellitariWindowService.nextAnimationStep(0);
  }

  reload(masks: MascheraLayerRaster[]): void {
    this.nextStatus(AnimationStatus.Loading);
    const applyOnPolygons = SatelliteOverlayService.applyOverlaysOnPolygons(this.permessiUtenteService, masks);
    this.gisAnalisiMappeSatellitariWindowService.nextExternalLoadOnPolygons(applyOnPolygons);
  }

  goToFirstStep(): void {
    this.gisAnalisiMappeSatellitariWindowService.nextAnimationStep(0);
  }

  previousStep(): void {
    this.gisAnalisiMappeSatellitariWindowService.goNextAnimationStep(true);
  }

  nextStep(): void {
    this.gisAnalisiMappeSatellitariWindowService.goNextAnimationStep(false);
  }

  goToLastStep(): void {
    this.gisAnalisiMappeSatellitariWindowService.goLastAnimationStep();
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
        next: () => this.giasDialogService.baseSuccess('', 'OperazioneRiuscita', true),
        error: () => this.giasDialogService.baseError('', 'ErroreModifica', true)
      });
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

  private computeAnimationStep(step: number, data: MappeSatellitariData[], opacity: number): void {
    const date: string = this.getOverlayCurrentDate(step, data);
    if (date != null) {
      this.satelliteAnimationService.showAnimationStep(date, opacity);
      this.gisAnalisiMappeSatellitariWindowService.nextCurrentDate(GISAnalisiMappeSatellitariWindowService.stringToDate(date));
    }
  }

  private getOverlayCurrentDate(step: number, data: MappeSatellitariData[]): string {
    return data[step]?.overlay?.DataRiferimento;
  }

  private loadMappeSatellitariData(overlays: Gis_Sat_Sentinel_Overlay[], tiles: Tile[], date1: Date, date2: Date, sensor: string, features: any[], opacity: number, masks: MascheraLayerRaster[]): Observable<any> {
    const applyToPolygon = SatelliteOverlayService.applyOverlaysOnPolygons(this.permessiUtenteService, masks);
    if (applyToPolygon && overlays.length > 0 && overlays.length == 0 && features.length > 0) {
      this.notificationRef?.hide();
      this.notificationRef = this.giasMessageService.infoMessagge('gis.NonCiSonoDatiSatellitariDisponibiliNellIntervalloDiTempoSelezionato', false, true);
      this.gisAnalisiMappeSatellitariWindowService.nextOverlays([]);
      return of(null);
    }

    if (date1 == null || date2 == null) {
      this.gisAnalisiMappeSatellitariWindowService.nextOverlays([]);
      return of(null);
    }

    const validOverlays = overlays.filter(element => {
      const date = GISAnalisiMappeSatellitariWindowService.stringToDate(element.DataRiferimento);
      // Fix time to be sure it does not effect dates
      date1.setHours(0);
      date.setHours(11);
      date2.setHours(23);
      return date >= date1 && date <= date2;
    });

    if (applyToPolygon && overlays.length == 0 && features.length > 0) {
      this.gisAnalisiMappeSatellitariWindowService.nextOverlays([]);
      this.showNoFeatureDataError(features);
      return of(null);
    }

    if (!applyToPolygon && validOverlays.length == 0) {
      this.gisAnalisiMappeSatellitariWindowService.nextOverlays([]);
      return of(null);
    }

    const isEngineActive$ = this.configurazioneSitiService.leggiChiave(EnumChiaviConfigurazioneSiti.SatEngineIsActive)
      .pipe(
        map(config => config?.Valore?.toLowerCase() === 'true'),
        take(1)
      );

    return this.gisClient.gisLeggiBaseUrlMappeSatellitari()
      .pipe(
        switchMap(res => {
          if (validOverlays.length == 0) {
            return of([]);
          }

          // If we have multiple dates, group them together
          const groupedOverlays = validOverlays.reduce((acc, current) => {
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

          const requests = groupedOverlays.map(overlay => {
            if (res.RispostaStringa.legacy_endpoint != null && res.RispostaStringa.legacy_endpoint !== '') {
              const layerParam = {} as RasterParameterVisualizationLayer;
              layerParam.type = RasterParameterVisualizationType.SATELLITE;
              layerParam.baseUrl = `${res.RispostaStringa.legacy_endpoint}/${overlay.Passaggi[0].url}/${sensor}/`;
              return this.satelliteOverlayService.handleVisualizationParameter([layerParam], tiles, overlay, sensor);
            }

            const layerParams: RasterParameterVisualizationLayer[] = [];
            for (const passaggio of overlay.Passaggi) {
              const layerParam = {} as RasterParameterVisualizationLayer;
              const data = res.RispostaStringa.datiEndpoint;
              layerParam.type = data.type;
              layerParam.obj = sensor;
              layerParams.push(layerParam);
            }

            return this.satelliteOverlayService.handleVisualizationParameter(layerParams, tiles, overlay, sensor);
          });

          return forkJoin(requests);
        }),
        withLatestFrom(isEngineActive$),
        tap(([result, isEngineActive]: [[UrlFirmato | UrlFirmato[], Gis_Sat_Sentinel_Overlay][], boolean]) => {
          const resultOverlays = result.map(([url, overlay]) => ({ sensor: sensor, overlay: overlay, urls: url, features: features, opacity: opacity, applyToPolygon: applyToPolygon } as MappeSatellitariData));
          if (applyToPolygon && features.length > 0 && !SatelliteOverlayService.hasValidFeatures(isEngineActive, resultOverlays)) {
            this.showNoFeatureDataError(features);
            return;
          }

          if (resultOverlays.length > 0) {
            this.gisAnalisiMappeSatellitariWindowService.nextMapInfo(true);
          }

          this.gisAnalisiMappeSatellitariWindowService.nextOverlays(resultOverlays);
        })
      );
  }

  private showNoFeatureDataError(features: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]): void {
    if (this.dialogRef != null) {
      return;
    }

    const codes = features.map(feature => +feature.properties.Entita_Cod);
    //const timeout = interval(20000).pipe(take(1), map(() => ({ returnObj: false })));

    forkJoin(codes.map(code => this.gisClient.gisLeggiLogEsecuzioniConfigurazione(code)))
      .pipe(
        map(responses => {
          const noFeaturesCode = [];
          for (let i = 0; i < codes.length; i++) {
            if (responses[i].RispostaStringa.elencoLogEsecuzioniConfigurazione.length == 0) {
              noFeaturesCode.push(codes[i]);
            }
          }
          return noFeaturesCode
        }),
        filter(noFeaturesCode => noFeaturesCode.length > 0),
        switchMap(noFeatureCodes => {

          const title = this.translocoService.translate('gis.QuestoPoligonoConCodiceNonHaDatiSatellitari', { codes: noFeatureCodes.join(', ') });
          this.dialogRef = this.giasDialogService.dialogMessageRef('', title);
          return this.dialogRef.result;
        })
      )
      .subscribe((result: any) => {
        if (result?.returnObj) {
          this.enableSatelliteAlgorithm(codes);
        }

        this.dialogRef.close();
        this.dialogRef = null;
      });
  }

  private async enableSatelliteAlgorithm(codes: number[]) {
    const body = await this.satelliteOverlayService.buildAttivazioneBody(codes) as AttivazioneConfigurazioneAlgoritmiCartografici;

    this.gisClient
      .gisAttivaDisattivaConfigurazione(body)
      .subscribe({
        next: () => this.giasDialogService.baseSuccess('', 'OperazioneRiuscita', true),
        error: () => this.giasDialogService.baseError('', 'ErroreModifica', true)
      });
  }
}

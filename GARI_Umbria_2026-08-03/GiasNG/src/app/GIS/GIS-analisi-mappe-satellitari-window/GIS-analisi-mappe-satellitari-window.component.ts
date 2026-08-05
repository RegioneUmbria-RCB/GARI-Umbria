import { Component } from '@angular/core';
import { SelectEvent } from '@progress/kendo-angular-layout';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { GisClient } from 'app/Service/api.service';
import { combineLatest, filter, forkJoin, map, Observable, pairwise, startWith, Subject, switchMap, tap, withLatestFrom } from 'rxjs';
import { enum_LayerElementiGraficiStd } from '../GIS-enum/GIS-layer-elementi-grafici';
import { GoogleMapService } from '../google-map/google-map.service';
import { GISAnalisiMappeSatellitariWindowService, SatelliteDataVisualizationModality } from './GIS-analisi-mappe-satellitari-window.service';
import { enum_TipologiaLayer } from '../GIS-enum/GIS-tipologia-layer';
import { SDAC_DEFAULT as SDAC_DEFAULT_LOCAL } from './satellite-overlay.service';
import { SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { SatelliteAnimationService } from '../services/satellite-animation.service';
import { LayerService } from '../services/layer.service';

export { SDAC_DEFAULT_LOCAL as SDAC_DEFAULT };

@Component({
  standalone: false,
  selector: 'gis-analisi-mappe-satellitari-window',
  templateUrl: './GIS-analisi-mappe-satellitari-window.component.html',
  styleUrls: ['./GIS-analisi-mappe-satellitari-window.component.css']
})
export class GISAnalisiMappeSatellitariWindowComponent {
  selectedTabSubject = new Subject<SelectEvent>();
  isSmall = window.innerWidth < SMARTPHONE_WIDTH;

  isAllMaskLayersVisible$ = this.gisAnalisiMappeSatellitariWindowService.masks$
    .pipe(
      switchMap(masks => combineLatest(masks.map(mask => this.layerService.getIsLayerVisible$(mask.LayerElementiGrafici_cod)
        .pipe(map(visible => ({ mask, visible })))))
      ),
      map(layers => ({ visible: layers.every(l => l.visible), masks: layers.filter(l => !l.visible).map(l => l.mask.maschera_des) })),
    );

  windowArgs$: Observable<WindowArgs> = this.kendoWindowsService.windowToggle$
    .pipe(
      filter(([windowTypes, _]) => windowTypes === WindowTypes.AnalisiMappeSatellitariWindow),
      map(([_, args]) => args),
    );

  mapCleaner$ = this.windowArgs$
    .pipe(
      map(args => args.openState),
      startWith(false),
      pairwise(),
      tap(([wasOpen, isOpen]) => {
        if (wasOpen && !isOpen) {
          this.satelliteAnimationService.loadAllFrames([]);
        }
      }),
    );

  selectedTab$: Observable<number> = this.selectedTabSubject.asObservable()
    .pipe(
      map(x => x.index),
      startWith(0),
      tap(tab => {
        if (tab == 0) {
          this.gisAnalisiMappeSatellitariWindowService.nextModality(SatelliteDataVisualizationModality.Daily);
        }

        if (tab == 1) {
          this.gisAnalisiMappeSatellitariWindowService.nextModality(SatelliteDataVisualizationModality.Animation);
        }
      })
    );

  mapInfo$: Observable<boolean> = this.gisAnalisiMappeSatellitariWindowService
    .mapInfo$
    .pipe(
      withLatestFrom(this.gisAnalisiMappeSatellitariWindowService.overlays$),
      map(([value, overlays]) => {
        const gmap = this.googleMapService.googleMapWrapper?.data?.getMap();
        const cursor = value && (overlays.length == 0 || !overlays[0].applyToPolygon) ? 'crosshair' : '';
        gmap?.setOptions({ draggableCursor: cursor, });
        return value;
      })
    );

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    private googleMapService: GoogleMapService,
    private gisClient: GisClient,
    private satelliteAnimationService: SatelliteAnimationService,
    private layerService: LayerService
  ) {
    this.gisClient
      .gisLeggiMaschereLayerRaster({ TipologiaLayer_Raster_Cod: +enum_TipologiaLayer.Entita, LayerElementiGrafici_Raster_Cod: +enum_LayerElementiGraficiStd.ANALISI_MAPPE_SATELLITARI })
      .pipe(tap(res => this.gisAnalisiMappeSatellitariWindowService.nextMasks(res.RispostaStringa.elencoMaschere)))
      .subscribe();
  }
}

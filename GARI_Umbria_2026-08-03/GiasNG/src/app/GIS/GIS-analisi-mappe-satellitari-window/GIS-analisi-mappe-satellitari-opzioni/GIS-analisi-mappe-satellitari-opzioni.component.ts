import { Component } from '@angular/core';
import { Observable, catchError, of, switchMap, tap } from 'rxjs';
import { GISAnalisiMappeSatellitariWindowService } from '../GIS-analisi-mappe-satellitari-window.service';
import { AttivazioneMascheraLayerRaster, GisClient, MascheraLayerRaster } from 'app/Service/api.service';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { enum_TipologiaLayer } from 'app/GIS/GIS-enum/GIS-tipologia-layer';

@Component({
  standalone: false,
  selector: 'gis-analisi-mappe-satellitari-opzioni',
  templateUrl: './GIS-analisi-mappe-satellitari-opzioni.component.html',
  styleUrls: ['./GIS-analisi-mappe-satellitari-opzioni.component.css']
})
export class GISAnalisiMappeSatellitariOpzioniComponent {
  opacity$: Observable<number>;
  mapInfo$: Observable<boolean>;
  masks$: Observable<MascheraLayerRaster[]>;

  constructor(
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    private giasDialogService: GiasDialogService,
    private gisClient: GisClient
  ) {
    this.opacity$ = this.gisAnalisiMappeSatellitariWindowService.opacity$;
    this.mapInfo$ = this.gisAnalisiMappeSatellitariWindowService.mapInfo$;
    this.masks$ = this.gisAnalisiMappeSatellitariWindowService.masks$;
  }

  changeOpacity(opacity: number): void {
    this.gisAnalisiMappeSatellitariWindowService.nextOpacity(opacity);
  }

  changeMapInfo(mapInfo: boolean): void {
    this.gisAnalisiMappeSatellitariWindowService.nextMapInfo(mapInfo);
  }

  saveMask(mask: MascheraLayerRaster, active: boolean): void {
    const payload = { isAttivo: active, Maschera_Cod: mask.maschera_cod } as AttivazioneMascheraLayerRaster;
    this.gisClient
      .gisAttivaDisattivaMaschera(payload)
      .pipe(
        catchError(_ => {
          this.giasDialogService.baseError("", "gis.ErroreSalvataggioMascheraRaster", true);
          return of(null);
        }),
        switchMap(res => {
          if (res == null) {
            return of(null);
          }

          this.giasDialogService.baseSuccess("", "gis.MascheraRasterSalvataCorrettamente", true);
          return this.gisClient
            .gisLeggiMaschereLayerRaster({ TipologiaLayer_Raster_Cod: +enum_TipologiaLayer.Entita, LayerElementiGrafici_Raster_Cod: +enum_LayerElementiGraficiStd.ANALISI_MAPPE_SATELLITARI })
            .pipe(tap(result => this.gisAnalisiMappeSatellitariWindowService.nextMasks(result.RispostaStringa.elencoMaschere)));
        }),
        tap(masks => {
          if (masks == null || !this.gisAnalisiMappeSatellitariWindowService.isAnimationActive) {
            return;
          }

          this.gisAnalisiMappeSatellitariWindowService.nextIsAnimationActive(false);
          this.gisAnalisiMappeSatellitariWindowService.nextIsAnimationActive(true);
          this.gisAnalisiMappeSatellitariWindowService.nextExternalLoadOnPolygons(masks.RispostaStringa.elencoMaschere.length > 0);
        })
      )
      .subscribe();
  }
}

import { Component, OnDestroy } from '@angular/core';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { catchError, filter, forkJoin, map, Observable, of, share, switchMap, tap } from 'rxjs';
import { GISRasterConfigurationWindowService } from './GIS-raster-configuration-window.service';
import { GisClient, MascheraLayerRaster, RasterInfoClick_In, RasterInfoClick_Out, RispostaStandard_1OfList_1OfRasterInfoClick_Out, TipologiaLayer } from 'app/Service/api.service';
import { AttivazioneMascheraLayerRaster } from 'app/Service/api.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { GoogleMapService } from '../google-map/google-map.service';
import { TranslocoService } from '@jsverse/transloco';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasMessageService } from 'app/Service/gias-message.service';

@Component({
  standalone: false,
  selector: 'gis-raster-configuration-window',
  templateUrl: './GIS-raster-configuration-window.component.html',
  styleUrls: ['./GIS-raster-configuration-window.component.css']
})
export class GISRasterConfigurationWindowComponent implements OnDestroy {
  windowArgs$: Observable<WindowArgs>;
  raster$: Observable<TipologiaLayer>;
  opacity$: Observable<number>;
  clickInfo$: Observable<boolean>;
  clickData$: Observable<[RasterInfoClick_Out[], google.maps.LatLng]>;

  masks$: Observable<MascheraLayerRaster[]>;
  permission = this.permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_Gestione_Parametri_Maschere_Raster, 0);

  private infoWindows: google.maps.InfoWindow[] = [];
  private infoWindowContentStart =
    `<div class="maps-polygon-label-infowindow raster-info">` +
    `  <h6>${this.translocoService.translate('gis.InformazioniRaster')}</h6>` +
    `  <table>` +
    `    <tr>` +
    `      <th>${this.translocoService.translate('gis.Bande')}</th>` +
    `      <th>${this.translocoService.translate('gis.Descrizione')}</th>` +
    `      <th>${this.translocoService.translate('gis.ValoreStringa')}</th>` +
    `      <th>${this.translocoService.translate('gis.Valore')}</th>` +
    `    </tr>`;
  private infoWindowContentEnd =
    `  </table>` +
    `</div>`;

  constructor(
    private kendoWindowsService: KendoWindowsService,
    private gisRasterConfigurationWindowService: GISRasterConfigurationWindowService,
    private gisClient: GisClient,
    private giasDialogService: GiasDialogService,
    private giasMessageService: GiasMessageService,
    private permessiUtenteService: PermessiUtenteService,
    private googleMapService: GoogleMapService,
    private translocoService: TranslocoService
  ) {
    this.windowArgs$ = this.kendoWindowsService.windowToggle$.pipe(
      filter(([windowTypes, _]) => windowTypes === WindowTypes.RasterConfigurationWindow),
      map(([_, args]) => args),
    );

    this.raster$ = this.windowArgs$.pipe(
      map(args => args.additionalArgs?.raster as TipologiaLayer),
      filter(raster => raster != null),
      share(),
    );

    this.opacity$ = this.raster$.pipe(switchMap(raster => this.gisRasterConfigurationWindowService.getOpacity$(+raster?.id)));
    this.clickInfo$ = this.raster$.pipe(switchMap(raster => this.gisRasterConfigurationWindowService.getClickInfo$(+raster?.id)));
    this.masks$ = this.kendoWindowsService.windowToggle$.pipe(
      filter(([windowTypes, args]) => windowTypes === WindowTypes.RasterConfigurationWindow && args.openState),
      switchMap(([_, args]) => this.gisRasterConfigurationWindowService.getMasks$(+args.additionalArgs.raster.id)),
    );

    this.clickData$ = this.gisRasterConfigurationWindowService.currentVisible$
      .pipe(
        tap(() => this.closeAllInfoWindows()),
        filter(rasterId => rasterId != null),
        switchMap(rasterId => this.gisRasterConfigurationWindowService.getMapClickEvent$(rasterId)),
        switchMap(event => this.getMapInfo(event.latLng, this.gisRasterConfigurationWindowService.getCurrentVisible())),
        tap(([data, position]: [RasterInfoClick_Out[], google.maps.LatLng]) => this.showRasterInfo(data, position))
      );
  }

  ngOnDestroy(): void {
    this.gisRasterConfigurationWindowService.stopAllLoders();
  }

  changeOpacity(rasterId: number, opacity: number): void {
    this.gisRasterConfigurationWindowService.nextOpacity(rasterId, opacity);
  }

  changeMapClickInfo(rasterId: number, cickInfo: boolean): void {
    this.gisRasterConfigurationWindowService.nextMapClickInfo(rasterId, cickInfo);
  }

  saveRasterMasks(mask: MascheraLayerRaster, active: boolean): void {
    if (!this.permission) {
      this.giasDialogService.baseError('', 'gis.NonHaiIPermessiPerModificareQuestaMaschera', true);
      this.gisRasterConfigurationWindowService
        .fetchMasks(+mask.LayerElementiGrafici_Raster_cod)
        .subscribe();
      return;
    }

    const payload = { isAttivo: active, Maschera_Cod: mask.maschera_cod } as AttivazioneMascheraLayerRaster;
    this.gisClient
      .gisAttivaDisattivaMaschera(payload)
      .pipe(
        catchError(err => {
          this.giasDialogService.baseError("", "gis.ErroreSalvataggioMascheraRaster", true);
          return of(null);
        }),
        switchMap(res => {
          if (res == null) {
            return of(null)
          }

          this.giasDialogService.baseSuccess("", "gis.MascheraRasterSalvataCorrettamente", true);
          return this.gisRasterConfigurationWindowService.fetchMasks(+mask.LayerElementiGrafici_Raster_cod)
        }),
        tap(masks => {
          if (masks == null || !this.gisRasterConfigurationWindowService.isSomeVisible()) {
            return;
          }

          const running = this.gisRasterConfigurationWindowService.getCurrentVisible();
          this.gisRasterConfigurationWindowService.stopAllLoders();

          // should never be null
          if (running != null) {
            this.gisRasterConfigurationWindowService.nextVisibility(running, true);
          }
        })
      )
      .subscribe();
  }

  private getMapInfo(position: google.maps.LatLng, rasterId: number): Observable<[RasterInfoClick_Out[], google.maps.LatLng]> {
    const payload = {
      layerElementiGrafici_Cod: rasterId,
      PoligonoWKT: `POINT(${position.lng()} ${position.lat()})`
    } as RasterInfoClick_In;

    const request = this.gisClient.gisRasterInfoClick(payload)
      .pipe(
        catchError(err => {
          this.giasDialogService.baseError("", FunzioniComuniService.getResponseError(err, this.translocoService, "gis.ErroreCaricamentoDatiRaster"), false);
          return of({ RispostaStringa: null } as RispostaStandard_1OfList_1OfRasterInfoClick_Out);
        }),
        map(x => x.RispostaStringa)
      );

    return forkJoin([request, of(position)]);
  }

  private showRasterInfo(data: RasterInfoClick_Out[] | null, position: google.maps.LatLng): void {
    if (data == null) {
      return;
    }

    if (data.length == 0) {
      this.giasMessageService.infoMessagge('gis.NessunaInformazioneRaster', false, true);
      return;
    }

    let contentString = '';
    for (const element of data) {
      contentString +=
        `    <tr>` +
        `      <td>${element.BandN}</td>` +
        `      <td>${element.Descrizione}</td>` +
        `      <td>${(element as any).ValoreStringa}</td>` +
        `      <td>${element.Valore}</td>` +
        `    </tr>`;
    }

    const content = this.infoWindowContentStart + contentString + this.infoWindowContentEnd;
    const opts: google.maps.InfoWindowOptions = {
      position: position,
      content: content
    };

    const infoWindow = new google.maps.InfoWindow(opts);
    this.infoWindows.push(infoWindow);
    infoWindow.open({ anchor: null, map: this.googleMapService.googleMapWrapper.data.getMap(), shouldFocus: false });
  }

  private closeAllInfoWindows(): void {
    for (const infoWindow of this.infoWindows) {
      infoWindow.close();
    }

    this.infoWindows = [];
    this.kendoWindowsService.close(WindowTypes.RasterConfigurationWindow);
  }
}

import { Injectable } from '@angular/core';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { GisClient, MappaPrescrizioneGroundOverlay_Out } from 'app/Service/api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { TranslocoService } from '@jsverse/transloco';
import { NotificationRef } from '@progress/kendo-angular-notification';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';

const PRESCRIZIONE_LAYER_ID = +enum_LayerElementiGraficiStd.Mappe_Prescrizione;

@Injectable()
export class MappePrescrizioneRasterService {
  private visibilitySubject = new BehaviorSubject<boolean>(false);
  private opacitySubject = new BehaviorSubject<number>(1);

  private groundOverlay: google.maps.GroundOverlay | null = null;
  private opacitySubscription: Subscription | null = null;
  private notificationRef: NotificationRef | null = null;

  get visibility$(): Observable<boolean> {
    return this.visibilitySubject.asObservable();
  }

  get opacity$(): Observable<number> {
    return this.opacitySubject.asObservable();
  }

  get isVisible(): boolean {
    return this.visibilitySubject.value;
  }

  constructor(
    private gisClient: GisClient,
    private googleMapService: GoogleMapService,
    private giasMessageService: GiasMessageService,
    private translocoService: TranslocoService,
    private layerService: LayerService,
    private sharedDataService: SharedDataService
  ) { }

  public loadGroundOverlay(allegatoId: number): void {
    this.gisClient.gisMappaPrescrizioneGroundOverlay(allegatoId).subscribe({
      next: (res) => this.applyGroundOverlay(res),
      error: () => this.giasMessageService.errorMessage('gis.DatiAgricolturaPrecisoneErrore', false, true)
    });
  }

  public setOpacity(opacity: number): void {
    this.opacitySubject.next(opacity);
    this.groundOverlay?.setOpacity(opacity);
  }

  private applyGroundOverlay(res: MappaPrescrizioneGroundOverlay_Out): void {
    this.clearOverlay();

    const gMap = this.googleMapService.googleMapWrapper.data.getMap();
    const b = res.Bounds;
    const latLngBounds = new google.maps.LatLngBounds(
      new google.maps.LatLng(b.South, b.West),
      new google.maps.LatLng(b.North, b.East)
    );

    const imageUrl = `data:image/png;base64,${res.PngBase64}`;
    this.groundOverlay = new google.maps.GroundOverlay(imageUrl, latLngBounds, {
      opacity: this.opacitySubject.value
    });
    this.groundOverlay.setMap(gMap);
    this.visibilitySubject.next(true);

    const title = this.translocoService.translate('gis.VisualizzazioneDeiDatiRasterAttiva');
    this.notificationRef = this.giasMessageService.customMessage(title, '', '', () => {
      const layer = this.sharedDataService.getTipologiaLayerById(PRESCRIZIONE_LAYER_ID.toString());
      this.layerService.toggleLayerItemVisible(layer, false);
      this.clearOverlay();
    }, false);

    this.opacitySubscription = this.opacitySubject.subscribe(opacity => {
      this.groundOverlay?.setOpacity(opacity);
    });
  }

  public clearOverlay(): void {
    if (this.opacitySubscription) {
      this.opacitySubscription.unsubscribe();
      this.opacitySubscription = null;
    }

    if (this.notificationRef) {
      this.notificationRef.hide();
      this.notificationRef = null;
    }

    if (this.groundOverlay) {
      this.groundOverlay.setMap(null);
      this.groundOverlay = null;
    }

    this.visibilitySubject.next(false);
  }
}

import { Injectable } from '@angular/core';
import { FeatureService } from 'app/GIS/services/feature.service';
import { RicetteService } from 'app/menu-agenda/components/grid-ricette/ricette.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable()
export class GISGestionePianoRateoService {
  private isOpen = new BehaviorSubject<boolean>(false);

  constructor(
    private funzioniComuniService: FunzioniComuniService,
    private featureService: FeatureService,
    private giasDialogService: GiasDialogService,
    private ricetteService: RicetteService
  ) { }

  public get isOpen$(): Observable<boolean> {
    return this.isOpen.asObservable();
  }

  public open(): void {
    if (!this.isImpiantiSelected()) {
      this.giasDialogService.baseError("gis.GestionePianoRateo", "gis.GestionePianoRateoSelezionaFeature");
      return;
    }

    if (!this.isRicetteSelected()) {
      this.giasDialogService.baseError("gis.GestionePianoRateo", "gis.GestionePianoRateoSelezionaRicetta");
      return;
    }

    this.isOpen.next(true);
  }

  public close(): void {
    this.isOpen.next(false);
  }

  private isImpiantiSelected(): boolean {
    const features = this.featureService.getFeatureSelezionate();
    if (features.length == 0) {
      return false;
    }

    const chiaveAlbero = this.featureService.getChiaveAlberoCompletaByFeature(features[0]);
    const objChiaveAlbero = FunzioniComuniService.scomponiChiaveAlbero(chiaveAlbero);
    return this.funzioniComuniService.isTipoNodoImpianto(objChiaveAlbero.TipoNodo);
  }

  private isRicetteSelected(): boolean {
    const ricette = this.ricetteService.getSelected();
    return ricette.length > 0;
  }
}

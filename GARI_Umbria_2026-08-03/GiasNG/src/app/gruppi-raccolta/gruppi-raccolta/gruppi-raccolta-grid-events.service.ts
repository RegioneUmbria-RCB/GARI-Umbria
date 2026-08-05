import { ElementRef, Injectable } from '@angular/core';
import { from, Observable, of, switchMap } from 'rxjs';
import { enum_TipoOperazioneDB } from '../../Model/TipiEnumerativi';
import { GruppiRaccoltaService } from '../../Service/GruppiRaccolta/gruppi-raccolta.service';
import { GruppoRaccolta } from '../../Model/metaschema/GruppoRaccolta';
import { GiasMessageService } from '../../Service/gias-message.service';
import { TranslocoService } from '@jsverse/transloco';
import { LoadingService } from 'gias-ui-kit';

@Injectable()
export class GruppiRaccoltaGridEventsService {

  loadingService: LoadingService;
  component: ElementRef<any>;

  constructor(
    private gruppiRaccoltaService: GruppiRaccoltaService,
    private giasMessageService: GiasMessageService,
    private translocoService: TranslocoService
  ) {
  }

  deleteGruppoRaccolta(item: any): Observable<any> {
    this.loadingService.set_isLoading({ isLoading: true, component: this.component });
    return from(this.gruppiRaccoltaService.scriviModificaCancella_GruppoRaccolta(item, enum_TipoOperazioneDB.Cancellazione)).pipe(
      switchMap((resp) => {
        this.loadingService.set_isLoading({ isLoading: false, component: this.component });
        if (resp.RispostaOK) {
          this.giasMessageService.successMessage(this.translocoService.translate('GruppoRaccoltaSalvatoCorrettamente'));
        }
        return of([]);
      })
    );
  }

  updateGruppoRaccolta(item: GruppoRaccolta): Observable<any> {
    this.loadingService.set_isLoading({ isLoading: true, component: this.component });
    return from(this.gruppiRaccoltaService.scriviModificaCancella_GruppoRaccolta(item, enum_TipoOperazioneDB.Modifica)).pipe(
      switchMap((resp) => {
        this.loadingService.set_isLoading({ isLoading: false, component: this.component });
        if (resp.RispostaOK) {
          this.giasMessageService.successMessage(this.translocoService.translate('GruppoRaccoltaSalvatoCorrettamente'));
        }
        return of([]);
      })
    );
  }

  writeGruppoRaccolta(item: GruppoRaccolta): Observable<any> {
    this.loadingService.set_isLoading({ isLoading: true, component: this.component });
    return from(this.gruppiRaccoltaService.scriviModificaCancella_GruppoRaccolta(item, enum_TipoOperazioneDB.Scrittura)).pipe(
      switchMap((resp) => {
        this.loadingService.set_isLoading({ isLoading: false, component: this.component });
        if (resp.RispostaOK) {
          this.giasMessageService.successMessage(this.translocoService.translate('GruppoRaccoltaSalvatoCorrettamente'));
        }
        return of([]);
      })
    );
  }

}

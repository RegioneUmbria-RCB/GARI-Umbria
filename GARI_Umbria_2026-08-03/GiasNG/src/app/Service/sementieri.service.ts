import { Injectable } from '@angular/core';
import { combineLatest, map, Observable, share } from 'rxjs';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from './configurazione-siti.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';

@Injectable({ providedIn: 'root' })
export class SementieriService {

  constructor(
    private sharedDataService: SharedDataService,
    private configurazioneSitiService: ConfigurazioneSitiService) {
  }

  public isSementieriServer(): Observable<boolean> {
    return this.configurazioneSitiService.leggiChiave(EnumChiaviConfigurazioneSiti.Is_Sementieri)
      .pipe(
        map(chiave => {
          const isSementieriServer = chiave?.Valore?.toLowerCase() === 'true';
          return isSementieriServer;
        }),
        share()
      );
  }

  public isSementieriMappaturaLibera(): Observable<boolean> {
    return combineLatest([
      this.sharedDataService.getCfgSementiAsObs(),
      this.isSementieriServer()
    ]).pipe(
      map(([cfgSementi, isSementieriServer]) => {
        const isSportelloActive = cfgSementi != null && cfgSementi.Sementi != null && cfgSementi.SementiMappaturaLibera == 'True';
        return isSementieriServer && isSportelloActive;
      })
    );
  }

  public isSementieriSportello(): Observable<boolean> {
    return combineLatest([
      this.sharedDataService.getCfgSementiAsObs(),
      this.isSementieriServer()
    ]).pipe(
      map(([cfgSementi, isSementieriServer]) => {
        const isSportelloActive = cfgSementi != null && cfgSementi.Sementi != null && cfgSementi.SementiMappaturaLibera != 'True';
        return isSementieriServer && isSportelloActive;
      })
    );
  }
}

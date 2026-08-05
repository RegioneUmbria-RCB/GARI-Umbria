import { Injectable } from '@angular/core';
import { Observable, of, ReplaySubject, switchMap, take } from 'rxjs';
import { UtilityNGClient } from './api.service';

export class Configurazione_Siti{
  PivaSuperUser: string;
  Sito_Cod: number;
  Chiave: string;
  Valore: string;
}

export enum EnumChiaviConfigurazioneSiti {
  LinkAgronicaGiasNG = 'LinkAgronicaGiasNG',
  Gis_Clustering = 'Gis_Clustering',
  Is_Sementieri = 'Is_Sementieri',
  SatEngineIsActive = 'isActiveEngine_SAT',
  MappePrescrizioneEngineIsActive = 'isActiveEngine_MappePrescrizione',
  Utenti_Visibilita_Calcolo_Combinato_Pratiche = 'Utenti_Visibilita_Calcolo_Combinato_Pratiche'
}

@Injectable({ providedIn: 'root' })
export class ConfigurazioneSitiService {
  private configurazione_Siti: Configurazione_Siti[] =null;

  private Configurazione_SitiModelSource = new ReplaySubject<Configurazione_Siti[]>();
  currentConfigurazione_Siti: Observable<Configurazione_Siti[]> = this.Configurazione_SitiModelSource.asObservable();

  constructor(
    private utilityNgClient: UtilityNGClient) {
  }

  leggi(): Observable<Configurazione_Siti[]> {
    if (this.configurazione_Siti == null) {
      return this.utilityNgClient.utilityNGLeggiConfigurazioneSiti().pipe(
        take(1),
        switchMap((val) => {
          this.configurazione_Siti = JSON.parse(val.RispostaStringa);
          this.Configurazione_SitiModelSource.next(this.configurazione_Siti);
          return of(this.configurazione_Siti);
        })
      );
    } else {
      return of(this.configurazione_Siti)
    }
  }

  leggiChiave(chiave: string): Observable<Configurazione_Siti> {
    if (this.configurazione_Siti == null || !this.configurazione_Siti.length) {
      return this.utilityNgClient.utilityNGLeggiConfigurazioneSiti().pipe(
        take(1),
        switchMap((val) => {
          this.configurazione_Siti = JSON.parse(val.RispostaStringa);
          this.Configurazione_SitiModelSource.next(this.configurazione_Siti);
          let cfSingolo = this.configurazione_Siti.find((val) => { return val.Chiave == chiave })
          return of(cfSingolo);
        })
      );
    } else {
      let cfSingolo = this.configurazione_Siti.find((val) => { return val.Chiave == chiave })
      return of(cfSingolo)
    }
  }

}

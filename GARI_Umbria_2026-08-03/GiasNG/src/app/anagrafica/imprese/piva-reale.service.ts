import { Injectable } from '@angular/core';
import { FiltroRicercaClient } from "app/Service/net-core6-api.service";
import { map } from 'rxjs/operators';
import { of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PivaRealeService {

  constructor(
    private filtroRicercaClient: FiltroRicercaClient
  ) { }

  getPivaReale(piva: string) {
    // trascodifica Piva => PivaReale
    if (piva != null && piva !== '')
      return this.filtroRicercaClient.filtroRicercaGetPivaReale(piva).pipe(
        map(r => r.RispostaStringa));
    else
      return of(null);
  }
}
import { Injectable } from '@angular/core';
import {map, Observable, of, tap} from 'rxjs';
import { AnagraficaClient, UnitaDiMisura_Alternativa } from '../api.service';
import {EserciziRequestManager} from "../ServiceFactory/impianti.factory.service";

const UDM_COD_ETTARI = 2123;
const DEFAULT_ITEM = { codice: 0, descrizione: '', simbolo: null, tassoConversione: 0 } as UnitaDiMisura_Alternativa;

@Injectable({ providedIn: 'root' })
export class ConversioniService {
  protected unitaMisuraAlternative_Ettari:  UnitaDiMisura_Alternativa[] = null;
  constructor(private anagraficaClient: AnagraficaClient) { }

  leggi_async(includeDefault: boolean = false): Promise<UnitaDiMisura_Alternativa[]> {
    return new Promise<UnitaDiMisura_Alternativa[]>(async (resolve, _) => {
      this.leggi(includeDefault)
        .pipe(tap(items => resolve(items)))
        .subscribe();
    });
  }

  leggi(includeDefault: boolean = false): Observable<UnitaDiMisura_Alternativa[]> {
    if (this.unitaMisuraAlternative_Ettari == null){
      return this.anagraficaClient.anagraficaLeggiConversioniUdmAlt(new Date('1900-01-01'), new Date('2100-12-31'), UDM_COD_ETTARI)
        .pipe(
          map(res => includeDefault ? [DEFAULT_ITEM, ...res.RispostaStringa] : res.RispostaStringa),
          tap((res) => { this.unitaMisuraAlternative_Ettari=res })
        )
    } else {
      return of(this.unitaMisuraAlternative_Ettari);
    }
  }
}

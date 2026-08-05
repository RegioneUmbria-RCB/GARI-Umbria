import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { AjaxAgronicaNetCore6ApiService } from 'app/Service/ajax-agronica-net-core6-api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import {
  CatalogoPraticheApiItem,
  CatalogoPraticheRisposta,
  ConfigurazionePraticheUtente,
  PraticaSelezionabile,
} from '../models/pratiche-visibilita.model';

@Injectable({
  providedIn: 'root',
})
export class PraticheVisibilitaService {

  constructor(
    private ajaxApi: AjaxAgronicaNetCore6ApiService,
    private giasMessageService: GiasMessageService,
  ) { }

  public caricaCatalogoPratiche(): Observable<PraticaSelezionabile[]> {
    return this.ajaxApi
      .ajaxAPIGet('v1/catalogo/pratiche', "")
      .pipe(
        map((r: any) =>
          r.RispostaStringa["catalogo"].map((item: CatalogoPraticheApiItem) => ({
            Servizio_Cod: item.servizio_cod,
            ServizioDescrizione: item.servizio_descrizione,
            DataValiditaInizio: item.dataValiditaInizio ? new Date(item.dataValiditaInizio) : null,
            DataValiditaFine: item.dataValiditaFine ? new Date(item.dataValiditaFine) : null,
            IsValida: item.isValida,
            ConsideraValiditaTemporale: false,
            Selected: false,
          } as PraticaSelezionabile))
        )
      );
  }

  public caricaConfigurazioneUtente(username: string): Observable<ConfigurazionePraticheUtente> {
    const defaultConfig: ConfigurazionePraticheUtente = {
      Username: username,
      OperatoreFiltri: 'OR',
      FiltroPraticheAttivo: false,
      Pratiche: []
    };
    return this.ajaxApi
      .ajaxAPIGet<{ username: string }, any>('v1/profilo-utente/ConfigurazionePraticheUtente', { username })
      .pipe(map((R) => R.RispostaOK ? R.RispostaStringa as ConfigurazionePraticheUtente : defaultConfig));
  }

  public eseguiSalvataggio(
    username: string,
    pratiche: PraticaSelezionabile[],
    operatoreOR: boolean,
    filtroPraticheAttivo: boolean,
  ): Observable<boolean> {
    return this.salvaConfigurazione({
      Username: username,
      OperatoreFiltri: operatoreOR ? 'OR' : 'AND',
      FiltroPraticheAttivo: filtroPraticheAttivo,
      Pratiche: pratiche,
    });
  }

  public salvaConfigurazione(request: ConfigurazionePraticheUtente): Observable<boolean> {
    return this.ajaxApi
      .ajaxAPIPost<ConfigurazionePraticheUtente, any>('v1/profilo-utente/ConfigurazionePraticheUtente', request)
      .pipe(
        map((R) => {
          if (!R.RispostaOK) {
            this.giasMessageService.errorMessage(R.Errore ?? 'ImpossibileCompletareOperazione', true, !R.Errore);
          }
          return R.RispostaOK as boolean;
        }),
      );
  }
}

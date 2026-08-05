import { Injectable, TemplateRef } from '@angular/core';
import { FiltriAnalisiConformita, FiltriAnalisiConformitaDes } from 'app/qualita-tracciabilita/models/filtri-nuova-analisi-conformita.model';
import { AnalysisRequestDataItem } from 'app/qualita-tracciabilita/models/richiesta-analisi.model';
import { BehaviorSubject, Observable, of, take, map, filter, tap, ReplaySubject } from 'rxjs';
import { QdCaComplianceClient } from "app/Service/qdca-compliance-api.service";
import { GiasMessageService } from 'app/Service/gias-message.service';
import { ConfigurazioneSitiService } from "app/Service/configurazione-siti.service";
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { MasterService } from 'app/Service/master.service';
import { TranslocoService } from '@jsverse/transloco';

/** Temporary.
 *  TODO: Delete after deployment
 */
export enum ResultDataSource {
  FromTable = 0,
  FromEngine = 1
}

@Injectable({
  providedIn: 'root'
})
export class VerificaDisciplinariService {

  public risultatoAnalisi$: ReplaySubject<any> = new ReplaySubject<any>();
  public listaRichieste$: BehaviorSubject<AnalysisRequestDataItem[]> = new BehaviorSubject([]);
  /** TemplateRef della griglia di visualizzazione dettaglio analisi.
   *  Usata durante l'apertura della finestra di visualizzazione dei dettagli.
   */
  public analysisDetailRef: TemplateRef<any>;
  /** Riferiemnto ai filtri analisi da usare per il caricamento dei dettagli analisi. */
  public analysisDataItem?: AnalysisRequestDataItem;

  public readonly canReadQdc: boolean;
  public readonly canWriteQdc: boolean;
  private _controlloRiduzioneDiserbo: boolean = false;
  private _lastFilters: FiltriAnalisiConformita;

  constructor(
    private master: MasterService,
    private APIService: QdCaComplianceClient,
    private transloco: TranslocoService,
    private message: GiasMessageService,
    private config: ConfigurazioneSitiService,
    private permessiUtenteService: PermessiUtenteService,
  ) {
    this.canReadQdc = this.permessiUtenteService.canReadPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG);
    this.canWriteQdc = this.permessiUtenteService.canWritePermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG);

    this.config.leggiChiave('Flag_Nuovo_Controllo_Riduzione_Diserbo')
      .pipe(
        map(conf => conf?.Valore || ""),
        map(value => value.toLowerCase().trim())
      ).subscribe((value) => this._controlloRiduzioneDiserbo = value === 'true' || value === '1');
  }

  /**
   * Reads the analysis requests from the backend and updates the local stream.
   *
   * Side effects:
   * - Emits the parsed list into `this.listaRichieste$` via `next(...)`.
   *
   * @param filters Input filters used to query analysis requests. They are converted to
   *        the backend DTO via `FiltriAnalisiConformita.toLeggiAgendaHubQDCNew(filters)`.
   *
   * @returns `Observable<any[]>` — a cold stream that maps the raw backend response to a JSON array
   */
  public readActiveAnalysisRequests(filters: FiltriAnalisiConformita): Observable<any[]> {
    this._lastFilters = filters;
    return this.APIService.richiesteVerificaGetRichiesteAnalisi(FiltriAnalisiConformita.toLeggiAgendaHubQDCNew(filters))
      .pipe(
        map(x => x.RispostaOK ? x.RispostaStringa : '[]'),
        map(x => JSON.parse(x)),
        tap(x => this.listaRichieste$.next(x))
      );
  }

  /**
 * Reads the analysis requests from the backend and updates the local stream.
 * It reuses the filters from the previous call.
 *
 * Side effects:
 * - Emits the parsed list into `this.listaRichieste$` via `next(...)`.
 *
 * @returns `Observable<any[]>` — a cold stream that maps the raw backend response to a JSON array
 */
  public readAnalysisRequestsAsLastFiltered(): Observable<any[]> {
    if (this._lastFilters == null) return of([]);
    return this.readActiveAnalysisRequests(this._lastFilters);
  }

  /**
   * Sends a **new analysis request** to the backend and merges the created items
   * into the head of `listaRichieste$`.
   *
   * Side effects:
   * - Writes to `this.listaRichieste$` (BehaviorSubject).
   * - Produces console debug logs.
   * - Performs an internal subscription (method returns `void`).
   *
   * Assumptions:
   * - The backend returns JSON that parses into `AnalysisRequestDataItem[]`.
   * - `filters.veg_cod` and the comma-separated `filters.veg_des` lists are aligned
   *   by index so that `VegCod` → `VegDes` mapping is valid.
   *
   * @param filters Input model for the new analysis request. Converted with
   *        `FiltriAnalisiConformita.toLeggiAgendaHubQDCNew(filters)` before the call.
   *
   * @returns `void` — work is performed via side effects and the internal subscription.
   */
  public sendNewAnalysisRequest(filters: FiltriAnalisiConformitaDes, loading: boolean = true): void {
    console.debug('analisi verifica disciplinare ', filters);
    let currentValue = this.listaRichieste$.value ?? [];
    let params = FiltriAnalisiConformita.toLeggiAgendaHubQDCNew(filters);
    params.controlloRiduzioneDiserbo = this._controlloRiduzioneDiserbo;
    this.master.set_isLoading({ isLoading: loading });
    this.APIService.richiesteVerificaInviaRichiestaAnalisi(params)
      .pipe(
        map(x => x.RispostaOK ? x.RispostaStringa : '[]'),
        map(x => JSON.parse(x)),
        tap(x => console.debug('response', x)),
        tap((res: AnalysisRequestDataItem[]) => res.forEach(x => {
          x.RagSoc = filters.rag_soc;
          x.SaNome = filters.sa_nome;
          let desc = filters.veg_des.split(', ').map(x => x.trim());
          let idx = filters.veg_cod.findIndex(y => y == x.VegCod);
          x.VegDes = desc[idx];
          x.OperazioniDes = filters.operazioni_des;
        }))
      ).subscribe((res) => {
        currentValue = [...res].concat(currentValue);
        this.master.set_isLoading({ isLoading: false });
        this.listaRichieste$.next(currentValue);
        this.message.successMessage(this.transloco.translate("RichiestaConformitaInviataAspettaRisultati"));
      });
  }

  /**
   * Reads the analysis results for the selected analysis header.
   *
   * @returns An observable emitting an array of analysis operations result objects.
   */
  public readAnalysisResults(): Observable<any> {
    this.master.set_isLoading({ isLoading: true });
    const src = this.analysisDataItem['source'];
    if (src == ResultDataSource.FromTable) { // from table
      return this.APIService.richiesteVerificaGetRisultatoAnalisi(this.analysisDataItem.IdTestata)
        .pipe(
          map(x => x.RispostaOK ? x.RispostaStringa : '{"Trattamenti":[], "Fertilizzazioni":[], "Raccolte":[]}'),
          map(x => JSON.parse(x)),
          tap(x => this.risultatoAnalisi$.next(x)),
          map(x => [...x.Trattamenti, ...x.Fertilizzazioni, ...x.Raccolte, ...x.VerificheMagazzino]),
          tap((res) => { if (res.length == 0) this.message.warningMessage('Nessun risultato trovato.') }),
          tap(() => this.master.set_isLoading({ isLoading: false }))
        );
    } else { // from engine
      // this.analysisDataItem.ControlloRiduzioneDiserbo = this._controlloRiduzioneDiserbo;
      // return this.APIService.agendaVerificaConformitaOperazioniExtended(this.analysisDataItem)
      //   .pipe(
      //     map(x => x.RispostaOK ? x.RispostaStringa : '{"Trattamenti":[], "Fertilizzazioni":[], "Raccolte":[]}'),
      //     map(x => JSON.parse(x)),
      //     tap(x => this.risultatoAnalisi$.next(x)),
      //     map(x => [...x.Trattamenti, ...x.Fertilizzazioni, ...x.Raccolte, ...x.VerificheMagazzino]),
      //     tap((res) => { if (res.length == 0) this.message.warningMessage('Nessun risultato trovato.') }),
      //     tap(() => this.master.set_isLoading({ isLoading: false }))
      //   );
      this.message.warningMessage('Nessun risultato trovato.');
      this.master.set_isLoading({ isLoading: false });
      return of([]);
    }
  }

}

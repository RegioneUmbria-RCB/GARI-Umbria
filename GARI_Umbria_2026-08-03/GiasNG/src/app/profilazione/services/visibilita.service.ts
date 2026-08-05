import { Injectable } from '@angular/core';
import { AjaxAgronicaAPIService } from "../../Service/ajax-agronica.api.service";
import { ImpresaDto } from "../../Service/api.service";
import { BehaviorSubject, map, Observable, of, pipe, Subject, switchMap, take, tap } from "rxjs";
import { GiasDialogService } from "../../Service/gias-dialog.service";
import { MasterService, rispostaStandard } from "../../Service/master.service";
import { TranslocoService } from "@jsverse/transloco";
import { ConfrontaVisibilitaUtenti } from '../models/ConfrontaVisibilitaUtenti';
import { IUtenteDTO, UtenteDTO } from "../models/utente-dto.model";
import { isBoolean } from "lodash";
import { CopiaImpostazioniObj } from './impostazioni/impostazioni-utenti.service';


export class LeggiScriviVisibilitaUtenti {
  constructor(
    public Utenti = Array<IUtenteDTO>(),
    public AziendeVisibili = Array<ImpresaDto>(),
    public Sovrascrivi?: boolean,
    public LeggiDatiImprese: boolean = false,
  ) { }
}

export interface IUtenteImpresa extends ImpresaDto {
  Username: string;
  DettagliUtente: string;
}

export class CopiaVisibilitaObj extends CopiaImpostazioniObj {
  constructor(
    public base: string[],
    public template: string,
    public CopyHierarchy: boolean = false,
    public CopyProcedures: boolean = false
  ) {
    super(base, template);
  }
}


@Injectable({
  providedIn: 'root'
})
export class VisibilitaService {
  public utentiSelezionati: IUtenteDTO[] = [];
  public impreseSelezionate: ImpresaDto[] = [];
  public $imprese: BehaviorSubject<any[]> = new BehaviorSubject<any[]>(null);

  public gridRefresh$ = new Subject();
  public saved$ = new Subject();

  constructor(
    private APIService: AjaxAgronicaAPIService,
    private master: MasterService,
    private transloco: TranslocoService,
    private dialog: GiasDialogService
  ) { }

  public sovrascriviVisibilita(utenti?: IUtenteDTO[], imprese?: ImpresaDto[]): Observable<boolean> {
    utenti = (!utenti && this.utentiSelezionati?.length) ? this.utentiSelezionati : utenti;
    imprese = (!imprese && this.impreseSelezionate?.length) ? this.impreseSelezionate : imprese;
    return this.checkIfAnyHasSons(imprese).pipe(
      switchMap((_next: boolean) => _next
        ? this.modificaVisibilitaUtenti(utenti, imprese, true).pipe(map(saved => this.resetDataIfSaved(saved)))
        : of(false))
    );
  }

  public aggiungiVisibilita(utenti?: IUtenteDTO[], imprese?: ImpresaDto[]): Observable<boolean> {
    utenti = (!utenti && this.utentiSelezionati?.length) ? this.utentiSelezionati : utenti;
    imprese = (!imprese && this.impreseSelezionate?.length) ? this.impreseSelezionate : imprese;
    return this.checkIfAnyHasSons(imprese).pipe(
      tap(_next => {
        if (!_next) {
          this.impreseSelezionate = this.impreseSelezionate.filter(i => !!i?.rag_soc);
        }
      }),
      switchMap((_next: boolean) => _next
        ? this.modificaVisibilitaUtenti(utenti, imprese, false).pipe(map(saved => this.resetDataIfSaved(saved)))
        : of(false))
    );
  }

  public assegnaVisibilitaTotale(utenti?: IUtenteDTO[]) {
    if (!utenti && this.utentiSelezionati?.length) {
      utenti = this.utentiSelezionati;
    }
    return this.modificaVisibilitaUtenti(utenti, [], true).pipe(map(saved => {
      if (saved) {
        this.resetData();
        // this.gridRefresh$.next(true);
        this.saved$.next(true);
      }
      return saved;
    }));
  }

  public rimuoviVisibilita(utenti?: IUtenteDTO[]) {
    if (!utenti && this.utentiSelezionati?.length) {
      utenti = this.utentiSelezionati;
    }
    return this.APIService.ajaxAPIPost<LeggiScriviVisibilitaUtenti, any>(
      'Profilazione/RimuoviVisibilitaUtenti', new LeggiScriviVisibilitaUtenti(utenti, [], true)
    ).pipe(map(R => {
      if (R.RispostaOK) {
        this.dialog.baseSuccess('prof.VisibilitaAssociataOK', '');
      } else if (R.RispostaStringa) {
        this.dialog.baseWarning(this.transloco.translate("Attenzione"), R.RispostaStringa, false);
      } else {
        this.dialog.baseError('ImpossibileCompletareOperazione', '');
      }
      return R.RispostaOK;
    }));
    // return this.modificaVisibilitaUtenti(utenti, [new ImpresaDTO('###########')], true);
  }

  public rimuoviImpreseDaVisibilita(utenti: IUtenteDTO[], imprese: any[]) {
    if (!utenti && this.utentiSelezionati?.length) {
      utenti = this.utentiSelezionati;
    }
    return this.APIService.ajaxAPIPost<LeggiScriviVisibilitaUtenti, any>(
      'Profilazione/RimuoviVisibilitaUtenti',
      new LeggiScriviVisibilitaUtenti(utenti, imprese, true)
    ).pipe(map(R => {
      if (R.RispostaOK) {
        this.dialog.baseSuccess('prof.VisibilitaAssociataOK', '');
      } else if (R.RispostaStringa) {
        this.dialog.baseWarning(this.transloco.translate("Attenzione"), R.RispostaStringa, false);
      } else {
        this.dialog.baseError('ImpossibileCompletareOperazione', R.Errore);
      }
      return R.RispostaOK;
    }));
  }

  /**
   * Assegna agli utenti in base la visibilità dell'utente template.
   * @param base lista di username degli utenti a cui si vuole assegnare la visibilità
   * @param template username dell'utente template da cui copiare la visibilità
   */
  public copiaVisibilita(base: string[], template: string) {
    return this.APIService.ajaxAPIPost<any, any>(
      'Profilazione/CopiaVisibilitaUtenti', new CopiaVisibilitaObj(base, template, true, true)
    ).pipe(map(R => {
      if (R.RispostaOK) {
        void this.dialog.baseSuccess('prof.VisibilitaAssociataOK', '');
        this.gridRefresh$.next(true);
        this.saved$.next(true);
      } else {
        this.dialog.baseError('ImpossibileCompletareOperazione', '');
      }
      return R.RispostaOK;
    }));
  }

  /**
   * Carica la visibilità per le imprese specificate.
   * @param imprese di cui si deve leggere la visibilità. Se non viene passata alcuna impresa, viene caricata la visibilità di tutte le imprese.
   */
  public leggiVisibilitaImprese(imprese: ImpresaDto[] = []): Observable<IUtenteImpresa[]> {
    return this.APIService.ajaxAPIPost<LeggiScriviVisibilitaUtenti, any>(
      'Profilazione/LeggiAziendeVisibilita', new LeggiScriviVisibilitaUtenti([], imprese)
    ).pipe(map(R => R.RispostaOK ? R.RispostaStringa : []));
  }

  /**
   * Legge la visibilità dell'utente specificato.
   * @param username l'username dell'utente specificato. Se non specificato, si fa riferimento all'utente attuale.
   * @param leggiDatiImprese indica se caricare anche i dati relativi alle imprese in visibilità o meno
   */
  public leggiVisibilitaUtente(username: string, leggiDatiImprese: boolean, setLoading: boolean = true): Observable<IUtenteImpresa[]> {
    const params = new LeggiScriviVisibilitaUtenti();
    params.Utenti.push(new UtenteDTO(username || this.master.objP_utenti.UsernameOperazione));
    params.LeggiDatiImprese = leggiDatiImprese;
    this.master.set_isLoading({ isLoading: setLoading });
    return this.APIService.ajaxAPIPost<LeggiScriviVisibilitaUtenti, rispostaStandard<{ visibilita: IUtenteImpresa[]; imprese: any[] }>>(
      'Profilazione/leggiVisibilitaUtenti', params
    ).pipe(
      take(1),
      tap(() => this.master.set_isLoading({ isLoading: false })),
      map(R => R.RispostaOK
        ? R.RispostaStringa as unknown as { visibilita: IUtenteImpresa[]; imprese: any[] }
        : { visibilita: [], imprese: [] }
      ),
      tap(R => {
        if (R.imprese) this.$imprese.next(R.imprese);
      }),
      map(R => R.visibilita)
    );
  }

  /**
   * @param utenti usernames degli utenti di cui si vuole leggere la visibilità. Se non specificato fa riferimento a tutti gli utenti
   * @param imprese restrizione sulle aziende da visualizzare. Di default considera la visibilità dell'utente loggato
   * @param leggiDettagli Se true, restituisce anche i dettagli (nome, cognome) degli utenti
   */
  public leggiVisibilitaUtenti(utenti: string[] = [], imprese: ImpresaDto[] = [], leggiDettagli: boolean = false): Observable<IUtenteImpresa[]> {
    const params = new LeggiScriviVisibilitaUtenti([], imprese, leggiDettagli);
    if (utenti.length)
      utenti.forEach(u => params.Utenti.push(new UtenteDTO(u)));

    return this.APIService.ajaxAPIPost<LeggiScriviVisibilitaUtenti, rispostaStandard<{ visibilita: IUtenteImpresa[]; imprese: any[] }>>(
      'Profilazione/leggiVisibilitaUtenti', params
    ).pipe(map(R => R.RispostaOK ? R.RispostaStringa['visibilita'] : []));
  }

  /**
   * @return oggetto contente i seguenti campi:
   * - visibilita: `IUtenteImpresa[]` - explosione utenti x imprese in visibilità
   * - visibilitaTotale: `string[]` - usernames degli utenti con visibilità totale
   * - imprese: `any[]` - dettagli delle imprese presenti in visibilità
   * - fullResult: `boolean` - false se il numero di utenti con visibilità è maggiore di 10000 (troppi)
   */
  public leggiVisibilitaGruppi(gruppi: string[] = [], imprese: ImpresaDto[] = [], leggiDettagli: boolean = false): Observable<any> {
    const params = new LeggiScriviVisibilitaUtenti([], imprese, leggiDettagli);
    params.Utenti = gruppi.map(g => new UtenteDTO(g));
    return this.APIService.ajaxAPIPost<LeggiScriviVisibilitaUtenti, rispostaStandard<{ visibilita: IUtenteImpresa[]; imprese: any[] }>>(
      'Profilazione/leggiVisibilitaGruppi', params
    ).pipe(map(R => R.RispostaOK && R.RispostaStringa ? R.RispostaStringa : []));
  }

  public haVisibilitaTotale(username: string): Observable<boolean> {
    return this.APIService.ajaxAPIPost<string[], any>(
      'Profilazione/haVisibilitaTotale', [username]
    ).pipe(map(R =>
      R.RispostaOK ? R.RispostaStringa[0]['VisibilitaTotale'] : false
    ));
  }

  /**
   * Controlla quali degli utenti specificati hanno visibilità totale sulle imprese.
   * @param usernames una lista contenente gli username degli utenti desiderati.
   */
  public hannoVisibilitaTotale(usernames: string[]): Observable<any[]> {
    return this.APIService.ajaxAPIPost<string[], any>(
      'Profilazione/haVisibilitaTotale', usernames
    ).pipe(map(R => {
      if (R.RispostaOK) {
        return R.RispostaStringa;
      }
      return [];
    }));
  }

  /**
   * Stabilisce se gli utenti specificati hanno la stessa visibilità.
   * @param usernames lista di username degli utenti su cui eseguire la verifica.
   * @returns observable contente true se tutti gli utenti hanno la stessa visibilità, false altrimenti.
   */
  public hannoStessaVisibilita(usernames: string[]): Observable<boolean> {
    return this.APIService.ajaxAPIPost<string[], boolean>(
      'Profilazione/ControllaStessaVisibilita', usernames
    ).pipe(map(R => {
      if (R.RispostaOK) {
        return R.RispostaStringa as boolean;
      }
      return false;
    }));
  }

  /**
   * Confronta la visibilità di due utenti.
   * @param utente username dell'utente oggetto di confronto
   * @param base username dell'utente che funge da base di confronto
   * @return observable contente -1 se l'utente oggetto di confronto ha visibilità minore di quello che funge da base, 1 se la sua visibilità è maggiore o 0 se è uguale.
   */
  public confrontaVisibilita(utente: string, base: string) {
    return this.APIService.ajaxAPIPost<any, number>(
      'Profilazione/ConfrontaVisibilitaUtenti',
      new ConfrontaVisibilitaUtenti(utente, base)
    ).pipe(map(R => {
      if (R.RispostaOK) {
        return R.RispostaStringa;
      }
      return -1;
    }));
  }

  private resetData() {
    this.utentiSelezionati = [];
    this.impreseSelezionate = [];
  }

  private resetDataIfSaved(saved: boolean): boolean {
    if (saved) {
      this.resetData();
      this.saved$.next(true);
    }
    return saved;
  }

  /**
   * This code defines a private function modificaVisibilitaUtenti that modifies
   * the visibility of users and businesses. It sends a POST request to a specific
   * API endpoint with the modified data. Depending on the response, it shows
   * success, warning, or error messages using a dialog service.
   * @param utenti must adhere to the interface IUtenteDTO (only needs UserName valorized)
   * @param imprese which business will the users be able to see
   * @private
   */
  private modificaVisibilitaUtenti(utenti: IUtenteDTO[], imprese: ImpresaDto[], sovrascrivi: boolean) {
    const users = utenti.map(u => new UtenteDTO(u.UserName));
    return this.APIService.ajaxAPIPost<LeggiScriviVisibilitaUtenti, any>(
      'Profilazione/ModificaVisibilitaUtenti', new LeggiScriviVisibilitaUtenti(users, imprese, sovrascrivi)
    ).pipe(map(R => {
      if (R.RispostaOK) {
        void this.dialog.baseSuccess('prof.VisibilitaAssociataOK', '');
      } else if (R.RispostaStringa) {
        void this.dialog.baseWarning(this.transloco.translate("Attenzione"), R.RispostaStringa, false);
      } else {
        this.dialog.baseError('ImpossibileCompletareOperazione', '');
      }
      return R.RispostaOK;
    }));
  }

  private checkIfAnyHasSons(imprese: ImpresaDto[]): Observable<boolean> {
    return this.APIService.ajaxAPIPost<ImpresaDto[], string[]>(
      'Profilazione/CheckFathersInList', imprese
    ).pipe(
      map((r: rispostaStandard<string[]>) => r.RispostaOK ? r.RispostaStringa : []),
      switchMap(pive => pive.length
        ? this.dialog.warningObs('prof.BusinessInHierarchy', 'prof.BusinessInHierarchyWarningText')
        : of(true)
      ),
      map(dialogResult => isBoolean(dialogResult) ? dialogResult : false),
      take(1)
    );
  }
}

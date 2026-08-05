import {Injectable} from '@angular/core';
import {BehaviorSubject, map, Observable, Subject, take} from "rxjs";
import {GruppoUtente} from "../models/gruppi-utenti/GruppoUtente.model";
import {AjaxAgronicaAPIService} from "../../Service/ajax-agronica.api.service";
import {BaseCodeDescr} from "../../Model/baseClass/baseCodeDescr";
import {BaseCodeDescrStr} from "../../Model/baseClass/baseCodeDescrStr";
import {ScriviGruppoUtente, ScriviGruppoUtentexTransizioniStato} from "../models/gruppi-utenti/GruppiInData.model";
import {enum_TipoOperazioneDB} from "../../Model/TipiEnumerativi";
import { TransizioneDiStato } from '../models/gruppi-utenti/TransizioneDiStato.model';
import { DropdownListWithForm, ModelEntry } from 'gias-kendo-grid';
import { FormGroup } from '@angular/forms';

@Injectable()
export class GruppiUtentiService {
  public gruppo$ = new BehaviorSubject<GruppoUtente>(null);
  public transizioniGruppo$ = new BehaviorSubject<any[]>(null);
  public transizioni$ = new BehaviorSubject<any[]>(null);
  public servizi$ = new BehaviorSubject<any[]>(null);
  public gridRefresh$ = new Subject<boolean>();

  constructor(
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
  ) { }

  /**
   * Carica la lista di gruppi utenti esistenti.
   */
  public readGruppi(): Observable<GruppoUtente[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIGet<boolean, GruppoUtente[]>(
      "Provisioning/ListaGruppiUtente", true
    ).pipe(map(res => {
      if (res.RispostaOK && res.RispostaStringa['ListaDatiGruppoUtente']['length'])
        return res.RispostaStringa['ListaDatiGruppoUtente'].map(g => new GruppoUtente(+g.Codice, g.Descrizione, g.Identificativo));
      return [];
    }));
  }

  public caricaServiziTransazioniStatoXGruppi():Observable<BaseCodeDescr[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIGet<string, any>(
      "MetaschemaNG/CaricaServiziXTrasizioniStatoGruppiUtenti", ""
    ).pipe(map(res => res.RispostaOK ? res.RispostaStringa : []));
  }

  public caricaTransizioniStatoXServizio(transizione_cod: number):Observable<BaseCodeDescrStr[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<number, BaseCodeDescrStr[]>(
      "MetaschemaNG/CaricaTrasizioniStatoGruppiUtenti", transizione_cod
    ).pipe(map(res => res.RispostaOK ? res.RispostaStringa : []));
  }

  /**
   * Carica le transizioni di stato usate dal gruppo utente specificato.
   * Le transizioni sono caricate nell'Observable `transizioniGruppo$`.
   * @param gruppo gruppo utente per cui si vogliono caricare le transizioni usate
   */
  public caricaTransizioniXGruppo(gruppo: GruppoUtente): Observable<BaseCodeDescrStr[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<GruppoUtente, BaseCodeDescrStr[]>(
      'MetaschemaNG/CaricaTrasizioniStatoXGruppoUtente', gruppo
    ).pipe(take(1), map(R => R.RispostaOK ? R.RispostaStringa : []));
  }

  /**
   * Aggiunge le transizioni specificate alla lista di transizioni usate dal gruppo specificato.
   * @param gruppo codice del gruppo di riferimento
   * @param transizioni lista di transizioni da aggiungere
   */
  public aggiungiTransizioneUsata(gruppo: GruppoUtente, transizioni: BaseCodeDescrStr[]) {
    const params = new ScriviGruppoUtentexTransizioniStato(
      gruppo, transizioni, enum_TipoOperazioneDB.Modifica
    );
    this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviGruppoUtentexTransizioniStato, any>(
      'MetaschemaNG/AggiungiTrasizioniStatoXGruppoUtente',
      params, true
    ).pipe(take(1)).subscribe((R) => {
        if (R.RispostaOK) {
          this.gridRefresh$.next(true);
        }
    });
  }

  /**
   * Rimuove le transizioni di stato specificate da quelle assegnate al gruppo utente specificato.
   * @param gruppo codice del gruppo di riferimento
   * @param transizioni lista di transizioni da rimuovere
   */
  public rimuoviTransizioni(gruppo: GruppoUtente, transizioni: BaseCodeDescrStr[]) {
    const params = new ScriviGruppoUtentexTransizioniStato(
      gruppo, transizioni, enum_TipoOperazioneDB.Modifica
    );
    this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviGruppoUtentexTransizioniStato, any>(
      'MetaschemaNG/RimuoviTrasizioniStatoXGruppoUtente',
      params, true
    ).pipe(take(1)).subscribe();
  }

  /**
   * Rimuove il gruppo utente specificato.
   * @param gruppo codice del gruppo di riferimento
   */
  public rimuoviGruppo(gruppo: GruppoUtente) {
    const params = new ScriviGruppoUtente(
      gruppo, enum_TipoOperazioneDB.Cancellazione
    );
    this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviGruppoUtente, any>(
      'MetaschemaNG/ScriviGruppoUtente',
      params, true
    ).pipe(take(1)).subscribe();
  }

  /**
 * Modifica il gruppo utente specificato.
 * @param gruppo codice del gruppo di riferimento
 */
  public modificaGruppo(gruppo: GruppoUtente) {
    const params = new ScriviGruppoUtente(
      gruppo, enum_TipoOperazioneDB.Modifica
    );
    this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviGruppoUtente, any>(
      'MetaschemaNG/ScriviGruppoUtente',
      params, true
    ).pipe(take(1)).subscribe();
  }

  /**
 * Crea un nuovo gruppo utente.
 */
  public nuovoGruppo(gruppo: GruppoUtente) {
    const params = new ScriviGruppoUtente(
      gruppo, enum_TipoOperazioneDB.Scrittura
    );
    this.ajaxAgronicaAPIService.ajaxAPIPost<ScriviGruppoUtente, any>(
      'MetaschemaNG/ScriviGruppoUtente',
      params, true
    ).pipe(take(1)).subscribe((R) => {
        if (R.RispostaOK) {
          this.gridRefresh$.next(true);
        }
    });
  }

}

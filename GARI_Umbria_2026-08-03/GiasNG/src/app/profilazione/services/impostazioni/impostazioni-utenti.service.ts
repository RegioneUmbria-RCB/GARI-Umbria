import { Injectable } from '@angular/core';
import { MasterService } from '../../../Service/master.service';
import { map, Observable, take, tap } from 'rxjs';
import { AjaxAgronicaAPIService } from '../../../Service/ajax-agronica.api.service';
import { Utente } from '../../../Model/utente/utente';
import { Utente_Impostazioni } from '../../../Model/utente/utente_impostazioni';
import { GiasDialogService } from '../../../Service/gias-dialog.service';
import { GuidaValoreImpostazione } from 'gias-ui-kit';
import { BaseCodeDescrVal } from "../../../Model/baseClass/baseCodeDescrVal";

export class Leggi_Impostazioni {
  constructor(
    public Utenti: string[] = [],
    public Impostazioni: number[] = [],
    public flagLetturaSuperUser = 0
  ) { }
}
export class CopiaImpostazioniObj {
  /**
   * @param base stringa identificativa dell'utente o impresa-centro per cui si vogliono salvare le impostazioni
   * @param template stringa identificativa dell'utente o impresa-centro da cui copiare i valori delle impostazioni
   */
  constructor(
    public base: string[],
    public template: string
  ) { }
}

@Injectable({
  providedIn: 'root'
})
export class ImpostazioniUtentiService {

  constructor(
    private dialogService: GiasDialogService,
    private APIService: AjaxAgronicaAPIService,
    private masterService: MasterService
  ) { }

  /**
   * Legge le impostazioni e le relative sezioni in relazione all'utente corrente
   * @param flagLetturaSuperUser 0: solo utente, 1: solo superuser, 2: entrambi
   * @param listaUtenti Lista contente gli username degli utenti per cui si vogliono leggere le impostazioni
   */
  public leggiSezioniImpostazioniUtente(flagLetturaSuperUser = 2, listaUtenti = []): Observable<Array<any>> {
    if (listaUtenti.length === 0) {
      listaUtenti = [this.masterService.objP_utenti.UsernameOperazione];
    }
    if (!this.masterService.isSuperuser()) {
      flagLetturaSuperUser = 0;
    }
    return this.APIService.ajaxAPIPost<any, any>(
      'Profilazione/LeggiGuideImpostazioniUtenti', new Leggi_Impostazioni(
        listaUtenti, [0], flagLetturaSuperUser
      ), true
    ).pipe(map((R) => R.RispostaStringa));
  }

  /**
   * Carica i valori dei controlli impostazione.
   * @param codiciImpostazioni
   * @param listaUtenti Lista contente gli username degli utenti per cui si vogliono leggere le impostazioni
   */
  leggiControlliImpostazioniUtente(codiciImpostazioni: number[], listaUtenti?: string[]) {
    if (!listaUtenti || listaUtenti.length === 0) {
      listaUtenti = [this.masterService.objP_utenti.UtenteUsername];
    }
    return this.APIService.ajaxAPIPost<Leggi_Impostazioni, any>(
      'Profilazione/LeggiControlliImpostazioniUtenti', {
      Utenti: listaUtenti,
      Impostazioni: codiciImpostazioni,
      flagLetturaSuperUser: 0
    }
    ).pipe(map((R) => {
      /* Il campo data degli elementi appartiene alla classe
      AgronicaCoreModelsSTD.profilazione.ImpostazioneBase */
      for (let r of R.RispostaStringa) {
        r.data = r.data.map(d => {
          let guida = new GuidaValoreImpostazione(d.codice, d.descrizione, d.Valore, d.TipoCampo, d.Note);
          guida.Username = d.Username;
          return guida;
        });
      }
      return R.RispostaStringa;
    }));
  }

  /**
   *
   * @param impostazioni
   * @param utentiSelezionati
   */
  salvaImpostazioniUtente(impostazioni: Utente_Impostazioni[], utentiSelezionati?: string[]) {
    if (!utentiSelezionati || utentiSelezionati.length === 0) {
      utentiSelezionati = [this.masterService.objP_utenti.UtenteUsername];
    }
    if (impostazioni.length === 0) {
      console.warn("No settings to save");
    }
    const listaUtenti = utentiSelezionati.map(username => new Utente(username));
    listaUtenti.forEach(u => u.Impostazioni = impostazioni);
    this.masterService.set_isLoading({ isLoading: true, zIndex: 20000 });
    this.APIService.ajaxAPIPost<Utente[], any>(
      'Profilazione/SalvaImpostazioniUtenti', listaUtenti, false
    ).GiasSubscribe(R => {
      if (R.RispostaOK)
        this.dialogService.baseSuccess('SalvataggioAvvenutoConSuccesso', '');
      else
        this.dialogService.baseError('', 'ErroreSalvataggio');
      this.masterService.set_isLoading({ isLoading: false });
    });
  }

  salvaImpostazioniUtenteObs(impostazioni: Utente_Impostazioni[], utentiSelezionati?: string[]): Observable<boolean> {
    if (!utentiSelezionati || utentiSelezionati.length === 0) {
      utentiSelezionati = [this.masterService.objP_utenti.UtenteUsername];
    }
    const listaUtenti = utentiSelezionati.map(username => new Utente(username));
    listaUtenti.forEach(u => u.Impostazioni = impostazioni);
    this.masterService.set_isLoading({ isLoading: true, zIndex: 20000 });
    return this.APIService.ajaxAPIPost<Utente[], any>(
      'Profilazione/SalvaImpostazioniUtenti', listaUtenti, false
    ).pipe(
      tap(() => this.masterService.set_isLoading({ isLoading: false })),
      map(R => R.RispostaOK)
    );
  }

  /**
     * Copia le impostazione utente a partire da un altro utente
     * @param base username dell'utente per cui si vogliono salvare le impostazioni
     * @param template username dell'utente da cui copiare i valori delle impostazioni
     */
  copiaImpostazioni(base: string[], template: string): Observable<boolean> {
    return this.APIService.ajaxAPIPost<CopiaImpostazioniObj, any>(
      'Profilazione/CopiaImpostazioniUtenti', new CopiaImpostazioniObj(base, template), true
    ).pipe(map(R => {
      console.log('impostazioni salvate?', R);
      if (R.RispostaOK)
        this.dialogService.baseSuccess('SalvataggioAvvenutoConSuccesso', '');
      else
        this.dialogService.baseError('', 'ErroreSalvataggio');
      return R.RispostaOK;
    }));
  }

  /**
   * Reimposta le impostazioni specificate ai valori di default.
   * @param codes Codici delle impostazioni su cui eseguire il reset
   * @param selcted Usernames utente per cui si vuole eseguire l'operazione. Se non specificata, l'operazione è eseguita per l'utente corrente.
   */
  public resetImpostazioniToDefault(codes: number[], selected?: string[]) {
    if (!selected || !selected.length) {
      selected = [this.masterService.objP_utenti.UtenteUsername];
    }
    const utenti = selected.map(u => {
      let user = new Utente(u);
      user.Impostazioni = codes.map(c => new Utente_Impostazioni(c));
      return user;
    });
    return this.APIService.ajaxAPIPost<Utente[], any>(
      "Profilazione/ResetImpostazioniUtentiDefault", utenti, true
    ).pipe(take(1), map(R => {
      if (R.RispostaOK)
        this.dialogService.baseSuccess('SalvataggioAvvenutoConSuccesso', '');
      else
        this.dialogService.baseError('', 'ErroreSalvataggio');
      return R.RispostaOK;
    }));
  }

  /**
   * Ritorna i dati ausiliari letti nella tabella Utenti_Impostazioni_FiltroMono.
   *
   * @usageNotes Che valori assume `any`?
   * SuperUser_FiltroSQL_MateriePrime --> {@link BaseCodeDescrVal}
   * UTENTE_COD_FILTRO_VARIETA --> {@link Varieta}
   *
   * @param username
   * @param impostazioneCod
   */
  leggiDatiFiltroMono(username: string, impostazioneCod: number): Observable<any[]> {
    return this.APIService.ajaxAPIPost<any, any[]>("/MetaschemaNG/CaricaDatiDDL", {
      User: username,
      Impostazione_Cod: impostazioneCod
    }).pipe(take(1), map(R => R.RispostaStringa));
  }
}

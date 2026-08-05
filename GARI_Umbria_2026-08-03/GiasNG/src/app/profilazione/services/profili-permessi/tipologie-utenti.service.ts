import { Injectable } from "@angular/core";
import { TipologiaUtente } from "../../models/profili-permessi/tipologia-utente.model";
import { AjaxAgronicaAPIService } from "app/Service/ajax-agronica.api.service";
import { MasterService } from "app/Service/master.service";
import { Observable, map, BehaviorSubject, take, Subject, tap } from "rxjs";
import { GiasDialogService } from "../../../Service/gias-dialog.service";
import { Utente_Permesso } from "../../../Model/utente/utente_permesso";
import { enum_TipoPermesso } from "../../models/profilazione.model";
import { UtentePermessi } from "../../models/profili-permessi/UtentePermessi.model";
import { GridProfiliMenuActions } from "../../profili-permessi/griglia-profili/grid-profili.service";
import { Utente_Impostazioni } from '../../../Model/utente/utente_impostazioni';

export class AssociaProfiloObj {
  constructor(
    public Utenti: UtentePermessi[],
    public Profilo?: TipologiaUtente,
    public AssociaImpostazioni: boolean = false
  ) { }
}

export class CopyProfileObj {
  constructor(
    public Original: TipologiaUtente,
    public CopyTemplate: TipologiaUtente,
    public AlsoCopySettings: boolean = false
  ) { }
}

@Injectable()
export class TipologieUtentiService {
  public profileGridEvent$ = new Subject<{ event: GridProfiliMenuActions; item: TipologiaUtente }>();
  public selectedProfile$: BehaviorSubject<TipologiaUtente> = new BehaviorSubject(null);
  public allPermissions = [];

  private _isEditing: BehaviorSubject<boolean> = new BehaviorSubject(false);

  constructor(
    private masterService: MasterService,
    private dialog: GiasDialogService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) { }

  get isEditing(): BehaviorSubject<boolean> {
    return this._isEditing;
  }
  set isEditing(value: BehaviorSubject<boolean>) {
    this._isEditing = value;
  }

  public readTipologie(filterOutDefault: boolean = false): Observable<TipologiaUtente[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIGet<boolean, any>(
      "AgronicaCoreUtentiBIZ/ListaTipologie",
      true
    ).pipe(
      map((res) => res.RispostaStringa),
      map((res) => {
        if (filterOutDefault) {
          return res.filter((tipologia) => tipologia.codice !== 99999999);
        }
        return res;
      })
    );
  }

  public saveTipologie(tipologie: TipologiaUtente[]): Observable<boolean> {
    this.masterService.set_isLoading({ isLoading: true, message: '' });
    return this.ajaxAgronicaAPIService.ajaxAPIPost(
      "AgronicaCoreUtentiBIZ/ScriviTipologie",
      tipologie, true
    ).pipe(
      take(1),
      map(risposta => {
        this.masterService.set_isLoading({ isLoading: false, message: '' });
        if (risposta.RispostaOK) {
          this.dialog.salvataggioOk();
        }
        return risposta.RispostaOK;
      })
    );
  }

  public copyTipologie(copia: TipologiaUtente, originale: TipologiaUtente, copySettings): Observable<any> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      "AgronicaCoreUtentiBIZ/CopiaTipologia", new CopyProfileObj(originale, copia, copySettings)
    ).pipe(map((risposta) => {
      if (!risposta.RispostaOK)
        this.dialog.baseError("ErroreSalvataggio", "QualcosaEAndatoStorto");
    }));
  }

  public deleteTipologia(tipologia: TipologiaUtente): Observable<any> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost(
      "AgronicaCoreUtentiBIZ/CancellaTipologia", tipologia
    ).pipe(map((risposta) => {
      if (!risposta.RispostaOK)
        this.dialog.baseError("ErroreEliminazione", "QualcosaEAndatoStorto");
    }));
  }

  public setPermessiTipologia(
    profilo: TipologiaUtente, attivita: Utente_Permesso[], permesso: enum_TipoPermesso
  ): Observable<boolean> {
    attivita.forEach(a => a.Permesso_Tipo = permesso);
    profilo.Permessi = attivita;
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      'AgronicaCoreUtentiBIZ/AssegnaPermessi', profilo, true
    ).pipe(map((risposta) => {
      if (risposta.RispostaOK)
        this.dialog.salvataggioOk();
      else this.dialog.baseError('Errore_', '');
      return risposta.RispostaOK;
    }));
  }

  public associaTipologia(utenti: UtentePermessi[], profilo: TipologiaUtente, associaImpostazioni: boolean) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      'Profilazione/AssociaProfilo', new AssociaProfiloObj(utenti, profilo, associaImpostazioni), true
    ).pipe(map(R => {
      if (R.RispostaOK)
        this.dialog.salvataggioOk();
      return R.RispostaOK;
    }));
  }

  public modificaValidita(utenti: UtentePermessi[]) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      'Profilazione/ModificaValiditaPermessi', new AssociaProfiloObj(utenti), true
    ).pipe(map(R => {
      if (R.RispostaOK)
        this.dialog.salvataggioOk();
      return R.RispostaOK;
    }));
  }

  public updateConnectedUsers(tipologia: TipologiaUtente) {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<TipologiaUtente, any>(
      'Profilazione/AggiornaPermessiUtentiTipologia', tipologia, true
    ).pipe(take(1)).subscribe(R => {
      if (R.RispostaOK)
        this.dialog.salvataggioOk();
      return R.RispostaOK;
    });
  }

  public propagateSettings(tipologia: TipologiaUtente, settings: Utente_Impostazioni[] = []): Observable<boolean> {
    const params = { Profilo: tipologia, Impostazioni: settings };
    return this.ajaxAgronicaAPIService.ajaxAPIPost(
      'Profilazione/AggiornaImpostazioniUtentiTipologia', params, true
    ).pipe(
      take(1),
      map(R => {
        if (R.RispostaOK) this.dialog.salvataggioOk();
        return R.RispostaOK;
      })
    );
  }

  public hasPermissionsOrSettings(tipologia: TipologiaUtente): Observable<{ haImpostazioni: boolean; haPermessi: boolean }> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<TipologiaUtente, { haImpostazioni: boolean; haPermessi: boolean }>(
      'AgronicaCoreUtentiBIZ/TipologiaHaImpostazioniPermessiCollegati', tipologia
    ).pipe(take(1), map(R => R.RispostaStringa));
  }

  public readConnectedUsers(tipologia: TipologiaUtente): Observable<UtentePermessi[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<TipologiaUtente, UtentePermessi[]>(
      'AgronicaCoreUtentiBIZ/TipologiaLeggiUtentiCollegati', tipologia
    ).pipe(take(1), map(R => R.RispostaStringa));
  }

}


import {Injectable} from '@angular/core';
import {enum_Security_Attivita} from 'app/Model/TipiEnumerativi';
import {BehaviorSubject, map, Observable, ReplaySubject} from 'rxjs';
import {AjaxAgronicaAPIService} from './ajax-agronica.api.service';
import {Utente} from 'app/Model/utente/utente';
import {Utente_Impostazioni} from 'app/Model/utente/utente_impostazioni';
import {enum_TipoPermesso} from "../profilazione/models/profilazione.model";
import {MasterService} from "./master.service";
import { IUsernameService } from 'gias-ui-kit';

class UserToken { }

class RispostaCore {
  RispostaStringa: string;
}

export class MenuEntry {
  colore: string;
  testo: string;
  idHtml: string;
  idSezione: number;
  sitoRichiesto: number;
  paginaRichiesta: number;
  richiedeAziendaSelezionata: number;
  redirectUrl: string;
  preferito: boolean;
  Figli: MenuEntry[];
  classeCssIcona: string;
  presetIniziale: boolean;
}

@Injectable({ providedIn: 'root' })
export class PermessiUtenteService implements IUsernameService {
  private Utente_Permessi: Utente =null;
  public sidemenuBehaviorSubject = new BehaviorSubject([]);

  private Utente_PermessiModelSource = new ReplaySubject<Utente>();
  currentUtente_Permessi: Observable<Utente> = this.Utente_PermessiModelSource.asObservable();

  private Utente_PermessiLoaded = new BehaviorSubject<boolean>(false);
  currentUtente_PermessiLoaded: Observable<boolean> = this.Utente_PermessiLoaded.asObservable();

    ObsUtenteAbilitatoAccessoQdCColdiretti: Observable<boolean> = null;

    constructor(protected ajaxApiService: AjaxAgronicaAPIService,
                private masterservice: MasterService) {

    this.currentUtente_Permessi.subscribe((val) => {
      this.Utente_Permessi = val;
    })
  }

  async getSidebarMenu() {
    this.sidemenuBehaviorSubject.next([]);
    var callMenu = await this.ajaxApiService.ajaxAPIGet<string, object>('Menu/AlberoMenu', "", false, true).toPromise();
    this.sidemenuBehaviorSubject.next(callMenu.RispostaStringa['Menus']);
  }

  changeUtente_Permessi(upd_headerModel: Utente) {
    this.Utente_PermessiModelSource.next(upd_headerModel);
  }

  changeUtente_PermessiLoaded(val: boolean) {
    this.Utente_PermessiLoaded.next(val);
  }

  getUtente_PermessiLoaded() : boolean {
    return this.Utente_PermessiLoaded.getValue();
  }

  getCurrentUser(): Utente {
    //return this.Utente_PermessiModelSource.getValue();
    return this.Utente_Permessi;
  }

  /**
   *
   * @param Attivita codice rappresentante l'attività da controllare
   * @param Operazione tipo di operazione consentita. Possibile usare <code>enum_TipoPermsso</code>
   */
  getPermesso(Attivita: enum_Security_Attivita, Operazione: number | enum_TipoPermesso): boolean{
    let permesso = this.Utente_Permessi.Permessi.find((el) => el.Permesso_ID == Attivita);
    if (permesso == undefined) {
      return false;
    }
    if (Operazione == 0) {
      return true;
    }
    if (permesso.Permesso_Tipo == Operazione) {
      return true;
    }
    return false;
  }

  canReadPermesso(attivita: enum_Security_Attivita): boolean {
    return this.getPermesso(attivita, enum_TipoPermesso.LETTURA)
      || this.getPermesso(attivita, enum_TipoPermesso.LETTURA_SCRITTURA);
  }

  canWritePermesso(attivita: enum_Security_Attivita): boolean {
    return this.getPermesso(attivita, enum_TipoPermesso.LETTURA_SCRITTURA);
  }

  getImpostazione_Utente(Impostazione_Cod: number): Utente_Impostazioni{
    const impostazione = this.Utente_Permessi.Impostazioni.find((imp) => imp.Impostazione_Cod == Impostazione_Cod);
    return impostazione;
  }

  fetchPermessiUtente(): Observable<Utente> {
    return this.ajaxApiService.ajaxAPIGet<any, Utente>('UtilityNG/getUtente', "")
      .pipe(map(R => R.RispostaStringa));
    }

    public CheckUtenteTipologiaAccessoQDC(Piva: string,Data: Date){


      const Obs = this.ajaxApiService.ajaxAPIPost<Object,boolean>('UtilityNG/CheckUtenteTipologiaAccessoQDC',{ piva:  Piva,data: Data}).pipe(map(R=>{
        return R.RispostaStringa
      }));

      return Obs;
  }

  controllaEsistenzaTabellaCliente_Permessi(): Observable<boolean> {
    return this.ajaxApiService.ajaxAPIPost<any, boolean>('Profilazione/ControllaEsistenzaTabellaCliente_Permessi', "").pipe(map(R => R.RispostaStringa));
  }

}

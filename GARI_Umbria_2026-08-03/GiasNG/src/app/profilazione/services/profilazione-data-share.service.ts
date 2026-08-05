import {ChangeDetectorRef, Injectable} from '@angular/core';
import {BaseCodeDescr} from 'app/Model/baseClass/baseCodeDescr';
import {BehaviorSubject, Subject} from 'rxjs';
import {enum_PagineProfilazione} from "../models/PaginaProfilazione.model";
import {IUtenteImpresa} from "./visibilita.service";
import {FormArray, FormGroup} from "@angular/forms";
import { Impostazione } from 'gias-ui-kit';
import {GiasKendoGridComponent} from 'gias-kendo-grid';
import {IUtenteDTO} from "../models/utente-dto.model";
import {PermessiUtenteService} from "../../Service/permessi-utente.service";
import {enum_Security_Attivita} from "../../Model/TipiEnumerativi";
import {FiltriUtenteComponent} from "../gestione-utenti/utenti/filtri-utente/filtri-utente.component";

@Injectable()
export class ProfilazioneDataShareService {
  // GENERALE

  public userPermissions = {
    canReadAllUsers: false,
    canEditAllUsers: false,
    canEditSelf: false,
    canReadGroups: false,
    canEditGroups: false,
    canReadProfiles: false,
    canEditProfiles: false,
    canViewBusinessSettings: false,
    canEditBusinessSettings: false,
    canViewBaseUserSettings: false,
    canEditBaseUserSettings: false,
    canViewAdvancedUserSettings: false,
    canEditAdvancedUserSettings: false,
    canViewAgendaBlocksUserSettings: false,
    canEditAgendaBlocksUserSettings: false,
  };
  public initPermissions(service: PermessiUtenteService) {
    this.userPermissions.canEditSelf = service.canWritePermesso(enum_Security_Attivita.Profilazione_NG);
    this.userPermissions.canReadAllUsers = service.canReadPermesso(enum_Security_Attivita.AmministrazioneUtenti);
    this.userPermissions.canEditAllUsers = service.canWritePermesso(enum_Security_Attivita.AmministrazioneUtenti);
    this.userPermissions.canReadGroups = service.canReadPermesso(enum_Security_Attivita.Gest_UtentiGruppi);
    this.userPermissions.canEditGroups = service.canWritePermesso(enum_Security_Attivita.Gest_UtentiGruppi);
    this.userPermissions.canReadProfiles = service.canReadPermesso(enum_Security_Attivita.Gest_UtentiProfili);
    this.userPermissions.canEditProfiles = service.canWritePermesso(enum_Security_Attivita.Gest_UtentiProfili);
    this.userPermissions.canViewBusinessSettings = service.canReadPermesso(enum_Security_Attivita.ImpostazioniImprese_NG);
    this.userPermissions.canEditBusinessSettings = service.canWritePermesso(enum_Security_Attivita.ImpostazioniImprese_NG);
    this.userPermissions.canViewBaseUserSettings = service.canReadPermesso(enum_Security_Attivita.Gest_UtentiImpostazioni);
    this.userPermissions.canEditBaseUserSettings = service.canWritePermesso(enum_Security_Attivita.Gest_UtentiImpostazioni);
    this.userPermissions.canViewAdvancedUserSettings = service.canReadPermesso(enum_Security_Attivita.Gest_UtentiImpostazioni_Avanzate);
    this.userPermissions.canEditAdvancedUserSettings = service.canWritePermesso(enum_Security_Attivita.Gest_UtentiImpostazioni_Avanzate);
    this.userPermissions.canViewAgendaBlocksUserSettings = service.canReadPermesso(enum_Security_Attivita.Gest_UtentiAgendaBlocchi);
    this.userPermissions.canEditAgendaBlocksUserSettings = service.canReadPermesso(enum_Security_Attivita.Gest_UtentiAgendaBlocchi);
  }

  /** Mappa contenente i formgroup di alcune impostazioni gestite in modalità speciale. */
  public formsMap: Map<number, FormGroup> = new Map<number, FormGroup>([]);

  /** Emette un valore quando si esce dalla pafina della profilazione.
   *  Usato come riferimento per terminare l'iscrizione agli `Observable`. */
  public exitProfilazione: Subject<boolean>;
  public readonly currentPage = new BehaviorSubject<enum_PagineProfilazione>(enum_PagineProfilazione.UTENTI);
  //---------------------------------------------------------------------------
  // VISIBILITA'

  public visibilitaRichiesta = false;
  public utentiEditVisibilita: IUtenteDTO[] = [];
  public visibilitaImprese = new BehaviorSubject<IUtenteImpresa[]>([]);
  public visibilitaUtenti = new BehaviorSubject<IUtenteImpresa[]>(null);
  public nImpreseInEdit = 0;
  //---------------------------------------------------------------------------
  // UTENTI

  //public loadAllUtenti = false;
  public isAddingNew = false;
  public usersGrid: GiasKendoGridComponent;
  public filterService: FiltriUtenteComponent;
  public loadUsersFilters: FormGroup;
  public getUserFiltersJson(): string {
    if (this.loadUsersFilters)
      return JSON.stringify(this.loadUsersFilters.value.ctrls);
    return "[]";
  }
  public removeUserDateFilter() {
    const fArray = (this.loadUsersFilters.get('ctrls') as FormArray);
    const dateCtrl = fArray.controls.find(ctrl => ctrl.get('field').value === 'Data_Creazione');
    if (dateCtrl) {
      dateCtrl.get('value').patchValue('');
    }
  }

  //---------------------------------------------------------------------------
  // IMPOSTAZIONI

  public settingsLoaded = false;
  public cardInfo = new BehaviorSubject<Impostazione>(null);
  public specie: BehaviorSubject<BaseCodeDescr[]> = new BehaviorSubject([]);
  public aziendaCentriChangeDetector: ChangeDetectorRef;
  /**
   * Usato in {@link GeneralFieldComponent} per avviare l'aggiornamento di del componente sottostante.
   * @values oggetti di tipo `{setting: number, component: string}`, in cui il campo
   * `component` specifica il nome del componente da aggiornare. (es.: 'SettingsCategorieMagazzinoComponent'),
   * mentre `setting` specifica l'impostazione da considerare.
   */
  public reloadSetting$ = new Subject<{setting: number; component: string}>();
  public setChangeDetector(changeDetector: ChangeDetectorRef) {
    this.aziendaCentriChangeDetector = changeDetector;
  }

}

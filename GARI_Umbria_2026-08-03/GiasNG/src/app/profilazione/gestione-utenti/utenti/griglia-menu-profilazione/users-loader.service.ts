import {Injectable} from '@angular/core';
import {ProfilazioneDataShareService} from "../../../services/profilazione-data-share.service";
import {ProfilazioneUtentiService} from "../../../services/profilazione-utenti.service";
import {BehaviorSubject, filter, forkJoin, Observable, of, switchMap, take, tap} from "rxjs";
import {enum_Azienda_Persona, UtenteFlatModel} from "../../../models/utente.model";
import {TranslocoService} from "@jsverse/transloco";
import {ConfigurazioneSitiService} from "../../../../Service/configurazione-siti.service";
import {FormArray} from "@angular/forms";
import {enum_TipoFiltro} from "../filtri-utente/filtri-utente.component";

@Injectable()
export class UsersLoaderService {

  private _commonTranslations: any;
  private _mediumBuisinessSizeLimit = 200;
  private _userCount: number;
  private _initFinished$ = new BehaviorSubject<boolean>(false);
  private _self: UtenteFlatModel[] = [];
  private _lastUsedFilter = "";
  private _lastLoaded: UtenteFlatModel[] = [];

  constructor(
    private transloco: TranslocoService,
    private usersService: ProfilazioneUtentiService,
    private datashare: ProfilazioneDataShareService,
    private siteConfig: ConfigurazioneSitiService
  ) {
    this.initTranslations();
    this.initConstantParams();
  }

  private get _isSmallMediumBusiness(): boolean {
    return this._userCount <= this._mediumBuisinessSizeLimit;
  }

  private get _isFirstTimeLoading(): boolean {
    const firstTime = this.datashare.filterService.filters.ctrls.find(c => c.field === "firstTime").value;
    return firstTime === 'true';
  }

  private get _hasLoadedSelf(): boolean {
    return this.usersService.masterService.isSuperuser() || this._self.length > 0;
  }

  private get _isSearchingSelf(): boolean {
    const usernameCtrl = this.datashare.loadUsersFilters.value.ctrls.find(ctrl => ctrl.field === "Username");
    return usernameCtrl.value === this.usersService.masterService.getCurrentUserUsername()
    && usernameCtrl.filterType === enum_TipoFiltro.EQUALS;
  }

  private get _areFiltersEmpty(): boolean {
    const visibleFields = this.datashare.filterService.filterableProperties.map(fg => fg.value.field);
    return this.datashare.loadUsersFilters.value.ctrls
      .filter(ctrl => visibleFields.includes(ctrl.field))
      .map(ctrl => ctrl.value)
      .every(value => !value);
  }

  /**
   * @param forceRead if `true` forces the reload of the users from the database.
   */
  public loadUsersFromServer(forceRead: boolean = false): Observable<UtenteFlatModel[]> {
    if (this._initFinished$.value) {
      return this.innerUsersLoad(forceRead);
    } else {
      return this._initFinished$.pipe(
        filter(done => !!done),
        take(1),
        switchMap(() => this.innerUsersLoad(forceRead))
      );
    }
  }

  public setFiltersForLoadAll(preventIfBigBusiness = false) {
    // if (preventIfBigBusiness && !this._isSmallMediumBusiness) {
    //   return;
    // }
    this.datashare.removeUserDateFilter();
    const fArray = (this.datashare.loadUsersFilters.controls.ctrls as FormArray);
    for (let ctrlsKey in fArray.controls) {
      fArray.get(ctrlsKey).get("value").patchValue("");
    }
  }

  private innerUsersLoad(forceRead: boolean = false): Observable<UtenteFlatModel[]> {
    if (!this._isSmallMediumBusiness && this._hasLoadedSelf && this._isSearchingSelf && !forceRead) {
      return of(this._self);
    }
    const toLoad = this.datashare.getUserFiltersJson();
    if (toLoad === this._lastUsedFilter && !forceRead) {
      return of(this._lastLoaded);
    }
    this._lastUsedFilter = toLoad;
    return this.usersService.readUtentiFlat(true, false, toLoad).pipe(
      take(1),
      tap((users: UtenteFlatModel[]) => {
        for (let u of users) {
          this.fixTranslations(u);
          if (u.UserName === this.usersService.masterService.getCurrentUserUsername()) {
            this._self = [u];
          }
        }
        this._lastLoaded = users;
      })
    );
  }

  /**
   * Fixes the user fields applying the correct translations based on the current culture.
   * The function also rephrase the user's data of the users with type 'company' (Flag_Azienda_Persona = 1) changing
   * the value of the field Cognome to contain the company name, and the fields PIVA or CodFisc whith the value of
   * the field that is actually valorized.
   */
  private fixTranslations(user: UtenteFlatModel): void {
    user.Attivo = user.Attivo ? this._commonTranslations.active : this._commonTranslations.notActive;
    user.Azienda_Persona = user.Flag_Azienda_Persona === enum_Azienda_Persona.Impresa
      ? this._commonTranslations.business : this._commonTranslations.person;
    user.GDPR = +user.GDPR === 1 ? this._commonTranslations.yes : this._commonTranslations.no;

    user.Cognome = user.Flag_Azienda_Persona === enum_Azienda_Persona.Impresa ? user.Rag_Soc : user.Cognome;
    if (user.Flag_Azienda_Persona === enum_Azienda_Persona.Impresa) {
      if (user.PIVA == "" && user.CodFisc != "") user.PIVA = user.CodFisc;
      else if (user.CodFisc == "" && user.PIVA != "") user.CodFisc = user.PIVA;
    }
    // user.CodFisc = user.Flag_Azienda_Persona === enum_Azienda_Persona.Impresa ? user.PIVA : user.CodFisc;
  }

  private initTranslations() {
    this._commonTranslations = {
      no: this.transloco.translate('No'),
      yes: this.transloco.translate('Si'),
      active: this.transloco.translate('Attivo'),
      notActive: this.transloco.translate('NonAttivo'),
      business: this.transloco.translate('Azienda'),
      person: this.transloco.translate('Persona')
    };
  }

  private initConstantParams() {
    forkJoin([
      this.usersService.getUsersCount().pipe(tap(count => this._userCount = count)),
      this.siteConfig.leggiChiave("AgroProfilazione_MaxUtentiCaricatiDefault").pipe(
        take(1),
        tap(r => this._mediumBuisinessSizeLimit = !!(+r?.Valore) ? +r.Valore : this._mediumBuisinessSizeLimit)
      )
    ]).subscribe(() => this._initFinished$.next(true));
  }
}

import { Component, DoCheck, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { MenuProfilazioneGridConfigService } from './grid-menu-profilazione.service';
import { ProfilazioneUtentiService, Scrivi_Utenti } from 'app/profilazione/services/profilazione-utenti.service';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { TooltipDirective } from '@progress/kendo-angular-tooltip';
import { TranslocoService } from '@jsverse/transloco';
import { Utente, UtenteFlatModel } from 'app/profilazione/models/utente.model';
import { TipologiaUtente } from '../../../models/profili-permessi/tipologia-utente.model';
import { GiasDialogService } from '../../../../Service/gias-dialog.service';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { PermessiUtenteService } from '../../../../Service/permessi-utente.service';
import { forkJoin, map, Observable, of, Subject, switchMap, take, takeUntil } from 'rxjs';
import { VisibilitaService } from '../../../services/visibilita.service';
import { GruppoUtente } from '../../../models/gruppi-utenti/GruppoUtente.model';
import { GiasMultiSelectTemplateService, GiasWindowsService } from 'gias-ui-kit';
import { GruppiUtentiService } from '../../../services/gruppi-utenti.service';
import { FormControl, FormGroup } from '@angular/forms';
import { faArrowsToEye, faCalendarMinus, faCalendarWeek, faCopy, faPeopleGroup, faTrashAlt, faUser, faUserGear, faUserGroup } from '@fortawesome/free-solid-svg-icons';
import { TipologieUtentiService } from '../../../services/profili-permessi/tipologie-utenti.service';
import { ImpostazioniFormService } from '../../../services/impostazioni/impostazioni-form.service';
import { enum_PaginaImpostazioni } from '../../../impostazioni-utente/impostazioni.model';
import { BaseCodeDescrStr } from '../../../../Model/baseClass/baseCodeDescrStr';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { FormImpostazioniComponent } from '../../../impostazioni-utente/form-impostazioni/form-impostazioni.component';
import { AGRODATAFINE, AGRODATAINIZIO } from '../../../../Model/CostantiPersonalizzate';
import { UtentePermessi } from '../../../models/profili-permessi/UtentePermessi.model';
import { BaseCodeDescr } from '../../../../Model/baseClass/baseCodeDescr';
import { DatiBaseUtente } from 'app/Service/api.service';
import { ProfilazioneDataShareService } from '../../../services/profilazione-data-share.service';
import { IUtenteFinestraTemp } from '../../../models/IUtente.model';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { MasterService } from 'app/Service/master.service';
import { UsersLoaderService } from './users-loader.service';

enum DateRangeApplyMode {
  BOTH = 0,
  ONLY_START = 1,
  ONLY_END = 2
}

@Component({
  standalone: false,
  selector: 'app-grid-menu-profilazione',
  templateUrl: './grid-menu-profiliazione.component.html',
  styleUrls: ['./grid-menu-profilazione.component.css'],
  providers: [
    ...generateGridProviders(MenuProfilazioneGridConfigService, MenuProfilazioneGridComponent),
    GiasMultiSelectTemplateService,
    GiasDropDownTemplateService
  ]
})
export class MenuProfilazioneGridComponent implements OnInit, DoCheck, OnDestroy {
  @ViewChild('grid') grid: GiasKendoGridComponent;
  @ViewChild(TooltipDirective) public tooltipDir: TooltipDirective;
  @ViewChild('assignProfileForm') public assignProfileForm: TemplateRef<any>;
  @ViewChild('assignGroupsForm') public assignGroupsForm: TemplateRef<any>;
  @ViewChild('visibilitaRef') public visibilitaRef: TemplateRef<any>;
  @ViewChild('impostazioniRef') public impostazioniRef: TemplateRef<any>;
  @ViewChild('copySettingsRef') public copySettingsRef: TemplateRef<any>;
  @ViewChild('copyFromUserDdl') public copyFromUserDdl: GiasDropDownTemplateSComponent;
  @ViewChild('validitaTemplate') public validitaTemplate: TemplateRef<any>;
  @ViewChild('formImpostazioni') public formImpostazioni: FormImpostazioniComponent;

  public readonly faGroup = faPeopleGroup;
  public readonly faToEye = faArrowsToEye;
  public readonly faUser = faUser;
  public readonly faUserGear = faUserGear;
  public readonly faUserGroup = faUserGroup;
  public readonly faCopy = faCopy;
  public readonly faDelete = faTrashAlt;
  public readonly faCalendar = faCalendarWeek;
  public readonly faCalendarClock = faCalendarMinus;
  public readonly AGRODATA_INIZIO = AGRODATAINIZIO;
  public readonly AGRODATA_FINE = AGRODATAFINE;

  public dateRadioChoice: BaseCodeDescr[];
  public virtual: any = { itemHeight: 28 };
  public listProfiles: TipologiaUtente[];
  public listGroups: GruppoUtente[];
  public placeholderSelezioneProfilo = new TipologiaUtente(0, this.transloco.translate('SelezionaProfilo'));

  protected profileHasNoPermission: boolean;
  protected utentiDdl: BaseCodeDescrStr[] = [];
  protected form: FormGroup = new FormGroup({
    gruppi: new FormControl([]),
    profilo: new FormControl(''),
    utente: new FormControl(''),
    copySettings: new FormControl(false),
    validitaInizio: new FormControl(new Date()),
    validitaFine: new FormControl(new Date()),
    applyMode: new FormControl(0)
  });

  private currentProfile: TipologiaUtente;
  private signal = new Subject();
  private isUserDdlLoading = false;

  constructor(
    protected transloco: TranslocoService,
    private utentiService: ProfilazioneUtentiService,
    private gruppiService: GruppiUtentiService,
    private tipologiaService: TipologieUtentiService,
    private permessi: PermessiUtenteService,
    private master: MasterService,
    private impostazioniService: ImpostazioniFormService,
    private dialogService: GiasDialogService,
    private windowService: GiasWindowsService,
    private visibilitaService: VisibilitaService,
    protected dataShare: ProfilazioneDataShareService,
    private usersLoader: UsersLoaderService
  ) {
    this.tipologiaService.readTipologie(true).subscribe(t => this.listProfiles = t);
    this.gruppiService.readGruppi().subscribe(g => this.listGroups = g);
  }

  protected get isAddingNew(): boolean {
    return this.dataShare.isAddingNew;
  }

  protected get selectedRows(): Utente[] {
    const str2date = (dataStr) => this.utentiService.intlService.parseDate(dataStr, '', this.transloco.getActiveLang());
    return this.selectedFlatRows.map((flat: UtenteFlatModel) => {
      const u = new Utente();
      u.fromFlat(flat);
      u.DatiAccesso.UltimoAccesso = str2date(flat['UltimoAccesso']);
      return u;
    });
  }
  protected get selectedFlatRows(): UtenteFlatModel[] {
    return this.grid.rows.filter(row => row['Selected']) as UtenteFlatModel[];
  }

  protected get canManageSettings(): boolean {
    return this.dataShare.userPermissions.canViewBaseUserSettings;
  }

  private get _myUsername(): string {
    return this.master.objP_utenti.UtenteUsername;
  }

  getStringaUtenti(): string {
    return this.selectedRows.map(u => u.UserName).reduce((a, b) => a + ', ' + b);
  }

  ngOnInit() {
    this.dateRadioChoice = [
      new BaseCodeDescr(DateRangeApplyMode.ONLY_START, this.transloco.translate('ApplicaSoloValiditaInizio')),
      new BaseCodeDescr(DateRangeApplyMode.ONLY_END, this.transloco.translate('ApplicaSoloValiditaFine')),
      new BaseCodeDescr(DateRangeApplyMode.BOTH, this.transloco.translate('ApplicaIntervalloTempo')),
    ];
  }

  ngDoCheck(): void {
    this.resizeWindow();
    this.resizeDatePopup();
    this.handleUserDdlLoading();
    if (this.grid) this.dataShare.usersGrid = this.grid;
  }

  ngOnDestroy() {
    this.signal.next(true);
    this.signal.complete();
  }

  loadAll() {
    this.preventIfPendingChanges(() => {
      //this.dataShare.loadAllUtenti = true;
      this.usersLoader.setFiltersForLoadAll();
      this.utentiService.userReloaded$.next(true);
    });
  }

  /**
   * @history
   * **(22/11/2023)**: Se gli utenti selezionati non sono stati già stati associati a un profilo,
   * associo di default le impostazioni del profilo abilitando il check nel form.
   * Comunque possibile togliere il check per non associare le impostazioni.
   * Motivo: Fatto per facilitare la creazione e attribuzione di impostazioni sui nuovi utenti.
   *
   * **(11/03/2024)**: Come default, le impostazioni del profilo non vengono mai associate.
   * Motivo: Gli utenti lasciavano attivo il check e lamentavano poi perdita di impostazioni.
   */
  assignProfile() {
    this.preventOnSelf(() => this.preventIfPendingChangesOrNullSelection(() => {
      this.resetProfileForm();
      this.dialogService.dialogMessageObs_Result(
        this.transloco.translate('SelezionaProfilo'), this.assignProfileForm
      ).subscribe(result => {
        if (this.profileHasNoPermission) {
          this.dialogService.baseError("prof.ImpossibileAssegnareProfilo", "prof.ImpossibileAssegnareProfiloText");
        } else if (result['returnObj'])
          this.confirmAssignProfile();
      });
    }), false);
  }

  assignGroups() {
    this.preventOnSelf(() => this.preventIfPendingChangesOrNullSelection(() =>
      this.dialogService.dialogMessageObs_Result(
        this.transloco.translate('prof.SelezioneGruppi'), this.assignGroupsForm
      ).subscribe(result => {
        if (result['returnObj'])
          this.confirmAssignGroups();
      })), false);
  }

  editVisibilita() {
    this.preventIfNoPermission(() => this.preventIfPendingChangesOrNullSelection(() =>
      this.checkVisibilitaValida().pipe(take(1))
        .subscribe(canEdit => {
          if (canEdit) {
            this.visibilitaService.utentiSelezionati = this.selectedRows;
            this.apriVisibilitaEdit();
          }
        })
    ), true);
  }

  editSettings() {
    this.preventIfPendingChangesOrNullSelection(() => {
      if (!this.canManageSettings || (!this.isCurrentUserSelected() && !this.dataShare.userPermissions.canEditAllUsers)) {
        this.dialogService.baseError('_Error', 'prof.errNoPermissions');
        return;
      }
      if (this.hasNotMultiSelection()) {
        this.loadUsers4Ddl();
        const title = this.transloco.translate("prof.ImpostazioniUtenteDellUtente") + ' '
          + this.selectedRows[0].Dettagli + ' (' + this.selectedRows[0].UserName + ')';
        this.impostazioniService.USAGE_AREA = enum_PaginaImpostazioni.UTENTI;
        this.impostazioniService.utentiSelezionati = this.selectedRows.map(u => u.UserName);
        let winRef = this.windowService.open({
          title: title,
          content: this.impostazioniRef,
          height: window.innerHeight * 0.9,
          width: window.innerWidth * 0.9,
        });
        winRef.result.pipe(takeUntil(this.signal)).subscribe(() => this.deselectIfSingleUser());
      }
    });
  }

  onResetSettings() {
    this.dialogService.warningThen(
      'prof.ReimpostaPredefiniti', 'prof.WarningResetDefaultsAllUserSettings', true,
      () => this.impostazioniService.resetImpostazioniToDefault(
        this.selectedRows.map(u => u.UserName), [0]
      ).pipe(take(1))
        .subscribe(ok => ok ? this.formImpostazioni.loadSettings() : null)
    );
  }

  editValidityDates() {
    this.preventOnSelf(() => this.preventIfPendingChangesOrNullSelection(() => {
      const title = this.transloco.translate('prof.ModificaValiditaPermessi');
      this.valorizeValidityDates();
      this.dialogService.dialogMessageObs_Result(title, this.validitaTemplate)
        .subscribe(R => {
          if (R['returnObj']) {
            this.applicaIntervalloValidita();
          }
        });
    }), false);
  }

  editUserWindow() {
    this.preventOnSelf(() => this.preventIfPendingChangesOrNullSelection(() => {
      const title = this.transloco.translate('prof.ModificaFinestraTemporale');
      this.valorizeFormWithTemporalWindowDates();
      this.dialogService.dialogMessageObs_Result(title, this.validitaTemplate)
        .subscribe(R => {
          if (R['returnObj']) {
            this.applicaFinestraTemporale();
          }
        });
    }), false);
  }

  openCopySettingsDialog() {
    let winRefs = document.getElementsByClassName('k-window ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++) {
      winRefs[i]?.classList.remove('resized');
    }
    this.dialogService.dialogMessageObs_Result(
      this.transloco.translate('prof.SelezioneUtente'),
      this.copySettingsRef
    ).pipe(switchMap(res => {
      if (res['returnObj']) {
        let base = this.selectedRows.map(u => u.UserName);
        let template = this.form.value.utente;
        return this.impostazioniService.copiaImpostazioni(base, template);
      }
      return of(null);
    })).subscribe(res => {
      if (!!res) {
        //refresh form
        this.formImpostazioni.loadSettings();
      }
    });
  }

  saveSettings(onlyTouched: boolean) {
    let winRefs = document.getElementsByClassName('k-window ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++) {
      winRefs[i]?.classList.remove('resized');
    }
    let content = onlyTouched ? 'prof.WarningSalvaModificate' : 'prof.WarningSalvaTutto';
    if (!this.dataShare.userPermissions.canEditBaseUserSettings) {
      this.dialogService.baseError('_Error', 'prof.errNoPermissions');
      return;
    }
    this.dialogService.warningThen('prof.SalvataggioImpostazioni', content, true,
      () => {
        if (onlyTouched)
          this.impostazioniService.salvaImpostazioniModificate();
        else
          this.impostazioniService.salvaTutteImpostazioni();
      });
  }

  changeValue(e: any) {
    this.currentProfile = this.listProfiles.filter(p => p.codice === e.data).at(0);
    this.tipologiaService.hasPermissionsOrSettings(this.currentProfile)
      .subscribe(R => this.profileHasNoPermission = !R.haPermessi);
  }

  private loadUsers4Ddl() {
    if (this.utentiDdl.length === 0 && this.dataShare.userPermissions.canReadAllUsers) {
      this.isUserDdlLoading = true;
      this.utentiService.readUtentiDatiBase(true)
        .subscribe(r => {
          this.utentiDdl = r.map((el: DatiBaseUtente) =>
            new BaseCodeDescrStr(el.UserName, el.Descrizione + " (" + el.UserName + ")")
          );
          this.isUserDdlLoading = false;
          this.handleUserDdlLoading();
        });
    }
  }

  private resetProfileForm() {
    this.profileHasNoPermission = false;
    this.form.get('profilo').patchValue('');
    this.form.get('copySettings').patchValue(false);
  }

  private confirmAssignProfile() {
    if (this.currentProfile === undefined || this.currentProfile.codice === 0) {
      this.dialogService.baseError("Errore_", "prof.SelezionaProfiloDaAssegnare");
      return;
    }
    if (this.isCurrentUserSelected()) {
      this.dialogService.baseError("Errore_", "prof.errCannotEditSelf");
      return;
    }
    const selected = this.selectedFlatRows.map(u => ({
      UserName: u['UserName'],
      Password: '',
      ValiditaInizioPermessi: u['Validita_Inizio'],
      ValiditaFinePermessi: u['Validita_Fine'],
      Tipologia: null
    } as UtentePermessi));
    this.tipologiaService.associaTipologia(selected, this.currentProfile, this.form.value.copySettings)
      .pipe(take(1)).subscribe(() => this.utentiService.userReloaded$.next(true));
  }

  private confirmAssignGroups() {
    const selected = this.selectedFlatRows;
    const toAssign: number[] = this.form.get('gruppi').value;
    const groups = this.listGroups.filter(g => toAssign.includes(g.codice));
    for (let u of selected) {
      u.GruppiCod = groups.map(g => +g.codice);
      u.GruppiDes = groups.map(g => g.descrizione);
    }
    this.form.get('gruppi').patchValue([]);
    this.utentiService.saveUsers(new Scrivi_Utenti(selected, enum_TipoOperazioneDB.Modifica))
      .pipe(take(1)).subscribe(() => this.utentiService.userReloaded$.next(true));
  }

  private applicaIntervalloValidita() {
    const applyMode = this.form.get('applyMode').value;
    const selected = this.selectedRows;
    selected.forEach((u: UtentePermessi) => {
      if (applyMode === DateRangeApplyMode.BOTH || applyMode === DateRangeApplyMode.ONLY_START)
        u.ValiditaInizioPermessi = this.form.get('validitaInizio').value;
      if (applyMode === DateRangeApplyMode.BOTH || applyMode === DateRangeApplyMode.ONLY_END)
        u.ValiditaFinePermessi = this.form.get('validitaFine').value;
    });
    this.tipologiaService.modificaValidita(selected).subscribe(saved => {
      if (saved) this.utentiService.userReloaded$.next(true);
    });
  }

  private applicaFinestraTemporale() {
    const applyMode = this.form.get('applyMode').value;
    const selected = this.selectedRows;
    selected.forEach((u: IUtenteFinestraTemp) => {
      if (applyMode === DateRangeApplyMode.BOTH || applyMode === DateRangeApplyMode.ONLY_START)
        u.FinestraTemporale.inizio = this.form.get('validitaInizio').value;
      if (applyMode === DateRangeApplyMode.BOTH || applyMode === DateRangeApplyMode.ONLY_END)
        u.FinestraTemporale.fine = this.form.get('validitaFine').value;
    });
    this.utentiService.editFinestraTemporale(selected).subscribe(saved => {
      if (saved) this.utentiService.userReloaded$.next(true);
    });
  }

  /**
   * Controlla la visibilità degli utenti selezionati assicurandosi l'utente corrente sia in grado di modificarla.
   * @private
   * @return True se l'utente è in condizioni di modificare la visibilità degli utenti selezionati, False altrimenti.
   */
  private checkVisibilitaValida(): Observable<boolean> {
    const obs = new Array<Observable<any>>();
    const hasBiggerVisibility = 1;
    this.master.set_isLoading({ isLoading: true });
    obs.push(this.visibilitaService.confrontaVisibilita(
      this.selectedRows[0].UserName,
      this.permessi.getCurrentUser().Username)
    );
    // if (this.selectedRows.length > 1)
    //   obs.push(this.visibilitaService.hannoStessaVisibilita(
    //     this.selectedRows.map((u: UtenteFlatModel) => u.UserName))
    //   );
    return forkJoin(obs).pipe(map(values => {
      this.master.set_isLoading({ isLoading: false });
      if (values[0] === hasBiggerVisibility) {
        this.dialogService.baseError(
          'prof.ImpossibileGestioneContemporaneaVisibilitaUtenti',
          'prof.InfoErroreVisibilitaInsufficiente');
        return false;
      }
      // if (values[1] === false) {
      //   this.dialogService.baseError(
      //     'prof.ImpossibileGestioneContemporaneaVisibilitaUtenti',
      //     'prof.InfoErroreVisibilitaDiversa');
      //   return false;
      // }
      return true;
    }));
  }

  private apriVisibilitaEdit() {
    const resetData = () => {
      this.visibilitaService.utentiSelezionati = [];
      this.visibilitaService.impreseSelezionate = [];
    };
    let winRef = this.windowService.open({
      title: this.transloco.translate("prof.VisibilitaUtenti"),
      content: this.visibilitaRef,
      height: window.innerHeight * 0.9,
      width: window.innerWidth * 0.9,
    }, false);
    winRef.window.location.nativeElement.setAttribute('style', 'top: 5% !important;');
    this.visibilitaService.saved$.pipe(take(1))
      .subscribe(saved => {
        if (saved) winRef.close();
        this.deselectIfSingleUser();
        //   this.visibilitaService.gridRefresh$.next(true);
      });
    winRef.result.subscribe(() => resetData());
  }

  private deselectIfSingleUser() {
    const selected: any[] = this.selectedRows;
    if (selected?.length === 1)
      this.grid.deselectAllRows();
  }

  private resizeWindow(): void {
    let winRefs = document.getElementsByClassName('k-window ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++) {
      if (!winRefs[i]?.classList.contains('resized')) {
        winRefs[i]?.setAttribute('style', 'z-index:10000 !important; top: 5%; left: 5%; width: 90%; height: 90%;');
        winRefs[i]?.classList.add('resized');
      }
    }
    winRefs = document.getElementsByClassName('k-content k-window-content ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++)
      winRefs[i].setAttribute('style', 'overflow: auto !important');
    winRefs = document.getElementsByClassName('k-dialog-wrapper');
    for (let i = 0; i < winRefs.length; i++)
      winRefs[i].setAttribute('style', 'z-index:10005 !important;');
  }

  private resizeDatePopup() {
    let popups = document.getElementsByTagName('kendo-popup');
    for (let i = 0; i < popups.length; i++) {
      if (popups[i].children?.item(0)?.classList?.contains("k-datetime-container")
        && popups[i].getBoundingClientRect().bottom > document.body.offsetHeight) {
        const height = popups[i].getBoundingClientRect().height;
        const difference = popups[i].getBoundingClientRect().bottom - document.body.offsetHeight;
        let style = popups[i].getAttribute('style');
        style += " height: " + (height - difference) + "px;";
        style += " overflow-y: scroll;";
        popups[i].setAttribute('style', style);
      }
    }
  }

  private handleUserDdlLoading() {
    if (this.copyFromUserDdl) {
      this.copyFromUserDdl.loading = this.isUserDdlLoading;
    }
  }

  private valorizeValidityDates() {
    const selected = this.selectedRows;
    let maxDataInizio = selected.map(u => u.ValiditaInizioPermessi)
      .reduce((acc, d) => acc = d > acc ? d : acc);
    let minDataFine = selected.map(u => u.ValiditaFinePermessi)
      .reduce((acc, d) => acc = d < acc ? d : acc);
    if (minDataFine < maxDataInizio) {
      minDataFine = maxDataInizio;
    }
    this.form.get('validitaInizio').patchValue(maxDataInizio);
    this.form.get('validitaFine').patchValue(minDataFine);
  }

  private valorizeFormWithTemporalWindowDates() {
    const selected = this.selectedRows;
    let maxDataInizio = selected.map(u => u.FinestraTemporale.inizio)
      .reduce((acc, d) => acc = d > acc ? d : acc);
    let minDataFine = selected.map(u => u.FinestraTemporale.fine)
      .reduce((acc, d) => acc = d < acc ? d : acc);
    if (minDataFine < maxDataInizio) {
      minDataFine = maxDataInizio;
    }
    this.form.get('validitaInizio').patchValue(maxDataInizio);
    this.form.get('validitaFine').patchValue(minDataFine);
  }

  // EXECUTION PREVENTION

  private preventIfPendingChanges(action: Function) {
    if (this.grid.gridServices.incellHasChanges()) {
      this.dialogService.baseError('prof.ModificheSospese', 'prof.SalvaModifichePerContinuare');
    } else {
      action.call(null);
    }
  }

  private preventIfNullSelection(action: Function) {
    this.preventIf(action, this.selectedRows.length === 0, 'prof.WarningUtenteNullo');
  }

  private preventIfPendingChangesOrNullSelection(action: Function) {
    this.preventIfPendingChanges(() => this.preventIfNullSelection(action));
  }

  private preventEditOtherIfNoPermission(action: Function) {
    const hasSelectedOthers = this.selectedRows.length && this.selectedRows.every(u => u.UserName !== this._myUsername);
    const hasNoPermission = !this.dataShare.userPermissions.canEditAllUsers;
    this.preventIf(action, hasSelectedOthers && hasNoPermission, 'prof.errNoPermissions');
  }

  private preventOnSelf(action: Function, checkPermission: boolean) {
    const hasPermission = checkPermission ? this.dataShare.userPermissions.canEditSelf : false;
    this.preventIf(action, this.isCurrentUserSelected() && !hasPermission, "prof.errCannotEditSelf");
  }

  private preventIfNoPermission(action: Function, forcePreventSelf: boolean) {
    this.preventEditOtherIfNoPermission(() => this.preventOnSelf(action, !forcePreventSelf));
  }

  /**
   * @param preventCondition if true the action is prevented
   * @private
   */
  private preventIf(action: Function, preventCondition: boolean, errMsg?: string) {
    if (!preventCondition) {
      action.call(null);
    } else if (errMsg) {
      this.dialogService.baseError('Errore_', errMsg);
    }
  }

  // SELECTION CHECKS

  private hasNotMultiSelection(): boolean {
    if (this.selectedRows.length > 1) {
      this.dialogService.baseError('Errore_', 'prof.WarningOperazioneNonSupportaMultiSelezione');
      return false;
    }
    return true;
  }

  private isCurrentUserSelected(): boolean {
    return this.selectedRows.some(u => u.UserName === this._myUsername);
  }

}

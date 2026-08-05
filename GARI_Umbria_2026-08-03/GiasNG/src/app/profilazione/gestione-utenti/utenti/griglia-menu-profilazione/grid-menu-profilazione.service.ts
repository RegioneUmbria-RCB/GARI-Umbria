import { Injectable, Injector, Renderer2 } from '@angular/core';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { TipologiaUtente } from '../../../models/profili-permessi/tipologia-utente.model';
import { enum_Azienda_Persona, UtenteFlatModel } from 'app/profilazione/models/utente.model';
import { ProfilazioneUtentiService, Scrivi_Utenti } from 'app/profilazione/services/profilazione-utenti.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { isSuperUser } from 'app/Service/utils';
import { AbstractGridConfigService, CommandsColumnSettings, DettagliColumnSettings } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { BooleanSettings, DropdownListItem, DropdownListWithForm, EditingMode, HttpAction, InCellBatchSaveEventObject, KendoGridColumn, KendoServerResult, LoaderType, ModelEntry, RendererGridEvent, ToolbarSettings } from 'gias-kendo-grid';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { cloneDeep } from 'lodash';
import { BehaviorSubject, filter, forkJoin, map, Observable, of, switchMap, take, takeUntil, tap } from 'rxjs';
import { AGRODATAFINE, AGRODATAINIZIO } from "../../../../Model/CostantiPersonalizzate";
import { GridPasswordComponent } from "../../components/grid-password/grid-password.component";
import { FormArray, FormGroup, Validators } from "@angular/forms";
import { BaseCodeDescr } from "../../../../Model/baseClass/baseCodeDescr";
import { GruppiUtentiService } from "../../../services/gruppi-utenti.service";
import { MasterService } from "../../../../Service/master.service";
import { TipologieUtentiService } from "../../../services/profili-permessi/tipologie-utenti.service";
import { ProfilazioneDataShareService } from "../../../services/profilazione-data-share.service";
import { ColumnComponent } from "@progress/kendo-angular-grid";
import { GiasDialogService } from "../../../../Service/gias-dialog.service";
import { UsersLoaderService } from "./users-loader.service";
import { CfInputTemplateComponent } from "../../components/cf-input-template/cf-input-template.component";

export class GridProfilazioneServerResult extends KendoServerResult {
  constructor(model, columns, rows) {
    super(model, columns, rows);
  }
}

@Injectable()
export class MenuProfilazioneGridConfigService extends AbstractGridConfigService<GridProfilazioneServerResult> {
  editingMode: EditingMode = EditingMode.IN_CELL_BATCH;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = "kendoKey";
  gridId = "MenuProfilazioneGrid";
  cmdColumn: CommandsColumnSettings;
  dettagliColumn: DettagliColumnSettings;
  objParametriAgenda: ObjParametriAgenda;

  private bloccaEditProfiloInline = true;
  private hasAddedUser = false;
  /** Contiene gli utenti in fase di salvataggio. Se il salvataggio è andato a buon fine l'array viene svuotato.
   * In caso contrario manterrà i record degli utenti per cui si sono riscontrati errori,
   * @private
   */
  private newUsers: UtenteFlatModel[] = [];
  private ddlLoaded$ = new BehaviorSubject<boolean>(false);
  private _forceReload = false;
  /** Campo attualmente in fase di editing */
  private _editingField: string = null;

  private readonly essentialFields = [
    'UserName', 'Flag_Azienda_Persona', 'CodFisc', 'Tipologia_Cod', 'Validita_Inizio', 'Validita_Fine',
    'Nome', 'Cognome', 'Password', 'UserNameCommerciale', 'Email'
  ];
  private readonly newUserEditableFields = [
    'UserName', 'Flag_Azienda_Persona', 'CodFisc', 'Tipologia_Cod', 'Validita_Inizio', 'Validita_Fine'
  ];
  private kendoRows = [];
  private kendoModel = {
    Attivo: new ModelEntry(CELL_TYPES.STRING),
    Azienda_Persona: new ModelEntry(CELL_TYPES.STRING),
    CodFisc: new ModelEntry(CELL_TYPES.CUSTOM),
    Cognome: new ModelEntry(CELL_TYPES.STRING),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATE),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATE),
    Username_Creazione: new ModelEntry(CELL_TYPES.STRING),
    Username_Modifica: new ModelEntry(CELL_TYPES.STRING),
    Dettagli: new ModelEntry(CELL_TYPES.STRING),
    Email: new ModelEntry(CELL_TYPES.STRING),
    Flag_Azienda_Persona: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    Flag_Encrypted: new ModelEntry(CELL_TYPES.NUMBER),
    Flag_Accesso_SPID: new ModelEntry(CELL_TYPES.BOOLEAN),
    CF_SPID: new ModelEntry(CELL_TYPES.STRING),
    GDPR: new ModelEntry(CELL_TYPES.STRING),
    GruppiCod: new ModelEntry(CELL_TYPES.MULTI_DROPDOWNLIST),
    GruppiDes: new ModelEntry(CELL_TYPES.STRING),
    kendoKey: new ModelEntry(CELL_TYPES.STRING),
    Lingua_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    Lingua_Des: new ModelEntry(CELL_TYPES.STRING),
    Nome: new ModelEntry(CELL_TYPES.STRING),
    NumeroAccessi: new ModelEntry(CELL_TYPES.NUMBER),
    Password: new ModelEntry(CELL_TYPES.CUSTOM),
    PIVA: new ModelEntry(CELL_TYPES.STRING),
    Piva_SuperUser: new ModelEntry(CELL_TYPES.STRING),
    Rag_Soc: new ModelEntry(CELL_TYPES.STRING),
    ServiziAttivi: new ModelEntry(CELL_TYPES.STRING),
    Tel: new ModelEntry(CELL_TYPES.STRING),
    Tipologia_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    Tipologia_Des: new ModelEntry(CELL_TYPES.STRING),
    UltimoAccesso: new ModelEntry(CELL_TYPES.STRING),
    UserName: new ModelEntry(CELL_TYPES.STRING),
    UserNameCommerciale: new ModelEntry(CELL_TYPES.STRING),
    Utente: new ModelEntry(CELL_TYPES.STRING),
    Utente_Profilo: new ModelEntry(CELL_TYPES.STRING),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE),
    FinestraTemporaleInizio: new ModelEntry(CELL_TYPES.DATE),
    FinestraTemporaleFine: new ModelEntry(CELL_TYPES.DATE),
  };
  /** Define the columns of the grid.
   * @private
   */
  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'GDPR', title: this.transloco.translate('GDPR') }, { editable: false, hidden: false }),
    new KendoGridColumn({ field: 'UltimoAccesso', title: this.transloco.translate('UltimoAccesso') }, { editable: false }),
    new KendoGridColumn({ field: 'UserName', title: this.transloco.translate('Username') }, { editable: true, width: 140 }),
    new KendoGridColumn({ field: 'Flag_Azienda_Persona', title: this.transloco.translate('AziendaPersona') }, { editable: true, width: 153, hidden: true }),
    new KendoGridColumn({ field: 'Cognome', title: this.transloco.translate('Cognome') }, { editable: true, width: 110 }),
    new KendoGridColumn({ field: 'Nome', title: this.transloco.translate('Nome') }, { editable: true, width: 110 }),
    new KendoGridColumn({ field: 'NumeroAccessi', title: this.transloco.translate('NumeroAccessi') }, { hidden: true, editable: false }),
    new KendoGridColumn({ field: 'UserNameCommerciale', title: this.transloco.translate('UserNameCommerciale') }, { editable: true, width: 205 }), // Ex 'Qualifica'
    new KendoGridColumn({ field: 'CodFisc', title: this.transloco.translate('CodiceFiscale') }, { editable: true, width: 180, component: CfInputTemplateComponent, filterable: false }),
    new KendoGridColumn({ field: 'GruppiCod', title: this.transloco.translate('Gruppo') }, { editable: true, width: 140 }),
    new KendoGridColumn({ field: 'Tipologia_Cod', title: this.transloco.translate('Profilo') }, { editable: true, width: 140 }),
    new KendoGridColumn({ field: 'Attivo', title: this.transloco.translate('Attivo') }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'Validita_Inizio', title: this.transloco.translate('DataInizio') }, { editable: true, hidden: true, width: 150, date: { placeholder: '...' } }),
    new KendoGridColumn({ field: 'Validita_Fine', title: this.transloco.translate('DataFine') }, { editable: true, hidden: true, width: 150, date: { placeholder: '...' } }),
    new KendoGridColumn({ field: 'FinestraTemporaleInizio', title: this.transloco.translate('FinestraTempInizio') }, { editable: false, hidden: true, width: 150, date: { placeholder: '...' } }),
    new KendoGridColumn({ field: 'FinestraTemporaleFine', title: this.transloco.translate('FinestraTempFine') }, { editable: false, hidden: true, width: 150, date: { placeholder: '...' } }),
    new KendoGridColumn({ field: 'Lingua_Cod', title: this.transloco.translate('Lingua') }, { editable: true, hidden: true, width: 110 }),
    new KendoGridColumn({ field: 'Tel', title: this.transloco.translate('Telefono') }, { editable: true, width: 110 }),
    new KendoGridColumn({ field: 'Email', title: this.transloco.translate('Email') }, { editable: true, width: 205 }),
    new KendoGridColumn({ field: 'Password', title: this.transloco.translate('Password') }, { editable: true, hidden: false, filterable: false, width: 250, component: GridPasswordComponent }),
    new KendoGridColumn({ field: 'Username_Creazione', title: this.transloco.translate('CreatoDa') }, { editable: false, hidden: false }),
    new KendoGridColumn({ field: 'Username_Modifica', title: this.transloco.translate('UltimaModificaDi') }, { editable: false, hidden: false }),
    new KendoGridColumn({ field: 'Data_Creazione', title: this.transloco.translate('DataCreazione') }, { editable: false, hidden: false }),
    new KendoGridColumn({ field: 'Data_Modifica', title: this.transloco.translate('DataModifica') }, { editable: false, hidden: false })
  ];

  constructor(
    injector: Injector,
    private renderer: Renderer2,
    private master: MasterService,
    private dialog: GiasDialogService,
    private utentiService: ProfilazioneUtentiService,
    private gruppiService: GruppiUtentiService,
    private tipologieService: TipologieUtentiService,
    private datashare: ProfilazioneDataShareService,
    private usersLoader: UsersLoaderService
  ) {
    super(injector);
    this.handleCustomizations();
    this.handleDropdowns();
    this.handleEvents();
    this.disableSuperuserRow();
    this.pagination.gridState.sort = [{ field: 'Attivo', dir: 'asc' }];
  }

  /** Resistiusce gli elementi del DOM corrispondenti alle righe visibili.
   * N.B. Gli elementi restituiti sono il doppio rispetto alle righe visibili perché considera
   * il gruppo di colonne dei comandi separato da quello delle altre colonne.
   */
  private get visibleRows() {
    return this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr');
  }

  private get gridRows(): UtenteFlatModel[] {
    return this.gridPublicService.gridComp.data['data'];
  }

  private get _myUsername(): string {
    return this.master.objP_utenti.UtenteUsername;
  }

  /**
   * Default legge sia utenti attivi che non, esclude superuser
   * @param options
   */
  read(options?: any): Observable<GridProfilazioneServerResult> {
    const obs: Observable<any>[] = [];
    this.master.set_isLoading({ isLoading: true });
    obs.push(this.usersLoader.loadUsersFromServer(this._forceReload));
    if (!this.ddlLoaded$.value)
      obs.push(this.ddlLoaded$);
    return forkJoin(obs).pipe(map(obss => {
      const lingue = this.kendoColumns.find(c => c.field === 'Lingua_Cod')?.ddl?.data;
      const value = this.gridPublicService.getValue();
      const waitingForDeletion = (value?.data?.rows ?? []).filter(r => r['pending-deletion-row'])
        .map(r => r['kendoKey']);
      if (Array.isArray(obss[0])) {
        for (let u of (obss[0] as UtenteFlatModel[])) {
          u['Lingua_Des'] = lingue.find(l => l.id === u['Lingua_Cod'])?.name;
          if (waitingForDeletion.length > 0 && waitingForDeletion.includes(u.kendoKey)) {
            u['pending-deletion-row'] = true;
          }
        }
        this.kendoRows = obss[0];
      }
      this.addSpidCols();
      this.restoreUsersWithErrors();
      this.manageColumns();
      this.flagFisrtTime();
      this._forceReload = false;
      this.master.set_isLoading({ isLoading: false });
      return new GridProfilazioneServerResult(this.kendoModel, this.kendoColumns, this.kendoRows);
    }));
  }

  /**
   * Eseguita alla pressione del pulsante 'SalvaTutto'.
   *  Se sono state fatte più azioni (es. creazione e modifica), la funzione
   *  viene chiamata più volte dividendo le righe toccate per azione eseguita.
   * @param actionType azione da eseguire
   * @param items righe interessate dall'azione corrente
   */
  perform(actionType: HttpAction, items: InCellBatchSaveEventObject): Observable<any[]> {
    if (!this.datashare.userPermissions.canEditAllUsers || !items) return;
    if (actionType === HttpAction.BATCH_SAVE) {
      this.master.set_isLoading({ isLoading: true });
      return this.saveNewUsers(items.added).pipe(
        tap(saved => {
          if (saved && items.added.length) {
            this.setLastCreatedAt();
          }
        }),
        switchMap((saved) => {
          if (!saved) return of([false]);
          else return forkJoin([
            this.utentiService.deactivateUsers(items.deleted),
            this.utentiService.saveUsers(new Scrivi_Utenti(items.updated, enum_TipoOperazioneDB.Modifica))
          ]);
        }),
        tap(() => this.master.set_isLoading({ isLoading: false })),
        switchMap(res => {
          if (res.every(ok => ok)) {
            this.gridPublicService.refresh(true);
            return of([]);
          }
          this.applyRendererRules();
          return null;
        })
      );
    }
    return null;
  }

  private setLastCreatedAt() {
    const tableHasNewUsers = () => this.kendoRows.filter(u => u["isNew"]).length;
    if (tableHasNewUsers()) {
      const fArray = (this.datashare.loadUsersFilters.get('ctrls') as FormArray);
      const dateCtrl = fArray.controls.find(ctrl => ctrl.get('field').value === 'Data_Creazione');
      if (dateCtrl) {
        dateCtrl.get('value').patchValue(new Date());
      }
    }
  }

  private saveNewUsers(users: UtenteFlatModel[]): Observable<boolean> {
    if (users.length === 0) {
      return of(true);
    }
    users.forEach(u => {
      u.Lingua_Cod = u.Lingua_Cod['id'] ?? this.kendoColumns.find(c => c.field === 'Lingua_Cod').ddl.defaultValue.id;
      u.Azienda_Persona = u.Flag_Azienda_Persona === enum_Azienda_Persona.Impresa ? 'I' : 'P';
      u['isNew'] = true;
    });
    return this.dialog.warningObs(
      'newUsersProfileAssignation', 'inheritUserSettingsFromProfile',
    ).pipe(
      take(1),
      switchMap((inherit: boolean) => this.utentiService.saveUsers(
        new Scrivi_Utenti(users, enum_TipoOperazioneDB.Scrittura, inherit)
      )),
      tap(ok => this.newUsers = ok ? [] : users),
      tap(() => this.newUsers.forEach(u => {
        u.Azienda_Persona = this.getUserTypeDescr(u);
        u['Lingua_Des'] = this.kendoColumns.find(c => c.field === 'Lingua_Cod')
          .ddl.data.find(x => x.id == u.Lingua_Cod).name;
      }))
    );
  }

  private restoreUsersWithErrors() {
    this.newUsers.forEach((user, i) => {
      user.GruppiCod = user.GruppiCod.toString().split('|').map(x => +x);
      user.GruppiDes = user.GruppiDes.toString().split('|');
      user.kendoKey = '-' + (i + 1); // Assegno un id fittizio per poter gestire l'eliminazione della riga
      this.kendoRows.unshift(user);
    });
    // this.newUsers = [];
  }

  // #region Render
  override applyRendererRules(opts?: RendererGridEvent): void {
    let visibleRows = this.visibleRows;
    let rows = this.gridRows;
    // if (!this.hideSuperUser) { // SuperUser is always hidden
    //   this.hideRemoveBtn(this.renderer, visibleRows, rows);
    // }
    if (!this.datashare.userPermissions.canEditAllUsers) {
      this.removeCheckboxToOthers(this.renderer, visibleRows, rows);
    }
    this.colorRows(visibleRows, rows);
    this.gridPublicService.gridComp?.columns.filter(c => c.sticky && !c.locked)
      .forEach(col => col.locked = true);
  }

  private removeCheckboxToOthers(renderer: any, domElems: any[], users: UtenteFlatModel[]) {
    for (let i = 0; i < users.length; i++) {
      let domRow = domElems[i];
      domRow.querySelector('.k-checkbox').hidden = (users[i].UserName !== this._myUsername);
    }
  }

  private colorRows(visibleRows: NodeList, rows: UtenteFlatModel[]) {
    let style = "";
    const mid = visibleRows.length / 2;
    const doubleBody = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody').length > 1;
    rows.forEach((row, i) => {
      if (row['pending-deletion-row']) {
        style = 'background-color: mistyrose';
      } else if (rows[i].Validita_Fine < new Date()) {
        style = 'background-color: #eaeaea; color: #8f8f8f !important;';
      } else if (rows[i]['isNew']) {
        style = 'color: red !important';
      } else style = "";
      for (let j = 0; j < visibleRows[i]['children'].length; j++) {
        visibleRows[i]['children'][j].setAttribute('style', style);
      }
      if (doubleBody) {
        for (let j = 0; j < visibleRows[i + mid]['children'].length; j++) {
          visibleRows[i + mid]['children'][j].setAttribute('style', style);
        }
      }
    });
  }
  // #endregion Render

  /** Change columns based on type of users present.
   * See also {@link manageColsForBusiness}
   */
  private manageColumns() {
    if (!this.bloccaEditProfiloInline) {
      this.kendoColumns.find(s => s.field === 'Tipologia_Cod').editable = true;
    }
    if (this.kendoRows.every(r => r.NumeroAccessi == 0)) {
      this.kendoColumns.find(c => c.field === 'NumeroAccessi').hidden = true;
      this.kendoColumns.find(c => c.field === 'NumeroAccessi').includeInChooser = false;
    }
    if (this.kendoRows.length > 0 && this.kendoRows.every(r => r.Flag_Azienda_Persona === enum_Azienda_Persona.Impresa)) {
      this.kendoColumns.find(c => c.field === 'Flag_Azienda_Persona').hidden = false;
      this.kendoColumns.find(s => s.field === 'Cognome').title = this.transloco.translate('RagioneSociale');
      this.kendoColumns.find(c => c.field === 'Nome').title = "---";
      this.kendoColumns.find(s => s.field === 'CodFisc').title = this.transloco.translate('PIVA');
    } else if (this.kendoRows.length > 0 && this.kendoRows.some(r => r.Flag_Azienda_Persona === enum_Azienda_Persona.Impresa)) {
      this.kendoColumns.find(c => c.field === 'Flag_Azienda_Persona').hidden = false;
      this.kendoColumns.find(s => s.field === 'Cognome').title = this.transloco.translate('Cognome') + ' / ' + this.transloco.translate('RagioneSociale');
      this.kendoColumns.find(c => c.field === 'Nome').title = this.transloco.translate("Nome");
      this.kendoColumns.find(c => c.field === 'CodFisc').title = this.transloco.translate("CodiceFiscale");
    } else {
      this.kendoColumns.find(s => s.field === 'Cognome').title = this.transloco.translate('Cognome');
      this.kendoColumns.find(c => c.field === 'Nome').title = this.transloco.translate("Nome");
      this.kendoColumns.find(c => c.field === 'CodFisc').title = this.transloco.translate("CodiceFiscale");
    }
  }

  private addSpidCols() {
    if (this.kendoRows.some(r => r['Flag_Accesso_SPID'] !== undefined)
      && !this.kendoColumns.find(c => c.field === 'Flag_Accesso_SPID')) {
      this.kendoColumns.push(new KendoGridColumn({
        field: 'Flag_Accesso_SPID',
        title: this.transloco.translate('prof.LoginSPID')
      }, { editable: true, hidden: false, boolean: new BooleanSettings() }));
      this.kendoColumns.push(new KendoGridColumn({
        field: 'CF_SPID',
        title: this.transloco.translate('prof.CodFiscSpid')
      }, { editable: true, hidden: false }));
    }
  }


  private handleEvents() {
    this.utentiService.userReloaded$.pipe(
      takeUntil(this.signal),
      filter(reloaded => !!reloaded),
      tap(() => this._forceReload = true)
    ).subscribe(() => this.gridPublicService.refresh(true));
    this.gridPublicService.changeDetected.pipe(takeUntil(this.signal))
      .subscribe((event: any) => this.handleEditableFields(event));
    this.handelFormGroup();
  }

  private handleEditableFields(event) {
    switch (event.action) {
      case 'add': // pulsante nuova riga
        this.showEssentials();
        this.datashare.isAddingNew = true;
        break;
      case 'save': // salva nuova riga
        if (!this.hasAddedUser) {
          this.hasAddedUser = true;
          this.utentiService.dialogService.baseInfo('AggiuntaUtente', 'InfoAggiuntaUtente');
        }
        this.datashare.isAddingNew = false;
        break;
      case 'save-failed':
        console.log('failed save');
        this.scrollToInvalidField(event.formGroup);
        break;
      case 'cancel':
        this.datashare.isAddingNew = false;
        break;
    }
  }

  private scrollToInvalidField(formGroup: FormGroup) {
    const invalidField = this.getFirstInvalid(formGroup);
    const idx = this.kendoColumns.filter(col => !col.hidden)
      .findIndex(col => col.field === invalidField);
    const div = this.gridPublicService.gridElRef.nativeElement.querySelector('.k-grid-content')
      .querySelector('.k-grid-add-row').children[idx];
    div.scrollIntoView();
    div.animate([
      { transform: "scale(1)" },
      { transform: "scale(1.05)" },
      { transform: "scale(1)" },
    ], {
      duration: 300,
      iterations: 2,
    });
  }

  /**
   * Imposta i dati base del nuovo utente.
   *  I permessi partono dal giorno prima della creazione per fare in modo che
   *  l'utente possa eseguire il login anche il giorno stesso in cui viene
   *  creato (date limite escluse dall'intervallo).
   * @param form
   * @private
   */
  private configNewUser(form: FormGroup) {
    let today = new Date();
    // form.get('CodFisc').patchValue('pippo');
    form.get('Data_Creazione').patchValue(today);
    today.setDate(today.getDate() - 1); // yesterday
    form.get('Validita_Inizio').patchValue(today);
    form.get('Validita_Fine').patchValue(AGRODATAFINE);
    form.get('FinestraTemporaleInizio').patchValue(AGRODATAINIZIO);
    form.get('FinestraTemporaleFine').patchValue(AGRODATAFINE);
    form.get('Flag_Azienda_Persona').patchValue(enum_Azienda_Persona.Persona);
    form.get('Azienda_Persona').patchValue(this.transloco.translate('prof.Persona'));
    form.get('kendoKey').patchValue("-" + (this.kendoRows.length + 1));
  }

  /** Determines if a specific cell can be edited in the grid.
   * @param dataItem the row that is going to be edited
   * @param col the column that is going to be edited
   * @private
   * @returns `true` if the cell cannot be edited, `false` otherwise.
   */
  private preventEditFunction(dataItem, col: ColumnComponent) {
    const isNew = dataItem['isNew'] || +dataItem.kendoKey < 0;
    const isEditingBusinessName = dataItem.Flag_Azienda_Persona === enum_Azienda_Persona.Impresa && col.field === 'Nome';
    const isOnlyEditableForNewUsers = this.newUserEditableFields.includes(col.field);
    const mustBeValorized = ['UserName', 'CodFisc'].includes(col.field) && !dataItem[col.field];
    const isEditingSelf = dataItem.UserName === this.master.objP_utenti.UtenteUsername;
    if (isEditingSelf) {
      if (!this.datashare.userPermissions.canEditSelf) {
        return true;
      }
      const preventSelfEdit = !(['Email', 'Tel', 'Password', 'Nome', 'Cognome'].includes(col.field));
      if (!preventSelfEdit) this._editingField = col.field;
      return preventSelfEdit;
    }
    if (isNew || mustBeValorized) {
      this._editingField = col.field;
      return false;
    }
    if (isEditingBusinessName || isOnlyEditableForNewUsers) {
      return true;
    }
    const prevent = !isEditingSelf && !this.datashare.userPermissions.canEditAllUsers;
    if (!prevent) this._editingField = col.field;
    return prevent;
  }

  // #region Edit
  /** Set the edit prevention function and listens to the creation of a new formGroup (start of line editing)*/
  private handelFormGroup() {
    this.preventEdit = this.preventEditFunction;
    this.gridPublicService.formGroup
      .pipe(
        takeUntil(this.signal),
        filter(fg => fg !== null)
      ).subscribe(formgroup => {
        const original: UtenteFlatModel = cloneDeep(this.kendoRows.find(r => r.UserName === formgroup.value.UserName));
        if (!original) { // Creating new user
          this.configNewUser(formgroup);
          this.addUserValidators(formgroup);
        }
        if (original && original.Email && !formgroup.get("Email").hasValidator(Validators.required)) {
          formgroup.get("Email").addValidators([Validators.required, Validators.email]);
        }
        this.handleFormGroupInlineChanges(original, formgroup);
      });
  }

  /** Handles the change over the inline form (cell editing)
   * See also {@link manageColumns}
   */
  private handleFormGroupInlineChanges(original: UtenteFlatModel, formgroup: FormGroup) {
    const isNew = original == undefined;
    let selectedProfile: number = original?.Tipologia_Cod;
    formgroup.valueChanges.pipe(takeUntil(this.signal)).subscribe(change => {
      if (change.Tipologia_Cod && selectedProfile !== change.Tipologia_Cod) {
        selectedProfile = this.handleProfileSelection(change.Tipologia_Cod, formgroup);
      }
      this.manageColsForBusiness(formgroup.value);
      this.handleNameControl(formgroup);
      this.handleEmailControl(formgroup);
      this.handleCodFiscControl(original, formgroup);
      if (!change.Password)
        change.Password = original?.Password;

      let invalid = this.getFirstInvalid(formgroup);
      while (!isNew && invalid != undefined && this._editingField != invalid) {
        formgroup.get(invalid).disable();
        invalid = this.getFirstInvalid(formgroup);
      }
    });
  }
  // #endregion Edit

  // #region single fields
  // During the inline editing, handle the values on specified fields

  private handleProfileSelection(tipologiaCod: number, form: FormGroup): number {
    this.tipologieService.hasPermissionsOrSettings(new TipologiaUtente(tipologiaCod))
      .subscribe(r => {
        const ctrl = form.get("Tipologia_Cod");
        if (!r.haPermessi) {
          this.dialog.baseError('Errore_', 'prof.ProfiloNonHaPermessiSelezionaAltro');
          ctrl.setErrors({ noPerms: true });
        } else {
          ctrl.setErrors(null);
        }
      });
    return tipologiaCod;
  }

  private handleEmailControl(formgroup: FormGroup) {
    const spidActive = formgroup.get("Flag_Accesso_SPID").value === enum_Azienda_Persona.Persona;
    const emailRequired = formgroup.get("Email").hasValidator(Validators.required);
    if (spidActive && emailRequired) {
      formgroup.get("Email").removeValidators(Validators.required);
    } else if (!spidActive && !emailRequired) {
      formgroup.get("Email").addValidators(Validators.required);
    }
  }

  private handleNameControl(formgroup: FormGroup) {
    const nameRequired = formgroup.get("Nome").hasValidator(Validators.required);
    const isPerson = formgroup.get("Flag_Azienda_Persona").value === enum_Azienda_Persona.Persona;
    if (isPerson && !nameRequired) {
      formgroup.get("Nome").addValidators(Validators.required);
      formgroup.get("Nome").enable();
    } else if (!isPerson && nameRequired) {
      formgroup.get("Nome").removeValidators(Validators.required);
      formgroup.get("Nome").patchValue("");
      formgroup.get("Nome").disable();
    }
  }

  private handleCodFiscControl(original: any, formgroup: FormGroup) {
    if (this._editingField != "CodFisc" && formgroup.value.CodFisc == undefined && original.CodFisc != undefined) {
      formgroup.get("CodFisc").patchValue(original.CodFisc);
    }
  }

  // #endregion single fields

  private showEssentials() {
    this.essentialFields.forEach(field => {
      let col = this.gridPublicService.gridComp.columnList['columns']?._results?.find(c => c.field === field);
      if (col) col.hidden = false;
    });
  }

  private flagFisrtTime() {
    const fArray = (this.datashare.loadUsersFilters.get('ctrls') as FormArray);
    const ftCtrl = fArray.controls.find(ctrl => ctrl.get('field').value === 'firstTime');
    if (ftCtrl && ftCtrl.get('value').value == 'true') {
      ftCtrl.get('value').patchValue('false');
      fArray.controls.filter(ctrl => ctrl.get('field').value !== 'firstTime')
        .forEach(ctrl => ctrl.get('value').patchValue(''));
    }
  }

  /**
   * Add validators to the new user's formgroup for various fields.
   * @param {FormGroup} formgroup - the formgroup to add validators to
   */
  private addUserValidators(formgroup: FormGroup) {
    formgroup.get('Flag_Azienda_Persona').patchValue(enum_Azienda_Persona.Persona);
    formgroup.get('UserName').addValidators(Validators.required);
    formgroup.get('UserName').addValidators(Validators.pattern(new RegExp(/^[^,]+$/)));
    formgroup.get('Nome').addValidators(Validators.required);
    formgroup.get('Cognome').addValidators(Validators.required);
    formgroup.get('UserNameCommerciale').addValidators(Validators.required);
    formgroup.get('CodFisc').addValidators(Validators.required);
    formgroup.get('Password').addValidators(Validators.required);
    formgroup.get('Tipologia_Cod').addValidators(Validators.required);
    formgroup.get('Tel').addValidators(Validators.pattern(/^([0-9]+)$/));
    formgroup.get('Email').addValidators([Validators.email, Validators.required]);
  }

  private manageColsForBusiness(formgroup: UtenteFlatModel) {
    if (formgroup.Flag_Azienda_Persona === enum_Azienda_Persona.Impresa) {
      this.kendoColumns.find(c => c.field === 'Nome').title = "---";
      this.kendoColumns.find(c => c.field === 'CodFisc').title = this.transloco.translate("prof.CodFiscPiva");
    } else {
      this.kendoColumns.find(c => c.field === 'Nome').title = this.transloco.translate("Nome");
      this.kendoColumns.find(c => c.field === 'CodFisc').title = this.transloco.translate("CodiceFiscale");
    }
  }

  private disableSuperuserRow() {
    this.gridPublicService.formGroup.GiasSubscribe((fb) => {
      if (fb !== undefined && this.isSuperuser(fb?.value)) {
        this.disableSuperUserFields(false);
        this.applyRendererRules({
          gridElRef: this.gridPublicService.gridElRef,
          grid: this.gridPublicService.gridComp
        });
      }
    });
  }

  private disableSuperUserFields(editable: boolean) {
    this.kendoColumns.find(s => s.field === 'Azienda_Persona').editable = editable;
    this.kendoColumns.find(s => s.field === 'Cognome').editable = editable;
    this.kendoColumns.find(s => s.field === 'Nome').editable = editable;
    this.kendoColumns.find(s => s.field === 'UserNameCommerciale').editable = editable;
    this.kendoColumns.find(s => s.field === 'GruppiDes').editable = editable;
    this.kendoColumns.find(s => s.field === 'Validita_Inizio').editable = editable;
    this.kendoColumns.find(s => s.field === 'Validita_Fine').editable = editable;
  }

  // #region Grid config
  /** Attiva checkbox selezione, creazione nuovo elemento, autoresize */
  private handleCustomizations(): void {
    this.selectable.shouldShowCheckbox = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = true;
    this.selectable.columnSettings.showSelectAll = true;

    this.columnMenu.kendoGridColumnChooser = true;
    this.views.enabled = true;
    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = this.datashare.userPermissions.canEditAllUsers;
    this.toolbar.resetChanges = true;
    this.gridPublicService.resetChanges$.GiasSubscribe(() => this.hasAddedUser = false);
    this.toolbar.SaveBtn = true;
    this.toolbar.title_save_button = this.transloco.translate("SalvaTutto");
    this.generalSettings.performOnEdit = false;

    this.gridIsEditable = true;
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: this.datashare.userPermissions.canEditAllUsers,
    });
  }

  private handleDropdowns() {
    const ddl = new Array<Observable<any>>();
    ddl.push(this.configureLanguageDDL());
    ddl.push(this.configureProfilesDDL());
    ddl.push(this.configureFlagIpDDL());
    ddl.push(this.configureGroupsMDDL());
    forkJoin(ddl).pipe(take(1)).subscribe(() => {
      this.ddlLoaded$.next(true);
      this.ddlLoaded$.complete();
    });
  }

  private configureLanguageDDL() {
    const col = this.kendoColumns.find(s => s.field === 'Lingua_Cod');
    return this.utentiService.lingue.leggiLingue().pipe(map((lingue: BaseCodeDescr[]) => {
      let data = lingue.map(l => new DropdownListItem(l.codice, l.descrizione));
      col.ddl = new DropdownListWithForm('id', 'Lingua_Cod', 'name', data);
      col.ddl.descriptionField = 'Lingua_Des';
      col.ddl.defaultValue = col.ddl.data[0];
      return;
    }));
  }

  private configureFlagIpDDL() {
    const col = this.kendoColumns.find(s => s.field === 'Flag_Azienda_Persona');
    let data = [
      new DropdownListItem(enum_Azienda_Persona.Persona, this.transloco.translate('Persona')),
      new DropdownListItem(enum_Azienda_Persona.Impresa, this.transloco.translate('Azienda')),
    ];
    col.ddl = new DropdownListWithForm('id', 'Flag_Azienda_Persona', 'name', data);
    col.ddl.descriptionField = 'Azienda_Persona';
    return of(true);
  }

  private configureProfilesDDL() {
    let col = this.kendoColumns.find(s => s.field === 'Tipologia_Cod');
    return this.tipologieService.readTipologie(true).pipe(map((res: TipologiaUtente[]) => {
      let data = res.map(tipo => new DropdownListItem(tipo.codice, tipo.descrizione));
      col.ddl = new DropdownListWithForm('codice', 'Tipologia_Cod', 'descrizione', data);
      col.ddl.descriptionField = 'Tipologia_Des';
      return;
    }));
  }

  private configureGroupsMDDL() {
    let col = this.kendoColumns.find(s => s.field === 'GruppiCod');
    return this.gruppiService.readGruppi().pipe(map(res => {
      let data = res.map(gruppo => new DropdownListItem(gruppo.codice, gruppo.descrizione));
      col.ddl = new DropdownListWithForm('codice', 'GruppiCod', 'descrizione', data);
      col.ddl.descriptionField = 'GruppiDes';
      return;
    }));
  }
  // #endregion Grid config

  // #region Utility
  private isSuperuser(dataItem: UtenteFlatModel): boolean {
    return isSuperUser(dataItem as any);
  }

  private getFirstInvalid(formGroup: FormGroup): string {
    for (let ctrlName in formGroup.controls) {
      if (formGroup.controls[ctrlName].invalid)
        return ctrlName;
    }
  };

  private getUserTypeDescr(userRow): string {
    const flagCol = this.kendoColumns.find(s => s.field === 'Flag_Azienda_Persona');
    if (userRow.Flag_Azienda_Persona == 'I' || userRow.Flag_Azienda_Persona == enum_Azienda_Persona.Impresa) {
      return flagCol.ddl.data.find(x => x.id == enum_Azienda_Persona.Impresa).name;
    } else if (userRow.Flag_Azienda_Persona == 'P' || userRow.Flag_Azienda_Persona == enum_Azienda_Persona.Persona) {
      return flagCol.ddl.data.find(x => x.id == enum_Azienda_Persona.Persona).name;
    }
  }

  // #endregion Utility

}

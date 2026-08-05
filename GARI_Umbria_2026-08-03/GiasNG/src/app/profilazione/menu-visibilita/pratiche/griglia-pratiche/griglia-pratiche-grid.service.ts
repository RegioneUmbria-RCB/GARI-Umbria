import { Injectable, Injector } from '@angular/core';
import { CELL_TYPES } from 'gias-ui-kit';
import { GiasMessageService } from 'app/Service/gias-message.service';
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  CustomColumnSettings,
  EditingMode,
  HttpAction,
  KendoGridColumn,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  ToolbarSettings,
} from 'gias-kendo-grid';
import { Observable, of, Subject, takeUntil, filter, tap, switchMap, catchError } from 'rxjs';
import { ConfigurazionePraticheUtente, PraticaSelezionabile } from 'app/profilazione/models/pratiche-visibilita.model';
import { PraticheVisibilitaService } from 'app/profilazione/services/pratiche-visibilita.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ConsideraValiditaCellComponent } from '../considera-validita-cell/considera-validita-cell.component';
import { FormGroup } from '@angular/forms';

export class PraticheServerResult extends KendoServerResult {
  constructor(model: any, columns: KendoGridColumn[], rows: any[]) {
    super(model, columns, rows);
  }
}

@Injectable()
export class GrigliaPraticheGridService extends AbstractGridConfigService<PraticheServerResult> {
  editingMode = EditingMode.IN_CELL_BATCH;
  loader = LoaderType.SERVICE;
  rowId = 'Servizio_Cod';
  gridId = 'GrigliaPraticheSelezionabili';

  public readonly savedSuccessfully$ = new Subject<void>();
  public readonly configRefreshed$ = new Subject<ConfigurazionePraticheUtente>();

  private rows: PraticaSelezionabile[] = [];
  private username: string;
  private operatoreOR: boolean;
  private filtroPraticheAttivo: boolean;

  private readonly kendoModel = {
    Servizio_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    ServizioDescrizione: new ModelEntry(CELL_TYPES.STRING),
    Selected: new ModelEntry(CELL_TYPES.BOOLEAN),
    ConsideraValiditaTemporale: new ModelEntry(CELL_TYPES.CUSTOM),
    DataValiditaInizio: new ModelEntry(CELL_TYPES.STRING),
    DataValiditaFine: new ModelEntry(CELL_TYPES.STRING),
    IsValida: new ModelEntry(CELL_TYPES.BOOLEAN),
  };

  private get kendoColumns(): KendoGridColumn[] {
    return [
      new KendoGridColumn(
        { field: 'Servizio_Cod', title: this.transloco.translate('prof.ColonnaCodice') },
        { editable: false, width: 120 }
      ),
      new KendoGridColumn(
        { field: 'ServizioDescrizione', title: this.transloco.translate('prof.ColonnaDescrizione') },
        { editable: false }
      ),
      new KendoGridColumn(
        { field: 'ConsideraValiditaTemporale', title: this.transloco.translate('prof.ColonnaConsideraValidita') },
        { editable: true, width: 210, component: ConsideraValiditaCellComponent }
      ),
      new KendoGridColumn(
        { field: 'DataValiditaInizio', title: this.transloco.translate('prof.ColonnaValiditaInizio') },
        { editable: false, width: 130 }
      ),
      new KendoGridColumn(
        { field: 'DataValiditaFine', title: this.transloco.translate('prof.ColonnaValiditaFine') },
        { editable: false, width: 130 }
      ),
      new KendoGridColumn(
        { field: 'IsValida', title: this.transloco.translate('prof.ColonnaIsValida') },
        { editable: false, width: 100, boolean: { defaultValue: false } }
      ),
    ];
  }

  constructor(
    injector: Injector,
    private praticheService: PraticheVisibilitaService,
    private dialog: GiasDialogService,
    private messageService: GiasMessageService,
  ) {
    super(injector);
    this.handleCustomizations();
    this.handleEvents();
  }

  setConfig(username: string, operatoreOR: boolean, filtroPraticheAttivo: boolean): void {
    this.username = username;
    this.operatoreOR = operatoreOR;
    this.filtroPraticheAttivo = filtroPraticheAttivo;
  }

  setRows(rows: PraticaSelezionabile[]): void {
    this.rows = rows;
  }

  getRows(): PraticaSelezionabile[] {
    return this.rows;
  }

  toggleSelectAll(selectAll: boolean): void {
    this.rows.forEach(r => r.Selected = selectAll);
  }

  toggleRow(servizio_cod: number, selected: boolean): void {
    const row = this.rows.find(r => r.Servizio_Cod === servizio_cod);
    if (row) {
      row.Selected = selected;
    }
  }

  read(): Observable<PraticheServerResult> {
    let rows = this.rows.map(r => ({
      Servizio_Cod: r.Servizio_Cod,
      ServizioDescrizione: r.ServizioDescrizione,
      Selected: r.Selected,
      ConsideraValiditaTemporale: r.ConsideraValiditaTemporale,
      DataValiditaInizio: r.DataValiditaInizio.toLocaleDateString(),
      DataValiditaFine: r.DataValiditaFine.toLocaleDateString(),
      IsValida: r.IsValida,
    })).sort((a, b) => a.Selected === b.Selected ? (a.Servizio_Cod - b.Servizio_Cod) : a.Selected ? -1 : 1);
    return of(new PraticheServerResult(this.kendoModel, this.kendoColumns, rows));
  }

  perform(actionType: HttpAction, _items: { added: any[]; updated: any[]; deleted: any[] }): Observable<any> {
    if (actionType !== HttpAction.BATCH_SAVE) return of([]);

    const toSave = this.rows.filter((p) => p.Selected);
    const isRestrictive = !this.operatoreOR && this.filtroPraticheAttivo && toSave.length === 0;
    toSave.forEach(p => {
      p.ConsideraValiditaTemporale = p.ConsideraValiditaTemporale == true;
    });
    const saveObs = this.praticheService
      .eseguiSalvataggio(this.username, toSave, this.operatoreOR, this.filtroPraticheAttivo)
      .pipe(
        tap((ok) => {
          if (ok) {
            this.messageService.successMessage('prof.PraticheSalvateCorrettamente', false, true);
            this.savedSuccessfully$.next();
            this.gridPublicService.refresh(true);
          } else {
            this.messageService.errorMessage('prof.ErroreSalvataggioPratiche', false, true);
          }
        }),
        switchMap((ok) => ok
          ? this.praticheService.caricaConfigurazioneUtente(this.username).pipe(
            tap((config) => this.configRefreshed$.next(config)),
          )
          : of(null)
        ),
        catchError(() => {
          this.messageService.errorMessage('prof.ErroreSalvataggioPratiche', false, true);
          return of(null);
        }),
      );

    if (isRestrictive) {
      return (this.dialog.warningObs('prof.ConfigRestrittivaTitolo', 'prof.ConfigRestrittivaTesto') as Observable<boolean>).pipe(
        switchMap((confirmed) => (confirmed ? saveObs : of([]))),
      );
    }

    return saveObs;
  }

  private handleCustomizations(): void {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false,
    });
    this.selectable.shouldShowCheckbox = false;
    this.selectable.selectable.enabled = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.customColumn = new CustomColumnSettings({
      showColumn: true,
      useCustomColumnHeaderTemplate: true,
      useCustomColumnCellTemplate: true,
      width: 60,
      showBtn: false,
    });
    this.toolbar = new ToolbarSettings();
    this.toolbar.resetChanges = true;
    //this.gridPublicService.resetChanges$.GiasSubscribe(() => this.hasAddedUser = false);
    this.toolbar.SaveBtn = true;
    this.toolbar.title_save_button = this.transloco.translate("SalvaTutto");
    this.pagination.gridState.take = 999;
  }

  private handleEvents() {
    this.gridPublicService.formGroup
      .pipe(
        takeUntil(this.signal),
        filter(fg => fg !== null),
        switchMap((formgroup: FormGroup) => formgroup.valueChanges),
      ).subscribe((item) => {
        const row = this.rows.find(r => r.Servizio_Cod === item.Servizio_Cod);
        if (row) {
          row.ConsideraValiditaTemporale = item.ConsideraValiditaTemporale;
        }
      });
  }
}

import { Injectable, Injector } from '@angular/core';
import { catchError, combineLatest, debounceTime, map, Observable, of, share, startWith, switchMap, withLatestFrom } from 'rxjs';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, NumericSettings } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';
import { Avversita, RilieviAvversitaService } from './rilievi-avversita.service';
import { tap } from 'rxjs';
import { AgendaClient, Disciplinare, MisuraPerAvversitaAnagrafica, MisuraPerAvversitaAnagrafica_In, RispostaStandard, RispostaStandard_1OfListaMisurePerAvversitaAnagrafica } from 'app/Service/api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';

export class RilieviAvversitaResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class RilieviAvversitaGridModel extends KendoGridModel {
  CodiceMisura: ModelEntry;
  CodiceAnagrafica: ModelEntry;
  Descrizione: ModelEntry;
  valoreAnagrafica: ModelEntry;
  DPI_FlagPrivatoPubblico: ModelEntry;
  DPI_COD: ModelEntry;
  modificabile: ModelEntry;
  cancellabile: ModelEntry;
}

@Injectable()
export class RilieviAvversitaGridConfig extends AbstractGridConfigService<RilieviAvversitaResult> {
  gridId = 'RilieviAvversitaGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_CELL;
  rowId = 'CodiceAnagrafica';
  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'Descrizione', title: this.transloco.translate('RilieviAvversitaDescrizione') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 200
      }
    ),
    new KendoGridColumn(
      { field: 'valoreAnagrafica', title: this.transloco.translate('RilieviAvversitaValoreAnagrafica') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 50,
        numeric: new NumericSettings({ format: '#', decimals: 0, })
      }
    )
  ];

  RilieviAvversitaGridModel: RilieviAvversitaGridModel = {
    CodiceMisura: new ModelEntry(CELL_TYPES.NUMBER),
    CodiceAnagrafica: new ModelEntry(CELL_TYPES.NUMBER),
    Descrizione: new ModelEntry(CELL_TYPES.STRING),
    valoreAnagrafica: new ModelEntry(CELL_TYPES.NUMBER),
    DPI_FlagPrivatoPubblico: new ModelEntry(CELL_TYPES.NUMBER),
    DPI_COD: new ModelEntry(CELL_TYPES.NUMBER),
    modificabile: new ModelEntry(CELL_TYPES.BOOLEAN),
    cancellabile: new ModelEntry(CELL_TYPES.BOOLEAN),
  };

  private read$: Observable<RilieviAvversitaResult> | null = null;

  constructor(
    protected injector: Injector,
    protected transloco: TranslocoService,
    private rilieviAvversitaService: RilieviAvversitaService,
    private agendaClient: AgendaClient,
    private giasMessageService: GiasMessageService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings(true);
    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: true,
      onDisableRemoveBtn: (data: MisuraPerAvversitaAnagrafica) => !data.cancellabile,
      onDisableEditBtn: (data: MisuraPerAvversitaAnagrafica) => !data.modificabile
    });

    this.cmdColumn['widthSet'] = 25;
    this.behavior.excelSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.generalSettings.performOnEdit = true;
  }

  public read(): Observable<RilieviAvversitaResult> {
    // Define only the first time
    if (this.read$ != null) {
      return this.read$;
    }

    this.read$ = combineLatest([
      this.rilieviAvversitaService.disciplinare$,
      this.rilieviAvversitaService.avversita$,
      this.rilieviAvversitaService.reload$.pipe(startWith(null))
    ])
      .pipe(
        tap(() => this.isLoading(true)),
        debounceTime(100),
        switchMap(([disciplinare, avversita]) => this.processAvversita(disciplinare, avversita)),
        switchMap(data => of(new RilieviAvversitaResult(data.RispostaStringa.ListaMisure, this.columns, this.RilieviAvversitaGridModel))),
        tap(() => this.isLoading(false)),
        share()
      );

    return this.read$;
  }

  public perform(actionType: HttpAction, rows: Array<MisuraPerAvversitaAnagrafica>): Observable<KendoGridRow[]> {
    of(rows)
      .pipe(
        withLatestFrom(this.rilieviAvversitaService.disciplinare$, this.rilieviAvversitaService.avversita$),
        map(([rows, disciplinare, avversita]) => this.fillRowsMissingValues(rows, disciplinare, avversita, actionType)),
        switchMap(payload => this.agendaClient.agendaMisurePerAvversitaAnagrafiche(payload)),
        catchError(res => of({ Errore: res.toString() } as RispostaStandard))
      )
      .subscribe({
        next: res => {
          this.rilieviAvversitaService.nextReload();
          if (res?.RispostaStringa != null && res.RispostaStringa != "") {
            this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
            return;
          }

          this.giasMessageService.errorMessage(res.Errore, false, false);
        },
        error: error => {
          this.rilieviAvversitaService.nextReload();
          this.giasMessageService.errorMessage(error.Errore, false);
        }
      });

    return of(rows);
  }

  private processAvversita(disciplinare: Disciplinare, avversita: Avversita): Observable<RispostaStandard_1OfListaMisurePerAvversitaAnagrafica> {
    if (disciplinare == null || avversita?.MxAV_Cod == null) {
      this.toggleEnable(false);
      return of({ RispostaStringa: { ListaMisure: [] } } as RispostaStandard_1OfListaMisurePerAvversitaAnagrafica);
    }

    this.toggleEnable(true);
    return this.agendaClient.agendaLeggiMisurePerAvversitaAnagrafiche({ codice: +avversita.MxAV_Cod });
  }

  private fillRowsMissingValues(rows: MisuraPerAvversitaAnagrafica[], disciplinare: Disciplinare, avversita: Avversita, actionType: HttpAction): MisuraPerAvversitaAnagrafica_In {
    for (const row of rows) {
      row.CodiceMisura ??= +avversita?.MxAV_Cod;
      row.CodiceAnagrafica ??= 0;
      row.cancellabile ??= true;
      row.modificabile ??= true;
      row.DPI_FlagPrivatoPubblico ??= disciplinare?.disciplinarePubblicoPrivato ?? 0;
      row.DPI_COD ??= +(disciplinare?.codice?.split("/")[0] ?? '0')
    }

    let payload: MisuraPerAvversitaAnagrafica_In;
    switch (actionType) {
      case HttpAction.CREATE:
        payload = { MisureInsert: rows };
        break;
      case HttpAction.UPDATE:
        payload = { MisureUpdate: rows };
        break;
      case HttpAction.REMOVE:
        payload = { MisureDelete: rows };
        break;
    }

    return payload;
  }

  private toggleEnable(enable: boolean): void {
    this.gridIsEditable = enable;
    this.toolbar.newItem = enable;
    this.cmdColumn.removeBtn = enable;
  }
}
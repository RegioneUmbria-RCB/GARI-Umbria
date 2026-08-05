import {Injectable, Injector} from "@angular/core";
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {
  DateSettings,
  DateTimeSettings,
  DropdownListWithForm,
  EditingMode,
  KendoGridColumn,
  KendoServerResult,
  LoaderType,
  NumericSettings,
  RendererGridEvent
} from 'gias-kendo-grid';
import {filter, map, Observable, of, take, takeUntil, tap} from "rxjs";
import {
  LetturaContatoriGridItem,
  LetturaContatoriGridItemForm,
  LetturaContatoriGridItemModel,
  LetturaContatoriModel
} from "../../model/LetturaContatori.model";
import {CommandsColumnSettings, ToolbarSettings} from 'gias-kendo-grid';
import {BaseCodeDescr as IBaseCodeDescr} from "../../../Service/api.service";
import {ObjParametriAgendaService} from "../../../Service/obj-parametri-agenda.service";
import {FormGroup, Validators} from "@angular/forms";
import {InData} from "../../model/inData.model";
import {ContatoriAcquaClient, RispostaStandard} from "../../../Service/net-core6-api.service";
import {GiasMessageService} from "../../../Service/gias-message.service";
import {LettureContatoriFilters} from "../../pages/letture-contatori/letture-contatori.component";
import {AGRODATAINIZIO} from "../../../Model/CostantiPersonalizzate";
import {GiasDialogService} from "../../../Service/gias-dialog.service";
import {IntervalloTemporale} from "../../../Model/anagrafiche/IntervalloTemporale";
import { DateUtility } from 'gias-ui-kit';
import {WaterCounter} from "../../model/WaterCounter";
import {GridDataResult} from "@progress/kendo-angular-grid";

class KendoServerResultImpl extends KendoServerResult {
  constructor(model, cols, rows) {
    super(model, cols, rows);
  }
}

@Injectable()
export class LettureContatoriGridConfigService extends AbstractGridConfigService<KendoServerResult> {
  editingMode = EditingMode.IN_LINE;
  loader = LoaderType.SERVICE;
  rowId = "Id_Lettura";
  gridId = "LettureContatoriAziendaliGrid";

  private _model = new LetturaContatoriGridItemModel();
  private _rows: LetturaContatoriModel[] = [];
  private _columns: KendoGridColumn[] = [
    //new KendoGridColumn({ field: 'Id_Lettura', title: this.transloco.translate('ID') }, { editable: true, hidden: true, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'DataLettura', title: this.transloco.translate('DataOraLettura') }, { editable: true, date: new DateTimeSettings(), validators: [Validators.required] }),
    new KendoGridColumn({ field: 'ValoreMC', title: this.transloco.translate('ValoreRilevatoMC') }, { editable: true, numeric: new NumericSettings(), validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Id_Contatore', title: this.transloco.translate('Contatore') }, { editable: true, validators: [Validators.required] }),
    new KendoGridColumn({field: 'Modello', title: this.transloco.translate('Modello')}, {
      editable: false,
      hidden: true
    }),
    new KendoGridColumn({field: 'Targa', title: this.transloco.translate('Targa')}, {editable: false, hidden: true}),
    //new KendoGridColumn({ field: 'piva', title: this.transloco.translate('PIVA') }, { editable: false, validators: [Validators.required] }),
    new KendoGridColumn({ field: 'Validita_Inizio', title: this.transloco.translate('ValiditaInizioContatore') }, { editable: false, hidden:true, date: new DateSettings()}),
    new KendoGridColumn({ field: 'Validita_Fine', title: this.transloco.translate('ValiditaFineContatore') }, { editable: false, hidden:true, date: new DateSettings() }),
  ];

  private _counters: WaterCounter[] = [];
  private _filters = new LettureContatoriFilters(
    new IntervalloTemporale(
      new Date(DateUtility.getCurrentYear(), DateUtility.months.JANUARY, 1),
      new Date(DateUtility.getCurrentYear(), DateUtility.months.DECEMBER, 31)
    )
  );

  constructor(
    injector: Injector,
    private message: GiasMessageService,
    private dialog: GiasDialogService,
    private agenda: ObjParametriAgendaService,
    private client: ContatoriAcquaClient
  ) {
    super(injector);
    this.handleCustomizations();
    this.configureDdls();
    this.handleEvents();
  }

  private get piva(): string {
    return this.agenda.getObjParamValue().Piva;
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const gridResult = opts.grid.data as GridDataResult;
    gridResult.data.forEach(row => {
      const counter = this._counters.find(x => x.codice === row.Id_Contatore);
      if (counter) {
        row.Modello = counter.modello;
        row.Targa = counter.targa;
      }
    });
  }

  read(options?: any): Observable<KendoServerResultImpl> {
    if (options) {
      this._filters = options;
      return of(null);
    }
    return this.client.contatoriAcquaGetLetturaContatori(new InData.DSS.Letture_IN(
      this.piva, this._filters?.dateInterval
    )).pipe(
      map(r => JSON.parse(r.RispostaStringa)),
      tap((items: LetturaContatoriGridItem[]) => items.forEach(x => x.piva = this.piva)),
      tap(items => this._rows = items.map(x => new LetturaContatoriModel().fromFlatGridItem(x))),
      map(items => new KendoServerResultImpl(this._model, this._columns, items))
    );
  }

  perform(actionType: HttpAction, items: LetturaContatoriGridItem, oldRow?: any): Observable<any> {
    if ((actionType === HttpAction.CREATE || actionType === HttpAction.UPDATE)
      && !this.isCounterValidAt(items.Id_Contatore, items.DataLettura)
    ) {
      this.dialog.baseError("ErroreSalvataggio", "ContatoreInvalidoDataSelezionata");
      return of(null);
    }
    switch (actionType) {
      case HttpAction.CREATE:
        return this.addReading(items);
      case HttpAction.UPDATE:
        return this.editReading(items);
      case HttpAction.REMOVE:
        return this.deleteReading(items);
    }
  }

  private handleCustomizations() {
    this.cmdColumn = new CommandsColumnSettings({ editBtn: true, infoBtn: false, removeBtn: true });
    this.toolbar = new ToolbarSettings(true);
  }

  private configureDdls() {
    const col = this._columns.find(s => s.field === 'Id_Contatore');
    col.ddl = new DropdownListWithForm('codice', 'Id_Contatore', 'descrizione', []);
    col.ddl.descriptionField = 'Descrizione';
    col.ddl.loadOnEdit = true;
    col.ddl.loadFunction = this.loadCounter.bind(this);
    col.ddl.defaultOpen = true;
    col.ddl.valuePrimitive = true;
    this.client.contatoriAcquaGetContatori(new InData.DSS.LeggiContatori_IN(this.piva, AGRODATAINIZIO))
      .pipe(map(r => JSON.parse(r.RispostaStringa)))
      .subscribe(counters => this._counters = counters);
  };

  private getCountersAt(date: Date) {
    return this._counters.filter(x => date >= new Date(x.validita.inizio) && date <= new Date(x.validita.fine));
  }

  private isCounterValidAt(id: number, date: Date) {
    const selected = this._counters.find(x => x.codice === id);
    const counters = this.getCountersAt(date);
    return counters.includes(selected);
  }

  private loadCounter(row: LetturaContatoriGridItem): Observable<IBaseCodeDescr[]> {
    if (!row.DataLettura) {
      return of([]);
    } else return of(this.getCountersAt(row.DataLettura));
  }

  private handleEvents() {
    this.setDefaultsOnNewItem();
    this.fixDateValorizationOnEdit();
  }

  private setDefaultsOnNewItem() {
    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      filter(fg => fg != null && !this._rows.find(r => r[this.rowId] === fg.value[this.rowId])),
    ).subscribe((fg: FormGroup<LetturaContatoriGridItemForm>) => fg.controls.piva.patchValue(this.piva));
  }

  private fixDateValorizationOnEdit() {
    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      filter(fg => fg != null),
    ).subscribe((fg: FormGroup<LetturaContatoriGridItemForm>) => {
      const item = this._rows.find(r => r[this.rowId] === fg.value[this.rowId]);
      if (item && !fg.value.DataLettura) {
        fg.controls.DataLettura.patchValue(new Date(item.dataLettura));
      }
    });
  }

  private addReading(row: LetturaContatoriGridItem): Observable<any> {
    return this.client.contatoriAcquaWriteLetturaContatori(new InData.DSS.ScriviLetturaContatore(
      0, row.piva, row.Id_Contatore, row.ValoreMC, row.DataLettura, false
    )).pipe(
      take(1),
      tap((response: RispostaStandard) => {
        if (response.RispostaOK) {
          this.message.successMessage(this.transloco.translate("SalvataggioAvvenutoConSuccesso"));
        } else {
          this.message.errorMessage(this.transloco.translate("ErroreSalvataggio"));
        }
      })
    );
  }

  private editReading(row: LetturaContatoriGridItem) {
    return this.client.contatoriAcquaWriteLetturaContatori(new InData.DSS.ScriviLetturaContatore(
      row.Id_Lettura, row.piva, row.Id_Contatore, row.ValoreMC, row.DataLettura, false
    )).pipe(
      take(1),
      tap((response: RispostaStandard) => {
        if (response.RispostaOK) {
          this.message.successMessage(this.transloco.translate("ModificaAvvenutaConSuccesso"));
        } else {
          this.message.errorMessage(this.transloco.translate("ErroreSalvataggio"));
        }
      })
    );
  }

  private deleteReading(row: LetturaContatoriGridItem) {
    return this.client.contatoriAcquaWriteLetturaContatori(new InData.DSS.ScriviLetturaContatore(
      row.Id_Lettura, row.piva, row.Id_Contatore, row.ValoreMC, row.DataLettura, true
    )).pipe(
      take(1),
      tap((response: RispostaStandard) => {
        if (response.RispostaOK) {
          this.message.successMessage(this.transloco.translate("CancellazioneAvvenuta"));
        } else {
          this.message.errorMessage(this.transloco.translate("ErroreCancellazione"));
        }
      })
    );
  }
}

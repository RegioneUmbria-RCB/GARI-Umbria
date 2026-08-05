import {Injectable, Injector} from '@angular/core';
import {FormArray, FormGroup} from '@angular/forms';
import {
    CommandsColumnSettings,
    ExcelSettings,
    PaginationSettings
} from 'gias-kendo-grid';
import {
    EditingMode,
    KendoGridColumn,
    KendoServerResult,
    LoaderType
} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {filter, map, Observable, of, switchMap, takeUntil, tap} from 'rxjs';
import {GridRaccoltaModel, GridRaccoltaObject} from '../../raccolta.model';
import {
  GridImpiantoSelezionatoModel
} from '../../../../../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {QdCRaccoltaService} from '../../../../../../service/prodotti/raccolta.service';
import {QdCService} from '../../../../../../service/qdc.service';
import {DettaglioRaccolta} from 'app/Model/attivita/dettagli/DettaglioRaccolta';
import {RaccoltaDataShareService} from '../../data-share.service';
import {QuantitaSuImpianto} from 'app/Model/attivita/dettagli/QuantitaSuImpianto';
import {enum_Generazione_Lotto_Raccolta} from '../../opzioni-raccolta/opzioni-raccolta.model';
import {GiasMessageService} from "../../../../../../../../Service/gias-message.service";
import {CellCloseEvent} from "@progress/kendo-angular-grid";


export class GridRipManualeServerResult extends KendoServerResult {
  constructor(model, columns, rows) {
    super(model, columns, rows);
  }
}

@Injectable({providedIn: 'root'})
export class GridRipManualeConfigService extends AbstractGridConfigService<GridRipManualeServerResult> {
  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'APPEZ_COD';
  gridId = 'gridRipMan';

  private _impianti: FormArray;
  //private floatRegex = /[+-]?([0-9]*[.])?[0-9]+/;

  private kendoRows = [];
  private kendoModel = new GridRaccoltaModel();
  private kendoColumns: KendoGridColumn[] = [];

  constructor(
    injector: Injector,
    private qdcservice: QdCService,
    private raccoltaService: QdCRaccoltaService,
    private dataShare: RaccoltaDataShareService,
    private message: GiasMessageService,
  ) {
    super(injector);
    this.handleCustomizatons();
    this.handleEvents();
  }

  // GETTERS & SHORT UTILS

  private get carichiAttivi(): boolean {
    return this.raccoltaService.Raccolta_Con_Carico_Magazzino;
  }
  private get lottiAttivi(): boolean {
    return this.raccoltaService.lottiAttivi;
  }

  // GRID LOAD FUNCTIONS

  read(options?: any): Observable<GridRipManualeServerResult> {
    this.kendoRows = this.loadAppezzamenti();
    this.kendoColumns = this.dataShare.gridColumns;
    this.enableColumns();
    this.onProductSelected();
    this.updateLotto(this.kendoRows[0], true);
    return of(new GridRipManualeServerResult(
      this.kendoModel, this.kendoColumns, this.kendoRows
    ));
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return null;
  }

  private refresh() {
    const grid = new GridRipManualeServerResult(this.kendoModel, this.kendoColumns, this.kendoRows);
    this.gridPublicService.refresh(true, grid);
  }

  // GRID MANAGEMENT

  private handleCustomizatons() {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false,
    });
    this.groups.groupable.enabled = false;
    this.behavior.excelSettings = new ExcelSettings({enabled: false});
    this.views.enabled = false;
    this.pagination = this.handlePagination();

    this.raccoltaService.loadEndEvent.pipe(takeUntil(this.signal))
      .subscribe(end => this.afterLoadComplete(end));
  }

  private afterLoadComplete(end): void {
    if (!end) return;
    if (!this.kendoColumns.length) {
      this.kendoColumns = this.dataShare.gridColumns;
    }

    const lottoCol = this.kendoColumns.find(s => s.field === 'Lotto');
    const progCodCol = this.kendoColumns.find(s => s.field === 'Progetto_Cod');
    const progDesCol = this.kendoColumns.find(s => s.field === 'Progetto_Des');

    progDesCol.hidden = !this.lottiAttivi || !this.raccoltaService.isLottoFromProg;
    progCodCol.hidden = !this.lottiAttivi || !this.raccoltaService.isLottoFromProg;
    lottoCol.hidden = !this.lottiAttivi;
    lottoCol.editable = this.raccoltaService.isLottoManuale || this.raccoltaService.isLottoFromProg;
  }

  private handlePagination() {
    const result = new PaginationSettings();
    result.gridState = {sort: [], skip: 0, take: 10};
    result.pageable = true;
    result.navigable = true;
    return result;
  }

  private enableColumns() {
    this.kendoColumns.forEach(col => col.editable = false);
    this.kendoColumns.find(s => s.field === 'Qta').editable = this.qdcservice.abilitaGrid;
    this.kendoColumns.find(s => s.field === 'Lotto').editable = this.raccoltaService.isLottoManuale;
  }

  // EVENTS

  private handleEvents() {
    this.impiantiSelectionChange();
    this.handleLottoGenModeChange();
    this.handleInlineEdit();
    this.onStorageSelected();
  }

  private handleInlineEdit() {
    let orig;
    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      filter(fg => !!fg),
      tap(fg => orig = fg.value),
      switchMap(fg => fg.valueChanges)
    ).subscribe(val => this.raccoltaService.startEditing());

    this.onCellClose = (event: CellCloseEvent) => {
      this.calculateTotal();
      this.updateQuantitaSuImpianti();
      this.updateLotto(event.formGroup.value, event.formGroup.value.Lotto !== orig.Lotto);
      this.raccoltaService.stopEditing();
    };
    this.preventEdit = (dataItem, column) => {
      if (column.field === "Lotto" && dataItem.Qta <= 0) {
        this.message.warningMessage('ImpossibileAssegnareLottoQtaNulla', false, true);
        return true;
      }
      return false;
    };
  }

  private onStorageSelected() {
    this.dataShare.formRipartizione.pipe(
      takeUntil(this.signal),
      filter(form => !!form),
      switchMap(form => form.get("Magazzino").valueChanges)
    ).subscribe(storage => {
      // Visibilità lotto e codice esercizio gestite in this.enableLottoEdit
      // e nella funzione handleVisibility del component
      this.kendoRows.forEach(r => this.loadQtaLottoFromSection(r));
    });
  }

  private handleLottoGenModeChange() {
    const options = this.raccoltaService.sezione_prodotto
      .get("Opzioni_Raccolta") as FormGroup;
    options.get("GenerazioneLotto").valueChanges.pipe(takeUntil(this.signal))
      .subscribe(GenMode => {
        this.enableLottoEdit(GenMode);
        this.valorizeLotto(GenMode);
      });
  }

  private enableLottoEdit(genType: enum_Generazione_Lotto_Raccolta): void {
    if (!this.lottiAttivi) {
      this.kendoColumns.find(s => s.field === 'Lotto').hidden = true;
      this.hideEsercizioCol(true);
      return;
    }

    if (genType === enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD) {
      this.kendoColumns.find(s => s.field === 'Lotto').editable = true;
      this.hideEsercizioCol(false);
      if (!this.dataShare.Magazzino || this.dataShare.Magazzino.primaryKey.codice === 0) {
        this.hideEsercizioCol(true);
      }
    } else if (genType === enum_Generazione_Lotto_Raccolta.MANUALE) {
      this.kendoColumns.find(s => s.field === 'Lotto').editable = true;
      this.hideEsercizioCol(true);
    } else if (genType === enum_Generazione_Lotto_Raccolta.DA_DATA || genType === enum_Generazione_Lotto_Raccolta.UNIVOCO) {
      this.kendoColumns.find(s => s.field === 'Lotto').editable = false;
      this.hideEsercizioCol(true);
    }
  }

  private hideEsercizioCol(show: boolean) {
    this.kendoColumns.find(s => s.field === 'Progetto_Cod').hidden = show;
    this.kendoColumns.find(s => s.field === 'Progetto_Des').hidden = show;
  }

  private valorizeLotto(GenMode: enum_Generazione_Lotto_Raccolta): void {
    if (this.raccoltaService.isLottoEditable(GenMode)) {
      let prodotto = this.dataShare.Prodotto?.value;
      if (!prodotto) return;
      this.kendoRows.forEach(r => r.Lotto = prodotto.QuantitaSuImpianti[0].Lotto);
      return;
    }
    this.kendoRows.forEach(r => r.Lotto = this.transloco.translate('GeneratoAutomaticamente'));
  }

  // FIELDS MANAGEMENT

  /** Sorts the fields based on their project code.
   * @return 1 if b should be shown after a, -1 if a should be shown after b, 0 if the order is not significant.
   */
  private compareGridImpianto(a: GridImpiantoSelezionatoModel, b: GridImpiantoSelezionatoModel): number {
    let res = a.App_Nome.localeCompare(b.App_Nome);
    if (res !== 0) return res;

    res = a.Cul_Des.localeCompare(b.Cul_Des);
    if (res !== 0) return res;

    if (a.Progetto_Cod < b.Progetto_Cod) return 1;
    if (a.Progetto_Cod > b.Progetto_Cod) return -1;
    return 0;
  }

  private impiantiSelectionChange() {
    this._impianti = <FormArray>this.qdcservice.QdCForm
      .get('Trattamento').get('Impianti').get("ImpiantiSelezionati");
    this.qdcservice.SelezioneEsercizi.pipe(
      takeUntil(this.signal),
      filter(es => !!es && !!this.dataShare.Prodotto),
      map(es => es.value)
    ).subscribe(esercizi => {
      this.removeUnselected(esercizi);
      this.addSelected(esercizi);
      this.calculateTotal();
      this.updateQuantitaSuImpianti();
      this.refresh();
    });
  }

  /* Rimuove l'esercizio dalle righe della griglia e dala lista di
   * QuantitaSuImpianti riferita al prodotto corrente.
   */
  private removeUnselected(selected) {
    let unselected = this.kendoRows.filter(row =>
      !selected.find(s => s.Progetto_Cod === row.Progetto_Cod)
    );

    let prodotto = this.dataShare.Prodotto.value as DettaglioRaccolta;
    let QsI = prodotto.QuantitaSuImpianti;

    for (let item of unselected) {
      let index = this.kendoRows.findIndex(row =>
        row.Progetto_Cod === item.Progetto_Cod
      );
      this.kendoRows.splice(index, 1);

      index = QsI.findIndex(q =>
        q.esercizioCDC.esercizio.codice === item.Progetto_Cod
      );
      QsI.splice(index, 1);
    }

    this.dataShare.Prodotto.patchValue(prodotto);
    this.raccoltaService.updateFastHarvestTemplate([], unselected);
  }

  /** Aggiunge l'esercizio selezionato alle righe della griglia.
   * L'esercizio non viene automaticamente registrato nel modello del
   * prodotto, è necessario richiamare la funzione updateQuantitaSuImpianti()
   */
  private addSelected(selected) {
    let newRows = selected.filter(r =>
      !this.kendoRows.find(row => row.Progetto_Cod === r.Progetto_Cod)
    );
    newRows.forEach(row => this.kendoRows.push(this.evalObjRaccolta(row)));
    this.raccoltaService.updateFastHarvestTemplate(
      newRows.map((row: GridImpiantoSelezionatoModel) => row.Progetto_Cod),
      []
    );
  }

  // UPDATE

  private updateQuantitaSuImpianti() {
    let prodotto = this.dataShare.Prodotto.value as DettaglioRaccolta;
    let QsI = prodotto.QuantitaSuImpianti;
    let magazzino = this.dataShare.Magazzino;
    for (let row of this.kendoRows) {
      let q = QsI.find(c => c.esercizioCDC.esercizio.codice === row.Progetto_Cod);
      if (!q) {
        const CDC = this.qdcservice.GetEserciziCDCSelezionatiModel()
          .find(cdc => cdc.esercizio.codice === row.Progetto_Cod);
        q = new QuantitaSuImpianto();
        q.Qta = 0;
        q.esercizioCDC = CDC;
        QsI.push(q);
      }
      q.Magazzino = magazzino;
      q.Qta = parseFloat(row.Qta) || 0;
    }
    this.dataShare.Prodotto.patchValue(prodotto);
  }

  private updateLotto(ch: GridRaccoltaObject, force: boolean = false) {
    if (this.carichiAttivi || force) {
      let control = this.dataShare.Prodotto;
      let prodotto = control.value;
      let lotto = ch.Lotto.trim();
      if (!lotto && !force) {
        lotto = prodotto.QuantitaSuImpianti.map(q => q.Lotto).find(l => l) ?? "";
      }
      this.kendoRows.forEach(r => r.Lotto = (+r.Qta) ? lotto : "");
      prodotto.QuantitaSuImpianti.forEach(q => q.Lotto = q.Qta ? lotto : '');
      control.patchValue(prodotto);
    }
  }

  private calculateTotal() {
    let Qta_Tot = 0;
    if (this.kendoRows.length) {
      Qta_Tot = this.kendoRows.map(row => parseFloat(row.Qta))
        .reduce((q1, q2) => q1 + q2);
    }
    this.dataShare.formRipartizione.value
      .get('Qta_Tot').patchValue(Qta_Tot);
  }

  private onProductSelected() {
    this.kendoColumns.find(c => c.field === 'Qta').hidden = this.dataShare.Prodotto.value.prodotto.codice === 0;
    this.dataShare.Prodotto.valueChanges
      .pipe(takeUntil(this.signal))
      .subscribe((dettaglio) => {
        this.kendoColumns.find(c => c.field === 'Qta').hidden = !dettaglio.prodotto?.codice;
        if (!dettaglio.prodotto?.codice) {
          this.kendoRows.forEach(r => r.Qta = 0);
        }
      });
  }

  // LOAD

  /** Reads the selected fields and creates the grid's rows accordingly.
   *
   */
  private loadAppezzamenti(): GridRaccoltaObject[] {
    this.qdcservice.GridImpiantiHttpService.impiantiLoaded$
      .pipe(takeUntil(this.signal)).subscribe(() => this.refresh());
    let impianti = this._impianti.value as GridImpiantoSelezionatoModel[];
    impianti = impianti.sort((a, b) => this.compareGridImpianto(a, b));
    return impianti.map(i => this.evalObjRaccolta(i));
  }

  /** Map the specified field into a grid item used to model the grid rows. */
  private evalObjRaccolta(impianto: GridImpiantoSelezionatoModel): GridRaccoltaObject {
    const obj = new GridRaccoltaObject();
    const specie = this.qdcservice.GetSpeciefromUtilizzoTerreno();
    obj.Specie_Cod = specie.codice;
    obj.Specie_Des = specie.descrizione;
    obj.Progetto_Cod = impianto.Progetto_Cod;
    obj.Progetto_Des = impianto.Progetto_Des || impianto.App_Nome;
    obj.ID_Reg = impianto.ID_REG;
    obj.APPEZ_Des = impianto.App_Nome;
    obj.APPEZ_Cod = impianto.APPEZZA;
    obj.Cul_Cod = impianto.CUL_COD;
    obj.Cul_Des = impianto.Cul_Des;
    obj.Sup = impianto.Sup_Imp_help;
    this.loadQtaLottoFromSection(obj);
    return obj;
  }

  private loadQtaLottoFromSection(rigaRaccoltaImpianto: GridRaccoltaObject) {
    let quantitaSuImpianto: QuantitaSuImpianto = this.dataShare.Prodotto.value.QuantitaSuImpianti
      .find(q => q.esercizioCDC.esercizio.codice === rigaRaccoltaImpianto.Progetto_Cod);
    if (
      quantitaSuImpianto && quantitaSuImpianto.esercizioCDC?.esercizio
      && !quantitaSuImpianto.esercizioCDC.esercizio.descrizione
    ) {
      quantitaSuImpianto.esercizioCDC.esercizio.descrizione = rigaRaccoltaImpianto.Progetto_Des || rigaRaccoltaImpianto.APPEZ_Des;
    }
    rigaRaccoltaImpianto.Qta = !!quantitaSuImpianto ? quantitaSuImpianto.Qta : 0;
    rigaRaccoltaImpianto.Lotto = this.dataShare.Prodotto.value.MagazziniMovimentazioni?.at(0)?.Lotto;
    if (!rigaRaccoltaImpianto.Lotto) {
      if (this.raccoltaService.isLottoEditable()) rigaRaccoltaImpianto.Lotto = '';
      else rigaRaccoltaImpianto.Lotto = this.transloco.translate("GeneratoAutomaticamente");
    }
  }

}

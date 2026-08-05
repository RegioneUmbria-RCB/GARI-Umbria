import { Injectable, Injector } from '@angular/core';
import { GridServerResult } from 'app/qualita-tracciabilita/models/grid-server-result.model';
import {
  EditingMode, LoaderType, AbstractGridConfigService, HttpAction, KendoGridColumn, ModelEntry,
  CommandsColumnSettings, CommandsDropDownSettings, RendererGridEvent
} from 'gias-kendo-grid';
import { CELL_TYPES, GiasWindowsService } from 'gias-ui-kit';
import { VerificaDisciplinariService, ResultDataSource } from 'app/qualita-tracciabilita/services/verifica-disciplinari.service';
import { Observable, of, takeUntil, filter, tap, from, switchMap, take, map, Subscription, interval, catchError } from 'rxjs';
import { isNumber } from 'lodash';
import { GridCommandItem } from 'app/menu-agenda/components/utils';
import { AnalysisRequestDataItem } from 'app/qualita-tracciabilita/models/richiesta-analisi.model';
import { OperationTypes } from "app/qualita-tracciabilita/models/operation-types.enum";
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { AnalysisRequest_Status } from "app/Service/qdca-compliance-api.service";
import { MasterService } from 'app/Service/master.service';
import { Enum_OrigineRichiestaVerificaConformita, Enum_OrigineRichiestaDescrizione } from 'app/qualita-tracciabilita/models/origine-richieste.enum';
import { process } from '@progress/kendo-data-query';
import { faPrint } from '@fortawesome/free-solid-svg-icons';
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import { enum_PagineAgenda_2010 } from '../../../Model/siti.enum';
import { enum_CodificaStampe } from 'app/Model/TipiEnumerativi';

const MILLIS_PER_SECOND = 1000;

enum RequestStatus_number {
  Error = -1,
  New = 0,
  ToProcess = 1,
  Processing = 2,
  Completed = 3,
}

@Injectable({
  providedIn: 'root'
})
export class RiepilogoRichiesteGridService extends AbstractGridConfigService<GridServerResult> {
  editingMode: EditingMode = EditingMode.IN_LINE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'IdTestata';
  gridId = 'RiepilogoRichiesteGrid';

  private operations: BaseCodeDescr[] = [
    new BaseCodeDescr(OperationTypes.All, this.transloco.translate('TutteLeOperazioni')),
    new BaseCodeDescr(OperationTypes.Treatment, this.transloco.translate('Trattamenti')),
    new BaseCodeDescr(OperationTypes.Fertilization, this.transloco.translate('Fertilizzazioni')),
    new BaseCodeDescr(OperationTypes.Harvest, this.transloco.translate('Raccolte')),
  ];

  private readonly printIcon = faPrint;
  private _rows = [];
  private _processedRows = [];
  private _stateChangeSub: Subscription = null;
  private _state: any = null;

  private readonly cmdActions = {
    Print: 10
  };

  private _columns = [
    new KendoGridColumn({ field: 'Status', title: this.transloco.translate('StatoElaborazione') }, { editable: false }),
    new KendoGridColumn({ field: 'IdTestata', title: this.transloco.translate('CodiceRichiesta') }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'DataRichiesta', title: this.transloco.translate('DataRichiesta') }, { editable: false }),
    //new KendoGridColumn({ field: 'piva', title: this.transloco.translate('Piva') }, { editable: false }),
    new KendoGridColumn({ field: 'RagSoc', title: this.transloco.translate('Azienda') }, { editable: false }),
    new KendoGridColumn({ field: 'SaNome', title: this.transloco.translate('CentroAziendale') }, { editable: false }),
    new KendoGridColumn({ field: 'VegDes', title: this.transloco.translate('SpecieVegetale') }, { editable: false }),
    // new KendoGridColumn({ field: 'impianti_des', title: this.transloco.translate('Impianti') }, { editable: false }),
    new KendoGridColumn({ field: 'OperazioniDes', title: this.transloco.translate('TipoOperazioni') }, { editable: false }),
    new KendoGridColumn({ field: 'DpiDes', title: this.transloco.translate('Disciplinare') }, { editable: false }),
    // new KendoGridColumn({ field: 'flagIaf', title: this.transloco.translate('ImpegniFacoltativi') }, { editable: false }),
    new KendoGridColumn({ field: 'UserSettings', title: this.transloco.translate('SoloControlliUtente') }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'Storage', title: this.transloco.translate('VerificaConformitaMagazzino') }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'Regulations', title: this.transloco.translate('VerificaConformitaNormative') }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'IntervalloInizio', title: this.transloco.translate('Dal') }, { editable: false }),
    new KendoGridColumn({ field: 'IntervalloFine', title: this.transloco.translate('Al') }, { editable: false }),
    new KendoGridColumn({ field: 'Errore', title: this.transloco.translate('Errore_') }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'RegulationComplianceStr', title: this.transloco.translate('ConformitaNormative') }, { editable: false, hidden: false }),
    new KendoGridColumn({ field: 'StorageComplianceStr', title: this.transloco.translate('ConformitaMagazzino') }, { editable: false, hidden: false }),
  ];
  private _model = {
    IntervalloInizio: new ModelEntry(CELL_TYPES.DATE),
    IntervalloFine: new ModelEntry(CELL_TYPES.DATE),
    DataRichiesta: new ModelEntry(CELL_TYPES.DATETIME),
    Piva: new ModelEntry(CELL_TYPES.STRING),
    RagSoc: new ModelEntry(CELL_TYPES.STRING),
    IdTestata: new ModelEntry(CELL_TYPES.NUMBER),
    SaCod: new ModelEntry(CELL_TYPES.NUMBER),
    SaNome: new ModelEntry(CELL_TYPES.STRING),
    VegCod: new ModelEntry(CELL_TYPES.NUMBER),
    VegDes: new ModelEntry(CELL_TYPES.STRING),
    Impianti: new ModelEntry(CELL_TYPES.STRING),
    ImpiantiDes: new ModelEntry(CELL_TYPES.STRING),
    Operazioni: new ModelEntry(CELL_TYPES.STRING),
    OperazioniDes: new ModelEntry(CELL_TYPES.STRING),
    Dpi: new ModelEntry(CELL_TYPES.NUMBER),
    DpiDes: new ModelEntry(CELL_TYPES.STRING),
    IAF: new ModelEntry(CELL_TYPES.BOOLEAN),
    UserSettings: new ModelEntry(CELL_TYPES.BOOLEAN),
    Storage: new ModelEntry(CELL_TYPES.BOOLEAN),
    Regulations: new ModelEntry(CELL_TYPES.BOOLEAN),
    RequestStatus: new ModelEntry(CELL_TYPES.NUMBER),
    Status: new ModelEntry(CELL_TYPES.STRING),
    Errore: new ModelEntry(CELL_TYPES.STRING),
    Origin: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    OriginDes: new ModelEntry(CELL_TYPES.STRING),
    UsernameCreazione: new ModelEntry(CELL_TYPES.STRING),
    StorageCompliance: new ModelEntry(CELL_TYPES.NUMBER),
    StorageComplianceStr: new ModelEntry(CELL_TYPES.STRING),
    RegulationCompliance: new ModelEntry(CELL_TYPES.NUMBER),
    RegulationComplianceStr: new ModelEntry(CELL_TYPES.STRING),
  };

  constructor(
    injector: Injector,
    private analisiService: VerificaDisciplinariService,
    private gestioneRichieste: GestioneRichiesteService,
    private windowService: GiasWindowsService,
    private master: MasterService
  ) {
    super(injector);
    this.handleCustomization();
    // Any time the request list changes (initial load, new request, auto-reload), refresh the grid.
    // forceRefresh=true without data: the lib always re-calls read() on forceRefresh, so passing
    // a pre-built GridServerResult is wasted allocation.
    this.analisiService.listaRichieste$.pipe(takeUntil(this.signal))
      .subscribe(() => this.gridPublicService.refresh(true));

    interval(30 * MILLIS_PER_SECOND).pipe(
      takeUntil(this.signal),
      tap(() => this.toggleSyncButton()),
      filter(() => this.hasIncompletedRequests),
      tap(() => console.debug('Auto-refreshing grid')),
      switchMap(() => this.analisiService.readAnalysisRequestsAsLastFiltered())
    ).subscribe(); // listaRichieste$ subscription above handles the grid refresh
  }

  protected get hasIncompletedRequests(): boolean {
    return this._rows.some(r => [RequestStatus_number.Processing, RequestStatus_number.ToProcess, RequestStatus_number.New].includes(r.RequestStatus));
  }

  read(options?: any): Observable<GridServerResult> {
    if (this._stateChangeSub == null) {
      this._processedRows = this._rows;
      this._stateChangeSub = this.gridPublicService.gridComp.dataStateChange
        .subscribe((state) => {
          this._state = state;
          this._processedRows = process(this._rows, state).data;
          this.applyRendererRules();
        });
    }

    const isValorized = (str) => str == "" || str == undefined;
    this._rows = this.analisiService.listaRichieste$.value;
    this._rows.forEach((r: AnalysisRequestDataItem) => {
      r.DpiDes = this.getDPIDescr(r.Dpi, r.DpiDes);
      r.SaNome = r.SaCod == 0 ? this.transloco.translate('TuttiICentriAziendali') : r.SaNome;
      r.VegDes = r.VegCod == 0 ? this.transloco.translate('TutteLeSpecieVegetali') : r.VegDes;
      r.ImpiantiDes = isValorized(r.ImpiantiDes) ? this.transloco.translate('TuttiGliImpianti') : r.ImpiantiDes;
      r.OperazioniDes = this.getOperationTypeDesc(r.Operazioni);
      r.Status = this.getStatusDescr(r.RequestStatus);
      r.OriginDes = Enum_OrigineRichiestaDescrizione[Enum_OrigineRichiestaVerificaConformita[r.Origin]];
      r.StorageComplianceStr = this.getComplianceNullableString(r.StorageCompliance);
      r.RegulationComplianceStr = this.getComplianceNullableString(r.RegulationCompliance);
    });
    this._processedRows = this._state == null ? this._rows : process(this._rows, this._state).data;
    this.addSuperuserOnlyColumns();
    this.toggleSyncButton();
    return of(new GridServerResult(this._model, this._columns, this._rows));
  }

  perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
    return null;
  }

  override applyRendererRules(opts?: RendererGridEvent): void {
    const DOMRows = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr.k-master-row');
    if (DOMRows.length > this._processedRows.length) return;
    const compliant = [];
    const notCompliant = [];
    for (let i = 0; i < DOMRows.length; i++) {
      if (this.isRowNotCompliant(this._processedRows[i])) { notCompliant.push({ idx: i, row: DOMRows[i] }); }
      else { compliant.push({ idx: i, row: DOMRows[i] }); }
    }
    for (let item of notCompliant) {
      const style = `background-color: ${(item.idx % 2 != 0) ? '#ffebee' : '#ffdbe1'} !important; color: #c62828 !important;`;
      item.row.setAttribute('style', style);
      item.row.querySelectorAll('.k-command-cell').forEach(cell => cell.setAttribute('style', style));
    }
    for (let item of compliant) {
      const style = ``;
      item.row.setAttribute('style', style);
      item.row.querySelectorAll('.k-command-cell').forEach(cell => cell.setAttribute('style', style));
    }
  }

  private getComplianceNullableString(flag?: number): string {
    if (flag === undefined || flag === null) return '---';
    else if (flag === 0) return this.transloco.translate('NonConforme');
    else return this.transloco.translate('Conforme');
  }

  private getOperationTypeDesc(opType: string): string {
    const descr = opType.split(",")
      .map(x => x.trim())
      .map(x => this.operations.find(o => o.codice.toString() == x)?.descrizione || "")
      .join(", ");
    return descr == "" ? this.transloco.translate('TutteLeOperazioni') : descr;
  }

  /**
   * Determines if a row fails compliance checks based on storage and regulation compliance values.
   * 
   * @param row - The analysis request data item to evaluate
   * @returns boolean - Returns true when the row is non-compliant, false otherwise
   * 
   * @remarks
   * A row is considered non-compliant when:
   * 1. Both StorageCompliance and RegulationCompliance values exist (not undefined/null)
   * 2. At least one of these compliance values equals 0 (indicating non-compliance)
   * 
   * The method returns false in these scenarios:
   * - Either compliance value is undefined or null (check not performed)
   * - Both compliance values equal 1 (indicating full compliance)
   */
  private isRowNotCompliant(row: AnalysisRequestDataItem): boolean {
    // The check if considedered compliant if it hasn't been done (undefined or null) or it has been done and the result is compliant (1).
    const isValid = (value) => value === undefined || value === null || value === 1;
    return row != undefined && (!isValid(row.StorageCompliance) || !isValid(row.RegulationCompliance));
  }

  /**
   * Converts a status code or enum value into a localized description string.
   * 
   * @description This method translates status values into human-readable Italian text using the Transloco service.
   * It handles both numeric status codes and AnalysisRequest_Status enum values.
   * 
   * The status values and their translations are:
   * - -1 / AnalysisRequest_Status.Error → "InErrore" (Error)
   * - 1 / AnalysisRequest_Status.ToProcess → "DaElaborare" (To Process)
   * - 2 / AnalysisRequest_Status.Processing → "InElaborazione" (Processing)
   * - 3 / AnalysisRequest_Status.Completed → "Completata" (Completed)
   * - Any other value → "Sconosciuto" (Unknown)
   * 
   * @param {AnalysisRequest_Status | number} status - The status code or enum value to translate
   * @returns {string} The localized Italian description of the status
   * 
   * @example
   * // Using numeric status
   * const desc1 = this.getStatusDescr(1); // Returns "DaElaborare"
   * 
   * // Using enum status
   * const desc2 = this.getStatusDescr(AnalysisRequest_Status.Completed); // Returns "Completata"
   * 
   * // Using invalid status
   * const desc3 = this.getStatusDescr(99); // Returns "Sconosciuto"
   */
  private getStatusDescr(status: AnalysisRequest_Status | number): string {
    if (isNumber(status)) {
      switch (status) {
        case RequestStatus_number.Error: return this.transloco.translate('InErrore');
        case RequestStatus_number.ToProcess: return this.transloco.translate('DaElaborare');
        case RequestStatus_number.Processing: return this.transloco.translate('InElaborazione');
        case RequestStatus_number.Completed: return this.transloco.translate('Completata');
        default: return this.transloco.translate('DaElaborare');
      }
    } else {
      switch (status) {
        case AnalysisRequest_Status.Error: return this.transloco.translate('InErrore');
        case AnalysisRequest_Status.ToProcess: return this.transloco.translate('DaElaborare');
        case AnalysisRequest_Status.Processing: return this.transloco.translate('InElaborazione');
        case AnalysisRequest_Status.Completed: return this.transloco.translate('Completata');
        default: return this.transloco.translate('DaElaborare');
      }
    }
  }

  private getDPIDescr(dpiCod: string, dpiDes: string): string {
    const isValorized = (str) => str == "" || str == undefined;

    if (dpiCod == "-2")
      return this.transloco.translate("Biologico");
    if (dpiCod == "-1")
      return this.transloco.translate("Nessuno");

    return isValorized(dpiDes) ? this.transloco.translate('IndicatoInOperazione') : dpiDes;
  }

  private toggleSyncButton() {
    document.querySelector('button[id="syncRequestsBtn"]')["disabled"] = !this.hasIncompletedRequests;
  }

  private addSuperuserOnlyColumns() {
    if (this.master.isSuperuser() && !this._columns.some(x => x.field == 'UsernameCreazione')) {
      this._columns.push(new KendoGridColumn({ field: 'UsernameCreazione', title: this.transloco.translate('UsernameCreazione') }, { editable: false }));
    }
    if (this.master.isSuperuser() && !this._columns.some(x => x.field == 'OriginDes')) {
      this._columns.push(new KendoGridColumn({ field: 'OriginDes', title: this.transloco.translate('OrigineRichiesta') }, { editable: false }));
    }
  }

  private handleCustomization() {
    this.setupDdlCmdButtons();
    if (!/[?&]debug=gennaio26/.test(location.search)) {
      this.setupCmdColumn();
    }
    this.pagination.gridState.take = 25;
    this.generalSettings.height = "-webkit-fill-available";
    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;
  }

  private setupCmdColumn() {
    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, removeBtn: false, infoBtn: true });
    this.cmdColumn.onDisableInfoBtn = (x: AnalysisRequestDataItem) => x.RequestStatus as unknown as number != 3;
    this.gridPublicService.changeDetected.pipe(takeUntil(this.signal))
      .subscribe((ev) => this.openResultsWindow(ev.dataItem as AnalysisRequestDataItem, ResultDataSource.FromTable));
  }

  private setupDdlCmdButtons() {
    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, removeBtn: false, infoBtn: false });
    this.cmdDropDown = new CommandsDropDownSettings({
      fullEditBtn: false,
      hidecmdDropDown: (row) => row['RequestStatus'] == RequestStatus_number.Completed ? false : true
    });
    if (/[?&]debug=gennaio26/.test(location.search)) {
      this.cmdDropDown.addCommand(new GridCommandItem("Apri Dettaglio (da tabelle db)", ResultDataSource.FromTable, 'faQdCSearchDocument'));
      this.cmdDropDown.addCommand(new GridCommandItem("Apri Dettaglio (da engine)", ResultDataSource.FromEngine, 'faQdCSearchDocument'));
    }
    this.cmdDropDown.addCommand(new GridCommandItem("Stampa", this.cmdActions.Print, "", this.printIcon));

    this.gridPublicService.openCommands.pipe(takeUntil(this.signal), filter(x => x != null))
      .subscribe(request => {
        if (request.RequestStatus != RequestStatus_number.Completed)
          this.cmdDropDown.removeCommand(this.cmdActions.Print);
      });

    this.gridPublicService.commandEvent.pipe(takeUntil(this.signal), filter(x => x != null))
      .subscribe(x => {
        if (ResultDataSource[x.command.action] !== undefined) {
          this.openResultsWindow(x.dataItem as AnalysisRequestDataItem, x.command.action);
        } else if (x.command.action == this.cmdActions.Print) {
          this.print(x.dataItem);
        }
      });
  }

  private print(dataItem: any) {
    const page = enum_CodificaStampe.RisultatoAnalisiConformita;
    const params: Array<ParametriAggiuntivi_QueryString> = [
      { key: 'IdTestata', value: dataItem.IdTestata, codifica: false },
      { key: 'veg_cod', value: dataItem.VegCod, codifica: false },
      { key: 'veg_des', value: dataItem.VegDes == "" ? this.transloco.translate('TutteLeSpecie') : dataItem.VegDes, codifica: false },
      { key: 'sa_cod', value: dataItem.SaCod, codifica: false },
      { key: 'sa_nome', value: dataItem.SaNome == "" ? this.transloco.translate('TuttiICentri') : dataItem.SaNome, codifica: false },
      { key: 'data_da', value: new Date(dataItem.IntervalloInizio).toLocaleDateString(), codifica: false },
      { key: 'data_a', value: new Date(dataItem.IntervalloFine).toLocaleDateString(), codifica: false },
      { key: 'strKendoOperazioni', value: JSON.stringify([]), codifica: false },
      { key: 'strKendoDettagli', value: JSON.stringify([]), codifica: false },
      { key: 'strKendoOperazioniMagazzino', value: JSON.stringify([]), codifica: false },
      { key: 'strKendoDettagliMagazzino', value: JSON.stringify([]), codifica: false },
    ];

    this.analisiService.analysisDataItem = dataItem;
    this.analisiService.analysisDataItem['source'] = ResultDataSource.FromTable;
    this.analisiService.readAnalysisResults().pipe(
      take(1),
      switchMap(() => this.analisiService.risultatoAnalisi$.pipe(take(1))),
      tap((details) => {
        let operations = [...details.Trattamenti, ...details.Fertilizzazioni, ...details.Raccolte];
        this.mapOperationsForPrint(operations, params);

        if (details.VerificheGiacenzaInizio != undefined)
          details.VerificheGiacenzaInizio["IdAttivita"] = -11; // fake id to distinguish storage checks in the printout
        if (details.VerificheGiacenzaFine != undefined)
          details.VerificheGiacenzaFine["IdAttivita"] = -11; // fake id to distinguish storage checks in the printout
        let storages = [details.VerificheGiacenzaInizio, ...details.VerificheMagazzino, details.VerificheGiacenzaFine]
          .filter(x => x != undefined);
        this.mapStoragesForPrint(storages, params);
      }),
      switchMap(() => from(this.gestioneRichieste.gestionePassaggioAltroSito(
        Enum_SiteRedirector.Sito_AgronicaStampe_2010, page, params, null, true))),
      catchError(() => {
        console.error("Errore durante la generazione del report di stampa.")
        return of(null);
      })
    ).subscribe(url => {
      if (url == undefined) return;
      let redirectUrl = url as unknown as string;
      window.open(redirectUrl).focus();
    });
  }

  private mapOperationsForPrint(operations: any[], params: Array<ParametriAggiuntivi_QueryString>) {
    let mappedDetails = [];
    let mappedOperations = [];
    for (let op of operations) {
      mappedOperations.push({
        "lav_cod": op.CodiceAttivita,
        "id_agenda": +op.IdAttivita,
        "data_movimento": new Date(op.DataAttivita).toLocaleString(),
        "tipointervento": op.DescrizioneAttivita,
        "des_lib": this.stripHtmlTags(op.DettaglioTecnico),
        "disciplinare_des": op.CodiceDisciplinareVerificato,
        "sa_nome": op.SaNome,
        "appezzamenti": op.Impianti.map(i => i.DescrizioneImpianto).join(", "),
        "superficie": op.Impianti.map(i => i.Superficie).reduce((a, b) => a + b, 0),
      });

      let detailsSorted = op.DettagliVerifica.sort((a, b) => a.Descrizione.localeCompare(b.Descrizione));
      for (let det of detailsSorted) {
        mappedDetails.push({
          "id_agenda": +op.IdAttivita,
          "err_code": det.CodiceControllo,
          "err_des": det.Descrizione,
          "BooleanRisVer": det.Esito,
          "err_nota": det.Esito ? "" : det.Messaggio
        });
      }
    }
    params.find(p => p.key == 'strKendoOperazioni').value = JSON.stringify(mappedOperations);
    params.find(p => p.key == 'strKendoDettagli').value = JSON.stringify(mappedDetails);
  }

  private stripHtmlTags(input: string): string {
    let doc = new DOMParser().parseFromString(input, 'text/html');
    return doc.body.textContent || doc.body.innerText || '';
  }

  private mapStoragesForPrint(storages: any[], params: Array<ParametriAggiuntivi_QueryString>) {
    let mappedDetails = [];
    let mappedOperations = [];
    for (let op of storages) {
      mappedOperations.push({
        "lav_cod": 91619329, // didn't find it in the lavcod enum
        "id_agenda": +op.IdAttivita,
        "data_movimento": new Date(op.OperationDate).toLocaleString(),
        "des_lib": op.OperationDescription,
        "fabbricato_des": op.StorageName,
        "BooleanRisVer": op.Compliant,
      });

      let detailsSorted = op.DettagliVerifica.sort((a, b) => a.NomeProdotto.localeCompare(b.NomeProdotto));
      for (let det of detailsSorted) {
        mappedDetails.push({
          "id_agenda": +op.IdAttivita,
          "prodotto": det.NomeProdotto,
          "qta": det.QtaScaricata,
          "qta_presente": det.QtaPresenteInMagazzino,
          "udm_sim": det.UnitaDiMisuraDes,
          "BooleanRisVer": det.Esito,
          "RisultatoVerifica": det.Messaggio
        });
      }
    }
    params.find(p => p.key == 'strKendoOperazioniMagazzino').value = JSON.stringify(mappedOperations);
    params.find(p => p.key == 'strKendoDettagliMagazzino').value = JSON.stringify(mappedDetails);
  }

  private openResultsWindow(dataItem: AnalysisRequestDataItem, source: ResultDataSource) {
    this.analisiService.analysisDataItem = dataItem;
    this.analisiService.analysisDataItem['source'] = source;
    this.analisiService.readAnalysisResults().subscribe(() => {
      let winRef = this.windowService.open({
        title: this.transloco.translate("RisultatoVerifica"),
        content: this.analisiService.analysisDetailRef,
        height: window.innerHeight * 0.9,
        width: window.innerWidth * 0.9,
      }, false);
    });
  }

}

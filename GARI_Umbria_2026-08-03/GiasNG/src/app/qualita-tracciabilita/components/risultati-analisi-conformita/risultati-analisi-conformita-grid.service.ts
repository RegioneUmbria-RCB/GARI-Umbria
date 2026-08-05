import { Injectable, Injector } from '@angular/core';
import {
  EditingMode, LoaderType, AbstractGridConfigService, HttpAction, KendoServerResult, KendoGridColumn, ModelEntry,
  MasterDetailSettings, CommandsColumnSettings, ConfigTemplate, CommandsDropDownSettings,
  RendererGridEvent, CommandsDropDownEvents
} from 'gias-kendo-grid';
import { CELL_TYPES, Enum_DBTypeOperation, Tipo_Attivita, GiasIFrameWindowService } from 'gias-ui-kit';
import { Observable, filter, takeUntil, map, tap, of, take, catchError, switchMap, from, Subscription } from 'rxjs';
import { process } from '@progress/kendo-data-query';
import { VerificaDisciplinariService } from 'app/qualita-tracciabilita/services/verifica-disciplinari.service';
import { GridServerResult } from 'app/qualita-tracciabilita/models/grid-server-result.model';
import { DisciplinariService } from 'app/Service/Metaschema/disciplinari.service';
import { Disciplinare } from 'app/Model/metaschema/Disciplinari';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { MasterService } from 'app/Service/master.service';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { enum_PagineGiasNG } from 'app/Model/TipiEnumerativi';
import { GridCommandItem } from 'app/menu-agenda/components/utils';
import { ChiaveImpianto } from '../../../Service/api.service';

@Injectable({ providedIn: 'root' })
export class RisultatiAnalisiConformitaGridService extends AbstractGridConfigService<GridServerResult> {
  editingMode: EditingMode = EditingMode.IN_LINE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'Row_Key';
  gridId = 'RisultatiAnalisiConformitaGrid';

  private rows = [];
  private _columns = [
    new KendoGridColumn({ field: 'DataAttivita', title: this.transloco.translate('Data') }, { editable: false }),
    new KendoGridColumn({ field: 'IdAttivita', title: this.transloco.translate('ID') }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'DescrizioneAttivita', title: this.transloco.translate('Intervento') }, { editable: false }),
    new KendoGridColumn({ field: 'DettaglioTecnico', title: this.transloco.translate('DettaglioTecnico') }, { editable: false, hidden: true, showHTMLAsString: true }),
    new KendoGridColumn({ field: 'SaNome', title: this.transloco.translate('CentroAziendale') }, { editable: false }),
    new KendoGridColumn({ field: 'SpecieVegetale', title: this.transloco.translate('SpecieVegetale') }, { editable: false }),
    new KendoGridColumn({ field: 'CodiceDisciplinareVerificato', title: this.transloco.translate('DisciplinareVerificato') }, { editable: false }),
    // new KendoGridColumn({ field: 'Avversita', title: this.transloco.translate('Avversita') }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'AppezzamentiCoinvolti', title: this.transloco.translate('AppezzamentiCoinvolti') }, { editable: false }),
    new KendoGridColumn({ field: 'SuperficieTrattataTotale', title: this.transloco.translate('SupCoinvolta') }, { editable: false }),
    new KendoGridColumn({ field: 'Conforme', title: this.transloco.translate('Conforme') }, { editable: false }),
  ];
  private _model = {
    DataAttivita: new ModelEntry(CELL_TYPES.DATETIME),
    IdAttivita: new ModelEntry(CELL_TYPES.NUMBER),
    DescrizioneAttivita: new ModelEntry(CELL_TYPES.STRING),
    DettaglioTecnico: new ModelEntry(CELL_TYPES.STRING),
    CodiceDisciplinareVerificato: new ModelEntry(CELL_TYPES.STRING),
    SaNome: new ModelEntry(CELL_TYPES.STRING),
    SpecieVegetale: new ModelEntry(CELL_TYPES.STRING),
    Prodotto: new ModelEntry(CELL_TYPES.STRING),
    Avversita: new ModelEntry(CELL_TYPES.STRING),
    AppezzamentiCoinvolti: new ModelEntry(CELL_TYPES.STRING),
    SuperficieTrattataTotale: new ModelEntry(CELL_TYPES.STRING),
    Conforme: new ModelEntry(CELL_TYPES.STRING),
  };

  private _protocols: Disciplinare[] = [];
  private _processedRows = [];
  private _stateChangeSub: Subscription = null;

  constructor(
    injector: Injector,
    private master: MasterService,
    private message: GiasMessageService,
    private agenda: ObjParametriAgendaService,
    private gestioneRichieste: GestioneRichiesteService,
    private iFrame: GiasIFrameWindowService,
    private analisiService: VerificaDisciplinariService,
    private disciplinariService: DisciplinariService,
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.handleCustomization();
    this.loadProtocols().subscribe();
  }

  read(options?: any): Observable<GridServerResult> {
    if (this._stateChangeSub == null) {
      this._processedRows = this.rows;
      this._stateChangeSub = this.gridPublicService.gridComp.dataStateChange
        .subscribe((state) => {
          this._processedRows = process(this.rows, state).data;
          this.applyRendererRules();
        });
    }

    this.master.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
    return this.analisiService.risultatoAnalisi$.pipe(
      takeUntil(this.signal),
      filter(x => x != null),
      map(x => [...x.Trattamenti, ...x.Fertilizzazioni, ...x.Raccolte]
        .sort((a, b) => new Date(b.DataAttivita).valueOf() - new Date(a.DataAttivita).valueOf())),
      tap(x => x.forEach(y => {
        const plantSet = new Map<string, string>();
        y.Impianti.forEach(i => plantSet.set(i.ChiaveImpianto, i.DescrizioneImpianto));
        y.AppezzamentiCoinvolti = Array.from(plantSet.values()).join(', ');
        y.Conforme = this.isOverallOk(y) ? this.transloco.translate('Si') : this.transloco.translate('No');
        // y.DisciplinareVerificato = this._protocols.find(p => p.codice.startsWith(y.CodiceDisciplinareVerificato))?.descrizione || "";
      })),
      tap(x => this.rows = x),
      tap(x => this._processedRows = x),
      tap(() => this.master.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef })),
      map(() => new GridServerResult(this._model, this._columns, this._processedRows)),
      catchError(() => {
        this.master.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
        console.debug('Catching error during read() of RisultatiAnalisiConformitaGridService');
        this.message.errorMessage(this.transloco.translate('ErroreDuranteLaLetturaDeiDati'));
        return of(new GridServerResult(this._model, this._columns, []));
      })
    );
  }

  perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
    return null;
  }

  override applyRendererRules(opts?: RendererGridEvent): void {
    const btnExpand = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr kendo-icon');
    for (let i = 0; i < btnExpand.length; i++) {
      btnExpand[i].setAttribute('style', 'margin-left: -13px; width: 30px !important;');
    }
    const DOMRows = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr.k-master-row');
    const compliant = [];
    const notCompliant = [];
    for (let i = 0; i < DOMRows.length; i++) {
      if (this.isOverallOk(this._processedRows[i])) { compliant.push({ idx: i, row: DOMRows[i] }); }
      else { notCompliant.push({ idx: i, row: DOMRows[i] }); }
    }
    for (let item of notCompliant) {
      const style = `background-color: ${(item.idx % 2 != 0) ? '#ffebee' : '#ffdbe1'} !important; color: #c62828 !important;`;
      item.row.setAttribute('style', style);
      item.row.querySelector('.k-command-cell').setAttribute('style', style);
    }
    for (let item of compliant) {
      const style = ``;
      item.row.setAttribute('style', style);
      item.row.querySelector('.k-command-cell').setAttribute('style', style);
    }
  }

  private handleCustomization() {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      removeBtn: false,
    });
    this.masterdetailSettings = new MasterDetailSettings(true, {
      flag_grid_detail: false,
      flag_grid_master: true,
      showDetailTemplate: (dataitem: any, rowIndex: number) => true,
    });
    this.pagination.gridState.take = 100;
    this.generalSettings.height = "-webkit-fill-available";
    this.setupDdlCmdButtons();
  }

  private setupDdlCmdButtons() {
    this.cmdDropDown = new CommandsDropDownSettings({
      fullEditBtn: this.analisiService.canWriteQdc,
      infoBtn: this.analisiService.canReadQdc
    });
    this.gridPublicService.commandEvent.pipe(
      takeUntil(this.signal),
      filter(x => x != null),
      tap(x => this.setParametriAgenda(x.dataItem, x.command)),
      switchMap(() => from(this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Edit_Attivita))),
      switchMap((url: string) => {
        let fWin = this.iFrame.open({
          content: url + '?seFrame=1',
          width: window.innerWidth * 0.9,
          height: window.innerHeight * 0.9
        });
        fWin.window.location.nativeElement.setAttribute('style', 'top: 5% !important;');
        return fWin.result;
      })
    ).subscribe(() => this.master.changeShowBackground(true));
  }

  private setParametriAgenda(dataItem, command: GridCommandItem) {
    const objAgenda = this.agenda.getObjParamValue();
    objAgenda.Sa_Cod = 0;
    objAgenda.Data = dataItem.DataAttivita;
    objAgenda.Id_Agenda = dataItem.IdAttivita;
    objAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
    objAgenda.TipoOperazioneDB = command.action == CommandsDropDownEvents.INFO
      ? Enum_DBTypeOperation.Read : Enum_DBTypeOperation.Update;

    this.agenda.changeObjParametriAgenda(objAgenda);
  }

  private isOverallOk(dataItem): boolean {
    const set = new Map<number, boolean>();
    const addItem = (item: any) => {
      if (item == undefined) return;

      if (set.has(item.CodiceControllo)) {
        let outcome = set.get(item.CodiceControllo);
        outcome = outcome && item.Esito;
        set.set(item.CodiceControllo, outcome);
      } else {
        set.set(item.CodiceControllo, item.Esito);
      }
    };

    dataItem.DettagliVerifica?.forEach(addItem);
    const controls = Array.from(set.values());
    return controls.every(outcome => outcome == true);
  }

  private loadProtocols(): Observable<Disciplinare[]> {
    const dataitem = this.analisiService.analysisDataItem;
    return this.disciplinariService.leggi({
      lavorazione: null,
      specie: new BaseCodeDescr(dataitem.VegCod),
      data: new Date(),
      privato: false,
      regolamento: null,
      validita: new IntervalloTemporale(dataitem.IntervalloInizio, dataitem.IntervalloFine)
    }).pipe(
      takeUntil(this.signal),
      filter(d => d != undefined),
      tap(d => this._protocols = d));
  }
}

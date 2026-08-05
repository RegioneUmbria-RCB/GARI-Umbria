import { Injectable, Injector } from '@angular/core';
import { process } from '@progress/kendo-data-query';
import { Observable, of, takeUntil, map, filter, tap, from, take, switchMap, Subscription } from 'rxjs';
import {
  EditingMode, LoaderType, AbstractGridConfigService, HttpAction, KendoServerResult, KendoGridColumn, ModelEntry,
  MasterDetailSettings, CommandsColumnSettings, GridCustomizations, RendererGridEvent, KendoGridRow,
  CommandsDropDownSettings, CommandsDropDownEvents
} from 'gias-kendo-grid';
import { CELL_TYPES, Enum_DBTypeOperation, Tipo_Attivita, GiasIFrameWindowService } from 'gias-ui-kit';
import { GridServerResult } from 'app/qualita-tracciabilita/models/grid-server-result.model';
import { VerificaDisciplinariService } from 'app/qualita-tracciabilita/services/verifica-disciplinari.service';
import { MasterService } from 'app/Service/master.service';
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { enum_PagineGiasNG } from 'app/Model/TipiEnumerativi';
import { GridCommandItem } from 'app/menu-agenda/components/utils';

@Injectable({
  providedIn: 'root'
})
export class RisultatiAnalisiMagazzinoGridService extends AbstractGridConfigService<GridServerResult> {
  editingMode: EditingMode = EditingMode.IN_PAGE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'Row_Key';
  gridId = 'DettaglioMagazzino';

  private _columns = [
    new KendoGridColumn({ field: 'OperationDate', title: this.transloco.translate('Data') }, { editable: false, width: 150 }),
    new KendoGridColumn({ field: 'IdAttivita', title: this.transloco.translate('ID') }, { editable: false, hidden: true }),
    new KendoGridColumn({ field: 'OperationDescription', title: this.transloco.translate('Intervento') }, { editable: false }),
    new KendoGridColumn({ field: 'StorageName', title: this.transloco.translate('Magazzino') }, { editable: false }),
    new KendoGridColumn({ field: 'Lotti', title: this.transloco.translate('Lotto2') }, { editable: false }),
    new KendoGridColumn({ field: 'Conforme', title: this.transloco.translate('Conforme') }, { editable: false }),
  ];
  private _model = {
    OperationDate: new ModelEntry(CELL_TYPES.DATE),
    IdAttivita: new ModelEntry(CELL_TYPES.STRING),
    OperationDescription: new ModelEntry(CELL_TYPES.STRING),
    StorageName: new ModelEntry(CELL_TYPES.STRING),
    Compliant: new ModelEntry(CELL_TYPES.BOOLEAN),
    Conforme: new ModelEntry(CELL_TYPES.STRING),
    Lotti: new ModelEntry(CELL_TYPES.STRING),
  };
  /** The rows displayed in the grid. */
  private _rows = [];
  private _processedRows = [];
  private _stateChangeSub: Subscription;

  constructor(
    injector: Injector,
    private master: MasterService,
    private message: GiasMessageService,
    private agenda: ObjParametriAgendaService,
    private gestioneRichieste: GestioneRichiesteService,
    private iFrame: GiasIFrameWindowService,
    private analisiService: VerificaDisciplinariService
  ) {
    super(injector);
    this.handleCustomization();
  }

  read(dataItem?: any): Observable<GridServerResult> {
    if (this._stateChangeSub == null) {
      this._processedRows = this._rows;
      this._stateChangeSub = this.gridPublicService.gridComp.dataStateChange
        .pipe(takeUntil(this.signal), tap((state) => this._processedRows = process(this._rows, state).data))
        .subscribe(() => this.applyRendererRules());
    }

    return this.analisiService.risultatoAnalisi$.pipe(
      takeUntil(this.signal),
      filter(x => x != null),
      map(x => [
        x.VerificheGiacenzaInizio,
        ...x.VerificheMagazzino.sort((a, b) => new Date(a.OperationDate).valueOf() - new Date(b.OperationDate).valueOf()),
        x.VerificheGiacenzaFine
      ]),
      tap(rows => rows.forEach(x => {
        x.Conforme = x.Compliant ? this.transloco.translate("Si") : this.transloco.translate("No");
        let lotti: Set<string> = new Set<string>();
        if (x.DettagliVerifica != undefined && x.DettagliVerifica.length > 0) {
          lotti = new Set(x.DettagliVerifica.filter(d => d.Lotto != undefined && d.Lotto != "").map(d => d.Lotto));
        }
        x['Lotti'] = lotti.size > 0 ? Array.from(lotti).join(", ") : "";
      })),
      tap(x => this._rows = x),
      tap(() => this._processedRows = this._rows),
      map(() => new GridServerResult(this._model, this._columns, this._rows))
    );
  }

  perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
    return null;
  }

  override applyRendererRules(opts?: RendererGridEvent): void {
    const compliant = [];
    const notCompliant = [];

    const DOMRows = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr.k-master-row');
    const btnExpand = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr kendo-icon');
    const btnMenu = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody .k-menu-button');
    for (let i = 0; i < DOMRows.length; i++) {
      // Adjust expand button positioning
      btnExpand[i].setAttribute('style', 'margin-left: -13px; width: 30px !important;');
      // Hide menu button if no activity ID (IdAttivita) is present (just for stock checks without related activities)
      if (this._processedRows[i].IdAttivita == undefined) {
        btnMenu[i].setAttribute('style', 'display: none;');
      } else {
        btnMenu[i].setAttribute('style', '');
      }
      // Collect compliant and non-compliant rows for styling
      if (this._processedRows[i].Compliant) { compliant.push({ idx: i, row: DOMRows[i] }); }
      else { notCompliant.push({ idx: i, row: DOMRows[i] }); }
    }

    this.styleCompliantRows(compliant);
    this.styleNonCompliantRows(notCompliant);
  }

  private styleCompliantRows(compliant) {
    for (let item of compliant) {
      const style = ``;
      item.row.setAttribute('style', style);
      item.row.querySelector('.k-command-cell').setAttribute('style', style);
    }
  }

  private styleNonCompliantRows(notCompliant) {
    for (let item of notCompliant) {
      const style = `background-color: ${(item.idx % 2 != 0) ? '#ffebee' : '#ffdbe1'} !important; color: #c62828 !important;`;
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
          content: url,
          width: window.innerWidth * 0.9,
          height: window.innerHeight * 0.9
        });
        fWin.window.location.nativeElement.setAttribute('style', 'top: 5% !important;');
        return fWin.result;
      }),
      take(1)
    ).subscribe(() => this.master.changeShowBackground(true));
  }

  private setParametriAgenda(dataItem, command: GridCommandItem) {
    const objAgenda = this.agenda.getObjParamValue();
    objAgenda.Sa_Cod = 0;
    objAgenda.Data = dataItem.OperationDate;
    objAgenda.Id_Agenda = dataItem.IdAttivita;
    objAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
    objAgenda.TipoOperazioneDB = command.action == CommandsDropDownEvents.INFO
      ? Enum_DBTypeOperation.Read : Enum_DBTypeOperation.Update;

    this.agenda.changeObjParametriAgenda(objAgenda);
  }
}

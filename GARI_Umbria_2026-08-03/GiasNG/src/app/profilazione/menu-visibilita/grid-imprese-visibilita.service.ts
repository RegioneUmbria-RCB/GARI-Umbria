import {Injectable, Injector} from '@angular/core';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import {filter, map, Observable} from "rxjs";
import {CommandsColumnSettings, ExcelSettings} from 'gias-kendo-grid';
import {ProfilazioneDataShareService} from "../services/profilazione-data-share.service";
import {
  GridProfilazioneServerResult
} from "../gestione-utenti/utenti/griglia-menu-profilazione/grid-menu-profilazione.service";
import { VisibilitaService } from '../services/visibilita.service';


@Injectable()
export class GridImpreseVisibilitaService extends AbstractGridConfigService<GridProfilazioneServerResult> {
  public editingMode = EditingMode.IN_PAGE;
  public gridId = "impreseVisibilita";
  public rowId = "piva";
  public loader = LoaderType.SERVICE;

  private kRows = [];
  private kCols = [
    new KendoGridColumn({ field:'Username', title:this.transloco.translate('Username')}, {editable: false}),
    new KendoGridColumn({ field:'DettagliUtente', title:this.transloco.translate('Utente')}, {editable: false}),
    new KendoGridColumn({ field:'Gruppo_Des', title:this.transloco.translate('Gruppo')}, {editable: false}),
    new KendoGridColumn({ field:'rag_soc', title:this.transloco.translate('RagioneSociale')}, {editable: false}),
    new KendoGridColumn({ field:'partitaIvaReale', title:this.transloco.translate('Piva')}, {editable: false}),
    new KendoGridColumn({ field:'Cuaa', title:this.transloco.translate('Cuaa')}, {editable: false}),
    new KendoGridColumn({ field:'Sa_Nome', title:this.transloco.translate('CentroAziendale')}, {editable: false}),
  ];
  private kModel = {
    Username: new ModelEntry(CELL_TYPES.STRING),
    DettagliUtente: new ModelEntry(CELL_TYPES.STRING),
    Gruppo_Des: new ModelEntry(CELL_TYPES.STRING),
    piva: new ModelEntry(CELL_TYPES.STRING),
    Cuaa: new ModelEntry(CELL_TYPES.STRING),
    rag_soc: new ModelEntry(CELL_TYPES.STRING),
    Sa_Cod: new ModelEntry(CELL_TYPES.STRING),
    Sa_Nome: new ModelEntry(CELL_TYPES.STRING),
    partitaIvaReale: new ModelEntry(CELL_TYPES.STRING)
  };

  constructor(injector: Injector,
              private datashare: ProfilazioneDataShareService,
              private visibilitaService: VisibilitaService
  ) {
    super(injector);
    this.handleCustomization();
  }

  read(options: any): Observable<GridProfilazioneServerResult> {
    return this.datashare.visibilitaImprese.pipe(
      filter(value => !!value),
      map(value => {
        this.kRows = Array.isArray(value) ? value : [];
        return new GridProfilazioneServerResult(this.kModel, this.kCols, this.kRows);
      })
    );
  }

  perform(actionType: HttpAction, items: any): Observable<any> {
    return undefined;
  }

  private handleCustomization() {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false
    });

    this.behavior.excelSettings = new ExcelSettings({enabled: true});
    this.groups.groupable.enabled = true;
    this.pagination.gridState.take = 100;
  }

}

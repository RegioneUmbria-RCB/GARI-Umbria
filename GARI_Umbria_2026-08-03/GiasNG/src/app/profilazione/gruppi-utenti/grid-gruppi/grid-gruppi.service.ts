import {Injectable, Injector} from "@angular/core";
import {TranslocoService} from "@jsverse/transloco";
import {MasterService} from "../../../Service/master.service";
import {CommandsColumnSettings, ToolbarSettings} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {BehaviorSubject, from, map, Observable, of} from "rxjs";
import {GruppiUtentiService} from "../../services/gruppi-utenti.service";
import {GruppoUtente} from "../../models/gruppi-utenti/GruppoUtente.model";
import {PermessiUtenteService} from "../../../Service/permessi-utente.service";
import {enum_Security_Attivita} from "../../../Model/TipiEnumerativi";
import {enum_TipoPermesso} from "../../models/profilazione.model";

export class GridGruppiServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class GridGruppiService extends AbstractGridConfigService<GridGruppiServerResult> {
  editingMode: EditingMode = EditingMode.IN_LINE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'Gruppo_Cod';
  gridId = 'GruppiGridID';

  public permessoEdit: boolean = false;
  public permessoInfo: boolean = true;

  private kendoRows: GruppoUtente[] = [];
  private kendoModel = {
    codice: new ModelEntry(CELL_TYPES.STRING),
    descrizione: new ModelEntry(CELL_TYPES.STRING),
    identificativo: new ModelEntry(CELL_TYPES.STRING),
  };
  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({field: 'codice', title: this.transloco.translate('Codice')}, {hidden: true, editable: false}),
    new KendoGridColumn({field: 'descrizione', title: this.transloco.translate('prof.GruppoUtente')}),
    new KendoGridColumn({field: 'identificativo', title: this.transloco.translate('prof.Identificativo')}),
  ];

  constructor(
    injector: Injector,
    private masterService: MasterService,
    private gruppiService: GruppiUtentiService,
    private permessiService: PermessiUtenteService
  ) {
    super(injector);
    this.permessoInfo = this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiGruppi, enum_TipoPermesso.LETTURA);
    this.permessoEdit = this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiGruppi, enum_TipoPermesso.LETTURA_SCRITTURA);
    this.handleCustomization();
    this.handleEvents();
  }

  read(options?: any): Observable<GridGruppiServerResult> {
    this.masterService.set_isLoading({isLoading: true, message: ''});
    return this.gruppiService.readGruppi().pipe(map((gruppi: GruppoUtente[]) => {
      this.masterService.set_isLoading({isLoading: false, message: ''});
      this.kendoRows = gruppi;
      return new GridGruppiServerResult(gruppi, this.kendoColumns, this.kendoModel);
    }));
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    if (actionType === HttpAction.CREATE) {
      this.gruppiService.nuovoGruppo({codice: 0, descrizione: items.descrizione, identificativo: items.identificativo, transizioniUsate: []});
    } else if (actionType === HttpAction.UPDATE) {
      this.gruppiService.modificaGruppo({codice: items.codice, descrizione: items.descrizione, identificativo: items.identificativo, transizioniUsate: []});
    } else if (actionType === HttpAction.REMOVE) {
      this.gruppiService.rimuoviGruppo({codice: items.codice, descrizione: items.descrizione, identificativo: items.identificativo, transizioniUsate: []});
    }
    this.selectable.selectable.disableAllcheckbox = false;
    this.gridPublicService.refresh(true);
    return of([]);
  }

  private handleCustomization() {
    this.cmdColumn = new CommandsColumnSettings({
      infoBtn: false,
      editBtn: this.permessoEdit,
      removeBtn: this.permessoEdit,
    });
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = this.permessoEdit;
    this.groups.groupable.enabled = false;
    this.selectable.selectable.enabled = this.permessoInfo;
    this.selectable.selectable.mode = "single";
    this.selectable.selectable.checkboxOnly = false;
    this.selectable.shouldShowCheckbox = true;
  }

  private handleEvents() {
    this.gridPublicService.changeDetected.subscribe((event: any) => {
      if(event?.action === 'edit' || event?.action === 'remove' || event?.action === 'info' || event?.action === 'add') {
        this.gruppiService.gruppo$.next(null);
        this.gridPublicService.giasGridComponent.deselectAllRows();
        this.selectable.selectable.disableAllcheckbox = true;
      } else if (event?.action === 'cancel') {
        this.selectable.selectable.disableAllcheckbox = false;
      }
    });

    this.gridPublicService.selection.getSelectedValue.subscribe(selection => {
      if (selection && selection.value.selectedRows.length) {
        this.gruppiService.gruppo$.next(selection.value.selectedRows[0].dataItem);
      } else {
        this.gruppiService.gruppo$.next(null);
      }
    });
  }

}

import {Injectable, Injector, Renderer2} from "@angular/core";
import {BaseCodeDescr} from "app/Model/baseClass/baseCodeDescr";
import {ProfilazioneUtentiService} from "app/profilazione/services/profilazione-utenti.service";
import {MasterService} from "app/Service/master.service";
import { CELL_TYPES } from 'gias-ui-kit';
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  EditingMode,
  HttpAction,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  PaginationSettings,
  RendererGridEvent,
  SelectableSettings
} from 'gias-kendo-grid';
import {from, map, Observable} from "rxjs";
import {GridPermessiEditServerResult} from "../grid-permessi-edit/grid-permessi-edit.service";
import {TipologieUtentiService} from "app/profilazione/services/profili-permessi/tipologie-utenti.service";

export class GridPermessiServerResult extends KendoServerResult {
  constructor(model: KendoGridModel, columns: KendoGridColumn[], rows: KendoGridRow[]) {
    super(model, columns, rows);
  }
}

@Injectable()
export class GridPermessiService extends AbstractGridConfigService<GridPermessiServerResult> {
  editingMode: EditingMode = EditingMode.IN_PAGE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = "Attivita_Cod";
  gridId = "PermessiGridID";
  users: any[] = [];

  private selectedProfile: BaseCodeDescr = new BaseCodeDescr(0);
  private isWriteColShown = false;

  private kendoModel = {
    Utente: new ModelEntry(CELL_TYPES.STRING),
    Tipologia_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    MenuPrimoLivello: new ModelEntry(CELL_TYPES.STRING),
    MenuSecondoLivello: new ModelEntry(CELL_TYPES.STRING),
    Attivita_Des: new ModelEntry(CELL_TYPES.STRING),
    Attivita_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    Id_Operazione: new ModelEntry(CELL_TYPES.NUMBER),
    Ordinamento: new ModelEntry(CELL_TYPES.STRING),
    Lettura: new ModelEntry(CELL_TYPES.STRING),
    Scrittura: new ModelEntry(CELL_TYPES.STRING),
  };
  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({field: 'MenuPrimoLivello', title: this.transloco.translate('Modulo')}),
    new KendoGridColumn({field: 'MenuSecondoLivello', title: this.transloco.translate('prof.Funzione')}),
    new KendoGridColumn({field: 'Attivita_Des', title: this.transloco.translate('prof.DescrizioneAutorizzazione')}),
    new KendoGridColumn({field: 'Attivita_Cod', title: this.transloco.translate('prof.IDPermesso')}, {
      width: 40,
      hidden: true
    }),
    new KendoGridColumn({field: 'Lettura', title: this.transloco.translate('PermessoLettura')}, {
      width: 160,
      style: {"text-align": "center"}
    }),
    new KendoGridColumn({field: 'Scrittura', title: this.transloco.translate('PermessoScrittura')}, {
      width: 160,
      style: {"text-align": "center"}
    })
  ];
  private rows = [];

  constructor(injector: Injector,
              private utentiService: ProfilazioneUtentiService,
              private profileService: TipologieUtentiService,
              private renderer: Renderer2,
              private master: MasterService) {
    super(injector);
    this.profileService.selectedProfile$.GiasSubscribe(prof => {
      if (prof) {
        this.selectedProfile = prof;
        this.gridPublicService.refresh(true);
      }
    });
    this.handleCustomizations();
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    let visibleRows: [] = this.getVisibleRows();
    let rows: [] = this.getGridRows();

    if (this.isWriteColShown &&
      this.rows.some(p => p.Funzioni_Scrittura === null)) {
      this.greyCells(this.renderer, visibleRows, rows);
    }
  }

  read(options?: any): Observable<GridPermessiServerResult> {
    this.master.set_isLoading({isLoading: true, component: this.gridPublicService.gridElRef});
    return this.utentiService.readPermessixTipologia(this.selectedProfile.codice)
      .pipe(map((data: any) => {
          this.rows = data.Permessi;
          // this.handleColVisibility();
          this.users = data.Utenti;
          this.master.set_isLoading({isLoading: false, component: this.gridPublicService.gridElRef});
          return new GridPermessiEditServerResult(this.rows, this.kendoColumns, this.kendoModel);
        })
      );
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return from([]);
  }

  private handleCustomizations() {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false
    });
    this.selectable.selectable = new SelectableSettings({
      enabled: true,
      checkboxOnly: false,
      mode: "single"
    });
    this.pagination = new PaginationSettings();
    this.pagination.gridState = {sort: [], skip: 0, take: 999, group: [{field: "MenuPrimoLivello"}]};
    this.pagination.pageable = {
      buttonCount: 4,
      info: true,
      type: 'input',
      pageSizes: [5, 8, 10, 25, 50, 100, 999, {
        text: this.transloco.translate('Tutti'),
        value: "all",
      } as any as number]
    };
    this.pagination.navigable = false;
  }

  private getVisibleRows() {
    return this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr');
  }

  private getGridRows() {
    return this.gridPublicService.gridComp.data['data'];
  }

  private greyCells(renderer: any, domElems: any[], elems: any[]) {
    let nRows = elems.length;
    for (let i = 0; i < nRows; i++) {
      if (elems[i].Funzioni_Scrittura === null) {
        let currentRow = domElems[i];
        for (let cell of currentRow.children) {
          if (cell.cellIndex === currentRow.children.length - 1
            || cell.cellIndex === currentRow.children.length - 2) {
            renderer.setStyle(cell, 'background-color', 'lightgrey');
          }
        }
      }
    }
  }

}

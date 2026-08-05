import { Injectable, Injector, Renderer2 } from "@angular/core";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { ProfilazioneUtentiService } from "app/profilazione/services/profilazione-utenti.service";
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
import { filter, from, map, Observable, of, Subscription, switchMap, takeUntil, tap } from "rxjs";
import { TipologieUtentiService } from "app/profilazione/services/profili-permessi/tipologie-utenti.service";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { MasterService } from "../../../Service/master.service";
import { UtentePermessoGerarchia } from "../../models/profili-permessi/UtentePermessoGerarchia.model";

export class GridPermessiEditServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class GridPermessiEditService extends AbstractGridConfigService<GridPermessiEditServerResult> {
  editingMode: EditingMode = EditingMode.IN_PAGE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = "Attivita_Cod";
  gridId = "PermessiGridID";

  public permessoEdit: boolean;
  public permessoRemove: boolean;
  public permessoInfo: boolean;

  private rows = [];
  private selectedProfile: BaseCodeDescr = new BaseCodeDescr(0);
  private activated: UtentePermessoGerarchia[] = [];

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
    new KendoGridColumn({ field: 'MenuPrimoLivello', title: this.transloco.translate('Modulo') }),
    new KendoGridColumn({ field: 'MenuSecondoLivello', title: this.transloco.translate('prof.Funzione') }),
    new KendoGridColumn({ field: 'Attivita_Des', title: this.transloco.translate('prof.DescrizioneAutorizzazione') }),
    new KendoGridColumn({ field: 'Attivita_Cod', title: this.transloco.translate('prof.IDPermesso') }, {
      width: 40,
      hidden: true
    }),
    new KendoGridColumn({ field: 'Lettura', title: this.transloco.translate('PermessoLettura') }, {
      width: 160,
      style: { "text-align": "center" }
    }),
    new KendoGridColumn({ field: 'Scrittura', title: this.transloco.translate('PermessoScrittura') }, {
      width: 160,
      style: { "text-align": "center" }
    })
  ];


  private tmp: Subscription = null;
  constructor(
    injector: Injector,
    private utentiService: ProfilazioneUtentiService,
    private profileService: TipologieUtentiService,
    private permessiService: PermessiUtenteService,
    private renderer: Renderer2,
    private master: MasterService
  ) {
    super(injector);
    this.permessoEdit = true;
    this.permessoInfo = true;
    this.permessoRemove = true;

    this.profileService.selectedProfile$.pipe(
      takeUntil(this.signal),
      filter(profile => !!profile),
      tap(profile => this.selectedProfile = profile)
    ).subscribe(() => this.gridPublicService.refresh(true));
    this.handleCustomization();
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    let visibleRows: [] = this.getVisibleRows();
    let rows: [] = this.getGridRows();

    if (!this.rows.every(p => p.Funzioni_Scrittura === null)) {
      this.greyCells(this.renderer, visibleRows, rows);
    }

  }

  read(options?: any): Observable<GridPermessiEditServerResult> {
    this.master.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
    return this.loadAllPermissions().pipe(
      switchMap(permissions => {
        this.rows = permissions;
        return this.utentiService.readPermessixTipologia(this.selectedProfile.codice);
      }),
      map(params => {
        this.activated = params.Permessi;
        this.rows.forEach((row) => {
          let permesso = this.activated.find((p) => row.Attivita_Cod === p.Attivita_Cod);
          row.MenuPrimoLivello = row.MenuPrimoLivello?.descrizione;
          row.MenuSecondoLivello = row.MenuSecondoLivello?.descrizione;
          row.Lettura = permesso ? permesso['Lettura'] : this.transloco.translate('No');
          row.Scrittura = permesso ? permesso['Scrittura'] : this.transloco.translate('No');
        });
        this.master.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
        return new GridPermessiEditServerResult(this.rows, this.kendoColumns, this.kendoModel);
      }));
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return from([]);
  }

  private handleCustomization() {
    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });
    this.selectable.selectable = new SelectableSettings({
      enabled: true, mode: "multiple", drag: false,
      checkboxOnly: true, metaKeyMultiSelect: true
    });
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.showSelectAll = true;
    this.selectable.columnSettings.width = 35;
    this.pagination = new PaginationSettings();
    this.pagination.navigable = false;
    this.pagination.gridState = { sort: [], skip: 0, take: 999, group: [{ field: "MenuPrimoLivello" }] };
    this.pagination.pageable = {
      buttonCount: 4,
      info: true,
      type: 'input',
      pageSizes: [5, 8, 10, 25, 50, 100, 999, {
        text: this.transloco.translate('Tutti'),
        value: "all",
      } as any as number]
    };
  }

  private loadAllPermissions(): Observable<any[]> {
    if (this.profileService.allPermissions.length)
      return of(this.profileService.allPermissions);
    return this.utentiService.readGerarchiaPermessiAttivi();
  }

  private getVisibleRows() {
    return this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr');
  }

  private getGridRows() {
    return this.gridPublicService.gridComp.data['data'];
  }

  private greyCells(renderer: any, domElems: any[], elems: any[]) {
    let nRows = elems.length;
    for (let index = 0; index < nRows; index++) {
      if (elems.at(index).Funzioni_Scrittura === null) {
        let currentRow = domElems[index];
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

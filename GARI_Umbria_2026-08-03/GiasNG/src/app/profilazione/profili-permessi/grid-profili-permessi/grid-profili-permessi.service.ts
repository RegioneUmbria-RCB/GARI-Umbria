import {Injectable, Injector, Renderer2} from "@angular/core";
import {BaseCodeDescr} from "app/Model/baseClass/baseCodeDescr";
import {ProfilazioneUtentiService} from "app/profilazione/services/profilazione-utenti.service";
import {CommandsColumnSettings, SelectableSettings} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  RendererGridEvent
} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {Observable, map, from, takeUntil, lastValueFrom} from "rxjs";
import {
  GridUtentiPermessiServerResult
} from "../../gestione-utenti/utenti/grid-utenti-permessi/grid-utenti-permessi.service";
import {TipologieUtentiService} from "../../services/profili-permessi/tipologie-utenti.service";
import {ProfilazioneDataShareService} from "../../services/profilazione-data-share.service";
import {UtentePermessoGerarchia} from "../../models/profili-permessi/UtentePermessoGerarchia.model";
import { MasterService } from "app/Service/master.service";


export class GridProfiliPermessiServerResult extends KendoServerResult {
  constructor(model: KendoGridModel, column: KendoGridColumn[], rows: KendoGridRow[]) {
    super(model, column, rows);
  }
}

@Injectable()
export class GridProfiliPermessiService extends AbstractGridConfigService<GridProfiliPermessiServerResult> {
  editingMode: EditingMode = EditingMode.IN_PAGE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = "rowId";
  gridId = "profiliPermessiGrid";
  private tipologie_permessi: { Tipologia: BaseCodeDescr; Permessi: UtentePermessoGerarchia[]; Utenti: string[] }[];

  private rows: any[] = [];
  private kendoModel = {
    Tipologia_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    Tipologia_Des: new ModelEntry(CELL_TYPES.STRING),
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
    new KendoGridColumn({field: 'Tipologia_Cod', title: this.transloco.translate('IDProfilo')}, {hidden: true}),
    new KendoGridColumn({field: 'Tipologia_Des', title: this.transloco.translate('Profilo')}, {hidden: false}),
    new KendoGridColumn({field: 'MenuPrimoLivello', title: this.transloco.translate('Modulo')}),
    new KendoGridColumn({field: 'MenuSecondoLivello', title: this.transloco.translate('prof.Funzione')}),
    new KendoGridColumn({field: 'Attivita_Des', title: this.transloco.translate('prof.DescrizioneAutorizzazione')}),
    new KendoGridColumn({field: 'Attivita_Cod', title: this.transloco.translate('prof.IDPermesso')}, {width: 40,hidden: true}),
    new KendoGridColumn({field: 'Lettura', title: this.transloco.translate('PermessoLettura')}, {width: 160}),
    new KendoGridColumn({field: 'Scrittura', title: this.transloco.translate('PermessoScrittura')}, {width: 160}),
  ];

  constructor(injector: Injector,
              private renderer: Renderer2,
              private master: MasterService,
              private utentiService: ProfilazioneUtentiService,
              private profilService: TipologieUtentiService,
              private datashare: ProfilazioneDataShareService,
  ) {
    super(injector);
    this.handleCustomization();
    this.handleEvents();
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    let visibleRows: [] = this.getVisibleRows();
    let rows: [] = this.getGridRows();
    if (!this.rows.every(p => p.Funzioni_Scrittura === null)) {
      this.greyCells(this.renderer, visibleRows, rows);
    }
  }

  read(options?: any): Observable<GridUtentiPermessiServerResult> {
    this.master.set_isLoading({isLoading: true, component: this.gridPublicService.gridElRef});
    return this.utentiService.readPermessixTipologia().pipe(map(result => {
      this.tipologie_permessi = result;
      this.rows = this.combineProfilePermissions();
      this.master.set_isLoading({isLoading: false, component: this.gridPublicService.gridElRef});
      return new GridUtentiPermessiServerResult(this.rows, this.kendoColumns, this.kendoModel);
    }));
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return from([]);
  }

  private combineProfilePermissions() {
    return this.tipologie_permessi.flatMap(t => t.Permessi.flatMap(p => ({
      Tipologia_Cod: t.Tipologia.codice,
      Tipologia_Des: t.Tipologia.descrizione,
      Attivita_Cod: p.Attivita_Cod,
      Attivita_Des: p.Attivita_Des,
      MenuPrimoLivello: p.MenuPrimoLivello.descrizione,
      MenuSecondoLivello: p.MenuSecondoLivello.descrizione,
      Id_Operazione: p.Id_Operazione,
      Ordinamento: p.Ordinamento,
      Lettura: (p.Id_Operazione === 0 || p.Id_Operazione === 2) ? this.transloco.translate('Si') : this.transloco.translate('No'),
      Scrittura: (p.Id_Operazione === 2) ? this.transloco.translate('Si') : this.transloco.translate('No')
    })));
  }

  private handleCustomization() {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false,
    });

    this.selectable.selectable = new SelectableSettings({
      enabled: false,
      checkboxOnly: false,
      drag: false,
    });
    this.selectable.shouldShowCheckbox = true;

  }

  private handleEvents() {
    this.profilService.selectedProfile$
      .pipe(takeUntil(lastValueFrom(this.datashare.exitProfilazione)))
      .subscribe(() => this.gridPublicService.refresh(true));
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

  private getVisibleRows() {
    return this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr');
  }

  private getGridRows() {
    return this.gridPublicService.gridComp.data['data'];
  }
}

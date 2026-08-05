import {Injectable, Injector, Renderer2} from "@angular/core";
import {ProfilazioneUtentiService} from "app/profilazione/services/profilazione-utenti.service";
import {enum_TipoPermesso} from "../../../models/profilazione.model";
import {CommandsColumnSettings, SelectableSettings} from 'gias-kendo-grid';
import { EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, RendererGridEvent} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {BehaviorSubject, filter, forkJoin, from, map, Observable, of, switchMap, take, takeUntil} from "rxjs";
import {TranslocoService} from "@jsverse/transloco";
import {PermessiUtenteService} from "../../../../Service/permessi-utente.service";
import {enum_Security_Attivita} from "../../../../Model/TipiEnumerativi";
import {MasterService} from "../../../../Service/master.service";
import {ProfilazioneDataShareService} from "../../../services/profilazione-data-share.service";
import {UtentePermessoGerarchia} from "../../../models/profili-permessi/UtentePermessoGerarchia.model";


export class GridUtentiPermessiServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class GridUtentiPermessiService extends AbstractGridConfigService<GridUtentiPermessiServerResult> {
  editingMode: EditingMode = EditingMode.IN_PAGE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = "rowId";
  gridId = "utentiPermessiGrid";

  public permessoEdit: boolean;
  public permessoRemove: boolean;
  public permessoInfo: boolean;

  private isTipologieLoaded$ = new BehaviorSubject(false);
  private tipologie_permessi = new Map<number, {
    desc: string;
    perms: UtentePermessoGerarchia[];
  }>();
  private users: {
    UserName: string;
    Tipologia_Cod: number;
  }[] = [];
  private readUsersOpts = {escludiSuperUser: true, mostraSoloAttivi: true};

  private $rows: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);
  private kendoModel = {
    Utente: new ModelEntry(CELL_TYPES.STRING),
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
    new KendoGridColumn({field: 'Utente', title: this.translocoService.translate('Utente')}, {hidden: false}),
    new KendoGridColumn({field: 'Tipologia_Cod', title: this.translocoService.translate('IDProfilo')}, {hidden: true}),
    new KendoGridColumn({field: 'Tipologia_Des', title: this.translocoService.translate('Profilo')}, {hidden: false}),
    new KendoGridColumn({field: 'MenuPrimoLivello', title: this.transloco.translate('Modulo')}),
    new KendoGridColumn({field: 'MenuSecondoLivello', title: this.transloco.translate('prof.Funzione')}),
    new KendoGridColumn({field: 'Attivita_Des', title: this.transloco.translate('prof.DescrizioneAutorizzazione')}),
    new KendoGridColumn({field: 'Attivita_Cod', title: this.transloco.translate('prof.IDPermesso')}, {width: 40,hidden: true}),
    new KendoGridColumn({field: 'Lettura', title: this.transloco.translate('PermessoLettura')}, {width: 160}),
    new KendoGridColumn({field: 'Scrittura', title: this.transloco.translate('PermessoScrittura')}, {width: 160}),
  ];

  constructor(injector: Injector,
              private master: MasterService,
              private renderer: Renderer2,
              private utentiService: ProfilazioneUtentiService,
              private permessiService: PermessiUtenteService,
              private translocoService: TranslocoService,
              private datashare: ProfilazioneDataShareService
  ) {
    super(injector);
    this.setPermissions();
    this.handleCustomization();
    this.handleEvents();
    this.utentiService.readPermessixTipologia().pipe(take(1)).subscribe(r => {
      r.forEach(t => this.tipologie_permessi.set(t.Tipologia.codice, {desc: t.Tipologia.descrizione, perms: t.Permessi}));
      this.isTipologieLoaded$.next(true);
    });
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    let visibleRows: [] = this.getVisibleRows();
    let rows: [] = this.getGridRows();
    if (!this.$rows.value.every(p => p.Funzioni_Scrittura === null)) {
      this.greyCells(this.renderer, visibleRows, rows);
    }
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return from([]);
  }

  /**
   * Default solo utenti attivi, esclude superuser.
   * @param options
   */
  read(options?: any): Observable<GridUtentiPermessiServerResult> {
    if (!this.datashare.usersGrid?.rows?.length) {
      return of(new GridUtentiPermessiServerResult([], this.kendoColumns, this.kendoModel));
    }
    this.startRead();
    return this.$rows.pipe(
      filter(rows => rows.length > 0),
      map(rows => new GridUtentiPermessiServerResult(rows, this.kendoColumns, this.kendoModel))
    );
  }

  private startRead() {
    this.master.set_isLoading({isLoading: true, component: this.gridPublicService.gridElRef});
    if (this.utentiService.$utenti && !this.utentiService.$utenti.value) {
      // ho premuto cerca dai filtri e sto aspettando la risposta della chiamata
      this.utentiService.$utenti.pipe(filter(res => !!res),take(1))
        .subscribe(utenti => {
          this.users = utenti.map(u => ({UserName: u['UserName'], Tipologia_Cod: u['Tipologia_Cod']}))
          this.endRead();
        });
    } else if (this.utentiService.$utenti && this.utentiService.$utenti.value) {
      // ho premuto cerca dai filtri e il caricamento è già finito
      this.users = this.utentiService.$utenti.value.map(u => ({UserName: u['UserName'], Tipologia_Cod: u['Tipologia_Cod']}));
      this.endRead();
    } else {
      // ho probabilmente aperto il panel dopo aver già caricato la griglia utenti
      this.users = this.datashare.usersGrid?.rows.map(u => ({UserName: u['UserName'], Tipologia_Cod: u['Tipologia_Cod']}));
      this.endRead();
    }
  }

  private endRead() {
    const end = () => {
      let rows = this.combineProfilePermissions();
      this.$rows.next(rows);
      this.master.set_isLoading({isLoading: false, component: this.gridPublicService.gridElRef});
    };
    if (!this.isTipologieLoaded$.value) {
      this.isTipologieLoaded$.pipe(filter(t => t), take(1)).subscribe(() => end());
    } else {
      end();
    }
  }

  private combineProfilePermissions() {
    let list = [];
    this.users.forEach(user => {
      if (this.tipologie_permessi.has(user.Tipologia_Cod)) {
        let profilo = this.tipologie_permessi.get(user.Tipologia_Cod);
        profilo.perms.forEach(p => {
          let permesso = {
            Utente: user.UserName,
            Tipologia_Cod: user.Tipologia_Cod,
            Tipologia_Des: profilo.desc,
            Attivita_Cod: p.Attivita_Cod,
            Attivita_Des: p.Attivita_Des,
            MenuPrimoLivello: p.MenuPrimoLivello.descrizione,
            MenuSecondoLivello: p.MenuSecondoLivello.descrizione,
            Id_Operazione: p.Id_Operazione,
            Ordinamento: p.Ordinamento,
            Lettura: (p.Id_Operazione === 0 || p.Id_Operazione === 2) ? this.transloco.translate('Si') : this.transloco.translate('No'),
            Scrittura: (p.Id_Operazione === 2) ? this.transloco.translate('Si') : this.transloco.translate('No')
          };
          list.push(permesso);
        });
      }
    });
    return list;
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

  private setPermissions() {
    this.permessoRemove = true;
    this.permessoInfo = this.permessiService.getPermesso(enum_Security_Attivita.Profilazione_NG, enum_TipoPermesso.LETTURA)
      && this.permessiService.getPermesso(enum_Security_Attivita.Gest_UtentiProfili, enum_TipoPermesso.LETTURA);
    this.permessoEdit = this.permessoInfo
      && this.permessiService.getPermesso(enum_Security_Attivita.Profilazione_NG, enum_TipoPermesso.LETTURA_SCRITTURA);
  }

  private handleEvents() {
    this.utentiService.userReloaded$
      .pipe(takeUntil(this.signal), filter(reload => !!reload))
      .subscribe((reloaded: boolean) => this.gridPublicService.refresh(reloaded));
  }

  private getVisibleRows() {
    return this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr');
  }

  private getGridRows() {
    return this.gridPublicService.gridComp.data['data'];
  }

}

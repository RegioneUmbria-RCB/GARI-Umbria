import { Injectable, Injector } from '@angular/core';
import { MasterService } from 'app/Service/master.service';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  EditingMode,
  GridPublicService,
  HttpAction,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  PaginationSettings,
  RendererGridEvent,
  ToolbarSettings
} from 'gias-kendo-grid';
import { ClientePermesso, ProfilazioneUtentiService } from 'app/profilazione/services/profilazione-utenti.service';
import { TipologieUtentiService } from 'app/profilazione/services/profili-permessi/tipologie-utenti.service';
import { BehaviorSubject, forkJoin, from, map, Observable, Subscription, takeUntil, tap } from 'rxjs';
import { UtentePermessoGerarchia } from "../../models/profili-permessi/UtentePermessoGerarchia.model";
import { enum_TipoPermesso } from "../../models/profilazione.model";
import { FormGroup } from "@angular/forms";
import { DataResult, GroupDescriptor, process, SortDescriptor } from "@progress/kendo-data-query";
import { PERMS_CANNOT_BE_DISABLED } from "./cliente-permessi-grid.component";
import { CompositeFilterDescriptor } from "@progress/kendo-data-query/dist/npm/filtering/filter-descriptor.interface";

export class ClientePermessiGridServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class ClientePermessiGridService extends AbstractGridConfigService<ClientePermessiGridServerResult> {

  editingMode: EditingMode = EditingMode.IN_LINE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'codice';
  gridId = 'ClientePermessiGridID';

  private _prevEditingVal = false;
  private _isEditing$ = new BehaviorSubject<boolean>(false);
  private _gridStateChangeSub: Subscription = null;
  private _gridSort: SortDescriptor[] = [];
  private _gridFilter: CompositeFilterDescriptor | undefined = undefined;
  private _gridGroup: GroupDescriptor[] = [{ field: "modulo" }];
  private gridForm: BehaviorSubject<FormGroup>;
  private tuttiPermessi = [];

  private kendoRows = [];
  private kendoModel = {
    codice: new ModelEntry(CELL_TYPES.NUMBER),
    modulo: new ModelEntry(CELL_TYPES.STRING),
    funzione: new ModelEntry(CELL_TYPES.STRING),
    attivitadesc: new ModelEntry(CELL_TYPES.STRING),
    id_operazione: new ModelEntry(CELL_TYPES.NUMBER),
    permessolettura: new ModelEntry(CELL_TYPES.STRING),
    permessoscrittura: new ModelEntry(CELL_TYPES.STRING)
  };
  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'modulo', title: this.transloco.translate('Modulo') }, { resizable: true }),
    new KendoGridColumn({ field: 'funzione', title: this.transloco.translate('Funzione') }, { resizable: true }),
    new KendoGridColumn({ field: 'codice', title: this.transloco.translate('Codice') }, { resizable: true, hidden: true }),
    new KendoGridColumn({
      field: 'attivitadesc',
      title: this.transloco.translate('DescrizioneAutorizzazione')
    }, { resizable: true }),
    new KendoGridColumn({
      field: 'permessolettura',
      title: this.transloco.translate('PermessoLettura')
    }, { resizable: true }),
    new KendoGridColumn({
      field: 'permessoscrittura',
      title: this.transloco.translate('PermessoScrittura')
    }, { resizable: true })
  ];

  constructor(
    injector: Injector,
    private masterService: MasterService,
    public gridPublicService: GridPublicService,
    private utentiService: ProfilazioneUtentiService,
    private tipologieUtentiService: TipologieUtentiService
  ) {
    super(injector);
    this.gridForm = this.gridPublicService.formGroup;
    this.handleCustomization();
    this.handleEvents();
  }

  private get isEditing(): boolean {
    return this._isEditing$.getValue();
  }

  private set isEditing(value: boolean) {
    this._prevEditingVal = this._isEditing$.value;
    this._isEditing$.next(value);
  }

  read(): Observable<ClientePermessiGridServerResult> {
    this.masterService.set_isLoading({ isLoading: true, message: '' });
    return forkJoin([
      this.utentiService.readGerarchiaPermessi(),
      this.utentiService.leggiClientePermessi(this.masterService.objP_utenti.UtenteUsername)
    ]).pipe(map(result => {
      this.tuttiPermessi = result[0].Permessi;
      let permessiAbilitati = [];
      for (let t of result[1] as ClientePermesso[]) {
        let permesso: UtentePermessoGerarchia = this.tuttiPermessi.find(r => r.Attivita_Cod == t.Id_Attivita);
        if (permesso) permessiAbilitati.push(this.formattaPerGrid(t, permesso));
      }
      let permessiNonAbilitati = [];
      if (this.isEditing) {
        permessiNonAbilitati = this.tuttiPermessi.filter(permesso => !permessiAbilitati.filter(el => el.codice === permesso.Attivita_Cod).length)
          .map(t => {
            const template = new ClientePermesso(t.Attivita_Cod, enum_TipoPermesso.DISABILITATO);
            return this.formattaPerGrid(template, t);
          });
      }
      this.kendoRows = permessiAbilitati.concat(permessiNonAbilitati);
      this.masterService.set_isLoading({ isLoading: false, message: '' });
      return new ClientePermessiGridServerResult(this.kendoRows, this.kendoColumns, this.kendoModel);
    }));
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return from([]);
  }

  applyRendererRules(opts: RendererGridEvent) {
    super.applyRendererRules(opts);
    if (this.gridPublicService.gridComp && this._gridStateChangeSub == null) {
      this.handleCheckboxHide(); // set subscription to grid state change
    }
    // when entering in editing mode checkbox are shown
    if (this.isEditing && !this._prevEditingVal) {
      this.hideCheckbox();
      this._prevEditingVal = this.isEditing;
    }
  }

  private handleCheckboxHide() {
    console.log('subscribing');
    this._gridStateChangeSub = this.gridPublicService.gridComp.dataStateChange
      .pipe(
        takeUntil(this.signal),
        tap(stateChange => this._gridSort = stateChange.sort),
        tap(stateChange => this._gridFilter = stateChange.filter),
        tap(stateChange => this._gridGroup = stateChange.group)
      ).subscribe(() => this.hideCheckbox());
  }

  private hideCheckbox() {
    const dataResult: DataResult = process(this.kendoRows, {
      group: this._gridGroup,
      sort: this._gridSort,
      filter: this._gridFilter
    });
    let sortedRows: any[];
    if (this._gridGroup.length === 0) {
      sortedRows = dataResult.data;
    } else {
      sortedRows = dataResult.data.flatMap(gr => gr.items);
    }
    console.log('statechange', sortedRows);
    const domgrid = this.gridPublicService.gridElRef.nativeElement.querySelectorAll("tbody tr.k-master-row");
    for (let i = 0; i < domgrid.length; i++) {
      let checkbox = domgrid[i].querySelector(".k-checkbox");
      if (!!checkbox) {
        checkbox.style.display = this.permissionIsNotEditable(sortedRows[i]) ? "none" : "block";
      }
    }
  }

  private permissionIsNotEditable(row): boolean {
    return PERMS_CANNOT_BE_DISABLED.includes(row.codice);
  }

  private formattaPerGrid(attivo: ClientePermesso, permessoGerarchia: UtentePermessoGerarchia) {
    return {
      codice: attivo.Id_Attivita,
      modulo: permessoGerarchia.MenuPrimoLivello?.descrizione ?? '',
      funzione: permessoGerarchia.MenuSecondoLivello?.descrizione ?? '',
      attivitadesc: permessoGerarchia.Attivita_Des,
      id_operazione: attivo.Id_Operazione,
      permessolettura: attivo.Id_Operazione == 0 || attivo.Id_Operazione == 2
        ? this.transloco.translate('Si') : this.transloco.translate('No'),
      permessoscrittura: attivo.Id_Operazione == 2
        ? this.transloco.translate('Si') : this.transloco.translate('No')
    };
  }

  private handleCustomization() {
    this.cmdColumn = new CommandsColumnSettings({
      infoBtn: false,
      editBtn: false,
      removeBtn: false,
    });
    this.pagination = this.handlePagination();
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.groups.groupable.enabled = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.showSelectAll = true;
    this.selectable.selectable.enabled = this.isEditing;
    this.selectable.selectable.mode = "multiple";
    this.selectable.selectable.drag = false;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.metaKeyMultiSelect = true;
  }

  private handleEvents() {
    this.tipologieUtentiService.isEditing.pipe(takeUntil(this.signal))
      .subscribe((editing: boolean) => {
        this.isEditing = editing;
        this.gridPublicService.refresh(true);
        this.handleCustomization();
      });
  }

  private permessoGerarchia2row(r: UtentePermessoGerarchia) {
    return {
      codice: r.Attivita_Cod,
      modulo: r.MenuPrimoLivello.descrizione,
      funzione: r.MenuSecondoLivello.descrizione,
      attivitadesc: r.Attivita_Des,
      permessolettura: r.Id_Operazione == 0 || r.Id_Operazione == 2
        ? this.transloco.translate("Si") : this.transloco.translate("No"),
      permessoscrittura: r.Id_Operazione == 2
        ? this.transloco.translate("Si") : this.transloco.translate("No")
    };
  }

  private handlePagination(): PaginationSettings {
    const result: PaginationSettings = new PaginationSettings();
    result.gridState = {
      sort: [],
      skip: 0,
      take: 999,
      group: this._gridGroup
    };
    result.pageable = {
      buttonCount: 4,
      info: true,
      type: 'input',
      pageSizes: [5, 8, 10, 25, 50, 100, 999, {
        text: this.transloco.translate('Tutti'),
        value: "all",
      } as any as number]
    };
    result.navigable = false;
    return result;
  }
}

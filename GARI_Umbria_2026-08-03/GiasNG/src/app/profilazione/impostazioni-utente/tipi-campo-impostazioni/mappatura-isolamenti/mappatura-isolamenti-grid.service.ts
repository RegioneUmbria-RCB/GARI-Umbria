import { Injectable, Injector } from '@angular/core';
import { FormGroup } from "@angular/forms";
import {
  AbstractGridConfigService, HttpAction, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow,
  KendoServerResult, LoaderType, ModelEntry, DropdownListItem, DropdownListWithForm, CommandsColumnSettings,
  ToolbarSettings, RendererGridEvent, PaginationSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { filter, from, Observable, tap, map, of, takeUntil, switchMap } from "rxjs";
import { MappaturaIsolamentiService } from './mappatura-isolamenti.service';
import { ProfilazioneUtentiService } from 'app/profilazione/services/profilazione-utenti.service';

class MappaturaIsolamentiServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class MappaturaIsolamentiGridService extends AbstractGridConfigService<MappaturaIsolamentiServerResult> {
  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = "Elem_Cod";
  gridId = 'gridCategorieMagazzinoID';

  private users: DropdownListItem[] = [];

  private kendoRows = [];
  private kendoColumns = [
    new KendoGridColumn({ field: 'UserName', title: this.transloco.translate('Username') }, { editable: false, }),
    new KendoGridColumn({ field: 'Nome', title: this.transloco.translate('Nome') }, { editable: false, }),
    new KendoGridColumn({ field: 'Cognome', title: this.transloco.translate('Cognome') }, { editable: false, }),
    new KendoGridColumn({ field: 'id_specie', title: this.transloco.translate('SpecieVegetali') }, { editable: true }),
  ];
  private kendoModel: KendoGridModel = {
    UserName: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    Nome: new ModelEntry(CELL_TYPES.STRING),
    Cognome: new ModelEntry(CELL_TYPES.STRING),
    specie_des: new ModelEntry(CELL_TYPES.STRING),
    id_specie: new ModelEntry(CELL_TYPES.MULTI_DROPDOWNLIST),
  };

  constructor(
    injector: Injector,
    private mappaturaIsolamentiService: MappaturaIsolamentiService,
    private profilazioneUtentiService: ProfilazioneUtentiService,
  ) {
    super(injector);
    this.customizeGrid();
    this.handleEvents();
  }

  private get usersNotInGrid(): any[] {
    let inGrid: string[] = this.kendoRows?.map(r => r.UserName);
    return this.users.filter(u => !inGrid.includes(u.id));
  }

  override applyRendererRules(opts?: RendererGridEvent): void {
    let btnAdd = document.querySelector("kendo-grid-toolbar button.k-grid-add-command") as HTMLButtonElement;
    btnAdd.disabled = this.usersNotInGrid.length <= 0;
  }

  read(options?: any): Observable<MappaturaIsolamentiServerResult> {
    return this.mappaturaIsolamentiService.loadUsersClasses().pipe(
      takeUntil(this.signal),
      tap(userClasses => this.kendoRows = userClasses),
      map(userClasses => new MappaturaIsolamentiServerResult(userClasses, this.kendoColumns, this.kendoModel))
    );
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return from([]);
  }

  private handleEvents() {
    this.gridPublicService.changeDetected.pipe(takeUntil(this.signal))
      .subscribe((ch) => {
        switch (ch["action"]) {
          case "add":
            this.startAdding(true); break;
          case "cancel":
            this.startAdding(false); break;
          case "save":
            this.startAdding(false);
            this.internalSave(ch["dataItem"]);
            break;
          case "remove":
            ch["dataItem"].id_specie = [];
            this.internalSave(ch["dataItem"]);
            break;
        }
      });
    this.onCellClose = (event) => this.internalSave(event.dataItem);

    let rowFormGroup: FormGroup;
    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      filter(fg => fg != null),
      tap(fg => rowFormGroup = fg),
      switchMap((fg: FormGroup) => fg.get("UserName").valueChanges)
    ).subscribe(ddlItem => {
      let userData = this.usersNotInGrid.find(u => u.id == ddlItem).data;
      if (userData.Nome == "" && userData.Cognome == "") {
        rowFormGroup.get("Cognome").patchValue(userData.RagioneSociale);
      } else {
        rowFormGroup.get("Nome").patchValue(userData.Nome);
        rowFormGroup.get("Cognome").patchValue(userData.Cognome);
      }
    });
  }

  private startAdding(start: boolean) {
    const col = this.kendoColumns.find(s => s.field === 'UserName');
    col.editable = start;
  }

  private internalSave(dataItem) {
    if (dataItem.id_specie.includes(0)) {
      const col = this.kendoColumns.find(s => s.field === 'id_specie');
      dataItem.id_specie = col.ddl.data.map(d => d.id).filter(id => id != 0);
      dataItem.specie_des = col.ddl.data.map(d => d.name).filter(d => d != "").join(" | ");
    }
    this.mappaturaIsolamentiService.saveUserClassSpecies(dataItem)
      .subscribe(() => this.gridPublicService.refresh(true));
  }

  private customizeGrid() {
    this.generalSettings.performOnEdit = true;
    this.configureSpeciesClassDDL();
    this.configureUsersDDL();
    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = true;
    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: true });
    this.pagination.gridState.take = 100;
  }

  private configureSpeciesClassDDL() {
    const col = this.kendoColumns.find(s => s.field === 'id_specie');
    this.mappaturaIsolamentiService.speciesClasses.pipe(
      takeUntil(this.signal),
      filter(classes => classes != null),
    ).subscribe(result => {
      const data = result.map(sc => new DropdownListItem(sc.codice, sc.descrizione));
      if (result.find(x => x.codice == 0) == null) {
        data.unshift(new DropdownListItem(0, this.transloco.translate('TutteLeSpecie')));
      }
      col.ddl = new DropdownListWithForm('codice', 'id_specie', 'descrizione', data);
      col.ddl.descriptionField = 'specie_des';
    });
  }

  private configureUsersDDL() {
    const col = this.kendoColumns.find(s => s.field === 'UserName');
    this.profilazioneUtentiService.readUtentiDatiBase(true).pipe(
      takeUntil(this.signal),
      filter(users => users != null),
      map(users => users.map(user => new DropdownListItem(user.UserName, user.UserName, user))),
      tap(users => this.users = users)
    ).subscribe(() => {
      col.ddl = new DropdownListWithForm('id', 'UserName', 'name', []);
      col.ddl.descriptionField = 'UserName';
      col.ddl.valuePrimitive = true;
      col.ddl.loadOnEdit = true;
      col.ddl.loadFunction = this.loadUsersForDDL.bind(this);
    });
  }

  private loadUsersForDDL(): Observable<DropdownListItem[]> {
    let inGrid: string[] = this.kendoRows.map(r => r.UserName);
    let toShow: DropdownListItem[] = this.users.filter(u => !inGrid.includes(u.id));
    return of(toShow);
  }
}

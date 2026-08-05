import { Injectable, Injector } from '@angular/core';
import { FormGroup } from "@angular/forms";
import {
  AbstractGridConfigService, HttpAction, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow,
  KendoServerResult, LoaderType, ModelEntry, DropdownListItem, DropdownListWithForm, CommandsColumnSettings,
  ToolbarSettings, RendererGridEvent, PaginationSettings, GridCustomizations
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { filter, from, Observable, tap, map, of, takeUntil, switchMap } from "rxjs";
import { DocumentaryAuditService } from '../documentary-audit.service';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';

class ServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

class ChecklistData {
  public services: BaseCodeDescr[];
  public auditTypes: BaseCodeDescr[];
  public settingValue: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChecklistGridService extends AbstractGridConfigService<ServerResult> {
  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = "auditTypeCod";
  gridId = 'ChecklistGrid';

  private _services: DropdownListItem[] = [];
  private _auditTypes: DropdownListItem[] = [];
  private kendoRows = [];
  private kendoColumns = [
    new KendoGridColumn({ field: 'serviceCod', title: this.transloco.translate('Servizio') }, { editable: true, }),
    new KendoGridColumn({ field: 'auditTypeCod', title: this.transloco.translate('TipoAuditGestione') }, { editable: true, }),
  ];
  private kendoModel: KendoGridModel = {
    serviceCod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    serviceDes: new ModelEntry(CELL_TYPES.STRING),
    auditTypeCod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    auditTypeDes: new ModelEntry(CELL_TYPES.STRING),
  };

  constructor(
    injector: Injector,
    private documentaryAuditService: DocumentaryAuditService
  ) {
    super(injector);
    this.customizeGrid();
    this.handleEvents();
    this.documentaryAuditService.GetChecklistMamagementData()
      .pipe(
        tap((data) => this.configureDDLs(data)),
        map((data) => this.deserializeSettingValue(data.settingValue)),
      ).subscribe(() => this.gridPublicService.refresh(true, new ServerResult(this.kendoRows, this.kendoColumns, this.kendoModel)));
  }

  read(options?: any): Observable<ServerResult> {
    return of(new ServerResult(this.kendoRows, this.kendoColumns, this.kendoModel));
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return from([]);
  }

  private handleEvents() {
    this.gridPublicService.changeDetected.pipe(takeUntil(this.signal))
      .subscribe((ch) => {
        switch (ch["action"]) {
          case "save":
            this.internalSave(ch["dataItem"]);
            break;
          case "remove":
            ch["dataItem"].id_specie = [];
            this.internalSave(ch["dataItem"]);
            break;
        }
      });
    this.onCellClose = (event) => this.internalSave(event.dataItem);
  }

  private internalSave(dataItem) {
    const newValue = this.kendoRows.map(r => `${r.auditTypeCod}_${r.serviceCod}`).join('|') ?? '';
    this.documentaryAuditService.SaveChecklistManagement(newValue).subscribe();
  }

  private customizeGrid() {
    this.generalSettings.performOnEdit = true;
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = true;
    this.groups.groupable.enabled = false;
    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: true });
    this.pagination.gridState.take = 100;
    this.views = new GridCustomizations({ enabled: false });
  }

  private configureDDLs(data: ChecklistData) {
    let col = this.kendoColumns.find(s => s.field === 'serviceCod');
    this._services = data.services.map(x => new DropdownListItem(x.codice, x.descrizione));
    col.ddl = new DropdownListWithForm('id', 'serviceCod', 'name', this._services);
    col.ddl.descriptionField = 'serviceDes';
    col.ddl.valuePrimitive = true;
    // col.ddl.loadOnEdit = true;
    // col.ddl.loadFunction = this.getAvailableServices.bind(this);

    col = this.kendoColumns.find(s => s.field === 'auditTypeCod');
    this._auditTypes = data.auditTypes.map(x => new DropdownListItem(x.codice, x.descrizione));
    col.ddl = new DropdownListWithForm('id', 'auditTypeCod', 'name', this._auditTypes);
    col.ddl.descriptionField = 'auditTypeDes';
    col.ddl.valuePrimitive = true;
    // col.ddl.loadOnEdit = true;
    // col.ddl.loadFunction = this.getAvailableAudit.bind(this);
  }

  private getAvailableServices(): Observable<DropdownListItem[]> {
    return of(this._services.filter(s => !this.kendoRows.map(r => r.serviceCod).includes(s.id)));
  }

  private getAvailableAudit(): Observable<DropdownListItem[]> {
    return of(this._auditTypes.filter(s => !this.kendoRows.map(r => r.auditTypeCod).includes(s.id)));
  }

  private deserializeSettingValue(value: string) {
    const rows = value.split('|').map(row => ({
      auditTypeCod: +row.split('_')[0],
      auditTypeDes: this.transloco.translate('NonInLista'),
      serviceCod: +row.split('_')[1],
      serviceDes: this.transloco.translate('NonInLista'),
    }));
    rows.forEach(r => {
      if (this._services.find(s => +s.id == +r.serviceCod) == undefined)
        this._services.push(new DropdownListItem(r.serviceCod, this.transloco.translate('NonInLista') + ` (${r.serviceCod})`));

      if (this._auditTypes.find(a => +a.id == +r.auditTypeCod) == undefined)
        this._auditTypes.push(new DropdownListItem(r.auditTypeCod, this.transloco.translate('NonInLista') + ` (${r.auditTypeCod})`));
    });
    rows.forEach(r => {
      r.serviceDes = this._services.find(s => +s.id == +r.serviceCod)?.name;
      r.auditTypeDes = this._auditTypes.find(a => +a.id == +r.auditTypeCod)?.name;
    });
    this.kendoRows = rows;
  }
}

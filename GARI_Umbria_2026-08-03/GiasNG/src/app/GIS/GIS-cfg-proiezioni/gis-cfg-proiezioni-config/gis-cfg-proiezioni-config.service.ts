import {Injectable, Injector} from '@angular/core';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode,
  KendoGridColumn,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import {Observable, of} from 'rxjs';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {GisCfgProiezioniConfigModel, GisCfgProiezioniConfigServerResult} from './gis-cfg-proiezioni-config.model';
import {ConfigTemplate} from 'gias-kendo-grid';
import {
  AgrSelectableSettings,
  CommandsColumnSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import {SMARTPHONE_WIDTH} from '../../../Model/CostantiPersonalizzate';
import {GISCfgProiezioniConfigDataService} from './gis-cfg-proiezioni-config-data.service';
import {AbstractControl, ValidatorFn, Validators} from '@angular/forms';

@Injectable()
export class GISCfgProiezioniConfigService extends AbstractGridConfigService<GisCfgProiezioniConfigServerResult> {
  editingMode: EditingMode = EditingMode.IN_LINE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = 'key';
  gridId: string = 'CfgConfig';

  private avoidWhiteSpaceValidator: ValidatorFn = (control: AbstractControl) => {
    if (control?.value?.toString() !== '') {
      return control?.value?.toString().lastIndexOf(' ') !== -1 ? {'whiteSpace': true} : null;
    } else {
      return null;
    }
  }

  private gridColumns: KendoGridColumn[] = [
    new KendoGridColumn({field: 'key', title: this.transloco.translate('Chiave')}, {validators: [this.avoidWhiteSpaceValidator]}),
    new KendoGridColumn({field: 'value', title: this.transloco.translate('Valore')}, {}),
  ];

  private gridModel: GisCfgProiezioniConfigModel = {
    key: new ModelEntry(CELL_TYPES.STRING, false),
    value: new ModelEntry(CELL_TYPES.STRING, false),
  }

  private cfgArray: any[] = [];

  constructor(
    injector: Injector,
    private gisCfgProiezioniConfigDataService: GISCfgProiezioniConfigDataService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.handleCustomizations();
  }
  read(options?: any): Observable<GisCfgProiezioniConfigServerResult> {
    this.cfgArray = this.getRowsFromCfgJson();
    return of(new GisCfgProiezioniConfigServerResult(this.cfgArray, this.gridColumns, this.gridModel));
  }
  perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
    switch (actionType) {
      case HttpAction.UPDATE:
        this.update(items, oldRow);
        break;
      case HttpAction.CREATE:
        this.create(items);
        break;
      case HttpAction.REMOVE:
        this.remove(items);
        break;
    }

    this.gisCfgProiezioniConfigDataService.cfg = this.composeCfgJsonString();
    return of([]);
  }

  private getRowsFromCfgJson(): any[] {
    let cfgObj: any;
    let rows: any[] = [];
    let cfgProiezione: string = this.gisCfgProiezioniConfigDataService.cfg;

    if (cfgProiezione != undefined && cfgProiezione.trim() !== '') {
      cfgObj = JSON.parse(cfgProiezione?.trim() ?? '');
    }

    if (cfgObj != undefined) {
      Object.keys(cfgObj).forEach(
        function (key: string, idx: number): void {
          rows.push({key: key, value: cfgObj[key]});
        }
      );
    }
    return rows;
  }

  private update(dataItem: any, oldRow: any): void {
    this.cfgArray.splice(this.cfgArray.findIndex(el => el['key'] === oldRow['key']), 1, dataItem);
  }

  private create(dataItem: any): void {
    this.cfgArray.push({key: dataItem['key'], value: dataItem['value']});
  }

  private remove(dataItem: any): void {
    this.cfgArray.splice(this.cfgArray.findIndex(el => el['key'] === dataItem['key']), 1);
  }

  private composeCfgJsonString(): string {
    if (this.cfgArray.length > 0) {
      let cfgObj = this.cfgArray.reduce((prev, curr, idx, array) => {
        prev[curr['key']] = curr['value'];
        return prev;
      }, {});
      return JSON.stringify(cfgObj ?? {});
    } else {
      return '';
    }
  }

  private handleCustomizations(): void {
    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = false;
    this.selectable.selectable.enabled = false;
    this.selectable.shouldShowCheckbox = false;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = true;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: true,
      infoBtn: false,
      removeBtn: true,
      onDisableInfoBtn: () => false
    });

    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;

    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.toolbar.newItem = true;
      this.cmdColumn.editBtn = true;
    }
  }
}

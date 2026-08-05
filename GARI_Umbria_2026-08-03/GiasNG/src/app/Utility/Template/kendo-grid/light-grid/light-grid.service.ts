import {Injectable, Injector} from '@angular/core';
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  EditingMode, ExcelSettings, GridCustomizations,
  HttpAction, KendoGridColumn, KendoGridModel,
  KendoServerResult,
  LoaderType,
  ToolbarSettings
} from 'gias-kendo-grid';
import {Observable, ReplaySubject, skip} from 'rxjs';

export class LightGridServerResult<T> extends KendoServerResult{
  constructor(columns: KendoGridColumn[], model: KendoGridModel, rows: T[]) {
    super(model, columns, rows);
  }
}

@Injectable()
export class LightGridService<T extends unknown[]> extends AbstractGridConfigService<LightGridServerResult<T>> {
  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  gridId: string = 'light-grid';
  rowId: string = 'key';

  constructor(
    injector: Injector,
    private codesData: LightGridDataService<T>
  ) {
    super(injector);
    this.handleCustomization();
    this.codesData.data.pipe(skip(1)).subscribe((value) => {
      this.gridPublicService.refresh(true, new LightGridServerResult(this.codesData.columns, this.codesData.model, value));
    });
  }

  read(options: any): Observable<KendoServerResult> {
    return this.codesData.read(options);
  }

  perform(action: HttpAction, items: any): Observable<any[]> {
    return this.codesData.perform(action, items);
  }

  private handleCustomization(): void {
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      showCmdColumn: () => false
    });

    this.resizable.autoFitColumns = false;
    this.resizable.isResizable = true;

    this.groups.groupable.enabled = false;
    this.views = new GridCustomizations();

    this.behavior.excelSettings = new ExcelSettings({enabled: false});
  }
}

@Injectable()
export class LightGridDataService<T> {
  get columns(): KendoGridColumn[] {
    if (this._columns == undefined) {
      throw new Error('columns not valued');
    }
    return this._columns;
  }

  set columns(value: KendoGridColumn[]) {
    this._columns = value;
  }

  get model(): KendoGridModel {
    if (this._model == undefined) {
      throw new Error('model not valued');
    }
    return this._model;
  }

  set model(value: KendoGridModel) {
    this._model = value;
  }

  data: ReplaySubject<T> = new ReplaySubject<T>(1);

  customization: (...args) => void;
  read: (options: any) => Observable<KendoServerResult>;
  perform: (action: HttpAction, items: any) => Observable<any[]>;

  private _model: KendoGridModel;
  private _columns: KendoGridColumn[];
}

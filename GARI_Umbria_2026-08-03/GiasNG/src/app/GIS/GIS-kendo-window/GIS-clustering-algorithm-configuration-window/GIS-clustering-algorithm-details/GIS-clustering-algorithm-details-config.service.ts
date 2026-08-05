import { Injectable, Injector } from '@angular/core';
import { map, Observable, of, ReplaySubject, startWith, Subject, tap } from 'rxjs';
import { EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';

export class GISClusteringAlgorithmDetailsResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class GISClusteringAlgorithmDetailsGridModel extends KendoGridModel {
  name: ModelEntry;
  value: ModelEntry;
}


@Injectable()
export class GISClusteringAlgorithmDetailsGridConfigService extends AbstractGridConfigService<GISClusteringAlgorithmDetailsResult> {
  gridId = 'GISClusteringAlgorithmDetailsGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'name';

  inputData = new ReplaySubject<string>();
  outputData = new Subject<NameValue[]>();

  private currentData: NameValue[] = [];

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'name', title: this.transloco.translate('gis.NomeParametro') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 150,
      },
    ),
    new KendoGridColumn(
      { field: 'value', title: this.transloco.translate('gis.ValoreParametro') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 150,
      },
    )
  ];

  model: GISClusteringAlgorithmDetailsGridModel = {
    name: new ModelEntry(CELL_TYPES.STRING),
    value: new ModelEntry(CELL_TYPES.STRING),
  };

  constructor(
    protected injector: Injector,
    protected transloco: TranslocoService,
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings(true);
    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: true,
      infoBtn: false,
      removeBtn: true,
    });

    this.behavior.excelSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
  }

  public read(): Observable<GISClusteringAlgorithmDetailsResult> {
    return this.inputData.asObservable()
      .pipe(
        map(data => GISClusteringAlgorithmDetailsGridConfigService.parseParameters(data)),
        startWith([]),
        tap(data => this.currentData = data),
        map(data => new GISClusteringAlgorithmDetailsResult(
          data,
          this.columns,
          this.model)
        )
      );
  }

  public perform(actionType: HttpAction, data: NameValue): Observable<KendoGridRow[]> {
    if (actionType == HttpAction.CREATE) {
      this.currentData.push(data);
    } else if (actionType == HttpAction.UPDATE) {
      const index = this.currentData.findIndex(item => item.name === data.name);
      if (index !== -1) {
        this.currentData[index] = data;
      }
    } else if (actionType == HttpAction.REMOVE) {
      this.currentData = this.currentData.filter(item => item.name !== data.name);
    }

    this.outputData.next(this.currentData);
    return of([data]);
  }

  static parseParameters(parameters: string): NameValue[] {
    try {
      const result = JSON.parse(parameters);
      return result;
    } catch (e) {
      return [];
    }
  }
}

interface NameValue {
  name: string;
  value: string;
}
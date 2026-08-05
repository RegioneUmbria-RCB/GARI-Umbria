import { Injectable, Injector } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { DefaultDistintaProduzione, ProfilazioneImpreseService } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { DistintaProduzione_In, DistintaProduzioneItem, DistintaProduzioneValue } from 'app/Service/net-core6-api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { catchError, combineLatest, map, Observable, of, startWith, Subject, switchMap, tap, withLatestFrom } from 'rxjs';

export class ProfilazioneImpreseDefaultDistintaProduzioneGridResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}
export class ProfilazioneImpreseDefaultDistintaProduzioneGridModel extends KendoGridModel {
  PosId: ModelEntry;
  Veg_Cod: ModelEntry;
  Cul_Cod: ModelEntry;
  Veg_Des: ModelEntry;
  Cul_des: ModelEntry;
}

@Injectable()
export class ProfilazioneImpreseDefaultDistintaProduzioneGridConfigService extends AbstractGridConfigService<ProfilazioneImpreseDefaultDistintaProduzioneGridResult> {
  gridId = 'ProfilazioneImpreseDefaultDistintaProduzioneGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE_BATCH;
  rowId = 'Cul_Cod';

  private seminaField = 'semina_';
  private fiorituraField = 'fioritura_';
  private raccoltaField = 'raccolta_';

  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'PosId', title: '' }, { hidden: true, editable: false }),
    new KendoGridColumn({ field: 'Veg_Cod', title: '' }, { hidden: true, editable: false }),
    new KendoGridColumn({ field: 'Cul_Cod', title: '' }, { hidden: true, editable: false }),
    new KendoGridColumn({ field: 'Veg_Des', title: this.translocoService.translate('Specie') }, { editable: false }),
    new KendoGridColumn({ field: 'Cul_des', title: this.translocoService.translate('Varieta') }, { editable: false }),
  ];

  private data: DefaultDistintaProduzione;
  private reloader = new Subject<void>();
  private kendoModel: ProfilazioneImpreseDefaultDistintaProduzioneGridModel = {
    PosId: new ModelEntry(CELL_TYPES.NUMBER, false),
    Veg_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Cul_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Veg_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Cul_des: new ModelEntry(CELL_TYPES.STRING, false),
  };

  constructor(
    protected injector: Injector,
    private objParametriAgendaService: ObjParametriAgendaService,
    private translocoService: TranslocoService,
    private service: ProfilazioneImpreseService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings();
    this.resizable = new ResizableSettings(true, false);
    this.resizable.autoFitColumns = true;
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: true,
      infoBtn: false,
      removeBtn: false
    });

    this.behavior.excelSettings.enabled = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.toolbar.customToolbar = true;
  }

  public read(): Observable<ProfilazioneImpreseDefaultDistintaProduzioneGridResult> {
    return combineLatest([this.service.reloadData.pipe(startWith(null)), this.service.defaultPianiColturaliSpecieSubject, this.service.globalSubject, this.objParametriAgendaService.currentObjParametriAgenda])
      .pipe(
        tap(() => this.isLoading(true)),
        switchMap(([_, vegCod, global, obj]) => this.service.getDistintaDiProduzione(global ? undefined : obj.Piva, vegCod)),
        catchError(() => of({} as DefaultDistintaProduzione)),
        tap((data: DefaultDistintaProduzione) => this.data = { GruCod: data.GruCod, NCicli: data.NCicli, Data: data.Data.map((x, i) => ({ PosId: i, ...x })) }),
        switchMap(x => combineLatest([of(x), this.reloader.pipe(startWith(null))])),
        map(() => {
          const [model, columns] = this.mapData(this.data);
          return new ProfilazioneImpreseDefaultDistintaProduzioneGridResult(this.data.Data, columns, model);
        }),
        tap(() => this.isLoading(false)),
      );
  }

  public perform(action: HttpAction, data: any): Observable<any> {
    if (action == HttpAction.BATCH_SAVE) {
      const rows = data.updated as any[];

      return of(true)
        .pipe(
          withLatestFrom(this.service.defaultPianiColturaliSpecieSubject, this.service.globalSubject, this.objParametriAgendaService.currentObjParametriAgenda),
          map(([_, vegCod, global, obj]) => this.getUpdatePayload(vegCod, rows, global, obj)),
          switchMap(body => this.service.salvaDefaultDistintaDiProduzione(body))
        );
    }

    return of(true);
  }

  public addCycle(): void {
    this.data.NCicli++;
    this.reloader.next(null);
  }

  private mapData(data: DefaultDistintaProduzione): [KendoGridModel, KendoGridColumn[]] {
    const model = { ...this.kendoModel };
    const columns = [...this.kendoColumns];
    for (let i = 1; i <= data.NCicli; i++) {
      if (data.GruCod == 3) {
        model[`${this.seminaField}${i}`] = new ModelEntry(CELL_TYPES.STRING, true);
        columns.push(new KendoGridColumn({ field: `${this.seminaField}${i}`, title: this.translocoService.translate('profImprese.SeminaXCiclo', { 'ciclo': i }) }, { editable: true }));
      }

      model[`${this.fiorituraField}${i}`] = new ModelEntry(CELL_TYPES.STRING, true);
      columns.push(new KendoGridColumn({ field: `${this.fiorituraField}${i}`, title: this.translocoService.translate('profImprese.FiorituraXCiclo', { 'ciclo': i }) }, { editable: true }));

      model[`${this.raccoltaField}${i}`] = new ModelEntry(CELL_TYPES.STRING, true);
      columns.push(new KendoGridColumn({ field: `${this.raccoltaField}${i}`, title: this.translocoService.translate('profImprese.RaccoltaXCiclo', { 'ciclo': i }) }, { editable: true }));
    }

    return [model, columns];
  }

  private getUpdatePayload(vegCod: number, rows: any[], global: boolean, obj: ObjParametriAgenda): DistintaProduzione_In {
    const data = [] as DistintaProduzioneItem[];
    for (const el of this.data.Data) {
      data.push({
        CulCod: el['Cul_Cod'],
        GruCod: this.data.GruCod,
        VegCod: el['Veg_Cod'],
        Values: this.getValuePayload(rows.find(x => x['PosId'] == el['PosId']) ?? el, this.data.GruCod),
      });
    }

    const payload = {
      VegCod: vegCod,
      Piva: global ? '' : obj.Piva,
      Data: data
    } as DistintaProduzione_In;
    return payload;
  }

  getValuePayload(data: Object, gruCod: number): DistintaProduzioneValue[] {
    const result = [];
    for (let ciclo = 1; ciclo <= this.data.NCicli; ciclo++) {
      const value = {
        NCiclo: ciclo,
        DataFioritura: data[`${this.fiorituraField}${ciclo}`],
        DataRaccolta: gruCod == 3 ? data[`${this.raccoltaField}${ciclo}`] : null,
        DataSemina: data[`${this.seminaField}${ciclo}`],
      } as DistintaProduzioneValue;
      result.push(value);
    }

    return result;
  }
}

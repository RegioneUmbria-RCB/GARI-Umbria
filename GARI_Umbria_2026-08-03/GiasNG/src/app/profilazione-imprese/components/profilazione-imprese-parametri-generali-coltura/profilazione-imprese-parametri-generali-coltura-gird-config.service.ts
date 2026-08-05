import { Injectable, Injector } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { DefaultGenerale, ProfilazioneImpreseService } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { DefaultGenerali_In, DefaultGeneraliData_In } from 'app/Service/net-core6-api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { catchError, combineLatest, map, Observable, of, startWith, switchMap, tap, withLatestFrom } from 'rxjs';

export class ProfilazioneImpreseParametriGeneraliColturaGridResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}
export class ProfilazioneImpreseParametriGeneraliColturaGridModel extends KendoGridModel {
  Veg_Cod: ModelEntry;
  Cul_Cod: ModelEntry;
  Veg_Des: ModelEntry;
  Cul_des: ModelEntry;
  unita_calore: ModelEntry;
  gg: ModelEntry;
  resa: ModelEntry;
  dosi_ha: ModelEntry;
  dosi_ha_udm: ModelEntry;
}

@Injectable()
export class ProfilazioneImpreseParametriGeneraliColturaGridConfigService extends AbstractGridConfigService<ProfilazioneImpreseParametriGeneraliColturaGridResult> {
  gridId = 'ProfilazioneImpreseParametriGeneraliColturaGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'Area_Cod';

  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'Veg_Cod', title: '' }, { hidden: true, editable: false }),
    new KendoGridColumn({ field: 'Cul_Cod', title: '' }, { hidden: true, editable: false }),
    new KendoGridColumn({ field: 'Veg_Des', title: this.translocoService.translate('Specie') }, { editable: false }),
    new KendoGridColumn({ field: 'Cul_des', title: this.translocoService.translate('Varieta') }, { editable: false }),
    new KendoGridColumn({ field: 'unita_calore', title: this.translocoService.translate('profImprese.UnitaCalore') }, { editable: true, numeric: { defaultValue: 0, format: 'n0' } }),
    new KendoGridColumn({ field: 'gg', title: this.translocoService.translate('profImprese.GgMat') }, { editable: true, numeric: { defaultValue: 0, format: 'n0' } }),
    new KendoGridColumn({ field: 'resa', title: this.translocoService.translate('profImprese.ResaHa') }, { editable: true, numeric: { defaultValue: 0, format: 'n0' } }),
    new KendoGridColumn({ field: 'dosi_ha', title: this.translocoService.translate('profImprese.DosiXHa') }, { editable: true, numeric: { defaultValue: 0, format: 'n0' } }),
    new KendoGridColumn({ field: 'dosi_ha_udm', title: this.translocoService.translate('profImprese.DosiXHa') }, { editable: true }),
  ];

  private data: DefaultGenerale[];
  private kendoModel: ProfilazioneImpreseParametriGeneraliColturaGridModel = {

    Veg_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Cul_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Veg_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Cul_des: new ModelEntry(CELL_TYPES.STRING, false),
    unita_calore: new ModelEntry(CELL_TYPES.NUMBER, true),
    gg: new ModelEntry(CELL_TYPES.NUMBER, true),
    resa: new ModelEntry(CELL_TYPES.NUMBER, true),
    dosi_ha: new ModelEntry(CELL_TYPES.NUMBER, true),
    dosi_ha_udm: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
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
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: true,
      infoBtn: false,
      removeBtn: true
    });

    this.behavior.excelSettings.enabled = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.toolbar.customToolbar = true;
  }

  public read(): Observable<ProfilazioneImpreseParametriGeneraliColturaGridResult> {
    return combineLatest([this.service.reloadData.pipe(startWith(null)), this.service.globalSubject, this.objParametriAgendaService.currentObjParametriAgenda])
      .pipe(
        tap(() => this.isLoading(true)),
        switchMap(([_, global, obj]) => this.service.getParametriGeneraliColtura(global ? undefined : obj.Piva)),
        catchError(() => of([] as DefaultGenerale[])),
        map(data => {
          this.handleDropdown();
          this.data = data;
          return new ProfilazioneImpreseParametriGeneraliColturaGridResult(data, this.kendoColumns, this.kendoModel);
        }),
        tap(() => this.isLoading(false)),
      );
  }

  public perform(action: HttpAction, row: DefaultGenerale): Observable<any> {
    if (action == HttpAction.UPDATE) {
      const vegCod = row.Veg_Cod;

      return of(true)
        .pipe(
          withLatestFrom(this.objParametriAgendaService.currentObjParametriAgenda),
          map(([_, obj]) => ({
            Piva: this.service.globalSubject.value ? '' : obj.Piva,
            Data: this.data.map(d => ({
              VegCod: d.Veg_Cod,
              CulCod: d.Cul_Cod,
              // Cal1: d.Veg_Cod == vegCod ? row.cal_1 : d.cal_1,
              // Cal2: d.Veg_Cod == vegCod ? row.cal_2 : d.cal_2,
              // Cal3: d.Veg_Cod == vegCod ? row.cal_3 : d.cal_3,
              // Cal4: d.Veg_Cod == vegCod ? row.cal_4 : d.cal_4,
              // Cal5: d.Veg_Cod == vegCod ? row.cal_5 : d.cal_5,
              // Cal6: d.Veg_Cod == vegCod ? row.cal_6 : d.cal_6,
              DosiHa: d.Veg_Cod == vegCod ? row.dosi_ha : d.dosi_ha,
              DosiHaUdm: d.Veg_Cod == vegCod ? row.dosi_ha_udm : d.dosi_ha_udm,
              Gg: d.Veg_Cod == vegCod ? row.gg : d.gg,
              Resa: d.Veg_Cod == vegCod ? row.resa : d.resa,
              UnitaCalore: d.Veg_Cod == vegCod ? row.unita_calore : d.unita_calore,
            } as DefaultGeneraliData_In))
          } as DefaultGenerali_In)),
          switchMap(body => this.service.scriviDefaultGenerale(body))
        );
    }

    if (action == HttpAction.REMOVE) {
      return of(true)
        .pipe(
          withLatestFrom(this.objParametriAgendaService.currentObjParametriAgenda),
          switchMap(([_, obj]) => this.service.cancellaDefaultGenerale(this.service.globalSubject.value ? undefined : obj.Piva, row.Veg_Cod, row.Cul_Cod))
        );
    }

    return of(row);
  }

  private handleDropdown() {
    const colDosi = this.kendoColumns.find(s => s.field === 'dosi_ha_udm');
    const defaultValue = new DropdownListItem("93", "Unita' di seme");
    const data = [
      new DropdownListItem("0", "Non Specificata"),
      new DropdownListItem("2", "Chilogrammi"),
      new DropdownListItem("92", "n. piante"),
      defaultValue,
      new DropdownListItem("1003", "Confezioni")
    ];
    colDosi.ddl = new DropdownListWithForm('dosi_ha_udm', 'dosi_ha_udm', 'descrizione', data);
    colDosi.ddl.valuePrimitive = true;
    colDosi.ddl.defaultValue = defaultValue;
  }
}

import { Injectable, Injector } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { DefaultSpecie, ProfilazioneImpreseService } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { SpecieVegetali_In, SpecieVegetaliData_In } from 'app/Service/net-core6-api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { catchError, combineLatest, map, Observable, of, startWith, switchMap, tap, withLatestFrom } from 'rxjs';

export class ProfilazioneImpreseDefaultSpecieGridResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}
export class ProfilazioneImpreseDefaultSpecieGridModel extends KendoGridModel {
  Veg_Cod: ModelEntry;
  Cul_Cod: ModelEntry;
  Veg_Des: ModelEntry;
  Cul_des: ModelEntry;
  tipo_maturazione: ModelEntry;
  Soglia_Minima: ModelEntry;
  resa_stabilimento: ModelEntry;
  peso_sgocciolato: ModelEntry;
}

@Injectable()
export class ProfilazioneImpreseDefaultSpecieGridConfigService extends AbstractGridConfigService<ProfilazioneImpreseDefaultSpecieGridResult> {
  gridId = 'ProfilazioneImpreseDefaultSpecieGri';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'Area_Cod';

  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn({ field: 'Veg_Cod', title: '' }, { hidden: true, editable: false }),
    new KendoGridColumn({ field: 'Cul_Cod', title: '' }, { hidden: true, editable: false }),
    new KendoGridColumn({ field: 'Cul_des', title: '' }, { hidden: true, editable: false }),
    new KendoGridColumn({ field: 'Veg_Des', title: this.translocoService.translate('Specie') }, { editable: false }),
    new KendoGridColumn({ field: 'tipo_maturazione', title: this.translocoService.translate('profImprese.TipoMaturazione') }, { editable: true, }),
    new KendoGridColumn({ field: 'Soglia_Minima', title: this.translocoService.translate('profImprese.SogliaMinimaUtile') }, { editable: true, numeric: { defaultValue: 0, format: 'n0' } }),
    new KendoGridColumn({ field: 'resa_stabilimento', title: this.translocoService.translate('profImprese.ResaDiStabilimentoPerc') }, { editable: true, numeric: { defaultValue: 0, format: 'n0' } }),
    new KendoGridColumn({ field: 'peso_sgocciolato', title: this.translocoService.translate('profImprese.PesoSgocciolatoScatola12Kg') }, { editable: true, numeric: { defaultValue: 0, format: 'n0' } }),
  ];

  private data: DefaultSpecie[];
  private kendoModel: ProfilazioneImpreseDefaultSpecieGridModel = {
    Veg_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Cul_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Veg_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Cul_des: new ModelEntry(CELL_TYPES.STRING, false),
    tipo_maturazione: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    Soglia_Minima: new ModelEntry(CELL_TYPES.NUMBER, true),
    resa_stabilimento: new ModelEntry(CELL_TYPES.NUMBER, true),
    peso_sgocciolato: new ModelEntry(CELL_TYPES.NUMBER, true),
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

  public read(): Observable<ProfilazioneImpreseDefaultSpecieGridResult> {
    return combineLatest([this.service.reloadData.pipe(startWith(null)), this.service.globalSubject, this.objParametriAgendaService.currentObjParametriAgenda])
      .pipe(
        tap(() => this.isLoading(true)),
        switchMap(([_, global, obj]) => this.service.getDefaultGeneraleSpecie(global ? undefined : obj.Piva)),
        tap(data => data.forEach(x => x.tipo_maturazione ??= 'U')),
        catchError(() => of([])),
        map(data => {
          this.handleDropdown();
          this.data = data;
          return new ProfilazioneImpreseDefaultSpecieGridResult(data, this.kendoColumns, this.kendoModel);
        }),
        tap(() => this.isLoading(false)),
      );
  }

  public perform(action: HttpAction, row: DefaultSpecie): Observable<any> {
    if (action == HttpAction.UPDATE) {
      const vegCod = row.Veg_Cod;

      return of(true).pipe(
        withLatestFrom(this.objParametriAgendaService.currentObjParametriAgenda),
        map(([_, obj]) => ({
          Piva: this.service.globalSubject.value ? '' : obj.Piva,
          Data: this.data.map(d => ({
            VegCod: d.Veg_Cod,
            TipoMaturazione: d.Veg_Cod == vegCod ? row.tipo_maturazione : d.tipo_maturazione,
            SogliaMinima: d.Veg_Cod == vegCod ? row.Soglia_Minima : d.Soglia_Minima,
            ResaStabilimento: d.Veg_Cod == vegCod ? row.resa_stabilimento : d.resa_stabilimento,
            PesoSgocciolato: d.Veg_Cod == vegCod ? row.peso_sgocciolato : d.peso_sgocciolato,
          } as SpecieVegetaliData_In))
        } as SpecieVegetali_In)),
        switchMap(body => this.service.scriviDefaultSpecie(body))
      );
    }

    if (action == HttpAction.REMOVE) {
      return of(true).pipe(
        withLatestFrom(this.objParametriAgendaService.currentObjParametriAgenda),
        switchMap(([_, obj]) => this.service.cancellaDefaultSpecie(this.service.globalSubject.value ? undefined : obj.Piva, row.Veg_Cod))
      );
    }

    return of(row);
  }

  private handleDropdown() {
    const col = this.kendoColumns.find(s => s.field === 'tipo_maturazione');
    const data = [new DropdownListItem('U', 'Unita calore'), new DropdownListItem('G', 'Giorni')];
    col.ddl = new DropdownListWithForm('tipo_maturazione', 'tipo_maturazione', 'descrizione', data);
    col.ddl.valuePrimitive = true;
  }
}

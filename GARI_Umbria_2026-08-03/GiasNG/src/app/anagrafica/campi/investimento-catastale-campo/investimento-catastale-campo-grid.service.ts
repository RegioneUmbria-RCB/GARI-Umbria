import {Inject, Injectable, Injector, Renderer2} from '@angular/core';
import { TranslocoService } from "@jsverse/transloco";
import { MasterService } from "app/Service/master.service";
import { CommandsColumnSettings, ToolbarSettings } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  DateSettings,
  EditingMode,
  GridCustomizations,
  KendoGridColumn,
  KendoGridModel,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  RendererGridEvent
} from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import {catchError, map, Observable, of} from 'rxjs';
import { InvestimentoCatastaleCampoService } from "./investimento-catastale-campo.service";
import {CAMPI_SERVICE_TOKEN, CampiFactoryService} from '../../../Service/ServiceFactory/campi.factory.service';

export class KendoInvestimentoCatastoCampoModel extends KendoGridModel {
  chiave: ModelEntry;
  rag_soc: ModelEntry;
  sa_nome: ModelEntry;
  Piva: ModelEntry;
  SA_COD: ModelEntry;
  PROV: ModelEntry;
  COMUNI_PROV: ModelEntry;
  COM: ModelEntry;
  LOCALITA: ModelEntry;
  SEZIONE: ModelEntry;
  FOGLIO: ModelEntry;
  NUMERO: ModelEntry;
  SUBALTERNO: ModelEntry;
  Campo_Des: ModelEntry;
  APP_NOME: ModelEntry;
  SUP_APP: ModelEntry;
  Validita_Inizio: ModelEntry;
  Validita_Fine: ModelEntry;
  AREA: ModelEntry;
  Utilizzo: ModelEntry;
  Attivo: ModelEntry;
}

export class InvestimentoCatastoCampoKendoServerResult extends KendoServerResult {
  constructor(model, columns, rows) {
    super(model, columns, rows);
  }
}

@Injectable()
export class InvestimentoCatastaleCampoGridService extends AbstractGridConfigService<InvestimentoCatastoCampoKendoServerResult> {
  editingMode = EditingMode.IN_LINE;
  loader = LoaderType.SERVICE;
  rowId = 'chiave';
  gridId = 'InvestimentoCatastaleGrid';

  cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });

  toolbar = new ToolbarSettings(false, false);
  views = new GridCustomizations({enabled: false});

  kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      {field: 'PROV', title: this.transloco.translate('ProvinciaAbbr')},
      {resizable: true, editable: true, width: 50}
    ),
    new KendoGridColumn(
      {field: 'COMUNI_PROV', title: this.transloco.translate('ProvinciaAbbr')},
      {resizable: true, editable: true, width: 50}
    ),
    new KendoGridColumn(
      {field: 'COM', title: this.transloco.translate('Comune')},
      {resizable: true, editable: true, width: 50}
    ),
    new KendoGridColumn(
      {field: 'LOCALITA', title: this.transloco.translate('Comune')},
      {resizable: true, editable: true, width: 120}
    ),
    new KendoGridColumn(
      {field: 'SEZIONE', title: this.transloco.translate('Sezione')},
      {resizable: true, editable: true, width: 50}
    ),
    new KendoGridColumn(
      {field: 'FOGLIO', title: this.transloco.translate('Foglio')},
      {resizable: true, editable: true, width: 50, numeric: {multiCheckFiltering: true}}
    ),
    new KendoGridColumn(
      {field: 'NUMERO', title: this.transloco.translate('Numero')},
      {resizable: true, editable: true, width: 50, numeric: {multiCheckFiltering: true}}
    ),
    new KendoGridColumn(
      {field: 'SUBALTERNO', title: this.transloco.translate('Subalterno')},
      {resizable: true, editable: true, width: 50}
    ),
    new KendoGridColumn(
      {field: 'Campo_Des', title: this.transloco.translate('Campo')},
      {resizable: true, editable: true, width: 80}
    ),
    new KendoGridColumn(
      {field: 'AREA', title: this.transloco.translate('SuperficieIntersecataAbbr')},
      {resizable: true, editable: true, width: 50, numeric: {}}
    ),
    new KendoGridColumn(
      {field: 'Validita_Inizio', title: this.transloco.translate('ValiditàInizio')},
      {resizable: true, editable: true, date: new DateSettings(), width: 60}
    ),
    new KendoGridColumn(
      {field: 'Validita_Fine', title: this.transloco.translate('ValiditàFine')},
      {resizable: true, editable: true, date: new DateSettings(), width: 60}
    )
  ];

  kendoModel: KendoInvestimentoCatastoCampoModel = {
    chiave: new ModelEntry(CELL_TYPES.STRING, false),
    rag_soc: new ModelEntry(CELL_TYPES.STRING, false),
    sa_nome: new ModelEntry(CELL_TYPES.STRING, false),
    Piva: new ModelEntry(CELL_TYPES.STRING, false),
    SA_COD: new ModelEntry(CELL_TYPES.NUMBER, false),
    PROV: new ModelEntry(CELL_TYPES.STRING, false),
    COMUNI_PROV: new ModelEntry(CELL_TYPES.STRING, false),
    COM: new ModelEntry(CELL_TYPES.STRING, false),
    LOCALITA: new ModelEntry(CELL_TYPES.STRING, false),
    SEZIONE: new ModelEntry(CELL_TYPES.STRING, false),
    FOGLIO: new ModelEntry(CELL_TYPES.NUMBER, false),
    NUMERO: new ModelEntry(CELL_TYPES.NUMBER, false),
    SUBALTERNO: new ModelEntry(CELL_TYPES.STRING, false),
    Campo_Des: new ModelEntry(CELL_TYPES.STRING, false),
    APP_NOME: new ModelEntry(CELL_TYPES.STRING, false),
    SUP_APP: new ModelEntry(CELL_TYPES.NUMBER, false),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, false),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE, false),
    AREA: new ModelEntry(CELL_TYPES.NUMBER, false),
    Utilizzo: new ModelEntry(CELL_TYPES.STRING, false),
    Attivo: new ModelEntry(CELL_TYPES.NUMBER, false),
  };

  constructor(
    injector: Injector,
    private investimentoCatastaleCampoService: InvestimentoCatastaleCampoService,
    @Inject(CAMPI_SERVICE_TOKEN) private campiService: CampiFactoryService,
    private masterService: MasterService,
    private renderer: Renderer2,
    protected transloco: TranslocoService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
  }

  read(options?: any): Observable<any> {
    this.masterService.set_isLoading({ message: '', isLoading: true });
    const filtro = this.investimentoCatastaleCampoService.filtri.getValue();

    if (filtro != undefined) {
      return this.campiService.leggiInvestimentoCatastale(filtro).pipe(
        map((r) => {
          const result = new InvestimentoCatastoCampoKendoServerResult(
            this.kendoModel,
            this.kendoColumns,
            r.RispostaStringa
          );
          this.masterService.set_isLoading({message: '', isLoading: false});
          return result;
        }),
        catchError((e, c) => {
          this.masterService.set_isLoading({message: '', isLoading: false});
          return of(new InvestimentoCatastoCampoKendoServerResult(this.model, this.columns, []));
        })
      );
    } else {
      return of(new InvestimentoCatastoCampoKendoServerResult(this.model, this.columns, []));
    }
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    throw new Error("Method not implemented.");
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const { grid, gridElRef } = { ...opts };
    let a = gridElRef;
    let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
    grid.view.forEach((el, index) => {
      if (el.Attivo === 0) {
        this.renderer.addClass(visibleRows[index], 'nonAttivo');
      }
      if (el.Attivo === 1) {
        this.renderer.removeClass(visibleRows[index], 'nonAttivo');
      }
    });
  }

}

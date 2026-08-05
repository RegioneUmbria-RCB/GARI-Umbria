import { Inject, Injectable, Injector, Renderer2 } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { MasterService } from "app/Service/master.service";
import { InvestimentoCatastaleFactoryService, INVESTIMENTOCATASTALE_SERVICE_TOKEN, TipoInvestimento } from "app/Service/ServiceFactory/investimento-catastale.factory.service";
import { AggregateSettings, CommandsColumnSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DateSettings, EditingMode, GridCustomizations, KendoGridColumn, KendoGridModel, KendoServerResult, LoaderType, ModelEntry, RendererGridEvent } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { map, Observable } from "rxjs";
import { InvestimentoCatastaleFiltriService } from "./investimento-catastale-filtri.service";

export class KendoInvestimentoCatastoModel extends KendoGridModel {
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
  ZVN: ModelEntry;
  Campo_Des: ModelEntry;
  APP_NOME: ModelEntry;
  SUP_APP: ModelEntry;
  Validita_Inizio: ModelEntry;
  Validita_Fine: ModelEntry;
  AREA: ModelEntry;
  Utilizzo: ModelEntry;
  Attivo: ModelEntry;
}

export class InvestimentoCatastoKendoServerResult extends KendoServerResult {
  constructor(model, columns, rows) {
    super(model, columns, rows);
  }
}

@Injectable()
export class InvestimentoCatastaleGridService extends AbstractGridConfigService<any> {
  editingMode = EditingMode.IN_LINE;
  loader = LoaderType.SERVICE;
  rowId = 'chiave';
  gridId = 'InvestimentoCatastaleGrid';

  kendoColumns: KendoGridColumn[] = [];

  kendoModel: KendoInvestimentoCatastoModel = {
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
    ZVN: new ModelEntry(CELL_TYPES.STRING, false),
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
    @Inject(INVESTIMENTOCATASTALE_SERVICE_TOKEN) private investimentoCatastaleService: InvestimentoCatastaleFactoryService,
    private masterService: MasterService,
    private investimentoCatastaleFiltriService: InvestimentoCatastaleFiltriService,
    private renderer: Renderer2,
    protected transloco: TranslocoService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.handleCustomization();
    this.setKendoColumns();
  }


  read(options?: any): Observable<any> {
    this.masterService.set_isLoading({ message: '', isLoading: true });
    return this.investimentoCatastaleService.leggi().pipe(
      map((val) => {
        if (this.investimentoCatastaleFiltriService.tipo == TipoInvestimento.Catasto) {
          val = val.map((el) => {
            el.Campo_Des = '';
            el.APP_NOME = '';
            el.Validita_Inizio = AGRODATAINIZIO;
            el.Validita_Fine = AGRODATAFINE;
            el.Utilizzo = '';
            el.Attivo = 1;
            el.descrizione = '';
            el.rag_soc = '';
            el.Veg_Des = '';
            el.Cul_Des = '';
            return el;
          });

          let answer: any[] = []

          val.forEach(x => {
            if (!answer.some(y => JSON.stringify(y) === JSON.stringify(x))) {
              answer.push(x)
            }
          })

          val = [...answer];
        }

        this.masterService.set_isLoading({ message: '', isLoading: false });
        return new InvestimentoCatastoKendoServerResult(this.kendoModel, this.kendoColumns, val);
      })
    );
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
    })
  }

  setKendoColumns(): void {
    if (this.investimentoCatastaleFiltriService.tipo == TipoInvestimento.Catasto) {
      this.kendoColumns.push(new KendoGridColumn({ field: 'PROV', title: this.transloco.translate('ProvinciaAbbr') }, { resizable: true, editable: true, width: 50 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'COMUNI_PROV', title: this.transloco.translate('ProvinciaAbbr') }, { resizable: true, editable: true, width: 50 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'COM', title: this.transloco.translate('ComuneAbbr') }, { resizable: true, editable: true, width: 50 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'LOCALITA', title: this.transloco.translate('Comune') }, { resizable: true, editable: true, width: 120 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'SEZIONE', title: this.transloco.translate('Sezione') }, { resizable: true, editable: true, width: 50 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'FOGLIO', title: this.transloco.translate('Foglio') }, { resizable: true, editable: true, width: 50, numeric: { multiCheckFiltering: true } }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'NUMERO', title: this.transloco.translate('Numero') }, { resizable: true, editable: true, width: 50, numeric: { multiCheckFiltering: true } }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'SUBALTERNO', title: this.transloco.translate('Subalterno') }, { resizable: true, editable: true, width: 50 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'ZVN', title: this.transloco.translate('ZVN') }, { resizable: true, editable: true, width: 50 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'SUP_APP', title: this.transloco.translate('SuperficieAppezzamentoAbbr') }, { resizable: true, editable: true, width: 50, numeric: {} }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'AREA', title: this.transloco.translate('SuperficieIntersecataAbbr') }, { resizable: true, editable: true, width: 50, numeric: {} }));
    } else {
      this.kendoColumns.push(new KendoGridColumn({ field: 'Campo_Des', title: this.transloco.translate('Campo') }, { resizable: true, editable: true, width: 80 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'APP_NOME', title: this.transloco.translate('NomeAppezzamento') }, { resizable: true, editable: true, width: 50 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'SUP_APP', title: this.transloco.translate('SuperficieAppezzamentoAbbr') }, { resizable: true, editable: true, width: 50, numeric: {} }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'AREA', title: this.transloco.translate('SuperficieIntersecataAbbr') }, { resizable: true, editable: true, width: 50, numeric: {} }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'Utilizzo', title: this.transloco.translate('Utilizzo') }, { resizable: true, editable: true, width: 120 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'Validita_Inizio', title: this.transloco.translate('ValiditàInizio') }, { resizable: true, editable: true, date: new DateSettings(), width: 60 }));
      this.kendoColumns.push(new KendoGridColumn({ field: 'Validita_Fine', title: this.transloco.translate('ValiditàFine') }, { resizable: true, editable: true, date: new DateSettings(), width: 60 }));
      this.aggregates = new AggregateSettings({
        enabled: true,
        descriptors: [
          { field: 'AREA', aggregate: 'sum', format: 'n4' },
          { field: 'SUP_APP', aggregate: 'sum', format: 'n4' },
        ]}
      );
    }
  }

  handleCustomization() {
    this
      .cmdColumn = new
    CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false
    });

    this.toolbar = new ToolbarSettings(false, false);
    this.views = new GridCustomizations({enabled: false});
  }

}

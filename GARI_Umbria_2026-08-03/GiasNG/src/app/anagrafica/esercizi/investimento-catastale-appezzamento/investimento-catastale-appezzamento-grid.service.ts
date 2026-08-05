import {Inject, Injectable, Injector, Renderer2} from '@angular/core';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  EditingMode, GridCustomizations,
  KendoGridColumn,
  LoaderType,
  ModelEntry,
  RendererGridEvent
} from 'gias-kendo-grid';
import {map, Observable} from 'rxjs';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {
  InvestimentoCatastaleAppezzamentoKendoServerResult,
  InvestomentoCatastaleAppezzamentoKendoModel
} from './investimento-catastale-appezzamento.model';
import {ConfigTemplate} from 'gias-kendo-grid';
import {
  IMPIANTI_SERVICE_TOKEN,
  ImpiantiFactoryService,
  CaricaDatiCatastali
} from '../../../Service/ServiceFactory/impianti.factory.service';
import {InvestimentoCatastaleAppezzamentoDataService} from './investimento-catastale-appezzamento-data.service';
import {LeggiInvestimentoCatastale} from '../../../Service/ServiceFactory/investimento-catastale.factory.service';
import {CommandsColumnSettings, ToolbarSettings} from 'gias-kendo-grid';

@Injectable()
export class InvestimentoCatastaleAppezzamentoGridService extends AbstractGridConfigService<InvestimentoCatastaleAppezzamentoKendoServerResult> {
  editingMode: EditingMode = EditingMode.IN_LINE;
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = 'chiave';
  gridId: string = 'investimentoCatastaleAppezzamento';

  private kendoModel: InvestomentoCatastaleAppezzamentoKendoModel = {
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

  private kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'PROV', title: this.transloco.translate('ProvinciaAbbr') },
      { resizable: true, editable: true, width: 50 }
    ),
    new KendoGridColumn(
      { field: 'COMUNI_PROV', title: this.transloco.translate('ProvinciaAbbr') },
      { resizable: true, editable: true, width: 50 }
    ),
    new KendoGridColumn(
      { field: 'COM', title: this.transloco.translate('ComuneAbbr') },
      { resizable: true, editable: true, width: 50 }
    ),
    new KendoGridColumn(
      { field: 'LOCALITA', title: this.transloco.translate('Comune') },
      { resizable: true, editable: true, width: 120 }
    ),
    new KendoGridColumn(
      { field: 'SEZIONE', title: this.transloco.translate('Sezione') },
      { resizable: true, editable: true, width: 50 }
    ),
    new KendoGridColumn(
      { field: 'FOGLIO', title: this.transloco.translate('Foglio') },
      { resizable: true, editable: true, width: 50, numeric: { multiCheckFiltering: true } }
    ),
    new KendoGridColumn(
      { field: 'NUMERO', title: this.transloco.translate('Numero') },
      { resizable: true, editable: true, width: 50, numeric: { multiCheckFiltering: true } }
    ),
    new KendoGridColumn(
      { field: 'SUBALTERNO', title: this.transloco.translate('Subalterno') },
      { resizable: true, editable: true, width: 50 }
    ),
    new KendoGridColumn(
      { field: 'ZVN', title: this.transloco.translate('ZVN') },
      { resizable: true, editable: true, width: 50 }
    ),
    new KendoGridColumn(
      { field: 'SUP_APP', title: this.transloco.translate('SuperficieAppezzamentoAbbr') },
      { resizable: true, editable: true, width: 50, numeric: {} }
    ),
    new KendoGridColumn(
      { field: 'AREA', title: this.transloco.translate('SuperficieIntersecataAbbr') },
      { resizable: true, editable: true, width: 50, numeric: {} }
    )
  ];

  constructor(
    injector: Injector,
    private renderer: Renderer2,
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private investimentoCatastaleAppezzamentoDataService: InvestimentoCatastaleAppezzamentoDataService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);
    this.handleCustomization();
  }

  read(options?: any): Observable<InvestimentoCatastaleAppezzamentoKendoServerResult> {
    let params: LeggiInvestimentoCatastale = this.investimentoCatastaleAppezzamentoDataService.leggiInvestimentoCatastale
    return this.appezzamentiService.leggiInvestimentoCatastale(params).pipe(
      map(r => {
        return new InvestimentoCatastaleAppezzamentoKendoServerResult(this.kendoModel, this.kendoColumns, r);
      })
    );
  }

  perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
    throw new Error('Method not implemented.');
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const { grid, gridElRef } = { ...opts };
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

  private handleCustomization(): void {
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false
    });

    this.toolbar = new ToolbarSettings(false, false);
    this.views = new GridCustomizations({enabled: false});
  }
}

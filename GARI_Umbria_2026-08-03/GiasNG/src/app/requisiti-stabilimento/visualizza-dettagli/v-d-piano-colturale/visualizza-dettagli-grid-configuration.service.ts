import { Injectable, Injector } from "@angular/core";
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { RequisitiStabilimentoKendoServerResult } from "../../requisiti-stabilimento-contratti-grid/requisiti-stabilimento-contratti-grid-configuration.service";
import { EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { map, Observable, of, tap } from "rxjs";
import { RequisitiStabilimentoAPIService } from "../../../Service/RequisitiStabilimento/requisiti-stabilimento.service";
import { AggregateSettings, AgrSelectableSettings, CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import { GiasMessageService } from "../../../Service/gias-message.service";
import { ConfigTemplate } from 'gias-kendo-grid';
import { SMARTPHONE_WIDTH } from "../../../Model/CostantiPersonalizzate";
import { VisualizzaDettagliService } from "../visualizza-dettagli.service";
import { PercentGridViewerComponent } from "../../percent-grid-viewer/percent-grid-viewer.component";
import { CELL_TYPES } from "gias-ui-kit";

export class VisualizzaDettagliKendoServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class VisualizzaDettagliGridModel extends KendoGridModel {
  chiave: ModelEntry;
  Contratto_Cod: ModelEntry;
  Fase_Cod: ModelEntry;
  PIVA: ModelEntry;
  Id_Budget: ModelEntry;
  Cuaa: ModelEntry;
  Rag_Soc_Padre: ModelEntry;
  rag_soc: ModelEntry;
  mat_cod: ModelEntry;
  Prodotto: ModelEntry;
  Superficie: ModelEntry;
  QtaPrevista: ModelEntry;
  ResaPrevista: ModelEntry;
  Piva_Azienda: ModelEntry;
  Sa_Cod_Azienda: ModelEntry;
  Fabbricato_Cod_Azienda: ModelEntry;
  Rag_Soc_Azienda: ModelEntry;
  Sa_Nome_Azienda: ModelEntry;
  Fabbricato_Des_Azienda: ModelEntry;
  GruppoRaccolta_Des: ModelEntry;
  Certificazioni: ModelEntry;
  Tecnico: ModelEntry;
  Sup: ModelEntry;
  Resa: ModelEntry;
  Percentuale_Superficie: ModelEntry;
  Percentuale_Qta: ModelEntry;
}

@Injectable()
export class VisualizzaDettagliGridConfigurationService extends AbstractGridConfigService<VisualizzaDettagliKendoServerResult> {
  editingMode: EditingMode = EditingMode.IN_PAGE;
  gridId: string = 'requisiti-stabilimento-visualizza-dettagli-grid';
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = 'chiave';

  aggregates = new AggregateSettings(
    {
      enabled: true,
      descriptors: [
        { field: 'Superficie', aggregate: 'sum', format: 'n0' },
        { field: 'QtaPrevista', aggregate: 'sum', format: 'n0' },
        //{ field: 'ResaPrevista', aggregate: 'average', format: 'n0' },
        { field: 'Sup', aggregate: 'sum', format: 'n0' },
        { field: 'Resa', aggregate: 'sum', format: 'n0' }
      ]
    }
  );

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'Rag_Soc_Padre', title: this.transloco.translate('CooperativaReferente') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Cuaa', title: this.transloco.translate('CodiceUnicoAziendaAgricolaSigla') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'rag_soc', title: this.transloco.translate('RagioneSociale') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Prodotto', title: this.transloco.translate('Prodotto') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Superficie', title: this.transloco.translate('SupAssegnata') },
      { resizable: true, filterable: true, editable: false, width: 100, format: 'n2' }
    ),
    new KendoGridColumn(
      { field: 'QtaPrevista', title: this.transloco.translate('QtaAssegnata') },
      { resizable: true, filterable: true, editable: false, width: 100, format: 'n0' }
    ),
    // new KendoGridColumn(
    //   { field: 'ResaPrevista', title: this.transloco.translate('ResaPrevista') },
    //   { resizable: true, filterable: true, editable: false, width: 100, format:'n0' }
    // ),
    new KendoGridColumn(
      { field: 'Rag_Soc_Azienda', title: this.transloco.translate('Rag_Soc_Destinazione') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Sa_Nome_Azienda', title: this.transloco.translate('Sa_Nome_Destinazione') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'GruppoRaccolta_Des', title: this.transloco.translate('GruppoRaccolta') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Certificazioni', title: this.transloco.translate('Certificazione') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Tecnico', title: this.transloco.translate('Tecnico') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Sup', title: this.transloco.translate('SuperficieHa') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Resa', title: this.transloco.translate('QuantitaKg') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Percentuale_Superficie', title: this.transloco.translate('Percentuale_Sup') },
      { resizable: true, filterable: true, editable: false, width: 100, component: PercentGridViewerComponent }
    ),
    new KendoGridColumn(
      { field: 'Percentuale_Qta', title: this.transloco.translate('Percentuale_Qta') },
      { resizable: true, filterable: true, editable: false, width: 100, component: PercentGridViewerComponent }
    )
  ];

  standardModel: VisualizzaDettagliGridModel = {
    chiave: new ModelEntry(CELL_TYPES.STRING),
    Contratto_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    Fase_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    PIVA: new ModelEntry(CELL_TYPES.STRING),
    Id_Budget: new ModelEntry(CELL_TYPES.NUMBER),
    Cuaa: new ModelEntry(CELL_TYPES.STRING),
    Rag_Soc_Padre: new ModelEntry(CELL_TYPES.STRING),
    rag_soc: new ModelEntry(CELL_TYPES.STRING),
    mat_cod: new ModelEntry(CELL_TYPES.NUMBER),
    Prodotto: new ModelEntry(CELL_TYPES.STRING),
    Superficie: new ModelEntry(CELL_TYPES.NUMBER),
    QtaPrevista: new ModelEntry(CELL_TYPES.NUMBER),
    ResaPrevista: new ModelEntry(CELL_TYPES.NUMBER),
    Piva_Azienda: new ModelEntry(CELL_TYPES.STRING),
    Sa_Cod_Azienda: new ModelEntry(CELL_TYPES.NUMBER),
    Fabbricato_Cod_Azienda: new ModelEntry(CELL_TYPES.NUMBER),
    Rag_Soc_Azienda: new ModelEntry(CELL_TYPES.STRING),
    Sa_Nome_Azienda: new ModelEntry(CELL_TYPES.STRING),
    Fabbricato_Des_Azienda: new ModelEntry(CELL_TYPES.STRING),
    GruppoRaccolta_Des: new ModelEntry(CELL_TYPES.STRING),
    Certificazioni: new ModelEntry(CELL_TYPES.STRING),
    Tecnico: new ModelEntry(CELL_TYPES.STRING),
    Sup: new ModelEntry(CELL_TYPES.NUMBER),
    Resa: new ModelEntry(CELL_TYPES.NUMBER),
    Percentuale_Superficie: new ModelEntry(CELL_TYPES.CUSTOM),
    Percentuale_Qta: new ModelEntry(CELL_TYPES.CUSTOM),
  };

  constructor(
    injector: Injector,
    private visualizzaDettagliService: VisualizzaDettagliService,
    private requisitiStabilimentoAPIService: RequisitiStabilimentoAPIService,
    private giasMessageService: GiasMessageService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = false;
    this.selectable.selectable.enabled = false;
    this.selectable.shouldShowCheckbox = false;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.toolbar.resetChanges = false;

    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      removeBtn: false,
    });

    this.groups.groupable.enabled = true;
    this.views.enabled = true;

    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.toolbar.newItem = false;
      this.cmdColumn.editBtn = false;
      this.groups.groupable.enabled = false;
    }

    this.visualizzaDettagliService.reloadGrid$.subscribe((val) => {
      if (val) {
        this.gridPublicService.refresh(true);
      }
    })

  }

  read(_: any): Observable<VisualizzaDettagliKendoServerResult> {
    this.isLoading(true);
    let payload = this.visualizzaDettagliService.requisitiPayload$
    return this.requisitiStabilimentoAPIService.readDettaglioAziendale(payload).pipe(
      map((value) => {
        return new RequisitiStabilimentoKendoServerResult(value, this.columns, this.standardModel);
      }),
      tap(() => this.isLoading(false))
    )

  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return of([]);
  }

}

import { Injectable, Injector } from '@angular/core';
import { finalize, Observable, of, switchMap } from 'rxjs';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { BehaviorSettings, CommandsColumnSettings, ResizableSettings } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';
import { WidgetsClient } from 'app/Service/api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';

const DEFAULT_NUMERO_PRODOTTI = 5;

export class WidgetProdottoResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class WidgetProdottoGridModel extends KendoGridModel {
    Piva: ModelEntry;
    Sa_cod: ModelEntry;
    Id_Agenda: ModelEntry;
    Des_Lib: ModelEntry;
    Qta: ModelEntry;
    Giacenza: ModelEntry;
    Data_Movimento: ModelEntry;
    Udm: ModelEntry;
    Udm_Des: ModelEntry;
    Udm_Sim: ModelEntry;
    Id_Destinazione: ModelEntry;
    Fabbricato_Des: ModelEntry;
    Cod_Articolo: ModelEntry;
    Descrizione_Prodotto: ModelEntry;
    Lotto: ModelEntry;
    Elem_Cod: ModelEntry;
    Pro_Cod: ModelEntry;
    Mat_Cod: ModelEntry;
}

@Injectable({
    providedIn: 'root'
})
export class UltimiProdottiWidgetGridConfig extends AbstractGridConfigService<WidgetProdottoResult> {
    gridId = 'UltimiProdottiWidgetGrid';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;
    behavior: BehaviorSettings;
    rowId = 'chiave';
    columns: KendoGridColumn[] = [
        new KendoGridColumn(
            { field: 'Descrizione_Prodotto', title: this.transloco.translate('Prodotto') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 125
            }
        ),
        new KendoGridColumn(
            { field: 'Data_Movimento', title: this.transloco.translate('UltimoScarico') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 140
            }
        ),
        new KendoGridColumn(
            { field: 'Fabbricato_Des', title: this.transloco.translate('Magazzino') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 150
            }
        ),
        new KendoGridColumn(
            { field: 'Qta', title: this.transloco.translate('qta') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100,
                format: "{0:0.00}"
            }
        ),
        new KendoGridColumn(
            { field: 'Giacenza', title: this.transloco.translate('Giacenza') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 110,
                format: "{0:0.00}"
            }
        ),
        new KendoGridColumn(
            { field: 'Udm_Sim', title: this.transloco.translate('UnitàDiMisuraAbbr') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 90
            }
        )
    ];

    widgetProdottoGridModel: WidgetProdottoGridModel = {
        Piva: new ModelEntry(CELL_TYPES.STRING),
        Sa_cod: new ModelEntry(CELL_TYPES.NUMBER),
        Id_Agenda: new ModelEntry(CELL_TYPES.NUMBER),
        Des_Lib: new ModelEntry(CELL_TYPES.STRING),
        Qta: new ModelEntry(CELL_TYPES.NUMBER),
        Giacenza: new ModelEntry(CELL_TYPES.NUMBER),
        Data_Movimento: new ModelEntry(CELL_TYPES.DATE),
        Udm: new ModelEntry(CELL_TYPES.NUMBER),
        Udm_Des: new ModelEntry(CELL_TYPES.STRING),
        Udm_Sim: new ModelEntry(CELL_TYPES.STRING),
        Id_Destinazione: new ModelEntry(CELL_TYPES.NUMBER),
        Fabbricato_Des: new ModelEntry(CELL_TYPES.STRING),
        Cod_Articolo: new ModelEntry(CELL_TYPES.STRING),
        Descrizione_Prodotto: new ModelEntry(CELL_TYPES.STRING),
        Lotto: new ModelEntry(CELL_TYPES.STRING),
        Elem_Cod: new ModelEntry(CELL_TYPES.NUMBER),
        Pro_Cod: new ModelEntry(CELL_TYPES.NUMBER),
        Mat_Cod: new ModelEntry(CELL_TYPES.STRING)
    }

    constructor(
        private objParametriAgendaService: ObjParametriAgendaService,
        private widgetsClient: WidgetsClient,
        protected injector: Injector,
        protected transloco: TranslocoService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.resizable = new ResizableSettings(true, true);
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false
        });

        this.behavior.excelSettings.enabled = false;
        this.columnMenu.kendoGridColumnChooser = false;
        this.views.enabled = false;
        this.groups.groupable.enabled = false
    }

    public read(): Observable<WidgetProdottoResult> {
        this.isLoading(true);

        const piva = this.objParametriAgendaService.getObjParamValue().Piva;
        return this.widgetsClient
            .widgetsLeggiUltimiMovimentiMagazzino({ NumeroMovimenti: DEFAULT_NUMERO_PRODOTTI, Piva: piva })
            .pipe(
                switchMap(data => of(new WidgetProdottoResult(data.RispostaStringa, this.columns, this.widgetProdottoGridModel))),
                finalize(() => this.isLoading(false))
            )
    }

    public perform(_: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
        return of(rows);
    }
}

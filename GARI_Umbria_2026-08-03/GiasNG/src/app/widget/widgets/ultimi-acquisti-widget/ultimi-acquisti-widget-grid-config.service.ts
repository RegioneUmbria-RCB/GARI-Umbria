import { Injectable, Injector } from '@angular/core';
import { finalize, Observable, of, switchMap, tap } from 'rxjs';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { BehaviorSettings, CommandsColumnSettings, ResizableSettings } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';
import { WidgetsClient } from 'app/Service/api.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';

const DEFAULT_NUMERO_ACQUISTI = 15;

export class WidgetAcquistoResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class WidgetAcquistoGridModel extends KendoGridModel {
    Piva: ModelEntry;
    Sa_cod: ModelEntry;
    Id_Agenda: ModelEntry;
    Lav_Cod: ModelEntry;
    Des_Lib: ModelEntry;
    Fornitore: ModelEntry;
    DataDoc: ModelEntry;
    NrDoc: ModelEntry;
    Prodotto: ModelEntry;
    Udm_Sim: ModelEntry;
    Qta: ModelEntry;
    PrezzoNetto: ModelEntry;
}

@Injectable()
export class UltimiAcquistiWidgetGridConfig extends AbstractGridConfigService<WidgetAcquistoResult> {
    gridId = 'UltimiAcquistiWidgetGrid';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;
    rowId = 'chiave';
    columns: KendoGridColumn[] = [
        new KendoGridColumn(
            { field: 'Des_Lib', title: this.transloco.translate('Descrizione') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 200
            }
        ),
        new KendoGridColumn(
            { field: 'DataDoc', title: this.transloco.translate('DataDocumentoAbbr') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'NrDoc', title: this.transloco.translate('NumeroDocumentoAbbr') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Prodotto', title: this.transloco.translate('Prodotto') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Fornitore', title: this.transloco.translate('Fornitore') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Qta', title: this.transloco.translate('Quantita') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Udm_Sim', title: this.transloco.translate('UnitàDiMisuraAbbr') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'PrezzoNetto', title: this.transloco.translate('PrezzoNetto') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        )
    ];

    widgetAcquistoGridModel: WidgetAcquistoGridModel = {
        Piva: new ModelEntry(CELL_TYPES.STRING),
        Sa_cod: new ModelEntry(CELL_TYPES.NUMBER),
        Id_Agenda: new ModelEntry(CELL_TYPES.NUMBER),
        Lav_Cod: new ModelEntry(CELL_TYPES.NUMBER),
        Des_Lib: new ModelEntry(CELL_TYPES.STRING),
        Fornitore: new ModelEntry(CELL_TYPES.STRING),
        DataDoc: new ModelEntry(CELL_TYPES.DATE),
        NrDoc: new ModelEntry(CELL_TYPES.STRING),
        Prodotto: new ModelEntry(CELL_TYPES.STRING),
        Udm_Sim: new ModelEntry(CELL_TYPES.STRING),
        Qta: new ModelEntry(CELL_TYPES.NUMBER),
        PrezzoNetto: new ModelEntry(CELL_TYPES.NUMBER)
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
        this.groups.groupable.enabled = false;
    }

    public read(): Observable<WidgetAcquistoResult> {
        this.isLoading(true);

        const piva = this.objParametriAgendaService.getObjParamValue().Piva;
        return this.widgetsClient
            .widgetsLeggiUltimiAcquisti({ NumeroMovimenti: DEFAULT_NUMERO_ACQUISTI, Piva: piva })
            .pipe(
                switchMap(data => of(new WidgetAcquistoResult(data.RispostaStringa, this.columns, this.widgetAcquistoGridModel))),
                finalize(() => this.isLoading(false))
            )
    }

    public perform(_: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
        return of(rows);
    }
}

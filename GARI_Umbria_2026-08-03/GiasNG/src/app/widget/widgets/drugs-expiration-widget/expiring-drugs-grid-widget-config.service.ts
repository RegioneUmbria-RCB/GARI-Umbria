import { Injectable, Injector } from "@angular/core";
import { DatePipe } from "@angular/common";

import { CookieService } from "ngx-cookie-service";
import { TranslocoService } from "@jsverse/transloco";
import { Observable, finalize, of, switchMap } from "rxjs";
import { CommandsColumnSettings, RendererGridEvent, ResizableSettings } from 'gias-kendo-grid';

import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction, ConfigTemplate, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, NumericSettings } from 'gias-kendo-grid';

import { Enum_SiteRedirector, enum_PagineAgenda_2010 } from "app/Model/siti.enum";
import { WidgetZooClient, Widget_Zoo_IN } from "app/Service/net-core6-api.service";
import { GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString } from "app/Service/gestione-richieste.service";

const DRUGS_EXPIRATION_COOKIE_NAME = 'DrugsExpirationCookie';

export class WidgetExpiringDrugsResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class WidgetExpiringDrugsGridModel extends KendoGridModel {
    Piva: ModelEntry;
    Impresa: ModelEntry;
    Sa_Cod: ModelEntry;
    Sa_Nome: ModelEntry;
    Id_Destinazione: ModelEntry;
    Fabbricato_Des: ModelEntry;
    Pro_Cod: ModelEntry;
    Descrizione_Prodotto: ModelEntry;
    Lotto: ModelEntry;
    Lotto_Interno: ModelEntry;
    Udm_Cod: ModelEntry;
    Udm_Des: ModelEntry;
    Udm_Sim: ModelEntry;
    Cod_Articolo: ModelEntry;
    Giacenza: ModelEntry;
    Data_Scadenza: ModelEntry;
}

@Injectable()
export class ExpiringDrugsWidgetGridConfig extends AbstractGridConfigService<WidgetExpiringDrugsResult> {
    gridId = 'ExpiringDrugsWidgetGrid';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;
    rowId = 'chiave';
    param: Widget_Zoo_IN = null;

    numericsettings = new NumericSettings({
        defaultValue: 0,
        format: 'n0',
        min: 0,
        step: 1
    });

    columns: KendoGridColumn[] = [
        new KendoGridColumn(
            { field: 'Sa_Nome', title: this.transloco.translate('Centro') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Fabbricato_Des', title: this.transloco.translate('Magazzino') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Lotto', title: this.transloco.translate('Lotto') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Udm_Sim', title: this.transloco.translate('UnitaMisura') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Descrizione_Prodotto', title: this.transloco.translate('Prodotto') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Cod_Articolo', title: this.transloco.translate('CodiceAIC') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Giacenza', title: this.transloco.translate('Giacenza') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Data_Scadenza', title: this.transloco.translate('DataScadenza') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        )
    ];

    widgetExpiringDrugsGridModel: WidgetExpiringDrugsGridModel = {
        Piva: new ModelEntry(CELL_TYPES.STRING),
        Impresa: new ModelEntry(CELL_TYPES.STRING),
        Sa_Cod: new ModelEntry(CELL_TYPES.STRING),
        Sa_Nome: new ModelEntry(CELL_TYPES.STRING),
        Id_Destinazione: new ModelEntry(CELL_TYPES.NUMBER),
        Fabbricato_Des: new ModelEntry(CELL_TYPES.STRING),
        Pro_Cod: new ModelEntry(CELL_TYPES.NUMBER),
        Descrizione_Prodotto: new ModelEntry(CELL_TYPES.STRING),
        Lotto: new ModelEntry(CELL_TYPES.STRING),
        Lotto_Interno: new ModelEntry(CELL_TYPES.STRING),
        Udm_Cod: new ModelEntry(CELL_TYPES.NUMBER),
        Udm_Des: new ModelEntry(CELL_TYPES.STRING),
        Udm_Sim: new ModelEntry(CELL_TYPES.STRING),
        Cod_Articolo: new ModelEntry(CELL_TYPES.STRING),
        Giacenza: new ModelEntry(CELL_TYPES.NUMBER),
        Data_Scadenza: new ModelEntry(CELL_TYPES.DATE)
    };

    constructor(
        private widgetsClient: WidgetZooClient,
        protected injector: Injector,
        protected transloco: TranslocoService,
        private cookies: CookieService,
        private gestioneRichiesteService: GestioneRichiesteService,
        private datePipe: DatePipe
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.resizable = new ResizableSettings(true, true);
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false
        });

        this.behavior.saveExternalChanges = true;
        this.behavior.excelSettings.enabled = false;
        this.columnMenu.kendoGridColumnChooser = true;
        this.views.enabled = true;
        this.groups.groupable.enabled = false;
    }

    public read(): Observable<WidgetExpiringDrugsResult> {
        this.isLoading(true);

        if (!this.param)
            return of(new WidgetExpiringDrugsResult([], this.columns, this.widgetExpiringDrugsGridModel));
        return this.widgetsClient
            .widgetZooGetExpiringDrugs(this.param)
            .pipe(
                switchMap(data => of(new WidgetExpiringDrugsResult(JSON.parse(data.RispostaStringa) as KendoGridRow[], this.columns, this.widgetExpiringDrugsGridModel))),
                finalize(() => this.isLoading(false))
            );
    }

    public perform(_: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
        return of(rows);
    }

    override applyRendererRules(opts: RendererGridEvent): Observable<KendoGridRow[]> {
        this.isLoading(true);

        try {
            const { grid, gridElRef } = { ...opts };
            const rows: [] = grid.data['data'];
            if (!rows || rows.length === 0) return;

            const headerCells: HTMLElement[] = Array.from(
                gridElRef.nativeElement.querySelectorAll('thead th')
            );

            const targetHeader = headerCells.find(th => {
                const titleSpan = th.querySelector('span.k-column-title');
                return titleSpan && titleSpan.textContent?.trim() === this.transloco.translate('GiorniInStalla');
            });

            if (!targetHeader) return;

            const visualColIndex = headerCells.indexOf(targetHeader);

            const trEls = gridElRef.nativeElement.querySelectorAll('tbody tr.k-master-row');
            rows.forEach((row: any, rowIndex: number) => {
                if (row.alertGG) {
                    const tr = trEls[rowIndex];
                    if (tr) {
                        const td = tr.children[visualColIndex];
                        if (td) (td as HTMLElement).style.backgroundColor = 'red';
                    }
                }
            });
        } finally {
            this.isLoading(false);
        }
    }

    getExpiringDrugsCookie(): Widget_Zoo_IN | null {
        const cookie = this.cookies.get(DRUGS_EXPIRATION_COOKIE_NAME);
        return cookie ? JSON.parse(cookie) as Widget_Zoo_IN : null;
    }

    setExpiringDrugsCookie(param: Widget_Zoo_IN) {
        this.cookies.set(DRUGS_EXPIRATION_COOKIE_NAME, JSON.stringify(param), { path: '/' });
    }

    deleteExpiringDrugsCookie() {
        this.cookies.delete(DRUGS_EXPIRATION_COOKIE_NAME);
    }

    redirectToRicercaDocumenti() {
        const parametri: ParametriAggiuntivi_QueryString[] = [
            KeyValuePair.Create("type", "doc"),
            KeyValuePair.Create("du", this.datePipe.transform(this.param.timeStart, 'dd/MM/yyyy HH:mm'))
        ];
        this.gestioneRichiesteService.gestionePassaggioAltroSito(
                Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                enum_PagineAgenda_2010.Pagina_Scadenzario_Lista, parametri
            )
            .then((val) => window.location.href = val);
    }
}
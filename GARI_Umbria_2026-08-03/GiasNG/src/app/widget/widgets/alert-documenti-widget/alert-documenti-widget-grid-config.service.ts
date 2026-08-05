import { DatePipe } from "@angular/common";
import { Injectable, Injector } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { Enum_SiteRedirector, enum_PagineAgenda_2010 } from "app/Model/siti.enum";
import { GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString } from "app/Service/gestione-richieste.service";
import { WidgetDocumentaleClient } from "app/Service/net-core6-api.service";
import { CommandsColumnSettings, ResizableSettings } from 'gias-kendo-grid';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, NumericSettings } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { CookieService } from "ngx-cookie-service";
import { Observable, finalize, of, switchMap } from "rxjs";

export const DEFAULT_TIMESTAMP_DOCUMENTI = new Date(new Date().getFullYear(), 0, 1, 0, 0);
const DOCUMENTI_COOKIE_NAME = 'AlertDocumentiCookie';

export class WidgetDocumentiResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class WidgetDocumentiGridModel extends KendoGridModel {
    company: ModelEntry;
    user_upload: ModelEntry;
    category: ModelEntry;
    count_document: ModelEntry;
    count_considered: ModelEntry;
    count_to_consider: ModelEntry;
}

@Injectable()
export class AlertDocumentiWidgetGridConfig extends AbstractGridConfigService<WidgetDocumentiResult> {

    gridId = 'AlertDocumentiWidgetGrid';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;
    rowId = 'company' + 'user_upload' + 'category' + 'count_document' + 'count_considered' + 'count_to_consider';

    param: Date;

    numericsettings = new NumericSettings({
        defaultValue: 0,
        format: 'n0',
        min: 0,
        step: 1
    });


    columns: KendoGridColumn[] = [
        new KendoGridColumn(
            { field: 'company', title: this.transloco.translate('Azienda') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'user_upload', title: this.transloco.translate('UtenteUpload') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'category', title: this.transloco.translate('Categoria') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'count_document', title: this.transloco.translate('NrDocumenti') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'count_considered', title: this.transloco.translate('Considerati') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'count_to_consider', title: this.transloco.translate('DaConsiderare') },
            {
                resizable: true,
                numeric: this.numericsettings,
                filterable: true,
                editable: false,
                width: 50
            }
        )
    ];

    widgetAcquistoGridModel: WidgetDocumentiGridModel = {
        company: new ModelEntry(CELL_TYPES.STRING),
        user_upload: new ModelEntry(CELL_TYPES.STRING),
        category: new ModelEntry(CELL_TYPES.STRING),
        count_document: new ModelEntry(CELL_TYPES.NUMBER),
        count_considered: new ModelEntry(CELL_TYPES.NUMBER),
        count_to_consider: new ModelEntry(CELL_TYPES.NUMBER)
    }


    constructor(
        private widgetsClient: WidgetDocumentaleClient,
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

        this.behavior.excelSettings.enabled = false;
        this.columnMenu.kendoGridColumnChooser = false;
        this.views.enabled = false;
        this.groups.groupable.enabled = false;

        let cookie = this.getAlertDocumentiCookie();

        if (cookie)
            this.param = new Date(cookie);
        else 
            this.param = DEFAULT_TIMESTAMP_DOCUMENTI;
    }

    public read(): Observable<WidgetDocumentiResult> {
        this.isLoading(true);

        return this.widgetsClient
            .widgetDocumentaleGetDocumentRecap(this.param)
            .pipe(
                switchMap(data => of(new WidgetDocumentiResult(JSON.parse(data.RispostaStringa) as KendoGridRow[], this.columns, this.widgetAcquistoGridModel))),
                finalize(() => this.isLoading(false))
            )
    }

    public perform(_: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
        return of(rows);
    }

    getAlertDocumentiCookie(): string {
        let cookie = this.cookies.get(DOCUMENTI_COOKIE_NAME);
        return cookie.replace(/"/g, '');
    }

    setAlertDocumentiCookie(param: Date) {
        this.cookies.set(
            DOCUMENTI_COOKIE_NAME, JSON.stringify(param), { path: '/' }
        );
        this.param = param;
    }

    deleteAlertDocumentiCookie() {
        this.cookies.delete(DOCUMENTI_COOKIE_NAME);
        this.param = DEFAULT_TIMESTAMP_DOCUMENTI;
    }

    redirectToRicercaDocumenti(){

        const parametri: ParametriAggiuntivi_QueryString[] = [
            KeyValuePair.Create("type", "doc"),
            KeyValuePair.Create("du", this.datePipe.transform(this.param, 'dd/MM/yyyy HH:mm'))
        ];
      
        this.gestioneRichiesteService.gestionePassaggioAltroSito(
                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                    enum_PagineAgenda_2010.Pagina_Scadenzario_Lista,
                    parametri).then((val) => {
                                                window.location.href = val;
        });
    }
}
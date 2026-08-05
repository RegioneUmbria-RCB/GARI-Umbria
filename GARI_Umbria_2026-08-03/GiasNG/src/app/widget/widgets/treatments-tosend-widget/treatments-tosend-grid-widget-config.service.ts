import { Injectable, Injector } from "@angular/core";
import { DatePipe } from "@angular/common";

import { CookieService } from "ngx-cookie-service";
import { TranslocoService } from "@jsverse/transloco";
import { Observable, finalize, of, switchMap } from "rxjs";
import { CommandsColumnSettings, ResizableSettings } from 'gias-kendo-grid';

import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction, ConfigTemplate, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, NumericSettings } from 'gias-kendo-grid';

import { Enum_SiteRedirector, enum_PagineAgronicaSincro } from "app/Model/siti.enum";
import { WidgetZooClient, Widget_Zoo_IN } from "app/Service/net-core6-api.service";
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";

const TREATMENTS_TOSEND_COOKIE_NAME = 'TreatmentsToSendCookie';

export class WidgetTreatmentsToSendResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class WidgetTreatmentsToSendGridModel extends KendoGridModel {
    Id_Ricetta: ModelEntry;
    Id_RigaRicetta: ModelEntry;
    Id_Agenda: ModelEntry;
    PIVA: ModelEntry;
    Rag_Soc: ModelEntry;
    Sa_Cod: ModelEntry;
    Sa_Des: ModelEntry;
    Sta_Num: ModelEntry;
    Sta_Des: ModelEntry;
    Pro_Cod: ModelEntry;
    Pro_Des: ModelEntry;
    Qta: ModelEntry;
    Udm_Cod: ModelEntry;
    Pres_Numero: ModelEntry;
    PresRiga_Numero: ModelEntry;
    Data_Prescrizione: ModelEntry;
    Pres_Tipo: ModelEntry;
    RegSco_Numero: ModelEntry;
    Gruppo_Ricetta: ModelEntry;
    Prot_Numero: ModelEntry;
    Id_Protocollo: ModelEntry;
    Numero_Somm: ModelEntry;
    Num_Somm_Des: ModelEntry;
    Note: ModelEntry;
    GEN_COD: ModelEntry;
    SPE_COD: ModelEntry;
    Azienda_Codice: ModelEntry;
    Prop_IdFiscale: ModelEntry;
    Data_Inizio: ModelEntry;
    Data_Fine: ModelEntry;
    Somm_Tipo: ModelEntry;
    Somm_Stato: ModelEntry;
    Num_Somm_Gruppo: ModelEntry;
}

@Injectable()
export class TreatmentsToSendWidgetGridConfig extends AbstractGridConfigService<WidgetTreatmentsToSendResult> {
    gridId = 'TreatmentsToSendWidgetGrid';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;
    rowId = 'Id_Agenda';
    param: Widget_Zoo_IN;

    numericsettings = new NumericSettings({
        defaultValue: 0,
        format: 'n0',
        min: 0,
        step: 1
    });

    columns: KendoGridColumn[] = [
        new KendoGridColumn(
            { field: 'Prot_Numero', title: this.transloco.translate('NumProtocollo') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Sa_Des', title: this.transloco.translate('Centro') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Sta_Des', title: this.transloco.translate('Stalla') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Pro_Des', title: this.transloco.translate('Farmaco') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Qta', title: this.transloco.translate('Quantitativo') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Pres_Numero', title: this.transloco.translate('PresNumero') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'PresRiga_Numero', title: this.transloco.translate('PresRigaNumero') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Data_Prescrizione', title: this.transloco.translate('DataPrescrizione') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Pres_Tipo', title: this.transloco.translate('TipoPrescrizione') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'RegSco_Numero', title: this.transloco.translate('RegScoNumero') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Num_Somm_Des', title: this.transloco.translate('NumSomministrazione') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Note', title: this.transloco.translate('Note') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Azienda_Codice', title: this.transloco.translate('BDNCodiceAzienda') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Prop_IdFiscale', title: this.transloco.translate('PropIdFiscale') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Data_Inizio', title: this.transloco.translate('Inizio') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Data_Fine', title: this.transloco.translate('Fine') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Somm_Tipo', title: this.transloco.translate('TipoSomministrazione') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        )
    ];

    widgetTreatmentsToSendGridModel: WidgetTreatmentsToSendGridModel = {
        Id_Ricetta: new ModelEntry(CELL_TYPES.NUMBER),
        Id_RigaRicetta: new ModelEntry(CELL_TYPES.NUMBER),
        Id_Agenda: new ModelEntry(CELL_TYPES.NUMBER),
        PIVA: new ModelEntry(CELL_TYPES.STRING),
        Rag_Soc: new ModelEntry(CELL_TYPES.STRING),
        Sa_Cod: new ModelEntry(CELL_TYPES.NUMBER),
        Sa_Des: new ModelEntry(CELL_TYPES.STRING),
        Sta_Num: new ModelEntry(CELL_TYPES.NUMBER),
        Sta_Des: new ModelEntry(CELL_TYPES.STRING),
        Pro_Cod: new ModelEntry(CELL_TYPES.NUMBER),
        Pro_Des: new ModelEntry(CELL_TYPES.STRING),
        Qta: new ModelEntry(CELL_TYPES.NUMBER),
        Udm_Cod: new ModelEntry(CELL_TYPES.NUMBER),
        Tratt_Numero: new ModelEntry(CELL_TYPES.STRING),
        Pres_Numero: new ModelEntry(CELL_TYPES.STRING),
        PresRiga_Numero: new ModelEntry(CELL_TYPES.STRING),
        Data_Prescrizione: new ModelEntry(CELL_TYPES.DATE),
        Pres_Tipo: new ModelEntry(CELL_TYPES.STRING),
        RegSco_Numero: new ModelEntry(CELL_TYPES.STRING),
        Gruppo_Ricetta: new ModelEntry(CELL_TYPES.NUMBER),
        Prot_Numero: new ModelEntry(CELL_TYPES.STRING),
        Id_Protocollo: new ModelEntry(CELL_TYPES.NUMBER),
        Numero_Somm: new ModelEntry(CELL_TYPES.NUMBER),
        Num_Somm_Des: new ModelEntry(CELL_TYPES.STRING),
        Note: new ModelEntry(CELL_TYPES.STRING),
        GEN_COD: new ModelEntry(CELL_TYPES.NUMBER),
        SPE_COD: new ModelEntry(CELL_TYPES.NUMBER),
        Azienda_Codice: new ModelEntry(CELL_TYPES.STRING),
        Prop_IdFiscale: new ModelEntry(CELL_TYPES.STRING),
        Data_Inizio: new ModelEntry(CELL_TYPES.DATE),
        Data_Fine: new ModelEntry(CELL_TYPES.DATE),
        Somm_Tipo: new ModelEntry(CELL_TYPES.STRING),
        Somm_Stato: new ModelEntry(CELL_TYPES.STRING),
        Num_Somm_Gruppo: new ModelEntry(CELL_TYPES.NUMBER)
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

    public read(): Observable<WidgetTreatmentsToSendResult> {
        this.isLoading(true);

        this.param.timeStart.setHours(0, 0, 0, 0);

        return this.widgetsClient
            .widgetZooGetTreatmentsToSend(this.param)
            .pipe(
                switchMap(data => of(new WidgetTreatmentsToSendResult(JSON.parse(data.RispostaStringa) as KendoGridRow[], this.columns, this.widgetTreatmentsToSendGridModel))),
                finalize(() => this.isLoading(false))
            );
    }

    public perform(_: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
        return of(rows);
    }

    getTreatmentsToSendCookie(): Widget_Zoo_IN | null {
        const cookie = this.cookies.get(TREATMENTS_TOSEND_COOKIE_NAME);
        return cookie ? JSON.parse(cookie) as Widget_Zoo_IN : null;
    }

    setTreatmentsToSendCookie(param: Widget_Zoo_IN) {
        this.cookies.set(TREATMENTS_TOSEND_COOKIE_NAME, JSON.stringify(param), { path: '/' });
    }

    deleteTreatmentsToSendCookie() {
        this.cookies.delete(TREATMENTS_TOSEND_COOKIE_NAME);
    }

    redirectToTreatmentsToSend() {
        const chiave = this.param.Piva + '_' + this.param.Sa_Cod + '_' + this.param.Sta_Num;
        this.gestioneRichiesteService.gestionePassaggioAltroSito(
            Enum_SiteRedirector.Sito_AgronicaSincronizzatore,
            enum_PagineAgronicaSincro.InvioTrattamentiZooVetInfo,
            [
                { key: "tipoSincro", value: "1", codifica: false },
                { key: "chiave", value: chiave, codifica: true }
            ])
        .then((val) => window.location.href = val);
    }
}
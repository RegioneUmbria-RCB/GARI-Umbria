import { CookieService } from "ngx-cookie-service";
import { Injectable, Injector } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { Observable, filter, finalize, forkJoin, from, map, of, switchMap, takeUntil } from "rxjs";

import { CELL_TYPES, Enum_DBTypeOperation, ObjParametriAgenda } from 'gias-ui-kit';
import { CommandsColumnSettings, CustomColumnSettings, ResizableSettings } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction, ConfigTemplate, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, NumericSettings } from 'gias-kendo-grid';

import { Enum_SiteRedirector } from "app/Model/siti.enum";
import { enum_PagineGiasNG } from "app/Model/TipiEnumerativi";
import { enum_TypeTab_Zootecnia } from 'app/zoo/models/tipi-enumerativi-zoo';

import { enum_menuZooGridCommands } from "app/zoo/zoo.utils";
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { PrescrizioniClient, WidgetZooClient, Widget_Zoo_IN } from "app/Service/net-core6-api.service";

const TREATMENTS_TODO_COOKIE_NAME = 'TreatmentsToDoCookie';

export class WidgetTreatmentsToDoResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class WidgetTreatmentsToDoGridModel extends KendoGridModel {
    IdProt: ModelEntry;
    Numero: ModelEntry;
    Piva: ModelEntry;
    Impresa: ModelEntry;
    SaCod: ModelEntry;
    SaDes: ModelEntry;
    StaNum: ModelEntry;
    StaDes: ModelEntry;
    FamigliaAic: ModelEntry;
    FarmacoDes: ModelEntry;
    Capi: ModelEntry;
}

@Injectable()
export class TreatmentsToDoWidgetGridConfig extends AbstractGridConfigService<WidgetTreatmentsToDoResult> {
    gridId = 'TreatmentsToDoWidgetGrid';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;
    rowId = 'chiave';
    param: Widget_Zoo_IN;

    public stopLoading = false;

    numericsettings = new NumericSettings({
        defaultValue: 0,
        format: 'n0',
        min: 0,
        step: 1
    });

    columns: KendoGridColumn[] = [
        new KendoGridColumn(
            { field: 'Numero', title: this.transloco.translate('NumProtocollo') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'SaDes', title: this.transloco.translate('Centro') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'StaDes', title: this.transloco.translate('Stalla') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'FamigliaAic', title: this.transloco.translate('FamigliaAic') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'FarmacoDes', title: this.transloco.translate('Farmaco') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Capi', title: this.transloco.translate('Capi') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        )
    ];

    widgetTreatmentsToDoGridModel: WidgetTreatmentsToDoGridModel = {
        IdProt: new ModelEntry(CELL_TYPES.NUMBER),
        Numero: new ModelEntry(CELL_TYPES.STRING),
        Piva: new ModelEntry(CELL_TYPES.STRING),
        Impresa: new ModelEntry(CELL_TYPES.STRING),
        SaCod: new ModelEntry(CELL_TYPES.NUMBER),
        SaDes: new ModelEntry(CELL_TYPES.STRING),
        StaNum: new ModelEntry(CELL_TYPES.NUMBER),
        StaDes: new ModelEntry(CELL_TYPES.STRING),
        FamigliaAic: new ModelEntry(CELL_TYPES.STRING),
        FarmacoDes: new ModelEntry(CELL_TYPES.STRING),
        Capi: new ModelEntry(CELL_TYPES.STRING),
    };

    constructor(
        private widgetsClient: WidgetZooClient,
        protected agendaClient: ObjParametriAgendaService,
        protected prescriptionsClient: PrescrizioniClient,
        protected injector: Injector,
        protected transloco: TranslocoService,
        private cookies: CookieService,
        private gestioneRichieste: GestioneRichiesteService,
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.resizable = new ResizableSettings(true, true);
        this.cmdColumn = new CommandsColumnSettings({editBtn: false, infoBtn: false, removeBtn: false});

        this.behavior.saveExternalChanges = true;
        this.behavior.excelSettings.enabled = false;
        this.columnMenu.kendoGridColumnChooser = true;
        this.views.enabled = true;
        this.groups.groupable.enabled = false;

        this.customColumn = new CustomColumnSettings({
            title: '',
            showColumn: true,
            useCustomColumnCellTemplate: true
        });
        this.gridPublicService.commandEvent
            .pipe(
                takeUntil(this.signal),
                filter(cmdEvent => !!cmdEvent)
            )
            .subscribe(cmd => {
              switch (cmd.command.action) {
                case enum_menuZooGridCommands.RegisterOperationToAgenda:
                  this.createOperationFromProtocol(cmd.dataItem);
              }
        });
        this.stopLoading = true;
    }

    public read(): Observable<WidgetTreatmentsToDoResult> {
        if (this.stopLoading)
            return of(new WidgetTreatmentsToDoResult([], this.columns, this.widgetTreatmentsToDoGridModel));

        this.isLoading(true);

        this.param.timeStart.setHours(0, 0, 0, 0);

        return this.widgetsClient
            .widgetZooGetTreatmentsToDo(this.param)
            .pipe(
                switchMap(data => of(new WidgetTreatmentsToDoResult(JSON.parse(data.RispostaStringa) as KendoGridRow[], this.columns, this.widgetTreatmentsToDoGridModel))),
                finalize(() => this.isLoading(false))
            );
    }

    public perform(_: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
        return of(rows);
    }

    getTreatmentsToDoCookie(): Widget_Zoo_IN | null {
        const cookie = this.cookies.get(TREATMENTS_TODO_COOKIE_NAME);
        return cookie ? JSON.parse(cookie) as Widget_Zoo_IN : null;
    }

    setTreatmentsToDoCookie(param: Widget_Zoo_IN) {
        this.cookies.set(TREATMENTS_TODO_COOKIE_NAME, JSON.stringify(param), { path: '/' });
    }

    deleteTreatmentsToDoCookie() {
        this.cookies.delete(TREATMENTS_TODO_COOKIE_NAME);
    }

    public toAgenda(dataItem: any) {
      this.createOperationFromProtocol(dataItem);
    }

    private createOperationFromProtocol(protocol: any) {
        let queryParams: { [key:string] : string | string[] } = {"t_Tab":enum_TypeTab_Zootecnia.FuturePrescriptions.toString()};
        forkJoin([
            from(
                this.gestioneRichieste.gestionePassaggioAltroSito(
                    Enum_SiteRedirector.GiasNG,
                    enum_PagineGiasNG.Pagina_Trattamento_Zoo
                )
            ),
            this.prescriptionsClient.prescrizioniGetAttivitaFromPrescrizione(protocol.IdProt)
                .pipe(map(r => r.RispostaOK ? r.RispostaStringa as any : null)
            )
        ]).subscribe(([redirectUrl, attivita]) => {
        const objP = this.agendaClient.getObjParamValue() as ObjParametriAgenda;
        objP.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        objP.GenericObj_string = JSON.stringify(attivita);
        this.agendaClient.navigateTo(redirectUrl, queryParams, objP, false);
        });
  }
}
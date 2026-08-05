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

const INVALID_ANIMALS_COOKIE_NAME = 'InvalidAnimalsCookie';

export class WidgetInvalidAnimalsResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class WidgetInvalidAnimalsGridModel extends KendoGridModel {
    sa_nome: ModelEntry;
    STA_DES: ModelEntry;
    Raggruppamento_Des: ModelEntry;
    BDN_Codice_Azienda: ModelEntry;
    Matricola: ModelEntry;
    Sesso: ModelEntry;
    Dat_Nascita: ModelEntry;
    giorni_in_stalla: ModelEntry;
    Lotto: ModelEntry;
    SPE_DES: ModelEntry;
    RAZ_DES: ModelEntry;
    IPRO_DES: ModelEntry;
    Stato_Des: ModelEntry;
    Mat_Madre: ModelEntry;
    RazDes_Madre: ModelEntry;
    Mat_Padre: ModelEntry;
    RazDes_Padre: ModelEntry;
    Metodo_Produzione: ModelEntry;
    Validita_Inizio: ModelEntry;
    Validita_Fine: ModelEntry;
    alertGG: ModelEntry;
}

@Injectable()
export class InvalidAnimalsWidgetGridConfig extends AbstractGridConfigService<WidgetInvalidAnimalsResult> {
    gridId = 'InvalidAnimalsWidgetGrid';
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
            { field: 'sa_nome', title: this.transloco.translate('Centro') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'STA_DES', title: this.transloco.translate('Stalla') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Raggruppamento_Des', title: this.transloco.translate('SottoGruppoStalla') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'BDN_Codice_Azienda', title: this.transloco.translate('BDNCodiceAzienda') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'Matricola', title: this.transloco.translate('Matricola') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Sesso', title: this.transloco.translate('Sesso') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Dat_Nascita', title: this.transloco.translate('DataNascita') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'giorni_in_stalla', title: this.transloco.translate('GiorniInStalla') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Lotto', title: this.transloco.translate('LottoZoo') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 100
            }
        ),
        new KendoGridColumn(
            { field: 'SPE_DES', title: this.transloco.translate('Specie') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'RAZ_DES', title: this.transloco.translate('Razza') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'IPRO_DES', title: this.transloco.translate('IndirizzoProduttivo') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Stato_Des', title: this.transloco.translate('Stato') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Mat_Madre', title: this.transloco.translate('MatricolaMadre') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'RazDes_Madre', title: this.transloco.translate('RazzaMadre') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Mat_Padre', title: this.transloco.translate('MatricolaPadre') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'RazDes_Padre', title: this.transloco.translate('RazzaPadre') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Metodo_Produzione', title: this.transloco.translate('MetodoProduzione') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Validita_Inizio', title: this.transloco.translate('ValiditaInizio') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        ),
        new KendoGridColumn(
            { field: 'Validita_Fine', title: this.transloco.translate('ValiditaFine') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 50
            }
        )
    ];

    widgetInvalidAnimalsGridModel: WidgetInvalidAnimalsGridModel = {
        sa_nome: new ModelEntry(CELL_TYPES.STRING),
        STA_DES: new ModelEntry(CELL_TYPES.STRING),
        Raggruppamento_Des: new ModelEntry(CELL_TYPES.STRING),
        BDN_Codice_Azienda: new ModelEntry(CELL_TYPES.STRING),
        Matricola: new ModelEntry(CELL_TYPES.STRING),
        Sesso: new ModelEntry(CELL_TYPES.STRING),
        Dat_Nascita: new ModelEntry(CELL_TYPES.DATE),
        giorni_in_stalla: new ModelEntry(CELL_TYPES.NUMBER),
        Lotto: new ModelEntry(CELL_TYPES.STRING),
        SPE_DES: new ModelEntry(CELL_TYPES.STRING),
        RAZ_DES: new ModelEntry(CELL_TYPES.STRING),
        IPRO_DES: new ModelEntry(CELL_TYPES.STRING),
        Stato_Des: new ModelEntry(CELL_TYPES.STRING),
        Mat_Madre: new ModelEntry(CELL_TYPES.STRING),
        RazDes_Madre: new ModelEntry(CELL_TYPES.STRING),
        Mat_Padre: new ModelEntry(CELL_TYPES.STRING),
        RazDes_Padre: new ModelEntry(CELL_TYPES.STRING),
        Metodo_Produzione: new ModelEntry(CELL_TYPES.STRING),
        Validita_Inizio: new ModelEntry(CELL_TYPES.DATE),
        Validita_Fine: new ModelEntry(CELL_TYPES.DATE),
        alertGG: new ModelEntry(CELL_TYPES.BOOLEAN)
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

    public read(): Observable<WidgetInvalidAnimalsResult> {
        this.isLoading(true);

        if (!this.param) 
            return of(new WidgetInvalidAnimalsResult([], this.columns, this.widgetInvalidAnimalsGridModel));

        return this.widgetsClient
            .widgetZooGetInvalidAnimals(this.param)
            .pipe(
                switchMap(data => of(new WidgetInvalidAnimalsResult(JSON.parse(data.RispostaStringa) as KendoGridRow[], this.columns, this.widgetInvalidAnimalsGridModel))),
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

    getInvalidAnimalsCookie(): Widget_Zoo_IN | null {
        const cookie = this.cookies.get(INVALID_ANIMALS_COOKIE_NAME);
        return cookie ? JSON.parse(cookie) as Widget_Zoo_IN : null;
    }

    setInvalidAnimalsCookie(param: Widget_Zoo_IN) {
        this.cookies.set(INVALID_ANIMALS_COOKIE_NAME, JSON.stringify(param), { path: '/' });
    }

    deleteInvalidAnimalsCookie() {
        this.cookies.delete(INVALID_ANIMALS_COOKIE_NAME);
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
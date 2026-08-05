import { Injectable, Injector } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { SMARTPHONE_WIDTH } from "app/Model/CostantiPersonalizzate";
import { ImpreseService } from "app/Service/Anagrafica/imprese.service";
import { MasterService } from "app/Service/master.service";
import { CommandsColumnSettings } from 'gias-kendo-grid';
import { EditingMode, KendoGridColumn, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, map, Observable, of } from "rxjs";
import { ImpostazioniAziendeCentriService } from "../../../../services/impostazioni/impostazioni-aziende-centri.service";
import { distinct } from "@progress/kendo-data-query";
import { AziendaCentro } from "../../../impostazioni.model";


export class SettigsAziendeCentriServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}

@Injectable({ providedIn: 'root' })
export class ImpostazioniAziendeCentriGridConfigService extends AbstractGridConfigService<SettigsAziendeCentriServerResult> {
    editingMode: EditingMode = EditingMode.IN_PAGE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'Azienda_Cod';
    gridId = 'ImpAziendeCentriGrid';

    permessoEdit = false;
    permessoRemove = false;
    permessoInfo = false;

    private kendoRows = [];
    private kendoModel = {
        Attivo: new ModelEntry(CELL_TYPES.STRING),
        Cap: new ModelEntry(CELL_TYPES.STRING),
        chiave: new ModelEntry(CELL_TYPES.STRING),
        Codice_Cuaa: new ModelEntry(CELL_TYPES.STRING),
        Codice_Fiscale: new ModelEntry(CELL_TYPES.STRING),
        Codice_Socio: new ModelEntry(CELL_TYPES.STRING),
        Com: new ModelEntry(CELL_TYPES.STRING),
        Com_Cod_Istat: new ModelEntry(CELL_TYPES.STRING),
        Contratto_Produzione: new ModelEntry(CELL_TYPES.STRING),
        Cooperativa_Referente: new ModelEntry(CELL_TYPES.STRING),
        Data_Creazione: new ModelEntry(CELL_TYPES.STRING),
        Data_Modifica: new ModelEntry(CELL_TYPES.STRING),
        frz_des: new ModelEntry(CELL_TYPES.STRING),
        ind_des: new ModelEntry(CELL_TYPES.STRING),
        Indirizzo: new ModelEntry(CELL_TYPES.STRING),
        Num_Padri: new ModelEntry(CELL_TYPES.STRING),
        Piva: new ModelEntry(CELL_TYPES.STRING),
        Piva_Padre: new ModelEntry(CELL_TYPES.STRING),
        partitaIvaReale: new ModelEntry(CELL_TYPES.STRING),
        Pro_Cod_Istat: new ModelEntry(CELL_TYPES.STRING),
        Prov: new ModelEntry(CELL_TYPES.STRING),
        Provincia: new ModelEntry(CELL_TYPES.STRING),
        Rag_Soc: new ModelEntry(CELL_TYPES.STRING),
        Rag_Soc_Padre: new ModelEntry(CELL_TYPES.STRING),
        SAU_Totale: new ModelEntry(CELL_TYPES.STRING),
        Sa_Cod: new ModelEntry(CELL_TYPES.STRING),
        Sa_Nome: new ModelEntry(CELL_TYPES.STRING),
        Stato: new ModelEntry(CELL_TYPES.STRING),
        Stato_Cod: new ModelEntry(CELL_TYPES.STRING),
        Superficie_Tare: new ModelEntry(CELL_TYPES.STRING),
        Superficie_Totale: new ModelEntry(CELL_TYPES.STRING),
        Tecnico_Referente: new ModelEntry(CELL_TYPES.STRING),
        Utente_Creazione: new ModelEntry(CELL_TYPES.STRING),
        Utente_Modifica: new ModelEntry(CELL_TYPES.STRING),
        Validita_Inizio: new ModelEntry(CELL_TYPES.STRING),
        Validita_Fine: new ModelEntry(CELL_TYPES.STRING),
    };

    private kendoColumns: KendoGridColumn[] = [
        new KendoGridColumn({ field: 'Rag_Soc', title: this.translocoService.translate('RagioneSociale') }, { editable: false }),
        new KendoGridColumn({ field: 'Codice_Cuaa', title: this.translocoService.translate('CodiceUnicoAziendaAgricolaSigla') }, { editable: false }),
        new KendoGridColumn({ field: 'Codice_Socio', title: this.translocoService.translate('CodiceSocio') }, { editable: false }),
        new KendoGridColumn({ field: 'partitaIvaReale', title: this.translocoService.translate('PartitaIVA') }, { editable: false }),
        new KendoGridColumn({ field: 'Indirizzo', title: this.translocoService.translate('Indirizzo') }, { editable: false }),
        new KendoGridColumn({ field: 'Sa_Cod', title: this.translocoService.translate('CentroCodice') }, { editable: false, hidden: true }),
        new KendoGridColumn({ field: 'Sa_Nome', title: this.translocoService.translate('NomeCentro') }, { editable: false, hidden: true }),
    ];

    constructor(injector: Injector,
        private impreseService: ImpreseService,
        private ACService: ImpostazioniAziendeCentriService,
        private masterService: MasterService,
        private translocoService: TranslocoService
    ) {
        super(injector);
        this.handleCustomization();
        this.handleEvents();
    }

    read(options?: any): Observable<SettigsAziendeCentriServerResult> {
        if (this.ACService.imprese.length > 0) {
            return of(new SettigsAziendeCentriServerResult(
                this.kendoModel, this.kendoColumns, this.ACService.imprese
            ));
        }
        this.masterService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
        return this.impreseService.leggiImprese().pipe(map(srvRes => {
            this.kendoRows = [];
            let distinctImprese = Object.fromEntries(srvRes.kendo_rows.map(x => [x['Piva'], x]));
            for (let piva in distinctImprese) {
                distinctImprese[piva]['Sa_Cod'] = 0;
                distinctImprese[piva]['Sa_Nome'] = 'Tutti i centri';
                distinctImprese[piva]['piva'] = piva;
                this.kendoRows.push(distinctImprese[piva]);
            }
            this.ACService.imprese = this.kendoRows;
            this.masterService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
            return new SettigsAziendeCentriServerResult(this.kendoModel, this.kendoColumns, this.kendoRows);
        }));
    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        return from([]);
    }

    private handleCustomization() {
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: this.permessoEdit,
            infoBtn: this.permessoInfo,
            removeBtn: this.permessoRemove,
            onDisableInfoBtn: () => false,
        });

        this.selectable.selectable.checkboxOnly = false;
        this.selectable.columnSettings.showSelectAll = true;
        this.selectable.shouldShowCheckbox = true;
        this.selectable.selectable.drag = true;
        this.selectable.selectable.enabled = true;
        this.selectable.selectable.mode = "multiple";
        // this.selectable.selectable.enabled = this.permessoEdit;

        this.resizable.autoFitColumns = false;
        this.resizable.isResizable = true;
        if (window.innerWidth < SMARTPHONE_WIDTH) {
            this.groups.groupable.enabled = false;
            this.views.enabled = false;
            this.cmdColumn.editBtn = false;
            this.toolbar.newItem = false;
        }
    }

    private handleEvents() {
        this.ACService.modeChangeEvent.GiasSubscribe(event => {
            if (event.type === 'centersToggled') {
                let newData: SettigsAziendeCentriServerResult;
                if (event.value) {
                    this.kendoColumns.find(c => c.field === 'Sa_Nome').hidden = false;
                    newData = new SettigsAziendeCentriServerResult(
                        this.kendoModel,
                        this.kendoColumns,
                        this.ACService.ImpresexCentri
                    );
                } else {
                    this.kendoColumns.find(c => c.field === 'Sa_Nome').hidden = true;
                    newData = new SettigsAziendeCentriServerResult(
                        this.kendoModel,
                        this.kendoColumns,
                        this.ACService.imprese
                    );
                }
                this.gridPublicService.refresh(false, newData);
            }
        });
    }

}

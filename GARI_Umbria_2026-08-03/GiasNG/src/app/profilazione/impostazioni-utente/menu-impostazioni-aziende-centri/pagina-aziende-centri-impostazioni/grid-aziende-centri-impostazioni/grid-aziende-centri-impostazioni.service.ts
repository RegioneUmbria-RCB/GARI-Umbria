import { Injectable, Injector } from "@angular/core";
import { SMARTPHONE_WIDTH } from "app/Model/CostantiPersonalizzate";
import {
    ImpostazioniAziendeCentriService,
    ImpresaDTO
} from "../../../../services/impostazioni/impostazioni-aziende-centri.service";
import {
    CommandsColumnSettings, DeletionMode,
    DettagliColumnSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
    EditingMode,
    KendoGridColumn,
    KendoServerResult,
    LoaderType,
    ModelEntry
} from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, map, Observable } from "rxjs";
import { ProfilazioneDataShareService } from '../../../../services/profilazione-data-share.service';
import { ImpostazioniFormService } from "../../../../services/impostazioni/impostazioni-form.service";
import { Impostazione } from 'gias-ui-kit';
import { GiasDialogService } from "../../../../../Service/gias-dialog.service";
import { enum_TipoControllo } from 'gias-ui-kit';
import { Imprese_Impostazioni } from "../../../../../Service/master.service";

export class GridAziendeImpostazioniServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}

@Injectable({ providedIn: 'root' })
export class GridAziendeCentriImpostazioniGridConfigService extends AbstractGridConfigService<GridAziendeImpostazioniServerResult> {
    editingMode: EditingMode = EditingMode.IN_PAGE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'key';
    gridId = 'AziendeXImpostazioni';

    kendoRows = [];
    kendoColumns = [
        new KendoGridColumn({ field: 'partitaIvaReale', title: this.transloco.translate('PartitaIVA') }, { editable: false }),
        new KendoGridColumn({ field: 'rag_soc', title: this.transloco.translate('RagioneSociale') }, { editable: false }),
        new KendoGridColumn({ field: 'sa_nome', title: this.transloco.translate('NomeCentro') }, { editable: false }),
        new KendoGridColumn({ field: 'Sa_Cod', title: this.transloco.translate('CentroCodice') }, { editable: false, hidden: true }),
        new KendoGridColumn({ field: 'Impostazione_Des', title: this.transloco.translate('Impostazione') }, { editable: false }),
        new KendoGridColumn({ field: 'Flag_InApp', title: this.transloco.translate('UsataInApp') }, { editable: false }),
        new KendoGridColumn({ field: 'Username_Modifica', title: this.transloco.translate('UltimaModificaDi') }, { editable: false, hidden: true }),
        new KendoGridColumn({ field: 'Data_Modifica', title: this.transloco.translate('DataModifica') }, { editable: false, hidden: true }),
    ];
    kendoModel = {
        Piva: new ModelEntry(CELL_TYPES.STRING),
        rag_soc: new ModelEntry(CELL_TYPES.STRING),
        Sa_Cod: new ModelEntry(CELL_TYPES.STRING),
        sa_nome: new ModelEntry(CELL_TYPES.STRING),
        Impostazione_Cod: new ModelEntry(CELL_TYPES.NUMBER),
        Impostazione_Des: new ModelEntry(CELL_TYPES.STRING),
        Impostazione_Valore: new ModelEntry(CELL_TYPES.STRING),
        Tipo_Campo: new ModelEntry(CELL_TYPES.STRING),
        Flag_InApp: new ModelEntry(CELL_TYPES.STRING),
        Username_Modifica: new ModelEntry(CELL_TYPES.STRING),
        Data_Modifica: new ModelEntry(CELL_TYPES.DATE),
        partitaIvaReale: new ModelEntry(CELL_TYPES.STRING),
    };

    permessoEdit = true;
    permessoInfo = true;
    permessoRemove = true;

    constructor(injector: Injector,
        private dialog: GiasDialogService,
        private ACSettings: ImpostazioniAziendeCentriService,
        private impostazioniService: ImpostazioniFormService,
        private dataShare: ProfilazioneDataShareService
    ) {
        super(injector);
        this.handleCustomization();
    }

    read(options?: any): Observable<GridAziendeImpostazioniServerResult> {
        return this.ACSettings.leggiImpreseImpostazioni().pipe(map(ii => {
            ii.filter(i => !i.sa_nome).forEach(i => i.sa_nome = this.transloco.translate('TuttiICentriAziendali'));
            ii.filter(i => this.settingNeedsNumericValue(i))
                .forEach(i => i.Impostazione_Valore = +i.Impostazione_Valore);
            this.kendoRows = ii;
            return new GridAziendeImpostazioniServerResult(this.kendoModel, this.kendoColumns, this.kendoRows);
        }));
    }

    perform(actionType: HttpAction, item: any | any[]): Observable<any[]> {
        if (actionType === 'destroy') {
            let toDelete = [];
            let content = '';
            if (Array.isArray(item)) {
                content = 'prof.WarningResetDefaults';
                toDelete = item.map(i => new Imprese_Impostazioni(i.Piva, i.Impostazione_Cod, i.Impostazione_Valore, i.Sa_Cod));
            } else {
                content = 'prof.WarningResetDefault';
                toDelete = [new Imprese_Impostazioni(item.Piva, item.Impostazione_Cod, item.Impostazione_Valore, item.Sa_Cod)];
            }
            this.dialog.warningThen('Attenzione', content, true, () => this.delete(toDelete));
        }
        return from([]);
    }

    private handleCustomization() {
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: this.permessoRemove,
        });
        this.dettagliColumn = new DettagliColumnSettings({
            editBtn: false,
            // editBtn: this.permessoEdit,
            // edit: (data) => {
            //     this.ACSettings.impreseSelezionate = [new ImpresaDTO(data.Piva, data.Sa_Cod, data.rag_soc, data.sa_nome)];
            //     const guidaImpostazione = new Impostazione(data.Impostazione_Cod, data.Impostazione_Des, data.Tipo_Campo);
            //     guidaImpostazione.value = data.Impostazione_Valore;
            //     this.dataShare.cardInfo.next(guidaImpostazione);
            // }
        });

        // this.selectable.selectable.enabled = this.permessoEdit;
        this.selectable.selectable.enabled = true;
        this.selectable.selectable.drag = true;
        this.selectable.selectable.checkboxOnly = false;
        this.selectable.columnSettings.showSelectAll = true;
        this.selectable.shouldShowCheckbox = true;
        this.behavior.deletionMode = DeletionMode.HandleAllRowsTogether;

        this.resizable.autoFitColumns = false;
        this.resizable.isResizable = true;
        if (window.innerWidth < SMARTPHONE_WIDTH) {
            this.groups.groupable.enabled = false;
            this.views.enabled = false;
            this.cmdColumn.editBtn = false;
            this.toolbar.newItem = false;
        }
    }

    private delete(settings: Imprese_Impostazioni[]) {
        this.ACSettings.resetImpostazioni(settings)
            .subscribe(() => this.gridPublicService.refresh(true));
    }

    private updateRows(rowIndex, dataItem) {
        let iImpresa = this.ACSettings.impreseInModifica.findIndex(i =>
            i.impresa.Piva === dataItem.Piva && i.impresa.Sa_Cod === dataItem.Sa_Cod);
        let impresa = this.ACSettings.impreseInModifica.at(iImpresa);

        if (iImpresa !== -1) {
            let iImpostazione = impresa.impostazioni.findIndex(i => i.Impostazione_Cod === dataItem.Impostazione_Cod);

            while (iImpostazione !== -1) {
                this.ACSettings.impreseInModifica.at(iImpresa).impostazioni.splice(iImpostazione, 1);
                iImpostazione = impresa.impostazioni.findIndex(i => i.Impostazione_Cod === dataItem.Impostazione_Cod);
            }

            if (impresa.impostazioni.length === 0) {
                this.ACSettings.impreseInModifica.splice(iImpresa, 1);
            }
        }

        this.kendoRows.splice(rowIndex, 1);
    }

    private refreshView() {
        const grid = new GridAziendeImpostazioniServerResult(this.kendoModel, this.kendoColumns, this.kendoRows);
        this.gridPublicService.refresh(false, grid);
    }

    private nonEsisteRigaEdit(impresa: ImpresaDTO, impostazione: Impostazione) {
        return this.kendoRows.find(krow => krow.Impostazione_Cod === impostazione.Impostazione_Cod
            && krow.piva === impresa.piva
            && krow.Sa_Cod === impresa.Sa_Cod
        ) === undefined;
    }

    private aggiungiRigaImpostazione(impresa, impostazione) {
        let guida: Impostazione = this.impostazioniService.findGuidaImpostazione(impostazione.Impostazione_Cod);
        guida['key'] = impresa.piva + "_" + impresa.Sa_Cod + "_" + guida.Impostazione_Cod;
        guida['piva'] = impresa.piva;
        guida['Sa_Cod'] = impresa.Sa_Cod;
        guida['Rag_Soc'] = impresa.rag_soc;
        guida['Sa_Nome'] = impresa.Sa_Nome;
        guida['Valore'] = impostazione.Valore;
        guida.Flag_InApp = guida.Flag_InApp ? this.transloco.translate('Si') : this.transloco.translate('No');
        this.kendoRows.push(guida);
    }

    private settingNeedsNumericValue(data): boolean {
        return data.Tipo_Campo === enum_TipoControllo.CASELLA_SPUNTA;
    }

}


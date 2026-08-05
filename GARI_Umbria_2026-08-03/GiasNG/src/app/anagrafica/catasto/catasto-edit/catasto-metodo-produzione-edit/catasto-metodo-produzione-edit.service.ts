import { Inject, Injectable, Injector } from '@angular/core';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable } from 'rxjs';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import {  DateSettings, DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { map, takeUntil } from 'rxjs/operators';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { ExcelSettings, PDFSettings, ToolbarSettings } from 'gias-kendo-grid';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { Validators } from '@angular/forms';
import { MetodoProduzioneId, MetodoProduzioneService } from './MetodoProduzioneService';
import { MetodoProduzione } from 'app/Model/metaschema/MetodoProduzione';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { CatastoFactoryService, CATASTO_SERVICE_TOKEN } from 'app/Service/ServiceFactory/catasto.factory.service';
import { TranslocoService } from '@jsverse/transloco';
import { ObjParametriAgenda } from 'gias-ui-kit';


export class MetodoProduzioneKendo extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable({
    providedIn:'root'
})

export class MetodoProduzioneParticellaService extends AbstractGridConfigService<MetodoProduzioneKendo>{
    gridId = 'MetodoProduzioneParticella';
    rowId = 'id';

    edit: boolean;
    objParametriAgenda: ObjParametriAgenda;
    toolbar = new ToolbarSettings(true, false);

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;

    kendoColumns: Array<KendoGridColumn>;

    kendoModel: KendoGridModel = {
        id: {
            editable: false,
            type: CELL_TYPES.STRING
        },
        MetodoProduzione: {
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST,
            validators: [Validators.required]
        },
        Validita_Inizio: {
            editable: true,
            type: CELL_TYPES.DATE,
            validators: [Validators.required]
        },
        Validita_Fine: {
            editable: true,
            type: CELL_TYPES.DATE,
            validators: [Validators.required]
        }
    };

    constructor(injector: Injector,
        private metodoProduzioneCatastoService: MetodoProduzioneService,
        private ObjParametriAgendaService: ObjParametriAgendaService,
        protected transloco: TranslocoService,
        @Inject(CATASTO_SERVICE_TOKEN) private catastoService: CatastoFactoryService) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.objParametriAgenda = this.ObjParametriAgendaService.getObjParamValue();
        this.edit = true;
        if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
            this.edit = false;
        }

        // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
        this.cmdColumn.editBtn = false;
        this.cmdColumn.infoBtn = false;
        this.cmdColumn.removeBtn = this.edit;
        this.toolbar.newItem = this.edit;

        // this.resizable.autoFitColumns = true;
        this.resizable.isResizable = true;

        // Setting generali
        this.columnMenu.columnMenu = false;
        this.columnMenu.filterable = false;
        this.behavior.saveExternalChanges = true;
        this.behavior.excelSettings = new ExcelSettings({enabled: false});
        this.behavior.pdfSettings = new PDFSettings({enabled: false});
        this.generalSettings.reordable = false;
        this.generalSettings.performOnEdit = true;
        this.groups.groupable.enabled = false;
        this.views.enabled = false;
        this.pagination.pageable = false;
        this.kendoColumns = [
        // new KendoGridColumn(
        //     { field: "id", title: "id" }, { resizable: true, editable: false}
        // ),
        new KendoGridColumn(
            { field: 'MetodoProduzione', title: this.transloco.translate('MetodoProduzione') },
            { resizable: true, editable: this.edit, validators: [Validators.required], width: 135 }
        ),
        new KendoGridColumn(
            { field: 'Validita_Inizio', title: this.transloco.translate('ValiditàInizio') },
            { resizable: true, editable: this.edit, date: new DateSettings({ defaultValue: AGRODATAINIZIO }), validators: [Validators.required], width: 135 }
        ),
        new KendoGridColumn(
            { field: 'Validita_Fine', title: this.transloco.translate('ValiditàFine') },
            { resizable: true, editable: this.edit, date: new DateSettings({ defaultValue: AGRODATAFINE }), validators: [Validators.required], width: 135 }
        )
    ];

    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {

        const catastoApp = this.metodoProduzioneCatastoService.getMetodoProduzioneCatasto();

        switch (actionType) {
            case HttpAction.CREATE:
                var metodoProduzioneId = new MetodoProduzioneId();
                metodoProduzioneId.validita = new IntervalloTemporale(items[0].Validita_Inizio, items[0].Validita_Fine);
                metodoProduzioneId.metodoProduzione = { codice: items[0].MetodoProduzione.id, descrizione: items[0].MetodoProduzione.name }
                catastoApp.push(metodoProduzioneId);
                break;
            case HttpAction.REMOVE:
                var index = catastoApp.findIndex((el) => el.id == parseInt(items[0].id));
                if (index != -1) {
                    catastoApp.splice(index, 1);
                }
                break;
            case HttpAction.UPDATE:
                var index = catastoApp.findIndex((el) => el.id == parseInt(items[0].id));
                var el = catastoApp.find((el) => el.id == parseInt(items[0].id));
                el.validita = new IntervalloTemporale(items[0].Validita_Inizio, items[0].Validita_Fine);
                el.metodoProduzione = { codice: items[0].MetodoProduzione.id, descrizione: items[0].MetodoProduzione.name }
                catastoApp[index] = el;
                break;
        }

        this.metodoProduzioneCatastoService.setMetodoProduzioneParticella(catastoApp);

        return from([this.metodoProduzioneCatastoService.getMetodoProduzioneCatasto()])

    }


    read(): Observable<MetodoProduzioneKendo> {
        const MetodiProduzioneParticelle: any = {};
        MetodiProduzioneParticelle.kendo_columns = this.kendoColumns;
        MetodiProduzioneParticelle.kendo_model=this.kendoModel;
        this.handleDropdowns(this.catastoService.metodiProduzione);
        return this.metodoProduzioneCatastoService.metodoProduzioneParticellaSource.pipe(
            takeUntil(this.gridPublicService.signal$),
            map((data) => {
                const rows: KendoGridRow[] = new Array<KendoGridRow>();
                let id = 0;
                for (const metodoProduzione of data) {
                    const row: KendoGridRow = {
                        id: String(metodoProduzione.id),
                        MetodoProduzione: metodoProduzione.metodoProduzione.codice,
                        Validita_Inizio: metodoProduzione.validita.inizio,
                        Validita_Fine: metodoProduzione.validita.fine
                    };
                    rows.push(row);
                    id++;
                }
                const p = new MetodoProduzioneKendo(rows, MetodiProduzioneParticelle.kendo_columns, MetodiProduzioneParticelle.kendo_model);
                return p;
            }));
    }


    handleDropdowns(ddlist: MetodoProduzione[]): void {
        const col = this.kendoColumns.find(s => s.field === 'MetodoProduzione');

        const data: DropdownListItem[] = ddlist.map(cod => new DropdownListItem(cod.codice, cod.descrizione));

        col.ddl = new DropdownListWithForm('codice', 'MetodoProduzione', 'descrizione', data);
        col.ddl.valuePrimitive = false;
    }

}

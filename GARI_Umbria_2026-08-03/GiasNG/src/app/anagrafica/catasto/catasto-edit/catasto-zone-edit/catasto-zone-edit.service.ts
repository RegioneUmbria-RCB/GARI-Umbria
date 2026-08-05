import { Injectable, Injector } from '@angular/core';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable } from 'rxjs';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import {  DateSettings, DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, NumericSettings } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { map, takeUntil } from 'rxjs/operators';
import { ExcelSettings, PDFSettings, ToolbarSettings } from 'gias-kendo-grid';
import { Validators } from '@angular/forms';
import { ParticelleCatastaliZonaId, ZoneCatastoService } from './ZoneCatasto.service';
import { ZoneService } from 'app/Service/Metaschema/Zone.service';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { TranslocoService } from '@jsverse/transloco';
import { ObjParametriAgenda } from 'gias-ui-kit';


export class ZoneKendo extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable({
    providedIn:'root'
})

export class ZoneParticellaService extends AbstractGridConfigService<ZoneKendo>{
    gridId = 'ZoneParticella';
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
        Zona: {
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST,
            validators: [Validators.required]
        },
        Zona_Des: {
            editable: true,
            type: CELL_TYPES.STRING
        },
        Area: {
            editable: true,
            type: CELL_TYPES.NUMBER,
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
        private zoneCatastoService: ZoneCatastoService,
        private zoneService: ZoneService,
        private ObjParametriAgendaService: ObjParametriAgendaService,
        protected transloco: TranslocoService) {

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
                { field: 'Zona', title: this.transloco.translate('Zona') },
                { resizable: true, editable: this.edit, validators: [Validators.required], width: 135 }
            ),
            new KendoGridColumn(
                { field: 'Area', title: this.transloco.translate('Superficie') },
                { resizable: true, editable: this.edit, numeric: new NumericSettings({format: 'n4'}), validators: [Validators.required], width: 135 }
            ),
            new KendoGridColumn(
                { field: 'Validita_Inizio', title: this.transloco.translate('ValiditàInizio') }, { resizable: true, editable: true, date: new DateSettings({ defaultValue: AGRODATAINIZIO }), width: 135 }
            ),
            new KendoGridColumn(
                { field: 'Validita_Fine', title: this.transloco.translate('ValiditàFine') }, { resizable: true, editable: true, date: new DateSettings({ defaultValue: AGRODATAFINE }), width: 135 }
            )
        ];


    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {

        const zonaCat = this.zoneCatastoService.getZoneCatasto();

        switch (actionType) {
            case HttpAction.CREATE:
                var zonaId = new ParticelleCatastaliZonaId();
                zonaId.Area = items[0].Area;
                zonaId.zona = new BaseCodeDescr(parseInt(items[0].Zona));
                zonaId.zona.descrizione = items[0].Zona_Des;
                zonaId.validita = new IntervalloTemporale(items[0].Validita_Inizio, items[0].Validita_Fine);
                zonaCat.push(zonaId);
                break;
            case HttpAction.REMOVE:
                var index = zonaCat.findIndex((el) => el.id == parseInt(items[0].id));
                if (index != -1) {
                    zonaCat.splice(index, 1);
                }
                break;
            case HttpAction.UPDATE:
                var index = zonaCat.findIndex((el) => el.id == parseInt(items[0].id));
                var el = zonaCat.find((el) => el.id == parseInt(items[0].id));
                el.Area = items[0].Area;
                el.zona = new BaseCodeDescr(parseInt(items[0].Zona));
                el.zona.descrizione = items[0].Zona_Des;
                el.validita.inizio = items[0].Validita_Inizio;
                el.validita.fine = items[0].Validita_Fine;
                zonaCat[index] = el;
                break;
        }

        this.zoneCatastoService.setZoneParticella(zonaCat);


        return from([this.zoneCatastoService.getZoneCatasto()]);
    }

    read(): Observable<ZoneKendo> {
        const ZoneParticelle: any = {};
        ZoneParticelle.kendo_columns = this.kendoColumns;
        ZoneParticelle.kendo_model=this.kendoModel;
        this.handleDropdowns();
        return this.zoneCatastoService.zoneParticellaSource.pipe(
            takeUntil(this.gridPublicService.signal$),
            map((data) => {
                const rows: KendoGridRow[] = new Array<KendoGridRow>();
                let id = 0;
                for (const zona of data) {
                    const row: KendoGridRow = {
                        id: String(zona.id),
                        Zona: zona.zona.codice,
                        Zona_Des: zona.zona.descrizione,
                        Area: zona.Area,
                        Validita_Inizio: zona.validita.inizio,
                        Validita_Fine: zona.validita.fine
                    };
                    rows.push(row);
                    id++;
                }
                const p = new ZoneKendo(rows, ZoneParticelle.kendo_columns, ZoneParticelle.kendo_model);
                return p;
            })
        );
    }


    handleDropdowns(): void {
        const col = this.kendoColumns.find(s => s.field === 'Zona');

        const data: DropdownListItem[] = [];
        col.ddl = new DropdownListWithForm('codice', 'Zona', 'descrizione', data);
        col.ddl.valuePrimitive = true;
        col.ddl.loadOnEdit = true;
        col.ddl.descriptionField = 'Zona_Des';
        col.ddl.loadFunction = this.loadZone.bind(this);

    }


    loadZone() {
        return this.zoneService.leggi();
    }

}

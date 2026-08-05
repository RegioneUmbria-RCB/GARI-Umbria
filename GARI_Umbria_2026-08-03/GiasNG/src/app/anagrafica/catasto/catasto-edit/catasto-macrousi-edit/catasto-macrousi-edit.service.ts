import { Injectable, Injector } from '@angular/core';
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
import { Macrouso } from 'app/Model/metaschema/Macrouso';
import { MacrousiCatastoService, ParticelleCatastaliMacrousoId } from './MacrousiCatasto.service';
import { MacrousiService } from 'app/Service/Metaschema/Macrousi.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { TranslocoService } from '@jsverse/transloco';
import { ObjParametriAgenda } from 'gias-ui-kit';


export class MacrousiKendo extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable({
    providedIn:'root'
})

export class MacrousiParticellaService extends AbstractGridConfigService<MacrousiKendo>{
    gridId = 'MacrousiParticella';
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
        Macrouso: {
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST,
            validators: [Validators.required]
        },
        Macrouso_Des: {
            editable: true,
            type: CELL_TYPES.STRING
        },
        Area: {
            editable: true,
            type: CELL_TYPES.NUMBER
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
        },
        Piva: {
            editable: false,
            type: CELL_TYPES.STRING
        },
        NumeroFascicolo: {
            editable: false,
            type: CELL_TYPES.STRING
        },
        DataValidazioneFascicolo: {
            editable: false,
            type: CELL_TYPES.DATE
        }
    };

    constructor(injector: Injector,
        private macrousoCatastoService: MacrousiCatastoService,
        private macrousiService: MacrousiService,
        private ObjParametriAgendaService: ObjParametriAgendaService,
        protected transloco: TranslocoService) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.objParametriAgenda = this.ObjParametriAgendaService.getObjParamValue();
        this.edit = true;
        if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
            this.edit = false;
        }

        // Nascondo la colonna Azioni con i bottoni di Info, Modifica e Cancella
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
            new KendoGridColumn(
                { field: 'Macrouso', title: this.transloco.translate('Macrouso') },
                { resizable: true, editable: true, validators: [Validators.required], width: 135 }
            ),
            new KendoGridColumn(
                { field: 'Area', title: this.transloco.translate('Superficie') },
                { resizable: true, editable: true, numeric: { min: 0, format: 'n4', defaultValue: 0 }, validators: [Validators.required], width: 135 }
            ),
            new KendoGridColumn(
                { field: 'Validita_Inizio', title: this.transloco.translate('ValiditàInizio') },
                { resizable: true, editable: true, date: new DateSettings({ defaultValue: AGRODATAINIZIO }), validators: [Validators.required], width: 135 }
            ),
            new KendoGridColumn(
                { field: 'Validita_Fine', title:  this.transloco.translate('ValiditàFine') },
                { resizable: true, editable: true, date: new DateSettings({ defaultValue: AGRODATAFINE }), validators: [Validators.required], width: 135 }
            ),
            new KendoGridColumn(
                { field: 'NumeroFascicolo', title:  this.transloco.translate('NumeroFascicolo') },
                { resizable: true, editable: false, width: 135 }
            ),
            new KendoGridColumn(
                { field: 'DataValidazioneFascicolo', title: this.transloco.translate('DataValidazioneFascicolo') },
                { resizable: true, editable: false, date: new DateSettings({ defaultValue: AGRODATAFINE }), width: 135}
            )
        ];

        this.gridPublicService.formGroup.subscribe((fb) => {
            if(fb != undefined) {
                if(fb.controls['Macrouso'].pristine) {
                    let col = this.kendoColumns.find(s => s.field === 'Macrouso');
                    col.ddl.reload.next(true);
                }
            }
        });

    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {

        const macrousoCat = this.macrousoCatastoService.getMacrousiCatasto();

        switch (actionType) {
            case HttpAction.CREATE:
                var macrousoId = new ParticelleCatastaliMacrousoId();
                macrousoId.validita = new IntervalloTemporale(items[0].Validita_Inizio, items[0].Validita_Fine);
                macrousoId.macrouso = new Macrouso(items[0].Macrouso.id);
                macrousoId.macrouso.descrizione = items[0].Macrouso.name;
                macrousoId.Area = items[0].Area;
                macrousoId.NumeroFascicolo = items[0].NumeroFascicolo;
                macrousoId.DataValidazioneFascicolo = items[0].DataValidazioneFascicolo;
                macrousoCat.push(macrousoId);
                break;
            case HttpAction.REMOVE:
                var index = macrousoCat.findIndex((el) => el.id == parseInt(items[0].id));
                if (index != -1) {
                    macrousoCat.splice(index, 1);
                }
                break;
            case HttpAction.UPDATE:
                var index = macrousoCat.findIndex((el) => el.id == parseInt(items[0].id));
                var el = macrousoCat.find((el) => el.id == parseInt(items[0].id));
                el.validita = new IntervalloTemporale(items[0].Validita_Inizio, items[0].Validita_Fine);
                el.macrouso = new Macrouso(items[0].Macrouso.id);
                el.macrouso.descrizione = items[0].Macrouso.name;
                el.Area = items[0].Area;
                el.NumeroFascicolo = items[0].NumeroFascicolo;
                el.DataValidazioneFascicolo = items[0].DataValidazioneFascicolo;
                macrousoCat[index] = el;
                break;
        }

        this.macrousoCatastoService.setMacrousiParticella(macrousoCat);


        return from([this.macrousoCatastoService.getMacrousiCatasto()]);
    }


    read(): Observable<MacrousiKendo> {
        const MetodiProduzioneParticelle: any = {};
        MetodiProduzioneParticelle.kendo_columns = this.kendoColumns;
        MetodiProduzioneParticelle.kendo_model=this.kendoModel;
        this.handleDropdowns();
        return this.macrousoCatastoService.macrousiParticellaSource.pipe(
            takeUntil(this.gridPublicService.signal$),
            map((data) => {
                const rows: KendoGridRow[] = new Array<KendoGridRow>();
                let id = 0;
                for (const macrouso of data) {
                    const row: KendoGridRow = {
                        id: String(macrouso.id),
                        Macrouso: macrouso.macrouso.codice,
                        Macrouso_Des: macrouso.macrouso.descrizione,
                        Validita_Inizio: macrouso.validita.inizio,
                        Validita_Fine: macrouso.validita.fine,
                        Area: macrouso.Area,
                        Piva: macrouso.Piva,
                        NumeroFascicolo: macrouso.NumeroFascicolo,
                        DataValidazioneFascicolo: macrouso.DataValidazioneFascicolo
                    };
                    rows.push(row);
                    id++;
                }
                const p = new MacrousiKendo(rows, MetodiProduzioneParticelle.kendo_columns, MetodiProduzioneParticelle.kendo_model);
                return p;
            }));
    }


    handleDropdowns(): void {
        const col = this.kendoColumns.find(s => s.field === 'Macrouso');

        const data: DropdownListItem[] = [];
        col.ddl = new DropdownListWithForm('codice', 'Macrouso', 'descrizione', data);
        col.ddl.valuePrimitive = false;
        col.ddl.id = 'codice';
        col.ddl.formControlValue = 'descrizione';
        col.ddl.loadOnEdit = true;
        col.ddl.descriptionField = 'Macrouso_Des';
        col.ddl.loadFunction = this.loadMacrousi.bind(this);
    }


    loadMacrousi() {
        return this.macrousiService.leggi();
    }

}

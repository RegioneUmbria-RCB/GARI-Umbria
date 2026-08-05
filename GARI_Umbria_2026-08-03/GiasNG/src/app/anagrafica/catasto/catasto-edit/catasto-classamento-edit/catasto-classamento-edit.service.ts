import { Injectable, Injector } from '@angular/core';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable } from 'rxjs';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { map, takeUntil } from 'rxjs/operators';
import { ExcelSettings, PDFSettings, ToolbarSettings } from 'gias-kendo-grid';
import { Validators } from '@angular/forms';
import { ClassamentoCatastoService, ParticelleCatastaliClassamentoId } from './ClassamentoCatasto.service';
import { QualitaCatastoService } from 'app/Service/Metaschema/QualitaCatasto.service';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { TranslocoService } from '@jsverse/transloco';
import { ObjParametriAgenda } from 'gias-ui-kit';

export class ClassamentiKendo extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable({
    providedIn:'root'
})

export class ClassamentiParticellaService extends AbstractGridConfigService<ClassamentiKendo>{
    gridId = 'ClassamentiParticella';
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
        Qualita: {
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST,
            validators: [Validators.required]
        },
        Qualita_Des: {
            editable: true,
            type: CELL_TYPES.STRING
        },
        porzione: {
            editable: true,
            type: CELL_TYPES.STRING
        },
        Area: {
            editable: true,
            type: CELL_TYPES.NUMBER
        },
        classe: {
            editable: true,
            type: CELL_TYPES.STRING,
            validators: [Validators.maxLength(2)]
        },
        redditoDomiciliare: {
            editable: true,
            type: CELL_TYPES.NUMBER
        },
        redditoAgrario: {
            editable: true,
            type: CELL_TYPES.NUMBER
        },
    };

    constructor(injector: Injector,
        private classamentoCatastoService: ClassamentoCatastoService,
        private qualitaService: QualitaCatastoService,
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
        this.pagination.pageable = false;
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

        this.kendoColumns = [
        // new KendoGridColumn(
        //     { field: "id", title: "id" }, { resizable: true, editable: false}
        // ),
        new KendoGridColumn(
            { field: 'Qualita', title: this.transloco.translate('Qualità') },
            { resizable: true, editable: true, validators: [Validators.required], width: 135 }
        ),
        new KendoGridColumn(
            { field: 'Area', title: this.transloco.translate('Superficie') },
            { resizable: true, editable: true, numeric: { defaultValue: 0, min: 0, format: 'n4' }, validators: [Validators.required], width: 135 }
        ),
        new KendoGridColumn(
            { field: 'porzione', title: this.transloco.translate('Porzione') },
            { resizable: true, editable: true, validators: [Validators.required], width: 135 }
        ),
        new KendoGridColumn(
            { field: 'classe', title: this.transloco.translate('Classe') },
            { resizable: true, editable: true, validators: [Validators.required, Validators.maxLength(2)], width: 135 }
        ),
        new KendoGridColumn(
            { field: 'redditoDomiciliare', title: this.transloco.translate('RedditoDomiciliare') },
            { resizable: true, editable: true, numeric: { defaultValue: 0, min: 0, format: 'n2' }, validators: [Validators.required], width: 135 }
        ),
        new KendoGridColumn(
            { field: 'redditoAgrario', title: this.transloco.translate('RedditoAgrario') },
            { resizable: true, editable: true, numeric: { defaultValue: 0, min: 0, format: 'n2' }, validators: [Validators.required], width: 135 }
        ),
    ];

    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {

        const classamentoCat = this.classamentoCatastoService.getClassamentoCatasto();

        switch (actionType) {
            case HttpAction.CREATE:
                var classamentoId = new ParticelleCatastaliClassamentoId();
                classamentoId.porzione = items[0].porzione;
                classamentoId.Area = items[0].Area;
                classamentoId.classe = items[0].classe;
                classamentoId.redditoDomiciliare = items[0].redditoDomiciliare;
                classamentoId.redditoAgrario = items[0].redditoAgrario;
                classamentoId.qualita = new BaseCodeDescr(parseInt(items[0].Qualita));
                classamentoId.qualita.descrizione = items[0].Qualita_Des;
                classamentoCat.push(classamentoId);
                break;
            case HttpAction.REMOVE:
                var index = classamentoCat.findIndex((el) => el.id == parseInt(items[0].id));
                if (index != -1) {
                    classamentoCat.splice(index, 1);
                }
                break;
            case HttpAction.UPDATE:
                var index = classamentoCat.findIndex((el) => el.id == parseInt(items[0].id));
                var el = classamentoCat.find((el) => el.id == parseInt(items[0].id));
                el.porzione = items[0].porzione;
                el.Area = items[0].Area;
                el.classe = items[0].classe;
                el.redditoDomiciliare = items[0].redditoDomiciliare;
                el.redditoAgrario = items[0].redditoAgrario;
                el.qualita = new BaseCodeDescr(parseInt(items[0].Qualita));
                el.qualita.descrizione = items[0].Qualita_Des;
                el.qualita = new BaseCodeDescr(parseInt(items[0].Qualita));
                el.qualita.descrizione = items[0].Qualita_Des;
                classamentoCat[index] = el;
                break;
        }

        this.classamentoCatastoService.setClassamentoParticella(classamentoCat);


        return from([this.classamentoCatastoService.getClassamentoCatasto()]);
    }


    read(): Observable<ClassamentiKendo> {
        const MetodiProduzioneParticelle: any = {};
        MetodiProduzioneParticelle.kendo_columns = this.kendoColumns;
        MetodiProduzioneParticelle.kendo_model=this.kendoModel;
        this.handleDropdowns();
        return this.classamentoCatastoService.classamentoParticellaSource.pipe(
            takeUntil(this.gridPublicService.signal$),
            map((data) => {
                const rows: KendoGridRow[] = new Array<KendoGridRow>();
                let id = 0;
                for (const classamento of data) {
                    const row: KendoGridRow = {
                        id: String(classamento.id),
                        Qualita: classamento.qualita.codice,
                        Qualita_Des: classamento.qualita.descrizione,
                        porzione: classamento.porzione,
                        Area: classamento.Area,
                        classe: classamento.classe,
                        redditoDomiciliare: classamento.redditoDomiciliare,
                        redditoAgrario: classamento.redditoAgrario
                    };
                    rows.push(row);
                    id++;
                }
                const p = new ClassamentiKendo(rows, MetodiProduzioneParticelle.kendo_columns, MetodiProduzioneParticelle.kendo_model);
                return p;
            }));
    }


    handleDropdowns(): void {
        const col = this.kendoColumns.find(s => s.field === 'Qualita');

        const data: DropdownListItem[] = [];
        col.ddl = new DropdownListWithForm('codice', 'Qualita', 'descrizione', data);
        col.ddl.valuePrimitive = true;
        col.ddl.loadOnEdit = true;
        col.ddl.descriptionField = 'Qualita_Des';
        col.ddl.loadFunction = this.loadQualita.bind(this);

    }


    loadQualita() {
        return this.qualitaService.leggi();
    }

}

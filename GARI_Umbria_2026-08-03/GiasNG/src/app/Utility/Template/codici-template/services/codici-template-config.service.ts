import { Injectable, Injector } from '@angular/core';
import { forkJoin, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { ConfigTemplate } from 'gias-kendo-grid';
import { CodiciServerResult, CODICI_TOKEN } from '../models/codici.model';
import { ICodiciTemplateService } from './codici-template.service';
import { Inject } from '@angular/core';
import {
    DropdownListWithForm,
    DropdownListItem,
    EditingMode,
    KendoGridRow,
    LoaderType,
    KendoGridColumn,
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
    AbstractGridConfigService,
    HttpAction
} from 'gias-kendo-grid';
import {
    BehaviorSettings, ColumnMenuSettings, CommandsColumnSettings,
    ExcelSettings, GeneralSettings, PaginationSettings, PDFSettings,
    ToolbarSettings } from 'gias-kendo-grid';
import { CodiciAnagrafeValoriChiave } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { CodiceAnagrafe } from 'app/Model/anagrafiche/CodiceAnagrafe';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { EserciziEditService } from 'app/anagrafica/esercizi/esercizio-edit/esercizio-edit-codici.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { GridPublicService } from 'gias-kendo-grid';
import { tap } from 'lodash';
import { AbstractControl, Validators } from '@angular/forms';
import { TranslocoConfig, TranslocoService } from '@jsverse/transloco';

export function startdateValidator(control: AbstractControl) {
    if (control.value > control.parent?.controls['Validita_Fine'].value) {
        return { 'startDate': true };
    }
    return null;
}

export function endDateValidator(control: AbstractControl) {
    if (control.value < control.parent?.controls['Validita_Inizio'].value) {
        return { 'endDate': true };
    }
    return null;
}

@Injectable()
export class CodiciTemplateConfigService extends AbstractGridConfigService<CodiciServerResult> {
    gridId = '';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;
    behavior: BehaviorSettings;
    rowId = 'chiave';
    generalSettings = new GeneralSettings();
    columnMenu = new ColumnMenuSettings;
    ddlValues = [];
    model = {
        chiave:{
            editable: true,
            type: CELL_TYPES.NUMBER
        },
        Val_Cod:
        {
            editable: true,
            type: CELL_TYPES.STRING
        },
        Descrizione:
        {
            editable: true,
            type: CELL_TYPES.STRING
        },
        Id_Cod:
        {
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST
        },
        Validita_Inizio:
        {
            editable: true,
            type: CELL_TYPES.DATE
        },
        Validita_Fine:
        {
            editable: true,
            type: CELL_TYPES.DATE,
        },
    };
    columns = [
        // new KendoGridColumn({
        //   field: "chiave",
        //   title: "chiave"},{
        //   hidden: false,
        //   width:200,
        //   resizable:true
        // }),
        new KendoGridColumn({
            field: 'Id_Cod',
            title: this.translocoService.translate('Descrizione')},{
            hidden: false,
            width:200,
            resizable: true,
            validators: [Validators.required]
        }),
        new KendoGridColumn({
            field: 'Val_Cod',
            title: this.translocoService.translate('Valore')
        },
        {
            hidden: false,
            width: 200,
            resizable: true,
            validators: [Validators.required]
        }),
        new KendoGridColumn({
           field: "Validita_Inizio",
           title: this.translocoService.translate("ValiditaInizio")},{
           hidden: false,
           width:200,
           resizable:true,
           date: { defaultValue: AGRODATAINIZIO },
           validators: [startdateValidator]
        }),
        new KendoGridColumn({
           field: "Validita_Fine",
           title: this.translocoService.translate("ValiditaFine")}, {
           hidden: false,
           width:200,
           resizable:true,
           date: { defaultValue: AGRODATAFINE },
           validators: [endDateValidator]
        })
    ].map(x => Object.assign({}, x));

    private edit: boolean;
    public gridPublicService: GridPublicService;
    constructor(injector: Injector,
        @Inject(CODICI_TOKEN) private codiciTemplateService: ICodiciTemplateService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private translocoService: TranslocoService) {

        super(injector, ConfigTemplate.DefaultTemplate);
        this.columnMenu.columnMenu = false;
        this.columnMenu.filterable = false;
        this.behavior.saveExternalChanges = true;
        this.behavior.excelSettings = new ExcelSettings({enabled: false});
        this.behavior.pdfSettings = new PDFSettings({enabled: false});
        this.generalSettings.reordable = false;
        this.generalSettings.performOnEdit = true;
        this.groups.groupable.enabled = false;
        this.columnMenu.kendoGridColumnChooser = false;
        this.views.enabled = false;

        const objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        this.edit = true;
        if (objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
            this.edit = false;
        }
        this.gridId = this.codiciTemplateService.gridId;
        this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: this.edit });
        this.toolbar = new ToolbarSettings(this.edit, false);

        this.codiciTemplateService.disableCodesGrid = this.disableCodesGrid.bind(this);
    }

    read(): Observable<CodiciServerResult> {
        const rows: Observable<CodiciAnagrafeValoriChiave[]> = this.codiciTemplateService.leggiDatiTabella();
        const ddlists = this.codiciTemplateService.leggiDropdowns();
        const observables = [rows, ddlists];

        return forkJoin(observables).pipe(map(
            (result) => {
                this.handleDropdowns(result[1] as CodiceAnagrafe[]);
                this.setRowsUniqueId(result[0] as CodiciAnagrafeValoriChiave[]);

                for (let i = 0; i < this.columns.length; i++) {
                    this.columns[i].editable = this.edit;
                }

                this.ddlValues = result[1];

                const mappedResult: Array<any> = result[0].map((el) => ({
                    chiave: el.chiave,
                    Val_Cod: (<CodiciAnagrafeValoriChiave>el).valore,
                    Descrizione: (<CodiciAnagrafeValoriChiave>el).codiceAnagrafe.descrizione,
                    Id_Cod: (<CodiciAnagrafeValoriChiave>el).codiceAnagrafe.codice,
                    Validita_Inizio: (<CodiciAnagrafeValoriChiave>el).validita.inizio,
                    Validita_Fine: (<CodiciAnagrafeValoriChiave>el).validita.fine
                }));
                return new CodiciServerResult (
                    this.model, this.columns, mappedResult as KendoGridRow[]);
            }));
    }

    disableCodesGrid() {
        this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: this.edit });
        this.toolbar = new ToolbarSettings(false, false);
        for (let i = 0; i < this.columns.length; i++) {
            this.columns[i].editable = false;
        }
    }

    perform(action: HttpAction, row: Array<any>): Observable<KendoGridRow[]> {
        const codiceAnagrafe = new CodiceAnagrafe(row[0].Id_Cod);
        codiceAnagrafe.descrizione = row[0].Descrizione;
        if (row[0].Validita_Inizio === undefined) {
            row[0].Validita_Inizio = AGRODATAINIZIO;
        }
        if (row[0].Validita_Fine === undefined) {
            row[0].Validita_Fine = AGRODATAFINE;
        }
        const validita = new IntervalloTemporale(row[0].Validita_Inizio, row[0].Validita_Fine);
        const valore = row[0].Val_Cod;
        const codiceAnagrafeValore = new CodiciAnagrafeValoriChiave();
        codiceAnagrafeValore.codiceAnagrafe = codiceAnagrafe;
        codiceAnagrafeValore.valore = valore;
        codiceAnagrafeValore.validita = validita;
        codiceAnagrafeValore.chiave = row[0].chiave;
        // let subs = this.codiciTemplateService.leggiDatiTabella().subscribe((codici) => {
        //     if (this.validateAction(action, codiceAnagrafeValore, codici)) {
        //         this.codiciTemplateService.updateTable(action, codiceAnagrafeValore);
        //     } else {
        //         console.log("error");
        //     }
        // })
        this.codiciTemplateService.updateTable(action, codiceAnagrafeValore);
        // subs.unsubscribe();
        //return from([]);
        return this.codiciTemplateService.leggiDatiTabella();
    }

    handleDropdowns(ddlist: CodiceAnagrafe[]): void {
        const col = this.columns.find(s => s.field === 'Id_Cod');

        //const data: DropdownListItem[] = ddlist.map(cod => new DropdownListItem(cod.codice, cod.descrizione));
        const data = []
        col.ddl = new DropdownListWithForm('codice', 'Id_Cod', 'descrizione', data);
        col.ddl.valuePrimitive = true;
        col.ddl.loadOnEdit = true;
        col.ddl.loadFunction = this.loadDdl.bind(this);
        col.ddl.descriptionField = 'Descrizione';
        // col.ddl.loadFunction = this.loadDdl.bind(this);
        // col.ddl.loadOnEdit = true;
        // col.ddl.descriptionField = 'Descrizione';
    }
    //(dataItem: any) => Subject<any[]>;
    loadDdl() {
        return of(this.ddlValues);
    }

    private validateAction(action: HttpAction, codiceAnagrafeValore: CodiciAnagrafeValoriChiave, dt: CodiciAnagrafeValoriChiave[]) {
        // this.read()
        return true;
    }

    private setRowsUniqueId(rows: CodiciAnagrafeValoriChiave[]) {
        let i = 0;
        if (rows != null) {
            rows.forEach((row) => {
                row.chiave = i++;
            });
        }
    }


}

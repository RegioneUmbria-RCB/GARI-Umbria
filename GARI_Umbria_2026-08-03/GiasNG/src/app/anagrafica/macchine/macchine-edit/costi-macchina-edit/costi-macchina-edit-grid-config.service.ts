import { Inject, Injectable, Injector } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { CostoUnitarioChiave } from 'app/Model/anagrafiche/CostoUnitario';
import { ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { UnitaDiMisura } from 'app/Model/metaschema/UnitaDiMisura';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { MacchineFactoryService, MACCHINE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/macchine.factory.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { AgrSelectableSettings, BehaviorSettings, CommandsColumnSettings, GeneralSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable, of, Subscription } from 'rxjs';
import { MacchinaKendoServerResult } from '../../macchine.model';

import { CostiMacchinaDataService } from './costi-macchina-data.service';
import { CostiMacchinaEditService } from './costi-macchina-edit.service';
import { CostiMacchinaParentFormService } from './costi-macchina-parent-form.service';


export function forbiddenPriceValidator(control: AbstractControl) {
    if (control.value == undefined || control.value <= 0) {
        return { 'brandCode': true };
    }
    return null;
}

@Injectable()
export class CostiMacchinaEditConfigService extends AbstractGridConfigService<MacchinaKendoServerResult> {
    gridId = 'MacchineEditConfigService';

    editingMode: EditingMode = EditingMode.IN_CELL;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'chiave';
    generalSettings = new GeneralSettings;
    costiColumns: KendoGridColumn[];
    behavior = new BehaviorSettings({});
    costiDataSub: Subscription;
    private objParametriAgenda;
    private edit;

    public form: ParcoMacchine;
    public costi: CostoUnitarioChiave[];

    costiMacchinaParentForm: FormGroup = this.fb.group({});
    costiMacchinaParentFormSub: Subscription;

    // array per ddl Unita_Misura
    TipoUnitaMisura: Array<UnitaDiMisura> = [
        { descrizione: 'Ettaro', codice: 1},
        { descrizione: 'Ora', codice: 2}
    ];

    constructor(
        injector: Injector,
        @Inject(MACCHINE_SERVICE_TOKEN) private macchineService: MacchineFactoryService,
        private masterService: MasterService,
        private fb: FormBuilder,
        private parametriAgenda: ObjParametriAgendaService,
        private macchineEditCostsService: CostiMacchinaEditService,
        private costiMacchinaDataService: CostiMacchinaDataService,
        private costiMacchinaParentFormService: CostiMacchinaParentFormService,
        private translocoService: TranslocoService){
        super(injector, ConfigTemplate.DefaultTemplate);

        this.objParametriAgenda = this.parametriAgenda.getObjParamValue();
        this.edit = true;

        if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
            this.edit = false;
        }

        this.resizable = new ResizableSettings();
        this.resizable.autoFitColumns = true;

        this.behavior.saveExternalChanges = true;

        this.generalSettings.performOnEdit = true;
        this.handleCustomizations();

        this.costiMacchinaParentFormSub = this.costiMacchinaParentFormService.costimacchinaParentFormSource
        .subscribe(i => {
            this.costiMacchinaParentForm = this.costiMacchinaParentFormService.getCostiMacchinaParentForm();
        });
    }



    read(): Observable<MacchinaKendoServerResult> {

        const columns = this.setColumnsGridCostiUnitari();
        const model = this.setModelGridCostiUnitari();

        this.handleDropdowns(this.TipoUnitaMisura, columns);

        let rows = new Array<KendoGridRow>();

        rows = this.getRows();

        const result: MacchinaKendoServerResult = new MacchinaKendoServerResult(model, columns, rows);
        return of(result)

    }

    private getRows() {
        let rows = this.costiMacchinaDataService.getCostiMacchina();
        let kendoRows = new Array();
        rows.forEach(r => {
            let row: any = {};

            row.chiave = r.chiave;
            row.codice = r.codice;
            row.prezzo = r.prezzo;
            row.Unita_Misura_Des = r.unitaDiMisura.descrizione;
            row.Unita_Misura_Cod = r.unitaDiMisura.codice;
            row.Validita_Inizio = r.validita.inizio;
            row.Validita_Fine = r.validita.fine;
            row.flag_cancellazione = r.flag_cancellazione;
            kendoRows.push(row);
        });
        return kendoRows;
    }

    handleCustomizations(): void {
        this.selectable = new AgrSelectableSettings();
        this.selectable.selectable.checkboxOnly = false;

        this.toolbar = new ToolbarSettings();
        this.toolbar.newItem = this.edit;
        this.toolbar.resetChanges = false;

        this.cmdColumn = new CommandsColumnSettings(
            {
                editBtn: false,
                infoBtn: false,
                removeBtn: true,
                onDisableInfoBtn: () => false
            });

        this.resizable.autoFitColumns = false;
        this.resizable.isResizable = true;
        this.views.enabled = false;
        this.columnMenu.columnMenu = false;
        this.groups.groupable.enabled = false;
    }

    setColumnsGridCostiUnitari(){

        const columns: Array<KendoGridColumn>=[
            new KendoGridColumn(
                {
                    field: 'prezzo',
                    title: this.translocoService.translate('Prezzo')},
                {
                    hidden: false,
                    resizable:true,
                    numeric:{ defaultValue: 0, min: 0 },
                    editable: this.edit,
                    validators: [Validators.required, forbiddenPriceValidator],
                    width: 130
                }
            ),
            new KendoGridColumn(
                {
                    field: 'Unita_Misura_Cod',
                    title: this.translocoService.translate('UnitàDiMisura')
                },
                {
                    hidden: false,
                    resizable:true,
                    editable: this.edit,
                    validators: [Validators.required],
                    width: 150
                }
            ),
            new KendoGridColumn(
                {
                    field: 'Validita_Inizio',
                    title: this.translocoService.translate('ValiditàInizio')
                },
                {
                    hidden: false,
                    resizable:true,
                    editable: this.edit,
                    date: {
                        defaultValue: AGRODATAINIZIO
                    },
                    width: 130
                }
            ),
            new KendoGridColumn(
                {
                    field: 'Validita_Fine',
                    title: this.translocoService.translate('ValiditàFine')
                },
                {
                    hidden: false,
                    resizable:true,
                    editable: this.edit,
                    date: {
                        defaultValue: AGRODATAFINE
                    },
                    width: 130
                }
            )
        ];

        return columns;
    }

    setModelGridCostiUnitari(){

        const model: KendoGridModel={
            chiave:{
                editable: true,
                type: CELL_TYPES.NUMBER
            },
            codice:{
                editable: false,
                type: CELL_TYPES.NUMBER
            },
            prezzo:{
                editable: true,
                type: CELL_TYPES.NUMBER
            },
            Unita_Misura_Des:{
                editable: true,
                type: CELL_TYPES.STRING
            },
            Unita_Misura_Cod:{
                editable: true,
                type: CELL_TYPES.DROPDOWNLIST
            },
            Validita_Inizio:{
                editable: true,
                type: CELL_TYPES.DATE
            },
            Validita_Fine:{
                editable: true,
                type: CELL_TYPES.DATE
            },
            flag_cancellazione: {
                editable: true,
                type: CELL_TYPES.BOOLEAN
            }
        };
        return model;
    }

    handleDropdowns(ddlist: UnitaDiMisura[], columns: KendoGridColumn[]): void {
        const col = columns.find(s => s.field === 'Unita_Misura_Cod');

        const data: DropdownListItem[] = ddlist.map(cod => new DropdownListItem(cod.codice, cod.descrizione));

        col.ddl = new DropdownListWithForm('Unita_Misura', 'Unita_Misura_Cod', 'descrizione', data);
        col.ddl.valuePrimitive = true;
        col.ddl.descriptionField = 'Unita_Misura_Des';
    }

    perform(actionType: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
        rows.forEach((row)=> {
            this.macchineEditCostsService.perform(actionType, row);
        });
        return from([]);
    }

}

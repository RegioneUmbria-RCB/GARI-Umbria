import { Injectable, Injector } from '@angular/core';
import { CommandsColumnSettings, CommandsDropDownSettings, RemoveMultipleRowsParams, ToolbarSettings } from 'gias-kendo-grid';
import {  DropdownListWithForm, EditingMode, GridCustomizations, KendoGridColumn, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { Observable, of } from 'rxjs';
import { ConfigTemplate } from 'gias-kendo-grid';
import { map, takeUntil } from 'rxjs/operators';
import { Validators } from '@angular/forms';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { TranslocoService } from '@jsverse/transloco';
import { RemovedYears, ValutazioniService } from '../service/valutazioni.service';
import { AnniKendoServerResult, AnniModel, KendoAnniRow } from '../valutazioni.models';
import { ObjParametriAgenda } from 'gias-ui-kit';

export class AnnniObject {

    constructor(id: string, name: string) {
        this.id = id;
        this.name = name;
    }

    id: string;
    name: string;
}

export class TipoObject {

    constructor(id_: string, tipo_: string) {
        this.id_ = id_;
        this.tipo_ = tipo_;
    }

    id_: string;
    tipo_: string;
}



@Injectable()
export class AnniHttpService extends AbstractGridConfigService<any> {
    gridId = 'AnniValutazioniTestataHttpService';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;
    rowId = 'id';
    objParametriAgenda: ObjParametriAgenda;

    AnniValutazioniTestataHttpService: any = {};

    public anni: AnnniObject[] = [new AnnniObject("1", "2020"), new AnnniObject("2", "2021"), new AnnniObject("3", "2022"),
    new AnnniObject("4", "2023"), new AnnniObject("5", "2024"), new AnnniObject("6", "2025"), new AnnniObject("7", "2026"),
    new AnnniObject("8", "2027"), new AnnniObject("9", "2028"), new AnnniObject("10", "2029"),
    new AnnniObject("11", "2030"), new AnnniObject("12", "2031"), new AnnniObject("12", "2032"), new AnnniObject("13", "2033"),
    new AnnniObject("14", "2034"), new AnnniObject("15", "2035"), new AnnniObject("16", "2036"),
    new AnnniObject("17", "2037"), new AnnniObject("18", "2038"), new AnnniObject("19", "2039"), new AnnniObject("20", "2040")];

    public tipi: TipoObject[] = [new TipoObject("1", "Chiuso"),
    new TipoObject("2", "Aperto"),
    new TipoObject("3", "Previsionale")];

    /** Optional parameters */
    toolbar = new ToolbarSettings(true, false);
    views = new GridCustomizations({ enabled: false });

    public permessoEdit: boolean;
    public permessoRemove: boolean;
    public permessoInfo: boolean;

    public readPerformed: boolean = false;
    public anniKendoServerResultGlobal: AnniKendoServerResult;
    public kendoAnniRowInitial: KendoAnniRow[] = [];


    columns: Array<KendoGridColumn>;
    model: AnniModel;
    comuneColumn: KendoGridColumn;


    constructor(injector: Injector,
        private objParametriService: ObjParametriAgendaService,
        private permessiUtenteService: PermessiUtenteService,
        private translocoService: TranslocoService,
        private _valutazioni: ValutazioniService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.objParametriAgenda = this.objParametriService.getObjParamValue();

        this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 2);
        this.permessoRemove = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 2);
        this.permessoInfo = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 0);

        this.gridPublicService.changeDetected.pipe(takeUntil(this.signal)).subscribe((event: any) => {
            console.log(event);


            switch (event?.action) {
                case 'cellClose':
                    this.columns.find(x => x.field === "id_").editable = true;
                    break;
                case 'add':
                    this.columns.find(x => x.field === "id").editable = true;
                    this.columns.find(x => x.field === "id_").editable = true;
                    break;

                case 'save':
                    let gridElement: any = this._valutazioni.gridPublicService.getValue();

                    if (gridElement.data.rows.length >= 3) {
                        this.toolbar.newItem = false;
                    }

                    let index = this._valutazioni.removedYearsObjects.indexOf(this._valutazioni.removedYearsObjects.find(x => x.year == event.dataItem.name));

                    if (index != -1) {
                        this._valutazioni.removedYearsObjects.splice(index, 1);
                    }

                    break;
                case 'remove':

                    let gridElement2: any = this._valutazioni.gridPublicService.getValue();

                    if (gridElement2.data.rows.length < 3) {
                        this.toolbar.newItem = true;
                    }

                    if (event.dataItem.name != null) {
                        if (!this._valutazioni.removedYearsObjects.find(x => x.year == event.dataItem.name)) {
                            this.handleElementInRemoveList(event.dataItem.name);
                        }
                    } else {
                        if (!this._valutazioni.removedYearsObjects.find(x => x.year == event.dataItem.id.name)) {
                            this.handleElementInRemoveList(event.dataItem.id_.name);
                        }
                    }

                    break;
                default:
                    break;
            }
        });

        this.model = {
            id: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
            name: new ModelEntry(CELL_TYPES.STRING, true),
            id_: new ModelEntry(CELL_TYPES.DROPDOWNLIST, (this.objParametriAgenda.TipoOperazioneDB != Enum_DBTypeOperation.Read)),
            tipo_: new ModelEntry(CELL_TYPES.STRING, true),
        };

        this.selectable.selectable.checkboxOnly = false;
        this.selectable.selectable.enabled = this.permessoEdit;
        this.behavior.saveExternalChanges = true;

        this.toolbar = new ToolbarSettings();
        this.toolbar.newItem = true && (this.objParametriAgenda.TipoOperazioneDB != Enum_DBTypeOperation.Read);
        this.toolbar.resetChanges = false;

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            removeBtn: this.permessoRemove && (this.objParametriAgenda.TipoOperazioneDB != Enum_DBTypeOperation.Read)
        });

        this.cmdDropDown = new CommandsDropDownSettings({
            fullEditBtn: false,
            infoBtn: false,
        });


        this.resizable.autoFitColumns = false;
        this.selectable.columnSettings.showSelectAll = false;
        this.selectable.shouldShowCheckbox = false;
        this.selectable.columnSettings.title = ' ';
        this.resizable.isResizable = true;
        this.selectable.selectable.enabled = true;
        this.generalSettings.performOnEdit = true;

        this.behavior.excelSettings.enabled = false;
        this.behavior.pdfSettings.enabled = false;
        this.groups.groupable.enabled = false;

        if (window.innerWidth < SMARTPHONE_WIDTH) {
            this.groups.groupable.enabled = false;
            this.views.enabled = false;
            this.cmdColumn.editBtn = false;
        }

    }

    handleElementInRemoveList(value: string) {
        if (this.kendoAnniRowInitial.find(x => x.name == value)) {
            let removed = new RemovedYears(value, true);
            this._valutazioni.removedYearsObjects.push(removed);
        }
    }

    setColumnsGridAnni() {

        //let defaultvalue: DropdownListItem = new DropdownListItem(this.getThisDateAnniObject().id, this.getThisDateAnniObject().name, this.getThisDateAnniObject());

        //Imposto le Dropdown della Griglia delle Macchine
        let ddl_RisorsaAnni = new DropdownListWithForm('id', 'id', 'name', []);

        let ddl_RisorsaTipi = new DropdownListWithForm('id_', 'id_', 'tipo_', []);


        ddl_RisorsaAnni.valuePrimitive = true;
        ddl_RisorsaAnni.descriptionField = 'name';
        ddl_RisorsaAnni.loadOnEdit = true;
        ddl_RisorsaAnni.loadFunction = this.loadFunctionAnno.bind(this);

        ddl_RisorsaTipi.valuePrimitive = false;
        ddl_RisorsaTipi.descriptionField = 'tipo_';
        ddl_RisorsaTipi.loadOnEdit = true;
        ddl_RisorsaTipi.loadFunction = this.loadFunctionTipoAnno.bind(this);

        this.columns = [
            new KendoGridColumn({ field: 'id', title: this.translocoService.translate('Anno') }, { resizable: true, hidden: false, editable: false, width: 135, ddl: ddl_RisorsaAnni, validators: [Validators.required] }),
            new KendoGridColumn({ field: 'id_', title: this.translocoService.translate('Tipo') }, { resizable: true, hidden: false, editable: (this.objParametriAgenda.TipoOperazioneDB != Enum_DBTypeOperation.Read), media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135, ddl: ddl_RisorsaTipi, validators: [Validators.required] })
        ].map(x => Object.assign({}, x));;
    }

    loadFunctionTipoAnno() {
        return of(this.tipi);
    }

    loadFunctionAnno() {

        let anniActualSelectable: AnnniObject[] = Object.create(this.anni);

        let gridElementValue: any = this._valutazioni.gridPublicService.getValue();

        gridElementValue.data.rows.forEach(x => {

            if (x.name != null) {
                let index = anniActualSelectable.indexOf(anniActualSelectable.find(y => y.name == x.name));

                if (index != -1) {
                    anniActualSelectable.splice(index, 1);
                }
            } else {
                let index = anniActualSelectable.indexOf(anniActualSelectable.find(y => y.name == x.id.name));

                if (index != -1) {
                    anniActualSelectable.splice(index, 1);
                }
            }
        });

        return of(anniActualSelectable);
    }

    read(): Observable<AnniKendoServerResult> {
        if (!this.readPerformed) {
            this._valutazioni.removedYearsObjects = [];

            this.setColumnsGridAnni();
            let testata_lettura = this._valutazioni.getLeggiTestataValue();
            this.readPerformed = true;
            if (testata_lettura.idTestata != 0) {
                return this._valutazioni.leggiValutazioneObservable(testata_lettura).pipe(
                    map((data: any[]) => this.transformAnni(data))
                );
            }

            return of(this.DefaultNoArray());
        }

        return of(this.anniKendoServerResultGlobal);

    }

    DefaultNoArray(): AnniKendoServerResult {
        this.kendoAnniRowInitial = new Array();

        return this.anniKendoServerResultGlobal = {
            model: this.model,
            rows: new Array(),
            columns: this.columns
        };

    }

    private transformAnni(data: any[]): AnniKendoServerResult {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });

        let dataMappin: KendoAnniRow[] = new Array();

        if (data[0].Anno1 != null) {
            dataMappin.push(new KendoAnniRow(this.anni.find(y => y.name == data[0].Anno1).id, data[0].Anno1, this.tipi.find(y => y.tipo_ == data[0].Anno1_Tipo_Des).id_, data[0].Anno1_Tipo_Des));
        }

        if (data[0].Anno2 != null) {
            dataMappin.push(new KendoAnniRow(this.anni.find(y => y.name == data[0].Anno2).id, data[0].Anno2, this.tipi.find(y => y.tipo_ == data[0].Anno2_Tipo_Des).id_, data[0].Anno2_Tipo_Des));
        }

        if (data[0].Anno3 != null) {
            dataMappin.push(new KendoAnniRow(this.anni.find(y => y.name == data[0].Anno3).id, data[0].Anno3, this.tipi.find(y => y.tipo_ == data[0].Anno3_Tipo_Des).id_, data[0].Anno3_Tipo_Des));
        }

        if (dataMappin.length >= 3) {
            this.toolbar.newItem = false;
        }

        this.kendoAnniRowInitial = dataMappin;

        return this.anniKendoServerResultGlobal = {
            model: this.model,
            rows: dataMappin,
            columns: this.columns
        };

    }

    perform(actionType: HttpAction, items: any): Observable<any> {

        switch (actionType) {
            case HttpAction.REMOVE:

                break;

        }
        this.columns.find(x => x.field == "id").editable = false;
        this.columns.find(x => x.field == "id_").editable = true;
        return of([]);
    }

    override async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
        let msg: string;

        msg = this.translocoService.translate('annoValutazioni.Delete_Msg');
        opts.data.forEach(r => {
            msg = msg.concat('\n' + r['name']);
        });

        return msg;
    }
}

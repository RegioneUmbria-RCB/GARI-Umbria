import { Injectable, Injector } from '@angular/core';
import { CommandsColumnSettings, ExcelSettings, PDFSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable } from 'rxjs';
import { ConfigTemplate } from 'gias-kendo-grid';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { map, take } from 'rxjs/operators';
import { Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { ImpreseFilterService } from 'app/Master/menu-contestuale/imprese-filter/imprese-filter.service';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { RegolamentiService } from 'app/Service/Metaschema/regolamenti.service';
import { VarietaService } from 'app/Service/Metaschema/varieta.service';
import { ImpreseParametriGHGServerResult, ImpreseParametriGHGService } from './imprese-parametri-ghg.service';
import { ImpreseParametriGHGKendoServerResult } from './imprese-parametri-ghg-grid-model';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

export class ImpreseParametriGHGKendo extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class ImpreseParametriGHGGridService extends AbstractGridConfigService<ImpreseParametriGHGKendo> {
    gridId = 'impreseParametriGHGKendoGrid';
    rowId = 'Chiave';

    edit: boolean;

    objParametriAgenda: ObjParametriAgenda;
    toolbar = new ToolbarSettings(false, false);

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;

    KendoImpreseParametriGHG: any;

    impreseParametriGHGRows: any[];
    impreseParametriGHGColumns: KendoGridColumn[];

    kendoColumns: Array<KendoGridColumn> = [
        new KendoGridColumn(
            { field: 'Piva', title: this.translocoLoc.translate('Azienda') },
            { resizable: true, editable: true, validators: [Validators.required], width: 100}
        ),
        new KendoGridColumn(
            { field: 'Specie_Cod', title: this.translocoLoc.translate('Specie') },
            { resizable: true, editable: true, width: 100 }
        ),
        new KendoGridColumn(
            { field: 'Varieta_Cod', title: this.translocoLoc.translate('Varieta') },
            { resizable: true, editable: true, width: 100 }
        ),
        new KendoGridColumn(
            { field: 'Regolamento_Cod', title: this.translocoLoc.translate('Regolamento') },
            { resizable: true, editable: true, width: 100 }
        ),
        new KendoGridColumn(
            { field: 'Validita_Inizio', title: this.translocoLoc.translate('Validita_Inizio') },
            { resizable: true, editable: true, validators: [Validators.required], width: 100, date:{ defaultValue: AGRODATAINIZIO} }
        ),
        new KendoGridColumn(
            { field: 'Validita_Fine', title: this.translocoLoc.translate('Validita_Fine') },
            { resizable: true, editable: true, validators: [Validators.required], width: 100, date:{ defaultValue: AGRODATAFINE} }
        ),
        new KendoGridColumn(
            { field: 'EEC', title: this.translocoLoc.translate('Eec') },
            { resizable: true, editable: true, validators: [Validators.required], width: 140, numeric: {defaultValue: 0, format: 'n2'} }
        )
    ];

    constructor(
        injector: Injector,
        private giasMessageService: GiasMessageService,
        private impreseParametriGHGService: ImpreseParametriGHGService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private translocoLoc: TranslocoService,
        private impreseFilterService: ImpreseFilterService,
        private specieVegetaliService: SpecieVegetaliService,
        private specieVarietaService: VarietaService,
        private regolamentoService: RegolamentiService,
        private permessiUtenteService: PermessiUtenteService) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        this.edit = false;
        if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.Gestione_GHG, 2)) {
            this.edit = true;
        }


        // this.resizable.autoFitColumns = true;
        this.resizable.isResizable = true;

        // Setting generali
        this.columnMenu.columnMenu = true;
        this.behavior.excelSettings = new ExcelSettings({enabled: false});
        this.behavior.pdfSettings = new PDFSettings({enabled: false});
        this.generalSettings.reordable = true;
        this.groups.groupable.enabled = false;
        this.behavior.saveExternalChanges = this.edit;
        this.generalSettings.performOnEdit = this.edit;


        //* ************************
        this.handleCustomizations();
        /*this.objParametriAgendaService.currentObjParametriAgenda.GiasSubscribe(p => {
            this.gridPublicService.refresh(true);
        })*/
        this.handleDropdowns([]);

        this.gridPublicService.changeDetected.GiasSubscribe((event: any) => {
            if(event?.action === 'save' || event?.action === 'remove'){
                this.gridPublicService.refresh(true);
            }
            if(event?.action === 'add'){
                let fb = this.gridPublicService.formGroup.value
                if (fb != undefined) {
                    const colAzienda = this.kendoColumns.find(s => s.field === 'Piva');
                    if(this.impreseParametriGHGService.getFiltroRicerca().piva != '') {
                        colAzienda.ddl.data = [new DropdownListItem(this.impreseParametriGHGService.getImpresaSelezionata().codice, this.impreseParametriGHGService.getImpresaSelezionata().descrizione)]
                        fb.controls['Piva'].setValue(this.impreseParametriGHGService.getImpresaSelezionata().codice)
                    } else {
                        colAzienda.ddl.data = [new DropdownListItem('', '')]
                        fb.controls['Piva'].setValue({id:'', name:''})
                    }
                };
            }
        });
    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        switch (actionType) {
            case HttpAction.CREATE:

                if (items.Validita_Inizio > items.Validita_Fine) {
                    this.giasMessageService.errorMessage(this.transloco.translate('DataInizioNonDeveEssereMaggioreDiDataFine'));
                    break;
                }

                this.impreseParametriGHGService.scriviImpreseParametriGHG({   ID: 0,
                    Piva: items.Piva,
                    Veg_Cod: items.Specie_Cod ? items.Specie_Cod : 0,
                    Cul_Cod: items.Varieta_Cod ? items.Varieta_Cod : 0,
                    Regolamento_Cod: items.Regolamento_Cod ? items.Regolamento_Cod : 0,
                    EEC: items.EEC,
                    Validita_Inizio: items.Validita_Inizio,
                    Validita_Fine: items.Validita_Fine}
                ).subscribe()
            break;
            case HttpAction.REMOVE:
                this.impreseParametriGHGService.cancellaImpreseParametriGHG(items).subscribe()
            break;
            case HttpAction.UPDATE:
                
                if (items.Validita_Inizio > items.Validita_Fine) {
                    this.giasMessageService.errorMessage(this.transloco.translate('DataInizioNonDeveEssereMaggioreDiDataFine'));
                    break;
                }

                this.impreseParametriGHGService.modificaImpreseParametriGHG({   ID: items.ID,
                    Piva: items.Piva,
                    Veg_Cod: items.Specie_Cod ? items.Specie_Cod : 0,
                    Cul_Cod: items.Varieta_Cod ? items.Varieta_Cod : 0,
                    Regolamento_Cod: items.Regolamento_Cod ? items.Regolamento_Cod : 0,
                    EEC: items.EEC,
                    Validita_Inizio: items.Validita_Inizio,
                    Validita_Fine: items.Validita_Fine}
                ).subscribe()
            break;
        }
        this.impreseParametriGHGService.setImpreseParametriGHGGridRows(this.impreseParametriGHGRows)
        this.gridPublicService.refresh(true);
        return from([this.impreseParametriGHGService.getImpreseParametriGHGGridRows()])
    }

    read(): Observable<ImpreseParametriGHGKendo> {

        const agenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        const impreseParametriGHG: Observable<any> = this.impreseParametriGHGService.caricaImpreseParametriGHG();

        return impreseParametriGHG.pipe(map(result => {
            const righe = result.map((elem: ImpreseParametriGHGServerResult) => {
                return { ID: elem.ID,
                    Azienda: {codice: elem.Piva, descrizione: elem.Rag_Soc},
                    Specie: {codice: elem.Specie, descrizione: elem.Veg_Des},
                    Varieta: {codice: elem.Varieta, descrizione: elem.Cul_Des},
                    Regolamento: {codice: elem.Regolamento, descrizione: elem.Reg_Des},
                    Validita_Inizio: elem.Validita_Inizio,
                    Validita_Fine: elem.Validita_Fine,
                    EEC: elem.EEC
                }
            });
            const model = this.setKendoModel();
            const cols = this.kendoColumns;

            this.setKendoRows(righe);

            const tableData = new ImpreseParametriGHGKendoServerResult(model,
                cols,
                this.impreseParametriGHGRows
            );
            return tableData;

        }));
    }

    handleCustomizations(): void {
        //this.selectable = new AgrSelectableSettings();
        //this.selectable.selectable.checkboxOnly = true;
        //this.selectable.selectable.enabled = true;
        //this.selectable.shouldShowCheckbox = true;
        //this.selectable.columnSettings.showSelectAll = true;

        this.toolbar = new ToolbarSettings();
        this.toolbar.newItem = this.edit;
        this.toolbar.resetChanges = false;

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: this.edit,
            infoBtn: false,
            removeBtn: this.edit,
            onDisableInfoBtn: () => false
        });;

        this.resizable.autoFitColumns = false;
        //this.selectable.columnSettings.showSelectAll=true;
        //this.selectable.shouldShowCheckbox = this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB != Enum_DBTypeOperation.Read;
        //this.selectable.columnSettings.title=' ';
        this.resizable.isResizable = true;
        //this.selectable.selectable.checkboxOnly = true;
        //this.selectable.selectable.enabled = true;
        this.views.enabled = true
        this.columnMenu.columnMenu = false;
        this.behavior.excelSettings = new ExcelSettings({enabled:true});
        this.groups.groupable.enabled = true;
    }

    private setKendoModel () {
        const kendoModel: KendoGridModel = {
            ID: {
                editable: false,
                type: CELL_TYPES.NUMBER
            },
            Piva: {
                editable: true,
                type: CELL_TYPES.DROPDOWNLIST
            },
            Azienda_Des: {
                editable: true,
                type: CELL_TYPES.STRING
            },
            Specie_Cod: {
                editable: true,
                type: CELL_TYPES.DROPDOWNLIST
            },
            Specie_Des: {
                editable: true,
                type: CELL_TYPES.STRING
            },
            Varieta_Cod: {
                editable: true,
                type: CELL_TYPES.DROPDOWNLIST
            },
            Varieta_Des: {
                editable: true,
                type: CELL_TYPES.STRING
            },
            Regolamento_Cod: {
                editable: true,
                type: CELL_TYPES.DROPDOWNLIST
            },
            Regolamento_Des: {
                editable: true,
                type: CELL_TYPES.STRING
            },
            Validita_Inizio: {
                editable: true,
                type: CELL_TYPES.DATE
            },
            Validita_Fine: {
                editable: true,
                type: CELL_TYPES.DATE
            },
            EEC: {
                editable: true,
                type: CELL_TYPES.NUMBER
            }
        };

        return kendoModel;
    }

    private setKendoRows(righe: any[]) {
        this.impreseParametriGHGRows = righe.map((item) => ({
            ID: item.ID,
            Piva: item.Azienda.codice,
            Azienda_Des: item.Azienda.descrizione,
            Specie_Cod: item.Specie.codice,
            Specie_Des: item.Specie.descrizione,
            Varieta_Cod: item.Varieta.codice,
            Varieta_Des: item.Varieta.descrizione,
            Regolamento_Cod: item.Regolamento.codice,
            Regolamento_Des: item.Regolamento.descrizione,
            Validita_Inizio: item.Validita_Inizio,
            Validita_Fine: item.Validita_Fine,
            EEC: item.EEC
        }));
    }

    handleDropdowns(ddlist: any[]): void {
        const colAzienda = this.kendoColumns.find(s => s.field === 'Piva');
        const dataAzienda: DropdownListItem[] = [];
        colAzienda.ddl = new DropdownListWithForm('codice', 'Piva', 'descrizione', dataAzienda);
        colAzienda.ddl.valuePrimitive = true;
        colAzienda.ddl.id = 'codice';
        colAzienda.ddl.descriptionField = 'Azienda_Des';
        colAzienda.ddl.formControlValue = 'descrizione';
        colAzienda.ddl.loadOnEdit = true;
        colAzienda.ddl.loadFunction = this.caricaDropdownAzienda.bind(this);

        const colSpecie = this.kendoColumns.find(s => s.field === 'Specie_Cod');
        const dataSpecie: DropdownListItem[] = [];
        colSpecie.ddl = new DropdownListWithForm('codice', 'Specie_Cod', 'descrizione', dataSpecie);
        colSpecie.ddl.valuePrimitive = true;
        colSpecie.ddl.descriptionField = 'Specie_Des';
        colSpecie.ddl.loadOnEdit = true;
        colSpecie.ddl.loadFunction = this.caricaDropdownSpecie.bind(this);

        const colVarieta = this.kendoColumns.find(s => s.field === 'Varieta_Cod');
        const dataVarieta: DropdownListItem[] = [];
        colVarieta.ddl = new DropdownListWithForm('codice', 'Varieta_Cod', 'descrizione', dataVarieta);
        colVarieta.ddl.valuePrimitive = true;
        colVarieta.ddl.descriptionField = 'Varieta_Des';
        colVarieta.ddl.loadOnEdit = true;
        colVarieta.ddl.loadFunction = this.caricaDropdownVarieta.bind(this);

        const colRegolamento = this.kendoColumns.find(s => s.field === 'Regolamento_Cod');
        const dataRegolamento: DropdownListItem[] = [];
        colRegolamento.ddl = new DropdownListWithForm('codice', 'Regolamento_Cod', 'descrizione', dataRegolamento);
        colRegolamento.ddl.valuePrimitive = true;
        colRegolamento.ddl.descriptionField = 'Regolamento_Des';
        colRegolamento.ddl.loadOnEdit = true;
        colRegolamento.ddl.loadFunction = this.caricaDropdownRegolamento.bind(this);

    }

    private caricaDropdownAzienda() {
        const colAzienda = this.kendoColumns.find(s => s.field === 'Piva');
        if(colAzienda.ddl.data == undefined || colAzienda.ddl.data.length <= 1) {
            colAzienda.ddl.loading = true;
            return this.impreseFilterService.filtraImprese('').pipe(take(1), map(R => {
                colAzienda.ddl.loading = false;
                return R.map(t => {
                    return {codice:t.partitaIva, descrizione:t.ragioneSociale}
                })

            }));
        }
        return from([])
    }

    private caricaDropdownSpecie() {
        return from(this.specieVegetaliService.leggi_FiltroUtente()).pipe(take(1), map(R => {
            return R.map(t => {
                return {codice:t.codice, descrizione:t.descrizione}
            })
        }));
    }

    private caricaDropdownVarieta() {
        if(this.gridPublicService.formGroup.value.controls['Specie_Cod'].value != null) {
            return this.specieVarietaService.leggiAsObs(new Specie(this.gridPublicService.formGroup.value.controls['Specie_Cod'].value))
              .pipe(take(1), map(R => R.map(t => ({codice: t.codice, descrizione: t.descrizione}))));
        } else {
            return from([]);
        }
    }

    private caricaDropdownRegolamento() {
        return this.regolamentoService.leggi().pipe(map(R => {
            return R.map(t => {return {codice: t.codice, descrizione: t.descrizione}})
        }));
    }
}

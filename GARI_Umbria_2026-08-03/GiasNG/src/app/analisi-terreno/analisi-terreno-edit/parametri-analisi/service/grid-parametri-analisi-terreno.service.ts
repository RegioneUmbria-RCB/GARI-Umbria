import { Injectable, Injector } from "@angular/core";
import { Validators } from "@angular/forms";
import { TranslocoService } from "@jsverse/transloco";
import { ColumnComponent } from "@progress/kendo-angular-grid";
import { enum_AnalisiTipo } from "app/Model/TipiEnumerativi";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { CommandsColumnSettings, ExcelSettings, GroupSettings, PDFSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DropdownListWithForm, EditingMode, GridCustomizations, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { AnalisiTerrenoFormService } from "app/analisi-terreno/griglia-analisi-terreno/service/analisi-terreno-form.service";
import { AnalisiTerrenoService } from "app/analisi-terreno/griglia-analisi-terreno/service/analisi-terreno.service";
import { Observable, of } from "rxjs";
import { ObjParametriAgenda } from 'gias-ui-kit';

export class GridParametriAnalisiTerrenoServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

const NUMERIC_PATTERN = /^\d{1,10}([.,]\d{1,5})?$/;

@Injectable()
export class GridParametriAnalisiTerrenoHttpService extends AbstractGridConfigService<GridParametriAnalisiTerrenoServerResult> {
    
    editingMode: EditingMode = EditingMode.IN_CELL;
    loader: LoaderType = LoaderType.SERVICE;

    rowId: string = "codice";
    gridId: string = "ParamAnalisiTerrenoGridId";

    views = new GridCustomizations({ enabled: true });

    //array che contiene tutti i possibili parametri sulla DDL
    arrayAllParametri: any[];

    //array che contiene solo i parametri coerenti con lo schema selezionato sulla DDL
    arrayParametriToShow: any[];

    //parametri effettivamente mostrati in griglia
    paramOnGrid: any[] = [];

    edit: boolean = true;
    editCell: boolean = true;

    protected objParametriAgenda: ObjParametriAgenda;

    constructor(injector: Injector,
        public gridpublicService: GridPublicService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private analisiDataService: AnalisiTerrenoService,
        public analisiFormService: AnalisiTerrenoFormService,
        public transloco: TranslocoService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.groups = new GroupSettings({groupable: {enabled: false, showFooter: false}}, this.transloco);

        // this.columnMenu.columnMenu = false;
        // this.columnMenu.filterable = false;
        // this.behavior.saveExternalChanges = true;
        this.behavior.excelSettings = new ExcelSettings({enabled: false});
        this.behavior.pdfSettings = new PDFSettings({enabled: false});
        // this.generalSettings.reordable = false;
        this.generalSettings.performOnEdit = true;
        this.columnMenu.kendoGridColumnChooser = false;
        this.views.enabled = false;

        //DCA20250109 commentata perché fa casino in fase di update delle celle della griglia, 
        //rischiando di replicare alcuni parametri
        // this.pagination.gridState.sort = [{
        //   field: 'Valore',
        //   dir: 'desc'
        // }];

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read || this.analisiFormService.modeAgganciataPC_PUA) {
            this.edit = false;
            this.editCell = false;
        }

        let schema = this.analisiFormService.formAnalisiTerreno.get('analisiTipologia').value;

        if (schema && schema.codice !== 0) {
            this.arrayParametriToShow = schema.dettagli.map(item => item.parametro);
            this.paramOnGrid = this.arrayParametriToShow.map(item => {
                return {
                    ...item,
                    udm: item.unitaMisura.simbolo,
                    codiceDettaglio: 0,
                    Valore: null,
                    Errore: null
                };
            });
            this.edit = false;
        } else {
            this.arrayParametriToShow = this.arrayAllParametri;
            this.paramOnGrid = [];
            this.edit = true;
        }

        this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: this.edit && this.editCell });
        this.toolbar = new ToolbarSettings(this.edit && this.editCell, false);

        //questo lo devo eseguire se siamo in edit ed entriamo per la prima volta sulla griglia
        // if (this.analisiFormService.formAnalisiTerreno.get('dettagli').value?.length > 1) {

            // let arrDettagli = this.analisiFormService.formAnalisiTerreno.get('dettagli').value;

        if (this.analisiFormService.arrayParametri.length > 0) {

            let arrDettagli = this.analisiFormService.arrayParametri;

            if (this.paramOnGrid.length == 0)
                // this.paramOnGrid = arrDettagli.slice(1).map(item => {
                //     return {
                //         ...item.parametro,
                //         Valore: item.valore1,
                //         Errore: item.valore2
                //     };
                // });
                this.paramOnGrid = arrDettagli;
            else {

                this.paramOnGrid.forEach(item => {
                    let index = arrDettagli.findIndex(itemDett => item.codice == itemDett.codice);
                    if (index >= 0) {
                        item.codiceDettaglio = arrDettagli[index].codiceDettaglio;
                        item.Valore = arrDettagli[index].Valore;
                        item.Errore = arrDettagli[index].Errore;
                    }
                });

                this.analisiFormService.arrayParametri = this.analisiFormService.arrayParametri.filter(item => 
                                                                                                                this.paramOnGrid.some(ref => ref.codice === item.codice) &&
                                                                                                                (item.Valore !== null || item.Errore !== null) 
                );
            }

            //ordino prima quelli valorizzati, poi gli altri per codice
            this.paramOnGrid.sort((a, b) => {
                
                const aHasValore = a.Valore != null;
                const bHasValore = b.Valore != null;
            
                if (aHasValore && !bHasValore) return -1;
                if (!aHasValore && bHasValore) return 1;

                const aHasErrore = a.Errore != null;
                const bHasErrore = b.Errore != null;
            
                if (aHasErrore && !bHasErrore) return -1;
                if (!aHasErrore && bHasErrore) return 1;
            
                // 3. Se né `valore` né `errore` sono valorizzati, ordina per `codice`
                return (a.codice <= b.codice) ? -1 : 1;
            });
        }

        // this.analisiFormService.formAnalisiTerreno.get('analisiTipologia').valueChanges.subscribe(r => {
            
        //     if (!r || r?.codice === 0) {
        //         this.arrayParametriToShow = this.arrayAllParametri;
        //         this.paramOnGrid = [];
        //         this.edit = true;
        //     } else {
        //         this.arrayParametriToShow = r.dettagli.map(item => item.parametro);
        //         this.paramOnGrid = this.arrayParametriToShow.map(item => {
        //             return {
        //                 ...item,
        //                 Valore: null,
        //                 Errore: null
        //             };
        //         });
        //         this.edit = false;
        //     }

        //     this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: this.edit });

        //     this.toolbar = new ToolbarSettings(this.edit, false);

        //     this.gridPublicService.refresh(true);
        // });

        // this.behavior.excelSettings.enabled = false;
        let paramAnalisi = {
            Tipo_Analisi: enum_AnalisiTipo.Analisi_Terreno //,
            // Analisi_Tipologia_Cod: (this.analisiFormService.formAnalisiTerreno.get('analisiTipologia').value) ? this.analisiFormService.formAnalisiTerreno.get('analisiTipologia').value.codice : 0
        };

        this.analisiDataService.LeggiParametriAnalisi(paramAnalisi).subscribe((r:any) => {
            this.arrayAllParametri = r.map(item => {
                                                    return { 
                                                    ...item.parametro,
                                                    udm: item.parametro.unitaMisura.simbolo
                                                    }
        });
        });

        // this.gridPublicService.changeDetected.GiasSubscribe((event: any) => { 
        //     console.log(event);
        //     if (event?.action == 'save') {
        //         if (!this.isNumber(event.Valore) || !this.isNumber(event.Errore))
        //             event.preventDefault();
        //     }
        // });

        this.gridPublicService.formGroup.subscribe(fb => {
            if (fb) {
                fb.controls['udm'].disable({ emitEvent: false });
                fb.controls['codice'].valueChanges.subscribe(val => {
                    let udm = this.arrayAllParametri.find(item => item.codice == val)?.udm || null;
                    fb.controls['udm'].setValue(udm);
                });
            }
        });

        this.preventEdit = this.preventEditCellUdM;

    }


    private preventEditCellUdM(dataItem, col: ColumnComponent) {

        if (col.field == 'udm' || col.field == 'codice')
            return true;

        return false;
    }

    read(options?: any): Observable<GridParametriAnalisiTerrenoServerResult> {
        // this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

        // let paramAnalisi = {
        //     Tipo_Analisi: enum_AnalisiTipo.Analisi_Terreno //,
        //     // Analisi_Tipologia_Cod: (this.analisiFormService.formAnalisiTerreno.get('analisiTipologia').value) ? this.analisiFormService.formAnalisiTerreno.get('analisiTipologia').value.codice : 0
        // };

        // return this.analisiDataService.LeggiParametriAnalisi(paramAnalisi).pipe(catchError((err) => {
        //     this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
        //     return of();
        // }), map((r:any) => {

            // this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

        let gridListaParamAnalisiServerResult;

        // let arrayToShow = [];

        // if (this.analisiFormService.formAnalisiTerreno.get('dettagli').value?.length > 1) {

        //     let arrDettagli = this.analisiFormService.formAnalisiTerreno.get('dettagli').value;

        //     arrayToShow = arrDettagli.slice(1).map(item => { 
        //         return {
        //             ...item.parametro,
        //             Valore: item.valore1,
        //             Errore: item.valore2
        //         };
        //     });
        // }

        gridListaParamAnalisiServerResult = new GridParametriAnalisiTerrenoServerResult(
            this.setRowGrid(this.paramOnGrid),
            this.setColumnsGrid(),
            this.setKendoModelGrid()
        );

        return of(gridListaParamAnalisiServerResult);
        // }));
    }

    setRowGrid(responseRows) {
        let rows: Array<KendoGridRow> = [];
        // let indexRowParamAnalisiTerreno = 0;

        for (let itemOfRows of responseRows) {
            // indexRowParamAnalisiTerreno++;
            // itemOfRows.idParamAnalisiTerrenoGrid = indexRowParamAnalisiTerreno;
            rows.push(
                itemOfRows
            );
        }

        return rows;
    }

    setColumnsGrid(): Array<KendoGridColumn> {
        let columns: Array<KendoGridColumn> = [];
        // for (let itemOfColumns of columnsName) {
        //     columns.push(
        //         new KendoGridColumn({ field: itemOfColumns, title: this.transloco.translate(itemOfColumns) }, { resizable: true, editable: false, hidden: (itemOfColumns == 'Analisi_Parametro_Des') ? false : true })
        //     );
        // }

        // columns.push(
        //     new KendoGridColumn({ field: 'Analisi_Parametro_Cod', title: this.transloco.translate('Analisi_Parametro_Cod') }, { resizable: true, editable: false, hidden: true })
        // );

        let ddl;

        const data = [];
        ddl = new DropdownListWithForm('codice', 'codice', 'descrizione', data);
        ddl.valuePrimitive = true;
        ddl.loadOnEdit = true;
        ddl.loadFunction = this.loadDdl.bind(this);
        ddl.descriptionField = 'descrizione';

        columns.push(
            new KendoGridColumn({ field: 'codice', title: this.transloco.translate('Descrizione') }, { resizable: true, editable: this.edit, ddl: ddl, validators: this.edit ? [Validators.required] : [] })
        );

        columns.push(
            new KendoGridColumn({ field: 'udm', title: this.transloco.translate('RisorsaUDM') }, { resizable: true, editable: this.edit })
        );

        // columns.push(
            // new KendoGridColumn({ field: 'Analisi_Parametro_Des', title: this.transloco.translate('Analisi_Parametro_Des')}, { resizable: true, editable: false, ddl: ddl, validators: [Validators.required] })
        // );
        columns.push(
            new KendoGridColumn({ field: 'Valore', title: this.transloco.translate('Valore') }, { resizable: true, 
                                                                                                  editable: this.editCell, 
                                                                                                  validators: this.edit ? [Validators.required, Validators.pattern(NUMERIC_PATTERN)] : [Validators.pattern(NUMERIC_PATTERN)],
                                                                                                //   numeric: new NumericSettings(
                                                                                                //                                 // {  
                                                                                                //                                 //     defaultValue: null,
                                                                                                //                                 //     min: 0,
                                                                                                //                                 //     max: Number.MAX_VALUE,
                                                                                                //                                 //     format: 'n2',
                                                                                                //                                 //     autoCorrect: false,
                                                                                                //                                 //     decimals: 2,
                                                                                                //                                 //     step: null,
                                                                                                //                                 //     multiCheckFiltering: false
                                                                                                //                                 // }
                                                                                                //                             )
                                                                                                })
        );
        columns.push(
            new KendoGridColumn({ field: 'Errore', title: this.transloco.translate('Errore_') }, { resizable: true, editable: this.editCell, validators: [Validators.pattern(NUMERIC_PATTERN)]
                                                                                                                                // , numeric: new NumericSettings(
                                                                                                                                //                                 // {  
                                                                                                                                //                                 //     defaultValue: null,
                                                                                                                                //                                 //     min: 0,
                                                                                                                                //                                 //     max: Number.MAX_VALUE,
                                                                                                                                //                                 //     format: 'n2',
                                                                                                                                //                                 //     autoCorrect: false,
                                                                                                                                //                                 //     decimals: 2,
                                                                                                                                //                                 //     step: null,
                                                                                                                                //                                 //     multiCheckFiltering: false
                                                                                                                                //                                 // }
                                                                                                                                //                             ) 
                                                                                                })
        );

        this.columns = columns.map(x => Object.assign({}, x));

        return columns;
    }

    setKendoModelGrid(): KendoGridModel {

        let gridModel = new KendoGridModel();

        // arrayModel.forEach((item) => {
        //     gridModel[item] = { editable: false, type: 'string' };
        // });

        gridModel['codice'] = { editable: false, type: CELL_TYPES.DROPDOWNLIST };
        gridModel['descrizione'] = { editable: true, type: CELL_TYPES.STRING };
        gridModel['udm'] = { editable: this.edit, type: CELL_TYPES.STRING };

        gridModel['Valore'] = { editable: true, type: CELL_TYPES.STRING };
        gridModel['Errore'] = { editable: true, type: CELL_TYPES.STRING };

        return gridModel;
    }

    // handleDropdowns(): void {
    //     const col = this.columns.find(s => s.field === 'Analisi_Parametro_Des');

    //     //const data: DropdownListItem[] = ddlist.map(cod => new DropdownListItem(cod.codice, cod.descrizione));
    //     const data = [];
    //     col.ddl = new DropdownListWithForm('parametri', 'Analisi_Parametro_Des', 'descrizione', data);
    //     col.ddl.valuePrimitive = true;
    //     col.ddl.loadOnEdit = true;
    //     col.ddl.loadFunction = this.loadDdl.bind(this);
    //     col.ddl.descriptionField = 'Analisi_Parametro_Des';

    // }

    loadDdl() {
        let arrRows = this.gridPublicService.value.data.rows;
        // console.log(arrRows);
        let arrayParametri = (this.arrayParametriToShow) ? this.arrayParametriToShow : this.arrayAllParametri; 

        let arrToShow = arrayParametri.filter(itemToShow => arrRows.findIndex(itemRow => itemRow['codice'] == itemToShow.codice) == -1);
        return of(arrToShow);
    }

    perform(actionType: HttpAction, item: any): Observable<any> {
        // throw new Error("Method not implemented.");

        switch(actionType) {
            case HttpAction.CREATE:
                let paramObj = this.arrayAllParametri.find(itemOfAll => itemOfAll.codice === item[0].codice);
                this.paramOnGrid.push({
                    ...paramObj,
                    codiceDettaglio: 0,
                    Valore: item[0].Valore,
                    Errore: item[0].Errore
                });
                this.analisiFormService.arrayParametri = this.paramOnGrid;
            break;

            case HttpAction.REMOVE:
                const indexRemove = this.paramOnGrid.findIndex(itemOnGrid => item[0].codice === itemOnGrid.codice);
                this.paramOnGrid.splice(indexRemove, 1);
            break;

            case HttpAction.UPDATE:
                const indexUpdate = this.analisiFormService.arrayParametri.findIndex(itemOnGrid => item[0].codice === itemOnGrid.codice);
                if (indexUpdate < 0)
                    this.analisiFormService.arrayParametri.push(item[0]);
                else {
                    this.analisiFormService.arrayParametri[indexUpdate].codiceDettaglio = item[0].codiceDettaglio;
                    this.analisiFormService.arrayParametri[indexUpdate].Valore = item[0].Valore;
                    this.analisiFormService.arrayParametri[indexUpdate].Errore = item[0].Errore;
                }
            break;
        }

        // mi salvo mano a mano l'array dei parametri che potrei salvare
        // this.analisiFormService.arrayParametri = this.paramOnGrid;

        this.gridPublicService.refresh(true);
        return of(this.paramOnGrid);

    }

    // public override onCellClose = (event: CellCloseEvent, inputElementRef) => { 
    //     if ((event.column.field === 'Valore' && !this.isNumber(event.Valore)) || (event.column.field === 'Errore' && !this.isNumber(event.Errore))) {
    //         // Blocca l'evento e riapre la cella
    //         event.preventDefault();
    //     }
    //     // console.log(event);
    //     // console.log(inputElementRef);
    // };

    // private isNumber(value: any): boolean {
    //     if (typeof value === 'string') {
    //         value = value.replace(',', '.');
    //     }
    //     return !isNaN(parseFloat(value)) && isFinite(value);
    // }

}
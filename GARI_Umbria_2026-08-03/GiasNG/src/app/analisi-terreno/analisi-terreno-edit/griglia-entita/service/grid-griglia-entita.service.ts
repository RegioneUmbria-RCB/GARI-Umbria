import { Injectable, Injector } from "@angular/core";
import { enum_Entita_Analisi } from "app/Model/TipiEnumerativi";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { CommandsColumnSettings, ExcelSettings, PDFSettings, SelectableSettings, Sortable, SortMode, SortSettings } from 'gias-kendo-grid';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { AnalisiTerrenoFormService } from "app/analisi-terreno/griglia-analisi-terreno/service/analisi-terreno-form.service";
import { AnalisiTerrenoService } from "app/analisi-terreno/griglia-analisi-terreno/service/analisi-terreno.service";
import { BehaviorSubject, Observable, catchError, map, of, switchMap } from "rxjs";
import { EntitaConfigService } from "./entita-config.service";
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { SelectionEvent } from "@progress/kendo-angular-grid";
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";

export class GridEntitaServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class GridEntitaHttpService extends AbstractGridConfigService<GridEntitaServerResult> {

    editingMode: EditingMode = EditingMode.IN_PAGE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId: string = "idEntita";
    gridId: string = "EntitaGridId";

    objParams;

    allRows: any[] = [];

    entitaCoinvolta: BehaviorSubject<enum_Entita_Analisi> = new BehaviorSubject<enum_Entita_Analisi>(null);

    constructor(injector: Injector,
        public gridpublicService: GridPublicService,
        private objParametriAgenda: ObjParametriAgendaService,
        private entitaConfigService: EntitaConfigService,
        public analisiFormService: AnalisiTerrenoFormService,
        public analisiDataService: AnalisiTerrenoService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.objParams = this.objParametriAgenda.getObjParamValue();

        this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });

        this.behavior.excelSettings = new ExcelSettings({ enabled: false });
        this.behavior.pdfSettings = new PDFSettings({ enabled: false });
        // this.generalSettings.reordable = false;
        this.generalSettings.performOnEdit = true;
        this.columnMenu.kendoGridColumnChooser = true;
        this.views.enabled = false;

        this.selectable.selectable = new SelectableSettings({
            checkboxOnly: true,
            enabled: (this.objParams.TipoOperazioneDB !== Enum_DBTypeOperation.Read && this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').value.length <= 1)
        });

        this.selectable.preselectedRows.selectionChangeFn = this.selectionChangeFn;
        this.selectable.columnSettings.showSelectAll = true;
        this.selectable.shouldShowCheckbox = true;
        this.selectable.columnSettings.title = ' ';

        this.groups.groupable.enabled = false;

        this.resizable.autoFitColumns = true;
    }

    private handleDuplicates(row: any[], component: GiasKendoGridComponent, selected: boolean) {
        if (this.entitaCoinvolta.getValue() == enum_Entita_Analisi.Particella) {

            const buildKey = (o: any) =>
                            `${o.Prov}|${o.Com}|${o.SEZIONE}|${o.FOGLIO}|${o.NUMERO}|${o.SUBALTERNO}|${o.Sa_Cod}|${o.Piva}`; 

            const lookup = new Set(row.map(r => buildKey(r)));

            let rows = this.gridPublicService.getValue().data.rows.filter(riga => lookup.has(buildKey(riga)));

            rows.forEach(riga => riga['Selected'] = selected);

        }
    }

    public selectionChangeFn = (event: SelectionEvent, component: GiasKendoGridComponent) => {
        const selectedRows = event.selectedRows.map(item => item.dataItem);
        const deselectedRows = event.deselectedRows.map(item => item.dataItem);

        if (selectedRows.length > 0) {
            const toBeAdded = selectedRows.filter(item => !this.analisiFormService.arrayEntita.find(itemEntita => itemEntita.chiave == item.chiave));
            this.analisiFormService.arrayEntita.push(...toBeAdded);
            this.handleDuplicates(selectedRows, component, true);
        }

        if (deselectedRows.length > 0) {
            this.analisiFormService.arrayEntita = this.analisiFormService.arrayEntita.filter(item => !deselectedRows.find(itemEntita => itemEntita.chiave == item.chiave));
            this.handleDuplicates(deselectedRows, component, false);
        }

        if (this.analisiFormService.arrayEntita.length > 0) {
            this.analisiFormService.formAnalisiTerreno.get('validita')?.disable({ emitEvent: false });
        } else {
            this.analisiFormService.formAnalisiTerreno.get('validita')?.enable({ emitEvent: false });
        }


        GridEntitaHttpService.applySort(this.gridPublicService?.getValue()?.data?.rows ?? []);
    }

    read(options?: any): Observable<GridEntitaServerResult> {

        return this.entitaCoinvolta.pipe(
            switchMap(entitaValue => {

                let resp: Observable<any>;

                this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

                if (entitaValue !== null || entitaValue !== undefined) {
                    switch (entitaValue) {

                        case enum_Entita_Analisi.Campo:
                            resp = this.analisiDataService.LeggiCampiEntita(this.objParams);
                            break;

                        case enum_Entita_Analisi.Centro:
                            resp = this.analisiDataService.LeggiCentriEntita(this.objParams);
                            break;

                        case enum_Entita_Analisi.Appezzamento:
                            resp = this.analisiDataService.LeggiAppezzamentiEntita(this.objParams);
                            break;

                        case enum_Entita_Analisi.Impianto:
                            resp = this.analisiDataService.LeggiImpiantiEntita(this.objParams);
                            break;

                        case enum_Entita_Analisi.Particella:
                            resp = this.analisiDataService.LeggiCatastoEntita(this.objParams);
                            break;
                    }
                }

                return resp.pipe(catchError((err) => {
                    this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
                    return of();
                }), map((r: any) => {

                    this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

                    const valInizio = this.analisiFormService.formAnalisiTerreno.get('validita')?.get('inizio')?.value ?? AGRODATAINIZIO;
                    const valFine = this.analisiFormService.formAnalisiTerreno.get('validita')?.get('fine')?.value ?? AGRODATAFINE;

                    if (Array.isArray(r)) {
                        this.allRows = r;
                        r = this.arrayFiltersData(r, entitaValue, valInizio, valFine);
                    } else {
                        this.allRows = r.kendo_rows;
                        r.kendo_rows = this.arrayFiltersData(r.kendo_rows, entitaValue, valInizio, valFine);
                    }

                    const rows = Array.isArray(r) ? this.setRowGrid(r, entitaValue) : this.setRowGrid(r.kendo_rows, entitaValue);
                    const columns = Array.isArray(r) ? this.setColumnsGrid(r.length > 0 ? Object.keys(r[0]) : [], entitaValue) : this.setColumnsGrid(r.kendo_columns, entitaValue);
                    const model = Array.isArray(r) ? this.setModelGrid(r.length > 0 ? Object.keys(r[0]) : [], entitaValue) : r.kendo_model;

                    let gridEntitaServerResult: GridEntitaServerResult = new GridEntitaServerResult(rows, columns, model);
                    return gridEntitaServerResult;
                }));
            })
        );
    }

    private arrayFiltersData(arrEntities: any[], entita: enum_Entita_Analisi, valInizio, valFine) {

        if (entita == enum_Entita_Analisi.Impianto) {
            //per gli impianti, occorre anche fare una distinct 
            const seen = new Set();
            return arrEntities
                .filter(item => {
                    return item.Validita_Inizio_Impianto <= valFine && item.Validita_Fine_Impianto >= valInizio;
                })
                .filter(item => {
                    const key = item.PIVA + "_" + item.SA_COD + "_" + item.APPEZZA + "_" + item.id_Reg;
                    if (seen.has(key)) return false;
                    seen.add(key);
                    return true;
                })
                .sort((a, b) => {
                    return a.chiave.localeCompare(b.chiave)
                });
        } else
            return arrEntities.filter(item => {
                return item.Validita_Inizio <= valFine && item.Validita_Fine >= valInizio;
            });
    }

    setRowGrid(responseRows, entitaValue: enum_Entita_Analisi) {
        let rows: Array<KendoGridRow> = [];
        let idEntita = 0;

        for (let itemOfRows of responseRows) {
            itemOfRows.idEntita = idEntita;
            itemOfRows.Selected = this.setSelectedAttributeRow(itemOfRows, entitaValue);

            if (itemOfRows.Selected && this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').value.length == 1) {
                this.analisiFormService.formAnalisiTerreno.get('validita')?.disable({ emitEvent: false });
                this.analisiFormService.arrayEntita.push(itemOfRows);
            }

            idEntita++;

            if ((this.objParams.TipoOperazioneDB !== Enum_DBTypeOperation.Read && this.analisiFormService.formAnalisiTerreno.get('entitaCoinvolte').value.length <= 1) || itemOfRows.Selected)
                rows.push(
                    itemOfRows
                );
        }

        return GridEntitaHttpService.applySort(rows);
    }

    setColumnsGrid(columnsName, entitaValue: enum_Entita_Analisi): Array<KendoGridColumn> {

        switch (entitaValue) {

            case enum_Entita_Analisi.Campo:
                return this.entitaConfigService.kendoColumnsCampi;
                break;

            case enum_Entita_Analisi.Centro:
                return this.entitaConfigService.kendoColumnsCentri;
                break;

            case enum_Entita_Analisi.Impianto:
                return this.entitaConfigService.kendoColumnsImpianti;
                break;

            case enum_Entita_Analisi.Appezzamento:
                return this.entitaConfigService.kendoColumnsAppezzamenti;
                break;

            case enum_Entita_Analisi.Particella:
                return columnsName.filter(item => item.field !== 'part_cod' && item.field !== 'Sa_Cod' && item.field !== 'Piva' &&
                    item.field !== 'Titolo_Possesso_Cod' && item.field !== 'MetodoProduzione_Cod' &&
                    item.field !== 'Attivo' && item.field !== 'Data_Creazione' && item.field !== 'Utente_Creazione' &&
                    item.field !== 'Data_Modifica' && item.field !== 'Utente_Modifica' && item.field !== 'Prov' &&
                    item.field !== 'Com'
                );
                break;

            default:
                let columns: Array<KendoGridColumn> = [];
                for (let itemOfColumns of columnsName) {
                    columns.push(
                        new KendoGridColumn({ field: itemOfColumns, title: itemOfColumns }, { resizable: true, editable: false })
                    );
                }
                return columns;
                break;

        }
    }

    setModelGrid(model, entitaValue: enum_Entita_Analisi) {

        switch (entitaValue) {

            case enum_Entita_Analisi.Campo:
                return this.entitaConfigService.kendoModelCampi;
                break;

            case enum_Entita_Analisi.Centro:
                return this.entitaConfigService.kendoModelCentri;
                break;

            case enum_Entita_Analisi.Impianto:
                return this.entitaConfigService.kendoModelImpianti;
                break;

            case enum_Entita_Analisi.Appezzamento:
                return this.entitaConfigService.kendoModelAppezzamenti;
                break;

            default:
                const objModel: { [key: string]: any } = {};
                model.forEach(key => {
                    objModel[key] = new ModelEntry(CELL_TYPES.STRING);
                });
                return objModel;
                break;
        }
    }

    setSelectedAttributeRow(itemOfRows: any, entitaValue: enum_Entita_Analisi): boolean {

        //sono in scrittura, devo postare il primo elemento a selected true e aggiornare l'array
        if (this.objParams.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
            return false;
        }

        switch (entitaValue) {
            case enum_Entita_Analisi.Campo:
                let arrCampi = this.analisiFormService.formAnalisiTerreno.get('entitaCampi').getRawValue();
                return arrCampi.some(item => item.elementoAnagrafico.primaryKey.centroAziendalePK.codice == itemOfRows.sa_cod
                    && item.elementoAnagrafico.primaryKey.centroAziendalePK.partitaIva == itemOfRows.PIVA
                    && item.elementoAnagrafico.primaryKey.codice == itemOfRows.Campo_Cod);
                break;

            case enum_Entita_Analisi.Centro:
                let arrCentri = this.analisiFormService.formAnalisiTerreno.get('entitaCentri').getRawValue();
                return arrCentri.some(item => item.elementoAnagrafico.primaryKey.codice == itemOfRows.sa_cod
                    && item.elementoAnagrafico.primaryKey.partitaIva == itemOfRows.Piva);
                break;

            case enum_Entita_Analisi.Appezzamento:
                let arrAppezzamenti = this.analisiFormService.formAnalisiTerreno.get('entitaAppezzamenti').getRawValue();
                return arrAppezzamenti.some(item => item.elementoAnagrafico.primaryKey.centroAziendalePK.codice == itemOfRows.Sa_Cod
                    && item.elementoAnagrafico.primaryKey.centroAziendalePK.partitaIva == itemOfRows.Piva
                    && item.elementoAnagrafico.primaryKey.codice == itemOfRows.Appezza);
                break;

            case enum_Entita_Analisi.Impianto:
                let arrImpianti = this.analisiFormService.formAnalisiTerreno.get('entitaImpianti').getRawValue();
                return arrImpianti.some(item => item.elementoAnagrafico.primaryKey.appezzamentoPK.centroAziendalePK.codice == itemOfRows.SA_COD
                    && item.elementoAnagrafico.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva == itemOfRows.PIVA
                    && item.elementoAnagrafico.primaryKey.appezzamentoPK.codice == itemOfRows.APPEZZA
                    && item.elementoAnagrafico.primaryKey.codice == itemOfRows.id_Reg);
                break;

            case enum_Entita_Analisi.Particella:
                let arrCatasto = this.analisiFormService.formAnalisiTerreno.get('entitaParticelleCatastali').getRawValue();
                return arrCatasto.some(item => item.elementoAnagrafico.particella.primaryKey.Prov == itemOfRows.Prov
                    && item.elementoAnagrafico.particella.primaryKey.Com == itemOfRows.Com
                    && item.elementoAnagrafico.particella.primaryKey.Sezione == itemOfRows.SEZIONE
                    && item.elementoAnagrafico.particella.primaryKey.Foglio == itemOfRows.FOGLIO
                    && item.elementoAnagrafico.particella.primaryKey.Numero == itemOfRows.NUMERO
                    && item.elementoAnagrafico.particella.primaryKey.Subalterno == itemOfRows.SUBALTERNO
                    && item.elementoAnagrafico.centro.codice == itemOfRows.Sa_Cod
                    && item.elementoAnagrafico.centro.partitaIva == itemOfRows.Piva);
                break;
        }
    }

    handlerChangeDates(values, entitaValue: enum_Entita_Analisi) {
        this.analisiFormService.arrayEntita.splice(0, this.analisiFormService.arrayEntita.length);

        let statusGrid = this.gridpublicService.getValue();

        const validitaInizio = values.inizio;
        const validitaFine = values.fine;

        if (validitaFine && validitaInizio && new Date(validitaFine) < new Date(validitaInizio)) {
            statusGrid.data.rows = [];
        } else {
            if (this.allRows.length > 0) {    //ricarico la griglia filtrata
                statusGrid.data.rows = this.arrayFiltersData(this.allRows, entitaValue, validitaInizio, validitaFine);
            }
        }

        this.gridpublicService.refresh(false, statusGrid.data);
    }

    perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
        throw new Error("Method not implemented.");
    }

    public static applySort(rows: KendoGridRow[]): KendoGridRow[] {
        return rows.sort((a, b) => Number(b['Selected'] || 0) - Number(a['Selected'] || 0))
    }
}
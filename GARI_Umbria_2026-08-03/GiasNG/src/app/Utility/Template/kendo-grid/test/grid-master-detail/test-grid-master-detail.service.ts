import {Injectable, Injector} from '@angular/core';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import { CommandsColumnSettings, ConfigTemplate, EditingMode, GridCustomizations, GridPublicService, GroupSettings, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, MasterDetailSettings, ModelEntry, ToolbarSettings} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {Observable, of, takeUntil} from 'rxjs';
import {KendoGridMasterDetailService} from "../../services/grid-master-detail.service";

export class GridMasterServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()

export class TestGridMasterService extends AbstractGridConfigService<GridMasterServerResult>{
    gridId = 'GridMaster';

    //Indicare un rowId non modificabile e univoco se si vuole fare una GridMaster per il masterdetail
    rowId = 'key_grid';


    GridMasterServerResult: GridMasterServerResult;

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;

    views = new GridCustomizations({enabled: true});

    rows = [];

    constructor(injector: Injector,
                public gridpublicService: GridPublicService,
                private kendoGridMasterservice: KendoGridMasterDetailService) {

        super(injector, ConfigTemplate.DefaultTemplate);

        // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: true,
            infoBtn: false,
            removeBtn: true,
            onDisableInfoBtn: () => false,
            width: 30
        });

        this.cmdColumn.title= null;

        this.toolbar = new ToolbarSettings(
            true,
            false,
            'Azioni',
            150,
            '',
            false
        );

        this.resizable.autoFitColumns=false;

        this.resizable.isResizable=true;

        this.selectable.shouldShowCheckbox = false;

        this.columnMenu.kendoGridColumnChooser=true;

        this.behavior.excelSettings.enabled=false;

        this.behavior.pdfSettings.enabled=false;

        this.generalSettings.performOnEdit=false;

        this.generalSettings.height = 'auto';

        this.masterdetailSettings = new MasterDetailSettings(true,{flag_grid_detail: false,flag_grid_master: true,showDetailTemplate: (dataitem:any,rowIndex:number)=> dataitem.codice !== 3});

        this.pagination.pageable = true;

        this.groups = new GroupSettings({groupable: {enabled: false, showFooter: false}},null);

        this.gridpublicService.changeDetected.pipe(takeUntil(this.signal)).subscribe((event: any) => {
            switch(event?.action){
                case 'save':
                case 'cellClose':
                    console.log("Master Rows:"+ this.kendoGridMasterservice.GridDataMaster.data.rows);
                    this.kendoGridMasterservice.GridDataDetail.forEach(d=>{
                        console.log("Detail_"+d.GridMasterRowId+"Rows:"+d.GridData.data.rows);
                    })

                    break;
                case 'edit':

                    break;
                case 'remove':
                    break;
                default:
                    break;
            }
        });

        for(let x = 1; x< 20;x++){
            this.rows.push({
                codice: x,
                descrizione: "Descrizione_Master_"+x,
                key_grid: x
            })
        }

    }

    perform(actionType: HttpAction, item: any): Observable<any> {

        switch(actionType){
            case HttpAction.CREATE:
                this.rows.push(item);
                break;
            case HttpAction.REMOVE:
                this.rows.splice(this.rows.findIndex(x=>x[this.rowId] === item[this.rowId]),1);
                break;
            case HttpAction.UPDATE:
                this.rows[this.rows.findIndex(x=>x[this.rowId] === item[this.rowId])]=  this.rows;
                break;
        }

        return of([]);
    }

    read(): Observable<GridMasterServerResult> {

        this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});

        return of(this.setGridMasterServerResult(this.rows));

    }

    setGridMasterServerResult(rows): GridMasterServerResult{

        this.GridMasterServerResult = new GridMasterServerResult (<KendoGridRow[]>(rows),
            this.setColumnsGridMacchine(),
            this.setModelGridMacchine());

        this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});

        return this.GridMasterServerResult;
    }

    setColumnsGridMacchine(){

        let columns: Array<KendoGridColumn>=[
            new KendoGridColumn({field: 'codice',title: 'Codice'},{resizable:true,editable: true,numeric:{defaultValue: 0}}),
            new KendoGridColumn({field: 'descrizione',title: 'Descrizione'},{resizable:true,editable: true})
        ];


        return columns;
    }

    setModelGridMacchine(){

        const model: KendoGridModel={
            codice: new ModelEntry(CELL_TYPES.NUMBER),
            descrizione: new ModelEntry(CELL_TYPES.STRING),
            key_grid: new ModelEntry(CELL_TYPES.NUMBER)
        };

        return model;
    }

}

import { Injectable, Injector } from "@angular/core";
import { DSSClient, LeggiRilieviPiogge } from "app/Service/api.service";
import { AbstractGridConfigService,CommandsColumnSettings, EditingMode, HttpAction, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ConfigTemplate, StringManager } from "gias-kendo-grid";
import { map, Observable } from "rxjs";


export class GridRilievoPioggeServerResult extends KendoServerResult{
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class GridRilievoPioggeService extends AbstractGridConfigService<GridRilievoPioggeServerResult> {
    editingMode: EditingMode = EditingMode.IN_LINE;
    loader: LoaderType= LoaderType.SERVICE;
    gridId: string = "RilievoPioggeGridId";
    rowId: string = "RilievoPioggeRowId";

    constructor(injector: Injector,
        private dssClient: DSSClient
    ){
        super(injector, ConfigTemplate.DefaultTemplate);

        this.views.enabled = false;
        this.groups.groupable.enabled = false;
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false
        });

        this.behavior.excelSettings.enabled = true;
        this.behavior.pdfSettings.enabled = true;
    }

    Piva: string;
    DataDa: Date;
    DataA: Date;

    public refreshGrid() : void {
        this.gridPublicService.refresh(true);
    }

    read(options?: any): Observable<GridRilievoPioggeServerResult> {
        this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });
        let leggiRilieviPiogge: LeggiRilieviPiogge = {
            Piva: this.Piva,
            DataDa: this.DataDa,
            DataA: this.DataA
        };

        return this.dssClient.dSSCercaPioggeIrrigazione(leggiRilieviPiogge)
            .pipe(map((r: any) => {
                let objResult = JSON.parse(r.RispostaStringa);

                let gridRilievoPioggeServerResult = new GridRilievoPioggeServerResult(
                    this.setRowGrid(objResult.kendo_rows),
                    this.setColumnsGrid(objResult.kendo_columns),
                    objResult.kendo_model
                );

                this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

                return gridRilievoPioggeServerResult;
            }));
    }

    setRowGrid(responseRows){
        let rows: Array<KendoGridRow> = [];
        for (let itemOfRows of responseRows){
            rows.push(itemOfRows);
        }

        return rows;
    }

    setColumnsGrid(columnsName): Array<KendoGridColumn>{
        let columns: Array<KendoGridColumn> = [];
        for (let itemOfColumns of columnsName){
            let kendoGridColumn = new KendoGridColumn({field: itemOfColumns.field, title: itemOfColumns.title});
            columns.push(kendoGridColumn);
        }
        return columns;
    }

    perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
        throw new Error("Method not implemented.");
    }
}
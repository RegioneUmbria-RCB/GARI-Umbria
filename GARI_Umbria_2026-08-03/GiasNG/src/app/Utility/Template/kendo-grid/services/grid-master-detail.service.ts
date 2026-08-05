import {Inject, Injectable, Optional} from '@angular/core';
import {KendoGridService, GridDataWithFilter} from 'gias-kendo-grid';
import {cloneDeep} from "lodash";
@Injectable()
export class KendoGridMasterDetailService {

    /*
    *Memorizzo il KendoGridService (private service della griglia) perchè li ho a disposizione il PublicService e il Servizio che ha creato la griglia
    * dove ci sono le funzioni di read e perform
    * */

    private _GridMasterService: KendoGridService = null;

    private _Array_GridDetail: Array<GridDetailInfo> = [];

    private _Last_RowExpanded: any = null;

    private _RowIdGridMaster: string = "";

    public get GridMasterService(){
        return cloneDeep(this._GridMasterService);
    }

    public set GridMasterService(service:KendoGridService ){
        this._GridMasterService = service;
    }

    public set GridDetailService(obj:GridDetailInfo){

        if(this._Last_RowExpanded && this._GridMasterService){

            if(!obj.GridMasterRowId)
                obj.GridMasterRowId = this._Last_RowExpanded[this._GridMasterService.conf.gridId];

            let index = this._Array_GridDetail.findIndex(d=>d.GridMasterRowId ===  obj.GridMasterRowId);

            if(index > -1)
                this._Array_GridDetail.splice(index,1);
        }

        this._Array_GridDetail.push(obj);
    }

    public get Last_RowExpanded(){
        return cloneDeep(this._Last_RowExpanded);
    }

    public set Last_RowExpanded(row:any ){
        this._Last_RowExpanded = row;
    }

    public get GridDataMaster(): GridDataWithFilter{
        return cloneDeep(this._GridMasterService?.pubService?.getValue());
    }

    public get GridDataDetail(): Array<GridDataDetail>{

        let array_GridDataDetail: Array<GridDataDetail> = [];

        this._Array_GridDetail.forEach(obj=>{
            array_GridDataDetail.push({
                GridMasterRowId: obj.GridMasterRowId,
                GridData: obj.Service?.pubService?.getValue()
            })
        });

        return array_GridDataDetail;
    }

    public get_GridDetailService(GridMasterRowId:any): KendoGridService{
        return cloneDeep(this._Array_GridDetail.find(x=>x.GridMasterRowId === GridMasterRowId).Service);
    }

    public get RowIdGridMaster(){
        return this._RowIdGridMaster;
    }

    public set RowIdGridMaster(RowId: string){
        this._RowIdGridMaster = RowId;
    }
}

export class GridDetailInfo{
    GridMasterRowId: any;

    Service: KendoGridService;

    constructor(GridMasterRowId: any, Service: KendoGridService) {
        this.GridMasterRowId = GridMasterRowId;

        this.Service = Service;
    }
}

export class GridDataDetail{
    GridMasterRowId: any;

    GridData: GridDataWithFilter;

    constructor(GridMasterRowId: any, GridData: GridDataWithFilter) {
        this.GridMasterRowId = GridMasterRowId;

        this.GridData = GridData;
    }
}

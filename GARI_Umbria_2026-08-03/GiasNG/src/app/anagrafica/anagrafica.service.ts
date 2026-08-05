import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CompositeFilterDescriptor, FilterDescriptor } from '@progress/kendo-data-query';
import { AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { isNullOrUndefined } from 'app/Service/utils';
import { GridPublicService } from 'gias-kendo-grid';
import { TreeResolverInput } from 'app/Utility/Template/kendo-tree/model';
import { TreeContainerService } from 'app/Utility/Template/kendo-tree/services/tree-container.service';
import { SessionStorageService } from 'ngx-webstorage';
import { BehaviorSubject, Subject } from 'rxjs';
import { AnagraficaRoutes } from './anagrafica-routes';
import {ObjParametriAgendaService} from "../Service/obj-parametri-agenda.service";
import {ConversionService} from "../Service/conversion.service";

export class FilterData{
  data: Date;
  filter: boolean;
}

export const Storage_Anagrafica_FilterData = 'Anagrafica.FilterData';

@Injectable({providedIn:'root'})
export class AnagraficaService {
  [key: string]: any;
  filterData = new BehaviorSubject<FilterData>({
    data: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()),
    filter: true
  });

  publicServiceSubject: Subject<GridPublicService> = new Subject();
  pubSrviceCtx: GridPublicService;
  signal: Subject<void>;

  constructor(private router: Router,
              private treeCService: TreeContainerService,
              private objParametriAgendaService: ObjParametriAgendaService,
              private conversionService: ConversionService,
              private sessionSt: SessionStorageService) {

    if (this.sessionSt.retrieve(Storage_Anagrafica_FilterData) != null) {
      let filters = this.sessionSt.retrieve(Storage_Anagrafica_FilterData);
      if (this.conversionService.convertStringToDate(filters.data) != AGRODATAINIZIO){
        filters.filter = true;
      }
      let objPData = this.objParametriAgendaService.getObjParamValue();
      objPData.Data = this.sessionSt.retrieve(Storage_Anagrafica_FilterData).data;
      this.objParametriAgendaService.changeObjParametriAgenda(objPData);
      this.filterData.next(filters);
    }

    this.filterData.subscribe((val) => {
      this.sessionSt.store(Storage_Anagrafica_FilterData, val);
      if (this.pubSrviceCtx){
        this.pubSrviceCtx.refresh(true);
      }
    });
    this.publicServiceSubject.subscribe((val) => {
      this.pubSrviceCtx = val;
    })
  }

  applicaFiltri() {
    // const filtri = this.filterData.getValue();
    //
    // if (!this.pubSrviceCtx || !this.pubSrviceCtx?.gridComp || !this.pubSrviceCtx?.gridComp?.filter) {
    //     return;
    // }
    //
    // const actualFilter = this.leggiFiltriData(this.pubSrviceCtx.gridComp.filter)
    //
    // if (actualFilter.filter == filtri.filter && actualFilter.data == filtri.data) {
    //     return;
    // }
    //
    // let modifiedSomething = false;
    // let findIndex = true;
    //
    // while (findIndex) {
    //     const index = this.pubSrviceCtx.gridComp.filter.filters.findIndex((val: CompositeFilterDescriptor) => {
    //         if(isNullOrUndefined(val.filters)) {
    //             return false;
    //         }
    //
    //         if (val.filters.findIndex((cVal: any) => cVal.field == 'Validita_Inizio' && cVal.filterData == true) >= 0) {
    //             return true;
    //         }
    //         return false;
    //     });
    //     if (index >= 0) {
    //         this.pubSrviceCtx.gridComp.filter.filters.splice(index, 1);
    //         modifiedSomething = true;
    //     } else {
    //         findIndex = false;
    //     }
    // }
    //
    // findIndex = true;
    // while (findIndex) {
    //     const index = this.pubSrviceCtx.gridComp.filter.filters.findIndex((val: CompositeFilterDescriptor) => {
    //         if (val.filters.findIndex((cVal: any) => cVal.field == 'Validita_Fine' && cVal.filterData == true) >= 0) {
    //             return true;
    //         }
    //         return false;
    //     });
    //
    //     if (index >= 0) {
    //         this.pubSrviceCtx.gridComp.filter.filters.splice(index, 1);
    //         modifiedSomething = true;
    //     } else {
    //         findIndex = false;
    //     }
    // }
    //
    //
    //
    // if (filtri.filter) {
    //     this.pubSrviceCtx?.gridComp?.filter?.filters.push(<any>{
    //         filters: [{
    //             field: 'Validita_Inizio',
    //             operator: 'lte',
    //             value: filtri.data,
    //             filterData: true
    //         }],
    //         logic: 'and'
    //     })
    //
    //     this.pubSrviceCtx?.gridComp?.filter?.filters.push(<any>{
    //         filters: [{
    //             field: 'Validita_Fine',
    //             operator: 'gte',
    //             value: filtri.data,
    //             filterData: true
    //         }],
    //         logic: 'and'
    //     })
    //     modifiedSomething = true;
    // }
    // if (modifiedSomething) {
    //     this.pubSrviceCtx.refresh(false);
    // }

  }

  leggiFiltriData(filter: CompositeFilterDescriptor): FilterData{
    let FiltroAttuale: FilterData = {
      data: AGRODATAINIZIO,
      filter: false
    }
    let validita_inizio_filter;
    <FilterDescriptor>filter.filters.find((cVal: CompositeFilterDescriptor) => {
      if(isNullOrUndefined(cVal.filters)) {
        return false;
      }

      cVal.filters.forEach((cVal: any) => {
        if (cVal.field == 'Validita_Inizio' && cVal.operator == 'lte' && cVal.filterData == true) {
          validita_inizio_filter = cVal;
        }
      });
    });
    let validita_fine_filter;
    <FilterDescriptor>filter.filters.find((cVal: CompositeFilterDescriptor) => {
      if(isNullOrUndefined(cVal.filters)) {
        return false;
      }

      const val_fine_filter = cVal.filters.forEach((cVal: any) =>{
        if (cVal.field == 'Validita_Fine' && cVal.operator == 'gte' && cVal.filterData == true) {
          validita_fine_filter = cVal;
        }
      });
    });

    if (validita_inizio_filter != undefined &&
      validita_fine_filter != undefined &&
      validita_inizio_filter?.value == validita_fine_filter?.value) {
      FiltroAttuale.filter = true;
      FiltroAttuale.data = validita_inizio_filter?.value;
    }

    return FiltroAttuale;
  }

  navigateTo(target: AnagraficaRoutes, qparams?: TreeResolverInput) {
    this.treeCService.expander.next(false);
    this.router.navigate([...target.link.split('/')], { queryParams: qparams });
  }
}

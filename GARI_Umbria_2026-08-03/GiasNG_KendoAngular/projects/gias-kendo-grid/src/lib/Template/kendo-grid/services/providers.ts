import { forwardRef, Optional, Provider } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GridParentComponent, GRID_HTTP_TOKEN } from '../models/grid.model';
import { GridErrorService } from './grid-log.service';
import { GridPublicService } from './grid-public.service';
import { KendoGridService } from './kendo-grid.service';
import { IQueryParamsService } from './utilities';
import { KendoGridMasterDetailService } from './grid-master-detail.service';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';

export const gridPubServiceFactory = (function () {
    let index = 0;

    return (resolver: IQueryParamsService[]) => {
        // console.log(index);

        const pubs = new GridPublicService(resolver);
        pubs.multiIndex = index;
        index++;
        return pubs;
    };
})();


export function generateGridProviders(configService: any, gridParent: any, gridMasterDetail: any = null): Provider[] {

    let gridConfigs: Provider[] = [];
    let pubServices: Provider[] = [];
    let privServices: Provider[] = [];
    let gridMasterServices: Provider[] = [];

    if (Array.isArray(configService)) {
        gridConfigs = getGridConfigs(configService);
        pubServices = getPubServices(configService.length);
        privServices = getPrivateServices(configService.length);
    } else {
        gridConfigs.push({
            provide: GRID_HTTP_TOKEN,
            useClass: configService
        });
        pubServices.push({
            provide: GridPublicService,
        });
        privServices.push({
            provide: KendoGridService,
        });
    }

    if (gridMasterDetail) {

        gridMasterServices.push({
            provide: KendoGridMasterDetailService
        });

    }

    const fatherComponent = {
        provide: GridParentComponent,
        useExisting: forwardRef(() => gridParent)
    };

    return [
        ...gridConfigs,
        fatherComponent,
        ...privServices,
        ...pubServices,
        ...gridMasterServices,
        GridErrorService,
        { provide: LOADING_TOKEN, useClass: LoadingService },
    ];
}

function getGridConfigs(configService: any): Provider[] {
    const result: Provider[] = [];

    configService.forEach((config) => {
        result.push({
            provide: GRID_HTTP_TOKEN,
            useClass: config,
            multi: true
        });
    });

    return result;
}


function getPrivateServices(count: number): Provider[] {
    const result: Provider[] = [];
    for (let i = 0; i < count; ++i) {
        result.push({
            provide: KendoGridService,
            multi: true
        });
    }
    return result;
}


function getPubServices(num: number): Provider[] {
    const result: Provider[] = [];
    for (let i = 0; i < num; i++) {
        result.push({
            provide: GridPublicService,
            useFactory: gridPubServiceFactory,
            multi: true,
            deps: [[new Optional()], ActivatedRoute]
        });
    }
    return result;
}

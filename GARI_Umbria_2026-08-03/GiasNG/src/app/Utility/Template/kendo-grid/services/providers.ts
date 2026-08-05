import { forwardRef, Injector, Optional, Provider } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AnagraficaService } from 'app/anagrafica/anagrafica.service';
import { QryParamsResolver } from 'gias-kendo-grid';
import { BreadcrumbsService } from '../../breadcrumbs/breadcrumbs.service';
import { TreeService } from '../../kendo-tree/services/tree-anagrafica.service';
import { TreeGisService } from '../../kendo-tree/services/tree-gis.service';
import { GRID_HTTP_TOKEN, GridPublicService, KendoGridService, GridParentComponent, GridErrorService, gridPubServiceFactory } from 'gias-kendo-grid';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';


export function generateGridProvidersAnagrafica(configService: any, gridParent: any): Provider[] {

    let gridConfigs: Provider[] = [];
    let pubServices: Provider[] = [];
    let privServices: Provider[] = [];

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

    const fatherComponent = {
        provide: GridParentComponent,
        useExisting: forwardRef(() => gridParent)
    };

    return [
        ...gridConfigs,
        provideAnagraficaPubService(),
        fatherComponent,
        ...privServices,
        GridErrorService,
        { provide: LOADING_TOKEN, useClass: LoadingService },
    ];
}

function provideAnagraficaPubService() {
    return {
        provide: GridPublicService,
        useFactory: anagraficaPubServiceFactory(null),
        deps: [Injector, AnagraficaService]
    };
}

export function anagraficaPubServiceFactory(closureVariable: number) {
    return (injector: Injector, anagraficaService: AnagraficaService): GridPublicService => {
        const qryParams: any = injector.get(QryParamsResolver);
        let publicService = new GridPublicService(qryParams);

        anagraficaService.publicServiceSubject.next(publicService);

        return publicService;
    };
};

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
            deps: [[new Optional(), TreeService, TreeGisService, BreadcrumbsService], ActivatedRoute]
        });
    }
    return result;
}

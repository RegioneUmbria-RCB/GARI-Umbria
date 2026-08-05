import { InjectionToken, Injector, Provider } from '@angular/core';
import { AnagraficaService } from 'app/anagrafica/anagrafica.service';
import { TreeGridService } from 'app/anagrafica/services/TreeGrid.service';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { BreadcrumbsService } from '../../breadcrumbs/breadcrumbs.service';
import { IQueryParamsService, QryParamsResolver } from 'gias-kendo-grid';
import { TreeService } from '../services/tree-anagrafica.service';
import { TreeGisService } from '../services/tree-gis.service';

export const is = (fileName: string, ext: string) =>
    new RegExp(`.${ext}\$`).test(fileName);

export const Tree_Token = new InjectionToken<IQueryParamsService>('app.tree');

export function ProvideAnagraficaTreeDeps(): Provider[] {

    return [
        TreeService,
        // TreeGisService,
        // TreeContainerService,
        AnagraficaService,
        {
            provide: QryParamsResolver,
            useClass: TreeGridService,
            multi: true
        },
        {
            provide: TreeGridService,
            useFactory: ProvideTreeGridService(),
            deps: [Injector]
        },
        {
            provide: QryParamsResolver,
            useClass: BreadcrumbsService,
            multi: true
        },
        {
            provide: BreadcrumbsService,
            useFactory: ProvideBreadcrumbsService(),
            deps: [Injector]
        },
        {
            provide: LOADING_TOKEN,
            useClass: LoadingService
        },
    ];
}

function ProvideTreeGridService() {
    return (injector: Injector) => {
        const gridSerivce = injector.get(QryParamsResolver);
        return gridSerivce[0];
    }
}

function ProvideBreadcrumbsService() {
    return (injector: Injector) => {
        const gridSerivce = injector.get(QryParamsResolver);
        return gridSerivce[1];
    }
}


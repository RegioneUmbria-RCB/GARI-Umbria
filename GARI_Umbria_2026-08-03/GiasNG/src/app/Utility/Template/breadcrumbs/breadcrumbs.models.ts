/* eslint no-shadow: 0 */
import { BreadCrumbItem } from '@progress/kendo-angular-navigation';
import { FilterDescriptor } from '@progress/kendo-data-query';
import { AnagraficaLevels } from 'app/anagrafica/anagrafica-routes';
import { isNullOrUndefined } from 'app/Service/utils';


/** These levels corrispond to the item's type inside the chain
 *  of breadcrumbs.
 */
 export enum BreadcrumbLevels {
    centro = 0,
    campo = 1
}

export class BreadCrumb {
    item: BreadCrumbItem;
    level: AnagraficaLevels;
    breadcrumbLevel: BreadcrumbLevels;
    treeElemPendingSelection: string;
    gridRowId: string;
}

export class FilterBreadCrumb {
    items: BreadCrumb[];
    Piva: string;
}


export class BreadCrumbsSettings {
    // Che viene aggiunto nell'elenco breadcrumb
    displayedItems: BreadCrumb[];
}

export const BreadCrumbCookieKey = "AnagraficaNG.FilterSelected"
function intersaction(arr1: any[], arr2: any[]) {
    return arr1.filter(value => arr2.includes(value));
}

export function removeNaNValues(filters: FilterDescriptor[]) {
    return filters.filter(
        (filter) => !Number.isNaN(filter?.value));
}

export function removeNullValues(filters: any[]) {
    return filters.filter(
        (value) => !isNullOrUndefined(value));
}

export function removeNullAndNaNValues(filters: any[]): any[] {
    return intersaction(
        removeNullValues(filters),
        removeNaNValues(filters)
    );
}

export function addFilter(field: string, value: string | number): FilterDescriptor {
    if(isNullOrUndefined(value)) {
        return null;
    }

    return {
        field: field,
        operator: 'eq',
        value: value
    };
}

import { InjectionToken, QueryList } from '@angular/core';
import { ColumnBase } from '@progress/kendo-angular-grid';
import { FilterDescriptor } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import {
    KendoGridColumn,
    KendoGridModel,
    KendoGridRow,
    SelectedOpts
} from '../models/grid.model';
import { CELL_TYPES } from 'gias-ui-kit';

export const QryParamsResolver = new InjectionToken<IQueryParamsService>('Interface for handling http operations on the grid public service');


export interface IQueryParamsService {
    init(signal: Subject<void>);
    execute(params: any);
}


export function mapDateFilter(descriptor: any, model: KendoGridModel, columns: KendoGridColumn[]) {
    const filters = descriptor?.filters || [];

    filters.forEach((filter) => {
        if (filter.filters) {
            mapDateFilter(filter, model, columns);
        } else if ((model[filter.field].type === CELL_TYPES.DATE || model[filter.field].type === CELL_TYPES.DATETIME) &&
            findColumn(columns, filter) != null && filter.value &&
            !(filter.value instanceof Date)) {
            filter.value = new Date(filter.value);
        }
    });
};

function findColumn(columns: KendoGridColumn[], filter: FilterDescriptor): KendoGridColumn {
    return columns.find(c => c.field === filter.field);
}

export const getCircularReplacer = () => {
    const seen = new WeakSet();
    return (key, value) => {
        if (typeof value === 'object' && value !== null) {
            if (seen.has(value)) {
                return;
            }
            seen.add(value);
        }
        return value;
    };
};

export function scrollToFirstColumn(parentName: string) {
    const pattern = '#' + parentName + ' th:first-child';
    document.querySelectorAll(pattern).forEach(item => item.scrollIntoView());
}

export function columnAlreadyInView(
    inCols: QueryList<ColumnBase>, curCols: QueryList<ColumnBase>) {
    return inCols.some((inCol: any) => curCols.some(cur => cur === inCol));
}


export function findMatchedIndex(key: string | number, rows: KendoGridRow[], rowId: string, opts: SelectedOpts) {
    let index;
    if (opts.usePartialMatch)
        index = rows.findIndex(s => s[rowId].includes(key));
    else
        index = rows.findIndex(s => s[rowId] === key);
    return index;
}

export function resetSelection(rows: KendoGridRow[]) {
    rows?.forEach(s => s['Selected'] = false);
}


export function _isNull(value) {
    return value == null || Number.isNaN(value);
}

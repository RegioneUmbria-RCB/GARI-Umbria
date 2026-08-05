
import { ElementRef } from '@angular/core';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { DropdownListItem } from '../../models/grid.model';

export abstract class MulticheckInput {
    textField: string;
    valueField: string;
    currentFilter: CompositeFilterDescriptor;
    allRows: any[];
    store: { [key: string]: string[] };
    field: string;
    shutdownPagination: Subject<void>;
    ordering: (rows: any) => any | null;
}

export enum WorkType {
    String = 0,
    Dropdown = 1
}

export const operators: DropdownListItem[] = [
    { id: 'eq', name: 'Equal to' },
    { id: 'neq', name: 'Not equal to' },
    { id: 'isnull', name: 'Is equal to null' },
    { id: 'isnotnull', name: 'Is not equal to null' },
    { id: 'lt', name: 'Less than' },
    { id: 'lte', name: 'Less than or equal to' },
    { id: 'gt', name: 'Greater than' },
    { id: 'gte', name: 'Greater than or equal to' }
]

export const logicalOperators: DropdownListItem[] = [
    { id: 'and', name: 'And' },
    { id: 'or', name: 'Or' }
];

export class PopupSettingsManager {
    public settings: any = {
        popupClass: 'timepicker-filter'
    };
    public filterContainer: any;

    initializeFilterContainer(element: ElementRef<any>) {
        this.filterContainer = closest(element.nativeElement,
            node => (String(node.className).indexOf('k-filter-menu-container') >= 0));
    }

}

export const closest = (node: any, predicate: any): any => {
    while (node && !predicate(node)) {
        node = node.parentNode;
    }

    return node;
};

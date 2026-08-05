import { Injectable } from '@angular/core';
import { NextDropdownValue } from 'gias-kendo-grid';
import { Subject } from 'rxjs';

@Injectable({providedIn: 'root'})
export class GridRootHelper {
    public nextDropdownValue: Subject<NextDropdownValue> = new Subject();
    // TODO Razvan. Da inoltrare il valore al Subject dalla griglia
    public nextMultiDropdownValue: Subject<NextDropdownValue> = new Subject();

}

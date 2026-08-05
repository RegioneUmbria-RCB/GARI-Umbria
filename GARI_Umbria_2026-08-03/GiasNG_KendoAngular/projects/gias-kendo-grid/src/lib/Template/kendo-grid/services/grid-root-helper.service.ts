import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { NextDropdownValue } from '../components/grid-dropdownlists/grid-dropdown.service';

@Injectable({ providedIn: 'root' })
export class GridRootHelper {
    public nextDropdownValue: Subject<NextDropdownValue> = new Subject();
    // TODO Razvan. Da inoltrare il valore al Subject dalla griglia
    public nextMultiDropdownValue: Subject<NextDropdownValue> = new Subject();

}

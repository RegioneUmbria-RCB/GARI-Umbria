import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BehaviorSubject, Observable } from 'rxjs';
import { DropdownList, DropdownListItem, KendoGridColumn, KendoGridRow } from '../../models/grid.model';
import { InMemoryView } from '../grid-customizations/model';

export interface NextDropdownValue {
  controlName: string;
  value: string;
  loadData: (dataItem: any) => Observable<any[]>;
  formGroup: FormGroup;
  loadOnEdit: boolean;
  currentRow: KendoGridRow;
  column: KendoGridColumn;
}


@Injectable()
export class GridDropdownService {
  readonly defaultItem = new DropdownListItem('-1', 'Predefinita');


  public sourceChange: BehaviorSubject<DropdownList> = new BehaviorSubject(null);

  public extractDropdownItems(data: InMemoryView[]): DropdownListItem[] {
    const items: DropdownListItem[] = [];
    data.forEach((view: InMemoryView) => {
      items.push(new DropdownListItem(view.IdVista, view.NomeVista));
    });
    return items;
  }

  public getDefaultItem(): DropdownListItem {
    return this.defaultItem;
  }

  public getDefaultItemId(): string {
    return this.defaultItem.id;
  }

  public generateUID() {
    //return Math.random().toString(16).slice(2);
    return window.crypto.getRandomValues(new Uint32Array(1))[0].toString(16)
  }
}


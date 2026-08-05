import { Injectable } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { PropertyValidator } from '../../../../shared/ValidateProperty';
import { lastValueFrom } from 'rxjs';
import { DropdownListItem, KendoGridColumn, KendoGridRow } from '../../models/grid.model';
import { GridPublicService } from '../../services/grid-public.service';
import { InMemoryView } from '../grid-customizations/model';
import { NextDropdownValue } from '../grid-dropdownlists/grid-dropdown.service';

export class DropdownHelper {
  gridCellClosedOuput(col: KendoGridColumn, row: KendoGridRow): { items: DropdownListItem[], columnDataNeedsUpdate: boolean } {
    let ddlItems: DropdownListItem[] = [];

    if (this.datiSonoStatiCaricatiInAnticipo(col)) {
      let ctrlName = col.ddl.formControlName;
      if (!Array.isArray(row[ctrlName]))
        throw Error(`Value for ${ctrlName} must be an array!`);

      if (col.ddl.valuePrimitive) {
        let values = row[ctrlName];
        ddlItems = col.ddl.data.filter(x => values.includes(x['id']));
      }
      else {
        let values: any[] = row[ctrlName];

        if (this.containsDropdownListItems(values))
          values = values.map(s => s['id']);

        ddlItems = col.ddl.data.filter(x => (values).includes(x['id']));
      }

      return { items: ddlItems, columnDataNeedsUpdate: false };
    } else {
      const { computedDdlItems, dataAlreadyLoaded } = this.getCellClosedOutputFromDataItem(row, col, ddlItems);
      return { items: computedDdlItems, columnDataNeedsUpdate: dataAlreadyLoaded };
    }
  }

  private containsDropdownListItems(values: any[]) {
    return values?.length > 0 && values[0].id != null;
  }

  private datiSonoStatiCaricatiInAnticipo(col: KendoGridColumn) {
    return !col.ddl.loadOnEdit;
  }

  private getCellClosedOutputFromDataItem(row: KendoGridRow, col: KendoGridColumn, currentDdlItems: DropdownListItem[]) {
    let descriptions = row[col.ddl.descriptionField];
    let codes = row[col.ddl.formControlName];

    if (!Array.isArray(descriptions))
      throw Error(`Value for ${col.ddl.descriptionField} must be an array`)

    let computedDdlItems: DropdownListItem[] = currentDdlItems;
    // let dataAlreadyLoaded = true;
    // if (col.ddl.data == null || col.ddl.data.length == 0) {
    codes.forEach((code, index) => {
      let ddlItem = new DropdownListItem(code, descriptions[index]);
      computedDdlItems.push(ddlItem);
    });

    let dataAlreadyLoaded = false;
    // }
    return { computedDdlItems, dataAlreadyLoaded };
  }

  getMultiDropdownCtrlInput(c: KendoGridColumn, row: KendoGridRow) {
    if (!c.ddl?.valuePrimitive) {
      let ctrlInput = [];
      let codici = row?.[c.ddl.formControlName] || c.ddl?.defaultValue || [];

      if (codici.length > 0 && codici[0] instanceof DropdownListItem)
        return codici;

      let valori = row?.[c.ddl.formControlValue] || c.ddl?.defaultValue || [];

      if (Array.isArray(codici)) {
        let ctrlName = c.ddl?.formControlName;
        let ctrlValue = c.ddl?.formControlValue;
        codici.forEach((codice, index) => {
          let input = {};
          input[ctrlName] = codice;
          input[ctrlValue] = valori[index];
          ctrlInput.push(input);
        })
      }
      else {
        throw Error(`Value for ${c.ddl.formControlName} must be an array!`);
      }
      c.validators?.forEach(s => {
        if (s instanceof PropertyValidator) {
          s.setCurrentRow(row);
        }
      });

      return ctrlInput;
    } else {
      return row?.[c.ddl.formControlName] || c.ddl?.defaultValue || [];
    }
  }
}

@Injectable()
export class GridDropdownBase {

  data: any[];
  source: any[];

  constructor(private gridPublicService: GridPublicService) {

  }

  readonly defaultItem = new DropdownListItem('-1', 'Predefinita');

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

  getData() {
    return this.data;
  }

  getSource() {
    return this.source;
  }

  public patchValue(data: NextDropdownValue) {
    let formGrp = data.formGroup;
    let ctrlName = data.controlName;
    let value = data.value;
    let col = data.column;

    let ctrl = formGrp.controls[ctrlName];
    let exists = this.checkIfValueExists(value);

    if (exists)
      ctrl.patchValue(value);
    else {
      lastValueFrom(data.loadData(this.gridPublicService.currentDataItem))
        .then(data => {
          this._patchValue(ctrl, value, data, col);
        });
    }
  }

  private _patchValue(ctrl: AbstractControl, value: string, data: any[], column: KendoGridColumn) {
    this.source = data;
    this.data = data;

    let exists2 = this.checkIfValueExists(value);
    if (exists2) {
      column.ddl.data = this.data;
      ctrl.patchValue(value);
    }
    else {
      console.log('You are trying to patch a non existing value');
      if (this.data.length > 0) {
        let item = this.data[0];
        if (!item.hasOwnProperty('id') || !item.hasOwnProperty('name')) {
          console.log('Read data does not have required fields: "id" and "name"');
        }

      }
    }
  }

  checkIfValueExists(itemId: string): boolean {
    let exists = false;

    let index = this.data.findIndex(s => s.id == itemId);

    if (index >= 0)
      exists = true;

    return exists;
  }
}


@Injectable()
export class GridMultiDropdownService {
  readonly defaultItem = new DropdownListItem('-1', 'Predefinita');


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


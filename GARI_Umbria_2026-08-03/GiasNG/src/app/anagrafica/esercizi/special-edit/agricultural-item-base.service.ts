import {ReplaySubject} from 'rxjs';
import {KendoGridRow} from 'gias-kendo-grid';
import {AgriculturalItem} from './agricultural-item.model';

export abstract class AgriculturalItemBaseService {
  listViewRows$: ReplaySubject<AgriculturalItem[]> = new ReplaySubject<AgriculturalItem[]>(1);
  loading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  set checkedRows(rows: KendoGridRow[]) {
    this._checkedRows = rows;
  }

  protected _checkedRows: KendoGridRow[] = [];

  protected abstract setListViewRows(): void;

  protected abstract mapToListViewItems(row: object): AgriculturalItem;

  protected abstract extractDescription(r: KendoGridRow): string;
}

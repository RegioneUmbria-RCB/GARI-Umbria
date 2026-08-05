import {Subject} from 'rxjs';
import {DropdownListWithForm, KendoGridColumn} from 'gias-kendo-grid';

export function handleDdlConfig(
  gridColumns: Readonly<KendoGridColumn[]>,
  formControlName: Readonly<string>,
  formControlValue: Readonly<string>,
  id: Readonly<string>,
  descriptionField: Readonly<string>,
  loadFunction: (dataItem: any) => Subject<any[]>,
  data: any[] = [],
  loadOnEdit: boolean = true
): void {
  let col: KendoGridColumn = gridColumns.find(s => s.field === formControlName);
  col.ddl = new DropdownListWithForm(id, formControlName, formControlValue, data);
  col.ddl.loadOnEdit = loadOnEdit;
  col.ddl.descriptionField = descriptionField;
  col.ddl.loadFunction = loadFunction;
}

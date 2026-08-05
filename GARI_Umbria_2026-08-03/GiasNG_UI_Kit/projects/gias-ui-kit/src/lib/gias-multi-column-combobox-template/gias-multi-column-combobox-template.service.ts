import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { CELL_TYPES } from '../utils/models';

@Injectable()
export class MultiColumnComboboxService {
  private MultiColumnComboboxValueObject = new MultiColumnComboboxFormItem;
  private MultiColumnComboboxValueObjectSource = new BehaviorSubject(this.MultiColumnComboboxValueObject);

  currentMultiColumnComboboxValueObject: Observable<MultiColumnComboboxFormItem> = this.MultiColumnComboboxValueObjectSource.asObservable();

  setMultiColumnComboboxValue(MultiColumnComboboxValueObject: MultiColumnComboboxFormItem) {
    this.MultiColumnComboboxValueObjectSource.next(MultiColumnComboboxValueObject);
  }
}

export class MultiColumnComboboxFormItem {
  FormControlName: string;
  Value: any;
}

export class ColumnCombobox {
  field: string;
  title: string;
  /**
   * @description
   * Formatta la colonna numero con la lingua del LOCALE_ID.
   * Se formatNumbertolocal è true allora la colonna la proprietà type deve essere CELL_TYPES.number
   */
  formatNumbertolocal?: boolean;
  digitsInfo: string;
  type?: CELL_TYPES;
  hidden?: boolean;
  media?: string;
  width?: number;
  isArray?: boolean;
  Arrayfield?: string;
  Arrayseparator?: string;
  ColumnComboBoxColumnCellTemplate?: boolean;
  style?: { [key: string]: string };
  class?: string;

  constructor(required: { field: string; title: string; }, optional?: Partial<ColumnCombobox>) {
    this.field = required.field;
    this.title = required.title;
    this.formatNumbertolocal = optional?.formatNumbertolocal ?? false;
    this.digitsInfo = optional?.digitsInfo ?? undefined;
    this.type = optional?.type ?? CELL_TYPES.STRING;
    this.hidden = optional?.hidden ?? false;
    this.media = optional?.media || "";
    this.width = optional?.width || null;
    this.isArray = optional?.isArray ?? false;
    this.Arrayfield = optional?.Arrayfield || "";
    this.Arrayseparator = optional?.Arrayseparator || " ,\n";
    this.ColumnComboBoxColumnCellTemplate = optional?.ColumnComboBoxColumnCellTemplate ?? false;
    this.style = optional?.style ?? null;
    this.class = optional?.class ?? null;
  }
}

export class MultiColumnComboboxGiasTemplate {
  Codice_Concatenato: string;
  Descrizione_Concatenata: string;
}

import {FormControl} from "@angular/forms";
import {enum_TipoPrescrizione} from "./tipo-prescrizione.enum";
import { CodeType } from '../../Model/attivita/centri_di_costo/CentroDiCosto';

export class ZooOperationsFilters {
  public forceReload = false;

  constructor(
    public center: number,
    public stable: number,
    public from: Date,
    public to: Date,
    public operations: string[]
  ) { }

  static fromFormGroup(form: ZooOperationsFiltersForm, defaultFromDate?: Date, defaultToDate?: Date) {
    let obj = new ZooOperationsFilters(0,0,new Date(),new Date(),[]);
    obj.center = form.center.value as number;
    obj.stable = form.stable.value['codice'] as number ?? 0;
    obj.from = form.from.value as Date ?? defaultFromDate;
    obj.to = form.to.value as Date ?? defaultToDate;
    obj.operations = form.operations.value as string[];
    return obj;
  }
}

export class ZooPrescriptionsFilters extends ZooOperationsFilters {
  public prescriptionType: enum_TipoPrescrizione | null;
}

export interface ZooOperationsFiltersForm {
  center: FormControl<number>;
  stable: FormControl<any>;
  from: FormControl<Date>;
  to: FormControl<Date>;
  operations: FormControl<string[]>;
}

export interface ZooBDNform {
  center: FormControl<number>;
  stable: FormControl<number>;
}
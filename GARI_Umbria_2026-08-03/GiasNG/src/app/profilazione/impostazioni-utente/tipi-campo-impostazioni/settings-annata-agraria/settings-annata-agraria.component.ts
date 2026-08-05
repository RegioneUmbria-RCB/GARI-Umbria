import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import {FormatSettings} from '@progress/kendo-angular-dateinputs';
import {AGRODATAFINE, AGRODATAINIZIO} from "../../../../Model/CostantiPersonalizzate";

function formatNumberStr(n: number): string {
  return n < 10 ? '0' + n : n.toString();
}

@Component({
  standalone: false,
  selector: 'app-settings-annata-agraria',
  templateUrl: './settings-annata-agraria.component.html',
  styleUrls: ['./settings-annata-agraria.component.css']
})
export class SettingsAnnataAgrariaComponent implements OnInit {

  @Input('formGroup') form: FormGroup;
  public internalForm: FormGroup;

  public readonly format: FormatSettings = {
    displayFormat: 'dd/MM',
    inputFormat: 'dd/MM',
  };

  constructor() {
    this.initInternalForm();
  }

  ngOnInit(): void {
    if (this.isValid()) {
      this.internalForm.get('dataInizio').patchValue(this.dataInizioStr2Date());
      this.internalForm.get('dataFine').patchValue(this.dataFineStr2Date());
    } else (
      this.updateValue(this.internalForm.value)
    );
    this.handleChange();
  }

  private initInternalForm() {
    this.internalForm = new FormGroup({
      'dataInizio': new FormControl(AGRODATAINIZIO),
      'dataFine': new FormControl(AGRODATAFINE)
    });
  }

  private dataFineStr2Date(): Date {
    const currentValue = this.form.value.valoreCorrente;
    const str = currentValue.slice(-4);
    const month = +str.slice(2) - 1;
    const day = +str.replace(str.slice(2), '');
    return new Date(0, month, day);
  }

  private dataInizioStr2Date(): Date {
    const currentValue = this.form.value.valoreCorrente;
    const str = currentValue.slice(0, 4);
    const month = +str.slice(2) - 1;
    const day = +str.replace(str.slice(2), '');
    return new Date(0, month, day);
  }

  /**
   * @returns `true` fi the loaded value is valid, `false` otherwise.
   * @private
   */
  private isValid(): boolean {
    const val: any = this.form.value;
    let current: string;

    if (val.valori.length > 0 && val.valori[0].valore !== val.valoreCorrente) {
      current = val.valori[0].valore;
      this.form.get('valoreCorrente').patchValue(current);
    } else {
      current = val.valoreCorrente;
    }

    return current.length === 8 && !isNaN(+current);
  }

  private handleChange() {
    this.internalForm.valueChanges.subscribe(value => this.updateValue(value));
  }

  private updateValue(value: {dataInizio: Date; dataFine: Date}): void {
    if (value.dataInizio && value.dataFine) {
      this.form?.get('valoreCorrente')?.patchValue(this.parseNewValue(value.dataInizio, value.dataFine));
      this.form.markAsTouched();
    }
  }

  private parseNewValue(dataInizio: Date, dataFine: Date) {
    let newValue = formatNumberStr(dataInizio.getDate());
    newValue += formatNumberStr(dataInizio.getMonth() + 1);
    newValue += formatNumberStr(dataFine.getDate());
    newValue += formatNumberStr(dataFine.getMonth() + 1);
    return newValue;
  }

}

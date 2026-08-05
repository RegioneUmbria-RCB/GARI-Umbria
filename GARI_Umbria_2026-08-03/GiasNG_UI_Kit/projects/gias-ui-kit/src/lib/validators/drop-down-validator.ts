import { AbstractControl, ValidationErrors, Validator } from '@angular/forms';
import { getValueOfPropertyInObject } from '../utils/get-value-of-property-in-object';


export class DropDownValidator implements Validator {

  constructor(private ValuePrimitive: boolean, private valuefield = "codice") { }

  validate(control: AbstractControl): ValidationErrors {
    let error = null;
    let valuefield = this.valuefield;
    let value = control.value;

    if (!value) {
      error = { valid: false };
    } else {

      if (this.ValuePrimitive) {
        if (+ value === 0) {
          error = { valid: false };
        }

      } else {
        value = getValueOfPropertyInObject(value, valuefield);

        if (!value ||
          + value === 0) {
          error = { valid: false };
        }
      }
    }

    return error;
  }
}

import { AbstractControl, ValidationErrors, Validator } from '@angular/forms';
import { UtilityFunctions } from '../UtilityFunctions';


export class MultiSelectValidator implements Validator {

    constructor(private ValuePrimitive: boolean, private valuefield = "codice") { }

    validate(control: AbstractControl): ValidationErrors {

        let error = null;

        let valuefield = this.valuefield;

        let value = control.value;

        if (!value || value.length === 0) {
            error = { valid: false };
        } else {

            if (this.ValuePrimitive) {

                let val = value.find(v => + v === 0);

                if (!val)
                    error = { valid: false };

            } else {

                value = UtilityFunctions.GetListofPropertyinArrayObject(value, valuefield);

                let val = value.find(v => !value || + v === 0);

                if (val)
                    error = { valid: false };

            }
        }

        return error;
    }

}



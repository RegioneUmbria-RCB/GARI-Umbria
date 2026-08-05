import { AbstractControl, ValidationErrors, Validator } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO } from '../shared/CostantiPersonalizzate';


export class DataValidator implements Validator {

    constructor(private MINDATE: Date = AGRODATAINIZIO, private MAXDATE: Date = AGRODATAFINE) { }

    validate(control: AbstractControl): ValidationErrors {

        let error = null;

        if (!control.value) {
            error = { valid: false };
        } else {

            if (this.MINDATE && control.value <= this.MINDATE) {
                error = { valid: false };
            }

            if (this.MAXDATE && control.value >= this.MAXDATE) {
                error = { valid: false };
            }
        }

        return error;

    }

}



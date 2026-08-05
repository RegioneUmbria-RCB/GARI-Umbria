import { AbstractControl, FormControl, ValidationErrors, Validator, ValidatorFn } from "@angular/forms";

// function validateProperty(property: string, validator: ValidatorFn): ValidatorFn 
// {
//     return (control: AbstractControl): {[key: string]: any} | null => {
//         // get the value and assign it to a new form control
//         let utilityFns = new UtilsService();
//         let propertyVal = null;
//         if(control.value != null)
//           propertyVal = utilityFns.getPrimitiveValue(control.value, property);

//         const newFc = new FormControl(propertyVal);
//         // run the validators on the new control and keep the ones that fail
//         let failedValidators = null;
//         failedValidators = validator(newFc);

//         // if any fail, return the list of failures, else valid
//         return failedValidators?.length ? {'invalidProperty': failedValidators} : null;
//     };
// }


// export class FormGroupValidator implements Validator {

//     constructor(private property: string, 
//       private validatorFn?: CustomValidatorFn) {}

//     validate(control: AbstractControl): ValidationErrors {
//         let customValidatorFn = this.validatorFn(control);
       
//         return validateProperty(this.property, customValidatorFn)(control);
//     }
// }

// type CustomValidatorFn = (formCtrl) => ValidatorFn;

export function match(toCompare: string, expected: string): ValidatorFn {
    return (control: AbstractControl) : {[ key: string ]: any } | null => {
        const password = control.get(expected).value;
        const confirm = control.get(toCompare).value;
        if (password !== confirm) {
            return { wrongPassword: confirm};
        }
        return null;
    }
        // control.value === other ? null : { wrongPassword: control.value};
}

/** Cross field validator */
export class FormGroupValidator implements Validator {
    private expected: string;
    private toCompare: string;

    constructor(fieldToCompare: string, expectedField: string) {
        this.toCompare = fieldToCompare;
        this.expected = expectedField;
    }

    matchPassword(control: AbstractControl): ValidationErrors | null {
        const password = control.get(this.expected).value;
        const confirm = control.get(this.toCompare).value;
        if (password !== confirm) {
            return { 'noMatch': true };
        }
        return null;
    }

    validate(control: AbstractControl): { [key: string]: any } | null {
        return match(this.toCompare, this.expected)(control);
    }
}


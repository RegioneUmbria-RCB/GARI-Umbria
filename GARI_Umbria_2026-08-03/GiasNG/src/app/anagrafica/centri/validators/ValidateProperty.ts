import { AbstractControl, FormControl, ValidationErrors, Validator, ValidatorFn } from '@angular/forms';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { KendoGridRow } from 'gias-kendo-grid';


// pass in property to validate and list of validators to run on it
function validateProperty(property: string, validators: ValidatorFn[]): ValidatorFn 
{
    return (control: AbstractControl): {[key: string]: any} | null => {
        // get the value and assign it to a new form control
        let propertyVal = null;
        if(control.value != null)
          propertyVal = UtilityFunctions.getPrimitiveValue(control.value, property);

        const newFc = new FormControl(propertyVal);
        // run the validators on the new control and keep the ones that fail
        let failedValidators = null;
        failedValidators = validators.map(v =>  v(newFc)).filter(v => !!v);


        // if any fail, return the list of failures, else valid
        return failedValidators.length ? {'invalidProperty': failedValidators} : null;
    };
}


export class PropertyValidator implements Validator {

    public row: KendoGridRow;

    // Bisogna estendere il validatore in modo tale di permettere aggiungere più
    // di un validatore.
    private validators: ValidatorFn[] = [];

    constructor(private property: string, 
      private validatorFn?: CustomValidatorFn) {}

    validate(control: AbstractControl): ValidationErrors {

        let customValidator = this.validatorFn(this.row);

        if(customValidator.name !== REQUIRED_VALIDATOR_NAME)
          throw Error(`The name of the validator fn must be ${REQUIRED_VALIDATOR_NAME}`)

        if(!this.validators.some(s => s.name === REQUIRED_VALIDATOR_NAME))
        {
            this.validators.push(customValidator);
        }
        

        return validateProperty(this.property, this.validators)(control);
    }

    public setCurrentRow(row: KendoGridRow) {
      this.row = row;
      this.resetValidators();
    } 
    private resetValidators() {
      this.validators = [];
    }

}

const UNNAMED_FN = '';
type CustomValidatorFn = (formctrl) => ValidatorFn;

export class PropertyValidatorFields {

}

const REQUIRED_VALIDATOR_NAME = 'customValidator';
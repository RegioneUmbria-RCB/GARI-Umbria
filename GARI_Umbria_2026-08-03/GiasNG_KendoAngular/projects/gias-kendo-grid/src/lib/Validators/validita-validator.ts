import { AbstractControl, ValidationErrors, Validator } from '@angular/forms';


export class ValiditaValidator implements Validator {

    validate(control: AbstractControl): ValidationErrors {
        const validita_inizio = control.get('inizio')?.value as Date;
        const validita_fine = control.get('fine')?.value as Date;

        if (validita_inizio > validita_fine) {
            control.get('inizio').setErrors({ 'validita': false });
            control.get('fine').setErrors({ 'validita': false });
            return { 'validita': false }
        }
        control.get('inizio')?.setErrors(null);
        control.get('fine')?.setErrors(null);
        return null;

    }

}



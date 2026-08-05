import {FormGroup, ValidationErrors, Validator} from '@angular/forms';
import {FERTILIZZANTI, FORMULATI, INSETTI} from '../../../Model/CostantiPersonalizzate';

export class GridDosiProdottiValidator implements Validator {

    constructor(private elem_cod:number = 0) { }


    validate(formarray: FormGroup):ValidationErrors {

        let error = null;

        let value = formarray.getRawValue();

        //Per le sementi non è obbligatorio aver selezionato un prodotto perchè se non seleziono nulla sto facendo la semina 'fast'
        //come se fosse una aratura

        if(this.elem_cod === FORMULATI || this.elem_cod === FERTILIZZANTI || this.elem_cod === INSETTI){

            if(!value || value?.Riga_Salvata === false)
                error = { valid: false };
        }

        return error;
    }
}



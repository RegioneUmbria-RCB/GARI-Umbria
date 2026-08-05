import {QdCService} from "../service/qdc.service";
import {FormControl, ValidationErrors, Validator} from "@angular/forms";



export class SupTrattataValidator  implements Validator {

  constructor(private qdcservice: QdCService) {}

  validate(Sup_TrattataForm: FormControl): ValidationErrors {

    let error = null;

    let Sup_Selezionata: number = this.qdcservice.SuperficiForm.get("Sup_Selezionata").getRawValue();

    let Sup_Trattata = Sup_TrattataForm.getRawValue();

    if(Sup_Trattata > Sup_Selezionata){
      error = { valid: false };
    }

    return error;
  }
}

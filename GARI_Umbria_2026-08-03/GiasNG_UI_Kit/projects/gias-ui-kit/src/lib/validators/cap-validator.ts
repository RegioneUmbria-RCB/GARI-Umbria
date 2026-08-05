import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

const maxItalianCAPDigits: number = 5;

export function capValidatorAppezzamenti(): ValidatorFn {
  return capValidator('codice_stato');
}

export function capValidatorCentriGrid(): ValidatorFn {
  return capValidator('Stato_Cod');
}

export function capValidatorAziendeGrid(): ValidatorFn {
  return capValidator('Stato_Cod');
}

export function capValidatorCentriEdit(): ValidatorFn {
  return capValidator('stato');
}

export function capValidatorAziendeEdit(): ValidatorFn {
  return capValidator('stato');
}

function capValidator(controlName: string): ValidatorFn {
  return (cap: AbstractControl): ValidationErrors => {
    const countryCod = cap?.parent?.get(controlName)?.value?.codice ?? cap?.parent?.get(controlName)?.value;
    if (countryCod == 'IT') {
      return cap.value.length > maxItalianCAPDigits ? { lenght: 'excides max lenght for italian CAP' } : null;
    }

    return null;
  }
}

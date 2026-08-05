import { Injectable } from "@angular/core";
import { AbstractControl, ValidationErrors } from "@angular/forms";
import { BehaviorSubject, Observable } from "rxjs";

export class ErrorTree {
  name: string;
  field: string;
  form: AbstractControl;
  errors: ValidationErrors;
}

@Injectable({ providedIn: 'root' })
export class GiasFormErrorVisualizerService {
  private errors = new BehaviorSubject<ErrorTree[]>([]);
  currentErrors: Observable<ErrorTree[]> = this.errors.asObservable();

  public resetFormError() {
    this.errors.next([]);
  }

  public getErrors(): ErrorTree[] {
    return this.errors.getValue();
  }

  private setError(name: string, field: string, form: AbstractControl, errors: ValidationErrors) {
    let index = this.errors.getValue().findIndex((er) => er.field == field);
    let errorsArr: ErrorTree[] = [];
    Object.assign(errorsArr, this.errors.getValue());
    if (index >= 0) {
      errorsArr[index] = { name, field, form, errors }
    } else {
      errorsArr.push({ name, field, form, errors })
    }
    this.errors.next(errorsArr);
  }

  private removeError(name: string, field: string) {
    let index = this.errors.getValue().findIndex((er) => er.field == field && er.name == name);
    let errorsArr: ErrorTree[] = [];
    Object.assign(errorsArr, this.errors.getValue());
    if (index >= 0) {
      let errorDeletedArr = errorsArr.splice(index, 1);
      this.errors.next(errorsArr);
    }
  }

  public handleFormControl(name: string, field: string, form: AbstractControl, isSavings: boolean = false) {
    if (form?.valid) {
      this.removeError(name, field);
    } else {
      if ((form?.touched && form?.errors) || isSavings && form?.errors) {
        this.setError(name, field, form, form.errors);
      }
    }
  }

}

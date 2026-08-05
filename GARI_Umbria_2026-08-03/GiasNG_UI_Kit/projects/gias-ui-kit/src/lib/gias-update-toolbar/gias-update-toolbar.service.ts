import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { BehaviorSubject, Observable } from 'rxjs';
import { debounceTime, filter } from 'rxjs/operators';
import { compareObjects } from '../utils/compare-objects';
import { cloneDeep } from 'lodash';

class FormGroupChange {
  changedAt: Date;
  changes: FormGroup;
}

class AvailableChangeOperation {
  canUndo: boolean;
  canRedo: boolean;
}

@Injectable()
export class UpdateToolbarService {
  private lastIndex = -1;
  private saveValueChanges = true;
  private userForm: FormGroup;
  private changes: Array<FormGroupChange>;
  availableChangeOperation: AvailableChangeOperation = {
    canRedo: false,
    canUndo: false
  };

  private availableChangeOperationObjectSource = new BehaviorSubject(this.availableChangeOperation);
  currentAvailableChangeOperation: Observable<AvailableChangeOperation> = this.availableChangeOperationObjectSource.asObservable();

  setAvailableChangeOperation(availableChangeOperation: AvailableChangeOperation) {
    this.availableChangeOperationObjectSource.next(availableChangeOperation);
  }

  undo(): void {
    this.saveValueChanges = false;
    if (this.changes[this.lastIndex - 1] != undefined) {
      this.updateComplexFormGroup(this.changes[this.lastIndex - 1].changes);
      this.lastIndex -= 1;
    }
    this.updateAvailableChangeOperation();
    this.saveValueChanges = true;
  }

  redo(): void {
    this.saveValueChanges = false;
    if (this.changes[this.lastIndex + 1] != undefined) {
      this.updateComplexFormGroup(this.changes[this.lastIndex + 1].changes);
      this.lastIndex += 1;
    }
    this.updateAvailableChangeOperation();
    this.saveValueChanges = true;
  }


  initializeChanges(userForm: FormGroup) {
    this.changes = new Array<{ changedAt: Date; changes: any }>();
    this.userForm = userForm;

    this.userForm.valueChanges
      .pipe(
        filter(() => this.saveValueChanges),
        debounceTime(500)
      )
      .subscribe((values) => {
        if (this.formHasChanges(values) || this.changes.length == 0) {
          const userformCopy = cloneDeep(this.userForm);

          if (this.lastIndex + 1 == this.changes.length) {

            this.changes.push({ changedAt: new Date(), changes: userformCopy });
            this.lastIndex = this.changes.length - 1;

          } else if (this.lastIndex < this.changes.length) {

            this.changes.length = this.lastIndex + 1;
            this.changes.push({ changedAt: new Date(), changes: userformCopy });
            this.lastIndex = this.changes.length - 1;

          }
          //console.log(['subscribe1', this.changes.length, this.lastIndex]);
          this.updateAvailableChangeOperation();
        }

      }
      );
  }

  getLastIndex(): number {
    return this.lastIndex;
  }

  getChanges(): Array<FormGroupChange> {
    return this.changes;
  }

  updateAvailableChangeOperation() {
    let canRedo = false;
    let canUndo = false;
    if (this.changes.length > 1 && this.lastIndex > 0) {
      if (this.changes[this.lastIndex - 1] != undefined) {
        canUndo = true;
      }
    }

    if ((this.changes.length > 1) && (this.lastIndex < (this.changes.length - 1))) {
      if (this.changes[this.lastIndex + 1] != undefined) {
        canRedo = true;
      }
    }

    this.setAvailableChangeOperation({ canRedo: canRedo, canUndo: canUndo });
  }

  updateComplexFormGroup(values: FormGroup) {
    for (const key of Object.keys(values.value)) {
      this.userForm.removeControl(key);
      const control = values.controls[key]; // AbstractControl
      this.userForm.addControl(key, control);
    }

    this.userForm.patchValue(values.getRawValue(), { emitEvent: false });

    this.userForm.updateValueAndValidity({ emitEvent: true, onlySelf: false });

  }

  formHasChanges(values: any): boolean {
    if (this.changes.length > 0) {
      const oldValue = this.changes[this.lastIndex].changes.value;
      return !compareObjects(values, oldValue);
    }
    return false;
  }
}

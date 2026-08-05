import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BehaviorSubject, Observable, of } from 'rxjs';


@Injectable({
    providedIn:'root'
})
export class CostiMacchinaParentFormService{
    costimacchinaParentForm: FormGroup = this.fb.group({});

    costimacchinaParentFormSource = new BehaviorSubject(this.costimacchinaParentForm);

    constructor(private fb: FormBuilder) {  }

    public setCostiMacchinaParentForm(parent: FormGroup) {
        this.costimacchinaParentFormSource.next(parent);
    }

    public getCostiMacchinaParentForm(): FormGroup {
        return this.costimacchinaParentFormSource.getValue();
    }
}

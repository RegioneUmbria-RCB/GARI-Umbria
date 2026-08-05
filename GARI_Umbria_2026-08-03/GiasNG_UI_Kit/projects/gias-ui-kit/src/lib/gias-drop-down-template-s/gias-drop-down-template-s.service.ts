import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { DropdownListFormItem } from '../utils/models';

@Injectable()
export class GiasDropDownTemplateService {

    private DropDownValueObject = new DropdownListFormItem;

    private DropDownValueObjectSource = new BehaviorSubject(this.DropDownValueObject);
    currentDropDownValueObject: Observable<DropdownListFormItem> = this.DropDownValueObjectSource.asObservable();

    setDropDownValue(DropDownValueObject: DropdownListFormItem) {
        this.DropDownValueObjectSource.next(DropDownValueObject);
    }

}

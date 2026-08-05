import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable()
export class GiasMultiSelectTemplateService {
  private MultiSelectValueObject = new MultiSelectFormItem;
  private MultiSelectValueObjectSource = new BehaviorSubject(this.MultiSelectValueObject);

  currentMultiSelectValueObject: Observable<MultiSelectFormItem> = this.MultiSelectValueObjectSource.asObservable();

  setMultiSelectValue(MultiSelectValueObject: MultiSelectFormItem) {
    this.MultiSelectValueObjectSource.next(MultiSelectValueObject);
  }
}

export class MultiSelectFormItem {
  FormControlName: string;
  Values: Array<any>;
  newValue: any;
}

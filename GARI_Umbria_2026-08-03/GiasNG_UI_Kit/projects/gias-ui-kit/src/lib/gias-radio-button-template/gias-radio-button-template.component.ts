import { Component, EventEmitter, Input, Output } from '@angular/core';

export class RadioButtonValue {
  constructor(
    public name?: string,
    public value?: any,
    public enable?: boolean
  ) { }
}

@Component({
  standalone: false,
  selector: 'gias-radio-button-template',
  templateUrl: './gias-radio-button-template.component.html',
  styleUrls: ['./gias-radio-button-template.component.scss']
})
export class GiasRadioButtonTemplateComponent {
  @Input() radioButtonValues: RadioButtonValue[];
  @Input() value: any;
  @Input() formEnable: boolean;
  @Input() formControlName: string;
  @Output() valueChange = new EventEmitter<any>();

  onItemChange(event: Event) {
    this.valueChange.emit(this.value);
  }
}

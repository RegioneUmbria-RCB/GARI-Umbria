import { Component, EventEmitter, Input, OnInit, Output, AfterViewChecked } from '@angular/core';
import { ControlContainer, FormControl, FormGroup } from '@angular/forms';
import { ButtonGroupSelection } from '@progress/kendo-angular-buttons';

@Component({
  standalone: false,
  selector: 'gias-button-group-template',
  templateUrl: './gias-button-group-template.component.html',
  styleUrls: ['./gias-button-group-template.component.css']
})
export class GiasButtonGroupTemplateComponent implements OnInit, AfterViewChecked {
  @Input() name: string;
  @Input() value: any;
  @Input() giasFormControlName: string;
  @Input() isdisabled: boolean;
  @Input() listItems: Array<any>;
  @Input() textField: string;
  @Input() valueField: string;
  @Input() valuePrimitive = true;
  @Input() multiple = false;
  @Output() valueChange = new EventEmitter<any>();

  public selection: ButtonGroupSelection = 'single';
  disable: boolean = false;
  obligatory: boolean = false;
  
  private form: FormGroup;
  private fc: FormControl;

  constructor(public controlContainer: ControlContainer) { }

  ngOnInit(): void {
    this.form = <FormGroup>this.controlContainer.control;
    this.fc = <FormControl>this.form.get(this.giasFormControlName);
    if (this.fc) {
      if (this.valuePrimitive) {
        this.value = this.fc.value;
      } else {
        this.value = this.fc.value[this.valueField];
      }
    }
    if (this.multiple) {
      this.selection = 'multiple';
    }
    if (!this.fc.enabled) {
      this.disable = true;
    }
  }

  ngAfterViewChecked() {
    this.form = <FormGroup>this.controlContainer.control;
    this.fc = <FormControl>this.form.get(this.giasFormControlName);
    if (this.fc) {
      if (this.valuePrimitive) {
        this.value = this.fc.value;
      } else {
        this.value = this.fc.value[this.valueField];
      }
    }
  }

  change(val: any) {
    this.value = val[this.valueField];
    if (this.fc) {
      if (this.valuePrimitive) {
        this.fc.setValue(val[this.valueField]);
      } else {
        this.fc.setValue(val);
      }
    }
    this.valueChange.emit(val);
  }
}

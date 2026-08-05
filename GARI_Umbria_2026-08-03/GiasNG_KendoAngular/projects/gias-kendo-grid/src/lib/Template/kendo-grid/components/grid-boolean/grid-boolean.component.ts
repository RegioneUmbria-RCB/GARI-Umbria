import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { ControlContainer, FormControl, FormGroup, FormGroupDirective } from "@angular/forms";

/**
 * Questa componente è stata creata per la kendo grid.
 * Non dovrebbe essere usata fuori da questo modulo.
 * */
@Component({
    standalone: false,
  selector: 'gias-grid-boolean',
  templateUrl: './grid-boolean.component.html',
  styleUrls: ['./grid-boolean.component.css'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }],
})
export class GridBooleanComponent implements OnInit, OnChanges {
  @Input() formGroup: any;
  @Input() controlName: string;
  @Input() defaultValue: boolean;
  @Input() editable: boolean;
  @Input() useCheckbox: boolean;

  @ViewChild('checkContainer') container;

  @Output() valueChange = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit(): void {
    if (!this.formGroup && !this.controlName) {
      this.formGroup = new FormGroup({
        value: new FormControl(this.defaultValue)
      });
      this.controlName = 'value';
    }
    if (this.editable) this.formGroup.get(this.controlName).enable();
    else this.formGroup.get(this.controlName).disable();
  }

  onChange(value: boolean): void {
    this.valueChange.emit(value);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.container) {
      let checkbox = this.container.nativeElement.querySelector('.k-checkbox');
      checkbox.checked = this.defaultValue;
    }
  }

}

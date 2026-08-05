import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
    standalone: false,
    selector: 'gis-checkbox',
    templateUrl: './gis-checkbox.component.html',
    styleUrls: ['./gis-checkbox.component.css']
})
export class GisCheckboxComponent {
    @Input() name = '';
    @Input() giasFormControl: FormControl;

    @Output() gisChange = new EventEmitter<boolean>();

    toggle() {
        const value = !this.giasFormControl.value;
        this.giasFormControl.setValue(value);
        this.gisChange.emit(value);
    }
}

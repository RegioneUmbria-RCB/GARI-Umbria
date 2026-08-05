import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FormControl, FormGroup, FormGroupDirective } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO } from '../utils/models';

@Component({
    standalone: false,
    selector: 'gias-validita-datetime',
    templateUrl: './gias-validita-datetime.component.html',
    styleUrls: ['./gias-validita-datetime.component.css']
})
export class GiasValiditaDateTimeComponent implements OnInit {
    @Input() formGroupName: string;
    @Output('onBlur') onBlurEvent = new EventEmitter<string>();
    form: FormGroup<IntervalloTemporaleForm>;

    AGRODATA_INIZIO: Date = AGRODATAINIZIO;
    AGRODATA_FINE: Date = AGRODATAFINE;

    constructor(private rootFormGroup: FormGroupDirective) { }

    ngOnInit(): void {
        this.form = this.rootFormGroup.control.get(this.formGroupName) as FormGroup;
    }

    handleBlur(field: string) {
        this.onBlurEvent.emit(field);
    }
}

export interface IntervalloTemporaleForm {
    inizio: FormControl<Date>;
    fine: FormControl<Date>;
}
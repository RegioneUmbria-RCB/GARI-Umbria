import { Component, EventEmitter, Inject, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ControlContainer, FormControl, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { DropDownFilterSettings, DropDownListComponent, PopupSettings } from '@progress/kendo-angular-dropdowns';
import { Observable, Subject, Subscription, takeUntil } from 'rxjs';
import { GiasDropDownTemplateService } from './gias-drop-down-template-s.service';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from '../utils/gias-master.service';
import { DropdownListFormItem, enum_InputType } from '../utils/models';
import { GiasFormErrorVisualizerService } from '../gias-form-error-visualizer/gias-form-error-visualizer.service';
import { recursiveParentName } from '../utils/recursive-parent-name';

@Component({
    standalone: false,
    selector: 'gias-drop-down-template-s',
    templateUrl: './gias-drop-down-template-s.component.html',
    styleUrls: ['./gias-drop-down-template-s.component.css'],
    viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class GiasDropDownTemplateSComponent implements OnInit, OnDestroy {

    @Input() id = '';
    @Input() name: string;
    @Input() value: any;
    @Input() listItems: Array<any>;
    @Input() obligatory: boolean;
    @Input() clearButton: boolean;
    @Input() isDisabled: boolean;
    @Input() textField = 'descrizione';
    @Input() valueField = 'codice';
    @Input() filterable = true;
    @Input() defaultItem: any;
    @Input() giasFormControlName: string;
    @Input() parentGroup: FormGroup;
    @Input() parentFormControl: FormControl;
    @Input() valuePrimitive = true;
    @Input() applyMinimumFilterLength = false;
    @Input() MinimumFilterLength = 0;
    @Input() ddlServerFiltering = false;
    @Input() loadFunctionddlServerFiltering: (filter: string) => Observable<any>;
    @Input() popupSettings: PopupSettings = { width: 'auto' };
    @Input() hasInfo: boolean = false;
    @Input() infoTitle: string = '';
    @Input() isRequired: boolean = false;
    @Input() showStar: boolean = true;

    @Input() virtual: any = null;
    @Output() open = new EventEmitter<GiasDropDownTemplateSComponent>();
    @Output() filterChange = new EventEmitter<any>();

    @ViewChild('dropdownlist') public dropdownlist: DropDownListComponent;
    public listItemsNoFiltered: Array<any>;

    public filterSettings: DropDownFilterSettings;

    loading = false;

    Subs: Subscription = new Subscription();

    fg: FormControl;
    public completeFormControlName: string;

    public signal$: Subject<void> = new Subject();
    inputType: enum_InputType;
    constructor(
        @Inject(GIAS_MASTER_SERVICE_TOKEN) private giasMasterService: IGiasMasterService,
        private dropdownService: GiasDropDownTemplateService,
        private formErrorVisualizerService: GiasFormErrorVisualizerService,
        private rootFormGroup: FormGroupDirective) {

    }

    getCtrlName() {
        return this.giasFormControlName;
    }

    updateFormErrorVisualizer() {
        this.formErrorVisualizerService.handleFormControl(this.name, this.completeFormControlName, this.fg);
    }

    ngOnInit(): void {
        this.fg = this.rootFormGroup?.form?.controls[this.giasFormControlName] as FormControl;
        if (this.fg) {
            this.completeFormControlName = recursiveParentName(this.fg);
        }

        this.Subs.add(this.giasMasterService.currentInputType.subscribe((it) => {
            this.inputType = it;
        }));

        this.fg?.statusChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
            this.updateFormErrorVisualizer();
        });

        this.fg?.valueChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
            this.updateFormErrorVisualizer();
        });

        this.updateFormErrorVisualizer();
        if (this.filterable) {
            this.filterSettings = {
                caseSensitive: false,
                operator: 'contains'
            };
        }

    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
        this.signal$.next();
        this.signal$.complete();
    }

    changeValue(newValue: any) {

        const newObjectValue = new DropdownListFormItem;

        newObjectValue.Id = this.id;

        newObjectValue.FormControlName = this.giasFormControlName;

        if (this.valuePrimitive) {
            newObjectValue.Value = this.listItems?.find((i) => {
                if (i[this.valueField] === newValue) {
                    return i;
                }
            });
        } else {
            newObjectValue.Value = this.listItems?.find((i) => {
                if (i[this.valueField] === newValue[this.valueField]) {
                    return i;
                }
            });
        }

        this.dropdownService.setDropDownValue(newObjectValue);
    }

    openDdl(): void {

        this.open.emit(this);
    }

    handleFilter(value): void {

        if (this.applyMinimumFilterLength && this.MinimumFilterLength > 0) {
            if (value.length >= this.MinimumFilterLength) {

                if (this.ddlServerFiltering && this.loadFunctionddlServerFiltering !== undefined && this.loadFunctionddlServerFiltering !== null) {

                    this.Subs.add(this.loadFunctionddlServerFiltering(value).subscribe((listItems) => {
                        this.listItems = listItems;
                        this.filterChange.emit(value);
                    }));

                } else {
                    this.listItems = this.listItemsNoFiltered.filter(
                        (s) => s[this.textField].toLowerCase().indexOf(value.toLowerCase()) !== -1
                    );

                    this.filterChange.emit(value);
                }

            } else {
                this.listItems = [];
                this.filterChange.emit(value);
            }
        }
        else {
            this.filterChange.emit(value);
        }
    }

    get validator() {
        if (this.fg.hasValidator(Validators.required) || this.obligatory == true) {
            return true;
        } else {
            return false;
        }
    }

    onBlur() {
        this.updateFormErrorVisualizer();
    }

}

import { Component, EventEmitter, Inject, Input, OnDestroy, OnInit, Output, SkipSelf, ViewChild } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective, FormControl, Validators } from '@angular/forms';
import { DropDownFilterSettings, DropDownListComponent } from '@progress/kendo-angular-dropdowns';
import { Observable, Subject, Subscription, takeUntil } from 'rxjs';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from '../utils/gias-master.service';
import { DropdownEventType, DropdownListEvent, DropdownListItem, enum_InputType } from '../utils/models';
import { recursiveParentName } from '../utils/recursive-parent-name';
import { GiasFormErrorVisualizerService } from '../gias-form-error-visualizer/gias-form-error-visualizer.service';

@Component({
  standalone: false,
  selector: 'gias-drop-down-template',
  templateUrl: './gias-drop-down-template.component.html',
  styleUrls: ['./gias-drop-down-template.component.scss'],
  //viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }],
  viewProviders: [{
    provide: ControlContainer,
    useFactory: (container?: ControlContainer) => container,
    deps: [[new SkipSelf(), ControlContainer]],
  }]
})
export class GiasDropDownTemplateComponent implements OnInit, OnDestroy {
  @Input() id = '';
  @Input() name: string;
  @Input() value: any;
  @Input() listItems: Array<any>;
  @Input() itemDisabledFn: (i: any) => boolean = () => false;
  @Input() obligatory: boolean;
  @Input() clearButton: boolean;
  @Input() isDisabled: boolean;
  @Input() textField: string;
  @Input() valueField: string;
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
  @Input() virtual: any = null;
  @Output() valueChange = new EventEmitter<DropdownListEvent>();
  @Output() open = new EventEmitter<GiasDropDownTemplateComponent>();
  @Output() filterChange = new EventEmitter<any>();
  @ViewChild('dropdownlist') public dropdownlist: DropDownListComponent;

  loading = false;

  Subs: Subscription = new Subscription();

  public listItemsNoFiltered: Array<any>;

  public filterSettings: DropDownFilterSettings;
  fg: FormControl;
  public completeFormControlName: string;

  public signal$: Subject<void> = new Subject();
  inputType: enum_InputType;

  constructor(
    @Inject(GIAS_MASTER_SERVICE_TOKEN) private giasMasterSErvice: IGiasMasterService,
    private rootFormGroup: FormGroupDirective,
    private formErrorVisualizerService: GiasFormErrorVisualizerService
  ) { }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  updateFormErrorVisualizer() {
    this.formErrorVisualizerService.handleFormControl(this.name, this.completeFormControlName, this.fg);
  }

  getCtrlName() {
    return this.giasFormControlName;
  }

  ngOnInit(): void {
    this.fg = this.rootFormGroup?.form?.controls[this.giasFormControlName] as FormControl;
    if (this.fg) {
      this.completeFormControlName = recursiveParentName(this.fg);
    }
    this.Subs.add(this.giasMasterSErvice.currentInputType.subscribe((it) => {
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

  changeValue(newValue: DropdownListItem) {
    this.valueChange.emit({ id: this.id, type: DropdownEventType.ON_CHANGE_VALUE, data: newValue, listItems: this.listItems });
  }

  openDdl(): void {

    this.open.emit(this);
  }

  onBlur() {
    this.updateFormErrorVisualizer();
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
    if (this.fg?.hasValidator(Validators.required)) {
      return true;
    } else {
      return false;
    }
  }

}

import { Component, EventEmitter, Inject, Input, OnDestroy, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { ControlContainer, FormControl, FormGroup, FormGroupDirective } from '@angular/forms';
import { DropDownFilterSettings, MultiSelectComponent, PopupSettings, RemoveTagEvent } from '@progress/kendo-angular-dropdowns';
import { Subscription } from 'rxjs';
import { MultiSelectFormItem, GiasMultiSelectTemplateService } from './gias-multiselect-template-s.service';
import { enum_InputType } from '../utils/models';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from '../utils/gias-master.service';
import { getValueOfPropertyInObject } from '../utils/get-value-of-property-in-object';

@Component({
  standalone: false,
  selector: 'gias-multiselect-template-s',
  templateUrl: './gias-multiselect-template-s.component.html',
  styleUrls: ['./gias-multiselect-template-s.component.css'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class GiiasMultiselectTemplateSComponent implements OnInit, OnDestroy {
  @Input() id = '';
  @Input() name: string;
  @Input() value: Array<any>;
  @Input() listItems: Array<any>;
  @Input() obligatory: boolean;
  @Input() clearButton = true;
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
  @Input() autoClose = false;
  @Input() hasInfo = false;
  @Input() placeholder: string;
  @Input() showRedAsterisk = false;
  @Input() labelTemplate: TemplateRef<any> = null;
  @Input() popupSettings: PopupSettings = { width: 'auto' };
  @Input() virtual: any = null;
  @Input() groupBy = false;
  @Output() open = new EventEmitter<GiiasMultiselectTemplateSComponent>();
  @Output() filterChange = new EventEmitter<any>();
  @Output() removeItem = new EventEmitter<RemoveTagEvent>();
  @Output() selectionItem = new EventEmitter<any>();

  @ViewChild('multiselect') public multiselect: MultiSelectComponent;
  public listItemsNoFiltered: Array<any>;
  public loading = false;
  public filterSettings: DropDownFilterSettings;
  public inputType: enum_InputType;
  public previousMultiSelectValue: Array<any> = [];
  protected readonly inputTypeValue = { twoLines: enum_InputType.Due_righe, giasClassic: enum_InputType.Gias_Classic };
  private sub: Subscription;

  constructor(
    @Inject(GIAS_MASTER_SERVICE_TOKEN) private masterService: IGiasMasterService,
    private multiselectService: GiasMultiSelectTemplateService,
    private parent: FormGroupDirective
  ) {
    this.sub = this.masterService.currentInputType.subscribe((it) => this.inputType = it);
  }

  protected get isUsingForm(): boolean {
    return (this.giasFormControlName != undefined && true && this.giasFormControlName != '');
  }

  getCtrlName() {
    return this.giasFormControlName;
  }

  ngOnInit(): void {
    if (this.filterable) {
      this.filterSettings = { caseSensitive: false, operator: 'contains' };
    }
    //previousMultiSelectValue prende il valore con cui è stata impostata di default la multiselect
    if (this.giasFormControlName && this.giasFormControlName !== "" && this.parent.form.get(this.getCtrlName()).value)
      this.previousMultiSelectValue = this.parent.form.get(this.getCtrlName()).value;
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  changeValue(Values: any[]) {
    let newValue = null;
    if (this.previousMultiSelectValue && this.previousMultiSelectValue.length > 0) {
      newValue = Values.find((element) => !this.previousMultiSelectValue.includes(element));
    } else if (Values.length === 1) {
      newValue = Values[0];
    }
    this.previousMultiSelectValue = Values;

    const newObjectValue = new MultiSelectFormItem;
    newObjectValue.FormControlName = this.giasFormControlName;
    if (newValue && this.listItems && this.valuePrimitive) {
      newObjectValue.newValue = this.listItems.find((i) => {
        if (getValueOfPropertyInObject(i, this.valueField) === newValue) {
          return i;
        }
      });
    } else if (newValue && this.listItems && !this.valuePrimitive) {
      newObjectValue.newValue = this.listItems.find((i) => {
        if (getValueOfPropertyInObject(i, this.valueField) === getValueOfPropertyInObject(newValue, this.valueField)) {
          return i;
        }
      });
    } else {
      newObjectValue.newValue = newValue;
    }

    newObjectValue.Values = Values;
    this.multiselectService.setMultiSelectValue(newObjectValue);
    this.selectionItem.next(Values);
  }

  openDdl(): void {
    this.open.emit(this);
  }

  handleFilter(value): void {
    if (!this.groupBy) {
      if (this.applyMinimumFilterLength && this.MinimumFilterLength > 0) {
        if (value.length >= this.MinimumFilterLength) {
          this.listItems = this.listItemsNoFiltered.filter(
            (s) => s[this.textField].toLowerCase().indexOf(value.toLowerCase()) !== -1
          );
        } else this.listItems = [];
      }
    } else {
      if (value.length > 0) {
        this.listItems = this.listItemsNoFiltered.map((group) => ({
          ...group,
          items: group.items.filter((s) =>
            s[this.textField].toLowerCase().indexOf(value.toLowerCase()) !== -1
          )
        })).filter((group) => group.items.length > 0);
      } else this.listItems = this.listItemsNoFiltered;
    }
    this.filterChange.emit(value);
  }

  removeTag(event: RemoveTagEvent) {
    this.removeItem.emit(event);
  }

}

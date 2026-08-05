
import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { AbstractControl, ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { filter, lastValueFrom, Subject, Subscription, takeUntil } from 'rxjs';
import { DropdownList, DropdownListItem, DropdownListWithForm, KendoGridColumn } from '../../models/grid.model';
import { GridPublicService } from '../../services/grid-public.service';
import { GridRootHelper } from '../../services/grid-root-helper.service';
import { KendoGridService } from '../../services/kendo-grid.service';
import { GridDropdownService, NextDropdownValue } from './grid-dropdown.service';
import { DropdownListEvent, DropdownEventType } from 'gias-ui-kit';

@Component({
    standalone: false,
  selector: 'gias-grid-ddl',
  templateUrl: './grid-dropdownlist.component.html',
  styleUrls: ['./grid-dropdownlist.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }],
  providers: [GridDropdownService],
  encapsulation: ViewEncapsulation.None
})
export class GridDropdownListComponent implements OnInit, OnDestroy {
  @Input() useForm = false;

  @Input() column: KendoGridColumn;
  @Input() id: string;
  @Input() filterable: boolean;
  @Input() defaultItem: DropdownListItem;
  @Input() textField: string;
  @Input() valueField: string;
  @Input() valuePrimitive: boolean;
  @Input() controlName: string;
  @Input() formGroup: FormGroup;
  @Input() selected: DropdownListItem;
  @Input() ddl: DropdownListWithForm;
  @Output() selectedChange = new EventEmitter<DropdownListItem>();

  @Output() valueChange = new EventEmitter<DropdownListEvent>();
  @Output() open = new EventEmitter<DropdownListEvent>();
  @Output() reload = new EventEmitter<DropdownListEvent>();

  signal: Subject<void> = new Subject();

  public onNgModelChange(data) {
    this.selectedChange.emit(data);
  }

  @Output() sourceChange = new EventEmitter<DropdownListItem[]>();
  @Input() source: DropdownListItem[];
  sourceChangeSub: Subscription;


  data: DropdownListItem[];
  updateSub: Subscription;

  addRemoveBorderClass: boolean = true;

  constructor(
    private gridService: KendoGridService,
    private gridRootService: GridRootHelper,
    private gridPublicService: GridPublicService,
    public dropdownSerivce: GridDropdownService,
  ) { }

  ngOnInit(): void {

    this.formGroup.controls[this.column.field].statusChanges.subscribe(state => {
      this.addRemoveBorderClass = state === 'VALID';
    });

    this.gridRootService.nextDropdownValue.pipe(takeUntil(this.signal), filter(s => s.controlName === this.controlName))
      .subscribe((data: NextDropdownValue) => {
        let formGrp = data.formGroup;
        let ctrlName = data.controlName;
        let value = data.value;
        let col = data.column;

        let ctrl = formGrp.controls[ctrlName];
        let exists = this.checkIfValueExists(value);

        if (exists)
          ctrl.patchValue(value);
        else {
          lastValueFrom(data.loadData(this.gridPublicService.currentDataItem))
            .then(data => {
              this.patchValue(ctrl, value, data, col);
            });
        }
      });

    this.sourceChangeSub = this.dropdownSerivce.sourceChange
      .subscribe((ddl: DropdownList) => {
        if (!ddl) {
          return;
        }

        this.data = ddl.data;
        this.source = ddl.data;
        this.defaultItem = ddl.defaultValue;
        this.id = ddl.id;
        this.textField = ddl.textField;
        this.valueField = ddl.valueField;

        this.selected = ddl.defaultValue;

        this.selectedChange.emit(this.selected);
      });

    this.formGroup.controls[this.controlName].valueChanges.pipe(takeUntil(this.signal)).subscribe();

    if (!this.source) {
      this.source = [];
    }
    this.data = this.source.slice();

    this.updateSub = this.gridService.updateDropdownColumn.subscribe((event: DropdownListEvent) => {
      if (event.id === this.id) {
        if ((event.data as DropdownListWithForm[]).length === 1) {
          this.source = event.data[0].ddl.data;
          this.data = this.source.slice();
        }
      }
    });

    if (this.ddl?.reload?.subscribe) {
      this.ddl?.reload?.subscribe(val => {
        if (val) {
          this.reload.emit({
            id: this.id,
            type: DropdownEventType.ON_UPDATE_DROPDOWN,
            data: this,
            listItems: this.source
          })
        }
      });
    } else {
      console.log(this.controlName + 'subscribe undefined');
    }

  }

  patchValue(ctrl: AbstractControl, value: string, data: any[], column: KendoGridColumn) {
    this.source = data;
    this.data = data;

    let exists2 = this.checkIfValueExists(value);
    if (exists2) {
      column.ddl.data = this.data;
      ctrl.patchValue(value);
    }
    else {
      console.log('You are trying to patch a non existing value');
      if (this.data.length > 0) {
        let item = this.data[0];
        if (!item.hasOwnProperty('id') || !item.hasOwnProperty('name')) {
          console.log('Read data does not have required fields: "id" and "name"');
        }

      }
    }
  }

  checkIfValueExists(itemId: string): boolean {
    let exists = false;

    let index = this.data.findIndex(s => s.id == itemId);

    if (index >= 0)
      exists = true;

    return exists;
  }

  handleFilter(value) {
    this.filter = value;
    this.data = this.source.filter(
      (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  changeValue(newValue: DropdownListItem) {
    this.sourceChange.emit(this.source);
    this.valueChange.emit(
      {
        id: this.id,
        type: DropdownEventType.ON_CHANGE_VALUE,
        data: newValue,
        listItems: this.source
      });

    this.selected = newValue;
    this.selectedChange.emit(newValue);
  }


  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
    this.updateSub.unsubscribe();
    this.sourceChangeSub.unsubscribe();
  }

  public filter: string;
  public addNew(): void {
    this.source.push({
      name: this.filter,
      id: this.dropdownSerivce.generateUID(),
    });
    this.handleFilter(this.filter);
  }

  onOpen() {
    this.open.emit(
      {
        id: this.id,
        type: DropdownEventType.ON_OPEN_DROPDOWN,
        data: this,
        listItems: this.source
      });
  }

}

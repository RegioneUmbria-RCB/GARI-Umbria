import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { DropdownListItem, DropdownListWithForm } from '../../models/grid.model';
import { GridMultiDropdownService } from './grid-multi-dropdown.service';
import { DropdownEventType, DropdownListEvent } from 'gias-ui-kit';

/**
 * Questa componente è stata creata per la kendo grid.
 * Non dovrebbe essere usata fuori da questo modulo.
 * Usare DropDownTemplateComponent invece.
 * */
@Component({
    standalone: false,
  selector: 'gias-grid-multi-ddl',
  templateUrl: './grid-multi-dropdownlist.component.html',
  styleUrls: ['./grid-multi-dropdownlist.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }],
  providers: [GridMultiDropdownService]
})
export class GridMultiDropdownComponent implements OnInit, OnDestroy {
  @Input() useForm = false;
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

  @Output() valueChange = new EventEmitter<DropdownListEvent>();
  @Output() open = new EventEmitter<DropdownListEvent>();
  @Output() reload = new EventEmitter<DropdownListEvent>();

  signal: Subject<void> = new Subject();
  data: DropdownListItem[];

  @ViewChild("multi", { static: true }) multi;
  @Input() defaultOpen: boolean;

  @Output() sourceChange = new EventEmitter<DropdownListItem[]>();
  @Input() source: DropdownListItem[] = [];

  ngOnInit(): void {
    this.data = this.source.slice();

    if (this.ddl?.reload?.subscribe) {
      this.ddl?.reload?.pipe(takeUntil(this.signal)).subscribe(val => {
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

    if (!this.defaultOpen) this.defaultOpen = false;
    else {
      this.multi.toggle(this.defaultOpen);
      this.onOpen();
    }

    this.formGroup.valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.fixValidationForThisControl());
  }

  public changeValue(newValue: DropdownListItem) {
    this.sourceChange.emit(this.source);
    this.valueChange.emit(
      {
        id: this.id,
        type: DropdownEventType.ON_CHANGE_VALUE,
        data: newValue,
        listItems: this.source
      });

    this.selected = newValue;
  }

  public filter: string;
  public handleFilter(value) {
    this.filter = value;
    this.data = this.source.filter(
      (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  public onOpen() {
    this.open.emit({
      id: this.id,
      type: DropdownEventType.ON_OPEN_DROPDOWN,
      data: this,
      listItems: this.source
    });
  }

  public ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }

  private fixValidationForThisControl() {
    if (this.formGroup.get(this.controlName).valid && this.multi?.wrapper?.nativeElement) {
      const domEl = this.multi.wrapper.nativeElement;
      if (domEl.classList.contains('ng-invalid')) {
        domEl.classList.remove("ng-invalid");
      }
    }
  }
}

import { DecimalPipe, DatePipe } from "@angular/common";
import { Component, OnInit, OnDestroy, OnChanges, Input, TemplateRef, Output, EventEmitter, ViewChild, ContentChild, Inject, LOCALE_ID, SimpleChanges } from "@angular/core";
import { ControlContainer, FormGroupDirective, FormGroup, FormControl } from "@angular/forms";
import { VirtualizationSettings, MultiColumnComboBoxComponent, DropDownFilterSettings, PopupSettings } from "@progress/kendo-angular-dropdowns";
import { Observable, Subscription, map } from "rxjs";
import { GiasJoinPipe } from "../pipes/join/join.pipe";
import { ColumnCombobox, MultiColumnComboboxFormItem, MultiColumnComboboxService } from "./gias-multi-column-combobox-template.service";
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from "../utils/gias-master.service";
import { AGRODATAFINE, AGRODATAINIZIO, CELL_TYPES, enum_InputType } from "../utils/models";
import { getValueOfPropertyInObject } from "../utils/get-value-of-property-in-object";

@Component({
  standalone: false,
  selector: 'gias-multi-column-combobox-template',
  templateUrl: './gias-multi-column-combobox-template.component.html',
  styleUrls: ['./gias-multi-column-combobox-template.component.css'],
  viewProviders: [
    { provide: ControlContainer, useExisting: FormGroupDirective }],
  providers: [DecimalPipe, GiasJoinPipe]
})
export class GiasMultiColumnComboboxTemplateComponent implements OnInit, OnDestroy, OnChanges {
  @Input() id = '';
  @Input() name: string;
  @Input() value: any;
  @Input() listItems: Array<any>;
  @Input() obligatory: boolean;
  @Input() clearButton: boolean = true;
  @Input() isDisabled: boolean;
  @Input() textField = 'descrizione';
  @Input() valueField = 'codice';
  @Input() filterable = true;
  @Input() giasFormControlName: string;
  @Input() parentGroup: FormGroup;
  @Input() parentFormControl: FormControl;
  @Input() valuePrimitive = true;
  @Input() applyMinimumFilterLength = false;
  @Input() MinimumFilterLength = 0;
  @Input() columns: Array<ColumnCombobox>;
  @Input() ddlServerFiltering = false;
  @Input() popupSettings: PopupSettings = { width: 'auto', animate: false };
  @Input() class: string = "";
  @Input() loadFunctionMultiColumnComboboxServerFiltering: (filter: string) => Observable<any>;
  @Input() ColumnComboBoxColumnCellTemplate: boolean = false;
  @Input() readonly: boolean = false;
  @Input() itemTemplate: TemplateRef<any>;
  @Input() placeholder: string;
  @Input() emptyObj: any = null;
  @Input() allowCustom: boolean = false;
  @Input() virtualization: VirtualizationSettings = {
    pageSize: 20,
    itemHeight: 50
  };
  @Output() open = new EventEmitter<GiasMultiColumnComboboxTemplateComponent>();
  @Output() filterChange = new EventEmitter<any>();
  @Output() close = new EventEmitter<any>();
  @Output() onclosed = new EventEmitter<any>();
  @Output() blur = new EventEmitter<any>();
  @ViewChild('multicolumncombobox') public Multicolumncombobox: MultiColumnComboBoxComponent;

  // ElementRef for no data template
  @ContentChild('noDataTemplate') noDataTemplate: TemplateRef<any>;

  public DefaultWidthColumnNumber: number = 80;
  public listItemsBeforeFilter: Array<any>;
  public listItemsBeforeValueNormalizer: Array<any>;
  public filterSettings: DropDownFilterSettings;

  loading = false;
  Subs: Subscription = new Subscription();
  inputType: enum_InputType;

  private currentFilter: string = '';

  constructor(
    @Inject(GIAS_MASTER_SERVICE_TOKEN) private masterService: IGiasMasterService,
    private multicolumncomboboxservice: MultiColumnComboboxService,
    private parent: FormGroupDirective,
    private decimalpipe: DecimalPipe,
    private datePipe: DatePipe,
    private giasjoinPipe: GiasJoinPipe,
    @Inject(LOCALE_ID) private locale_id: string) {

    this.Subs.add(this.masterService.currentInputType.subscribe((it) => {
      this.inputType = it;
    }));
  }

  ngOnInit(): void {
    if (this.filterable) {
      this.filterSettings = {
        caseSensitive: false,
        operator: 'contains'
      };
    }

    let formgroup = <FormGroup>this.parent.form;
    let formcontrol = <FormControl>formgroup.get(this.giasFormControlName);

    //Vado a impostare il valore con cui ho fatto patchValue al formgroup
    //(dato che sembra che non lo faccia in automatico come con le ddl)
    this.Subs.add(formcontrol.valueChanges.subscribe(newValue => {
      if (this.listItems && newValue) {

        let index = this.listItems.findIndex(i => i === newValue);

        if (index === -1) {
          this.listItems = [];

          this.listItems.push(newValue);
        }

      }
    }));

    this.ImpostaWidthInAutomatico();
  }

  ImpostaWidthInAutomatico() {
    //Per le colonne di tipo number imposto in automatico come width 50
    if (this.columns) {
      for (let i in this.columns) {
        if (this.columns[i].type === CELL_TYPES.NUMBER && (!this.columns[i].width || this.columns[i].width === 0))
          this.columns[i].width = this.DefaultWidthColumnNumber;
      }
    }
  }

  ngOnDestroy(): void {
    this.Subs.unsubscribe();
  }

  openDdl(): void {
    this.open.emit(this);
  }

  handleFilter(value): void {

    //setTimeout(()=> this.ApplicaFiltroMultiColumnCombobox(value),2000);

    this.currentFilter = value;

    this.ApplicaFiltroMultiColumnCombobox();

  }

  ApplicaFiltroMultiColumnCombobox() {

    if (this.ddlServerFiltering && this.loadFunctionMultiColumnComboboxServerFiltering !== undefined && this.loadFunctionMultiColumnComboboxServerFiltering !== null) {
      if (this.applyMinimumFilterLength && this.MinimumFilterLength > 0) {
        if (this.currentFilter.length >= this.MinimumFilterLength) {
          this.Subs.add(this.loadFunctionMultiColumnComboboxServerFiltering(this.currentFilter).subscribe((listItems) => {
            this.listItems = listItems;
          }));
        }
      }
    } else {
      this.listItems = this.listItemsBeforeFilter.filter((i) => this.filterlocalData(i));
    }

    this.filterChange.emit(this.currentFilter);
  }

  filterlocalData(item: any): boolean {
    //Filtro in tutte le colonne della multicolumncombobox

    let filter = false;

    if (item && this.columns && this.columns.length > 0) {

      for (let c of this.columns) {
        if (!c.hidden) {
          let value = this.FormatColumnValue(item, c)

          if (value !== "") {
            if (value.toLowerCase().indexOf(this.currentFilter.toLowerCase()) !== -1) {
              filter = true;
              break;
            }
          }
        }
      }
    }

    return filter;
  }

  changeValue(newValue: any) {

    const newObjectValue = new MultiColumnComboboxFormItem;

    newObjectValue.FormControlName = this.giasFormControlName;

    if (newValue !== undefined && newValue !== null) {

      if (!this.emptyObj || this.emptyObj[this.valueField] !== newValue[this.valueField]) {
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

        this.multicolumncomboboxservice.setMultiColumnComboboxValue(newObjectValue);
      }
      newObjectValue.Value = newValue;

      this.multicolumncomboboxservice.setMultiColumnComboboxValue(newObjectValue);
    }
  }

  handleClose(event: any): void {
    this.close.emit(event);
  }

  changeSelection(event: any) {
    setTimeout(() => {
      //Faccio l'onblur dopo 100 millisecondi perchè se no il change value non è valorizzato
      this.Multicolumncombobox.blur();
    }, 100);
  }

  private FormatNumberToLocalTime(dataItem: any, field: string, digitsInfo: string) {

    let number_str = "0";

    let value = getValueOfPropertyInObject(dataItem, field);

    if (value && value !== 0)
      number_str = this.decimalpipe.transform(value, digitsInfo, this.locale_id);

    return number_str;
  }

  private getDescription(dataItem: any, field: string) {

    let str = "";

    let value = getValueOfPropertyInObject(dataItem, field);

    if (value && value !== "")
      str = value.toString();

    return str;
  }

  private FormatDate(dataItem: any, field: string) {

    let date_str = "";

    let value: Date = getValueOfPropertyInObject(dataItem, field);

    if (value && value.getTime() !== AGRODATAINIZIO.getTime() && value.getTime() !== AGRODATAFINE.getTime())
      date_str = this.datePipe.transform(value, 'dd/MM/yy', undefined, this.locale_id);

    return date_str;
  }

  private FormatArray(dataItem: any, column: ColumnCombobox) {

    let descr = "";

    let first_field: string = column.Arrayfield.split(".")[0];

    let other_fileds: string = column.Arrayfield.split(".").splice(1).join(".");

    descr = this.giasjoinPipe.transform(dataItem[first_field], column.Arrayseparator, other_fileds, column.formatNumbertolocal, column.digitsInfo);

    return descr;
  }

  public onFocus(e: any) {
    //Quando si clicca sul multicolumn si apre in automatico
    if (!this.Multicolumncombobox.isOpen) {
      this.Multicolumncombobox.toggle(true);

      this.openDdl();
    }
  }

  public valueNormalizer = (text: Observable<string>) =>
    text.pipe(
      map((content: string) => {

        if (this.emptyObj) {
          this.listItemsBeforeValueNormalizer = this.Multicolumncombobox.data.filter(d => d[this.valueField] !== this.emptyObj[this.valueField]);

          let newObj: any = JSON.parse(JSON.stringify(this.emptyObj));
          newObj[this.textField] = content;

          this.listItemsBeforeValueNormalizer.push(newObj);

          return newObj;
        } else {
          return null;
        }

      })
    );


  public ShowElementInList(dataItem: any) {
    //Escludo il valore normalizzato
    let show = true;
    if (this.emptyObj && !this.valuePrimitive &&
      dataItem[this.valueField] === this.emptyObj[this.valueField]) {
      show = false;
    }

    return show;

  }

  public onBlur(event: any) {
    this.blur.emit(event);
  }

  public OnClosed(event: any) {
    this.onclosed.emit(event);

    if (this.listItemsBeforeValueNormalizer && this.listItemsBeforeValueNormalizer.length > 0) {
      this.listItems = this.listItemsBeforeValueNormalizer;
      this.listItemsBeforeValueNormalizer = null;
    }
  }

  public FormatColumnValue(dataItem: any, col: ColumnCombobox): string {
    let value = "";

    if (col.isArray === false) {
      if (col.type == 'number' && col.formatNumbertolocal === true) {
        value = this.FormatNumberToLocalTime(dataItem, col.field, col.digitsInfo);
      }

      if (col.type != 'date' && col.formatNumbertolocal === false) {
        value = this.getDescription(dataItem, col.field);
      }

      if (col.type == 'date') {
        value = this.FormatDate(dataItem, col.field);
      }
    } else {
      value = this.FormatArray(dataItem, col);
    }

    return value;
  }

  public GetNumberOfRows(): number {
    let number_rows: number = 0;

    if (this.currentFilter === "") {
      number_rows = this.listItems.filter(dataItem => (!this.emptyObj) || (dataItem[this.valueField] !== this.emptyObj[this.valueField])).length;
    } else {
      number_rows = this.Multicolumncombobox.data.filter(dataItem => (!this.emptyObj) || (dataItem[this.valueField] !== this.emptyObj[this.valueField])).length;
    }

    return number_rows;
  }

  ngOnChanges(changes: SimpleChanges) {
    this.listItemsBeforeFilter = this.listItems;
  }
}
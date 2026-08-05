import { Component, ElementRef, EventEmitter, Inject, Input, OnDestroy, OnInit, Output, SkipSelf, ViewChild } from '@angular/core';
import { ControlContainer, FormControl, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { DateTimePickerComponent, FormatSettings } from '@progress/kendo-angular-dateinputs';
import { debounceTime, Subject, takeUntil } from 'rxjs';
import { AGRODATAFINE, AGRODATAINIZIO, enum_InputType } from '../utils/models';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from '../utils/gias-master.service';
import { GiasFormErrorVisualizerService } from '../gias-form-error-visualizer/gias-form-error-visualizer.service';
import { recursiveParentName } from '../utils/recursive-parent-name';

@Component({
  standalone: false,
  selector: 'gias-date-time-picker-template',
  templateUrl: './gias-date-time-picker-template.component.html',
  styleUrls: ['./gias-date-time-picker-template.component.scss'],
  viewProviders: [{
    provide: ControlContainer,
    useFactory: (container: ControlContainer) => container,
    deps: [[new SkipSelf(), ControlContainer]],
  }]
})
export class GiasDateTimePickerTemplateComponent implements OnInit, OnDestroy {

  @ViewChild('datetimepicker') dateTimePicker: DateTimePickerComponent;

  @Input() public name: string;
  @Input() public value: Date;
  @Input() public nullValue: Date;
  @Input() public startValue: Date;
  @Input() public endValue: Date;
  @Input() public obligatory = false;
  @Input() public isDisabled: boolean;
  @Input() giasFormControlName: string;
  @Input() parentGroup: FormGroup;
  @Input() readOnlyInput: boolean;
  @Input() disabledDatesValidation: boolean = true;
  @Input() disabledDates: Date[] = [];
  @Output() valueChange = new EventEmitter<Date>();
  @Output() onBlur = new EventEmitter();

  public DateValue: Date;
  public signal$: Subject<void> = new Subject();
  public completeFormControlName: string;

  public format: FormatSettings = {
    displayFormat: "dd/MM/yyyy HH:mm",
    inputFormat: "dd/MM/yyyy HH:mm",
  };

  fg: FormControl;
  inputType: enum_InputType;

  // private millennium = 2000;
  private onChangeSubject = new Subject<Date>();

  constructor(@Inject(GIAS_MASTER_SERVICE_TOKEN) private masterService: IGiasMasterService,
    private rootFormGroup: FormGroupDirective,
    public intl: IntlService,
    private elementRef: ElementRef,
    private formErrorVisualizerService: GiasFormErrorVisualizerService) {
    this.masterService.currentInputType.subscribe((it) => {
      this.inputType = it;
    });
  }

  get validator() {
    if (this.fg.hasValidator(Validators.required)) {
      return true;
    } else {
      return false;
    }
  }

  ngOnInit(): void {
    this.onChangeSubject
      .pipe(debounceTime(1000), takeUntil(this.signal$))
      .subscribe((date: Date) => {
        this.adjustDateTime(date);
      });

    this.fg = this.rootFormGroup.form.controls[this.giasFormControlName] as FormControl;
    if (this.fg) {
      this.completeFormControlName = recursiveParentName(this.fg);
      // this.millennium = (this.fg.value as Date).getFullYear() < 2000 ? 1900 : 2000;
    }
    if (this.startValue === undefined || this.startValue == null) {
      this.startValue = AGRODATAINIZIO;
    }

    if (this.endValue === undefined || this.endValue == null) {
      this.endValue = AGRODATAFINE;
    }

    this.DateValue = AGRODATAINIZIO;
    try {
      this.DateValue = this.value;
    } catch (e) {

    }

    this.fg?.statusChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
      this.updateFormErrorVisualizer();
    });

    this.fg?.valueChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
      this.updateFormErrorVisualizer();
    });

    this.updateFormErrorVisualizer();

  }

  onChange(event: Date) {
    this.onChangeSubject.next(event);
    this.valueChange.emit(event);
  }

  ngAfterViewInit() {
    if (this.dateTimePicker.value && this.nullValue && this.dateTimePicker.value.getTime() === this.nullValue.getTime()) {
      this.elementRef.nativeElement.querySelector('input').value = '';
    }
  }

  handleFocus() {
    if (this.dateTimePicker.value && this.nullValue && this.dateTimePicker.value.getTime() === this.nullValue.getTime()) {
      this.elementRef.nativeElement.querySelector('input').value = '';
    }
  }

  handleBlur() {
    this.adjustDateTime(this.dateTimePicker.value);
    if (this.dateTimePicker.value && this.nullValue && this.dateTimePicker.value.getTime() === this.nullValue.getTime()) {
      this.elementRef.nativeElement.querySelector('input').value = '';
    }
    this.onBlur.emit();
  }

  onClose() {
    if (this.dateTimePicker.value && this.nullValue && this.dateTimePicker.value.getTime() === this.nullValue.getTime()) {
      this.elementRef.nativeElement.querySelector('input').value = '';
    }
  }

  onOpen() {
    if (this.dateTimePicker.value && this.nullValue && this.dateTimePicker.value.getTime() === this.nullValue.getTime()) {
      this.elementRef.nativeElement.querySelector('input').value = '';
    }
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  updateFormErrorVisualizer() {
    this.formErrorVisualizerService.handleFormControl(this.name, this.completeFormControlName, this.fg);
  }

  private adjustDateTime(event: Date) {
    if (event == null) {
      return;
    }

    let adjustedYear;

    adjustedYear = (event.getFullYear() < 1900) ? 1900 : ((event.getFullYear() > 2100) ? 2100 : event.getFullYear());
    // console.log(event.getFullYear() + " --> ", adjustedYear, event.getMonth(), event.getDate());
    this.fg.patchValue(new Date(adjustedYear, event.getMonth(), event.getDate(), event.getHours(), event.getMinutes()));
  }

}

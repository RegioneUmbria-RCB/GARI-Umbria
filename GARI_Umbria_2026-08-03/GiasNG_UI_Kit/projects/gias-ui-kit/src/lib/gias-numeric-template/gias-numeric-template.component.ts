import { AfterViewInit, Component, ElementRef, EventEmitter, Inject, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ControlContainer, FormControl, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { GiasFormErrorVisualizerService } from '../gias-form-error-visualizer/gias-form-error-visualizer.service';
import { recursiveParentName } from '../utils/recursive-parent-name';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from '../utils/gias-master.service';
import { enum_InputType } from '../utils/models';

@Component({
  standalone: false,
  selector: 'gias-numeric-template',
  templateUrl: './gias-numeric-template.component.html',
  styleUrls: ['./gias-numeric-template.component.css'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class GiasNumericTemplateComponent implements OnInit, OnDestroy, AfterViewInit {

  @Input() name: string;
  @Input() value: number;
  @Input() format: string;
  @Input() placeholder: string;
  @Input() decimals: number = null;
  @Input() obligatory: boolean;
  @Input() clearButton: boolean;
  @Input() isDisabled: boolean;
  @Input() readonly: boolean;
  @Input() giasFormControlName: string;
  @Input() parentGroup: FormGroup;
  @Input() maxValue: number = Number.MAX_SAFE_INTEGER;
  @Input() minValue: number = Number.MIN_SAFE_INTEGER;
  @Input() errorMsg: string;
  @Input() autoCorrect = false;
  @Input() step = 1;
  @Input() spinners: boolean = false;
  @Input() changeValueOnScroll: boolean = false;
  @Input() title: string;
  @Input() selectOnFocus: boolean = false;
  @Output() valueChange = new EventEmitter<number>();
  @Output() onBlur = new EventEmitter();
  @Output() onFocus = new EventEmitter();

  public signal$: Subject<void> = new Subject();

  fg: FormControl;
  completeFormControlName: string;
  inputType: enum_InputType;

  @ViewChild('Numeric', { static: false, read: ElementRef }) num: ElementRef;

  constructor(
    private rootFormGroup: FormGroupDirective,
    private formErrorVisualizerService: GiasFormErrorVisualizerService,
    @Inject(GIAS_MASTER_SERVICE_TOKEN) private giasMasterService: IGiasMasterService
  ) {
    this.giasMasterService.currentInputType.subscribe(it => this.inputType = it);
  }

  ngAfterViewInit() {

    //Disabilito il drag perchè ci sono dei problemi con le textbox vicine che viene aumentato in automatico il valore
    this.num.nativeElement.addEventListener("dragstart", (event) => {
      event.preventDefault();
    })
  }

  ngOnInit(): void {
    this.fg = this.rootFormGroup.form.controls[this.giasFormControlName] as FormControl;
    this.completeFormControlName = recursiveParentName(this.fg);
    if (this.value == undefined) { }

    this.fg.statusChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
      this.updateFormErrorVisualizer();
    });

    this.fg.valueChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
      this.updateFormErrorVisualizer();
    });

    this.updateFormErrorVisualizer();
  }

  updateFormErrorVisualizer() {
    this.formErrorVisualizerService.handleFormControl(this.name, this.completeFormControlName, this.fg);
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  changeValue(newValue: number) {
    this.value = newValue;
    this.valueChange.emit(this.value);
    this.updateFormErrorVisualizer();
  }

  Blur() {
    if (this.value == undefined && this.fg.value == undefined) {
      let newValue: number = this.minValue !== Number.MIN_SAFE_INTEGER ? this.minValue : (this.maxValue !== Number.MAX_SAFE_INTEGER ? this.maxValue : 0);
      this.fg.setValue(newValue);
      this.changeValue(newValue);
    }

    this.onBlur.emit();
    this.updateFormErrorVisualizer();
  }

  Focus() {
    this.onFocus.emit();
  }

  get validator() {
    if (this.fg.hasValidator(Validators.required)) {
      return true;
    } else {
      return false;
    }
  }

}

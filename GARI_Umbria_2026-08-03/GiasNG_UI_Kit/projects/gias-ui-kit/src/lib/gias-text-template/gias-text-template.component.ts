import { Component, EventEmitter, Inject, Input, OnDestroy, OnInit, Output, SkipSelf } from '@angular/core';
import { ControlContainer, FormControl, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { GiasFormErrorVisualizerService } from '../gias-form-error-visualizer/gias-form-error-visualizer.service';
import { recursiveParentName } from '../utils/recursive-parent-name';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from '../utils/gias-master.service';
import { enum_InputType } from '../utils/models';

@Component({
  standalone: false,
  selector: 'gias-text-template',
  templateUrl: './gias-text-template.component.html',
  styleUrls: ['./gias-text-template.component.scss'],
  viewProviders: [{
    provide: ControlContainer,
    useFactory: (container: ControlContainer) => container,
    deps: [[new SkipSelf(), ControlContainer]],
  }]
})
/** TextTemplate component*/
export class GiasTextTemplateComponent implements OnInit, OnDestroy {
  @Input() name: string;
  @Input() value: string;
  @Input() placeholder: string;
  @Input() obligatory: boolean;
  @Input() clearButton: boolean;
  @Input() isDisabled: boolean = false;
  @Input() giasFormControlName: string;
  @Input() parentGroup: FormGroup;
  @Input() maxlength: number;
  @Input() minlength: number;
  @Output() valueChange = new EventEmitter<string>();

  fg: FormControl;
  inputType: enum_InputType;
  public completeFormControlName: string;

  public signal$: Subject<void> = new Subject();

  constructor(
    private rootFormGroup: FormGroupDirective,
    private formErrorVisualizerService: GiasFormErrorVisualizerService,
    @Inject(GIAS_MASTER_SERVICE_TOKEN) private giasMasterService: IGiasMasterService
  ) {
    this.giasMasterService.currentInputType.subscribe(it => this.inputType = it);
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  ngOnInit(): void {
    this.fg = this.rootFormGroup.form.controls[this.giasFormControlName] as FormControl;
    if (this.fg) {
      this.completeFormControlName = recursiveParentName(this.fg);
    }

    //console.log(this.giasFormControlName, this.fg)
    if (this.value == undefined || this.value == '') {
      this.value = '';
    }

    this.fg?.statusChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
      this.updateFormErrorVisualizer();
    });

    this.fg?.valueChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
      this.updateFormErrorVisualizer();
    });

    this.updateFormErrorVisualizer();

  }

  updateFormErrorVisualizer() {
    this.formErrorVisualizerService.handleFormControl(this.name, this.completeFormControlName, this.fg);
  }

  changeValue(newValue: string) {
    this.value = newValue;
    this.valueChange.emit(this.value);
  }

  onBlur() {
    this.updateFormErrorVisualizer();
  }

  get validator() {
    if (this.fg?.hasValidator(Validators.required)) {
      return true;
    } else {
      return false;
    }
  }
}

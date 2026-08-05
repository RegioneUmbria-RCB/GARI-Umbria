import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, SkipSelf } from '@angular/core';
import { ControlContainer, FormControl, FormGroup } from '@angular/forms';
import { Subject, Subscription, takeUntil } from 'rxjs';

@Component({
  standalone: false,
  selector: 'gias-binary-button-group-template',
  templateUrl: './gias-binary-button-group-template.component.html',
  styleUrls: ['./gias-binary-button-group-template.component.css'],
  viewProviders: [{
    provide: ControlContainer,
    useFactory: (container: ControlContainer) => container,
    deps: [[new SkipSelf(), ControlContainer]],
  }]
})
export class GiasBinaryButtonGroupTemplateComponent implements OnInit, OnDestroy {
  @Input() value: boolean;
  @Input() giasFormControlName: string;
  @Input() disabledsibutton: boolean = false;
  @Input() disablednobutton: boolean = false;
  @Input() silabel: string;
  @Input() nolabel: string;
  @Output() valueChange = new EventEmitter<boolean>();

  fc: FormControl;
  public signal$: Subject<void> = new Subject();

  private subscription: Subscription;
  private form: FormGroup;

  constructor(public controlContainer: ControlContainer) { }

  ngOnInit(): void {
    this.form = <FormGroup>this.controlContainer.control;

    this.fc = <FormControl>this.form.get(this.giasFormControlName);
    if (this.fc) {
      this.value = this.fc.value;
      if (!this.fc?.enabled) {
        this.disablednobutton = true;
        this.disabledsibutton = true;
      }
    }

    this.subscription = this.form.valueChanges.subscribe((v) => {

      this.fc?.valueChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
        if (this.fc.enabled) {
          this.disablednobutton = false;
          this.disabledsibutton = false;
        } else {
          this.disablednobutton = true;
          this.disabledsibutton = true;
        }
      });

      this.fc?.statusChanges.pipe(takeUntil(this.signal$)).subscribe((val) => {
        if (this.fc.enabled) {
          this.disablednobutton = false;
          this.disabledsibutton = false;
        } else {
          this.disablednobutton = true;
          this.disabledsibutton = true;
        }
      });

    });
  }

  change(val: boolean) {
    this.value = val;
    if (this.fc) {
      this.fc.setValue(val);
    }
    this.valueChange.emit(val);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
    this.signal$.next();
    this.signal$.complete();
  }
}

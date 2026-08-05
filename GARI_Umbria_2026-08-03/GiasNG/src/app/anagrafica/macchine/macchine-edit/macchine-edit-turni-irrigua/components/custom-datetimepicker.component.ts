import { AfterContentChecked, Component, Input, OnDestroy, OnInit } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Subject, takeUntil } from "rxjs";

@Component({
  standalone: false,
  selector: 'app-custom-datetimepicker',
  templateUrl: './custom-datetimepicker.component.html',
//   styleUrls: ['./custom-datetimepicker.component.scss'],
  providers: []
})
export class CustomDateTimePickerComponent implements OnInit, OnDestroy {
  @Input() input: any;
  @Input() edit = false;
  @Input() field: string;

  @Input() formGroup: FormGroup;
  form: FormGroup;
  value: Date = new Date();

  private $signal = new Subject<boolean>();

  private get currentValue(): Date | null {
    return this.formGroup?.get(this.field)?.value ?? this.input[this.field] ?? null;
  }

  ngOnInit(): void {
    this.initForm();
  }

  ngOnDestroy(): void {
    this.$signal.next(true);
    this.$signal.complete();
  }

  private initForm(): void {
    this.form = new FormGroup({
        [this.field]: new FormControl(this.currentValue ?? new Date())
    });
    this.input[this.field] = this.form.get(this.field).value;
    this.form.get(this.field).valueChanges
      .pipe(takeUntil(this.$signal))
      .subscribe(value => this.formGroup.get(this.field).patchValue(value));
  }
  
}
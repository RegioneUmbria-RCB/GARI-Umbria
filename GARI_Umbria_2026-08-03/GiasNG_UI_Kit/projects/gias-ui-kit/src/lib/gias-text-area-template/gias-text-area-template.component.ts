import { Component, EventEmitter, Inject, Input, OnInit, Output, SkipSelf } from '@angular/core';
import { ControlContainer, FormControl, FormGroup, FormGroupDirective } from '@angular/forms';
import { TextAreaResize } from '@progress/kendo-angular-inputs';
import { Subscription } from 'rxjs';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from '../utils/gias-master.service';
import { enum_InputType } from '../utils/models';

@Component({
  standalone: false,
  selector: 'gias-text-area-template',
  templateUrl: './gias-text-area-template.component.html',
  styleUrls: ['./gias-text-area-template.component.css'],
  viewProviders: [{
    provide: ControlContainer,
    useFactory: (container: ControlContainer) => container,
    deps: [[new SkipSelf(), ControlContainer]],
  }]
})
export class GiasTextAreaTemplateComponent implements OnInit {
  @Input() name: string;
  @Input() value: string;
  @Input() placeholder: string;
  @Input() obligatory: boolean;
  @Input() isDisabled: boolean = false;
  @Input() giasFormControlName: string;
  @Input() parentGroup: FormGroup;
  @Input() resizable: TextAreaResize = 'vertical';
  @Output() valueChange = new EventEmitter<string>();

  Subs: Subscription = new Subscription();

  inputType: enum_InputType;
  constructor(@Inject(GIAS_MASTER_SERVICE_TOKEN) private masterService: IGiasMasterService,
    private rootFormGroup: FormGroupDirective) {

    this.Subs.add(this.masterService.currentInputType.subscribe((it) => {
      this.inputType = it;
    }));
  }

  ngOnInit(): void {
    let fg = this.rootFormGroup.form.controls[this.giasFormControlName] as FormControl;
    if (fg) {
      (<any>fg).label = this.name;
    }
  }

  changeValue(newValue: string) {
    this.value = newValue;
    this.valueChange.emit(this.value);
  }
}

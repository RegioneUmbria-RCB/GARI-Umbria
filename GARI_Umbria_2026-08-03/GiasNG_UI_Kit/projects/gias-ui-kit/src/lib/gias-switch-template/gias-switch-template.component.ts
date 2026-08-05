import { Component, EventEmitter, Inject, Input, OnInit, Output, SkipSelf } from '@angular/core';
import { ControlContainer } from '@angular/forms';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from '../utils/gias-master.service';
import { enum_InputType } from '../utils/models';

@Component({
  standalone: false,
  selector: 'gias-switch-template',
  templateUrl: './gias-switch-template.component.html',
  styleUrls: ['./gias-switch-template.component.css'],
  viewProviders: [{
    provide: ControlContainer,
    useFactory: (container: ControlContainer) => container,
    deps: [[new SkipSelf(), ControlContainer]],
  }]
})
export class GiasSwitchTemplateComponent implements OnInit {

  @Input() name: string;
  @Input() value: boolean;
  @Input() placeholder: string;
  @Input() isDisabled: boolean;
  @Input() giasFormControlName: string;
  @Input() siLabel: string;
  @Input() noLabel: string;
  @Input() isBlue = false;
  @Input() hideLabel = false;
  @Input() hideformfield = false;
  @Output() valueChange = new EventEmitter<boolean>();

  inputType: enum_InputType;
  constructor(
    @Inject(GIAS_MASTER_SERVICE_TOKEN) private giasMasterService: IGiasMasterService) {
    this.giasMasterService.currentInputType.subscribe((it) => {
      this.inputType = it;
    });
  }

  ngOnInit(): void {
    if (this.value === undefined) {
      this.value = false;
    }
  }

  changeValue(newValue: boolean) {
    this.value = newValue;
    this.valueChange.emit(this.value);
  }
}

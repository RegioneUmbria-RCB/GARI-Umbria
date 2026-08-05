import { Component, EventEmitter, Inject, Input, OnInit, Output, SkipSelf } from '@angular/core';
import { ControlContainer } from '@angular/forms';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from '../utils/gias-master.service';
import { enum_InputType } from '../utils/models';

@Component({
  standalone: false,
  selector: 'gias-checkbox-template',
  templateUrl: './gias-checkbox-template.component.html',
  styleUrls: ['./gias-checkbox-template.component.css'],
  viewProviders: [{
    provide: ControlContainer,
    useFactory: (container: ControlContainer) => container,
    deps: [[new SkipSelf(), ControlContainer]],
  }]
})
export class GiasCheckboxTemplateComponent implements OnInit {
  @Input() name: string;
  @Input() value: boolean;
  @Input() isDisabled: boolean;
  @Input() giasFormControlName: string;
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

  changeValue(newValue: Event) {
    this.value = (newValue.target as HTMLInputElement).checked;
    this.valueChange.emit((newValue.target as HTMLInputElement).checked);
  }
}

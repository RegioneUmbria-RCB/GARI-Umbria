import { Component, Input, TemplateRef } from '@angular/core';
import { faCircleInfo } from '@fortawesome/free-solid-svg-icons';

@Component({
  standalone: false,
  selector: 'gias-info-template',
  templateUrl: './gias-info-template.component.html',
  styleUrls: ['./gias-info-template.component.css']
})
export class GiasInfoTemplateComponent {

  @Input() title: string;
  @Input() size: any;
  @Input() tooltipTemplate: TemplateRef<any> = null;

  public faInfo = faCircleInfo;
}

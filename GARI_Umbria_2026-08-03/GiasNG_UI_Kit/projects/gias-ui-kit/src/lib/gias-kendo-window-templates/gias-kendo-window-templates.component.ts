import { AfterViewInit, Component, TemplateRef, ViewChild } from '@angular/core';
import { GiasIFrameWindowService } from '../gias-iframe-window/gias-iframe-window.service';

@Component({
  standalone: false,
  selector: 'gias-kendo-window-templates',
  templateUrl: './gias-kendo-window-templates.component.html',
  styleUrls: ['./gias-kendo-window-templates.component.css']
})
export class GiasKendoWindowTemplates implements AfterViewInit {

  @ViewChild('windowTitleBar') public windowTitleBar: TemplateRef<any>;

  public showMinimize: boolean = true;
  public showMaximize: boolean = true;
  public showRestore: boolean = true;
  public showClose: boolean = true;

  public title: string = '';

  constructor(
    private GiasIFrameWindowService: GiasIFrameWindowService
  ) {
    this.GiasIFrameWindowService.titleSub.subscribe(t => this.title = t);
    this.GiasIFrameWindowService.showMinimizeSub.subscribe(v => this.showMinimize = v);
    this.GiasIFrameWindowService.showMaximaizeSub.subscribe(v => this.showMaximize = v);
    this.GiasIFrameWindowService.showRestoreSub.subscribe(v => this.showRestore = v);
    this.GiasIFrameWindowService.showCloseSub.subscribe(v => this.showClose = v);
  }

  ngAfterViewInit(): void {
    this.GiasIFrameWindowService.setTemplateRef(this.windowTitleBar);
  }
}

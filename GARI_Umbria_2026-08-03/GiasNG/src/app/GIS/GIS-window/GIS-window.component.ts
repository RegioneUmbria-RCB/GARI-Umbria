import { Component, HostListener, Input, ViewChild, OnChanges } from '@angular/core';
import { WindowComponent } from '@progress/kendo-angular-dialog';
import { Constants, KendoWindowsService, WindowArgs } from 'app/Service';
import {SMARTPHONE_WIDTH} from '../../Model/CostantiPersonalizzate';
//import {makeRelatedInformation} from '@angular/compiler-cli/src/ngtsc/diagnostics';

@Component({
  standalone: false,
  selector: 'gis-window',
  templateUrl: './GIS-window.component.html',
  styleUrls: ['./GIS-window.component.css']
})
export class GISWindowComponent implements OnChanges {
  @ViewChild('window') window: WindowComponent;
  @Input() windowArgs: WindowArgs;

  moved = false;
  windowState: 'default' | 'minimized' | 'maximized' = 'default';

  constructor(private kendoWindowsService: KendoWindowsService) { }

  get getOpenState(): boolean {
    return this.windowArgs?.openState ?? false;
  }

  get isSmartphone(): boolean {
    return window.innerWidth < SMARTPHONE_WIDTH;
  }

  @HostListener('window:resize') onResize() {
    this.onDragEnd();
  }

  ngOnChanges(): void {
    if (this.windowArgs != null) {
      this.windowState = this.windowArgs.startMinimized ? 'minimized' : (this.windowArgs.state ?? 'default');
    }
  }

  close(): void {
    this.kendoWindowsService.close(this.windowArgs.windowType);
  }

  onDragEnd() {

    if (window && this.windowArgs) {

      const positionRight = window.innerWidth - this.windowArgs.width - Constants.offset;
      const positionTop = window.innerHeight - this.windowArgs.height - Constants.offset;

      if (this.windowArgs.top < Constants.offset) {
        this.windowArgs.top = Constants.offset;
      }

      if (this.windowArgs.top > positionTop) {
        this.windowArgs.top = positionTop;
      }

      if (this.windowArgs.left < Constants.offset) {
        this.windowArgs.left = Constants.offset;
      }

      if (this.windowArgs.left > positionRight) {
        this.windowArgs.left = positionRight;
      }

      // Scommentare per posizionare la finestra sempre a destra
      // if (!this.moved) {
      //      this.windowArgs.left = positionRight;
      // }

    }

  }

  onDragStart() {
    this.moved = true;
  }

  public setFocus() {
    if (this.getOpenState) {
      this.window?.focus();
    }
  }

  //protected readonly makeRelatedInformation = makeRelatedInformation;
}

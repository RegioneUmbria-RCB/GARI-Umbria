import { Component, OnInit } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-widget-grid',
  templateUrl: './widget-grid.component.html',
  styleUrls: ['./widget-grid.component.css']
})
export class WidgetGridComponent {
  public thumbnailSrc =
    "https://www.telerik.com/kendo-angular-ui-develop/components/layout/card/assets/rila.jpg";


  public dropTargets = ["A", "B", "C", "D"];
  public currentBox = "A";
  public enteredBox = "A";
  public btnText = "Press Me!";

  public handlePress(): void {
    this.btnText = "Drag Me!";
    console.log("handlePress");
  }

  public handleDragEnter(id: string): void {
    this.enteredBox = id;

    if (this.enteredBox !== this.currentBox) {
      this.btnText = "Drop Me!";
    }
  }

  public handleDragLeave(): void {
    this.enteredBox = "";
    this.btnText = "Drag Me!";
  }

  public handleDrop(id: string): void {
    this.currentBox = id;
  }

  public resetText(): void {
    this.btnText = "Press Me!";
  }

}

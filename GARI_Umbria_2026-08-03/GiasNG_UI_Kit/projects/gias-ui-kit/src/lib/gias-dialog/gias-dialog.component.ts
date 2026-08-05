import { Component } from '@angular/core';

@Component({
  standalone: false,
  selector: 'gias-dialog',
  templateUrl: './gias-dialog.component.html',
  styleUrls: ['./gias-dialog.component.scss']
})
export class GiasDialogComponent {
  public content?: string;
  public title?: string;
  public dialogType?: string;

  public height?: number | string;

  //@description:
  //Permette d'impostare come altezza massima del content 200 se l'altezza delle dialog è auto o non è specificata
  //per far renderizzare la scrollbar
  public CalculateHeight(): string {

    let height_calculated: string = "";

    if (typeof this.height === "string") {
      if (this.height === "" || this.height.toLowerCase() === "auto") {
        height_calculated = "200px";
      } else {
        height_calculated = this.height;
      }
    } else if (typeof this.height === "number") {
      height_calculated = this.height.toString() + "px";
    } else if (typeof this.height === "undefined") {
      height_calculated = "200px";
    }

    return height_calculated;

  }
}

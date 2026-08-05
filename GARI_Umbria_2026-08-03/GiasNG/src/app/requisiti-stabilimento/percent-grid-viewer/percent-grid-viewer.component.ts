import { Component, Input, OnInit } from "@angular/core";
import { CustomComponent } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-percent-grid-viewer',
  templateUrl: './percent-grid-viewer.component.html',
  styleUrls: ['./percent-grid-viewer.component.scss'],
  providers: []
})
export class PercentGridViewerComponent implements OnInit, CustomComponent {
  @Input() input: string;
  @Input() edit: boolean;
  @Input() field: string;

  val:number;

  show: boolean;

  ngOnInit(): void {
    if (this.input[this.field] != null){
      this.val = parseInt((<number>this.input[this.field]).toString());
    }else {
      this.val = 0;
    }
  }

  public colors = [
    {
      to: 25,
      color: "#e90000",
    },
    {
      from: 25,
      to: 50,
      color: "#b44b00",
    },
    {
      from: 50,
      to: 75,
      color: "#bfff00",
    },
    {
      from: 75,
      color: "#35f300",
    },
  ];

}

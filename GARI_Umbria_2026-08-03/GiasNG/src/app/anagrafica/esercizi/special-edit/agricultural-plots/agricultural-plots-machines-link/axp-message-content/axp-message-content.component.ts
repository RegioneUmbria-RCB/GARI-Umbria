import { Component, Input, OnInit } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-axp-message-content',
  templateUrl: './axp-message-content.component.html',
  styleUrls: ['./axp-message-content.component.css']
})
export class AxpMessageContentComponent implements OnInit {
  @Input() axpMultiLinkPlots: string[] = [];

  constructor() { }

  ngOnInit(): void {
  }

}

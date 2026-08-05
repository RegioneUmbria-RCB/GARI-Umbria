import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';

@Component({
  standalone: false,
  selector: 'btn-aggiungi',
  templateUrl: './aggiungi.component.html',
  styleUrls: ['./aggiungi.component.css']
})
export class AggiungiComponent implements OnInit {

  @Input() public disabilitaBtn: boolean = false;
  @Output() clickAggiungi = new EventEmitter<any>();

  constructor() { }

  ngOnInit(): void {
  }

  public BtnClick (){
    this.clickAggiungi.emit();
  }

}

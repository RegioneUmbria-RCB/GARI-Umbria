import { Component, HostBinding, Input, OnInit } from '@angular/core';

@Component({
  standalone: false,
  selector: 'gias-svg-icon-species-species',
  templateUrl: './svg-icon-species.component.html',
  styleUrls: ['./svg-icon-species.component.css']
})
export class SvgIconSpeciesComponent implements OnInit {
  public _path!: string;
  public show: boolean;

  @Input() veg_cod: number;
  @Input() type: 'outline-color' | 'outline-black' ;

  constructor() { }

  ngOnInit(): void {
    if (this.veg_cod > 0){
      this.show = true;
      if (this.type == 'outline-color'){
        this._path = "assets/custom-icons/icon-plant-species/png-transparent-ouline-color/" + this.veg_cod + "_STD.png"
      }
      if (this.type == 'outline-black'){
        this._path = "assets/custom-icons/icon-plant-species/png-transparent-outline-black/" + this.veg_cod + "_STD.png"
      }
    } else {
      this.show = false;
    }
  }

}

import { Component } from '@angular/core';
import { QdCService } from '../../service/qdc.service';

@Component({
  standalone: false,
  selector: 'app-costi-accessori',
  templateUrl: './costi-accessori.component.html',
  styleUrls: ['./costi-accessori.component.css']
})
export class CostiAccessoriComponent {

  constructor(public qdcservice: QdCService) { }

}

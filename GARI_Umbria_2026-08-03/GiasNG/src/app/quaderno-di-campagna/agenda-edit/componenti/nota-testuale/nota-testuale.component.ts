import { Component } from '@angular/core';
import { QdCService } from '../../service/qdc.service';

@Component({
  standalone: false,
  selector: 'app-nota-testuale',
  templateUrl: './nota-testuale.component.html',
  styleUrls: ['./nota-testuale.component.css']
})
export class NotaTestualeComponent {

  constructor(public qdcservice: QdCService) { }

}

import {Component, OnInit} from '@angular/core';
import {QdCService} from "../../../service/qdc.service";
import {QdCTestataService} from "../../../service/testata/testata.service";
import { Tipo_Attivita } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-data-operazione',
  templateUrl: './data-operazione.component.html',
  styleUrls: ['./data-operazione.component.css']
})
export class DataOperazioneComponent implements OnInit{

  constructor(public qdcservice: QdCService,
              public testataservice: QdCTestataService) { }

  ngOnInit() {

    //Abilito la Data se sto facendo un Reinnesco
    if(!this.qdcservice.TestataForm.get("Data").enabled && this.qdcservice.flag_Reinnesco)
      this.qdcservice.TestataForm.get("Data").enable({emitEvent: false});

  }

  /*
  * Mostra l'Ora solo se sono in una operazione QdC (esclude le ricette/brogliaccio e le Visite)
  * */
  public mostraOraOperazione(): boolean{

    let mostra: boolean = false;

    if(!this.qdcservice.mostraTestataVisita()
    ) {
      mostra = true;
    }

    return mostra;
  }

}

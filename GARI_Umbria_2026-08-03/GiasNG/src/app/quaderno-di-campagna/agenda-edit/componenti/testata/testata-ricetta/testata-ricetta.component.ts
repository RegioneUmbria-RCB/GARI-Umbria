import {Component, OnInit} from '@angular/core';
import {QdCService} from "../../../service/qdc.service";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {AgendaService} from "../../../../../Service/Agenda/Agenda.service";
import {Tipo_Ricetta} from "../../../../../Model/attivita/Attivita";

@Component({
  standalone: false,
  selector: 'app-testata-ricetta',
  templateUrl: './testata-ricetta.component.html',
  styleUrls: ['./testata-ricetta.component.scss']
})
export class TestataRicettaComponent implements OnInit {

  constructor(public qdcservice: QdCService,
              private agendaService: AgendaService) { }

  async ngOnInit() {

      //TODO Genero il Ricetta_Numero ma se cambio la data non è più aggioranto
      if(this.qdcservice.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write &&
         this.qdcservice.TestataForm.get("Ricetta_Numero").value === ""){

          let piva = this.qdcservice.getImpresa_Model().partitaIva;

          let data_operazione = this.qdcservice.TestataForm.get("Data").value.toLocaleString();

          let ricetta_numero = await this.agendaService.Ricetta_Numero_Default(piva,data_operazione);

          this.qdcservice.TestataForm.patchValue({
              Ricetta_Numero: ricetta_numero
          });

      }

      //Disabilito il flag invia ricetta,descrizione e date della testata ricetta se ho aperto una ricetta di tipo PUA
      if(this.qdcservice.get_Tipo_Ricetta() === Tipo_Ricetta.PianoDistribuzionePua){
          this.qdcservice.TestataForm.get("InviaRicetta").disable({emitEvent: false});
          this.qdcservice.TestataForm.get("Ricetta_Des").disable({emitEvent: false});
          this.qdcservice.TestataForm.get("Ricetta_Data_Da").disable({emitEvent: false});
          this.qdcservice.TestataForm.get("Ricetta_Data_A").disable({emitEvent: false});
      }

  }

}

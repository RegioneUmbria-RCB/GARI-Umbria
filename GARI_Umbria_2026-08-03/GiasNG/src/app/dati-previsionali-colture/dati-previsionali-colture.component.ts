import { Component, OnInit } from '@angular/core';
import { ObjParametriAgendaService } from '../Service/obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-dati-previsionali-colture',
  templateUrl: './dati-previsionali-colture.component.html',
  styleUrls: ['./dati-previsionali-colture.component.css']
})
export class DatiPrevisionaliColtureComponent implements OnInit {

  private objParametriAgenda: ObjParametriAgenda;

  constructor(
    private objParametriAgendaService: ObjParametriAgendaService
  ) { }

  ngOnInit(): void {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
  }

  public impresaSelezionata(): boolean {
    return this.objParametriAgenda.Piva != null && this.objParametriAgenda.Piva !== '';
  }

}

import { Component, Inject, OnInit } from '@angular/core';
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridOperatoriHttpService } from '../../service/grid-operatori/grid-operatori-http.service';
import { QdCService } from '../../service/qdc.service';

@Component({
  standalone: false,
  selector: 'app-grid-operatori',
  templateUrl: './grid-operatori.component.html',
  styleUrls: ['./grid-operatori.component.css'],
  providers: [
    ...generateGridProviders(GridOperatoriHttpService, GridOperatoriComponent)
  ]
})
export class GridOperatoriComponent implements OnInit {

  constructor(public qdcservice:QdCService,
                private gridpublicService: GridPublicService,
                @Inject(GRID_HTTP_TOKEN) public gridoperatorihttpService:GridOperatoriHttpService) { }

  ngOnInit(): void {

    this.qdcservice.GridOperatoriHttpService = this.gridoperatorihttpService;

    this.qdcservice.GridOperatoriPublicService = this.gridpublicService;
  }

    disabilitaBtnSalvaDefaultOperatori(){
        let disabilita = false;

        let Operazioni = this.qdcservice.TestataForm.get("Operazioni").getRawValue();

        if(this.gridpublicService?.giasGridComponent?.inLineModificaIsDisabled || !this.qdcservice.abilitaGrid ||
            !Operazioni || Operazioni.length === 0){
            disabilita = true;
        }

        return disabilita;
    }

    disabilitaBtnNuovoOperatore(): boolean{
      let disabilita = false;

      if(this.gridpublicService?.giasGridComponent?.inLineModificaIsDisabled || !this.qdcservice.abilitaGrid){
        disabilita = true;
      }

      return disabilita;
    }

}

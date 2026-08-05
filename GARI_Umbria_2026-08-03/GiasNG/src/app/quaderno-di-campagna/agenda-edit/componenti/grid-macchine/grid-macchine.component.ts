import { Component, Inject,OnInit } from '@angular/core';
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridMacchineHttpService } from '../../service/grid-macchine/grid-macchine-http.service';
import { QdCService } from '../../service/qdc.service';

@Component({
  standalone: false,
  selector: 'app-grid-macchine',
  templateUrl: './grid-macchine.component.html',
  styleUrls: ['./grid-macchine.component.css'],
  providers: [
    ...generateGridProviders(GridMacchineHttpService, GridMacchineComponent)
  ]
})
export class GridMacchineComponent implements OnInit {

  constructor(public qdcservice:QdCService,
              private gridpublicService: GridPublicService,
              @Inject(GRID_HTTP_TOKEN) public gridmacchinehttpService: GridMacchineHttpService) {}

  ngOnInit(): void {

      this.qdcservice.GridMacchineHttpService = this.gridmacchinehttpService;

      this.qdcservice.GridMacchinePublicService = this.gridpublicService;
  }

  disabilitaBtnSalvaDefaultMacchine(){
      let disabilita = false;

      let Operazioni = this.qdcservice.TestataForm.get("Operazioni").getRawValue();

      if(this.gridpublicService?.giasGridComponent?.inLineModificaIsDisabled || !this.qdcservice.abilitaGrid ||
          !Operazioni || Operazioni.length === 0){
          disabilita = true;
      }

      return disabilita;
  }

  disabilitaBtnNuovaMacchina(): boolean{
      let disabilita = false;

      if(this.gridpublicService?.giasGridComponent?.inLineModificaIsDisabled || !this.qdcservice.abilitaGrid){
          disabilita = true;
      }

      return disabilita;
  }

}

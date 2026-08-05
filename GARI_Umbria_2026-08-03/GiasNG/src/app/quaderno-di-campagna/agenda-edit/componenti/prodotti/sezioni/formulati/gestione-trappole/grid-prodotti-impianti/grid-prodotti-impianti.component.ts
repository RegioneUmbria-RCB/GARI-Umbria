import {Component, Inject, OnDestroy, OnInit, Optional} from '@angular/core';
import {
  GridProdottixImpiantiHttpService
} from "../../../../../../service/dettagli-formulati/gestione-trappole/grid-prodotti-impianti-http.service";
import {FormGroup, FormGroupDirective} from "@angular/forms";
import {QdCDettagliFormulatiService} from "../../../../../../service/prodotti/dettagli-formulati.service";
import {QdCService} from "../../../../../../service/qdc.service";
import {Subscription} from "rxjs";
import { generateGridProviders, GRID_HTTP_TOKEN, GridPublicService } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-grid-prodotti-impianti',
  templateUrl: './grid-prodotti-impianti.component.html',
  styleUrls: ['./grid-prodotti-impianti.component.css'],
  providers: [
    ...generateGridProviders(GridProdottixImpiantiHttpService, GridProdottiImpiantiComponent)
  ]
})
export class GridProdottiImpiantiComponent implements OnInit,OnDestroy {

  FormulatiForm: FormGroup;

  Subs = new Subscription();

  constructor(private parent: FormGroupDirective,
              @Optional() private QdCDettagliFormulati: QdCDettagliFormulatiService,
              private gridpublicService: GridPublicService,
              private qdcservice: QdCService,
              @Inject(GRID_HTTP_TOKEN) public gridprodottiximpiantihttpService: GridProdottixImpiantiHttpService) { }

  ngOnInit(): void {
    this.FormulatiForm = <FormGroup>this.parent.form;

    this.QdCDettagliFormulati.GridProdottiXImpiantiPublicService = this.gridpublicService;

    this.QdCDettagliFormulati.GridProdottiXImpiantiHttpService = this.gridprodottiximpiantihttpService;

    this.Subs.add(this.qdcservice.SubsRefreshGridProdottiXImpianti.subscribe(value=>{
      if(value.RicaricaGrid){
        this.gridprodottiximpiantihttpService.Flag_Calcola_Qta_Su_Impianti = value.Flag_Calcola_Qta_Su_Impianti;
        this.QdCDettagliFormulati.RicaricaGridProdottiXImpianti();
      }

    }));
  }

  ngOnDestroy() {
    this.Subs.unsubscribe();
  }

}

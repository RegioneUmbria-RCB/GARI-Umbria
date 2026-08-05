import { Component, OnInit } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridProdottiDaTrattareHttpService } from '../../service/grid-prodotti-da-trattare/grid-prodotti-da-trattare-http.service';
import { QdCService } from '../../service/qdc.service';
import { GridPublicService } from 'gias-kendo-grid';


@Component({
  standalone: false,
  selector: 'app-grid-prodotti-da-trattare',
  templateUrl: './grid-prodotti-da-trattare.component.html',
  styleUrls: ['./grid-prodotti-da-trattare.component.css'],
  providers: [...generateGridProviders(GridProdottiDaTrattareHttpService, GridProdottiDaTrattareComponent)]
})
export class GridProdottiDaTrattareComponent implements OnInit {
  constructor(
    private qdcService: QdCService,
    private gridPublicService: GridPublicService
  ) { }

  ngOnInit(): void {
    this.qdcService.GridProdottiDaTrattarePublicService = this.gridPublicService;
  }
}

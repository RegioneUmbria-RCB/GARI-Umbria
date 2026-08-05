import { Component, Inject, AfterViewInit, TemplateRef, ViewChild } from '@angular/core';
import { generateGridProviders, GRID_HTTP_TOKEN, KendoGridMasterDetailService } from 'gias-kendo-grid';
import { RisultatiAnalisiMagazzinoGridService } from './risultati-analisi-magazzino-grid.service';
import { SHARED_IMPORTS } from 'app/qualita-tracciabilita/qualita-tracciabilita.module';
import { DettaglioGiacenzaGridComponent } from './dettaglio-giacenza-grid/dettaglio-giacenza-grid.component';
import { TestGridMasterService } from 'app/Utility/Template/kendo-grid/test/grid-master-detail/test-grid-master-detail.service';

@Component({
  selector: 'app-risultati-analisi-magazzino',
  imports: [SHARED_IMPORTS, DettaglioGiacenzaGridComponent],
  templateUrl: './risultati-analisi-magazzino.component.html',
  styleUrl: './risultati-analisi-magazzino.component.css',
  providers: [...generateGridProviders(RisultatiAnalisiMagazzinoGridService, RisultatiAnalisiMagazzinoComponent, KendoGridMasterDetailService)]
})
export class RisultatiAnalisiMagazzinoComponent implements AfterViewInit {
  @ViewChild("TemplateDetail") templateDetail: TemplateRef<any> = null;

  constructor(@Inject(GRID_HTTP_TOKEN) private gridmasterservice: TestGridMasterService,) { }

  ngAfterViewInit() {
    this.gridmasterservice.masterdetailSettings.templateGridDetail = this.templateDetail;
  }
}

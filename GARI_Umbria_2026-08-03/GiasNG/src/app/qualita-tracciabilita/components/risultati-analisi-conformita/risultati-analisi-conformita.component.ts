import { Component, Inject, AfterViewInit, TemplateRef, ViewChild } from '@angular/core';
import { SHARED_IMPORTS } from 'app/qualita-tracciabilita/qualita-tracciabilita.module';
import { RisultatiAnalisiConformitaGridService } from './risultati-analisi-conformita-grid.service';
import { generateGridProviders, GRID_HTTP_TOKEN, KendoGridMasterDetailService } from 'gias-kendo-grid';
import { TestGridMasterService } from 'app/Utility/Template/kendo-grid/test/grid-master-detail/test-grid-master-detail.service';
import { DettaglioAnalisiGridComponent } from './dettaglio-analisi-grid/dettaglio-analisi-grid.component';

@Component({
  standalone: true,
  selector: 'app-risultati-analisi-conformita',
  imports: [SHARED_IMPORTS, DettaglioAnalisiGridComponent],
  templateUrl: './risultati-analisi-conformita.component.html',
  styleUrl: './risultati-analisi-conformita.component.css',
  providers: [...generateGridProviders(RisultatiAnalisiConformitaGridService, RisultatiAnalisiConformitaComponent, KendoGridMasterDetailService)]
})
export class RisultatiAnalisiConformitaComponent implements AfterViewInit {
  @ViewChild("TemplateDetail") templateDetail: TemplateRef<any> = null;

  constructor(@Inject(GRID_HTTP_TOKEN) private gridmasterservice: TestGridMasterService,) { }

  ngAfterViewInit() {
    this.gridmasterservice.masterdetailSettings.templateGridDetail = this.templateDetail;
  }

}

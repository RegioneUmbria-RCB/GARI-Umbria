import { Component, Inject } from '@angular/core';
import { TestGiasKendoGridConfigService } from './test-gias-kendo-grid-config.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from 'gias-ui-kit';

@Component({
  standalone: false,
  selector: 'app-test-gias-kendo-grid',
  templateUrl: './test-gias-kendo-grid.component.html',
  styleUrls: ['./test-gias-kendo-grid.component.scss'],
  providers: [...generateGridProviders(TestGiasKendoGridConfigService, TestGiasGiasKendoGridComponent)],
})
export class TestGiasGiasKendoGridComponent {
  constructor(@Inject(GIAS_MASTER_SERVICE_TOKEN) private giasKendoGridService: IGiasMasterService) {
    console.log("here");
    console.log(giasKendoGridService.link_API);
  }
}

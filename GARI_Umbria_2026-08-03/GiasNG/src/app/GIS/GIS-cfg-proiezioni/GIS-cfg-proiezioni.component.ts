import {Component, OnDestroy, ViewChild} from '@angular/core';
import { GisClient } from 'app/Service/api.service';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { map } from 'rxjs';
import { GISCfgProiezioniGridConfig } from './GIS-cfg-proiezioni-grid-config.service';
import { GisCfgProiezioniPermissionsService } from './GIS-cfg-proiezioni-permissions/GIS-cfg-proiezioni-permissions.service';
import {GISCfgProiezioniService} from './gis-cfg-proiezioni.service';
import {GISCfgProiezioniDataService} from './gis-cfg-proiezioni-data.service';

@Component({
  standalone: false,
  selector: 'gis-cfg-proiezioni',
  templateUrl: './GIS-cfg-proiezioni.component.html',
  styleUrls: ['./GIS-cfg-proiezioni.component.css'],
  providers: [
    ...generateGridProviders(GISCfgProiezioniGridConfig, GISCfgProiezioniComponent),
    GisCfgProiezioniPermissionsService,
    GISCfgProiezioniService,
    GISCfgProiezioniDataService
  ]
})
export class GISCfgProiezioniComponent {
  @ViewChild('kendoGrid') kendoGrid: GiasKendoGridComponent;

  algorithms$ = this.gisClient.gisLeggiAlgoritmiProiezione().pipe(map(res => res.RispostaStringa.Algoritmi));

  constructor(
    protected gisCfgProiezioniDataService: GISCfgProiezioniDataService,
    private gisClient: GisClient
  ) { }

  add(): void {
    this.gisCfgProiezioniDataService.selectedConfiguration = null;
    this.gisCfgProiezioniDataService.openConfigurationEditDialog = true;
  }

  closeAndReload(): void {
    this.gisCfgProiezioniDataService.openConfigurationEditDialog = false;
    this.gisCfgProiezioniDataService.openPermissionDialog = false;
    this.gisCfgProiezioniDataService.openCfgDialog = false;
    this.gisCfgProiezioniDataService.selectedConfiguration = null;
    this.kendoGrid.forceReload();
  }
}

import { Component, Input, OnChanges, ViewChild } from "@angular/core";
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { GISAttributiRasterMasksGridConfig, RasterMask } from "./GIS-attributi-raster-masks-grid-config.service";
import { LayerService } from "app/GIS/services/layer.service";
import { MascheraLayerRaster, TipologiaLayer } from "app/Service/api.service";
import { GisAttributiRasterMasksPermissionsService } from "./GIS-cfg-proiezioni-permissions/GIS-attributi-raster-masks-permissions.service";
import { GISRasterConfigurationWindowService } from "app/GIS/GIS-raster-configuration-window/GIS-raster-configuration-window.service";
import { Observable } from "rxjs";

@Component({
  standalone: false,
  selector: 'gis-attributi-raster-masks',
  templateUrl: './GIS-attributi-raster-masks.component.html',
  styleUrls: ['./GIS-attributi-raster-masks.component.css'],
  providers: [...generateGridProviders(GISAttributiRasterMasksGridConfig, GISAttributiRasterMasksComponent), GisAttributiRasterMasksPermissionsService]
})
export class GISAttributiRasterMasksComponent implements OnChanges {
  @ViewChild('kendoGrid') kendoGrid: GiasKendoGridComponent;

  @Input() raster: TipologiaLayer;

  masks$: Observable<MascheraLayerRaster[]>;
  openPermissionDialog: boolean = false;
  selectedMask: MascheraLayerRaster | null = null;

  constructor(
    private layerService: LayerService,
    private gisRasterConfigurationWindowService: GISRasterConfigurationWindowService,
    private gisAttributiRasterMasksPermissionsService: GisAttributiRasterMasksPermissionsService
  ) { }

  ngOnChanges(): void {
    this.masks$ = this.gisRasterConfigurationWindowService.getMasks$(+this.raster.id);
  }

  viewOnMap(dataItem: RasterMask): void {
    this.layerService.layerItemCenterMap(`${dataItem.layer}`);
  }

  editPermissions(dataItem: RasterMask, masks: MascheraLayerRaster[]): void {
    const mask = masks.find(x => x.maschera_cod == dataItem.mask_cod);
    if (mask == null) {
      return;
    }

    this.gisAttributiRasterMasksPermissionsService.resetAuths();
    this.selectedMask = mask;
    this.openPermissionDialog = true;
  }

  closeAndReload(): void {
    this.openPermissionDialog = false;
    this.selectedMask = null;
    this.kendoGrid.forceReload();
  }
}

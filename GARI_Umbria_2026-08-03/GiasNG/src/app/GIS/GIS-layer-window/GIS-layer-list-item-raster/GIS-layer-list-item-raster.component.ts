import { Component, Input, OnInit } from '@angular/core';
import { GisLayerColorPickerService } from 'app/GIS/GIS-layer-color-picker-window/GIS-layer-color-picker-window.service';
import { GisFixedLayerPropertyService } from 'app/GIS/GIS-fixed-layer-property-window/GIS-fixed-layer-property-window.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { TipologiaLayer, ObjOptionHTML_Out, MascheraLayerRaster } from 'app/Service/api.service';
import { Observable } from 'rxjs';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { GisToolbarService } from 'app/GIS/GIS-toolbar/gis-toolbar.service';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { TranslocoService } from '@jsverse/transloco';
import { GISRasterConfigurationWindowService } from 'app/GIS/GIS-raster-configuration-window/GIS-raster-configuration-window.service';
import { GoogleMapService } from 'app/GIS/google-map/google-map.service';
import { enum_FeatureProperty } from 'app/GIS/GIS-enum/GIS-feature';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { BaseLayerListItemComponent } from '../base-layer-list-item.component';
import { GISAnalisiMappeSatellitariWindowService } from 'app/GIS/GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-window.service';

@Component({
  standalone: false,
  selector: 'gis-layer-list-item-raster',
  templateUrl: './GIS-layer-list-item-raster.component.html',
  styleUrls: ['./GIS-layer-list-item-raster.component.css']
})
export class GISLayerListItemRasterComponent extends BaseLayerListItemComponent implements OnInit {
  @Input() public dataItem: TipologiaLayer = null;
  @Input() public borderTop: boolean;
  @Input() public type: ObjOptionHTML_Out;
  @Input() public numberOfFeatures: number;
  @Input() public numberOfVisibleFeatures: number;

  tipologiaLayerEntita = false;
  masks$: Observable<MascheraLayerRaster[]>;
  visible$: Observable<boolean>;
  tileLoader$: Observable<boolean>;

  constructor(
    private sharedDataService: SharedDataService,
    private gisRasterConfigurationWindowService: GISRasterConfigurationWindowService,
    private googleMapService: GoogleMapService,
    private permessiUtenteService: PermessiUtenteService,
    private giasDialogService: GiasDialogService,
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    pickerService: GisLayerColorPickerService,
    fixedLayerService: GisFixedLayerPropertyService,
    layerService: LayerService,
    gisToolbarService: GisToolbarService,
    kendoWindowsService: KendoWindowsService,
    translocoService: TranslocoService,
  ) {
    super(pickerService, fixedLayerService, layerService, gisToolbarService, kendoWindowsService, translocoService);
  }

  ngOnInit(): void {
    const item = this.dataItem;

    this.tipologiaLayerEntita = this.sharedDataService.selezionatoTipoLayerEntita();
    this.visible$ = this.gisRasterConfigurationWindowService.getVisibility$(+item.id);
    this.masks$ = this.gisRasterConfigurationWindowService.getMasks$(+item?.id);

    this.layerService.toggleLayerItemVisible(item, false); // Always hidden
  }

  toggleRasterVisible($event: Event, item: TipologiaLayer, masks: MascheraLayerRaster[], visible: boolean): void {
    $event.stopPropagation();

    if (!visible) {
      this.doSetVisible(item, masks, visible);
      return;
    }

    // this should never happen
    const satelliteActive = this.kendoWindowsService.getOpenState(WindowTypes.AnalisiMappeSatellitariWindow);
    if (satelliteActive) {
      this.giasDialogService.baseError('', 'gis.DisattivaAnimazioneDatiSatellitariPrimaDiAttivareCaricamentoRaster', true);
      return;
    }

    const rasterAlreadyOpen = this.gisRasterConfigurationWindowService.isSomeVisible();
    if (rasterAlreadyOpen) {
      this.giasDialogService.baseError('', 'gis.DisattivaIlCaricamentoRasterPrimaDiAttivarneUnAltro', true);
      return;
    }

    const hasPermissions = this.permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_Gestione_Parametri_Maschere_Raster, 0);
    if (!hasPermissions) {
      // If we have no masks throw error
      if (masks.length == 0) {
        this.giasDialogService.baseError('', 'gis.NonHaiIPermessiPerVisualizzareQuestoRaster', true);
        return;
      }

      // If all masks have no feature throw error
      let hasFeature = false;
      const layers = masks.map(mask => mask.LayerElementiGrafici_cod);

      this.googleMapService.googleMapWrapper.data.forEach(feature => {
        if (layers.includes(+feature.getProperty(enum_FeatureProperty.layer))) {
          hasFeature = true;
        }
      });

      if (!hasFeature) {
        this.giasDialogService.baseError('', 'gis.NonHaiIPermessiPerVisualizzareQuestoRaster', true);
        return;
      }
    }

    this.doSetVisible(item, masks, visible)
  }

  openRasterConfig($event: Event, item: TipologiaLayer): void {
    $event.stopPropagation();

    const title = `${this.translocoService.translate('gis.ImpostazioniLayers')} ${item.nome}`;
    const args = new WindowArgs(WindowTypes.RasterConfigurationWindow, true, title, null, 600, 340, undefined, undefined, true, true, true, false, false, false, true);
    args.additionalArgs = { raster: item };
    this.kendoWindowsService.open(WindowTypes.RasterConfigurationWindow, args, false);
  }

  private doSetVisible(item: TipologiaLayer, masks: MascheraLayerRaster[], visible: boolean): void {
    if (!visible) {
      this.gisRasterConfigurationWindowService.nextVisibility(+item.id, visible);
      super.toggleVisible(new Event('click'), item);
      return;
    }

    super.toggleVisible(new Event('click'), item);
    const hasMasks = masks.filter(x => x.isAttivaPerUtenteCorrente).length > 0;
    const hasPermissions = this.permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_Gestione_Parametri_Maschere_Raster, 0);
    if (hasMasks) {
      this.giasDialogService.baseInfo('', 'gis.SelezionaDeiPoligoniPerCaricareIlDatiRaster', true);
    } else if (hasPermissions) {
      this.giasDialogService.baseInfo('', 'gis.VerrannoCaricatiGliOverlayCompleti', true);
    } else {
      this.giasDialogService.baseError('', 'gis.NonSiHannoIPermessiPerCaricareIDatiRaster', true);
      return;
    }

    this.gisRasterConfigurationWindowService.nextVisibility(+item.id, visible);
  }
}

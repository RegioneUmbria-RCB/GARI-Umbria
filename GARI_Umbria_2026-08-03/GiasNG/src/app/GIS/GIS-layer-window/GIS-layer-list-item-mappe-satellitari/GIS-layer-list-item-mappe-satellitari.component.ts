import { Component, Input, OnInit } from '@angular/core';
import { LayerService } from 'app/GIS/services/layer.service';
import { MascheraLayerRaster, TipologiaLayer } from 'app/Service/api.service';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { GISAnalisiMappeSatellitariWindowService } from 'app/GIS/GIS-analisi-mappe-satellitari-window/GIS-analisi-mappe-satellitari-window.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { GISRasterConfigurationWindowService } from 'app/GIS/GIS-raster-configuration-window/GIS-raster-configuration-window.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { SatelliteOverlayService } from 'app/GIS/GIS-analisi-mappe-satellitari-window/satellite-overlay.service';

const TIME_DELTA_ANIMATION_SAT = 4;

@Component({
  standalone: false,
  selector: 'gis-layer-list-item-mappe-satellitari',
  templateUrl: './GIS-layer-list-item-mappe-satellitari.component.html',
  styleUrls: ['./GIS-layer-list-item-mappe-satellitari.component.css']
})
export class GISLayerListItemMappeSatellitariComponent implements OnInit {
  @Input() public dataItem: TipologiaLayer = null;

  masks$ = this.gisAnalisiMappeSatellitariWindowService.masks$;

  constructor(
    private layerService: LayerService,
    private kendoWindowsService: KendoWindowsService,
    private gisAnalisiMappeSatellitariWindowService: GISAnalisiMappeSatellitariWindowService,
    private giasDialogService: GiasDialogService,
    private gisRasterConfigurationWindowService: GISRasterConfigurationWindowService,
    private permessiUtenteService: PermessiUtenteService
  ) { }

  get themes(): boolean {
    return this.layerService.esistonoLayerConTemi;
  }

  ngOnInit(): void {
    const item = this.dataItem;

    if (item.id !== enum_LayerElementiGraficiStd.ANALISI_MAPPE_SATELLITARI) {
      throw new Error("Layer non di tipo analisi mappe satellitari")
    }

    this.prepareAnimation();

    const left = (window.innerWidth - Math.min(window.innerWidth, 800) - 50);
    const args = new WindowArgs(WindowTypes.AnalisiMappeSatellitariWindow, false, item.nome, null, Math.min(window.innerWidth, 800), 400, left, 100, false, false, true, false, false, false, window.innerWidth >= SMARTPHONE_WIDTH);
    this.kendoWindowsService.override(WindowTypes.AnalisiMappeSatellitariWindow, args);
  }

  toggleVisible($event: Event, item: TipologiaLayer, masks: MascheraLayerRaster[]): void {
    $event.stopPropagation();

    const rasterAlreadyOpen = this.gisRasterConfigurationWindowService.isSomeVisible();
    if (rasterAlreadyOpen) {
      this.giasDialogService.baseError('', 'gis.DisattivaIlCaricamentoRasterPrimaDiAttivareAnimazioneDatiSatellitari', true);
      return;
    }

    this.layerService.toggleLayerItemVisible(item, true);

    const hasMasks = masks.filter(x => x.isAttivaPerUtenteCorrente).length > 0;
    const hasPermissions = this.permessiUtenteService.getPermesso(enum_Security_Attivita.GIS_Gestione_Parametri_Maschere_Raster, 0);
    if (!hasMasks && !hasPermissions) {
      this.giasDialogService.baseError('', 'gis.NonSiHannoIPermessiPerCaricareIDatiSatellitari', true);
      return;
    }

    this.openWindow();

    this.gisAnalisiMappeSatellitariWindowService.nextExternalLoadOnPolygons(SatelliteOverlayService.applyOverlaysOnPolygons(this.permessiUtenteService, masks));
  }

  private prepareAnimation(): void {
    const firstOfTheYear = new Date(new Date().getFullYear(), 0, 1);
    const sixMonthsAgo = new Date();
    sixMonthsAgo.setMonth(sixMonthsAgo.getMonth() - TIME_DELTA_ANIMATION_SAT);

    this.gisAnalisiMappeSatellitariWindowService.nextDateFrom(new Date(Math.max.apply(null, [sixMonthsAgo, firstOfTheYear])));
    this.gisAnalisiMappeSatellitariWindowService.nextDateTo(new Date());
  }

  private openWindow(): void {
    const args = this.kendoWindowsService.getWindowArgs(WindowTypes.AnalisiMappeSatellitariWindow);
    args.additionalArgs = { originalTitle: this.dataItem.nome };
    this.kendoWindowsService.open(WindowTypes.AnalisiMappeSatellitariWindow, args, false);
  }
}

import { Component, Input, OnInit } from '@angular/core';
import { GisLayerColorPickerService } from 'app/GIS/GIS-layer-color-picker-window/GIS-layer-color-picker-window.service';
import { GisFixedLayerPropertyService } from 'app/GIS/GIS-fixed-layer-property-window/GIS-fixed-layer-property-window.service';
import { LayerService } from 'app/GIS/services/layer.service';
import { TipologiaLayer, ObjOptionHTML_Out } from 'app/Service/api.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { GisToolbarService } from 'app/GIS/GIS-toolbar/gis-toolbar.service';
import { KendoWindowsService } from 'app/Service';
import { enum_LayerElementiGraficiStd } from 'app/GIS/GIS-enum/GIS-layer-elementi-grafici';
import { TranslocoService } from '@jsverse/transloco';
import { BaseLayerListItemComponent } from '../base-layer-list-item.component';
import { MultiAziendaService } from 'app/GIS/services/multi-azienda.service';
import { GISModality } from 'app/GIS/GIS-enum/GIS-feature';

@Component({
    standalone: false,
    selector: 'gis-layer-list-item',
    templateUrl: './GIS-layer-list-item.component.html',
    styleUrls: ['./GIS-layer-list-item.component.css']
})
export class GISLayerListItemComponent extends BaseLayerListItemComponent implements OnInit {
    @Input() public dataItem: TipologiaLayer = null;
    @Input() public borderTop: boolean;
    @Input() public type: ObjOptionHTML_Out;
    @Input() public numberOfFeatures: number;
    @Input() public numberOfVisibleFeatures: number;
    @Input() public modality: GISModality = GISModality.Full;

    // private defaultIcon = '/assets/custom-icons/icons-gis-layers/x04_CampoNew.png';
    tipologiaLayerEntita = false;
    fixedLayer = false;
    fixedLayerWms = false;

    constructor(
        private sharedDataService: SharedDataService,
        pickerService: GisLayerColorPickerService,
        fixedLayerService: GisFixedLayerPropertyService,
        layerService: LayerService,
        gisToolbarService: GisToolbarService,
        kendoWindowsService: KendoWindowsService,
        translocoService: TranslocoService,
        private multiAziendaService: MultiAziendaService,
    ) {
        super(pickerService, fixedLayerService, layerService, gisToolbarService, kendoWindowsService, translocoService);
    }

    ngOnInit(): void {
        this.tipologiaLayerEntita = this.sharedDataService.selezionatoTipoLayerEntita();
        this.fixedLayer = +this.dataItem.id < 0;
        this.fixedLayerWms = this.tipologiaLayerEntita && this.dataItem.id === enum_LayerElementiGraficiStd.WMS;
    }

    openThemeWindow($event: Event, item: TipologiaLayer | null) {
        $event.stopPropagation();
        this.layerService.toggleLayerItemSelected(item, true);
        if (this.modality != GISModality.Full) {
            let size = this.multiAziendaService.themeBarWidgetStyle;
            this.gisToolbarService.themeBtnToggle(true, size.width, size.side);
        } else {
            this.gisToolbarService.themeBtnToggle();
        }
    }
}

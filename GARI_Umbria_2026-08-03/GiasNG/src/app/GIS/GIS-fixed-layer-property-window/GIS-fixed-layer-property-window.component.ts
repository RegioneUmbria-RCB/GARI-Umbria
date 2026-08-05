import { Component, OnDestroy } from '@angular/core';
import { OutputFormat } from '@progress/kendo-angular-inputs';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { GisFixedLayerPropertyWindowArgs } from './GIS-fixed-layer-property-window.service';
import { SharedDataService } from '../services/shared-data.service';
import { FixedLayerProperty } from 'app/Model/GIS/FixedLayerProperty';
import { LayerService } from '../services/layer.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
    standalone: false,
    selector: 'gis-fixed-layer-property-window',
    templateUrl: './GIS-fixed-layer-property-window.component.html',
    styleUrls: ['./GIS-fixed-layer-property-window.component.css']
})
export class GISFixedLayerPropertyWindowComponent implements OnDestroy {
    public selectedColor: string;
    public windowArgs: GisFixedLayerPropertyWindowArgs;
    public colorOutputFormat: OutputFormat = 'hex';

    public sliderTrasparenzaValue: string;
    private sliderTrasparenzaIniziale: string;
    private uscitaConSalvataggio = false;
    public infoClickMappaToggle = false;
    public infoClickMappaToggleIniziale = false;

    private signal = new Subject<void>();

    constructor(
        private kendoWindowsService: KendoWindowsService,
        private sharedDataService: SharedDataService,
        private layerService: LayerService
        ) {
        this.kendoWindowsService.windowToggle$
            .pipe(takeUntil(this.signal)).subscribe(([windowTypes, args]) => {
            if (windowTypes === WindowTypes.FixedLayerPropertyWindow) {
                this.windowArgs = args as GisFixedLayerPropertyWindowArgs;
                this.selectedColor = '#' + this.windowArgs.inputColor;
                if (args.openState) {
                    this.uscitaConSalvataggio = false;
                    this.sliderTrasparenzaIniziale = this.layerService.getSliderTransparencyByHex(this.selectedColor);
                    this.sliderTrasparenzaValue = this.sliderTrasparenzaIniziale;
                    const fixedLayerProperty = this.sharedDataService.getFixedLayerProperty();
                    if (fixedLayerProperty.InfoClickMappa !== undefined) {
                        this.infoClickMappaToggle = fixedLayerProperty.InfoClickMappa;
                    }
                    this.infoClickMappaToggleIniziale = this.infoClickMappaToggle;
                } else {
                    if (!this.uscitaConSalvataggio) {
                        this.impostaFixedLayerProperty(this.sliderTrasparenzaIniziale, this.infoClickMappaToggleIniziale);
                    }
                }
            }
        });
    }

    ngOnDestroy() {
        this.signal.next();
        this.signal.complete();
    }

    public onChangeTrasparenza(trasparenzaValue: string) {
        this.impostaFixedLayerProperty(trasparenzaValue, this.infoClickMappaToggle);
    }

    public onChangeInfoClickMappa(e: any) {
        this.impostaFixedLayerProperty(this.sliderTrasparenzaValue, e.checked);
    }

    impostaFixedLayerProperty(trasparenza: string, infoClickMappa: boolean) {
        const fxLayerProperty = new FixedLayerProperty();
        // Trasparenza + Colore
        fxLayerProperty.Trasparenza = parseInt(trasparenza);
        this.selectedColor = this.layerService.getWhiteHexColorBySliderTransparency(trasparenza);
        // Info Click Mappa
        fxLayerProperty.InfoClickMappa = infoClickMappa;
        // Set
        this.sharedDataService.setFixedLayerProperty(fxLayerProperty);
    }

    public submit(): void {
        this.uscitaConSalvataggio = true;
        this.windowArgs.selectedColor.next(this.selectedColor);
        this.kendoWindowsService.close(this.windowArgs.windowType);
    }

}

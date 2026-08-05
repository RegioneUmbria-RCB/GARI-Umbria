
import { Component, OnDestroy, OnInit } from '@angular/core';
import { OutputFormat, GradientSettings } from '@progress/kendo-angular-inputs';
import { KendoWindowsService, WindowTypes } from 'app/Service';
import { Subject, takeUntil } from 'rxjs';
import { ContestoColore, GisLayerColorPickerWindowArgs } from './GIS-layer-color-picker-window.service';
import { TranslocoService } from '@jsverse/transloco';

@Component({
    standalone: false,
    selector: 'gis-layer-color-picker-window',
    templateUrl: './GIS-layer-color-picker-window.component.html',
    styleUrls: ['./GIS-layer-color-picker-window.component.css']
})
export class GISLayerColorPickerWindowComponent implements OnDestroy {
    public selectedColor: string;
    public windowArgs: GisLayerColorPickerWindowArgs;
    public colorOutputFormat: OutputFormat = 'hex';
    public colorGradientSettings: GradientSettings = {
        opacity: true
    };
    public etichettaBottone: string;

    private signal = new Subject<void>();

    constructor(
        private kendoWindowsService: KendoWindowsService,
        private translocoService: TranslocoService
    ) {

        this.kendoWindowsService.windowToggle$
            .pipe(takeUntil(this.signal)).subscribe(([windowTypes, args]) => {
            if (windowTypes === WindowTypes.ColorPickerWindow) {
                this.windowArgs = args as GisLayerColorPickerWindowArgs;
                if (args.openState) {
                    this.selectedColor = '#' + this.windowArgs.inputColor;
                    this.seVisualizzaBarraOpacita();
                    if (this.windowArgs.contesto === ContestoColore.Layer) {
                        this.etichettaBottone = this.translocoService.translate('gis.SalvaColore');
                    } else {
                        this.etichettaBottone = this.translocoService.translate('gis.SelezionaColore');
                    }
                } else {
                    if (this.windowArgs.contesto === ContestoColore.Tema) {
                        this.colorGradientSettings = {opacity: true};
                    }
                }

            }
        });
    }

    ngOnDestroy() {
        this.signal.next();
        this.signal.complete();
    }

    public onChangeColor(color: string): void {
        this.selectedColor = color;
    }

    public submit(): void {
        this.windowArgs.selectedColor.next(this.selectedColor);
        this.kendoWindowsService.close(this.windowArgs.windowType);
    }

    private seVisualizzaBarraOpacita() {
        if (this.windowArgs.contesto === ContestoColore.Layer && !this.colorGradientSettings.opacity) {
            this.colorGradientSettings = {opacity: true};
        }
        if (this.windowArgs.contesto === ContestoColore.Tema && this.colorGradientSettings.opacity) {
            this.colorGradientSettings = {opacity: false};
        }
    }

}

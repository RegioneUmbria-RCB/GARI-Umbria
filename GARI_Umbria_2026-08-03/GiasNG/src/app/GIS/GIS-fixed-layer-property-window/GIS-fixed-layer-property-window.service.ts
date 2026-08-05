import { Injectable } from '@angular/core';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { Observable, Subject } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';

export class GisFixedLayerPropertyWindowArgs extends WindowArgs {
    public inputColor: string;
    public selectedColor: Subject<string>;
}

@Injectable()
export class GisFixedLayerPropertyService {

    constructor(
        private windowsService: KendoWindowsService,
        private translocoService: TranslocoService
    ) { }

    public open(inputColor: string): Observable<string> {
        const [top, left, width, height] = this.getWindowArgs();
        const args = new GisFixedLayerPropertyWindowArgs(
            WindowTypes.FixedLayerPropertyWindow,
            false,
            this.translocoService.translate('gis.GestioneWms'),
            height,
            width,
            undefined,
            left,
            top
        );
        args.inputColor = inputColor;
        args.selectedColor = new Subject<string>();
        args.isMinimizeHidden = true;
        args.isMaximizeHidden = true;
        args.isRestoreHidden = true;

        this.windowsService.open(WindowTypes.FixedLayerPropertyWindow, args);
        return args.selectedColor.asObservable();
    }

    private getWindowArgs(): [number, number, number, number] {
        const openedWindow = this.windowsService.getWindowArgs(WindowTypes.FixedLayerPropertyWindow);
        if (openedWindow != null) {
            return [openedWindow.top, openedWindow.left, openedWindow.width, openedWindow.height];
        }

        const height = 225;
        const width = 350;

        const layerWindow = this.windowsService.getWindowArgs(WindowTypes.LayerWindow);
        if (layerWindow != null) {
            return [layerWindow.top, layerWindow.left - width - 10, width, height];
        }

        return [800, undefined, width, height];
    }
}

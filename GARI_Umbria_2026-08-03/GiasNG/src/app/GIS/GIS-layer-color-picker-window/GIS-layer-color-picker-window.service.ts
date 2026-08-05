import { Injectable } from '@angular/core';
import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { Observable, Subject } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';

export enum ContestoColore {
    Layer = 1,
    Tema = 2
}

export class GisLayerColorPickerWindowArgs extends WindowArgs {
    public inputColor: string;
    public selectedColor: Subject<string>;
    public contesto: ContestoColore;
}

@Injectable()
export class GisLayerColorPickerService {

    constructor(
        private windowsService: KendoWindowsService,
        private translocoService: TranslocoService
    ) { }

    public open(
        inputColor: string,
        contesto: ContestoColore
    ): Observable<string> {
        const [top, left, width, height] = this.getWindowArgs(contesto);
        const titolo = this.getTitolo(contesto);
        const args = new GisLayerColorPickerWindowArgs(
            WindowTypes.ColorPickerWindow,
            false,
            titolo,
            height,
            width,
            undefined,
            left,
            top
        );
        args.inputColor = inputColor;
        args.selectedColor = new Subject<string>();
        args.contesto = contesto;
        args.isMinimizeHidden = true;
        args.isMaximizeHidden = true;
        args.isRestoreHidden = true;

        this.windowsService.open(WindowTypes.ColorPickerWindow, args);
        return args.selectedColor.asObservable();
    }

    private getTitolo(contesto: ContestoColore): string {
        if (contesto === ContestoColore.Layer) {
            return this.translocoService.translate('gis.ColoreLayer');
        } else {
            return this.translocoService.translate('gis.ColoreTema');
        }
    }

    private getWindowArgs(contesto: ContestoColore): [number, number, number, number] {
        const openedWindow = <GisLayerColorPickerWindowArgs>this.windowsService.getWindowArgs(WindowTypes.ColorPickerWindow);

        // if (openedWindow != null && contesto === openedWindow?.contesto) {
        //     return [openedWindow.top, openedWindow.left, openedWindow.width, openedWindow.height];
        // }

        let height = 490;
        let width = 350;
        let top = 800;
        let left = undefined;

        if(window.innerWidth <= 991) {
            width = window.innerWidth;
            height = window.innerHeight/2;
            top = window.innerHeight/2;
            left = 1;
        }

        if (contesto === ContestoColore.Layer) {
            const layerWindow = this.windowsService.getWindowArgs(WindowTypes.LayerWindow);
            if (layerWindow != null) {
                return [layerWindow.top, layerWindow.left - width - 10, width, height];
            }
        } else {
            const themeWindow = this.windowsService.getWindowArgs(WindowTypes.ThemeWindow);
            if (themeWindow != null) {
                return [themeWindow.top, themeWindow.left + themeWindow.width + 10, width, height];
            }
        }

        return [top, left, width, height];
    }
}

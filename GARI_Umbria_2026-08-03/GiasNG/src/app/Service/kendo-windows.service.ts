import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { Constants } from "./utils/constants";
import { WindowArgs } from "./utils/windowArgs";
import { WindowTypes } from "./utils/windowTypes";

@Injectable({
    providedIn: 'root'
})
export class KendoWindowsService {
    private windows = new Map<WindowTypes, WindowArgs>();
    private _windowToggle$ = new Subject<[WindowTypes, WindowArgs]>();
    private _windowToggleMap$ = new Map<WindowTypes, Subject<WindowArgs>>();

    constructor() {
        Object.values(WindowTypes).forEach((windowType) => {
            this._windowToggleMap$.set(windowType as WindowTypes, new Subject<WindowArgs>());
        });
    }

    public get windowToggle$(): Observable<[WindowTypes, WindowArgs?]> {
        return this._windowToggle$.asObservable();
    }

    public getWindowArgs$(windowType: WindowTypes): Observable<WindowArgs> {
        return this._windowToggleMap$.get(windowType).asObservable();
    }

    public getOpenState(windowType: WindowTypes): boolean {
        return this.windows.get(windowType)?.openState ?? false;
    }

    public getWindowArgs(windowType: WindowTypes): WindowArgs | undefined {
        return this.windows.get(windowType);
    }

    public setTitle(windowType: WindowTypes, title: string) {
        const args = this.getWindowArgs(windowType);
        args.title = title;
        this.windows.set(windowType, args);
    }

    public open(windowType: WindowTypes, windowArgs?: WindowArgs, checkOverlap: boolean = true) {
        if (checkOverlap) {
            this.checkOverlap(windowArgs);
        }

        this.toggleOpenState(windowType, true, windowArgs);
    }

    public close(windowType: WindowTypes) {
        this.toggleOpenState(windowType, false, null);
    }

    public reset() {
        this.windows.clear();
    }

    public override(windowType: WindowTypes, windowArgs?: WindowArgs): void {
        this.windows.set(windowType, windowArgs);
        this._windowToggle$.next([windowType, windowArgs]);
        this._windowToggleMap$.get(windowType).next(windowArgs);
    }

    private toggleOpenState(windowType: WindowTypes, open: boolean, windowArgs?: WindowArgs): void {
        const args = windowArgs ?? this.getWindowArgs(windowType) ?? new WindowArgs(windowType, true);
        args.openState = open;
        this.windows.set(windowType, args);
        this._windowToggle$.next([windowType, args]);
        this._windowToggleMap$.get(windowType).next(windowArgs);
    }

    private checkOverlap(newWindowArgs: WindowArgs): void {
        this.windows.forEach((args) => {
            if (newWindowArgs == null || args.windowType === newWindowArgs.windowType) {
                return;
            }

            if (args.openState) {
                if ((newWindowArgs.left >= args.left && newWindowArgs.left <= args.left + args.width) || (newWindowArgs.left + newWindowArgs.width >= args.left && newWindowArgs.left + newWindowArgs.width <= args.left + args.width)) {
                    let rightSpace = window.innerWidth - args.left - args.width - Constants.offset - 10;
                    if (newWindowArgs.width > rightSpace) {
                        newWindowArgs.left = args.left - newWindowArgs.width - 10;
                        return;
                    }

                    newWindowArgs.left = args.left + args.width + 10;
                }

                //controllo per top?
            }
        });
    }
}

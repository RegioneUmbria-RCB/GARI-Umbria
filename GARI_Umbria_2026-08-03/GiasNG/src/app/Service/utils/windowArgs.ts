import { WindowSettings } from "@progress/kendo-angular-dialog";
import { WindowTypes } from "./windowTypes";

export class WindowArgs extends WindowSettings {
    additionalArgs: any;

    constructor(
        public windowType: WindowTypes,
        public openState: boolean,
        title?: string,
        height?: number,
        width?: number,
        minWidth?: number,
        left?: number,
        top?: number,
        public isRestoreHidden?: boolean,
        public isMinimizeHidden?: boolean,
        public isMaximizeHidden?: boolean,
        public isCloseHidden?: boolean,
        public isPinnable?: boolean,
        public startMinimized?: boolean,
        public isDraggable?: boolean
    ) {
        super();

        // these fields are inherited from super class
        this.title = title;
        this.height = height;
        this.width = width ?? 450;
        this.minWidth = minWidth ?? 250;
        this.left = left;
        this.top = top;

        this.isRestoreHidden = isRestoreHidden ?? false;
        this.isMinimizeHidden = isMinimizeHidden ?? false;
        this.isMaximizeHidden = isMaximizeHidden ?? false;
        this.isCloseHidden = isCloseHidden ?? false;
        this.isPinnable = isPinnable ?? true;
        this.startMinimized = startMinimized ?? false;
        this.isDraggable = isDraggable ?? true;
    }
}

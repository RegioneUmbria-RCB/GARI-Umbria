import { Component, EventEmitter, Output } from "@angular/core";
import { enum_GISDrawingOperations } from "../../GIS-enum/GIS-drawing-operations";

@Component({
    standalone: false,
    selector: 'wms-window',
    templateUrl: './wms-window.component.html',
    styleUrls: ['./wms-window.component.css']
})
export class WMSWindowComponent {
    @Output() operationEvent = new EventEmitter<any>();

    opened: boolean = true;

    GISDrawingOperations = enum_GISDrawingOperations

    setOperation(op: enum_GISDrawingOperations) {
        this.operationEvent.emit(op);
    }

    public toggle(isOpened: boolean): void {
        //this.opened = isOpened;
    }
}

import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';

const MIN_WIDTH_IN_PERCENTAGE = 20;
const MAX_WIDTH_IN_PERCENTAGE = 90;

@Component({
    standalone: false,
    selector: 'app-draggable',
    templateUrl: './draggable.component.html',
    styleUrls: ['./draggable.component.scss'],
    encapsulation: ViewEncapsulation.None,
})
export class DraggableComponent {

    containerWidth: number = 440;
    @Input() minWidth: number = (MIN_WIDTH_IN_PERCENTAGE / 100) * window.innerWidth;
    @Input() maxWidth: number = (MAX_WIDTH_IN_PERCENTAGE / 100) * window.innerWidth;
    readonly DRAGGABLE_BAR_OFFSET = 10;

    @Output() updateContainerWidth: EventEmitter<number> = new EventEmitter();


    forwardNewComponentWidth(currentWidth: number) {
        let result = 0;
        if(this.minWidth && currentWidth < this.minWidth)
          result = this.minWidth;
        else if(this.maxWidth && currentWidth > this.maxWidth)
          result = this.maxWidth;
        else
          result = currentWidth;

        this.updateContainerWidth.emit(result);
    }

}

export interface DraggableInterface {
    resizableElementWidth: number;
}

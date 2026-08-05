import { Directive, ElementRef, HostListener, Input, NgZone, OnInit, Renderer2 } from '@angular/core';
import { DraggableComponent } from './draggable.component';

@Directive({ standalone:false,
  selector: '[draggable]'
})
export default class DraggableDirective implements OnInit {

    isResizing: boolean = false;
    selectedElement: any;

    constructor(private el: ElementRef, private _renderer: Renderer2,
                private parent: DraggableComponent,
                private zone: NgZone) { }

    ngOnInit(): void {
        this._renderer.setAttribute(this.el.nativeElement, 'draggable', 'true');

        this.zone.runOutsideAngular(() => {
            window.document.addEventListener('mousemove', this.onMouseMove.bind(this));
            window.document.addEventListener('mouseup', this.onMouseUp.bind(this));
          });
    }


    @HostListener('document:mousedown', ['$event'])
    public onMouseClick(event) {
        if (event.target.getAttribute('draggable')) {
            this.selectedElement = event.target;
            this.isResizing = true;
            event.preventDefault();
        }
    }

    onMouseMove(event) {
        if(!this.isResizing)
            return;
        this.parent.containerWidth = event.pageX;
        this.parent.forwardNewComponentWidth(event.pageX);

    }

    onMouseUp(event) {
        this.isResizing = false;
        this.selectedElement = null;
    }
}

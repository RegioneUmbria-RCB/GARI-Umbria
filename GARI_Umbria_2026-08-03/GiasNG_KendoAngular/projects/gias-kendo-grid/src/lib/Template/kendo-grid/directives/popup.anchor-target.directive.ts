// https://www.telerik.com/kendo-angular-ui/components/grid/editing/custom-reactive-editing/
import { Directive, ElementRef } from '@angular/core';

@Directive({
    standalone: false,
    selector: '[popupAnchor]',
    exportAs: 'popupAnchor',
})
export class PopupAnchorDirective {
    constructor(public element: ElementRef) { }
}

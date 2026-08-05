import { Directive, TemplateRef } from '@angular/core';

@Directive({
    standalone: false,
    selector: '[kendoCustomColumnContent]'
})
export class KendoGridCustomColumnDirective {

    constructor(public templateRef: TemplateRef<any>) {
    }

}

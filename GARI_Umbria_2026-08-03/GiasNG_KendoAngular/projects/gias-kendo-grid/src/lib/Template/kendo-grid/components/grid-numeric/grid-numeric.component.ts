import { Component, Input } from '@angular/core';

@Component({
    standalone: false,
    selector: 'gias-grid-numeric',
    templateUrl: './grid-numeric.component.html',
    styleUrls: ['./grid-numeric.component.css'],
})
export class GridNumericComponent {

    @Input() value: number;
    @Input() min: number;
    @Input() max: number;

}

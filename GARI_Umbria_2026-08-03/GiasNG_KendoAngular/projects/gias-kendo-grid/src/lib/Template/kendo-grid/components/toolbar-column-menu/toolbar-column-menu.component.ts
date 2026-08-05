import { Component, Input } from '@angular/core';
import { KendoGridColumn } from '../../models/grid.model';

@Component({
    standalone: false,
    selector: 'gias-toolbar-column-menu',
    templateUrl: './toolbar-column-menu.component.html',
    styleUrls: ['./toolbar-column-menu.component.css']
})
export class ToolbarColumnMenuComponent {
    @Input() columns: KendoGridColumn[];
    onChange(item: KendoGridColumn) {
        item.hidden = !item.hidden;
    }

}

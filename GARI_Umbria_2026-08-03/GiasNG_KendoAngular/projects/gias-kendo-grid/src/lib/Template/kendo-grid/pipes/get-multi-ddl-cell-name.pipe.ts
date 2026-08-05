import { Pipe, PipeTransform } from '@angular/core';
import { GridErrorService } from '../services/grid-log.service';
import { KendoGridColumn, KendoGridRow } from '../models/grid.model';
import { DropdownHelper } from '../components/grid-multi-dropdownlist/grid-multi-dropdown.service';

@Pipe({
    standalone: false,
    name: 'cellMultiDropdownName',
    pure: true
})
export class CellMultiDropdownNamePipe implements PipeTransform {

    constructor(public logService: GridErrorService) { }


    transform(row: KendoGridRow, column: KendoGridColumn): any {
        let helper = new DropdownHelper();
        let cellOutput = helper.gridCellClosedOuput(column, row);

        if (cellOutput.columnDataNeedsUpdate) {
            column.ddl.data = cellOutput.items;
        }

        let itemNames = cellOutput.items.map(item => item.name)

        let commaSeparatedResult = itemNames.join(" | ");


        return commaSeparatedResult;
    }
}


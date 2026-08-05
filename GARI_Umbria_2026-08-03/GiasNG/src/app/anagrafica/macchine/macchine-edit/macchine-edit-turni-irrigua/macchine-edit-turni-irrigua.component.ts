import { Component } from "@angular/core";
import { generateGridProviders } from "gias-kendo-grid";
import { TurniIrriguaGridService } from "./service/turni-irrigua-grid.service";

@Component({
    standalone: false,
    selector: 'app-macchine-edit-turni-irrigua',
    templateUrl: './macchine-edit-turni-irrigua.component.html',
    providers:[
        ...generateGridProviders(TurniIrriguaGridService, MacchineEditTurniIrriguaComponent)
    ]
  })
  
  export class MacchineEditTurniIrriguaComponent {
  
    onCellClick(event: any) {
        if (!event.isEditedRow)
            event.sender.closeRow(event.rowIndex);
    }
  }
  
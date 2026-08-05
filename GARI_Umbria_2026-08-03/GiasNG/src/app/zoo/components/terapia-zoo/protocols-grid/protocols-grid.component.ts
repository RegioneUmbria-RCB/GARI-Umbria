import { Component, Inject, Input, OnInit, ViewChild } from "@angular/core";
import { GRID_HTTP_TOKEN, generateGridProviders } from "gias-kendo-grid";
import { ProtocolsGridService } from "./protocols-grid.service";
import { FormArray } from "@angular/forms";

@Component({
  standalone: false,
  selector: 'app-protocols-grid',
  templateUrl: './protocols-grid.component.html',
  styleUrls: ['./protocols-grid.component.css'],
  providers: [
    ...generateGridProviders(ProtocolsGridService, ProtocolsGridComponent)
  ]
})
export class ProtocolsGridComponent implements OnInit {

  @Input() protocolsFormArray: FormArray;

  selectedProtocolsCount: number = 0;

  public protocolsGridData: FormArray;

  constructor(
    @Inject(GRID_HTTP_TOKEN) protected gridService: ProtocolsGridService
  ) {
    this.gridService.gridpublicService.subscribe(grid => {
      if (grid?.data) {
        this.selectedProtocolsCount = (!this.gridService.terapiaFormService.isInfoMode) ? (grid.data?.rows.filter((row: any) => row.Selected).length ?? 0) : (grid.data?.rows.length ?? 0);
      }
    });
  }

  ngOnInit() {
    this.gridService.currentProtocolsFormArray = this.protocolsFormArray;
    if (this.protocolsFormArray) this.selectedProtocolsCount = this.protocolsFormArray.controls.length;
    this.gridService.gridpublicService.refresh(true);
  }

  get gridSelectedRows(): Array<any> {
    return this.protocolsFormArray?.controls.map(c => c.value) ?? [];
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
    }
  }

}

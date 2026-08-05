import { Component, EventEmitter, Input, OnChanges, Output, ViewChild } from "@angular/core";
import { GISParticelleCatastaliGridConfigService } from "./GIS-particelle-catastali-grid-config.service";
import { generateGridProviders } from 'gias-kendo-grid';
import { GISParticelleCatastaliModel, GISParticelleCatastaliService } from "./GIS-particelle-catastali.service";
import { GiasKendoGridComponent } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'gis-particelle-catastali',
  templateUrl: './GIS-particelle-catastali.component.html',
  styleUrls: ['./GIS-particelle-catastali.component.css'],
  providers: [...generateGridProviders(GISParticelleCatastaliGridConfigService, GISParticelleCatastaliComponent)]

})
export class GISParticelleCatastaliComponent implements OnChanges {
  @ViewChild('kendoGrid') kendoGrid: GiasKendoGridComponent;

  @Input() piva: string;
  @Input() existings: string[];

  @Output() onAdd = new EventEmitter<GISParticelleCatastaliModel[]>();

  constructor(private gisParticelleCatastaliService: GISParticelleCatastaliService) { }

  ngOnChanges(): void {
    this.gisParticelleCatastaliService.nextPiva(this.piva);
    this.gisParticelleCatastaliService.nextExisting(this.existings ?? []);
  }

  submit(): void {
    const selected: GISParticelleCatastaliModel[] = [];
    for (const row of this.kendoGrid.rows) {
      if (row['Selected']) {
        selected.push(row as GISParticelleCatastaliModel);
        row['Selected'] = false;
      }
    }

    this.onAdd.emit(selected);
  }
}

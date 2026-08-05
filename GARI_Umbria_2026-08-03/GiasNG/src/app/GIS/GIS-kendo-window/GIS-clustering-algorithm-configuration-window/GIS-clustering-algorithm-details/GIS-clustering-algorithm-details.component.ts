import { Component, EventEmitter, Inject, Input, OnChanges, Output, SimpleChanges } from "@angular/core";
import { GISClusteringAlgorithmDetailsGridConfigService } from "./GIS-clustering-algorithm-details-config.service";
import { generateGridProviders, GRID_HTTP_TOKEN } from "gias-kendo-grid";

@Component({
  standalone: false,
  selector: 'gis-clustering-algorithm-details',
  templateUrl: './GIS-clustering-algorithm-details.component.html',
  styleUrls: ['./GIS-clustering-algorithm-details.component.css'],
  providers: [...generateGridProviders(GISClusteringAlgorithmDetailsGridConfigService, GisClusteringAlgorithmDetailsComponent)]
})
export class GisClusteringAlgorithmDetailsComponent implements OnChanges {
  @Input() parameters: string | null = null;
  @Output() saveEvent = new EventEmitter<string | null>();

  parsedParameters: { name: string, value: string }[] = [];

  constructor(@Inject(GRID_HTTP_TOKEN) private gridService: GISClusteringAlgorithmDetailsGridConfigService) {
    this.gridService.outputData
      .subscribe(data => this.save(data));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['parameters']) {
      this.gridService.inputData.next(this.parameters || '');
    }
  }

  save(data): void {
    const result = JSON.stringify(data);
    this.saveEvent.emit(result);
  }

  addParameter(): void {
    this.parsedParameters = [
      ...this.parsedParameters,
      { name: '', value: '' }
    ];
  }

  removeParameter(index: number): void {
    this.parsedParameters = this.parsedParameters.filter((_, i) => i !== index);
  }
}

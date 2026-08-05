import { Component, Inject, OnDestroy } from "@angular/core";
import { generateGridProviders, GRID_HTTP_TOKEN, HttpAction } from "gias-kendo-grid";
import { GISClusteringAlgorithmGridConfigService } from "./GIS-clustering-algorithm-grid-config.service";
import { Subject, take, takeUntil } from "rxjs";
import { GisClusterConfigDetail_InData, GisClusterConfigSave_InData } from "app/Service/net-core6-api.service";

@Component({
  standalone: false,
  selector: 'gis-clustering-algorithm-grid',
  templateUrl: './GIS-clustering-algorithm-grid.component.html',
  styleUrls: ['./GIS-clustering-algorithm-grid.component.css'],
  providers: [...generateGridProviders(GISClusteringAlgorithmGridConfigService, GisClusteringAlgorithmGridComponent)]
})
export class GisClusteringAlgorithmGridComponent implements OnDestroy {
  private signal$ = new Subject<void>();

  editingData: GisClusterConfigSave_InData | null = null;

  constructor(@Inject(GRID_HTTP_TOKEN) private gridService: GISClusteringAlgorithmGridConfigService) {
    this.gridService.onParameterUpdate$
      .pipe(takeUntil(this.signal$))
      .subscribe(e => {
        if (e.details == null || e.details.length === 0) {
          e.details = [{ id: 1, Description: '', Parameters: JSON.stringify([]) } as GisClusterConfigDetail_InData];
        }

        this.editingData = e;
      });
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  save(updated: string | null): void {
    if (this.editingData) {
      this.editingData.details = [{ id: 1, Description: '', Parameters: updated } as GisClusterConfigDetail_InData];
      this.gridService
        .perform(HttpAction.UPDATE, this.editingData)
        .pipe(take(1))
        .subscribe();
    }
  }
}

import { Component, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { generateGridProviders } from 'gias-kendo-grid';
import { CarburantiGridConfigService } from '../../services/carburanti-grid-config.service';

@Component({
  standalone: false,
  selector: 'app-carburanti-grid',
  templateUrl: './carburanti-grid.component.html',
  styleUrls: ['./carburanti-grid.component.scss'],
  providers: [
    ...generateGridProviders(CarburantiGridConfigService, CarburantiGridComponent)
  ]
})
export class CarburantiGridComponent implements OnDestroy {

  private destroy$ = new Subject<void>();

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

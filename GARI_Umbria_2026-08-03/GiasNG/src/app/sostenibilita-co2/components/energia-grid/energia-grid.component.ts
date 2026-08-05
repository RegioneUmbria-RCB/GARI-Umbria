import { Component, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { generateGridProviders } from 'gias-kendo-grid';
import { EnergiaGridConfigService } from '../../services/energia-grid-config.service';

@Component({
  standalone: false,
  selector: 'app-energia-grid',
  templateUrl: './energia-grid.component.html',
  styleUrls: ['./energia-grid.component.scss'],
  providers: [
    ...generateGridProviders(EnergiaGridConfigService, EnergiaGridComponent)
  ]
})
export class EnergiaGridComponent implements OnDestroy {

  private destroy$ = new Subject<void>();

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

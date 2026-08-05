import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { PerimetroFilters } from '../models/rischi-h20.model';

@Injectable()
export class FiltersPerimetroH20Service {
  private filtersSubject = new Subject<PerimetroFilters>();
  readonly filters$ = this.filtersSubject.asObservable();

  readonly applyRequest$ = new Subject<void>();

  publishFilters(filters: PerimetroFilters): void {
    this.filtersSubject.next(filters);
  }

  requestApply(): void {
    this.applyRequest$.next();
  }
}

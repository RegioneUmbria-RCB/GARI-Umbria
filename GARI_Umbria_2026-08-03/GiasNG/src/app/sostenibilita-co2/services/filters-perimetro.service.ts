import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { PerimetroFilters } from '../models/sostenibilita-co2.model';

@Injectable()
export class FiltersPerimetroService {
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

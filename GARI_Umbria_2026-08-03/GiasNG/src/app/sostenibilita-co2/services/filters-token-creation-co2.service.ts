import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { TokenCreationFilters } from '../models/sostenibilita-co2.model';

@Injectable()
export class FiltersTokenCreationCo2Service {
  private filtersSubject = new Subject<TokenCreationFilters>();
  readonly filters$ = this.filtersSubject.asObservable();

  readonly applyRequest$ = new Subject<void>();

  lastFilters: TokenCreationFilters | null = null;

  publishFilters(filters: TokenCreationFilters): void {
    this.lastFilters = filters;
    this.filtersSubject.next(filters);
  }

  requestApply(): void {
    this.applyRequest$.next();
  }
}

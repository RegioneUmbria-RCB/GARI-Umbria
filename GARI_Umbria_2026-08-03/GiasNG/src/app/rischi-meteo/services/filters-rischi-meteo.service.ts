import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class FiltersRischiMeteoService {
  private pivaSubject = new Subject<string | null>();
  readonly pivaFiliera$ = this.pivaSubject.asObservable();

  readonly applyRequest$ = new Subject<void>();

  publishFiliera(pivaFiliera: string | null): void {
    this.pivaSubject.next(pivaFiliera);
  }

  requestApply(): void {
    this.applyRequest$.next();
  }
}

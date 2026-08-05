import { Component, Input } from '@angular/core';
import { Subject, BehaviorSubject, takeUntil, switchMap, tap, debounceTime, Observable, map } from 'rxjs';
import { ComparisonWidgetService } from './comparison-widget.service';

@Component({
  standalone: false,
  selector: 'app-comparison-widget',
  templateUrl: './comparison-widget.component.html',
  styleUrls: ['./comparison-widget.component.scss']
})
export class ComparisonWidgetComponent {
  @Input() piva: string | null = null;
  @Input() index: string | null = null;

  private signal$ = new Subject<void>();

  yearsSubject = new BehaviorSubject<number[]>([]);
  data: ComparisonWidgetData[][] = [];
  validYears$: Observable<number[]>;
  loading = true;

  constructor(private comparisonWidgetService: ComparisonWidgetService) { }

  ngOnInit(): void {
    if (this.piva == null || this.index == null) {
      this.loading = false;
      return;
    }

    this.loading = true;
    this.yearsSubject
      .pipe(
        takeUntil(this.signal$),
        debounceTime(200),
        switchMap(years => this.comparisonWidgetService.getData$(this.piva, years)),
        tap(() => this.loading = false)
      )
      .subscribe(data => this.data = ComparisonWidgetService.getIndexData(data, this.index));

    this.validYears$ = this.comparisonWidgetService
      .getYears$(this.piva)
      .pipe(
        map(years => years.sort((a, b) => (a - b)).reverse()),
        tap(years => this.yearsSubject.next(years.slice(0, 2)))
      );
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }
}

export interface ComparisonWidgetData {
  index: number;
  name: string;
  year: string;
  color: string;
}

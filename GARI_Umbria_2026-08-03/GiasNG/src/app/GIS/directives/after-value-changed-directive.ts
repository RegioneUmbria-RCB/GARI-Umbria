import { Directive, Input, HostListener, OnDestroy, Output, EventEmitter } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Directive({ standalone:false,
  selector: '[afterValueChanged]'
})
export class AfterValueChangedDirective implements OnDestroy {
  @Output()
  public afterValueChanged: EventEmitter<number> = new EventEmitter<number>();

  @Input()
  public valueChangeDelay = 300;

  private stream: Subject<number> = new Subject<number>();
  private subscription: Subscription;

  constructor() {
    this.subscription = this.stream
      .pipe(debounceTime(this.valueChangeDelay))
      .subscribe((value: number) => this.afterValueChanged.next(value));
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  @HostListener('valueChange', [ '$event' ])
  public onValueChange(value: number): void {
    this.stream.next(value);
  }
}

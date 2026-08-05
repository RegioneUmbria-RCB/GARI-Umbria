import {Directive, Input} from '@angular/core';
import {BehaviorSubject, ReplaySubject, Subject} from 'rxjs';
import {KendoGridRow} from 'gias-kendo-grid';

@Directive()
export abstract class SpecialEditBaseComponent<T> {
  @Input() protected listViewHeight$: ReplaySubject<number> = new ReplaySubject<number>(1);
  @Input() protected checkedRows$: ReplaySubject<KendoGridRow[]> = new ReplaySubject<KendoGridRow[]>(1);
  protected rows$: BehaviorSubject<T[]> = new BehaviorSubject<T[]>([]);
  protected removeDataItem$: Subject<{ index: number, deleteCount: number; }> = new Subject<{ index: number; deleteCount: number; }>();
  protected resetStates$: Subject<boolean> = new Subject();
  protected loading$: ReplaySubject<boolean>;
}

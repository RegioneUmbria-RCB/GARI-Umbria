import { Injectable } from "@angular/core";
import { FiltroTemporaleAvanzato } from "app/Utility/date-utils";
import { Subject, Observable } from "rxjs";

@Injectable()
export class AdvancedTimeFilterService {
  private _advancedTimeFilter: Subject<FiltroTemporaleAvanzato>;

  constructor() {
    this._advancedTimeFilter = new Subject<FiltroTemporaleAvanzato>();
  }

  public getAdvancedTimeFilter(): Observable<FiltroTemporaleAvanzato> {
    return this._advancedTimeFilter.asObservable();
  }

  public setAdvancedTimeFilter(filter: FiltroTemporaleAvanzato) {
    this._advancedTimeFilter.next(filter);
  }
}

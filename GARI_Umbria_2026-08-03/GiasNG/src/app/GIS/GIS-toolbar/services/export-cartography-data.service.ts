import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';

@Injectable()
export class ExportCartographyDataService {

  private _saCod: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  constructor() { }

  set saCod(value: number) {
    this._saCod.next(value);
  }

  get saCod(): number {
    return this._saCod.getValue();
  }
}

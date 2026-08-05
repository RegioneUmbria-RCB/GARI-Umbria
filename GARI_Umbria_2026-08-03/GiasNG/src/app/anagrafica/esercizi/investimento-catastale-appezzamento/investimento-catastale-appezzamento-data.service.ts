import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {LeggiInvestimentoCatastale} from '../../../Service/ServiceFactory/investimento-catastale.factory.service';

@Injectable({providedIn: 'root'})
export class InvestimentoCatastaleAppezzamentoDataService {
  private _leggiInvestimentoCatastale$: BehaviorSubject<LeggiInvestimentoCatastale> = new BehaviorSubject<LeggiInvestimentoCatastale>(new LeggiInvestimentoCatastale());

  constructor() { }

  get leggiInvestimentoCatastale(): LeggiInvestimentoCatastale {
    return this._leggiInvestimentoCatastale$.getValue();
  }

  set leggiInvestimentoCatastale(value: LeggiInvestimentoCatastale) {
    this._leggiInvestimentoCatastale$.next(value);
  }
}

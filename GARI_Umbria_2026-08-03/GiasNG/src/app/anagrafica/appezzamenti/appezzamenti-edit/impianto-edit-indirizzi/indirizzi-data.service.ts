import { Injectable } from '@angular/core';
import { IndirizzoAssociato } from '../../../../Model/anagrafiche/addresses/IndirizzoAssociato';
import {BehaviorSubject, Observable} from 'rxjs';

export class IndirizzoAssociatoChiave extends IndirizzoAssociato {
  chiave: number;
}

@Injectable()
export class IndirizziAppezzamentoDataService{
  private _indirizziAppezzamento: IndirizzoAssociatoChiave[] = [];
  private _indirizziAppezzamentoSource: BehaviorSubject<IndirizzoAssociatoChiave[]> = new BehaviorSubject(this._indirizziAppezzamento);

  get indirizziAppezzamentoSource$(): Observable<IndirizzoAssociatoChiave[]> {
    return this._indirizziAppezzamentoSource.asObservable();
  }

  set indirizziAppezzamento(indirizzi: IndirizzoAssociatoChiave[]) {
    for (const indirizzo of indirizzi) {
      if (indirizzo.chiave == undefined || indirizzo.chiave < 0) {
        indirizzo.chiave = this.getNextId(indirizzi);
      }
    }
    this._indirizziAppezzamentoSource.next(indirizzi);
  }

  get indirizziAppezzamento(): IndirizzoAssociatoChiave[] {
    return this._indirizziAppezzamentoSource.getValue();
  }

  private getNextId(costi: IndirizzoAssociatoChiave[]): number {
    let newChiave: number = 0;
    const chiave = Math.max.apply(Math, costi.map(function (o) {
      if (o.chiave != undefined) {
        return o.chiave;
      } else {
        return -1;
      }
    }));
    if (chiave >= 0) {
      newChiave = chiave + 1;
    }
    return newChiave;
  }
}

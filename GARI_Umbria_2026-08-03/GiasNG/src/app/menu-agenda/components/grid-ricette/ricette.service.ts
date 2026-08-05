import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs';
import { RicettaRow } from '../utils';

@Injectable()
export class RicetteService {
  private selected = new BehaviorSubject<RicettaRow[]>([]);

  public select(selected: RicettaRow[], deselected: RicettaRow[]): void {
    const result = this.getSelected();

    this.addToArray(selected, result);
    this.removeFromArray(deselected, result);

    this.selected.next(result);
  }

  public get selected$(): Observable<RicettaRow[]> {
    return this.selected.asObservable();
  }

  public getSelected(): RicettaRow[] {
    return this.selected.value;
  }

  public reset() {
    this.selected.next([]);
  }

  private removeFromArray(deselected: RicettaRow[], result: RicettaRow[]): void {
    for (const d of deselected) {
      const index = result.indexOf(d);
      if (index > -1) {
        result.splice(index, 1);
      }
    }
  }

  private addToArray(selected: RicettaRow[], result: RicettaRow[]): void {
    for (const s of selected) {
      if (result.find(x => x == s) == null) {
        result.push(s);
      }
    }
  }
}
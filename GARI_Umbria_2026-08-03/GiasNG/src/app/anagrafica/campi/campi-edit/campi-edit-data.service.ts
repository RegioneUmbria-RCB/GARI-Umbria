import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {CatastoCampoId} from './catasto-campo-edit/catasto-campo.service';

@Injectable()
export class CampiEditDataService {
  private _catastoCampo: BehaviorSubject<CatastoCampoId[]> = new BehaviorSubject<CatastoCampoId[]>([]);

  public get catastoCampo$(): Observable<CatastoCampoId[]> {
    return this._catastoCampo.asObservable();
  }

  public get catastoCampo(): CatastoCampoId[] {
    return this._catastoCampo.getValue();
  }

  public set catastoCampo(cc: CatastoCampoId[]) {
    for (const particella of cc) {
      if (particella.id == undefined || particella.id < 0) {
        particella.id = this.getNextId(cc);
      }
    }
    this._catastoCampo.next(cc);
  }

  private getNextId(particelle: CatastoCampoId[]): number {
    let newId = 0;
    const chiave = Math.max.apply(Math, particelle.map(function (o) {
      if (o.id != undefined) {
        return o.id;
      } else {
        return -1;
      }
    }));
    if (chiave >= 0) {
      newId = chiave + 1;
    }
    return newId;
  }

}

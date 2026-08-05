import {Injectable} from '@angular/core';
import {BehaviorSubject, Subject} from 'rxjs';
import {IConfrontoPianoColturale, PianoColturale} from '../../Model/confronto-piano-colturale/confronto-piano-colturale';
import {FormGroup} from '@angular/forms';
import {AGRODATAFINE, AGRODATAINIZIO} from "../../Model/CostantiPersonalizzate";
import {IntervalloTemporale} from '../../Model/anagrafiche/IntervalloTemporale';

@Injectable()
export class ConfrontoPianoColturaleDataService {
  private _pianoColturale: BehaviorSubject<PianoColturale> = new BehaviorSubject<PianoColturale>(new PianoColturale());
  private _confrontoPC: BehaviorSubject<IConfrontoPianoColturale> = new BehaviorSubject<IConfrontoPianoColturale>({
    considerCatasto: true,
    considerVarieta: true,
    validita: new IntervalloTemporale()
  });
  private _loadComparison: Subject<void> = new Subject<void>();

  public set pianoColturale(value: PianoColturale) {
    this._pianoColturale.next(value)
  }

  public get pianoColturale$() {
    return this._pianoColturale.asObservable();
  }

  public get pianoColturale() {
    return this._pianoColturale.getValue();
  }

  public set confrontoPC(value: IConfrontoPianoColturale) {
    this._confrontoPC.next(value);
  }

  public get confrontoPC$() {
    return this._confrontoPC.asObservable();
  }

  public get confrontoPC() {
    return this._confrontoPC.getValue();
  }

  public get loadComparison$() {
    return this._loadComparison.asObservable();
  }

  public emitLoadComparison(): void {
    this._loadComparison.next();
  }

}

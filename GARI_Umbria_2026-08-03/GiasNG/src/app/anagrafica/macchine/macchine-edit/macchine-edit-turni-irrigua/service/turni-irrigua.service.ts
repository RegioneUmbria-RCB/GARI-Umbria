import { Injectable } from "@angular/core";
import { RateoTempo } from "app/Model/anagrafiche/ParcoMacchine";
import { BehaviorSubject } from "rxjs";

@Injectable()
export class RateiTempoService {

  private _originalRateiTempo: RateoTempo[];
  private _hasBeenEdited: boolean = false;
  public rateiTempo: RateoTempo[] = new Array();
  public rateiTempoSource = new BehaviorSubject(this.rateiTempo);

  constructor() { }

  public init(rateiTempo: RateoTempo[]) {
    this._originalRateiTempo = rateiTempo;
  }

  public setRateiTempo(rateiTempo: RateoTempo[]) {
    this.rateiTempoSource.next(rateiTempo);
    this._hasBeenEdited = true;
  }

  public getRateiTempo(): RateoTempo[] {
    if (this._hasBeenEdited)
      return this.rateiTempoSource.getValue();
     else return this._originalRateiTempo ?? [];
  }
}
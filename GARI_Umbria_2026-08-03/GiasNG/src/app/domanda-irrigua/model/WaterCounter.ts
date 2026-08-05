import {BaseCodeDescr} from "../../Model/baseClass/baseCodeDescr";
import {IntervalloTemporale} from "../../Model/anagrafiche/IntervalloTemporale";

export class WaterCounter extends BaseCodeDescr {
  public modello: string;
  public targa: string;
  public validita = new IntervalloTemporale();

  constructor(code?: number, description?: string, model?: string, tag?: string) {
    super(code, description);
    this.modello = model ?? '';
    this.targa = tag ?? '';
  }
}

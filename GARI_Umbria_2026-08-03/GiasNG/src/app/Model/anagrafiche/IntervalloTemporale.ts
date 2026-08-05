import { AGRODATAFINE, AGRODATAINIZIO } from "../CostantiPersonalizzate";
import {IntervalloTemporale as IIntervalloTemporale} from "app/Service/api.service";
import {IntervalloTemporale as IIntervalloTemporaleNetCore} from "app/Service/net-core6-api.service";
import {IntervalloTemporale as IIIntervalloTemporaleNetCore} from "app/Service/qdca-compliance-api.service";
import {FormControl} from "@angular/forms";

export class IntervalloTemporale implements IIntervalloTemporale, IIntervalloTemporaleNetCore, IIIntervalloTemporaleNetCore {
  inizio: Date;
  fine: Date;

  constructor(inizio?: Date, fine?: Date) {
    this.inizio = inizio ?? AGRODATAINIZIO;
    this.fine = fine ?? AGRODATAFINE;
  }

  overlaps(validity: IntervalloTemporale): boolean {
    return this.inizio <= validity.fine && this.fine >= validity.inizio;
  }

  contains(date: Date): boolean {
    return this.inizio <= date && this.fine >= date;
  }
}


import {IntervalloTemporale} from '../anagrafiche/IntervalloTemporale';

export class Contribute {
  code: number;
  type: number;
  description: string;
  validity: IntervalloTemporale;

  constructor(code: number = 0, type: number = 0) {
    this.code = code;
    this.type = type;
    this.description = '';
    this.validity = new IntervalloTemporale();
  }
}

export class LinkedContribute<T> extends Contribute {
  linkValidity: IntervalloTemporale;
  linkedItemPK: T;

  constructor(code: number = 0, type: number = 0, linkedItemPK: T = undefined) {
    super(code, type);
    this.linkedItemPK = linkedItemPK;
  }
}

export enum ContributeType {
  Anything = 0,
  ACA = 1
}

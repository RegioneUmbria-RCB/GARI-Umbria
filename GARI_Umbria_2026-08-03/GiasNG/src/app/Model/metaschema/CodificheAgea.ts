import {BaseCodeDescrStr} from '../baseClass/baseCodeDescrStr';

export interface CodificaMacchineAgeaRequest {
  ageaCod: string;
  ageaDes: string;
  classCod: string;
}

export class MacchineCodificaAgea extends BaseCodeDescrStr{
  constructor(code: string = '') {
    super(code, '');
  }
}

// import {IntervalloTemporale} from "../../Model/anagrafiche/IntervalloTemporale";

import {IntervalloTemporale} from "../../Service/api.service";

export interface IUtente {
  UserName: string;
}

export interface IUtentePassword extends IUtente{
  Password: string;
}

export interface IUtenteFinestraTemp extends IUtente {
   FinestraTemporale: IntervalloTemporale;
}

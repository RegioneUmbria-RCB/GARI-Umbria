import { Rubrica } from './Rubrica';

export class RubricaVoci {
    rubrica: Rubrica;
    valore: string;
    flag_cancellazione: boolean;

    constructor() {
        this.flag_cancellazione = false;
    }
}

export class RubricaVociConChiave extends RubricaVoci {
    chiave: number;
}
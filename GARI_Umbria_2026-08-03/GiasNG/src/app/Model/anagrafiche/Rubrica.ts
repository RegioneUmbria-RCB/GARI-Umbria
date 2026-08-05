export class Rubrica {
    codice: number;
    tipologia: string;
    flag_cancellazione: boolean;

    constructor(codice: number) {
        this.codice = codice;
        this.flag_cancellazione = false;
    }
}

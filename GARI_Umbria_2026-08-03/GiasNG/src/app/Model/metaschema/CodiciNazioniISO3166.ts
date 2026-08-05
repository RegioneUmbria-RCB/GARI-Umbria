export class CodiciNazioniISO3166 {
    codice: string;
    descrizione: string;
    codiceNumerico: string;
    codiceAlpha3: string;
    gestioneGerarchia: number;

    constructor(codice: string, descrizione: string, codiceNumerico: string, codiceAlpha3: string, gestioneGerarchia: number) {
        this.codice = codice;
        this.descrizione = descrizione;
        this.codiceNumerico = codiceNumerico;
        this.codiceAlpha3 = codiceAlpha3;
        this.gestioneGerarchia = gestioneGerarchia;
    }
}

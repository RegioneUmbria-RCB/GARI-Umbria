export class SementieriParametrizzazione {
    Sementi: string;
    DatiPassaggio: string;
    SementiMappaturaLibera: string;

    constructor(sementi: any) {
        this.Sementi = sementi.Sementi;
        this.DatiPassaggio = sementi.DatiPassaggio;
        this.SementiMappaturaLibera = sementi.SementiMappaturaLibera;
    }

    static isInstanceOfSementieri(obj: any) {
        return obj?.Sementi != undefined && obj?.DatiPassaggio != undefined && obj?.SementiMappaturaLibera != undefined;
    }
}

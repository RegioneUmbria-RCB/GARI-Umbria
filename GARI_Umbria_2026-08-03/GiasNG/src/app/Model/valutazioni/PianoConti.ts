export type PKPianoConti = typeof PianoConti.PK.prototype;
export class PianoConti {
    primaryKey: PKPianoConti;
    Valutazione_Piano_Des: string;

    flag_cancellazione: boolean;

    constructor(primaryKey: PKPianoConti) {
        this.primaryKey = primaryKey;
        this.flag_cancellazione = false;
    }

    static PK = class {
        Piva: string;
        Valutazione_Piano_Cod: number;

        constructor(piva: string, codice: number) {
            this.Piva = piva;
            this.Valutazione_Piano_Cod = codice;
        }
    };
}
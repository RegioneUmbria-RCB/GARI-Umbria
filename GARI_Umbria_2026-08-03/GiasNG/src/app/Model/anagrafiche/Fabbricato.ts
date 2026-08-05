import { CentroAziendale, PKCentroAziendale } from './CentroAziendale';
import { Indirizzo } from './addresses/Indirizzo';

export type PKFabbricato = typeof Fabbricato.PK.prototype;
export class Fabbricato {
    primaryKey: PKFabbricato;
    descrizione: string;
    indirizzo: Indirizzo;
    tipo: number;
    flag_cancellazione: boolean;
    usoDaTerzi: boolean;

    public static PK = class FabbricatoPK{
        codice: number;
        centroAziendalePK: PKCentroAziendale;

        constructor(codice: number, centroAziendalePK: PKCentroAziendale) {
            this.codice = codice;
            this.centroAziendalePK = centroAziendalePK;
        }
    };

    constructor(primaryKey: PKFabbricato) {
        this.primaryKey = primaryKey;
    }
}

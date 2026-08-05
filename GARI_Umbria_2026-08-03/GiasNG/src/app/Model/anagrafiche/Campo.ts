import { AppezzamentoCampo } from 'app/anagrafica/campi/campi-edit/appezzamento-campo-edit/appezzamento-campi-edit';
import { Specie } from '../metaschema/utilizzi/Specie';
import { CatastoCampo } from './CatastoCampo';
import { PKCentroAziendale } from './CentroAziendale';
import { CodiciAnagrafeValori } from './CodiciAnagrafeValori';
import { IntervalloTemporale } from './IntervalloTemporale';

export type PKCampo = typeof Campo.PK.prototype;
export class Campo {

    descrizione: string;
    validita: IntervalloTemporale;
    campo_Codice: string;
    orientamento_Colturale: number;
    catastoCampo: CatastoCampo[];
    primaryKey: PKCampo;
    codici: CodiciAnagrafeValori[];
    specie: Specie;
    serra: boolean;
    cartografia: string;
    flag_gps: boolean;

    appezzamentoCampo: AppezzamentoCampo[];

    flag_cancellazione: boolean;

    constructor(primaryKey: PKCampo) {
        this.primaryKey = primaryKey;
        this.flag_cancellazione = false;
    }

    public static PK = class {
        centroAziendalePK: PKCentroAziendale;
        codice: number;

        constructor(codice: number, centroAziendalePK: PKCentroAziendale) {
            this.codice = codice;
            this.centroAziendalePK = centroAziendalePK;
        }
    };

    static Empty(piva: string, sa_cod: number = 0) {
        const campo = new Campo(new this.PK(0, { codice: sa_cod, partitaIva: piva}));
        campo.catastoCampo = [];
        campo.codici = [];
        campo.specie = new Specie(0);
        campo.validita = new IntervalloTemporale();
        return campo;
    }

}



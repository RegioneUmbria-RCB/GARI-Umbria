import {EsercizioCDC} from './EsercizioCDC';
import {Tipo} from './CentroDiCosto';
import {Esercizio} from '../../anagrafiche/Esercizio';

export class EsercizioRilievoCDC extends EsercizioCDC {
    dataRiferimento: Date;

    constructor() {
        super();
        this.classType = 'EsercizioRilievoCDC';
        this.tipo = Tipo.Esercizio;
    }

    setKey(piva: string, saCod: number, appezza: number, idReg: number, progettoCod: number, progettoDes = '') {
        this.esercizio = new Esercizio(progettoCod, progettoDes);
        this.esercizio.impiantoPK = {
            codice: idReg,
            appezzamentoPK: {
                codice: appezza,
                centroAziendalePK: {
                    codice: saCod,
                    partitaIva: piva
                }
            }
        }
    }
}

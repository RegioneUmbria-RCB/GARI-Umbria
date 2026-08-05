
import { AvversitaGruppo } from './AvversitaGruppo';

export class GruppoAvversita extends AvversitaGruppo {

    constructor(codice: number) {
        super(codice);
        this.classType = 'GruppoAvversita'
        this.For_Veg_Av_Cod = 0;
        this.formulatiXAllegatiNormative_IDRiga = 0;
    }
}

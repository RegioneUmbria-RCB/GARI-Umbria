import { AvversitaGruppo } from './AvversitaGruppo';
import {GruppoAvversita} from './GruppoAvversita';

export class AvversitaRilievo extends AvversitaGruppo {
    public gruppo: GruppoAvversita;
    public MxAV_Cod: number;


    constructor(codice: number) {
        super(codice);
        this.For_Veg_Av_Cod = 0;
        this.formulatiXAllegatiNormative_IDRiga = 0;
    }
}

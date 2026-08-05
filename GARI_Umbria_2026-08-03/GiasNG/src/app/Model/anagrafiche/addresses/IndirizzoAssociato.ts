import { Indirizzo } from './Indirizzo';

export class IndirizzoAssociato {
    indirizzo: Indirizzo;
    tipo_Indirizzo: number;
    flag_cancellazione: boolean;

    constructor() {
        this.flag_cancellazione = false;
    }
}

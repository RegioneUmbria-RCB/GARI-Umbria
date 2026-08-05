import { ParticelleCatastali, PKParticelleCatastali } from './ParticelleCatastali';

export class CatastoCampo {
    area: number;
    superficieCondotta: number;
    particella: ParticelleCatastali;
    flag_cancellazione: boolean;

    constructor() {
        this.flag_cancellazione = false;
    }

}


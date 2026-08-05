import { PKParticelleCatastali } from './ParticelleCatastali';

export class CatastoAppezzamento {
    particella: PKParticelleCatastali;
    area: number;
    flag_cancellazione: boolean;

    constructor() {
        this.flag_cancellazione = false;
    }
}

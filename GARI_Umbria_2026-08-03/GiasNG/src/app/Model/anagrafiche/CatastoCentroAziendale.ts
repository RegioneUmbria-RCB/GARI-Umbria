import { PKCentroAziendale } from './CentroAziendale';
import { ParticelleCatastali } from './ParticelleCatastali';
import { PossessoParticella } from './PossessoParticella';

export class CatastoCentroAziendale {
    centro: PKCentroAziendale;
    particella: ParticelleCatastali;
    possessiParticella: PossessoParticella[];
    flag_cancellazione: boolean;

    constructor() {
        this.flag_cancellazione = false;
    }
}

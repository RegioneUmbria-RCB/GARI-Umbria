

import { UtilizzoTerreno } from './UtilizzoTerreno';
import { Specie } from './Specie';

export class Varieta extends UtilizzoTerreno {
    specie: Specie;


    constructor() {
        super();
        this.classType = 'Varieta';
    }
}

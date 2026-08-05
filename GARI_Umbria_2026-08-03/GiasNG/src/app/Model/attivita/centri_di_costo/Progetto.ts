import {CentroDiCosto, Tipo} from './CentroDiCosto';

export class Progetto extends CentroDiCosto {


    constructor() {
        super();
        this.classType = 'Progetto';
        this.tipo = Tipo.Progetto;
    }
}

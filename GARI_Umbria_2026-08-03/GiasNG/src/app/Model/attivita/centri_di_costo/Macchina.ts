import {CentroDiCosto, Tipo} from './CentroDiCosto';

export class Macchina extends CentroDiCosto {


    constructor() {
        super();
        this.classType = 'Macchina';
        this.tipo = Tipo.Macchina;
    }
}

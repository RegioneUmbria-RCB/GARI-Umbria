import { CentroDiCosto, Tipo } from './CentroDiCosto';
import { MovimentoDiMagazzino } from 'app/Service/api.service';


export class ProdottoDaTrattareCDC extends CentroDiCosto {
    giacenzaMagazzino: MovimentoDiMagazzino;
    qtaTrattata: number;

    constructor() {
        super();
        this.classType = 'ProdottoDaTrattareCDC';
        this.tipo = Tipo.Esercizio;
    }
}

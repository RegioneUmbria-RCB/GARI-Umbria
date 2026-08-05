import { UnitaDiMisura } from '../../metaschema/UnitaDiMisura';
import { Prodotto } from './Prodotto';
import { Risorsa } from './Risorsa';
import { RilevamentoDiMagazzino} from '../RilevamentoDiMagazzino';

export class RisorsaProdotto extends Risorsa {
    prodotto: Prodotto;
    MagazziniMovimentazioni: RilevamentoDiMagazzino[];
    flagDoseQuantitaTotale: number;
    flagTipoDose: number;
    doseHaReale: number;
    doseHlReale: number;
    quantitaTotaleReale: number;
    unitaDiMisura: UnitaDiMisura;
    unitaDiMisuraIndicata: UnitaDiMisura;


    constructor() {
        super();
        this.classType = 'RisorsaProdotto';
    }
}

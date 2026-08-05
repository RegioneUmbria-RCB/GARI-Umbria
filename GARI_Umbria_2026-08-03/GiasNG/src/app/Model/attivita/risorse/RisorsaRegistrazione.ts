import { UnitaDiMisura } from '../../metaschema/UnitaDiMisura';
import { Prodotto } from './Prodotto';
import { Risorsa } from './Risorsa';
import { RilevamentoDiMagazzino} from '../RilevamentoDiMagazzino';
import {DettaglioRegistrazione} from "../dettagli/DettaglioRegistrazione";
import {Causale} from "../../metaschema/Causale";

export class RisorsaRegistrazione extends Risorsa {
    prodotto: Prodotto;
    MagazziniMovimentazioni: RilevamentoDiMagazzino[];
    qta: number;
    unitaDiMisura: UnitaDiMisura;
    causale: Causale;
    dettaglioRegistrazione: DettaglioRegistrazione;
    constructor() {
        super();
        this.classType = 'RisorsaRegistrazione';
    }
}

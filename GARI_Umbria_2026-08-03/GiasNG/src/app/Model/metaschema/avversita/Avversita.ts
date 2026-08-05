
import { AvversitaGruppo } from './AvversitaGruppo';
import { GruppoAvversita } from './GruppoAvversita';
import {UnitaDiMisura} from '../UnitaDiMisura';
import {BaseCodeDescrVal} from '../../baseClass/baseCodeDescrVal';

export class Avversita extends AvversitaGruppo {
    gruppo: GruppoAvversita;
    soglia: number;
    unitaDiMisura: UnitaDiMisura;
    presets: BaseCodeDescrVal[] = [];
    MxAV_Cod: number = 0;

}

import { BaseCodeDescr } from '../baseClass/baseCodeDescr';
import { TitoloDiPossesso } from '../metaschema/TitoloDiPossesso';
import { IntervalloTemporale } from './IntervalloTemporale';

export class ParticelleCatastaliZona {
    zona: BaseCodeDescr;
    Area: number;
    validita: IntervalloTemporale;

}

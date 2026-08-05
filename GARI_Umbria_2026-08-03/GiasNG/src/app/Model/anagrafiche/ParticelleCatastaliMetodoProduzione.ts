import { MetodoProduzione } from '../metaschema/MetodoProduzione';
import { IntervalloTemporale } from './IntervalloTemporale';

export class ParticelleCatastaliMetodoProduzione {
    metodoProduzione: MetodoProduzione;
    validita: IntervalloTemporale;
}

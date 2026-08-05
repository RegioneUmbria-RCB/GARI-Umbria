import { BaseCodeDescr } from '../baseClass/baseCodeDescr';
import { Macrouso } from '../metaschema/Macrouso';
import { IntervalloTemporale } from './IntervalloTemporale';

export class ParticelleCatastaliClassamento {
    porzione: string;
    Area: number;
    qualita: BaseCodeDescr;
    classe: string;
    redditoDomiciliare: number;
    redditoAgrario: number;

}

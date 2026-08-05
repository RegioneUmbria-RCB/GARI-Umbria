import { IntervalloTemporale } from './IntervalloTemporale';
import { FormeGiuridiche } from '../metaschema/FormeGiuridiche';
import { CentroAziendale } from './CentroAziendale';
import { CodiciAnagrafeValori } from './CodiciAnagrafeValori';
import { IndirizzoAssociato } from './addresses/IndirizzoAssociato';
import { Contatto } from './Contatto';
import { RisorseUmane } from './RisorseUmane';
import { BaseCodeDescr } from '../baseClass/baseCodeDescr';
import { Impresa } from './Impresa';

export class ImpresaPadre extends Impresa {
    codice_iscrizione_libro_soci: string;
    data_iscrizione_libro_soci: Date;
}

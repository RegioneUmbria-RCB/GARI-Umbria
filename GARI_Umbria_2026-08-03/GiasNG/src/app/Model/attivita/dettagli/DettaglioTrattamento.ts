
import { RisorsaProdotto } from '../risorse/RisorsaProdotto';
import { AvversitaGruppo} from '../../metaschema/avversita/AvversitaGruppo';
import { PrincipioAttivo } from 'app/Model/metaschema/PrincipioAttivo';
import { BufferZone } from 'app/Model/metaschema/BufferZone';
import { DoseEtichetta } from 'app/Model/metaschema/DoseEtichetta';
import { Soglia } from 'app/Model/metaschema/Soglia';
import {Tipo_Polverulento} from "../../TipiEnumerativi";
import {QuantitaSuImpianto} from "./QuantitaSuImpianto";

export class DettaglioTrattamento extends RisorsaProdotto {
    avversitaGruppo: AvversitaGruppo
    principiAttivi: PrincipioAttivo[];
    tempoCarenza: number;
    bufferzone: BufferZone;
    dettaglioProdotto: number;
    dosiEtichetta: DoseEtichetta[];
    soglia: Soglia;
    descrizionePrecedente: string;
    dataSmaltimentoScorte: Date;
    classificazioni: string;
    inRevisione: string;
    dataAttoNormativo: Date;
    formulatiXAllegatiNormative_IDRiga: number;
    tipoFormulato: number;
    epocheBlocchi: string;
    dettagliDose: string;
    protezione: string;
    modalitaImpiego: string;
    polverulento: Tipo_Polverulento;
    isImpollinatore: boolean;
    durataFeromone: number;
    scadenzaFeromone: Date;
    quantitaSuImpianti: Array<QuantitaSuImpianto>;
    ripartizioneTrappole: enum_Ripartizione_Trappole;

    constructor() {
        super();
        this.classType = 'DettaglioTrattamento';
    }


}

export enum enum_Ripartizione_Trappole
{
  Manuale = 1,
  Automatica = 2
}

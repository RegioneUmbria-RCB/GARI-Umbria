import { Fabbricato } from '../anagrafiche/Fabbricato';
import { UnitaDiMisura } from '../metaschema/UnitaDiMisura';
import { Prodotto } from './risorse/Prodotto';
import {Attivita} from "./Attivita";

export class RilevamentoDiMagazzino {
    Prodotto: Prodotto;
    Magazzino: Fabbricato;
    Agenzia: Fabbricato;
    Qta: number;
    QtaTot: number;
    Lotto: string;
    Cal_Cod: number;
    Cod_Progetto: number;
    udm: UnitaDiMisura;
    TipoRilevamento: RilevamentoMagazzinoTipo;
    Descrizione: string;
    N: number | null;
    P2O5: number | null;
    K2O: number | null;
    Cu: number | null;
    doseHaIndicata: number;
    doseHlIndicata: number;
    registrazioniCollegate: Attivita[];
}

export enum RilevamentoMagazzinoTipo {
    carico = 1,
    scarico = -1,
    giacenza = 0
}

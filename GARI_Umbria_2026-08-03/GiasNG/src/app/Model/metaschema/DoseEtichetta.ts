import { BaseCodeDescr } from "../baseClass/baseCodeDescr";
import { FlagFioritura } from "./FlagFioritura";
import { FlagProtetto } from "./FlagProtetto";
import { Mdi } from "./Mdi";
import { UnitaDiMisura } from "./UnitaDiMisura";

export class DoseEtichetta extends BaseCodeDescr {

    CodiceConcatenato: string;
    DescrizioneConcatenata: string;
    DoseMin: number;
    DoseMax: number;
    Udm: UnitaDiMisura;
    AcquaMin: number;
    AcquaMax: number;
    UdmAcqua: UnitaDiMisura;
    Da_Epoca: string;
    A_Epoca: string;
    Epoca_Des: string;
    Limite: number;
    UdmLimite: UnitaDiMisura;
    strCLTOSS_Grado: string;
    Flag_Fioritura: FlagFioritura;
    IntervalloTrattamenti_Min: number;
    IntervalloTrattamenti_Max: number;
    Mdi: Mdi;
    Flag_Protetto: FlagProtetto;
    FormulatiXAllegatiNormative_IDRiga: number;
    DataSmaltimentoScorte: string;
    Gruppo_Dosaggi: number;
    Num_Max_Interventi_Globali: number;

    constructor(codice: number) {
        super(codice);
    }

}

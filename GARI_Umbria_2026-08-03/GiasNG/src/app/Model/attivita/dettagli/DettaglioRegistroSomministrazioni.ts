import { AvversitaGruppo } from "app/Model/metaschema/avversita/AvversitaGruppo";
import { RisorsaProdotto } from "../risorse/RisorsaProdotto";
import { IntervalloTemporale } from "app/Model/anagrafiche/IntervalloTemporale";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";


export class DettaglioRegistroSomministrazioni extends RisorsaProdotto {
    /**
     * Numero somministrazione
     */
    codice: string;

    /**
     * Numero trattamento
     */
    numTrattamento: string;

    /**
     * Movimenti_Dettagli.Pro_Cod da Farmaci.AIC
     */
    codiceAIC: string;

    /**
     * Mov_Dettaglio_Tecnico.av_cod, Mov_Dettaglio_Tecnico.av_gru
     */
    avversitaGruppo: AvversitaGruppo;

    /**
     * Movimenti_dettagli.tempocarenza
     */
    sospensione: TempiSospensione[];

    /**
     * Data inizio e fine del trattamento
     */
    validita: IntervalloTemporale;

    /**
     * Data della prescrizione
     */
    dataPrescrizione: Date;

    /**
     * Durata del trattamento in giorni
     */
    durataTrattamento: number;

    /**
     * Numero del registro di scorta (da VetInfo)
     */
    regSco_Numero: string;

    constructor() {
        super();
        this.classType = 'DettaglioRegistroSomministrazioni'; // Equivalente di costanti.ClassType
    }

    /**
     * Metodo per clonare l'oggetto
     */
    clona(): DettaglioRegistroSomministrazioni {
        try {
            return JSON.parse(JSON.stringify(this)) as DettaglioRegistroSomministrazioni;
        } catch (error) {
            throw new Error(`DettaglioRegistroSomministrazioni.clona: ${error.message}`);
        }
    }
}

export class TempiSospensione {
    tempoSospensione: number;
    Alimento: BaseCodeDescr;
}
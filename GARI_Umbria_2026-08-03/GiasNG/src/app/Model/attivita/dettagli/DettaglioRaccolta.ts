import { RisorsaProdotto } from '../risorse/RisorsaProdotto';
import { Specie } from '../../metaschema/utilizzi/Specie';
import { Varieta } from '../../metaschema/utilizzi/Varieta';
import { QuantitaSuImpianto } from '../dettagli/QuantitaSuImpianto';
import { Tipo_Raccolta } from '../Attivita';
import { OpzioniRaccolta } from 'app/quaderno-di-campagna/agenda-edit/componenti/prodotti/sezioni/raccolta/opzioni-raccolta/opzioni-raccolta.model';
import { Prodotto } from '../risorse/Prodotto';
import { UnitaDiMisura } from 'app/Model/metaschema/UnitaDiMisura';
import { enum_UnitaMisura } from 'app/Model/TipiEnumerativi';
import {GruppoFinalita} from "../../metaschema/utilizzi/GruppoFinalita";
import { Regolamenti } from 'app/Model/metaschema/Regolamenti';

export class DettaglioRaccolta extends RisorsaProdotto {
    specie: Specie;
    codArticolo: string;
    regolamento: Regolamenti;
    culCod: number;
    varieta: Varieta;
    finalita: GruppoFinalita;
    calCod: number;
    QuantitaSuImpianti: QuantitaSuImpianto[];
    TipoRaccolta: Tipo_Raccolta;

    /** Valorizzato in quaderno-di-campagna-form-to-attivita per il passaggio
     * delle opzioni a lato server. Le opzioni sono prese dall'oggetto
     * Sezione_Prodotto_Raccolta.
     * FormGroup di riferimento creato in qdc.service
     **/
    Opzioni_Raccolta: OpzioniRaccolta;
    /** Riferita alla data del relativo carico di magazzino */
    dataIngresso: Date;

    /** Identificativo RisorsaProdotto, non presente nel modello da database*/
    codice: number;

    /**
     *
     * @param tipo se valorizzato a Tipo_Raccolta.Fast, genera un template
     * contenetne i dati sufficienti al salvataggio di una raccolta senza
     * prodotto.
     */
    constructor(tipo?: Tipo_Raccolta) {
        super();
        this.classType = 'DettaglioRaccolta';
        this.QuantitaSuImpianti = [];

        if (tipo && tipo === Tipo_Raccolta.Fast)
            this.createFastHarvestTemplate();
    }

    private createFastHarvestTemplate(): void {
        this.TipoRaccolta = Tipo_Raccolta.Fast;
        this.dataIngresso = new Date();
        this.prodotto = new Prodotto(0);
        this.unitaDiMisura = new UnitaDiMisura(enum_UnitaMisura.KG, '');
        this.quantitaTotaleReale = 0;
        this.QuantitaSuImpianti = [];
        this.MagazziniMovimentazioni = [];
    }

}

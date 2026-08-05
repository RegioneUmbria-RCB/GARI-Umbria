import { Risorsa } from '../risorse/Risorsa';
import {FormControl, FormGroup} from '@angular/forms';
import {EsercizioRilievoCDC} from '../centri_di_costo/EsercizioRilievoCDC';
import {AvversitaGruppo} from '../../metaschema/avversita/AvversitaGruppo';
import {AvversitaRilievo} from '../../metaschema/avversita/AvversitaRilievo';
import {MisuraAvversita} from '../../metaschema/avversita/MisuraAvversita';
import {FaseFenologica} from "../../metaschema/FaseFenologica";
import {UnitaDiMisura} from "../../metaschema/UnitaDiMisura";
import {RisorsaProdotto} from "../risorse/RisorsaProdotto";

export class DettaglioRilievo extends Risorsa {
    faseFenologica: FaseFenologica;
    indiceMaturita: number = 0;
    indiceResa: number = 0;
    dannoRaccolta: number = 0;
    erbaInfestante: AvversitaGruppo = null;
    avversitaGruppo: AvversitaRilievo = null;

    dataOraRilievo: string | Date = new Date();
    qtaRilevata: number = 0;
    impianto: string;
    descrizione: string;
    qtaRilevataString: string;
    esercizioCDC: EsercizioRilievoCDC;
    note: string;
    unitaDiMisura: UnitaDiMisura;

    id_agenda: string = "0";
    /** Specifica il codice completo dell'avversità (indice o altro) rilevata.
     * Usato per verificare la presenza di un rilievo dello stesso tipo in griglia*/
    codRilievo: string;
    /** Lista contenente le quantità personalizzate */
    presets: MisuraAvversita[] = [];
    resaUltimoRilievo: string | null = '0';

    //Utilizzato nel Rilievo Avversità Trappole
    risorsaProdotto: RisorsaProdotto = null;

    constructor() {
        super();
        this.classType = 'DettaglioRilievo';
    }

    public toFormGroup(): FormGroup {
        const form = new FormGroup({
            faseFenologica: new FormControl(this.faseFenologica),
            indiceMaturita: new FormControl(this.indiceMaturita),
            indiceResa: new FormControl(this.indiceResa),
            dannoRaccolta: new FormControl(this.dannoRaccolta),
            erbaInfestante: new FormControl(this.erbaInfestante),
            avversitaGruppo: new FormControl(this.avversitaGruppo),
            dataOraRilievo: new FormControl(this.dataOraRilievo),
            qtaRilevata: new FormControl(this.qtaRilevata),
            impianto: new FormControl(this.impianto),
            esercizioCDC: new FormControl(this.esercizioCDC),
            descrizione: new FormControl(this.descrizione),
            qtaRilevataString: new FormControl(this.qtaRilevataString),
            classType: new FormControl(this.classType),
            id_agenda: new FormControl(this.id_agenda),
            codRilievo: new FormControl(this.codRilievo),
            presets: new FormControl(this.presets),
            resaUltimoRilievo: new FormControl(this.resaUltimoRilievo),
            note: new FormControl(this.note),
            unitaDiMisura: new FormControl(this.unitaDiMisura),
            risorsaProdotto: new FormControl(this.risorsaProdotto)
        });
        return form;
    }
}

import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { NoteInterventoGruppi } from "./NoteInterventoGruppi";


export class NoteIntervento extends BaseCodeDescr {

    note_Valore_Numerico: number;

    note_Valore_Stringa: string;

    visibile: number;

    noteInterventoGruppi: NoteInterventoGruppi;

    constructor(codice: number) {
        super(codice);
    }
}

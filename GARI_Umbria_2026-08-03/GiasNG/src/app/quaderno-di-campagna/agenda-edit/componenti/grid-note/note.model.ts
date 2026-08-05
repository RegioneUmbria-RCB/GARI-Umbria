import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { KendoGridRow } from 'gias-kendo-grid';

export class GruppoNoteModel extends KendoGridRow {
    id: string;
    Gruppo_Cod: number[];
    Gruppo_Des: string;
    Nota_Cod: number[];
    Nota_Des: string[];
    Defaults: BaseCodeDescr[];
    isRadio: boolean;

    constructor(id: string, Gruppo_Cod: number, Gruppo_Des: string, isRadio?: boolean) {
        super();
        this.id = id;
        this.Gruppo_Cod = [];
        this.Gruppo_Cod.push(Gruppo_Cod);
        this.Gruppo_Des = Gruppo_Des;
        this.Nota_Cod = [];
        this.Nota_Des = [];
        this.Defaults = [];
        this.isRadio = isRadio || false;
    }
}

export class NotaInterventoDdlItem {
    id: number;
    descrizione: string;
    NotaGruppo_Cod: number;

    constructor(codice: number, descrizione: string, Gruppo_Cod: number) {
        this.id = codice;
        this.descrizione = descrizione;
        this.NotaGruppo_Cod = Gruppo_Cod;
    }
}


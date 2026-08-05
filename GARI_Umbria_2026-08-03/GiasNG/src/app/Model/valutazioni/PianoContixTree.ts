
export class PianoContixTree {
    piva : string;
    id : string;
    tipo : string;
    codice : number;
    descrizione : string;
    flag_collegato : boolean;

    TreeValutazione : Array<TreeValutazione> 
}

export class TreeValutazione {
    id : string;
    tipo : string;
    codice : number;
    descrizione : string;
    ordine : number;
    flag_collegato : boolean;
    flag_selezionato : boolean;

    children : Array<children> 
}

export class children {
    id : string;
    tipo : string;
    codice : number;
    descrizione : string;
    ordine : number;
    flag_collegato : boolean;
    flag_selezionato : boolean;

    children : Array<children> 
}
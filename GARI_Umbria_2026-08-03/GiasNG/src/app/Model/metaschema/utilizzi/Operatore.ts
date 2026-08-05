export class Operatore {

    username: string;
    nome_cognome: string;
    Cod_RisUm: number;
    Codice_Fiscale: string;

    constructor(username: string, nome_cognome: string, Cod_RisUm: number, Codice_Fiscale: string)  {
        this.username = username;
        this.Codice_Fiscale = Codice_Fiscale;
        this.nome_cognome = nome_cognome;
        this.Cod_RisUm = Cod_RisUm;
    }
}
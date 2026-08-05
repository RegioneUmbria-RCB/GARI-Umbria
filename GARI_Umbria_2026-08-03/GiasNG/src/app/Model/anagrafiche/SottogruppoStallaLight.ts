import { PKFabbricato } from "./Fabbricato";

export class SottogruppoStallaLight {

    codice: number;
    nome: string;
    stallaPK: PKFabbricato;


    constructor(fabbricatoPK: PKFabbricato, raggrCod: number, raggrDes: string) {
        this.stallaPK = fabbricatoPK;
        this.codice = raggrCod;
        this.nome = raggrDes;
    }

}
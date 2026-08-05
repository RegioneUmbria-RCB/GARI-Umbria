export class CentroDiCosto {
    classType: string;
    codice: CodeType;
    tipo: Tipo;
}

export enum Tipo {
    Macchina = 0,
    Esercizio = 1,
    Progetto = 2,
    CapoAnimale = 3,
    ProdottoDaTrattare = 4,
}

export class CodeType {
    intValue: number;
    stringValue: string;
    type: Type;
}

export enum Type {
    INT,
    STRING
}

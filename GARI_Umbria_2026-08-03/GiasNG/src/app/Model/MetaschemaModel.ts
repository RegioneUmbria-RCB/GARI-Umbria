export class FormaGiuridica {
    Fg_Cod: number;
    Fg_Des: string;
}

export class ISTAT_GetCAP_Request {
  Prov: string;
  Com: string;
}

export class GetProvince {
  stato: string;
  regione: string;
}

export class Provincia {
    sigla: string;
    regione_cod: string;
    regione_des: string;
    Provincia_Des: string;
    Istat_Prov: string;
    Stato_Country: string;
    comuneDefault: string;
}

export class Comune {
    provincia: Provincia;
    codice: string;
    descrizione: string;
}

export class Stato {
    codice: string;
    descrizione: string;
}

export class DestinazioneUso{
    codice: number;
    descrizione: string;
}

export class SpecieVegetale{
    veg_cod: number;
    veg_des: string;

    constructor(codice?: number, descrizione?: string) {
        this.veg_cod = codice?.valueOf();
        this.veg_des = descrizione?.toString();
    }
}

export class Cultivar{
    cul_cod: number;
    cul_des: string;

    constructor(codice?: number, descrizione?: string) {
        this.cul_cod = codice?.valueOf();
        this.cul_des = descrizione?.toString();
    }
}

export class CultivarxSpecie{
    Veg_Cod: number;
    Cultivar: Cultivar[];
}

export class Finalita{
    grfi_cod: number;
    grfi_des: string;
}

export class FinalitaxSpecie{
    Veg_Cod: number;
    Finalita: Finalita[];
}

export class ColturePrecedenti{
    Codice: string;
    Descrizione: string;
}

export class Ditta{
    codice: number;
    descrizione: string;
}

export class Tipo{
    CLASS_CODE: string;
    CLASS_DESC: string;
}

export class Tipo1{
    id: string;
    name: string;
}

export class Marca{
    codice: number;
    descrizione: string;
}

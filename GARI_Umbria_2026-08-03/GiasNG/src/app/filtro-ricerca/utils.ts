import { Enum_TipoComportamento_FiltroRicerca, Enum_TipoMostra_FiltroRicerca, enum_CodificaStampe, enum_TipoFiltrone } from "app/Model/TipiEnumerativi";
import { Enum_SiteRedirector } from "app/Model/siti.enum";
import { FiltriMovimenti, FiltriPianoColturale, FiltriTemporali } from "app/Service/net-core6-api.service";

export enum Enum_Type_Button_Clear {
    fromFilters = 1,
    fromDatiAggiuntivi = 2,
    fromDatiAggiuntiviAll = 3
}

export class FormFiltriAziende {
    ImpreseReferenti: any[];
    TipiImpresa: any[];
    RagioneSociale: string;
    CUAA: string;
    Piva: string;
    Stati: any[];
    Regioni: any[];
    Province: any[];
    Comuni: any[];
    Zone: any[];
    FiltroZone: any;
}

export class FormFiltriCentriAziendali {
    CentroAziendale: string;
    Stati: any[];
    Regioni: any[];
    Province: any[];
    Comuni: any[];
}

export class FormFiltriPianoColturale {
    ContributiACA: any[];
    UtilizzoTerreno: any[];
    FiltroDestinazioneUso: any;
    DestinazioniUso: any[];
    GruppoVegetale: any[];
    Specie: any[];
    Varieta: any[];
    TipologiaVarietale: any[];
    Lotto: string;
    Progetto: string;
}

export class FormFiltroTemporale {
    Entita: any;
    ColonnaData: any;
    TipoConfronto: any;
    ModalitaFiltroData: any;
    Date: FormFiltroTemporaleBreve;

    constructor(entita, colonnaData, tipoConfronto, modalitaFiltroData, date) {
        this.Entita = entita;
        this.ColonnaData = colonnaData;
        this.TipoConfronto = tipoConfronto;
        this.ModalitaFiltroData = modalitaFiltroData;
        this.Date = date;
    }
}

export class FormFiltriTemporali {
    // CreazioneImpianto: FormFiltroTemporale;
    // ValiditaEsercizio: FormFiltroTemporale;
    // ValiditaImpianto: FormFiltroTemporale;
    FiltriData: FormFiltroTemporale[] | null;
    OperatoreLogico: any;
}

export class FormFiltroTemporaleBreve {
    inizio: Date;
    fine: Date;

    constructor(inizio, fine) {
        this.inizio = inizio;
        this.fine = fine;
    }
}

export class FormFiltriMovimenti {
    GruppoOperazioni: any[];
    Operazioni: any[];
    FiltroOperazioni: any;
    DataMovimento: FormFiltroTemporaleBreve;
}

export class FormDatiIscrizioneLibroSoci {
    NumeroIscrizione: boolean;
    DataIscrizione: boolean;

    constructor(numero: boolean, data: boolean) {
        this.NumeroIscrizione = numero;
        this.DataIscrizione = data;
    }
}

export class FormCaricaDati {
    ImpreseReferenti: boolean;
    DatiIscrizioneLibroSoci: FormDatiIscrizioneLibroSoci;
    LegaleRappresentante: boolean;
    IndirizzoAzienda: boolean;
    IndirizzoCentroAziendale: boolean;
    IndirizziPianoColturale: boolean;
    AltriDatiAzienda: boolean;
    AltriDatiCentro: boolean;
    CatastoCampo: boolean;
    AltriDatiCampo: boolean;
    AltriDatiPianoColturale: boolean;
    AltriDatiFabbricato: boolean;
    Servizi: boolean;
    CodiciAzienda: any[];
    CodiciCentroAziendale: any[];
    CodiciCampo: any[];
    CodiciPianoColturale: any[];
    CodiciFabbricato: any[];
    CatastoAppezzamento: boolean;
    GISImpianto: boolean;
    ContributiACA: boolean;
    CatastoCentroAziendale: boolean;
}

export class FormFiltriServizi {
    Servizio: any;
    StatiPratica: any;
    Data: Date;
}

export class FormFiltriCatasto {
    FiltroRipartoCatastale: any;
}

export class FormFiltriGIS {
    FiltroPoligoni: any;
    Anomalie: any[];
}

export class FormFiltroRicercaConfig {

    FiltriAziende: FormFiltriAziende;

    FiltriCentriAziendali: FormFiltriCentriAziendali;

    FiltriPianoColturale: FormFiltriPianoColturale;

    FiltriTemporali: FormFiltriTemporali;

    FiltriMovimenti: FormFiltriMovimenti;

    FiltriServizi: FormFiltriServizi;

    FiltriCatasto: FormFiltriCatasto;

    FiltriGIS: FormFiltriGIS;

    CaricaDati: FormCaricaDati;

    CategoriaEsito: any;
    Budget: any;
}

export class ParametriFiltroRicercaNG {

    SitoOrigine: Enum_SiteRedirector;
    PaginaProvenienza: number;
    PaginaProvenienzaURL: string;
    SitoDestinazioneDopoIlRedirect: number;
    PaginaDestinazioneDopoIlRedirect: number;
    TipoFiltrone: enum_TipoFiltrone;
    CodificaStampe: enum_CodificaStampe;
    VegCod: number;
    IdSezione: number;
    Piva: string;
    G2g: number;
    NoPiva: string;
    IncludiVisite: boolean;
    HeaderFooter: boolean = true;
    Redir: string;
    FiltriPianoColturale: FiltriPianoColturale;
    FiltriTemporali: FiltriTemporali;
    FiltriMovimenti: FiltriMovimenti;
    TipoComportamentoFiltroRicercaNG: Enum_TipoComportamento_FiltroRicerca = Enum_TipoComportamento_FiltroRicerca.Ricerca;

    TipoMostraGestitiChiamante: Array<Enum_TipoMostra_FiltroRicerca>;

}

export class BloccoxReport {
    IdStampa: enum_CodificaStampe;
    SingolaPiva: boolean;
    SingoloSaCod: boolean;
    SingoloVegCod: boolean;

    constructor(idStampa, singolaPiva, singoloSaCod, singoloVegCod) {
        this.IdStampa = idStampa;
        this.SingolaPiva = singolaPiva;
        this.SingoloSaCod = singoloSaCod;
        this.SingoloVegCod = singoloVegCod;
    }
}
const enum_TipoRisorsa = {
    PRODUZIONE_VEGETALE: 1,
    PRODUZIONE_ANIMALE: 2,
    PARCO_MACCHINE: 3,
    ALTRE_MATERIE_AZIENDALI: 4,
    MANGIMI: 5,
    FARMACI: 6,
    CONTATTI: 7,
    CALI_DI_LAVORAZIONE: -1,
    CONFEZIONI_PRODOTTO: 8,
    SERVIZI: 9
}

var ElencoTipoTabella = [
    { Tipo: 1, Tipo_Des: "Tabella" },
    { Tipo: 2, Tipo_Des: "Campo Note" },
    { Tipo: 3, Tipo_Des: "ELI" }
];

var ElencoOmni = [
    { ChkOmni_Invisibili: 0, ChkOmni_Invisibili_Des: "Inclusi" },
    { ChkOmni_Invisibili: 1, ChkOmni_Invisibili_Des: "Esclusi" }
];
/**
 * 
 * @param {any} resxObj Se passato, imposta le descrizioni sulla lingua impostata per l'utente
 */
function Inizializza_enum_LavCod(resxObj) {

    if (resxObj === null || resxObj === undefined || (Array.isArray(resxObj) && resxObj.length === 0)) {
        resxObj = [{}];
    }

    enum_LavCod = {
        Fattura_Ricevuta: { value: 1000, name: TraduzioneMultiResx(resxObj, "FatturaRicevuta", "Fattura Ricevuta"), code: 1000 },
        Fattura_Emessa: { value: 1001, name: TraduzioneMultiResx(resxObj, "FatturaEmessa", "Fattura Emessa"), code: 1001 },
        Nota_Accredito_Ricevuta: { value: 1002, name: TraduzioneMultiResx(resxObj, "NotaAccreditoRicevuta", "Nota Accredito Ricevuta"), code: 1002 },
        Nota_Accredito_Emessa: { value: 1003, name: TraduzioneMultiResx(resxObj, "NotaAccreditoEmessa", "Nota Accredito Emessa"), code: 1003 },
        Corrispettivo_Vendita: { value: 1020, name: TraduzioneMultiResx(resxObj, "CorrispettivoVendita", "Corrispettivo Vendita"), code: 1020 },
        DDT_Ricevuto: { value: 1025, name: TraduzioneMultiResx(resxObj, "DDTRicevuto", "DDT Ricevuto"), code: 1025 },
        DDT_Emesso: { value: 1031, name: TraduzioneMultiResx(resxObj, "DDTEmesso", "DDT Emesso"), code: 1031 },
        DDT_Contabilizzato_Emesso: { value: 1069, name: TraduzioneMultiResx(resxObj, "DDTContabilizzatoEmesso", "DDT Contabilizzato Emesso"), code: 1069 },
        Accettazione_da_diversi: { value: 1054, name: TraduzioneMultiResx(resxObj, "DDTRicevuto", "DDT Ricevuto"), code: 1054 }, //Accettazione +
        Distinta_Carico: { value: 1075, name: TraduzioneMultiResx(resxObj, "DistintaDiCarico", "Distinta di Carico"), code: 1075 },
        Distinta_Carico_Accettazione: { value: 1076, name: TraduzioneMultiResx(resxObj, "DistintaDiCarico", "Distinta di Carico"), code: 1076 }, //Accettazione +
        Auto_Ddt_Emesso: { value: 1077, name: TraduzioneMultiResx(resxObj, "AutoDDTEmesso", "Auto DDT Emesso"), code: 1077 },
        Auto_Ddt_Emesso_Accettazione: { value: 1078, name: TraduzioneMultiResx(resxObj, "AutoDDTEmesso", "Auto DDT Emesso"), code: 1078 }, //Accettazione +
        Ordine_Vendita_Emesso: { value: 2002, name: TraduzioneMultiResx(resxObj, "OrdineDiVendita", "Ordine di Vendita"), code: 2002 },
        Ordine_Acquisto: { value: 2004, name: TraduzioneMultiResx(resxObj, "OrdineDiAcquisto", "Ordine di Acquisto"), code: 2004 },
        Lavorazione: { value: 5000, name: TraduzioneMultiResx(resxObj, "Lavorazione", "Lavorazione"), code: 5000 },
        Ordine_Lavorazione: { value: 2005, name: TraduzioneMultiResx(resxObj, "OrdineDiLavorazione", "Ordine di Lavorazione"), code: 2005 },
        Carico_Magazzino: { value: 1022, name: TraduzioneMultiResx(resxObj, "CaricoDiMagazzino", "Carico di Magazzino"), code: 1022 },
        Scarico_Magazzino: { value: 1023, name: TraduzioneMultiResx(resxObj, "ScaricoDiMagazzino", "Scarico di Magazzino"), code: 1023 },
        Contratto_Affitto: { value: 2006, name: TraduzioneMultiResx(resxObj, "ContrattoDiAffitto", "Contratto di Affitto"), code: 2006 }
    };
}

/**
 *
 * @param {any} resxObj Se passato, imposta le descrizioni sulla lingua impostata per l'utente
 */
function Inizializza_enum_udm(resxObj) {

    if (resxObj === null || resxObj === undefined || (Array.isArray(resxObj) && resxObj.length === 0)) {
        resxObj = [];
    }

    enum_Udm = {
        chilogrammi: { value: 2, name: "kg", decimals: 6, format: "0.######" },
        numero_trappole: { value: 11, name: TraduzioneMultiResx(resxObj, "NumeroTrappole", "n.trappole"), decimals: 0, format: "n0" },
        litri: { value: 29, name: TraduzioneMultiResx(resxObj, "LitriSigla", "L"), decimals: 6, format: "0.######" },
        numero: { value: 38, name: TraduzioneMultiResx(resxObj, "NumeroSigla", "n"), decimals: 3, format: "0.###" },
        n_piante: { value: 92, name: TraduzioneMultiResx(resxObj, "NumeroPiante", "Num. Piante"), decimals: 0, format: "n0" },
        unit_di_seme: { value: 93, name: TraduzioneMultiResx(resxObj, "UnitaDiSeme", "unità di seme"), decimals: 0, format: "n0" },
        confezioni: { value: 1003, name: TraduzioneMultiResx(resxObj, "ConfezioniSigla", "Cfz"), decimals: 0, format: "n0" },
        ettari: { value: 2123, name: TraduzioneMultiResx(resxObj, "EttariSigla", "ha"), decimals: 6, format: "0.######" },
        quintali: { value: 4, name: "q", decimals: 6, format: "0.######" },
        tonnellate: { value: 304, name: "t", decimals: 6, format: "0.######" },
        chilometri: { value: 305, name: "km", decimals: 3, format: "0.###" },
        miglia: { value: 306, name: "mi (miglia)", decimals: 3, format: "0.###" }
    };
}

/**
 *
 * @param {any} resxObj Se passato, imposta le descrizioni sulla lingua impostata per l'utente
 */
function Inizializza_elencoScontoModalita(resxObj) {

    if (resxObj === null || resxObj === undefined || (Array.isArray(resxObj) && resxObj.length === 0)) {
        resxObj = [];
    }

    elencoScontoModalita = [
        { Sconto_Modalita: 0, Sconto_Modalita_Descr: TraduzioneMultiResx(resxObj, "ScontoPercentualeSuPrezzoUnitario", "Sconto % su prezzo unitario") },
        { Sconto_Modalita: 1, Sconto_Modalita_Descr: TraduzioneMultiResx(resxObj, "ScontoMerce", "Sconto Merce") },
        { Sconto_Modalita: 3, Sconto_Modalita_Descr: TraduzioneMultiResx(resxObj, "CampioniGratuiti", "Campioni gratuiti") },
        { Sconto_Modalita: 2, Sconto_Modalita_Descr: TraduzioneMultiResx(resxObj, "CampioniOmaggioSenzaRivalsaIva", "Campioni omaggio senza rivalsa Iva") },
        { Sconto_Modalita: 4, Sconto_Modalita_Descr: TraduzioneMultiResx(resxObj, "CampioniOmaggioConRivalsaIva", "Campioni omaggio con rivalsa Iva") }
    ];
}

/**
 *
 * @param {any} resxObj Se passato, imposta le descrizioni sulla lingua impostata per l'utente
 */
function Inizializza_elencoTempoCarenza(resxObj) {

    if (resxObj === null || resxObj === undefined || (Array.isArray(resxObj) && resxObj.length === 0)) {
        resxObj = [];
    }

    elencoTempoCarenza = [
        { TempoCarenza: 0, TempoCarenza_Descr: TraduzioneMultiResx(resxObj, "PrezzoUnitario", "Prezzo unitario") },
        { TempoCarenza: 1, TempoCarenza_Descr: TraduzioneMultiResx(resxObj, "ImponibileTotale", "Imponibile totale") },
        { TempoCarenza: 2, TempoCarenza_Descr: TraduzioneMultiResx(resxObj, "ImportoTotale", "Importo totale") },
        { TempoCarenza: 3, TempoCarenza_Descr: TraduzioneMultiResx(resxObj, "ImportoUnitario", "Importo unitario") }
    ];
}


/**
 *
 * @param {any} resxObj Se passato, imposta le descrizioni sulla lingua impostata per l'utente
 */
function Inizializza_elenchiPrezzoLivello(resxObj) {

    if (resxObj === null || resxObj === undefined || (Array.isArray(resxObj) && resxObj.length === 0)) {
        resxObj = [];
    }

    elencoPrezzoLivelloFF = [
        { Prezzo_Livello: -1, Prezzo_Livello_Descr: TraduzioneMultiResx(resxObj, "PesoNetto", "Peso Netto") },
        { Prezzo_Livello: 4, Prezzo_Livello_Descr: TraduzioneMultiResx(resxObj, "Imballaggio", "Imballaggio") },
        { Prezzo_Livello: 8, Prezzo_Livello_Descr: TraduzioneMultiResx(resxObj, "Contenitore", "Contenitore") },
        { Prezzo_Livello: 5, Prezzo_Livello_Descr: TraduzioneMultiResx(resxObj, "Confezione", "Confezione") }
    ];

    elencoPrezzoLivelloNoFF = [
        { Prezzo_Livello: -1, Prezzo_Livello_Descr: "Kg / Lt" },
        { Prezzo_Livello: 0, Prezzo_Livello_Descr: TraduzioneMultiResx(resxObj, "RisorsaQuantità", "Quantità") }
    ];

    elencoPrezzoLivelloSoloQta = [
        { Prezzo_Livello: 0, Prezzo_Livello_Descr: TraduzioneMultiResx(resxObj, "RisorsaQuantità", "Quantità") }
    ];
}

/**
 *
 * @param {any} resxObj Se passato, imposta le descrizioni sulla lingua impostata per l'utente
 */
function Inizializza_elencoScontoMaggiorazione(resxObj) {

    if (resxObj === null || resxObj === undefined || (Array.isArray(resxObj) && resxObj.length === 0)) {
        resxObj = [];
    }

    elencoScontoMaggiorazione = [
        { ScontoMaggiorazione: 0, ScontoMaggiorazione_Descr: TraduzioneMultiResx(resxObj, "Sconto", "Sconto") },
        { ScontoMaggiorazione: 1, ScontoMaggiorazione_Descr: TraduzioneMultiResx(resxObj, "Maggiorazione", "Maggiorazione") }
    ];
}

/**
 *
 * @param {any} resxObj Se passato, imposta le descrizioni sulla lingua impostata per l'utente
 */
function Inizializza_elencoFinalitaMacchine(resxObj) {

    if (resxObj === null || resxObj === undefined || (Array.isArray(resxObj) && resxObj.length === 0)) {
        resxObj = [];
    }

    elencoFinalitaMacchine = [
        { Tipo: 0, Tipo_Des: TraduzioneMultiResx(resxObj, "AgricolaZootecnica", "Agricola/Zootecnica") },
        { Tipo: 1, Tipo_Des: TraduzioneMultiResx(resxObj, "Industriale", "Industriale") },
        { Tipo: 2, Tipo_Des: TraduzioneMultiResx(resxObj, "Commerciale", "Commerciale") }
    ];
}



var enum_TipoOperazioneDB = {
    Lettura: { value: 0, name: "Lettura", code: 0 },
    Scrittura: { value: 1, name: "Scrittura", code: 1 },
    Modifica: { value: 2, name: "Modifica", code: 2 },
    Cancellazione: { value: 3, name: "Cancellazione", code: 3 },
    Trasferimento: { value: 4, name: "Trasferimento", code: 4 },
    Copia: { value: 10, name: "Copia", code: 10 }
};

var enum_LavCod = {};
Inizializza_enum_LavCod();

var enum_Pendenza = {
    DocBolla: 0,                    //Movimento Allegato a Bolla di accompagnamento
    DocFattura: 1,                  //Movimento Allegato a Fattura
    MovPendente: 2,                 //Movimento Pendente
    MovESENTE: 3,                   //Movimento Esente
    MovGiustificato: 4,             //Movimento Giustificato
    MovForzato: 5,                  //Movimento Non Giustificato e Forzato dall'utente
    GiacenzeIniziali: 6,            //Giacenze Iniziali
    Conferimento: 7,                //Materia Prima/Lavorato in Conferimento
    AutoProduzione: 8,              //Materia Prima/Lavorato Autoprodotto
    AutoConsumo: 9,                 //Materia Prima/Lavorato Autoconsumato
    Smaltimento: 10,                //Materia Prima/Lavorato Smaltimento / perdita di lavorazione
    ZooConsistenzeIniziali: 11,     //Consistenze Iniziali Zootecniche
    Trasferimento: 12,              //Trasferimento Merci
    DocRicevuta: 13,                //Movimento Allegato a Ricevuta Fiscale
    Resi_Acquisti: 14,              //Scarico Giustificato da Resi su Acquisti
    Resi_Vendite: 15,               //Carico Giustificato da Resi su Vendite
    Furto: 16,                      //scarico giustificato da furto
    ScaricoFuoriRegione: 17,        //Scarico Fuori Regione
    ResoFornitore: 18,              //Reso a Fornitore

    //Questi sono i valori presenti nel LAN:
    //DocAccettazione = 16       'Movimento Allegato a Buono Di Accettazione
    //DocDoco = 17               'Movimento Allegato a Doco
    //DocDAA = 18                'Movimento Allegato a D.A.A.
    DocFattura_ProForma: 19,        //Movimento Allegato a Fattura ProForma
    DocCorrispettivo: 20,           //Movimento Allegato a Corrispettivo
    DocOrdine: 21,                  //Movimento Allegato a Ordine Vendita
 
    DocPreventivo_Vendita: 23,     //Movimento Allegato a Preventivo Vendita
    DocMVV: 24,                    //Movimento Allegato a MVV
    DocProcLiqS: 25,               //Movimento Allegato a Liquidazione soci
    Alienazione_Pendenza: 26,      //Movimento Giustificato da Alienazione
    Rottura: 27,                   //Movimento Giustificato da Rottura
    Altra_Pendenza: 99,            //Altro --> Des_Lib Editabile

    ZooAcquistoAnimali: 31,
    ZooNascita: 32
};


var enum_TipoRapporto = {
    Clienti: 0,
    Fornitori: 1,
    Professionisti: 2,
    DipendentiTerzisti: 3,
    Agenti: 4,
    CapoArea: 5,
    Conferenti: 6,
    Vettori: 7,
    ClientiFornitori: 8,
    TerzistiNoFiltro: 9,
    Dipendenti: 10,
    Referente_Conferimento: 11
};

var enum_Udm = {};
Inizializza_enum_udm();

var enum_ModalitaDocContabile = {
    Nessuno: 0,
    Ricevimento_Prodotto: 1,
    Ricevimento_Ortofrutta: 2
};

var elencoScontoModalita = [];
Inizializza_elencoScontoModalita();


// Per definire come viene salvato il prezzo (campo TempoCarenza della movimenti_dettagli)
var elencoTempoCarenza = [];
Inizializza_elencoTempoCarenza();

// Livello Prezzo (SOLO F&F)
var elencoPrezzoLivelloFF = [];
var elencoPrezzoLivelloNoFF = [];
var elencoPrezzoLivelloSoloQta = [];
Inizializza_elenchiPrezzoLivello();


// Sconto/Maggiorazione
var elencoScontoMaggiorazione = [];
Inizializza_elencoScontoMaggiorazione();

var enum_StatoOrdine = {
    indefinito : 0,
    inevaso : 1,
    evaso : 2,
    parzialmente_evaso : 3,
    non_pronto : 4,
    evaso_forzatamente : 5
};

var elencoFinalitaMacchine = [];
Inizializza_elencoFinalitaMacchine();


function RiempiElencoScontoModalita(options) {
    options.success(elencoScontoModalita);
}

function RiempiElencoValoreRiferimento(options) {
    options.success(elencoTempoCarenza);
}

function RiempiElencoPrezzoLivelloFF(options) {
    options.success(elencoPrezzoLivelloFF);
}
function RiempiElencoPrezzoLivelloNoFF(options) {
    options.success(elencoPrezzoLivelloNoFF);
}
function RiempiElencoPrezzoLivelloSoloQta(options) {
    options.success(elencoPrezzoLivelloSoloQta);
}

function RiempiElencoScontoMaggiorazione(options) {
    options.success(elencoScontoMaggiorazione);
}

//--------------------------------------------------------------------------------
// PARAMETRI QUALITATIVI
//--------------------------------------------------------------------------------

// Tipo parametro qualitativo

var enum_TipoParamQual = {
    CodiceNumerico: 1,
    Numero: 3,
    Stringa: 4,
    Data: 5
}

// Parametri qualitativi confezionamento

var enum_IdParamQual = {
    Imballaggio: "4",
    Confezione: "5",
    Contenitore: "8"
}

// Costanti nomenclatura colonne

var pq_pref_FF = "FF_"
var pq_suff_TipoCod = "_Tipo_Cod";
var pq_suff_Sigla = "_Sigla";
var pq_suff_ValCod = "_Val_Cod";

// Costanti nomencalture controlli

var pq_pref_ctr_txt = "txt";
var pq_pref_ctr_txtStr = "txtStr";
var pq_pref_ctr_date = "date";
var pq_pref_ctr_ddl = "ddl";

function getNomeControlloParamQual(paramQual) {

    let nomeControllo = "";

    switch (paramQual.Tipo) {

        case enum_TipoParamQual.Numero:
            nomeControllo = pq_pref_ctr_txt + paramQual.Tabella_Cod_Des;
            break;

        case enum_TipoParamQual.Stringa:
            nomeControllo = pq_pref_ctr_txtStr + paramQual.Tabella_Cod_Des;
            break;

        case enum_TipoParamQual.Data:
            nomeControllo = pq_pref_ctr_date + paramQual.Tabella_Cod_Des;
            break;

        default:
            nomeControllo = pq_pref_ctr_ddl + paramQual.Tabella_Cod_Des;
            break;

    }

    return nomeControllo;

}

function getNomeColonnaParamQual(paramQual) {

    let nomeColonna = "";

    switch (paramQual.Tipo) {

        case enum_TipoParamQual.Numero:
        case enum_TipoParamQual.Stringa:
        case enum_TipoParamQual.Data:
            nomeColonna = pq_pref_FF + paramQual.Tabella_Cod_Des.toLowerCase() + pq_suff_ValCod;
            break;

        default:
            nomeColonna = pq_pref_FF + paramQual.Tabella_Cod_Des.toLowerCase() + pq_suff_TipoCod;
            break;

    }

    return nomeColonna;

}

//--------------------------------------------------------------------------------
// MODULI GENERAZIONE
//--------------------------------------------------------------------------------

var Modulo_Cantine = 1;
var Modulo_FreshFood = 2;
var Modulo_Tabacco = 3;
var Modulo_Zoo = 5;



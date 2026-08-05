var enum_TipoOperazioneDB = {
    Lettura: { value: 0, name: "Lettura", code: 0 },
    Scrittura: { value: 1, name: "Scrittura", code: 1 },
    Modifica: { value: 2, name: "Modifica", code: 2 },
    Cancellazione: { value: 3, name: "Cancellazione", code: 3 },
    Trasferimento: { value: 4, name: "Trasferimento", code: 4 },
    Copia: { value: 10, name: "Copia", code: 10 }
};

var enum_LavCod = {
    Fattura_Ricevuta: { value: 1000, name: "Fattura Ricevuta", code: 1000 },
    Fattura_Emessa: { value: 1001, name: "Fattura Emessa", code: 1001 },
    Nota_Accredito_Ricevuta: { value: 1002, name: "Nota Accredito Ricevuta", code: 1002 },
    Nota_Accredito_Emessa: { value: 1003, name: "Nota Accredito Emessa", code: 1003 },
    Corrispettivo_Vendita: { value: 1020, name: "Corrispettivo Vendita", code: 1020 },
    DDT_Ricevuto: { value: 1025, name: "DDT Ricevuto", code: 1025 },
    DDT_Emesso: { value: 1031, name: "DDT Emesso", code: 1031 },
    DDT_Contabilizzato_Emesso: { value: 1069, name: "DDT Contabilizzato Emesso", code: 1069 },
    Accettazione_da_diversi: { value: 1054, name: "DDT Ricevuto", code: 1054 }, //Accettazione +
    Distinta_Carico: { value: 1075, name: "Distinta di Carico", code: 1075 },
    Distinta_Carico_Accettazione: { value: 1076, name: "Distinta di Carico", code: 1076 }, //Accettazione +
    Auto_Ddt_Emesso: { value: 1077, name: "Auto DDT Emesso", code: 1077 },
    Auto_Ddt_Emesso_Accettazione: { value: 1078, name: "Auto DDT Emesso", code: 1078 }, //Accettazione +
    Ordine_Vendita_Emesso: { value: 2002, name: "Ordine di Vendita", code: 2002 },
    Ordine_Acquisto: { value: 2004, name: "Ordine di Acquisto", code: 2004 },
    Lavorazione: { value: 5000, name: "Lavorazione", code: 5000 },
    Ordine_Lavorazione: { value: 2005, name: "Ordine di Lavorazione", code: 2005 },
    Carico_Magazzino: { value: 1022, name: "Carico di Magazzino", code: 1022 },
    Scarico_Magazzino: { value: 1023, name: "Scarico di Magazzino", code: 1023 },
    Contratto_Affitto: { value: 2006, name: "Contratto di Affitto", code: 2006 }
};

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
    Dipendenti: 10
};

var enum_Udm = {
    chilogrammi: { value: 2, name: "kg", decimals: 6, format: "0.######" },
    numero_trappole: { value: 11, name: "n.trappole", decimals: 0, format: "n0" },
    litri: { value: 29, name: "l", decimals: 6, format: "0.######" },
    numero: { value: 38, name: "n", decimals: 0, format: "n0" },
    n_piante: { value: 92, name: "n.piante", decimals: 0, format: "n0" },
    unit_di_seme: { value: 93, name: "unita` di seme", decimals: 0, format: "n0" },
    confezioni: { value: 1003, name: "Cfz", decimals: 0, format: "n0" },
    ettari: { value: 2123, name: "ha", decimals: 6, format: "0.######" }
};

var enum_ModalitaDocContabile = {
    Nessuno: 0,
    Ricevimento_Prodotto: 1,
    Ricevimento_Ortofrutta: 2
};

var elencoScontoModalita = [
    { Sconto_Modalita: 0, Sconto_Modalita_Descr: "Sconto % su prezzo unitario" },
    { Sconto_Modalita: 1, Sconto_Modalita_Descr: "Sconto Merce" },
    { Sconto_Modalita: 3, Sconto_Modalita_Descr: "Campioni gratuiti" },
    { Sconto_Modalita: 2, Sconto_Modalita_Descr: "Campioni omaggio senza rivalsa Iva" },
    { Sconto_Modalita: 4, Sconto_Modalita_Descr: "Campioni omaggio con rivalsa Iva" }
];

// Per definire come viene salvato il prezzo (campo TempoCarenza della movimenti_dettagli)
var elencoTempoCarenza = [
    { TempoCarenza: 0, TempoCarenza_Descr: "Prezzo unitario" },
    { TempoCarenza: 1, TempoCarenza_Descr: "Imponibile totale" },
    { TempoCarenza: 2, TempoCarenza_Descr: "Importo totale" },
    { TempoCarenza: 3, TempoCarenza_Descr: "Importo unitario" }
];

// Livello Prezzo (SOLO F&F)
var elencoPrezzoLivelloFF = [
    { "Prezzo_Livello": -1, "Prezzo_Livello_Descr": "Kg Netti" },
    { "Prezzo_Livello": 4, "Prezzo_Livello_Descr": "Imballaggio" },
    { "Prezzo_Livello": 8, "Prezzo_Livello_Descr": "Contenitore" },
    { "Prezzo_Livello": 5, "Prezzo_Livello_Descr": "Confezione" }
];
var elencoPrezzoLivelloNoFF = [
    { "Prezzo_Livello": -1, "Prezzo_Livello_Descr": "Kg / Lt" },
    { "Prezzo_Livello": 0, "Prezzo_Livello_Descr": "Quantità" }
];
var elencoPrezzoLivelloSoloQta = [
    { "Prezzo_Livello": 0, "Prezzo_Livello_Descr": "Quantità" }
];

// Sconto/Maggiorazione
var elencoScontoMaggiorazione = [
    { "ScontoMaggiorazione": 0, "ScontoMaggiorazione_Descr": "Sconto" },
    { "ScontoMaggiorazione": 1, "ScontoMaggiorazione_Descr": "Maggiorazione" }
];

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

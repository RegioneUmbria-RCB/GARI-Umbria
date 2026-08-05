var jSonParsed_Kendo_PreventivoColtivazione;
var jSonParsed_Kendo_Interferenze;
var jSonParsed_Kendo_Variazioni;
var jSonParsed_Kendo_Consuntivo;
var jSonParsed_Kendo_Sportelli;

var permessiRagSoc;

var useCache = true;

function Kendo_Preventivi_leggi(options) {
    options.success(jSonParsed_Kendo_PreventivoColtivazione.kendo_rows);
}

function Kendo_Interferenze_leggi(options) {
    options.success(jSonParsed_Kendo_Interferenze.kendo_rows);
}

function Kendo_Variazioni_leggi(options) {
    options.success(jSonParsed_Kendo_Variazioni.kendo_rows);
}

function Kendo_Consuntivo_leggi(options) {
    options.success(jSonParsed_Kendo_Consuntivo.kendo_rows);
}

function popolaSelezioneSportello(options) {
    caricaListaSportelli();
    options.success(jSonParsed_Kendo_Sportelli);
}

function popolaSelezioneRegioni(options) {
    var listaRegioni = [
        { regione_cod: 0, regione_des: 'Seleziona Regione' },
        { regione_cod: 1, regione_des: 'Emilia Romagna' }
    ];

    options.success(listaRegioni);
}

function CreaGrigliaKendoEstrazioni(which, sportello) {
    switch (which) {
        case "PreventivoColtivazione":
            leggiPreventivoColtivazioni(sportello);
            GrigliaKendoEstrazioni(Kendo_Preventivi_leggi, jSonParsed_Kendo_PreventivoColtivazione, mostraColonne(which));
            break;
        case "Interferenze":
            leggiInterferenze(sportello);
            GrigliaKendoEstrazioni(Kendo_Interferenze_leggi, jSonParsed_Kendo_Interferenze, mostraColonne(which));
            break;
        case "Variazioni":
            leggiVariazioni(sportello);
            GrigliaKendoEstrazioni(Kendo_Variazioni_leggi, jSonParsed_Kendo_Variazioni, mostraColonne(which));
            break;
        case "Consuntivo":
            leggiConsuntivo(sportello);
            GrigliaKendoEstrazioni(Kendo_Consuntivo_leggi, jSonParsed_Kendo_Consuntivo, mostraColonne(which));
            break;
        default:
            return;
    }
    $('#estrazioneKendoGrid').css("display", "block");
    useCache = true;
}

function leggiPreventivoColtivazioni(filtro) {
    SelectPreventivoColtivazioni(filtro);
}

function leggiInterferenze(filtro) {
    SelectInterferenze(filtro);
}

function leggiVariazioni(filtro) {
    SelectVariazioni(filtro);
}

function leggiConsuntivo(filtro) {
    SelectConsuntivo(filtro);
}

function mostraColonne(which) {
    var elencoColonne = [];

    VerificaPermessiEstrazione();

    switch (which) {
        case "PreventivoColtivazione":
            elencoColonne = [
                { field: "Progressivo", title: "Progressivo" },
                { field: "Ditta_Sementiera", title: "Ditta Sementiera" },
                { field: "Id_Appezzamento", title: "ID Appezzamento" },
                { field: "Specie_NC", title: "Specie (nome comune)" },
                { field: "Specie_NS", title: "Specie (nome scientifico)" },
                { field: "Tipologia", title: "Tipologia" },
                { field: "Provincia", title: "Prov." },
                { field: "Comune", title: "Comune" },
                { field: "Azienda_Agricola", title: "Azienda Agricola" },
                { field: "Indirizzo_Appezzamento", title: "Indirizzo Appezzamento" },
                { field: "Latitudine", title: "Latitudine" },
                { field: "Longitudine", title: "Longitudine" },
                { field: "Superficie", title: "Superficie" }
            ];

            if (!permessiRagSoc) {
                elencoColonne.splice(8, 1);
            }
            break;
        case "Interferenze":
            elencoColonne = [
                { field: "Progressivo", title: "Progressivo" },
                { field: "Ditta_Sementiera", title: "Ditta Sementiera" },
                { field: "Ditta_Sementiera_Interferente", title: "Ditta Sementiera Interferente" },
                { field: "Id_Appezzamento", title: "ID Appezzamento" },
                { field: "Id_Appezzamento_Interferente", title: "ID Appezzamento Interferente" },
                { field: "Specie_NC", title: "Specie (nome comune)" },
                { field: "Specie_NS", title: "Specie (nome scientifico)" },
                { field: "Tipologia", title: "Tipologia" },
                { field: "Azienda_Agricola", title: "Azienda Agricola" },
                { field: "Azienda_Agricola_Interferente", title: "Azienda Agricola Interferente" },
                { field: "Indirizzo_Appezzamento", title: "Indirizzo Appezzamento" },
                { field: "Indirizzo_Appezzamento_Interferente", title: "Indirizzo Appezzamento Interferente" },
                { field: "Distanza_Legge", title: "Distanza di Legge" },
                { field: "Distanza_Effettiva", title: "Distanza Effettiva" }
            ];

            if (!permessiRagSoc) {
                elencoColonne.splice(8, 2);
            }

            break;
        case "Variazioni":
            elencoColonne = [
                { field: "Progressivo", title: "Progressivo" },
                { field: "Ditta_Sementiera", title: "Ditta Sementiera" },
                { field: "Id_Appezzamento", title: "ID Appezzamento" },
                { field: "Specie_NC", title: "Specie (nome comune)" },
                { field: "Specie_NS", title: "Specie (nome scientifico)" },
                { field: "Tipologia", title: "Tipologia" },
                { field: "Provincia", title: "Prov." },
                { field: "Comune", title: "Comune" },
                { field: "Azienda_Agricola", title: "Azienda Agricola" },
                { field: "Indirizzo_Appezzamento", title: "Indirizzo Appezzamento" },
                { field: "Latitudine", title: "Latitudine" },
                { field: "Longitudine", title: "Longitudine" },
                { field: "Superficie", title: "Superficie" },
                { field: "Motivo_Variazione", title: "Motivo della Variazione" },
                { field: "Data_Variazione", title: "Data della Variazione" }
            ];

            if (!permessiRagSoc) {
                elencoColonne.splice(8, 1);
            }

            break;
        case "Consuntivo":
            elencoColonne = [
                { field: "Progressivo", title: "Progressivo" },
                { field: "Ditta_Sementiera", title: "Ditta Sementiera" },
                { field: "Id_Appezzamento", title: "ID Appezzamento" },
                { field: "Specie_NC", title: "Specie (nome comune)" },
                { field: "Specie_NS", title: "Specie (nome scientifico)" },
                { field: "Tipologia", title: "Tipologia" },
                { field: "Provincia", title: "Prov." },
                { field: "Comune", title: "Comune" },
                { field: "Azienda_Agricola", title: "Azienda Agricola" },
                { field: "Indirizzo_Appezzamento", title: "Indirizzo Appezzamento" },
                { field: "Superficie", title: "Superficie" },
            ];

            if (!permessiRagSoc) {
                elencoColonne.splice(8, 1);
            }

            break;
    }

    return elencoColonne;
}

function GrigliaKendoEstrazioni(funzioneRead, dataSource, elencoColonne) {

    let div = "estrazioneKendoGrid"

    var funzioniCRUD = {
        funzioneRead: funzioneRead
    };
    var idModel = "Progressivo"; 
    var campiKendoModel = dataSource.kendo_model; //kendo_model
    var colonneKendoGrid = elencoColonne; //kendo_columns
    var parametriPerLettura = [];
    var parametriDataSource = {
        sort: {
            field: "Progressivo",
            dir: "desc"
        }
    };

    var parametriKendoGrid = {
        groupable: false,
        editable: false,
        resizable: true,
        columnMenu: false,
        pdf: false,
        excel: true,

        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"] },
        filterable: false 
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: null, //postDataBoundRigheParticelle,
        funzioneDaChiamarePrimaDelDataBound: null, //onDataBoundRigheParticelle,
        funzioneDaChiamareDopoSelectAllRows: null, //postSelectedRigheParticelle,
        funzioneDaChiamareDopoSave: null//onEditKendoSup_Impiegata
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(div, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}
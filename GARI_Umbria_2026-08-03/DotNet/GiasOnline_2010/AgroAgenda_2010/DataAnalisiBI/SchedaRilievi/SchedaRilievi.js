

function Esporta(waTableGetData) {
    WaitFrame.show();

    var dati = waTablePrecedenti.getData(false, true);
    var ssxml = EsportaLatoClient(dati)

    WaitFrame.hide();
    return ssxml;
}




function kendoRilievi(divKendoRilievi) {

    var funzioniCRUD = {
        funzioneRead: kReadRilievi_rows,   //kendo_rows                                     
    };
    var idModel = "kendoKey";
    var campiKendoModel = kReadRilievi_mod(); //kendo_model
    var colonneKendoGrid = kReadRilievi_col(); //kendo_columns
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        columnMenu: false,
        impostaColonneKendoGridDaCookie: false,
        excel: true,
        pdf: false,
        sortable: true,
        groupable: false,
        pageable: true,
        filterable: { mode: "menu" }
    };
    var funzioniPrimaDopoEventi = {};
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = ["kendoKey"];

    creaKendoGrid(divKendoRilievi, // rappresenta l'ID del div a cui si associa la griglia
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




////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////     GRIGLIA   //////// ///////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function ConfiguraGrigliaCastelletto(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: CaricaGrigliaCastelletto,
        //funzioneSubmit: { funzione: SubmitGrid_Castelletto, flagInsert: false, flagUpdate: true, flagDelete: false },
        UtenteAbilitatoInserimentoModifica: false,
        UtenteAbilitatoCancellazione: false 
    };
    
    var idModel = "Cod_Iva";
    
    var campiKendoModel = {
        Cod_Iva: { editable: false, type: "number" },
        Imponibile: { editable: false, type: "number" },
        Aliquota: { editable: true, type: "string", validation: { required: true } },
        Imposta: { editable: true, type: "number", validation: { required: true } }
    };

    var colonneKendoGrid = [
        { field: "Imponibile", title: TraduzioneMultiResx(resxCastellettoUC, "Imponibile", "Imponibile"), format: "{0:n2}" },
        { field: "Aliquota", title: TraduzioneMultiResx(resxCastellettoUC, "AliquotaIva", "Aliquota Iva"), filterable: { multi: true, search: true } },
        { field: "Imposta", title: TraduzioneMultiResx(resxCastellettoUC, "Imposta", "Imposta"), format: "{0:n2}" }
    ];

    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pdf: false,
        excel: false,
        editable: {
            mode: "inline"
        },
        colonneCustomKendoGrid: "",
        //salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true,
        groupable: false
    }; 


    var funzioniPrimaDopoEventi = {}
    //{ funzioneDaChiamareDopoDataBound: DataBoundGrigliaCastelletto};


    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = "";

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

function DataBoundGrigliaCastelletto(e) {
}

function SaveGrigliaCastelletto(e) {
    
}





////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////// SUBMIT SINGOLE GRIGLIE ///////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function SubmitGrid_Castelletto(options) {

   
}
